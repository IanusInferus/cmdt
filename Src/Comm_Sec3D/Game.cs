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

using SlimDX;
using SlimDX.Direct3D9;

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
		private Direct3D d3d;
		private Device device;
		private bool device_lost;
		PresentParameters present_params;

		int batch; //高效render，一次DrawPrimitive所能够发送的最大Primitive个数/2

		//----------------------------------------------------------------------------------
		//Win32/D3D资源相关
		System.Drawing.Font font_selected; //用于选中区域显示
		System.Drawing.Font font_hud; //用于信息框中的文字显示
		SlimDX.Direct3D9.Font d3dfont_selected;
		SlimDX.Direct3D9.Font d3dfont_hud;

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
			world_translation = new Vector3();

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

			sl = null;

			curr_centroid = ms.polys[curr_district].GetCentroid();

			FileInfo fi = new FileInfo(fn);
            filename = fn;
			Text = "3D .Sec Viewer - " + filename;

			GenerateMessageString();
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void CreatePresentParameters()
		{
			present_params = new PresentParameters();

#if FULL_SCREEN
			present_params.Windowed = false;
			present_params.BackBufferCount = 2;
			present_params.BackBufferWidth = 1024;
			present_params.BackBufferHeight = 768;
			present_params.BackBufferFormat = Format.X8R8G8B8;
			present_params.SwapEffect = SwapEffect.Flip;
			present_params.EnableAutoDepthStencil = true;
			present_params.AutoDepthStencilFormat = Format.D24X8;
#else
			present_params.Windowed = true;
			present_params.BackBufferWidth = ClientSize.Width;
			present_params.BackBufferHeight = ClientSize.Height;
			present_params.SwapEffect = SwapEffect.Flip;
			present_params.BackBufferCount = 2;
			present_params.EnableAutoDepthStencil = true;
			present_params.AutoDepthStencilFormat = Format.D24X8;
#endif
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void InitializeGraphics()
		{
			CreatePresentParameters();

			d3d = new Direct3D();

			device = new Device(d3d, 0, DeviceType.Hardware, this.Handle,
				CreateFlags.HardwareVertexProcessing, present_params);

			font_selected = new System.Drawing.Font("新宋体", 12, FontStyle.Bold);
			font_hud = new System.Drawing.Font("新宋体", 12);

			bmp_hud = new Bitmap(200, 150);
			using (Graphics g = Graphics.FromImage(bmp_hud))
			using (SolidBrush brush = new SolidBrush(Color.White))
				g.FillRectangle(brush, 0, 0, 200, 150);

			SetupDevice();

			float radius = ms.CaculateBoundSphere(out center);

			camera = new Camera(radius);

			dir_light = new DirectionalLight();

			Capabilities caps = d3d.GetDeviceCaps(0, DeviceType.Hardware);
			batch = (caps.MaxPrimitiveCount + 1) / 2;

			world_translation = center;
		}

		////////////////////////////////////////////////////////////////////////////////////
		//重建除device之外的一切D3D资源
		public void SetupDevice()
		{
			if (ms.mesh == null) ms.CreateExtrusionMesh(device);
			if (ws.mesh == null) ws.CreateWallMesh(device);

			if (pl.vertexbuf == null) pl.CreatePartitionLinesVertexBuffer(device);
			if (wpl.vertexbuf == null) wpl.CreateWallPartitionLinesVertexBuffer(device);

			if (sl == null)
			{
				sl = new SelectionLines(ms.polys[curr_district]);
				sl.CreateSelectionLines(device);
			}

			if (d3dfont_selected == null) d3dfont_selected = new SlimDX.Direct3D9.Font(device, font_selected);
			if (d3dfont_hud == null) d3dfont_hud = new SlimDX.Direct3D9.Font(device, font_hud);

			if (bkground_hud == null) bkground_hud = TextureFromBitmap(device, bmp_hud);

			if (sprite == null) sprite = new Sprite(device);
		}

		static Texture TextureFromBitmap(Device device, Bitmap bmp)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				bmp.Save(ms, ImageFormat.Png);
				ms.Position = 0;
				return Texture.FromStream(device, ms, Usage.Dynamic, Pool.Default);
			}
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void ToDispose(IDisposable d)
		{
			if (d != null) d.Dispose();
		}

		//准备退出，销毁包括device在内的一切资源:D3D/Win32/Device
		public void CleanupGraphics()
		{
			ToDispose(ms.mesh); ms.mesh = null;
			ToDispose(ws.mesh); ws.mesh = null;

			ToDispose(pl.vertexbuf); pl.vertexbuf = null;
			ToDispose(wpl.vertexbuf); wpl.vertexbuf = null;

			ToDispose(sl); sl = null;

			ToDispose(d3dfont_selected); d3dfont_selected = null;
			ToDispose(d3dfont_hud); d3dfont_hud = null;

			ToDispose(sprite); sprite = null;
			ToDispose(bkground_hud); bkground_hud = null;

			ToDispose(font_selected); font_selected = null;
			ToDispose(font_hud); font_hud = null;
			ToDispose(bmp_hud); bmp_hud = null;

			ToDispose(device); device = null;
			ToDispose(d3d); d3d = null;
		}

		////////////////////////////////////////////////////////////////////////////////////
		protected void OnDeviceLost()
		{
			ToDispose(ms.mesh); ms.mesh = null;
			ToDispose(ws.mesh); ws.mesh = null;

			ToDispose(pl.vertexbuf); pl.vertexbuf = null;
			ToDispose(wpl.vertexbuf); wpl.vertexbuf = null;

			ToDispose(sl); sl = null;

			ToDispose(d3dfont_selected); d3dfont_selected = null;
			ToDispose(d3dfont_hud); d3dfont_hud = null;

			ToDispose(sprite); sprite = null;
			ToDispose(bkground_hud); bkground_hud = null;
		}

		protected void OnDeviceReset()
		{
			SetupDevice();
		}

		////////////////////////////////////////////////////////////////////////////////////
		protected void AttemptRecovery()
		{
			Result r = device.TestCooperativeLevel();

			if (r.Code == ResultCode.DeviceLost.Code)
			{
				Thread.Sleep(100);
				return;
			}
			if (r.Code == ResultCode.DeviceNotReset.Code)
			{
				try
				{
					present_params.BackBufferWidth = client_size.Width;
					present_params.BackBufferHeight = client_size.Height;
					OnDeviceLost();
					device.Reset(new[] { present_params });
					OnDeviceReset();
					device_lost = false;

					Debug.WriteLine("Device successfully reset");
				}
				catch (Direct3D9Exception)
				{
					Thread.Sleep(100);
				}
				catch (Exception)
				{
					MessageBox.Show("发生了不可预料的关键错误！");
					CleanupGraphics();
					Close();
				}
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
				device.SetRenderState(RenderState.Lighting, true);
				device.SetRenderState(RenderState.FillMode, FillMode.Solid);
				device.SetRenderState(RenderState.ShadeMode, ShadeMode.Gouraud);

				dir_light.SetDirectionalLight(device, 0, 1, 1);
				dir_light.SetDirectionalLight(device, 1, -1, -1);
				dir_light.SetDirectionalLight(device, 2, -1, 1);
				dir_light.SetDirectionalLight(device, 3, 1, -1);
			}
			else
			{
				device.SetRenderState(RenderState.Lighting, false);
			}
		}

		////////////////////////////////////////////////////////////////////////////////////
		protected void SetupMatrices()
		{
			device.SetTransform(TransformState.World, Matrix.Translation(-world_translation) * Matrix.Scaling(scaling, scaling, scaling));

			camera.SetViewTransform(device);

			float aspect = (float)client_size.Width / (float)client_size.Height;
			device.SetTransform(TransformState.Projection, Matrix.PerspectiveFovLH(
				(float)Math.PI / 4.0F,
				aspect,		//正确的横纵比
				40F,
				12000.0F));
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void RenderFrame()
		{
			device.BeginScene();
			if ((!DisplayHorizonalMesh && !DisplayVerticalMesh))
				device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, 0x0, 1.0f, 0);
			else
				device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, 0x353535, 1.0f, 0);

			SetupMatrices();
			SetupLights();

			device.VertexFormat = CustomVertex.PositionNormalColored.Format;
			device.SetRenderState(RenderState.SlopeScaleDepthBias, 1F);

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

			if (DisplayWireFrame)
			{
				device.VertexFormat = CustomVertex.PositionColored.Format;
				device.SetRenderState(RenderState.Lighting, false);

				DrawBatchLinelist(pl.vertexbuf, pl.NumberOfLines);

				if (wpl.vertexbuf != null)
					DrawBatchLinelist(wpl.vertexbuf, wpl.NumberOfLines);
			}

			//在当前选中的多边形质心处显示其编号
			Point p = CaculateScreenXYofWorldPoint(curr_centroid);
			string s = curr_district.ToString();
			Rectangle r = d3dfont_selected.MeasureString(null, s, DrawTextFormat.Center);
			p.X -= r.Width / 2;
			p.Y -= r.Height / 2;
			if (!EnableLights && !EnableColoring && DisplayHorizonalMesh)
				d3dfont_selected.DrawString(null, curr_district.ToString(), p.X, p.Y, Color.Black);
			else if (sec.districts[curr_district].attributes[0] == 1 && EnableColoring)
				d3dfont_selected.DrawString(null, curr_district.ToString(), p.X, p.Y, Color.Black);
			else
				d3dfont_selected.DrawString(null, curr_district.ToString(), p.X, p.Y, Color.White);

			//绘制信息框及其其中的文字
			if (DisplayInfoHUD)
			{
				sprite.Begin(SpriteFlags.AlphaBlend);
				sprite.Draw(bkground_hud, Color.FromArgb(80, 0, 0, 0));
				sprite.End();

				d3dfont_hud.DrawString(null, message, 10, 7, Color.White);
			}

			//显示多边形选择框(高亮度青色)
			device.SetRenderState(RenderState.Lighting, false);
			device.VertexFormat = CustomVertex.PositionColored.Format;
			device.SetRenderState(RenderState.AntialiasedLineEnable, true);
			DrawBatchLinelist(sl.vertexbuf, sl.NumberOfLines);
			device.SetRenderState(RenderState.AntialiasedLineEnable, false);

			device.EndScene();

			Result presentResult = device.Present();
			if (presentResult.Code == ResultCode.DeviceLost.Code)
			{
				device_lost = true;
				Debug.WriteLine("Device was lost");
			}
		}

		////////////////////////////////////////////////////////////////////////////////////
		//流水线Render，尽最大可能降低CPU负担
		void DrawBatchLinelist(VertexBuffer vbuf, int count_lines)
		{
			device.SetStreamSource(0, vbuf, 0, CustomVertex.PositionColored.SizeBytes);

			int n = count_lines / batch;
			int m = count_lines % batch;
			for (int i = 0; i < n; i++)
				device.DrawPrimitives(PrimitiveType.LineList, 2 * batch * i, batch);
			if (m != 0)
				device.DrawPrimitives(PrimitiveType.LineList, n * batch * 2, m);
		}

		////////////////////////////////////////////////////////////////////////////////////
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
		Point CaculateScreenXYofWorldPoint(Vector3 v)
		{
			Matrix world = device.GetTransform(TransformState.World);
			Matrix view = device.GetTransform(TransformState.View);
			Matrix proj = device.GetTransform(TransformState.Projection);
			Matrix trans = world * view * proj;
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
					camera.IncreaseRadius(100F);
					break;
				case Keys.J: //距离
					camera.DecreaseRadius(100F);
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
				curr_centroid = ms.polys[curr_district].GetCentroid();
				GenerateMessageString();
			}
		}

		private void Game_DoubleClick(object sender, EventArgs e)
		{
			MouseEventArgs me = (MouseEventArgs)e;
			if (me.Button != MouseButtons.Left) return;

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
			Matrix proj = device.GetTransform(TransformState.Projection);
			float P11 = proj.M11;
			float P22 = proj.M22;

			float px = ((2F * sx) / client_size.Width - 1F) / P11;
			float py = ((-2F * sy) / client_size.Height + 1F) / P22;
			float pz = 1F;

			Vector3 ray_pos = new Vector3(0F, 0F, 0F);
			Vector3 ray_dir = new Vector3(px, py, pz);

			Matrix world = device.GetTransform(TransformState.World);
			Matrix view = device.GetTransform(TransformState.View);
			Matrix invert = Matrix.Invert(world * view);

			ray_pos = Vector3.TransformCoordinate(ray_pos, invert);
			ray_dir = Vector3.TransformNormal(ray_dir, invert);
			ray_dir.Normalize();

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
		private void Game_Shown(object sender, EventArgs e)
		{
#if FULL_SCREEN
			client_size = new Size(1024, 768);
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

		private void Game_ResizeEnd(object sender, EventArgs e)
		{
#if FULL_SCREEN
			client_size = new Size(1024, 768);
#else
			if (ClientSize != client_size)
			{
				CreatePresentParameters();
				try
				{
					OnDeviceLost();
					device.Reset(new[] { present_params });
					OnDeviceReset();
				}
				catch (Direct3D9Exception)
				{
					device_lost = true;
					Debug.WriteLine("Device was lost during ResizeEnd");
				}
				client_size = ClientSize;
			}
			onpaint_enabled = true;
#endif
		}

		private void Game_Resize(object sender, EventArgs e)
		{
#if FULL_SCREEN
			client_size = new Size(1024, 768);
			if (WindowState == FormWindowState.Minimized)
				window_state = FormWindowState.Minimized;
			else
				window_state = FormWindowState.Maximized;
#else
			if (ClientSize != client_size && WindowState != window_state)
			{
				if (WindowState != FormWindowState.Minimized && window_state != FormWindowState.Minimized)
				{
					CreatePresentParameters();
					try
					{
						OnDeviceLost();
						device.Reset(new[] { present_params });
						OnDeviceReset();
					}
					catch (Direct3D9Exception)
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
				onpaint_enabled = false;
			}
#endif
		}
		#endregion

		////////////////////////////////////////////////////////////////////////////////////
		void Game_Paint(object sender, PaintEventArgs e)
		{
			if (onpaint_enabled)
			{
				Debug.Write(".");
				RenderScene();
				if (device_lost) Invalidate();
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////
	class DirectionalLight
	{
		float angle_beta;
		float angle_alpha;

		public DirectionalLight()
		{
			angle_beta = 0;
			angle_alpha = 0.32f;
		}

		public void AdjustLatitude(float d)
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

		public void AdjustLongitude(float d)
		{
			angle_beta = (float)((angle_beta + d) % (2 * Math.PI));
		}

		public void SetDirectionalLight(Device device, int lightidx, int signx, int signy)
		{
			Light light = new Light();
			light.Type = LightType.Directional;
			light.Ambient = Color.Gray;
			light.Diffuse = Color.LightGray;

			double t = Math.Cos(angle_alpha);
			float x = signx * (float)(100 * t * Math.Cos(angle_beta));
			float y = signy * (float)(100 * t * Math.Sin(angle_beta));
			float z = (float)(100 * Math.Sin(angle_alpha));
			light.Direction = new Vector3(x, y, z);

			device.SetLight(lightidx, light);
			device.EnableLight(lightidx, true);
		}
	}

	////////////////////////////////////////////////////////////////////////////////////
	class Camera
	{
		float radius;

		float R;
		float alpha;
		float beta;

		public Camera(float radius)
		{
			this.radius = radius;

			R = radius;
			alpha = (float)(Math.PI * 3 / 4);
			beta = (float)(-Math.PI / 2);
		}

		public void ResetRadius()
		{
			R = radius;
		}

		public void SetViewTransform(Device device)
		{
			double t = R * Math.Cos(alpha - Math.PI / 2);
			float z = (float)(-R * Math.Sin(alpha - Math.PI / 2));
			float x = (float)(t * Math.Cos(beta));
			float y = (float)(t * Math.Sin(beta));

			device.SetTransform(TransformState.View, Matrix.LookAtLH(
				new Vector3((float)x, (float)y, (float)z),
				new Vector3(0, 0, 0),
				new Vector3(0, 0, -1)));
		}

		public void IncreaseRadius(float d)
		{
			if (R + d <= 3 * radius) R += d; else R = 3 * radius;
		}
		public void DecreaseRadius(float d)
		{
			if (R - d >= 0.05 * radius) R -= d; else R = 0.05F * radius;
		}

		public void IncreaseLatitude(float d)
		{
			if (alpha + d <= Math.PI) alpha += d; else alpha = (float)Math.PI - 0.001F;
		}
		public void DecreaseLatitude(float d)
		{
			if (alpha - d >= 0) alpha -= d; else alpha = 0.001F;
		}

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
