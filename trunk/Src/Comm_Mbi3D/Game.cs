using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Comm_Mbi3D
{
	public partial class Game : Form
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////
		Device device;
		PresentParameters present_params;

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		MBI mbi;
		MbiMesh mm;

		CoordAxies axises;				//坐标轴
		//////////////////////////////////////////////////////////////////////////////////////////////////////
		bool deviceLost;				//设备丢失标志
		
		//////////////////////////////////////////////////////////////////////////////////////////////////////
		FreeRotateCamera camera;		//镜头控制
		
		int DisplayWireFrame = 0;		//显示贴图(0)、贴图线框(1)、还是纯线框(2)
		bool DrawBackground = false;	//黑色(false)、粉色(true)

		bool FSAA = false;				//开启全屏反锯齿否？
		bool DrawCoordinateAxies = true;//显示坐标轴否？

		bool MeshCenter = true;			//以Mesh中心为坐标系原点？还是以(0,0,0)为原点？

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public Game(string filename)
		{
			InitializeComponent();

			this.Shown += new EventHandler(Game_Shown);
			this.ResizeEnd += new EventHandler(Game_ResizeEnd);
			this.Resize += new EventHandler(Game_Resize);
			this.MouseWheel += new MouseEventHandler(this.Game_MouseWheel);
			this.MouseDoubleClick += new MouseEventHandler(Game_MouseDoubleClick);

			mbi = new MBI(filename);
			mm = new MbiMesh(mbi);

			//设置标题栏
			FileInfo fi = new FileInfo(filename);
			filename = fi.Name.ToLower();
			Text = "3D .Mbi Viewer - " + filename;

			//背景色相关
			BackColor = Color.Black;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public bool InitializeGraphics()
		{
			//初始化设备参数
			CreatePresentParameters(FSAA);

			Device.IsUsingEventHandlers = false; //关闭MDX的自动事件布线机制!

			if (D3DConfiguration.SupportsHardwareVertexProcessing())
				device = new Device(0, DeviceType.Hardware, this,CreateFlags.HardwareVertexProcessing, present_params);
			else
				device = new Device(0, DeviceType.Hardware, this,CreateFlags.SoftwareVertexProcessing, present_params);

			device.DeviceReset += new EventHandler(this.OnDeviceReset);
			device.DeviceLost += new EventHandler(this.OnDeviceLost);

			axises = new CoordAxies(); //初始化坐标轴

			SetupDevice(); //重建所有资源

			camera = new FreeRotateCamera(mm.radius);

			return true;
		}

		public void CreatePresentParameters(bool FSAA)
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
			present_params.BackBufferCount = 1;
			present_params.SwapEffect = SwapEffect.Flip;

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
			if (mm.vexbuf != null)
			{
				mm.vexbuf.Dispose();
				mm.vexbuf = null;
			}

			if (mm.texture != null)
			{
				mm.DisposeAllTextures();
				mm.texture = null;
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
			if (mm.vexbuf != null)
			{
				mm.vexbuf.Dispose();
				mm.vexbuf = null;
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
			if (mm.vexbuf == null) { mm.CreateVertexBuffer(device); }
			
			if (MeshCenter)
				axises.CreateAllBuffer(device, mm.center, mm.radius); //坐标轴以Mesh中心为原心
			else
				axises.CreateAllBuffer(device, new Vector3(), mm.radius); //坐标轴以(0,0,0)为原心

			if (mm.texture == null) { mm.CreateAllTextures(device); }
		}
	
		//////////////////////////////////////////////////////////////////////////////////////////////////////
		protected void SetupMatrices()
		{
			Matrix RHtoLH = Matrix.Identity;
			RHtoLH.M22 = 0; RHtoLH.M23 = 1;
			RHtoLH.M32 = 1; RHtoLH.M33 = 0;

			//由于模型数据是右手系的，所以这里我们互换模型Y/Z坐标，转换成显示所需的左手系
			//然后在设置镜头时，以+Y为正上方，+X为正右方，镜头位于-Z轴上
		
			if (MeshCenter)
				device.Transform.World = Matrix.Translation(-mm.center) * RHtoLH * camera.rotate; //以Mesh中心为坐标系原心
			else
				device.Transform.World = RHtoLH * camera.rotate; //以(0,0,0)为坐标系原心

			device.Transform.View = camera.GetViewMatrix();
				
			device.Transform.Projection = Matrix.PerspectiveFovLH(
				(float)Math.PI / 4.0F,
				(float)ClientSize.Width / (float)ClientSize.Height, //保持正确的横纵比
				20F,		//IMPORTANT!! 最重要的参数 about Hidden Lines Removal
				12000.0F);	//IMPORTANT!! 最重要的参数
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public void Render()
		{
			if (deviceLost) AttemptRecovery();
			if (deviceLost) return;

			device.RenderState.Lighting = false;
			device.RenderState.CullMode = Cull.CounterClockwise;//右手系拣选

			device.SamplerState[0].MagFilter = D3DConfiguration.GetAppropriateTextureMagFilter();//放大滤波
			device.SamplerState[0].MinFilter = D3DConfiguration.GetAppropriateTextureMinFilter();//缩小滤波

			if (DisplayWireFrame == 1 || DisplayWireFrame == 2) //显示贴图线框或纯线框
				device.RenderState.FillMode = FillMode.WireFrame;
			else //显示贴图
				device.RenderState.FillMode = FillMode.Solid;

			if (DrawBackground)
				device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(0xff,0x0,0xff), 1.0F, 0);
			else
				device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0F, 0);

			device.RenderState.SlopeScaleDepthBias = 3F; //全局性的加大所有Z-Depth间隔值，防止Z-Fighting
			//device.RenderState.DepthBias = 1F; //加性调整个别对象的Z-Depth值，其值因不同对象而异，因此效果远不及全局性调整SlopeScaleDepthBias好

			//处理Transparent Color Key的关键点：
			//只要alpha值大于等于1的点才显示并更新zbuf！即，alpha=0的点直接不画，也不会影响zbuf!
			device.RenderState.AlphaTestEnable = true;
			device.RenderState.AlphaFunction = Compare.GreaterEqual;
			device.RenderState.ReferenceAlpha = 1;

			//用于AlphaBlend的贴图Alpha混合参数，注意，AlphaBlend与AlphaTest是完全不同的概念
			device.RenderState.SourceBlend = Blend.SourceAlpha;
			device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
			device.TextureState[0].ColorOperation = TextureOperation.Modulate;
			device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
			device.TextureState[0].ColorArgument2 = TextureArgument.Diffuse;
			device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
			device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
			device.TextureState[0].AlphaArgument2 = TextureArgument.Diffuse;
		
			device.BeginScene();
			{
				SetupMatrices();
				device.VertexFormat = CustomVertex.PositionColoredTextured.Format;

				device.SetStreamSource(0, mm.vexbuf, 0);

				//以下是无需alpha混合的贴图处理过程
				device.RenderState.AlphaBlendEnable = false;
				for (int i = 0; i < mm.texture.Length; i++)
				{
					if (DisplayWireFrame == 2)//纯线框
						device.SetTexture(0, null);
					else //贴图线框或者是贴图
						device.SetTexture(0, mm.texture[i]);

					//必须首先绘制无Alpha混合的贴图(包括未知类型的贴图)，否则将导致后继Alpha混合不正确
					if (mbi.texturetype[i] != 2 && mbi.texturetype[i] != 4)
					{
						int count = (mm.txtoffset[i + 1] - mm.txtoffset[i]) / 3;
						if (count != 0)
							device.DrawPrimitives(PrimitiveType.TriangleList, mm.txtoffset[i], count);		
					}
				}

				//以下是需要alpha混合的贴图处理过程
				device.RenderState.AlphaBlendEnable = true;
				for (int i = 0; i < mm.texture.Length; i++)
				{
					if (DisplayWireFrame == 2) //纯线框
						device.SetTexture(0, null);
					else //贴图线框或者是贴图
						device.SetTexture(0, mm.texture[i]);

					//然后再绘制贴图类型为2,4的区域，因为这些区域需要Alpha混合
					//贴图的索引顺序其实部分决定了Alpha混合的顺序，所以值得特别注意！
					//这里，无需担心光晕贴图和反射贴图会发生混合错误，因为前者永远是最后一个贴图
					if (mbi.texturetype[i] == 2 || mbi.texturetype[i] == 4)
					{
						int count = (mm.txtoffset[i + 1] - mm.txtoffset[i]) / 3;
						if (count != 0)
							device.DrawPrimitives(PrimitiveType.TriangleList, mm.txtoffset[i], count);
					}
				}

				//画其他东西之前，先把Alpha混合关了!
				device.RenderState.AlphaBlendEnable = false; 
	
				if (DrawCoordinateAxies)
					axises.Render(device);
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
		private void Game_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.C:
					MeshCenter = !MeshCenter;	//切换原点位置标志
					axises.DisposeAllBuffer();	//删除原有的坐标轴
					axises = new CoordAxies();	//彻底重建坐标轴
					Vector3 center = MeshCenter ? mm.center : (new Vector3());
					axises.CreateAllBuffer(device, center, mm.radius);
					break;
				case Keys.X:
					DrawCoordinateAxies = !DrawCoordinateAxies;
					break;
				case Keys.E:
					mbi.ExportToObjFile(null, null, null);
					Debug.WriteLine("[导出功能] OBJ/MTL文件和所有贴图导出完成.");
					Text += "[*]";
					break;
				case Keys.A:
					//开启或者关闭FSAA
					FSAA = !FSAA;
					CreatePresentParameters(FSAA);
					device.Reset(present_params);
					break;
				case Keys.Escape:
				case Keys.Q:
					Close(); break;
				case Keys.K: //距离
					camera.IncreaseRadius(20f);
					break;
				case Keys.J: //距离
					camera.DecreaseRadius(20f);
					break;
				case Keys.R: //复位旋转阵
					camera.Reset();
					break;
				case Keys.F: //区域线框
					DisplayWireFrame++;
					DisplayWireFrame %= 3;
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
						BackColor = Color.FromArgb(0xff, 0x0, 0xff);
					else
						BackColor = Color.Black;
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

			MeshCenter = true;

			onpaint_enabled = true;

			mbi = new MBI(filename);
			mm = new MbiMesh(mbi);

			//设置标题栏
			FileInfo fi = new FileInfo(filename);
			filename = fi.Name.ToLower();
			Text = "3D .Mbi Viewer - " + filename;

			//背景色相关
			//DrawBackground = false;
			//BackColor = Color.Black;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////	
		private void Game_MouseWheel(object sender,MouseEventArgs e)
		{
			float d;
			d = (float)(100F * Math.Abs(e.Delta) / 120F);
			if (e.Delta < 0)
				camera.IncreaseRadius(d);
			else
				camera.DecreaseRadius(d);
		}

		private void Game_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				camera.StartRotate(e.X, e.Y, size);
				this.Cursor = Cursors.NoMove2D;
			}
			else if (e.Button == MouseButtons.Right)
				Game_KeyDown(null, new KeyEventArgs(Keys.F));
			else if (e.Button == MouseButtons.Middle)
				Game_KeyDown(null, new KeyEventArgs(Keys.A));
		}

		private void Game_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				camera.EndRotate();
				this.Cursor = Cursors.Default;
			}
		}

		private void Game_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				camera.UpdateRotateAccurately(e.X, e.Y, size);
		}

		void Game_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				Game_KeyDown(null, new KeyEventArgs(Keys.O));
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
	}
}