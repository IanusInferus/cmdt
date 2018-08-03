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
		///////////////////////////  ��ҪС�ĳ�ʼ����״̬���� //////////////////////////////
		Sec sec;
		WallMesh ws;
		ExtrusionMesh ms;
		PartitionLines pl;
		WallPartitionLines wpl;
		SelectionLines sl;

		//----------------------------------------------------------------------------------
		//GUI��ʾ���ƿ���
		bool EnableColoring;
		bool EnableLights;
		bool DisplayWireFrame;
		bool DisplayHorizonalMesh;	//��ѹMesh��ʾ
		bool DisplayVerticalMesh;	//Wallmesh��ʾ
		bool DisplayInfoHUD;		//��ʾ������Ϣ��

		//----------------------------------------------------------------------------------
		//��ѹMesh���������ĵ�
		Vector3 center;

		//��ǰѡ�еĶ���α�ż�������
		int curr_district;
		Vector3 curr_centroid;

		//world�е�ƽ��ʸ������ֵҪô����center��Ҫô���ڵ�ǰѡ�������������
		Vector3 world_translation;

		//��ͷ
		Camera camera;

		//ƽ�й��Դ
		DirectionalLight dir_light;

		//World�ȱȷŴ�ϵ������ʱ����
		float scaling;

		//----------------------------------------------------------------------------------
		//Direct3D�������
		private Device device;
		private bool device_lost;
		PresentParameters present_params;

		int batch; //��Чrender��һ��DrawPrimitive���ܹ����͵����Primitive����/2

		//----------------------------------------------------------------------------------
		//Win32/D3D��Դ���
		System.Drawing.Font font_selected; //����ѡ��������ʾ
		System.Drawing.Font font_hud; //������Ϣ���е�������ʾ
		Microsoft.DirectX.Direct3D.Font d3dfont_selected;
		Microsoft.DirectX.Direct3D.Font d3dfont_hud;

		Bitmap bmp_hud;			//���ڶ�̬����������ͼ��Bitmap
		Texture bkground_hud;	//��Ϣ�򱳾���ͼ
		Sprite sprite;			//������ʾ��Ϣ��ľ���

		//----------------------------------------------------------------------------------
		//��ʾ��Ϣ���
		string filename;
		string message;

		//----------------------------------------------------------------------------------
		//����״̬���
		bool onpaint_enabled;			//�ɷ񴥷�On_Paint�¼���

		////////////////////////////////////////////////////////////////////////////////////
		//���±�������Ӧ�Ĵ����¼�������٣�������Ҫÿ��resetallʱ��ʼ��
		bool middle_button_pressed = false;	//�����м��İ������
		bool right_button_pressed = false;	//�����Ҽ��İ������

		Size client_size;					//���ٴ��ڵ�ClientSize
		FormWindowState window_state;		//���ٴ��ڵ�״̬
		bool window_activated = true;		//���ٴ��ڵļ������

		////////////////////////////////////////////////////////////////////////////////////
		#region ������
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
			//���йؼ���������պͳ�ʼ��
			EnableColoring = true;
			EnableLights = false;
			DisplayWireFrame = true;
			DisplayHorizonalMesh = true;
			DisplayVerticalMesh = true;
			DisplayInfoHUD = true;

			//��ѹMesh���������ĵ�
			center = new Vector3();

			//��ǰѡ�еĶ���α�ż�������
			curr_district = 0;
			curr_centroid = new Vector3();

			//world�е�ƽ��ʸ��
			world_translation = new Vector3();	//��ֵҪô����center��Ҫô���ڵ�ǰѡ�������������

			//World�ȱȷŴ�ϵ������ʱ����
			scaling = 1F;

			//Direct3D���
			device = null;
			device_lost = false;
			present_params = null;
			batch = 0;

			//Direct3D��Դ���
			font_selected = font_hud = null;
			d3dfont_selected = d3dfont_hud = null;
			bmp_hud = null;
			bkground_hud = null;
			sprite = null;

			//��ʾ��Ϣ���
			filename = message = null;

			//����״̬���
			onpaint_enabled = true;

			////////////////////////////////////////////////////////////////////////////////////
			//����ԭʼ���ݺ����е㡢����߿�
			sec = new Sec(fn);
			ms = new ExtrusionMesh(sec, Color.White, EnableColoring);
			ws = new WallMesh(sec, Color.Gainsboro);
			pl = new PartitionLines(ms.polys, Color.Red);
			wpl = new WallPartitionLines(sec, Color.Blue);

			//��sl��ȡ�Ĺ�������ǲ�ͬ�ģ���VertexBuffer����������ͬʱ������ɾ��
			//���������ʼ����InitializeGraphics����Ȼ���ʼ��
			sl = null;

			//��ʼ����ǰѡ�еĶ���ε�����
			curr_centroid = ms.polys[curr_district].GetCentroid();

			//���ñ�����
			FileInfo fi = new FileInfo(fn);
			filename = fi.Name.ToLower();
			Text = "3D .Sec Viewer - " + filename;

			//��ʼ����Ϣ�ַ���
			GenerateMessageString();
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void CreatePresentParameters()
		{
			//��ʼ���豸����
			present_params = new PresentParameters();

#if FULL_SCREEN
			//ȫ��
			present_params.Windowed = false;
			present_params.BackBufferCount = 2;
			present_params.BackBufferWidth = 1024;
			present_params.BackBufferHeight = 768;
			present_params.BackBufferFormat = Format.X8R8G8B8;
			present_params.SwapEffect = SwapEffect.Flip;
			present_params.EnableAutoDepthStencil = true;
			present_params.AutoDepthStencilFormat = DepthFormat.D24X8;
#else
			//����
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
			//��ʼ���豸����
			CreatePresentParameters();

			//�ȶ��ԵĹؼ����ã�����MDX���Զ��¼�����ȥ������
			Device.IsUsingEventHandlers = false;

			//��ʼ���豸
			device = new Device(0, DeviceType.Hardware, this,
				CreateFlags.HardwareVertexProcessing, present_params);

			device.DeviceReset += new EventHandler(this.OnDeviceReset);
			device.DeviceLost += new EventHandler(this.OnDeviceLost);

			////////////////////////////////////////////////////////////////////////////
			//Ԥ��׼����Win32����
			font_selected = new System.Drawing.Font("Tahoma", 9, FontStyle.Bold);
			font_hud = new System.Drawing.Font("Courier New", 9);

			////////////////////////////////////////////////////////////////////////////
			//����������ͼ��Win32 BMP
			bmp_hud = new Bitmap(200, 150);
			using (Graphics g = Graphics.FromImage(bmp_hud))
			using (SolidBrush brush = new SolidBrush(Color.White))
				g.FillRectangle(brush, 0, 0, 200, 150);

			////////////////////////////////////////////////////////////////////////////
			//����device��ص�����D3D��Դ
			SetupDevice();

			////////////////////////////////////////////////////////////////////////////
			//��������
			float radius = ms.CaculateBoundSphere(out center);

			//������뾶��ʼ��Camera
			camera = new Camera(radius);

			////////////////////////////////////////////////////////////////////////////
			//��ʼ��ƽ�й�Դ
			dir_light = new DirectionalLight();

			////////////////////////////////////////////////////////////////////////////
			Caps caps = Manager.GetDeviceCaps(0, DeviceType.Hardware);
			batch = (caps.MaxPrimitiveCount + 1) / 2;

			////////////////////////////////////////////////////////////////////////////
			world_translation = center; //��ʼ��worldƽ��ʸ��
		}

		////////////////////////////////////////////////////////////////////////////////////
		//�ؽ���device֮���һ��D3D��Դ
		public void SetupDevice()
		{
			//�ؽ����е�Mesh
			if (ms.mesh == null) ms.CreateExtrusionMesh(device);
			if (ws.mesh == null) ws.CreateWallMesh(device);

			//�ؽ����е�VertexBuffer
			if (pl.vertexbuf == null) pl.CreatePartitionLinesVertexBuffer(device);
			if (wpl.vertexbuf == null) wpl.CreateWallPartitionLinesVertexBuffer(device);

			//һ�崴��sl����VertexBuffer
			if (sl == null)
			{
				sl = new SelectionLines(ms.polys[curr_district]);
				sl.CreateSelectionLines(device);
			}

			////������ʾ���������
			if (d3dfont_selected == null) d3dfont_selected = new Microsoft.DirectX.Direct3D.Font(device, font_selected);
			if (d3dfont_hud == null) d3dfont_hud = new Microsoft.DirectX.Direct3D.Font(device, font_hud);

			//������Ϣ�򱳾���ͼ
			if (bkground_hud == null) bkground_hud = new Texture(device, bmp_hud, Usage.Dynamic, Pool.Default);

			//������ʾ��Ϣ��ľ���
			if (sprite == null) sprite = new Sprite(device);
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void ToDispose(IDisposable d)
		{
			if (d != null) d.Dispose();
		}

		//׼���˳������ٰ���device���ڵ�һ����Դ:D3D/Win32/Device
		public void CleanupGraphics()
		{
			//D3D��Դ			
			ToDispose(ms.mesh); ms.mesh = null;
			ToDispose(ws.mesh); ws.mesh = null;

			ToDispose(pl.vertexbuf); pl.vertexbuf = null;
			ToDispose(wpl.vertexbuf); wpl.vertexbuf = null;

			//һ��Dispose sl����VertexBuffer
			ToDispose(sl); sl = null;

			//��������D3D����
			ToDispose(d3dfont_selected); d3dfont_selected = null;
			ToDispose(d3dfont_hud); d3dfont_hud = null;

			ToDispose(sprite); sprite = null; //���پ���
			ToDispose(bkground_hud); bkground_hud = null; //������Ϣ�򱳾���ص�һ����Դ

			//Win32��Դ
			ToDispose(font_selected); font_selected = null;
			ToDispose(font_hud); font_hud = null;
			ToDispose(bmp_hud); bmp_hud = null;

			//Device
			ToDispose(device); device = null;
		}

		////////////////////////////////////////////////////////////////////////////////////
		protected void OnDeviceLost(object sender, EventArgs e)
		{
			//�ͷų�device������D3D��Դ
			ToDispose(ms.mesh); ms.mesh = null;
			ToDispose(ws.mesh); ws.mesh = null;

			ToDispose(pl.vertexbuf); pl.vertexbuf = null;
			ToDispose(wpl.vertexbuf); wpl.vertexbuf = null;

			//һ��Dispose sl����VertexBuffer
			ToDispose(sl); sl = null;

			//��������D3D����
			ToDispose(d3dfont_selected); d3dfont_selected = null;
			ToDispose(d3dfont_hud); d3dfont_hud = null;

			ToDispose(sprite); sprite = null; //���پ���
			ToDispose(bkground_hud); bkground_hud = null; //������Ϣ�򱳾���ص�һ����Դ
		}

		protected void OnDeviceReset(object sender, EventArgs e)
		{
			//�ؽ���device������D3D��Դ
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
						//ע�⣺һ�����е���������ŷ����˼������صĴ���������ʱ�ı�����ɫ��ֱ��ʵ�
						MessageBox.Show("�����˲���Ԥ�ϵĹؼ�����");
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

				//Ϊ���⻭������������4յ�����
				dir_light.SetDirectionalLight(device, 0, 1, 1);
				dir_light.SetDirectionalLight(device, 1, -1, -1);
				dir_light.SetDirectionalLight(device, 2, -1, 1);
				dir_light.SetDirectionalLight(device, 3, 1, -1);

				/*
				//һ�����Դ�Ĳ�������
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
			//world�任��ע�������ƽ��ʸ����Ҫ��������worldԭ��
			device.Transform.World = Matrix.Translation(-world_translation) * Matrix.Scaling(scaling, scaling, scaling);

			//view�任
			camera.SetViewTransform(device);

			//projection�任
			float aspect = (float)client_size.Width / (float)client_size.Height;
			device.Transform.Projection = Matrix.PerspectiveFovLH(
				(float)Math.PI / 4.0F,
				aspect,		//��ȷ�ĺ��ݱ�
				40F,		//IMPORTANT!! ����Ҫ�Ĳ��� about Hidden Lines Removal
				12000.0F);	//IMPORTANT!! ����Ҫ�Ĳ���

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
			//���ñ任����͵ƹ�
			SetupMatrices();
			SetupLights();

			////////////////////////////////////////////////////////////////////////////////////
			//device.RenderState.UseWBuffer = true;
			//device.RenderState.CullMode = Cull.None;
			//device.RenderState.FillMode = FillMode.WireFrame;

			////////////////////////////////////////////////////////////////////////////////////
			//����mesh���湹�ɣ���˶�����Ҫ�ṩ������Ϣ
			device.VertexFormat = CustomVertex.PositionNormalColored.Format;
			device.RenderState.SlopeScaleDepthBias = 1F;	//����Ҫ�Ĳ������ã������������Z-Fightingd
			//device.RenderState.DepthBias = 0F;			//��Ҫ���� about Hidden Lines Removal

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
			//�߿���Ŀǰ���������������Ĳ���֮һ
			if (DisplayWireFrame)
			{
				//����������ֱ�߹��ɣ���˶��㲻��Ҫ������Ϣ
				device.VertexFormat = CustomVertex.PositionColored.Format;
				device.RenderState.Lighting = false;	//������������ƹ�Ч��

				DrawBatchLinelist(pl.vertexbuf, pl.NumberOfLines);

				if (wpl.vertexbuf != null)
					DrawBatchLinelist(wpl.vertexbuf, wpl.NumberOfLines);
			}

			////////////////////////////////////////////////////////////////////////////////////
			//�ڵ�ǰѡ�еĶ�������Ĵ���ʾ����
			Point p = CaculateScreenXYofWorldPoint(curr_centroid);
			string s = curr_district.ToString();
			Rectangle r = d3dfont_selected.MeasureString(null, s, DrawTextFormat.Center, Color.White); //����
			p.X -= r.Width / 2;
			p.Y -= r.Height / 2;
			if (!EnableLights && !EnableColoring && DisplayHorizonalMesh)
				d3dfont_selected.DrawText(null, curr_district.ToString(), p, Color.Black);
			else if (sec.districts[curr_district].attributes[0] == 1 && EnableColoring)
				d3dfont_selected.DrawText(null, curr_district.ToString(), p, Color.Black);
			else
				d3dfont_selected.DrawText(null, curr_district.ToString(), p, Color.White);

			////////////////////////////////////////////////////////////////////////////////////
			//������Ϣ�������е����֣��ؼ���:д������᲻����CPU��AGP��pipeline?
			if (DisplayInfoHUD)
			{
				Vector3 center = new Vector3(); //��struct��û�й�ϵ
				Vector3 pos = new Vector3();

				//���ư�͸������Ϣ��
				sprite.Begin(SpriteFlags.AlphaBlend);
				sprite.Draw(bkground_hud, Rectangle.Empty, center, pos, Color.FromArgb(80, 0, 0, 0));
				sprite.End();

				//������Ϣ�ַ���
				d3dfont_hud.DrawText(null, message, 10, 7, Color.White);
			}

			////////////////////////////////////////////////////////////////////////////////////
			//��ʾ�����ѡ���(��������ɫ)
			device.RenderState.Lighting = false;
			device.VertexFormat = CustomVertex.PositionColored.Format;
			device.RenderState.AntiAliasedLineEnable = true; //BUG? ��Ӱ������Render
			DrawBatchLinelist(sl.vertexbuf, sl.NumberOfLines);
			device.RenderState.AntiAliasedLineEnable = false; //MDX BUG? �޷�ȡ��ǰ���true��WHY?

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
		//��ˮ��Render���������ܽ���CPU����
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
		//������Ϣ���е���ʾ�ַ���
		void GenerateMessageString()
		{
			District d = sec.districts[curr_district];
			message = string.Format("District Index       :{0}\nBorders Number       :{1}\nTerrain Category     :0x{2:X}\nTerrain Sub-category :0x{3:X}\nIs Enterable?        :0x{4:X}\nIllumination         :0x{5:X}\nIs the Border?       :0x{6:X}",
				this.curr_district,
				d.borders.Length,
				d.attributes[0],
				d.attributes[1],
				d.attributes[4],
				d.attributes[5],
				d.attributes[6]
			);

			string tmp = string.Format("<<{0}>>\n\n{1}", filename, message);
			message = tmp;
		}

		////////////////////////////////////////////////////////////////////////////////////
		//�����ĵ�World����ת������Ļ����
		Point CaculateScreenXYofWorldPoint(Vector3 v)
		{
			Matrix trans = device.Transform.World * device.Transform.View * device.Transform.Projection; //������ȡ
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
				case Keys.K: //����
					//��ͼԽ�󣬽��������Զʱ��Խ���׳��ֱ�culling�����󣬹��Ƹ�projection�й�
					camera.IncreaseRadius(100F);
					//if (scaling + 0.1F <= 5) scaling += 0.1f;
					break;
				case Keys.J: //����
					camera.DecreaseRadius(100F);
					//if (scaling - 0.1F >= 0.1) scaling -= 0.1f;
					break;
				case Keys.Left: //�ӽ�
					camera.DecreaseLongitude((float)(0.02 * Math.PI));
					break;
				case Keys.Right: //�ӽ�
					camera.IncreaseLongitude((float)(0.02 * Math.PI));
					break;
				case Keys.Up: //�ӽ�
					camera.IncreaseLatitude((float)(0.02F * Math.PI));
					break;
				case Keys.Down: //�ӽ�
					camera.DecreaseLatitude((float)(0.02F * Math.PI));
					break;
				case Keys.Z: //���߷���
					dir_light.AdjustLongitude((float)(-0.02 * Math.PI));
					break;
				case Keys.X: //���߷���
					dir_light.AdjustLongitude((float)(0.02 * Math.PI));
					break;
				case Keys.A: //���߷���				
					dir_light.AdjustLatitude((float)(0.02 * Math.PI));
					break;
				case Keys.S: //���߷���
					dir_light.AdjustLatitude((float)(-0.02 * Math.PI));
					break;
				case Keys.F: //�����߿�
					DisplayWireFrame = !DisplayWireFrame;
					break;
				case Keys.L: //���տ���
					EnableLights = !EnableLights;
					break;
				case Keys.H: //ˮƽ�濪��
					DisplayHorizonalMesh = !DisplayHorizonalMesh;
					break;
				case Keys.V: //��ֱ�濪��
					DisplayVerticalMesh = !DisplayVerticalMesh;
					break;
				case Keys.I:
					DisplayInfoHUD = !DisplayInfoHUD;
					break;
				case Keys.R: //��λworldƽ��ʸ����camera�뾶
					world_translation = center;
					camera.ResetRadius();
					break;
#if (!FULL_SCREEN)
				case Keys.O:
					string fn = Program.SelectSecFile();
					if (fn != null)
					{
						//��ͷ�������е�һ�У�
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
		#region �����������¼�
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
				//���pick����Ч�ģ������world�б�ѡȡ����ε�����
				curr_centroid = ms.polys[curr_district].GetCentroid();

				//������Ϣ�ַ���
				GenerateMessageString();
			}
			//���ԣ����Զ������������ļ������ķ����Ƿ���ȷ
			//Matrix trans = device.Transform.World * device.Transform.View * device.Transform.Projection; //������ȡ
			//Vector3 t = Vector3.TransformCoordinate(cp, trans);
			//Debug.WriteLine(t);			
		}

		private void Game_DoubleClick(object sender, EventArgs e)
		{
			MouseEventArgs me = (MouseEventArgs)e;
			if (me.Button != MouseButtons.Left) return;

			//˫�����ʱ������worldƽ��ʸ��Ϊ��ǰѡ���������λ��
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
		#region Picking System���
		bool Picking(int sx, int sy)
		{
			float P11 = device.Transform.Projection.M11;
			float P22 = device.Transform.Projection.M22;

			//screen��projectionƽ�棬�൱�����ȵ���projection�任
			float px = ((2F * sx) / client_size.Width - 1F) / P11;
			float py = ((-2F * sy) / client_size.Height + 1F) / P22;
			float pz = 1F;

			Vector3 ray_pos = new Vector3(0F, 0F, 0F);
			Vector3 ray_dir = new Vector3(px, py, pz);

			//����world��view�任
			Matrix invert = Matrix.Invert(device.Transform.World * device.Transform.View);

			//��������world�е�����
			ray_pos.TransformCoordinate(invert);
			ray_dir.TransformNormal(invert);
			ray_dir.Normalize();

			//����ray��ms���ཻ�����
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
		#region ��¼���ڵ�ǰ�Ƿ񱻼���
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
		#region �����һ����ʾ��Resize����С��������¼�
		//���ڵ�һ����ʾ��ʱ�򴥷�
		private void Game_Shown(object sender, EventArgs e)
		{
#if FULL_SCREEN			
			client_size = new Size(1024, 768); //ȫ��ģʽ�£�size��Զ����1024*768
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

		//�������ڣ��ı����Сʱ������ֻ�޸�size
		//ȫ����ʱ����¼�����Զ���ᱻ������������Ϊ�˱������...
		private void Game_ResizeEnd(object sender, EventArgs e)
		{
#if FULL_SCREEN
			client_size = new Size(1024, 768); //ȫ��ģʽ�£�size��Զ����1024*768
#else
			if (ClientSize != client_size) //һ��size�����˱仯
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
			onpaint_enabled = true; //���촰����ϣ����Դ���OnPaint�¼���
#endif
		}

		//���ڴ���������󡢴��������ʱ�������޸�size��state
		//ȫ��ʱ�����¼�����Ч����Reset����Զ����ִ��
		private void Game_Resize(object sender, EventArgs e)
		{
#if FULL_SCREEN
			client_size = new Size(1024, 768); //ȫ��ģʽ�£�size��Զ����1024*768
			if (WindowState == FormWindowState.Minimized) //Ҫô���Ҫô��С
				window_state = FormWindowState.Minimized;
			else
				window_state = FormWindowState.Maximized;
#else
			//��󻯡���С�������߻ָ������
			if (ClientSize != client_size && WindowState != window_state)
			{
				//��<��С�������Ǵ���С���ָ�>����<��󻯻����Ǵ���󻯻ָ�>
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
				//�������ڱ߿�����
				onpaint_enabled = false; //��ϣ���ڴ�ʱ����OnPaint�¼�
			}
#endif
		}
		#endregion

		////////////////////////////////////////////////////////////////////////////////////	
		//ֻ������ҪOn_Paint�¼�����Ϊ��Ӧ��ģ̬�Ի��򵯳�ʱ�����治ˢ�µ����
		void Game_Paint(object sender, PaintEventArgs e)
		{
			if (onpaint_enabled)
			{
				Debug.Write(".");
				RenderScene();
				if (device_lost) Invalidate(); //On_Paintֱ���豸������LostΪֹ
			}
		}
	}
	////////////////////////////////////////////////////////////////////////////////////	
	/*
	class PickingSystem
	{
		//��ѹMesh���������ĵ�
		Vector3 center;

		//��ǰѡ�еĶ���α�ż�������
		int curr_district;
		Vector3 curr_centroid;

		//world�е�ƽ��ʸ������ֵҪô����center��Ҫô���ڵ�ǰѡ�������������
		Vector3 world_translation;

		//���������
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

			//screen��projectionƽ�棬�൱�����ȵ���projection�任
			float px = ((2F * sx) / client_size.Width - 1F) / P11;
			float py = ((-2F * sy) / client_size.Height + 1F) / P22;
			float pz = 1F;

			Vector3 ray_pos = new Vector3(0F, 0F, 0F);
			Vector3 ray_dir = new Vector3(px, py, pz);

			//����world��view�任
			Matrix invert = Matrix.Invert(device.Transform.World * device.Transform.View);

			//��������world�е�����
			ray_pos.TransformCoordinate(invert);
			ray_dir.TransformNormal(invert);
			ray_dir.Normalize();

			//����ray��ms���ཻ�����
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
		//ƽ�й��Դ������ϵ�еķ�λ��
		float angle_beta;
		float angle_alpha;

		public DirectionalLight()
		{
			angle_beta = 0;

			//��diffuse=grayƥ����趨ֵ
			//angle_alpha = (float)(Math.PI * (1f / 4f - 7f / 50f));

			//��diffuse=lightgrayƥ����趨ֵ
			angle_alpha = 0.32f;
		}

		public void AdjustLatitude(float d) //����γ�� 0-pi/2
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

		public void AdjustLongitude(float d) //�������� 0-2*pi
		{
			angle_beta = (float)((angle_beta + d) % (2 * Math.PI));
		}

		public void SetDirectionalLight(Device device, int lightidx, int signx, int signy)
		{
			device.Lights[lightidx].Type = LightType.Directional;
			device.Lights[lightidx].Ambient = Color.Gray; //������
			device.Lights[lightidx].Diffuse = Color.LightGray; //��ɢ��
			device.Lights[lightidx].Update();
			device.Lights[lightidx].Enabled = true;

			//���߷���Ҳ����������ϵ
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
		//��ѹMesh�������ԭʼ�뾶
		float radius;

		////////////////////////////////////////////////////////////////////////////////////////
		//����ϵ�е�Camera�뾶�ͷ�λ��
		float R;
		float alpha;
		float beta;

		////////////////////////////////////////////////////////////////////////////////////////
		public Camera(float radius)
		{
			this.radius = radius;

			R = radius;
			alpha = (float)(Math.PI * 3 / 4);	//γ�ȣ�XYƽ����Z�нǣ����֣� [-pi/2,+pi/2]ӳ�䵽[0:+pi]
			beta = (float)(-Math.PI / 2);		//���ȣ�X��OPͶӰ�ļнǣ����֣� 0:2pi
		}

		////////////////////////////////////////////////////////////////////////////////////////
		public void ResetRadius()
		{
			R = radius;
		}

		////////////////////////////////////////////////////////////////////////////////////////
		public void SetViewTransform(Device device)
		{
			//����������ת����worldֱ������
			double t = R * Math.Cos(alpha - Math.PI / 2);
			float z = (float)(-R * Math.Sin(alpha - Math.PI / 2)); //ת��������
			float x = (float)(t * Math.Cos(beta));
			float y = (float)(t * Math.Sin(beta));

			device.Transform.View = Matrix.LookAtLH(		//view�任
				new Vector3((float)x, (float)y, (float)z),	//camera���ڵ�worldλ��
				new Vector3(0, 0, 0),						//camera����worldԭ��
				new Vector3(0, 0, -1));						//camera��-ZΪ���Ϸ�
		}

		////////////////////////////////////////////////////////////////////////////////////////
		//�뾶 Radius
		public void IncreaseRadius(float d)
		{
			if (R + d <= 3 * radius) R += d; else R = 3 * radius;
		}
		public void DecreaseRadius(float d)
		{
			if (R - d >= 0.05 * radius) R -= d; else R = 0.05F * radius;
		}

		////////////////////////////////////////////////////////////////////////////////////////
		//γ�� Latitude
		public void IncreaseLatitude(float d)
		{
			if (alpha + d <= Math.PI) alpha += d; else alpha = (float)Math.PI - 0.001F;
		}
		public void DecreaseLatitude(float d)
		{
			if (alpha - d >= 0) alpha -= d; else alpha = 0.001F;
		}

		////////////////////////////////////////////////////////////////////////////////////////
		//���� Longitude
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