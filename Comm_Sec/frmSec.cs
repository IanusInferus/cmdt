using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using Microsoft.DirectX;

using Matrix2D = System.Drawing.Drawing2D.Matrix;

namespace Comm_Sec
{
	public partial class frmSec : Form
	{
		GeoTopology geo;
		DisplayTopology display;
		PathFinder pf;

		float zoom;			//���ŵȼ�(1/zoomΪ���ű���)
		bool show_idx;		//��ʾ����α�ŷ�
		bool show_grids;	//��ʾ���������
		bool full_screen;	//ȫ����ʾ
		bool colored;		//�����������ɫ��

		int selected;		//ѡ�еĶ���α��

		Bitmap bmp;			//����λͼ

		//------------------------------------------------------------------------------------------------
		public frmSec()
		{
			zoom = 3.5F;
			show_idx = false;
			show_grids = false;
			selected = -1;
			full_screen = false;
			colored = false;

			InitializeComponent();
		}

		//------------------------------------------------------------------------------------------------
		private void frmSec_Load(object sender, EventArgs e)
		{
			this.MouseWheel += new MouseEventHandler(this.frmSec_MouseWheel);

			OpenFileDialog ofg = new OpenFileDialog();

			ofg.Title = "Load .sec";
			ofg.Filter = "Sec files (*.sec)|*.sec";
			ofg.FilterIndex = 0;
			ofg.RestoreDirectory = true;

			if (ofg.ShowDialog() == DialogResult.OK)
			{
				geo = new GeoTopology(ofg.FileName);
				display = new DisplayTopology(geo);
				pf = new PathFinder(display);
				GenerateSketch();

				w = null;
				s = d = null;
			}
			else
			{
				Close();
			}

			Activate();//����Ч��
		}

		//------------------------------------------------------------------------------------------------
		private void frmSec_Paint(object sender, PaintEventArgs e)
		{
			int posx = (ClientSize.Width - bmp.Width) / 2;
			int posy = (ClientSize.Height - bmp.Height) / 2;
			e.Graphics.DrawImage(bmp, posx + delta.X, posy + delta.Y);

			if (show_grids)
				display.DisplayGrids(e.Graphics, trans_D2C, 1 / zoom);

			display.DisplaySelectedPolygon(e.Graphics, trans_D2C, selected);

			display.DisplayMessage(e.Graphics);

			display.DisplayLOS(e.Graphics, trans_D2C, losw);

			if (w != null)
				display.DisplayWaypoints(e.Graphics, trans_D2C, w);
		}

		//------------------------------------------------------------------------------------------------	
		private void GenerateSketch()
		{
			if (bmp != null) bmp.Dispose();
			bmp = display.GetSketchBitmap(1 / zoom, show_idx, colored);

			Width = bmp.Width + 8;
			Height = bmp.Height + 34;

			UpdateMapping(); //����ӳ��!!
		}

