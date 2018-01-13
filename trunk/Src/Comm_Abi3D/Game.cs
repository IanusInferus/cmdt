using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Comm_Abi3D
{
	public partial class Game : Form
	{
		Device device;
		PresentParameters present_params;

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		ABI abi;			//ABI文件数据
		AnimatedModel am;	//动画模型
		ShadowModel sm;		//阴影模型

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		CoordAxies axises;	//坐标轴

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		bool deviceLost;    //设备丢失标志

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		Microsoft.DirectX.Direct3D.Font d3dfont;//D3D字体
		System.Drawing.Font gdifont;			//GDI字体

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		Camera camera; //镜头
		int DisplayWireFrame = 0; //显示贴图(0)、贴图线框(1)、还是纯线框(2)

		bool DrawBackground = false; //黑色(false)、粉色(true)
		bool DrawCoordinateAxies = true; //画不画坐标轴
		bool DrawShadow = true; //画不画阴影
		int DrawInformationText = 1; //显示信息否

		bool FSAA = false; //全屏抗锯齿否
		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public Game(string filename)
		{
			InitializeComponent();

			this.Shown += new EventHandler(Game_Shown);
			this.ResizeEnd += new EventHandler(Game_ResizeEnd);
			this.Resize += new EventHandler(Game_Resize);
			this.MouseWheel += new MouseEventHandler(this.Game_MouseWheel);
			this.MouseDoubleClick += new MouseEventHandler(Game_MouseDoubleClick);

			abi = new ABI(filename);
			am = new AnimatedModel(abi);
			
			Vector3 n0=new Vector3(-1, 0, -1);
			sm = new ShadowModel(abi, am, n0, 0x505050);

			//设置标题栏
			FileInfo fi = new FileInfo(filename);
			filename = fi.Name.ToLower();
			Text = "3D .Abi Viewer - " + filename;

			//背景色相关
			BackColor = Color.DarkGray;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public bool InitializeGraphics()
		{
			//初始化设备参数
			CreatePresentParameters();

			Device.IsUsingEventHandlers = false; //关闭MDX的自动事件布线机制!

			CreateFlags cf = D3DConfiguration.GetAppropriateCreateFlags();
			device = new Device(0, DeviceType.Hardware, this, cf, present_params);

			device.DeviceReset += new EventHandler(this.OnDeviceReset);
			device.DeviceLost += new EventHandler(this.OnDeviceLost);

			gdifont = new System.Drawing.Font("新宋体", 9);//GDI字体准备

			axises=new CoordAxies(); //初始化坐标轴

			SetupDevice(); //重建所有资源

			camera = new Camera(am.radius * 5f);//镜头准备

			return true;
		}

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
			present_params.BackBufferCount = 1;
			present_params.EnableAutoDepthStencil = true;

			present_params.AutoDepthStencilFormat = D3DConfiguration.GetAppropriateDepthFormat();

			//谨慎开启全屏反锯齿FSAA:其性能冲击非常大
			if (FSAA)
			{
				int quality;
				MultiSampleType type;

				D3DConfiguration.GetAppropriateMultiSampleType(out type, out quality);
				if (type == MultiSampleType.NonMaskable)
				{
					present_params.SwapEffect = SwapEffect.Discard;	//必须!
					present_params.MultiSample = type;				//开启全屏反锯齿FSAA
					present_params.MultiSampleQuality = quality - 1;//容许的最高级别
				}
			}
#endif
		}

		public void Cleanup()
		{
			if (am.vexbuf != null)
			{
				am.vexbuf.Dispose();
				am.vexbuf = null;
			}

			if (sm.shadowbuf != null)
			{
				sm.shadowbuf.Dispose();
				sm.shadowbuf = null;
			}

			if (am.texture != null)
			{
				am.DisposeAllTextures();
				am.texture = null;
			}

			if (d3dfont!=null)
			{
				d3dfont.Dispose();
				d3dfont = null;
			}

			if (gdifont != null)
			{
				gdifont.Dispose();
				gdifont = null;
			}

			axises.DisposeAllBuffer();

			if (device != null)
			{
				device.Dispose();
				device = null;
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		protected void OnDeviceReset(object sender, EventArgs e)
		{
			SetupDevice(); //重建所有资源
		}

		protected void OnDeviceLost(object sender, EventArgs e)
		{
			if (am.vexbuf != null)
			{
				am.vexbuf.Dispose();
				am.vexbuf = null;
			}

			if (sm.shadowbuf != null)
			{
				sm.shadowbuf.Dispose();
				sm.shadowbuf = null;
			}

			if (d3dfont != null)
			{
				d3dfont.Dispose();
				d3dfont = null;
			}

			axises.DisposeAllBuffer();

			/*
			if (mm.texture != null) //因为texture的usage是managed，所以设备丢失时无需重建
			{
				mm.DisposeAllTextures();
				mm.texture = null;
			}
			*/
		}
		
		//////////////////////////////////////////////////////////////////////////////////////////////////////
		protected void SetupDevice()
		{
			if (am.vexbuf == null) { am.CreateVertexBuffer(device); }
			if (am.texture == null) { am.CreateAllTextures(device); }

			if (sm.shadowbuf == null) { sm.CreateVertexBuffer(device); }
			axises.CreateAllBuffer(device, new Vector3(), am.radius);

			if (d3dfont == null) d3dfont = new Microsoft.DirectX.Direct3D.Font(device, gdifont);
		}
	
		//////////////////////////////////////////////////////////////////////////////////////////////////////
		protected void SetupMatrices()
		{
			//float angle = Environment.TickCount / 2000.0F; //自动旋转
			//device.Transform.World = Matrix.Translation(-am.center) *Matrix.RotationZ(angle);//先平移到包络球心、然后在沿着Z轴旋转

			//交换y轴和z轴，将右手系数据转换到左手系里面来画
			Matrix RHtoLH = Matrix.Identity;
			RHtoLH.M22 = 0; RHtoLH.M23 = 1;
			RHtoLH.M32 = 1; RHtoLH.M33 = 0;

			device.Transform.World = RHtoLH; //使用模型原点而不是Mesh包围球体中心作为世界原点
			camera.SetViewTransform(device);

			device.Transform.Projection = Matrix.PerspectiveFovLH(
				(float)Math.PI / 4.0F,
				(float)ClientSize.Width / (float)ClientSize.Height, //保持正确的横纵比
				20F,		//IMPORTANT!! 最重要的参数 about Hidden Lines Removal
				12000.0F);	//IMPORTANT!! 最重要的参数
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public void Render()
		{
			//计算下一帧的模型
			am.Animate(delta_time);

			if (deviceLost) AttemptRecovery();
			if (deviceLost) return;

			device.RenderState.Lighting = false;
			device.RenderState.CullMode = Cull.CounterClockwise;//右手系拣选
			device.RenderState.SlopeScaleDepthBias = 2F;

			device.SamplerState[0].MinFilter = D3DConfiguration.GetAppropriateTextureMinFilter(); //缩小滤波
			device.SamplerState[0].MagFilter = D3DConfiguration.GetAppropriateTextureMagFilter(); //放大滤波

			if (DisplayWireFrame == 1 || DisplayWireFrame == 2) //显示贴图线框或纯线框
				device.RenderState.FillMode = FillMode.WireFrame;
			else //显示贴图
				device.RenderState.FillMode = FillMode.Solid;

			if (DrawBackground)
				device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0F, 0);
			else
				device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.DarkGray, 1.0F, 0);

			//关键：只要alpha值大于等于1的点才显示并更新zbuf！即，alpha=0的点直接不画，也不会影响zbuf!
			//注意：盟2的abi里面没有透明色的概念，不知道盟3有没有
			//device.RenderState.AlphaTestEnable = true;
			//device.RenderState.AlphaFunction = Compare.GreaterEqual;
			//device.RenderState.ReferenceAlpha = 1;

			device.BeginScene();
			{
				SetupMatrices();

				//---------------------------------------------------------------------------------------
				device.VertexFormat = CustomVertex.PositionColoredTextured .Format;

				device.SetStreamSource(0, am.vexbuf, 0);
				for (int i = 0; i < am.texture.Length; i++)
				{
					if (DisplayWireFrame == 2)//纯线框
						device.SetTexture(0, null);
					else //贴图线框或者是贴图
						device.SetTexture(0, am.texture[i]);

					//注意：无需Alpha贴图功能，盟2的abi里面没有Alpha贴图的概念，不知道盟3有没有
					//device.RenderState.SourceBlend = Blend.One;
					//device.RenderState.DestinationBlend = Blend.Zero;
					//device.RenderState.AlphaBlendEnable = false;

					int count = (am.txtoffset[i + 1] - am.txtoffset[i]) / 3;
					if (count != 0)
						device.DrawPrimitives(PrimitiveType.TriangleList, am.txtoffset[i], count);		
				}

				//---------------------------------------------------------------------------------------
				if (DrawShadow) //从前向后绘制，Z深度优化
				{
					sm.Animation(); //计算下一帧的阴影

					device.VertexFormat = CustomVertex.PositionColored.Format;
					device.SetStreamSource(0, sm.shadowbuf, 0);
					device.SetTexture(0, null);
					device.DrawPrimitives(PrimitiveType.TriangleList, 0, sm.totalf);
				}

				//---------------------------------------------------------------------------------------
				if (DrawCoordinateAxies)
				{
					axises.Render(device);
					PrintAxiesComment();
				}
				
				//---------------------------------------------------------------------------------------
				switch (DrawInformationText)
				{
					case 1:
						PrintMessageOnScene();
						break;
					case 2:
						PrintBoneHierarchyOnScene();
						break;
				}
			}	
			device.EndScene();

			try
			{
				device.Present();
			}
			catch (DeviceLostException)
			{
				deviceLost = true;
			}
		}

		void PrintAxiesComment()
		{
			Vector3 v = new Vector3();
			v.X += 2.15f * am.radius;	v.Z += 2;
			System.Drawing.Point p = ScreenPosistionFromWorld(v);
			d3dfont.DrawText(null, "X", p.X, p.Y, Color.Red);

			v = new Vector3();
			v.Y += 2.15f * am.radius; v.Z += 2;
			p = ScreenPosistionFromWorld(v);
			d3dfont.DrawText(null, "Y", p.X, p.Y, Color.Green);

			v = new Vector3();
			v.Z += 2.15f * am.radius; v.Z += 2;
			p = ScreenPosistionFromWorld(v);
			d3dfont.DrawText(null, "Z", p.X, p.Y, Color.Blue);
		}

		void PrintMessageOnScene()
		{
			int height = d3dfont.MeasureString(null, "0", DrawTextFormat.Left, Color.White).Height + 2;
			int x = 10, y = 7;

			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("< {0} >\n\n", abi.filename);
			sb.AppendFormat("文件版本: {0}\n", abi.version);
			sb.AppendFormat("材质数量: {0}\n", abi.num_texture);
			sb.AppendFormat("骨骼数量: {0}\n", abi.num_bone);
			sb.AppendFormat("模型编号: {0}/{1}\n", model + 1, abi.num_model);
			sb.AppendFormat("-顶点数量 {0}\n", abi.models[model].num_vertice);
			sb.AppendFormat("-多边形量 {0}\n", abi.models[model].num_polygon);
			sb.AppendFormat("动作编号: {0}/{1}\n", animation + 1, abi.num_animation);
			sb.AppendFormat("动作速率: {0:F1}\n", delta_time);
			sb.AppendFormat("动作名称: {0}\n", abi.animations[animation].name);
			
			d3dfont.DrawText(null, sb.ToString(), x, y, Color.White);
		}

		void PrintBoneHierarchyOnScene()
		{
			int height = d3dfont.MeasureString(null, "0", DrawTextFormat.Left, Color.White).Height + 2;
			int x = 10, y = 7;

			StringBuilder sb = new StringBuilder();
			sb.Append("Bone Hierarchy:\n\n");
			for (int i = 0; i < abi.num_bone; i++)
				sb.AppendFormat("{0,2} - [{1,2}]:{2}\n", i, abi.hierarchy[i].ParentIdx, abi.hierarchy[i].NodeName);

			d3dfont.DrawText(null, sb.ToString(), x, y, Color.White);
		}

		protected void AttemptRecovery()
		{
			int ret;
			device.CheckCooperativeLevel(out ret);

			switch (ret)
			{
				case (int)ResultCode.DeviceLost:
					break;
				case (int)ResultCode.DeviceNotReset:
					try
					{
						device.Reset(present_params);
						deviceLost = false;
					}
					catch (DeviceLostException)
					{
						Thread.Sleep(50);
					}
					break;
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		#region 窗口Resize处理相关
        Size size;                //跟踪窗口尺寸
        FormWindowState state;    //跟踪窗口状态

        private void Game_Shown(object sender, EventArgs e)
        {
            state = WindowState;
            size = ClientSize;

			WindowState = FormWindowState.Normal;
			FormBorderStyle = FormBorderStyle.Sizable;

			onpaint_enabled = true;
        }

        private void Game_ResizeEnd(object sender, EventArgs e)
        {
            if (size != ClientSize)
            {
				present_params.BackBufferWidth = ClientSize.Width;
				present_params.BackBufferHeight = ClientSize.Height;

                try
                {
					device.Reset(present_params);
                }
                catch (DeviceLostException)
                {
                    deviceLost = true;
                }

                size = ClientSize;
            }
			
			onpaint_enabled = true; //拉伸窗口完毕，可以触发OnPaint事件了
        }

        private void Game_Resize(object sender, EventArgs e)
        {
			//最大化、最小化、或者恢复的情况
			if (ClientSize != size && WindowState != state)
			{
				//非<最小化或者是从最小化恢复>，即<最大化或者是从最大化恢复>
				if (WindowState != FormWindowState.Minimized && state != FormWindowState.Minimized)
				{
					present_params.BackBufferWidth = ClientSize.Width;
					present_params.BackBufferHeight = ClientSize.Height;

					try
					{
						device.Reset(present_params);
					}
					catch (DeviceLostException)
					{
						deviceLost = true;
						Debug.WriteLine("Device was lost during Resize");
					}
				}
				size = ClientSize;
				state = WindowState;
			}
			else
			{
				//拉动窗口边框的情况
				onpaint_enabled = false; //不希望在此时触发OnPaint事件
			}
        }
#endregion
		
		//////////////////////////////////////////////////////////////////////////////////////////////////////
		int model = 0;
		int animation = 0;
		float delta_time = 1f;
		
		private void Game_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.W:
					DrawShadow = !DrawShadow;
					break;
				case Keys.S:
					//开启或者关闭FSAA
					FSAA = !FSAA;
					CreatePresentParameters();
					device.Reset(present_params);
					break;
				case Keys.I:
					DrawInformationText = (DrawInformationText + 1) % 3;
					break;
				case Keys.X:
					DrawCoordinateAxies = !DrawCoordinateAxies;
					break;
				case Keys.Oemplus:	//提高速度
					delta_time += 0.1f;
					if (delta_time >= 25) delta_time = 25f;
					break;
				case Keys.OemMinus:	//降低速度
					delta_time -= 0.1f;
					if (delta_time <= 0) delta_time = 0f;
					break;
				case Keys.A:
				case Keys.OemPeriod://下一个动作
					animation = (animation + 1) % abi.animations.Length;
					am.SetAnimation(device, animation);
					break;
				case Keys.Oemcomma://上一个动作
					animation = (animation - 1 + abi.animations.Length) % abi.animations.Length;
					am.SetAnimation(device, animation);
					break;
				case Keys.D:
				case Keys.PageUp:	//下一个模型
					model = (model + 1) % abi.models.Length;
					am.SetModel(device, model);
					sm.SetModel(device, model);
					break;
				case Keys.PageDown:	//上一个模型
					model = (model - 1 + abi.models.Length) % abi.models.Length;
					am.SetModel(device, model);
					sm.SetModel(device, model);
					break;
				case Keys.Escape:	//退出程序
				case Keys.Q:
					Close(); break;
				case Keys.K: //距离
					//地图越大，将距离调最远时，越容易出现被culling的现象，估计跟projection有关
					camera.IncreaseRadius(10F);
					//if (scaling + 0.1F <= 5) scaling += 0.1f;
					break;
				case Keys.J: //距离
					camera.DecreaseRadius(10F);
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
				case Keys.F: //区域线框
					DisplayWireFrame++;
					DisplayWireFrame %= 3;
					break;
				case Keys.R: //复位world平移矢量和camera半径
					camera.ResetRadius();
					break;
				case Keys.O:
					string fn = Program.SelectSecFile();
					if (fn != null)				
					{
						//从头创建所有的一切！
						Cleanup();
						GC.Collect();
						ResetAll(fn);
						InitializeGraphics();
					}
					break;
				case Keys.B:
					DrawBackground = !DrawBackground;
					if (DrawBackground)
						BackColor = Color.Black;
					else
						BackColor = Color.DarkGray;
					break;
				default:
					Debug.WriteLine(e.KeyCode);
					break;
			}
		}

		private void ResetAll(string filename)
		{
			//FSAA = false;
			deviceLost = false;
			DisplayWireFrame = 0;

			middle_button_pressed = false;
			right_button_pressed = false;
			
			onpaint_enabled = true;

			abi = new ABI(filename);
			am = new AnimatedModel(abi);

			Vector3 n0 = new Vector3(-1, 0, -1);
			sm = new ShadowModel(abi, am, n0, 0x505050);

			//设置标题栏
			FileInfo fi = new FileInfo(filename);
			filename = fi.Name.ToLower();
			Text = "3D .Abi Viewer - " + filename;

			//背景色相关
			DrawBackground = false;
			BackColor = Color.DarkGray;

			//动画相关
			model = 0;
			animation = 0;
			delta_time = 1f;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		bool middle_button_pressed = false;	//跟踪中键的按下情况
		bool right_button_pressed = false;	//跟踪右键的按下情况

		private void Game_MouseWheel(object sender,MouseEventArgs e)
		{
			float d;
			if (middle_button_pressed)
			{
				d = 50F * (Math.Abs(e.Delta) / 120);
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
				if (e.Delta < 0)
					camera.DecreaseLongitude(d);
				else
					camera.IncreaseLongitude(d);
			}
		}

		private void Game_MouseDown(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.Left:
					Game_KeyDown(null, new KeyEventArgs(Keys.A));
					break;
				case MouseButtons.Middle:
					middle_button_pressed = true;
					break;
				case MouseButtons.Right:
					right_button_pressed = true;
					break;
			}
		}

		private void Game_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle) middle_button_pressed = false;
			if (e.Button == MouseButtons.Right) right_button_pressed = false;
		}

		void Game_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
				Game_KeyDown(null, new KeyEventArgs(Keys.O));
			else if (e.Button == MouseButtons.Middle)
				Game_KeyDown(null, new KeyEventArgs(Keys.W));
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		bool onpaint_enabled = true; //是否容许OnPaint重绘

		private void Game_Paint(object sender, PaintEventArgs e)
		{
			if (onpaint_enabled)
			{
				Debug.Write(".");
				Render();
				if (deviceLost) Invalidate(); //On_Paint直到设备不再是Lost为止
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		//计算某世界坐标投影之后的屏幕坐标
		System.Drawing.Point ScreenPosistionFromWorld(Vector3 v)
		{
			Matrix trans = device.Transform.World * device.Transform.View * device.Transform.Projection; //常数提取
			Vector3 t = Vector3.TransformCoordinate(v, trans);

			t.X = (t.X + 1) * size.Width / 2;
			t.Y = (-t.Y + 1) * size.Height / 2;

			return new System.Drawing.Point((int)t.X, (int)t.Y);
		}
	}
}