//#define FULL_SCREEN

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Drawing.Imaging;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Comm_Sec3D
{
	public partial class Game : Form
	{
		///////////////////////////  需要小心初始化的状态变量 //////////////////////////////
		Sec sec;
		WallMesh ws;
		ExtrusionMesh ms;
		PartitionLines pl;
		WallPartitionLines wpl;
		SelectionLines sl;

		//----------------------------------------------------------------------------------
		//GUI显示控制开关
		bool EnableColoring;
		bool EnableLights;
		bool DisplayWireFrame;
		bool DisplayHorizonalMesh;	//挤压Mesh显示
		bool DisplayVerticalMesh;	//Wallmesh显示
		bool DisplayInfoHUD;		//显示区域信息否

		//----------------------------------------------------------------------------------
		//挤压Mesh的球面中心点
		Vector3 center;

		//当前选中的多边形编号及其质心
		int curr_district;
		Vector3 curr_centroid;

		//world中的平移矢量，其值要么等于center，要么等于当前选择的面质心坐标
		Vector3 world_translation;

		//镜头
		Camera camera;

		//平行光光源
		DirectionalLight dir_light;

		//World等比放大系数，暂时不用
		float scaling;

		//----------------------------------------------------------------------------------
		//Direct3D控制相关
		private Device device;
		private bool device_lost;
		PresentParameters present_params;

		int batch; //高效render，一次DrawPrimitive所能够发送的最大Primitive个数/2

		//----------------------------------------------------------------------------------
		//Win32/D3D资源相关
		System.Drawing.Font font_selected; //用于选中区域显示
		System.Drawing.Font font_hud; //用于信息框中的文字显示
		Microsoft.DirectX.Direct3D.Font d3dfont_selected;
		Microsoft.DirectX.Direct3D.Font d3dfont_hud;

		Bitmap bmp_hud;			//用于动态创建背景贴图的Bitmap
		Texture bkground_hud;	//信息框背景贴图
		Sprite sprite;			//用于显示信息框的精灵

		//----------------------------------------------------------------------------------
		//显示信息相关
		string filename;
		string message;

		//----------------------------------------------------------------------------------
		//窗口状态相关
		bool onpaint_enabled;			//可否触发On_Paint事件？

		////////////////////////////////////////////////////////////////////////////////////
		//以下变量由相应的窗口事件负责跟踪，并不需要每次resetall时初始化
		bool middle_button_pressed = false;	//跟踪中键的按下情况
		bool right_button_pressed = false;	//跟踪右键的按下情况

		Size client_size;					//跟踪窗口的ClientSize
		FormWindowState window_state;		//跟踪窗口的状态
		bool window_activated = true;		//跟踪窗口的激活情况

		////////////////////////////////////////////////////////////////////////////////////
		#region 框架相关
		public Game(string fn)
		{
			InitializeComponent();
			this.MouseWheel += new MouseEventHandler(this.Game_MouseWheel);

			ResetAll(fn);
		}

		////////////////////////////////////////////////////////////////////////////////////
		void ResetAll(string fn)
		{
			////////////////////////////////////////////////////////////////////////////////////
			//所有关键变量的清空和初始化
			EnableColoring = true;
			EnableLights = false;
			DisplayWireFrame = true;
			DisplayHorizonalMesh = true;
			DisplayVerticalMesh = true;
			DisplayInfoHUD = true;

			//挤压Mesh的球面中心点
			center = new Vector3();

			//当前选中的多边形编号及其质心
			curr_district = 0;
			curr_centroid = new Vector3();

			//world中的平移矢量
			world_translation = new Vector3();	//其值要么等于center，要么等于当前选择的面质心坐标

			//World等比放大系数，暂时不用
			scaling = 1F;

			//Direct3D相关
			device = null;
			device_lost = false;
			present_params = null;
			batch = 0;

			//Direct3D资源相关
			font_selected = font_hud = null;
			d3dfont_selected = d3dfont_hud = null;
			bmp_hud = null;
			bkground_hud = null;
			sprite = null;

			//显示信息相关
			filename = message = null;

			//窗口状态相关
			onpaint_enabled = true;

			////////////////////////////////////////////////////////////////////////////////////
			//生成原始数据和所有点、面和线框
			sec = new Sec(fn);
			ms = new ExtrusionMesh(sec, Color.White, EnableColoring);
			ws = new WallMesh(sec, Color.Gainsboro);
			pl = new PartitionLines(ms.polys, Color.Red);
			wpl = new WallPartitionLines(sec, Color.Blue);

			//对sl采取的管理策略是不同的，其VertexBuffer和其自身将被同时创建或删除
			//这里无需初始化，InitializeGraphics中自然会初始化
			sl = null;

			//初始化当前选中的多边形的质心
			curr_centroid = ms.polys[curr_district].GetCentroid();

			//设置标题栏
			FileInfo fi = new FileInfo(fn);
            filename = fn; // fi.Name.ToLower();
			Text = "3D .Sec Viewer - " + filename;

			//初始化信息字符串
			GenerateMessageString();
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void CreatePresentParameters()
		{
			//初始化设备参数
			present_params = new PresentParameters();

#if FULL_SCREEN
			//全屏
			present_params.Windowed = false;
			present_params.BackBufferCount = 2;
			present_params.BackBufferWidth = 1024;
			present_params.BackBufferHeight = 768;
			present_params.BackBufferFormat = Format.X8R8G8B8;
			present_params.SwapEffect = SwapEffect.Flip;
			present_params.EnableAutoDepthStencil = true;
			present_params.AutoDepthStencilFormat = DepthFormat.D24X8;
#else
			//窗口
			present_params.Windowed = true;
			present_params.BackBufferWidth = ClientSize.Width;
			present_params.BackBufferHeight = ClientSize.Height;
			present_params.SwapEffect = SwapEffect.Flip;
			present_params.BackBufferCount = 2;
			present_params.EnableAutoDepthStencil = true;
			present_params.AutoDepthStencilFormat = DepthFormat.D24X8;
#endif
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void InitializeGraphics()
		{
			//初始化设备参数
			CreatePresentParameters();

			//稳定性的关键设置！！让MDX的自动事件触发去死！！
			Device.IsUsingEventHandlers = false;

			//初始化设备
			device = new Device(0, DeviceType.Hardware, this,
				CreateFlags.HardwareVertexProcessing, present_params);

			device.DeviceReset += new EventHandler(this.OnDeviceReset);
			device.DeviceLost += new EventHandler(this.OnDeviceLost);

			////////////////////////////////////////////////////////////////////////////
			//预先准备好Win32字体
			font_selected = new System.Drawing.Font("新宋体", 12, FontStyle.Bold);
			font_hud = new System.Drawing.Font("新宋体", 12);

			////////////////////////////////////////////////////////////////////////////
			//创建背景贴图的Win32 BMP
			bmp_hud = new Bitmap(200, 150);
			using (Graphics g = Graphics.FromImage(bmp_hud))
			using (SolidBrush brush = new SolidBrush(Color.White))
				g.FillRectangle(brush, 0, 0, 200, 150);

			////////////////////////////////////////////////////////////////////////////
			//设置device相关的所有D3D资源
			SetupDevice();

			////////////////////////////////////////////////////////////////////////////
			//计算球体
			float radius = ms.CaculateBoundSphere(out center);

			//以球面半径初始化Camera
			camera = new Camera(radius);

			////////////////////////////////////////////////////////////////////////////
			//初始化平行光源
			dir_light = new DirectionalLight();

			////////////////////////////////////////////////////////////////////////////
			Caps caps = Manager.GetDeviceCaps(0, DeviceType.Hardware);
			batch = (caps.MaxPrimitiveCount + 1) / 2;

			////////////////////////////////////////////////////////////////////////////
			world_translation = center; //初始化world平移矢量
		}

		////////////////////////////////////////////////////////////////////////////////////
		//重建除device之外的一切D3D资源
		public void SetupDevice()
		{
			//重建所有的Mesh
			if (ms.mesh == null) ms.CreateExtrusionMesh(device);
			if (ws.mesh == null) ws.CreateWallMesh(device);

			//重建所有的VertexBuffer
			if (pl.vertexbuf == null) pl.CreatePartitionLinesVertexBuffer(device);
			if (wpl.vertexbuf == null) wpl.CreateWallPartitionLinesVertexBuffer(device);

			//一体创建sl及其VertexBuffer
			if (sl == null)
			{
				sl = new SelectionLines(ms.polys[curr_district]);
				sl.CreateSelectionLines(device);
			}

			////创建显示所需的字体
			if (d3dfont_selected == null) d3dfont_selected = new Microsoft.DirectX.Direct3D.Font(device, font_selected);
			if (d3dfont_hud == null) d3dfont_hud = new Microsoft.DirectX.Direct3D.Font(device, font_hud);

			//设置信息框背景贴图
			if (bkground_hud == null) bkground_hud = new Texture(device, bmp_hud, Usage.Dynamic, Pool.Default);

			//创建显示信息框的精灵
			if (sprite == null) sprite = new Sprite(device);
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void ToDispose(IDisposable d)
		{
			if (d != null) d.Dispose();
		}

		//准备退出，销毁包括device在内的一切资源:D3D/Win32/Device
		public void CleanupGraphics()
		{
			//D3D资源			
			ToDispose(ms.mesh); ms.mesh = null;
			ToDispose(ws.mesh); ws.mesh = null;

			ToDispose(pl.vertexbuf); pl.vertexbuf = null;
			ToDispose(wpl.vertexbuf); wpl.vertexbuf = null;

			//一体Dispose sl及其VertexBuffer
			ToDispose(sl); sl = null;

			//销毁所有D3D字体
			ToDispose(d3dfont_selected); d3dfont_selected = null;
			ToDispose(d3dfont_hud); d3dfont_hud = null;

			ToDispose(sprite); sprite = null; //销毁精灵
			ToDispose(bkground_hud); bkground_hud = null; //销毁信息框背景相关的一切资源

			//Win32资源
			ToDispose(font_selected); font_selected = null;
			ToDispose(font_hud); font_hud = null;
			ToDispose(bmp_hud); bmp_hud = null;

			//Device
			ToDispose(device); device = null;
		}

		////////////////////////////////////////////////////////////////////////////////////
		protected void OnDeviceLost(object sender, EventArgs e)
		{
			//释放除device外所有D3D资源
			ToDispose(ms.mesh); ms.mesh = null;
			ToDispose(ws.mesh); ws.mesh = null;

			ToDispose(pl.vertexbuf); pl.vertexbuf = null;
			ToDispose(wpl.vertexbuf); wpl.vertexbuf = null;

			//一体Dispose sl及其VertexBuffer
			ToDispose(sl); sl = null;

			//销毁所有D3D字体
			ToDispose(d3dfont_selected); d3dfont_selected = null;
			ToDispose(d3dfont_hud); d3dfont_hud = null;

			ToDispose(sprite); sprite = null; //销毁精灵
			ToDispose(bkground_hud); bkground_hud = null; //销毁信息框背景相关的一切资源
		}

		protected void OnDeviceReset(object sender, EventArgs e)
		{
			//重建除device外所有D3D资源
			SetupDevice();
		}

		////////////////////////////////////////////////////////////////////////////////////
		protected void AttemptRecovery()
		{
			int ret;
			device.CheckCooperativeLevel(out ret);

			switch (ret)
			{
				case (int)ResultCode.DeviceLost:
					Thread.Sleep(100);
					break;
				case (int)ResultCode.DeviceNotReset:
					try
					{
						present_params.BackBufferWidth = client_size.Width;  //!!
						present_params.BackBufferHeight = client_size.Height;//!!
						device.Reset(present_params);
						device_lost = false;

						Debug.WriteLine("Device successfully reset");
					}
					catch (DeviceLostException)
					{
						Thread.Sleep(100);
					}
					catch (Exception)
					{
						//注意：一般运行到这里，代表着发生了极其严重的错误，如运行时改变桌面色深、分辨率等
						MessageBox.Show("发生了不可预料的关键错误！");
						CleanupGraphics();
						Close();
					}
					break;
			}
		}

		public void RenderScene()
		{
			if (!window_activated)
			{
				Thread.Sleep(50);
#if FULL_SCREEN
				WindowState=FormWindowState.Minimized;
#endif
			}
			if (WindowState == FormWindowState.Minimized)
			{
				Thread.Sleep(100);
				return;
			}

			if (device_lost)
				AttemptRecovery();

			if (device_lost)
				return;

			RenderFrame();
		}
		#endregion

		////////////////////////////////////////////////////////////////////////////////////
		protected void SetupLights()
		{
			if (EnableLights)
			{
				device.RenderState.Lighting = true;
				device.RenderState.FillMode = FillMode.Solid;
				device.RenderState.ShadeMode = ShadeMode.Gouraud;

				//为避免画面过暗，添加了4盏有向灯
				dir_light.SetDirectionalLight(device, 0, 1, 1);
				dir_light.SetDirectionalLight(device, 1, -1, -1);
				dir_light.SetDirectionalLight(device, 2, -1, 1);
				dir_light.SetDirectionalLight(device, 3, 1, -1);

				/*
				//一个点光源的测试配置
				device.Lights[1].Type = LightType.Point;
				device.Lights[1].Diffuse = Color.White;
				device.Lights[1].Ambient = Color.White;
				device.Lights[1].Position = new Vector3(100, 100, -200);
				device.Lights[1].Update();
				device.Lights[1].Enabled = true;
				device.Lights[1].Range = 500;
				*/

				/*
				device.Lights[0].Type = LightType.Directional;
				device.Lights[0].Diffuse = Color.LightGray;
				device.Lights[0].Position = new Vector3(-100, -100, -100);
				device.Lights[0].Update();
				device.Lights[0].Enabled = true;
				device.Lights[0].Range = 0;
				device.Lights[0].Direction = new Vector3(100, 100, 100);

				device.Lights[1].Type = LightType.Directional;
				device.Lights[1].Diffuse = Color.LightGray;
				device.Lights[1].Position = new Vector3(100, 100, -100);
				device.Lights[1].Update();
				device.Lights[1].Enabled = true;
				device.Lights[1].Range = 0;
				device.Lights[1].Direction = new Vector3(-100, -100, 100);

				device.Lights[2].Type = LightType.Directional;
				device.Lights[2].Diffuse = Color.LightGray;
				device.Lights[2].Position = new Vector3(-100, 100, -200);
				device.Lights[2].Update();
				device.Lights[2].Enabled = true;
				device.Lights[2].Range = 0;
				device.Lights[2].Direction = new Vector3(100, -100, 200);

				device.Lights[3].Type = LightType.Directional;
				device.Lights[3].Diffuse = Color.LightGray;
				device.Lights[3].Position = new Vector3(100, -100, -200);
				device.Lights[3].Update();
				device.Lights[3].Enabled = true;
				device.Lights[3].Range = 0;
				device.Lights[3].Direction = new Vector3(-100, 100, 200);
				*/
			}
			else
			{
				device.RenderState.Lighting = false;
				//device.RenderState.FillMode = FillMode.WireFrame;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////
		protected void SetupMatrices()
		{
			//world变换，注意这里的平移矢量主要用来设置world原点
			device.Transform.World = Matrix.Translation(-world_translation) * Matrix.Scaling(scaling, scaling, scaling);

			//view变换
			camera.SetViewTransform(device);

			//projection变换
			float aspect = (float)client_size.Width / (float)client_size.Height;
			device.Transform.Projection = Matrix.PerspectiveFovLH(
				(float)Math.PI / 4.0F,
				aspect,		//正确的横纵比
				40F,		//IMPORTANT!! 最重要的参数 about Hidden Lines Removal
				12000.0F);	//IMPORTANT!! 最重要的参数

			//device.Transform.Projection = Matrix.OrthoLH(2000,2000, 40F, 12000F);
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void RenderFrame()
		{
			device.BeginScene();
			if ((!DisplayHorizonalMesh && !DisplayVerticalMesh))
				device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, 0x0, 1.0f, 0);
			else
				device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, 0x353535, 1.0f, 0);

			////////////////////////////////////////////////////////////////////////////////////
			//设置变换矩阵和灯光
			SetupMatrices();
			SetupLights();

			////////////////////////////////////////////////////////////////////////////////////
			//device.RenderState.UseWBuffer = true;
			//device.RenderState.CullMode = Cull.None;
			//device.RenderState.FillMode = FillMode.WireFrame;

			////////////////////////////////////////////////////////////////////////////////////
			//两个mesh由面构成，因此顶点需要提供法线信息
			device.VertexFormat = CustomVertex.PositionNormalColored.Format;
			device.RenderState.SlopeScaleDepthBias = 1F;	//最重要的参数设置，消除线与面的Z-Fightingd
			//device.RenderState.DepthBias = 0F;			//重要参数 about Hidden Lines Removal

			int numSubSets;
			if (DisplayHorizonalMesh)
			{
				numSubSets = ms.mesh.GetAttributeTable().Length;
				for (int i = 0; i < numSubSets; i++)
					ms.mesh.DrawSubset(i);
			}
			if (DisplayVerticalMesh && ws.mesh != null)
			{
				numSubSets = ws.mesh.GetAttributeTable().Length;
				for (int i = 0; i < numSubSets; i++)
					ws.mesh.DrawSubset(i);
			}

			////////////////////////////////////////////////////////////////////////////////////
			//线框是目前存在性能问题最大的部分之一
			if (DisplayWireFrame)
			{
				//区域网格由直线构成，因此顶点不需要法线信息
				device.VertexFormat = CustomVertex.PositionColored.Format;
				device.RenderState.Lighting = false;	//区域网格无需灯光效果

				DrawBatchLinelist(pl.vertexbuf, pl.NumberOfLines);

				if (wpl.vertexbuf != null)
					DrawBatchLinelist(wpl.vertexbuf, wpl.NumberOfLines);
			}

			////////////////////////////////////////////////////////////////////////////////////
			//在当前选中的多边形质心处显示其编号
			Point p = CaculateScreenXYofWorldPoint(curr_centroid);
			string s = curr_district.ToString();
			Rectangle r = d3dfont_selected.MeasureString(null, s, DrawTextFormat.Center, Color.White); //居中
			p.X -= r.Width / 2;
			p.Y -= r.Height / 2;
			if (!EnableLights && !EnableColoring && DisplayHorizonalMesh)
				d3dfont_selected.DrawText(null, curr_district.ToString(), p, Color.Black);
			else if (sec.districts[curr_district].attributes[0] == 1 && EnableColoring)
				d3dfont_selected.DrawText(null, curr_district.ToString(), p, Color.Black);
			else
				d3dfont_selected.DrawText(null, curr_district.ToString(), p, Color.White);

			////////////////////////////////////////////////////////////////////////////////////
			//绘制信息框及其其中的文字，关键是:写在这里会不会打断CPU和AGP的pipeline?
			if (DisplayInfoHUD)
			{
				Vector3 center = new Vector3(); //是struct，没有关系
				Vector3 pos = new Vector3();

				//绘制半透明的信息框
				sprite.Begin(SpriteFlags.AlphaBlend);
				sprite.Draw(bkground_hud, Rectangle.Empty, center, pos, Color.FromArgb(80, 0, 0, 0));
				sprite.End();

				//绘制信息字符串
				d3dfont_hud.DrawText(null, message, 10, 7, Color.White);
			}

			////////////////////////////////////////////////////////////////////////////////////
			//显示多边形选择框(高亮度青色)
			device.RenderState.Lighting = false;
			device.VertexFormat = CustomVertex.PositionColored.Format;
			device.RenderState.AntiAliasedLineEnable = true; //BUG? 会影响后面的Render
			DrawBatchLinelist(sl.vertexbuf, sl.NumberOfLines);
			device.RenderState.AntiAliasedLineEnable = false; //MDX BUG? 无法取消前面的true，WHY?

			////////////////////////////////////////////////////////////////////////////////////
			device.EndScene();

			try
			{
				device.Present();
			}
			catch (DeviceLostException)
			{
				device_lost = true;
				Debug.WriteLine("Device was lost");
			}
		}

		////////////////////////////////////////////////////////////////////////////////////
		//流水线Render，尽最大可能降低CPU负担
		void DrawBatchLinelist(VertexBuffer vbuf, int count_lines)
		{
			device.SetStreamSource(0, vbuf, 0);

			int n = count_lines / batch;
			int m = count_lines % batch;
			for (int i = 0; i < n; i++)
				device.DrawPrimitives(PrimitiveType.LineList, 2 * batch * i, batch);
			if (m != 0)
				device.DrawPrimitives(PrimitiveType.LineList, n * batch * 2, m);
		}

		////////////////////////////////////////////////////////////////////////////////////
		//生成信息框中的显示字符串
		void GenerateMessageString()
		{
			District d = sec.districts[curr_district];
            message = string.Format("District Index(区块编号)           :{0}\n" +
                                    "Borders Number(边线数量)           :{1}\n" +
                                    "Terrain Category(地形属性)         :0x{2:X}\n" +
                                    "Terrain Sub-category(地形属性子类) :0x{3:X}\n" +
                                    "Is Enterable?(能否通行)            :0x{4:X}\n" +
                                    "Illumination(明暗程度)             :0x{5:X}\n" +
                                    "Is the Border?(是否边缘)           :0x{6:X}",
                this.curr_district,
				d.borders.Length,
				d.attributes[0],
				d.attributes[1],
				d.attributes[4],
				d.attributes[5],
				d.attributes[6]
			);

			string tmp = string.Format("<{0}>\n\n{1}", filename, message);
			message = tmp;
		}

		////////////////////////////////////////////////////////////////////////////////////
		//将质心的World坐标转换成屏幕坐标
		Point CaculateScreenXYofWorldPoint(Vector3 v)
		{
			Matrix trans = device.Transform.World * device.Transform.View * device.Transform.Projection; //常数提取
			Vector3 t = Vector3.TransformCoordinate(v, trans);

			t.X = (t.X + 1) * client_size.Width / 2;
			t.Y = (-t.Y + 1) * client_size.Height / 2;

			return new Point((int)t.X, (int)t.Y);
		}

		////////////////////////////////////////////////////////////////////////////////////
		void Game_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.C:
					EnableColoring = !EnableColoring;
					EnableLights = false;

					ms.mesh.Dispose();
					ms = new ExtrusionMesh(sec, Color.White, EnableColoring);
					ms.CreateExtrusionMesh(device);
					break;
				case Keys.Escape:
				case Keys.Q:
					Close(); break;
				case Keys.K: //距离
					//地图越大，将距离调最远时，越容易出现被culling的现象，估计跟projection有关
					camera.IncreaseRadius(100F);
					//if (scaling + 0.1F <= 5) scaling += 0.1f;
					break;
				case Keys.J: //距离
					camera.DecreaseRadius(100F);
					//if (scaling - 0.1F >= 0.1) scaling -= 0.1f;
					break;
				case Keys.Left: //视角
					camera.DecreaseLongitude((float)(0.02 * Math.PI));
					break;
				case Keys.Right: //视角
					camera.IncreaseLongitude((float)(0.02 * Math.PI));
					break;
				case Keys.Up: //视角
					camera.IncreaseLatitude((float)(0.02F * Math.PI));
					break;
				case Keys.Down: //视角
					camera.DecreaseLatitude((float)(0.02F * Math.PI));
					break;
				case Keys.Z: //光线方向
					dir_light.AdjustLongitude((float)(-0.02 * Math.PI));
					break;
				case Keys.X: //光线方向
					dir_light.AdjustLongitude((float)(0.02 * Math.PI));
					break;
				case Keys.A: //光线方向				
					dir_light.AdjustLatitude((float)(0.02 * Math.PI));
					break;
				case Keys.S: //光线方向
					dir_light.AdjustLatitude((float)(-0.02 * Math.PI));
					break;
				case Keys.F: //区域线框
					DisplayWireFrame = !DisplayWireFrame;
					break;
				case Keys.L: //光照开关
					EnableLights = !EnableLights;
					break;
				case Keys.H: //水平面开关
					DisplayHorizonalMesh = !DisplayHorizonalMesh;
					break;
				case Keys.V: //垂直面开关
					DisplayVerticalMesh = !DisplayVerticalMesh;
					break;
				case Keys.I:
					DisplayInfoHUD = !DisplayInfoHUD;
					break;
				case Keys.R: //复位world平移矢量和camera半径
					world_translation = center;
					camera.ResetRadius();
					break;
#if (!FULL_SCREEN)
				case Keys.O:
					string fn = Program.SelectSecFile();
					if (fn != null)
					{
						//从头创建所有的一切！
						CleanupGraphics();
						GC.Collect();
						ResetAll(fn);
						InitializeGraphics();
					}
					break;
#endif
				case Keys.OemPeriod:
					curr_district = (curr_district + 1) % ms.polys.Length;
					sl.Dispose();
					sl = new SelectionLines(ms.polys[curr_district]);
					sl.CreateSelectionLines(device);

					curr_centroid = ms.polys[curr_district].GetCentroid();
					GenerateMessageString();
					break;
				case Keys.Oemcomma:
					curr_district = (curr_district + ms.polys.Length - 1) % ms.polys.Length;
					sl.Dispose();
					sl = new SelectionLines(ms.polys[curr_district]);
					sl.CreateSelectionLines(device);

					curr_centroid = ms.polys[curr_district].GetCentroid();
					GenerateMessageString();
					break;
				default:
					Debug.WriteLine(e.KeyCode);
					break;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////
		#region 处理鼠标控制事件
		private void Game_MouseWheel(Object sender, MouseEventArgs e)
		{
			float d;
			if (middle_button_pressed)
			{
				d = 150F * (Math.Abs(e.Delta) / 120);
				if (e.Delta > 0)
					camera.IncreaseRadius(d);
				else
					camera.DecreaseRadius(d);
			}
			else if (right_button_pressed)
			{
				d = (float)(0.04F * (Math.Abs(e.Delta) / 120F) * Math.PI);
				if (e.Delta > 0)
					camera.IncreaseLatitude(d);
				else
					camera.DecreaseLatitude(d);
			}
			else
			{
				d = (float)(0.06F * (Math.Abs(e.Delta) / 120F) * Math.PI);
				if (e.Delta > 0)
					camera.DecreaseLongitude(d);
				else
					camera.IncreaseLongitude(d);
			}
		}

		private void Game_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left) return;

			if (Picking(e.X, e.Y))
			{
				//如果pick是有效的，则计算world中被选取多边形的质心
				curr_centroid = ms.polys[curr_district].GetCentroid();

				//生产信息字符串
				GenerateMessageString();
			}
			//调试：测试多边形面积和质心计算结果的符号是否正确
			//Matrix trans = device.Transform.World * device.Transform.View * device.Transform.Projection; //常数提取
			//Vector3 t = Vector3.TransformCoordinate(cp, trans);
			//Debug.WriteLine(t);			
		}

		private void Game_DoubleClick(object sender, EventArgs e)
		{
			MouseEventArgs me = (MouseEventArgs)e;
			if (me.Button != MouseButtons.Left) return;

			//双击左键时，设置world平移矢量为当前选择面的质心位置
			world_translation = curr_centroid;
		}

		private void Game_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle) middle_button_pressed = true;
			if (e.Button == MouseButtons.Right) right_button_pressed = true;
		}

		private void Game_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle) middle_button_pressed = false;
			if (e.Button == MouseButtons.Right) right_button_pressed = false;
		}
		#endregion

		////////////////////////////////////////////////////////////////////////////////////
		#region Picking System相关
		bool Picking(int sx, int sy)
		{
			float P11 = device.Transform.Projection.M11;
			float P22 = device.Transform.Projection.M22;

			//screen到projection平面，相当于首先抵消projection变换
			float px = ((2F * sx) / client_size.Width - 1F) / P11;
			float py = ((-2F * sy) / client_size.Height + 1F) / P22;
			float pz = 1F;

			Vector3 ray_pos = new Vector3(0F, 0F, 0F);
			Vector3 ray_dir = new Vector3(px, py, pz);

			//抵消world和view变换
			Matrix invert = Matrix.Invert(device.Transform.World * device.Transform.View);

			//最后计算在world中的射线
			ray_pos.TransformCoordinate(invert);
			ray_dir.TransformNormal(invert);
			ray_dir.Normalize();

			//计算ray与ms的相交多边形
			int polyid = ms.MatchPicking(device, ray_pos, ray_dir);

			if (polyid == -1)
				return false;
			else
			{
				curr_district = polyid;
				sl.Dispose();
				sl = new SelectionLines(ms.polys[curr_district]);
				sl.CreateSelectionLines(device);
			}
			return true;
		}
		#endregion

		////////////////////////////////////////////////////////////////////////////////////
		#region 记录窗口当前是否被激活
		private void Game_Activated(object sender, EventArgs e)
		{
			window_activated = true;
		}
		private void Game_Deactivate(object sender, EventArgs e)
		{
			window_activated = false;
		}
		#endregion

		////////////////////////////////////////////////////////////////////////////////////	
		#region 处理第一次显示、Resize、最小化、最大化事件
		//窗口第一次显示的时候触发
		private void Game_Shown(object sender, EventArgs e)
		{
#if FULL_SCREEN			
			client_size = new Size(1024, 768); //全屏模式下，size永远等于1024*768
			window_state = FormWindowState.Maximized;
			WindowState = FormWindowState.Maximized;
			FormBorderStyle = FormBorderStyle.None;
#else
			client_size = ClientSize;
			window_state = FormWindowState.Normal;
			WindowState = FormWindowState.Normal;
			FormBorderStyle = FormBorderStyle.Sizable;
#endif
			onpaint_enabled = true;
		}

		//拉动窗口，改变其大小时触发，只修改size
		//全屏的时候该事件将永远不会被触发，但还是为了保险起见...
		private void Game_ResizeEnd(object sender, EventArgs e)
		{
#if FULL_SCREEN
			client_size = new Size(1024, 768); //全屏模式下，size永远等于1024*768
#else
			if (ClientSize != client_size) //一旦size发生了变化
			{
				CreatePresentParameters();
				try
				{
					device.Reset(present_params);
				}
				catch (DeviceLostException)
				{
					device_lost = true;
					Debug.WriteLine("Device was lost during ResizeEnd");
				}
				client_size = ClientSize;
			}
			onpaint_enabled = true; //拉伸窗口完毕，可以触发OnPaint事件了
#endif
		}

		//窗口从正常到最大、从最大到正常时触发，修改size和state
		//全屏时，该事件的有效主体Reset将永远不会执行
		private void Game_Resize(object sender, EventArgs e)
		{
#if FULL_SCREEN
			client_size = new Size(1024, 768); //全屏模式下，size永远等于1024*768
			if (WindowState == FormWindowState.Minimized) //要么最大、要么最小
				window_state = FormWindowState.Minimized;
			else
				window_state = FormWindowState.Maximized;
#else
			//最大化、最小化、或者恢复的情况
			if (ClientSize != client_size && WindowState != window_state)
			{
				//非<最小化或者是从最小化恢复>，即<最大化或者是从最大化恢复>
				if (WindowState != FormWindowState.Minimized && window_state != FormWindowState.Minimized)
				{
					CreatePresentParameters();
					try
					{
						device.Reset(present_params);
					}
					catch (DeviceLostException)
					{
						device_lost = true;
						Debug.WriteLine("Device was lost during Resize");
					}
				}
				client_size = ClientSize;
				window_state = WindowState;
			}
			else
			{
				//拉动窗口边框的情况
				onpaint_enabled = false; //不希望在此时触发OnPaint事件
			}
#endif
		}
		#endregion

		////////////////////////////////////////////////////////////////////////////////////	
		//只所以需要On_Paint事件，是为了应付模态对话框弹出时，画面不刷新的情况
		void Game_Paint(object sender, PaintEventArgs e)
		{
			if (onpaint_enabled)
			{
				Debug.Write(".");
				RenderScene();
				if (device_lost) Invalidate(); //On_Paint直到设备不再是Lost为止
			}
		}
	}
	////////////////////////////////////////////////////////////////////////////////////	
	/*
	class PickingSystem
	{
		//挤压Mesh的球面中心点
		Vector3 center;

		//当前选中的多边形编号及其质心
		int curr_district;
		Vector3 curr_centroid;

		//world中的平移矢量，其值要么等于center，要么等于当前选择的面质心坐标
		Vector3 world_translation;

		//多边形数组
		Polygon[] polys;

		////////////////////////////////////////////////////////////////////////////////////	
		public PickingSystem(Vector3 center, Polygon[] polys)
		{
			this.polys = polys;

			curr_district = 0;
			curr_centroid = polys[0].GetCentroid();

			this.center = center;
			world_translation = this.center;
		}

		////////////////////////////////////////////////////////////////////////////////////	
		public bool Picking(int sx, int sy)
		{
			float P11 = device.Transform.Projection.M11;
			float P22 = device.Transform.Projection.M22;

			//screen到projection平面，相当于首先抵消projection变换
			float px = ((2F * sx) / client_size.Width - 1F) / P11;
			float py = ((-2F * sy) / client_size.Height + 1F) / P22;
			float pz = 1F;

			Vector3 ray_pos = new Vector3(0F, 0F, 0F);
			Vector3 ray_dir = new Vector3(px, py, pz);

			//抵消world和view变换
			Matrix invert = Matrix.Invert(device.Transform.World * device.Transform.View);

			//最后计算在world中的射线
			ray_pos.TransformCoordinate(invert);
			ray_dir.TransformNormal(invert);
			ray_dir.Normalize();

			//计算ray与ms的相交多边形
			int polyid = ms.MatchPicking(device, ray_pos, ray_dir);

			if (polyid == -1)
				return false;
			else
			{
				curr_district = polyid;
				sl.Dispose();
				sl = new SelectionLines(ms.polys[curr_district]);
				sl.CreateSelectionLines(device);
			}
			return true;
		}

	}
	*/
	////////////////////////////////////////////////////////////////////////////////////	
	class DirectionalLight
	{
		//平行光光源在球面系中的方位角
		float angle_beta;
		float angle_alpha;

		public DirectionalLight()
		{
			angle_beta = 0;

			//与diffuse=gray匹配的设定值
			//angle_alpha = (float)(Math.PI * (1f / 4f - 7f / 50f));

			//与diffuse=lightgray匹配的设定值
			angle_alpha = 0.32f;
		}

		public void AdjustLatitude(float d) //调整纬度 0-pi/2
		{
			if (d > 0)
				if (angle_alpha + d <= Math.PI / 2)
					angle_alpha += d;
				else
					angle_alpha = (float)(Math.PI / 2 - 0.001F);
			else if (d < 0)
				if (angle_alpha + d >= 0)
					angle_alpha += d;
				else
					angle_alpha = 0.001F;

			Debug.WriteLine(angle_alpha);
		}

		public void AdjustLongitude(float d) //调整经度 0-2*pi
		{
			angle_beta = (float)((angle_beta + d) % (2 * Math.PI));
		}

		public void SetDirectionalLight(Device device, int lightidx, int signx, int signy)
		{
			device.Lights[lightidx].Type = LightType.Directional;
			device.Lights[lightidx].Ambient = Color.Gray; //环境光
			device.Lights[lightidx].Diffuse = Color.LightGray; //漫散射
			device.Lights[lightidx].Update();
			device.Lights[lightidx].Enabled = true;

			//光线方向也是球面坐标系
			double t = Math.Cos(angle_alpha);
			float x = signx * (float)(100 * t * Math.Cos(angle_beta));
			float y = signy * (float)(100 * t * Math.Sin(angle_beta));
			float z = (float)(100 * Math.Sin(angle_alpha));
			device.Lights[lightidx].Direction = new Vector3(x, y, z);
		}
	}

	////////////////////////////////////////////////////////////////////////////////////	
	class Camera
	{
		//挤压Mesh的球面的原始半径
		float radius;

		////////////////////////////////////////////////////////////////////////////////////////
		//球面系中的Camera半径和方位角
		float R;
		float alpha;
		float beta;

		////////////////////////////////////////////////////////////////////////////////////////
		public Camera(float radius)
		{
			this.radius = radius;

			R = radius;
			alpha = (float)(Math.PI * 3 / 4);	//纬度：XY平面与Z夹角（右手） [-pi/2,+pi/2]映射到[0:+pi]
			beta = (float)(-Math.PI / 2);		//经度：X与OP投影的夹角（右手） 0:2pi
		}

		////////////////////////////////////////////////////////////////////////////////////////
		public void ResetRadius()
		{
			R = radius;
		}

		////////////////////////////////////////////////////////////////////////////////////////
		public void SetViewTransform(Device device)
		{
			//将球面坐标转换成world直角坐标
			double t = R * Math.Cos(alpha - Math.PI / 2);
			float z = (float)(-R * Math.Sin(alpha - Math.PI / 2)); //转化成左手
			float x = (float)(t * Math.Cos(beta));
			float y = (float)(t * Math.Sin(beta));

			device.Transform.View = Matrix.LookAtLH(		//view变换
				new Vector3((float)x, (float)y, (float)z),	//camera所在的world位置
				new Vector3(0, 0, 0),						//camera正对world原点
				new Vector3(0, 0, -1));						//camera以-Z为正上方
		}

		////////////////////////////////////////////////////////////////////////////////////////
		//半径 Radius
		public void IncreaseRadius(float d)
		{
			if (R + d <= 3 * radius) R += d; else R = 3 * radius;
		}
		public void DecreaseRadius(float d)
		{
			if (R - d >= 0.05 * radius) R -= d; else R = 0.05F * radius;
		}

		////////////////////////////////////////////////////////////////////////////////////////
		//纬度 Latitude
		public void IncreaseLatitude(float d)
		{
			if (alpha + d <= Math.PI) alpha += d; else alpha = (float)Math.PI - 0.001F;
		}
		public void DecreaseLatitude(float d)
		{
			if (alpha - d >= 0) alpha -= d; else alpha = 0.001F;
		}

		////////////////////////////////////////////////////////////////////////////////////////
		//经度 Longitude
		public void IncreaseLongitude(float d)
		{
			beta = (float)((beta + d) % (2 * Math.PI));
		}
		public void DecreaseLongitude(float d)
		{
			beta = (float)((beta - d) % (2 * Math.PI));
		}
	}
}