		//------------------------------------------------------------------------------------------------	
		private void frmSec_KeyPress(object sender, KeyPressEventArgs e)
		{
			switch (e.KeyChar)
			{
				case 'z':
					if (zoom + 0.25 <= 16) zoom += 0.25F;
					break;
				case 'x':
					if (zoom - 0.25 > 0.5) zoom -= 0.25F;
					break;
				case 'd':
					show_idx = !show_idx;
					break;
				case 'g':
					show_grids = !show_grids;
					Refresh();
					return;
				case 'f':
				case (char)13:
					full_screen = !full_screen;
					if (full_screen)
					{
						WindowState = FormWindowState.Normal;
						FormBorderStyle = FormBorderStyle.None;
						WindowState = FormWindowState.Maximized;
					}
					else
					{
						FormBorderStyle = FormBorderStyle.Sizable;
						WindowState = FormWindowState.Normal;
					}

					UpdateMapping(); //����ӳ��!
					Refresh();
					return;
				case '.':
					selected = (selected + 1) % display.num_polys;
					Refresh();
					return;
				case ',':
					if (selected == -1)
						selected = display.num_polys - 1;
					else
					{
						selected = (selected - 1) + display.num_polys;
						selected %= display.num_polys;
					}
					Refresh();
					return;
				case 'q':
				case (char)27:
					Close();
					return;
				case 'l':
					OpenFileDialog ofg = new OpenFileDialog();

					ofg.Title = "Load .sec";
					ofg.Filter = "Sec files (*.sec)|*.sec";
					ofg.FilterIndex = 0;
					ofg.RestoreDirectory = true;

					if (ofg.ShowDialog() == DialogResult.OK)
					{
						geo = new GeoTopology(ofg.FileName);
						display = new DisplayTopology(geo);
						pf = new PathFinder(display);

						selected = -1;
						zoom = 3.5F;

						GenerateSketch();

						w = null;
						s = d = null;

						Refresh();
					}

					return;
				case 's':
					SaveFileDialog sfg = new SaveFileDialog();

					sfg.Title = "Save to PNG";
					sfg.Filter = "PNG files (*.png)|*.png";
					sfg.FilterIndex = 0;
					sfg.RestoreDirectory = true;

					if (sfg.ShowDialog() == DialogResult.OK)
						bmp.Save(sfg.FileName, ImageFormat.Png);
					return;
				case 'm':
					SaveFileDialog msfg = new SaveFileDialog();
					msfg.Title = "Save to EMF";
					msfg.Filter = "EMF files (*.emf)|*.emf";
					msfg.FilterIndex = 0;
					msfg.RestoreDirectory = true;

					if (msfg.ShowDialog() == DialogResult.OK)
					{
						using (Graphics refg = this.CreateGraphics())
						using (Metafile meta = new Metafile(msfg.FileName, refg.GetHdc(), EmfType.EmfPlusDual)) //�����EmfType.EmfPlusOnly��ߴ��������������Ƴߴ�
						using (Graphics g = Graphics.FromImage(meta))
						{
							Matrix2D trans = new Matrix2D(1, 0, 0, -1, 0, display.maxy);//Y��ת
							trans.Scale(1 / zoom, 1 / zoom, MatrixOrder.Append);

							display.DrawSketch(g, 1 / zoom, show_idx, colored);
							if (show_grids) display.DisplayGrids(g, trans, 1 / zoom);
							display.DisplaySelectedPolygon(g, trans, selected);
							if (w != null) display.DisplayWaypoints(g, trans, w);
						}
					}
					return;
				case 't':
					w = pf.PathFinding(s, d);
					Refresh();
					return;
				case 'r':
					delta.X = delta.Y = 0;
					UpdateMapping();
					Refresh();
					return;
				case 'c':
					colored = !colored;
					break;
				case 'o':
					LOS lost = new LOS(display);

					if (s == null || d == null) return;

					//FPoint st = display.polys[246].borders[0].from;
					//Vector2 n = lost.GetV2(st, d);
					//Vector2 n=lost.GetV2FromBorder(display.polys[254].borders[1]);
					//Vector2 n = new Vector2(0, 1);

					//n.Normalize();		

					lost.LOS_Engine(s, d);
					
					//lost.LOS_Engine(st, n);

					losw = lost.lfp;
					Refresh();
					return;
				default:
					return;
			}
			if (geo != null)
			{
				GenerateSketch();
				Refresh();
			}
		}

		List<FPoint> losw;

		/////////////////////////////////////////////////////////////////////////////////////////////////
		Waypoint w;		//Ѱ�����õ�λ����
		FPoint s, d;	//Ѱ���������յ�

		/////////////////////////////////////////////////////////////////////////////////////////////////

		Point from = new Point();	//ǰһ���λ��
		Point delta = new Point();	//����ֶ�ƫ����

		private void frmSec_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle)
			{
				from.X = e.X;
				from.Y = e.Y;
			}
			else
			{
				//�����Ļ����->DisplayTopologyӳ��
				PointF[] pos ={ new PointF(e.X, e.Y) };
				trans_C2D.TransformPoints(pos);

				if (e.Button == MouseButtons.Left)
					d = new FPoint(pos[0].X, pos[0].Y);
				else if (e.Button == MouseButtons.Right)
					s = new FPoint(pos[0].X, pos[0].Y);

				//�߾��ȼ�ʱ������Ѱ����ʱ
				QueryPerformance.Start();
				w = pf.PathFinding(s, d);
				QueryPerformance.End();
				this.Text = QueryPerformance.GetMessage();

				//ѡ������
				selected = display.SelectPolygon(pos[0].X, pos[0].Y);

				Refresh();
			}
		}

		private void frmSec_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle)
			{
				delta.X += e.X - from.X;
				delta.Y += e.Y - from.Y;

				UpdateMapping();
				Refresh();
			}
		}

		private void frmSec_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle)
			{
				delta.X += e.X - from.X;
				delta.Y += e.Y - from.Y;

				UpdateMapping();
				Refresh();

				from.X = e.X;
				from.Y = e.Y;
			}
		}

		private void frmSec_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta > 0)
			{
				if (zoom + 0.1 <= 16) zoom += 0.1F;
			}
			else
			{
				if (zoom - 0.1 > 0.5) zoom -= 0.1F;
			}

			if (geo != null)
			{
				GenerateSketch();
				Refresh();
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////
		private void frmSec_Resize(object sender, EventArgs e)
		{
			UpdateMapping(); //����ӳ��
			Refresh();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////
		Matrix2D trans_C2D; //���ڿͻ�������ϵ->��ʾ��������ϵ(ԭ����minx,miny��)
		Matrix2D trans_D2C; //��ʾ��������ϵ(ԭ����minx,miny��)->���ڿͻ�������ϵ

		void UpdateMapping() //����ӳ���ϵ
		{
			int offset_x = (ClientSize.Width - bmp.Width) / 2;
			int offset_y = (ClientSize.Height - bmp.Height) / 2;

			trans_D2C = new Matrix2D(1, 0, 0, -1, 0, display.maxy);		//Y��ת
			trans_D2C.Scale(1 / zoom, 1 / zoom, MatrixOrder.Append);	//������
			trans_D2C.Translate(offset_x, offset_y, MatrixOrder.Append);//��ƽ��
			trans_D2C.Translate(delta.X, delta.Y, MatrixOrder.Append);	//��ƽ�ƣ����ƽ�Ʒ�����

			//trans_C2D = new Matrix2D();
			//trans_C2D.Translate(-delta.X, -delta.Y, MatrixOrder.Append);	//�ȵ������ƽ�Ʒ���
			//trans_C2D.Translate(-offset_x, -offset_y, MatrixOrder.Append);	//��ƽ�ƻ�ԭ��
			//trans_C2D.Scale(zoom, zoom, MatrixOrder.Append);				//��������
			//trans_C2D.Multiply(new Matrix2D(1, 0, 0, -1, 0, display.maxy), MatrixOrder.Append);//�ߵ�Y������

			trans_C2D = trans_D2C.Clone();
			trans_C2D.Invert();
		}
	}

	class QueryPerformance
	{
		[DllImport("kernel32.dll")]
		static extern bool QueryPerformanceCounter(ref ulong lpPerformanceCount);

		[DllImport("kernel32.dll")]
		static extern bool QueryPerformanceFrequency(ref ulong lpFrequency);

		static ulong freq;

		static ulong start_ticks;
		static ulong end_ticks;

		static QueryPerformance()
		{
			freq = 0;
			QueryPerformanceFrequency(ref freq);
		}

		public static void Start()
		{
			start_ticks = end_ticks = 0;
			QueryPerformanceCounter(ref start_ticks);
		}

		public static void End()
		{
			QueryPerformanceCounter(ref end_ticks);
		}

		public static string GetMessage()
		{
			double t = (end_ticks - start_ticks) / (double)freq;
			string msg = string.Format("��ʱ: {0,6:F2} ms", t * 1000d);
			return msg;
		}
	}
}