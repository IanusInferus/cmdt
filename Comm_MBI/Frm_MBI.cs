using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace Comm_MBI
{
	public partial class Frm_MBI : Form
	{
		PictureInfo[] pi;
		Bitmap[] bmps;
		District[] di;
		
		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		public Frm_MBI()
		{
			InitializeComponent();
		}

		private void Frm_MBI_Load(object sender, EventArgs e)
		{
			if (!OpenFile()) Close();
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		bool OpenFile()
		{
			OpenFileDialog ofg = new OpenFileDialog();
			ofg.Title = "Select .MBI File";
			ofg.Filter = "MBI files (*.mbi)|*.mbi";
			ofg.FilterIndex = 0;
			ofg.RestoreDirectory = true;

			string fn;
			if (ofg.ShowDialog() == DialogResult.OK)
				fn = ofg.FileName;
			else
				return false; //无效

			ReadAll(fn);			//get di,pi
			GenerateAllBitmaps();	//get bmps

			PicIdx = 0;
			FrameLine = true;

			return true; //有效
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		public void ReadAll(string filename)
		{
			//用Dragon_UNPAcker读出来的数据是错误的，不要用这个软件来解压.mbi文件
			using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					Debug.Assert(br.ReadInt32() == 0x4d424931);
					
					int num_points = br.ReadInt32();
					int num_districts = br.ReadInt32();

					Point3D[] points = new Point3D[num_points];
					for (int i=0;i<num_points;i++)
					{
						points[i]=new Point3D();
						points[i].X = br.ReadSingle();
						points[i].Y = br.ReadSingle();
						points[i].Z = br.ReadSingle();
					}

					District[] districts = new District[num_districts];
					for (int i = 0; i < num_districts; i++ )
					{
						districts[i]=new District();
						districts[i].num_lines = br.ReadByte();
						districts[i].pic_id = br.ReadByte();

						districts[i].map_points = new Point2D[districts[i].num_lines];
						for (int j = 0; j < districts[i].num_lines; j++)
						{
							districts[i].map_points[j] = new Point2D();
							districts[i].map_points[j].point_id = br.ReadInt16();
							districts[i].map_points[j].U = br.ReadInt16(); //未除以16!
							districts[i].map_points[j].V = br.ReadInt16(); //未除以16!

							districts[i].map_points[j].point = points[districts[i].map_points[j].point_id];
						}
					}

					int num_objects = br.ReadInt32();
					Object[] objects = new Object[num_objects];
					for (int i=0;i<num_objects;i++)
					{
						objects[i]=new Object();

						byte[] pb = br.ReadBytes(44);
						int j;
						for (j = 0; j < 44; j++) if (pb[j] == 0) break;
						objects[i].obj_name = Encoding.ASCII.GetString(pb,0,j);
						Debug.WriteLine(objects[i].obj_name);

						objects[i].start_district_id = br.ReadInt32();
						objects[i].end_district_id = br.ReadInt32() - 1;
					}

					int num_pictures = br.ReadInt32();
					PictureInfo[] picinfos = new PictureInfo[num_pictures];
					for (int i=0;i<num_pictures;i++)
					{
						picinfos[i] = new PictureInfo();
						picinfos[i].UNKNOWN = br.ReadInt32();
						picinfos[i].width = br.ReadInt32();
						picinfos[i].height = br.ReadInt32();
						//picinfos[i].pic_name = br.ReadBytes(32); //只有comm3才需要！

						picinfos[i].color=new Color[256];
						for (int j=0;j<256;j++)
						{						
							byte r = br.ReadByte();
							byte g = br.ReadByte();
							byte b = br.ReadByte();
						
							picinfos[i].color[j] = Color.FromArgb(r, g, b);
						}

						picinfos[i].data = br.ReadBytes(picinfos[i].width * picinfos[i].height);
					}

					pi = picinfos;
					di = districts;
					Debug.WriteLine(fs.Position);
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		public void GenerateAllBitmaps()
		{
			bmps = new Bitmap[pi.Length];

			for (int i=0;i<pi.Length;i++)
			{
				PictureInfo p = pi[i];
				bmps[i] = new Bitmap(p.width,p.height);

				BitmapData bmpdata = bmps[i].LockBits(new Rectangle(0, 0, bmps[i].Width, bmps[i].Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				IntPtr ptr = bmpdata.Scan0;
				int pitch = bmpdata.Stride / 4;
				
				unsafe
				{
					int t = 0;
					int* start = (int*)ptr;
					for (int y = 0; y < p.height; y++)
					{
						for (int x = 0; x < p.width; x++)
						{
							int idx = p.data[t++];
							if (idx == 0xfe)
								//*(start + x) = Color.Fuchsia.ToArgb();
								*(start + x) = Color.Black.ToArgb();
							else
								*(start + x) = p.color[idx].ToArgb();
						}
						start += pitch;
					}
				}

				bmps[i].UnlockBits(bmpdata);
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		private void Frm_MBI_Paint(object sender, PaintEventArgs e)
		{
			Graphics gfx = e.Graphics;
			/*gfx.DrawImage(
				bmps[PicIdx], 
				new Rectangle(0,0,bmps[PicIdx].Width*2,bmps[PicIdx].Height*2),
				new Rectangle(0,0,bmps[PicIdx].Width,bmps[PicIdx].Height),
				GraphicsUnit.Pixel
			);*/

			gfx.DrawImage(bmps[PicIdx], 0, 0);

			if (FrameLine)
				for (int i = 0; i < di.Length; i++)
					if (di[i].pic_id == PicIdx)
						DrawDistrictMappingPoints(gfx, i);					

			//old_On_Paint(gfx); //慢速绘制！
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		int ratio = 16;
		public void DrawDistrictMappingPoints(Graphics g,int idx)
		{
			Pen p=new Pen(Color.Red);
			for (int i = 0; i < di[idx].num_lines; i++)
			{
				int from = i;
				int to = (i + 1) % di[idx].num_lines;

				g.DrawLine(p,
					di[idx].map_points[from].U / ratio, di[idx].map_points[from].V / ratio,
					di[idx].map_points[to].U / ratio, di[idx].map_points[to].V / ratio
					);
			}

			Text = String.Format(".MBI贴图浏览[{0}/{1}]", PicIdx+1, pi.Length);
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		//野蛮绘制
		public void old_On_Paint(Graphics gfx)
		{
			PictureInfo p = pi[0];
			int t = 0;
			for (int y = 0; y < p.height; y++)
				for (int x = 0; x < p.width; x++)
				{
					int idx = p.data[t++];
					Pen pen;
					if (idx == 0xfe)
						pen = new Pen(Color.Fuchsia);
					//pen = new Pen(Color.Black);
					else
						pen = new Pen(p.color[idx]);
					gfx.DrawLine(pen, x, y, x + 1, y);
				}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		public int PicIdx = 0;
		public bool FrameLine = true;
		private void Frm_MBI_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Right:
				case Keys.Space:
					PicIdx = (PicIdx + 1) % pi.Length;
					if (bmps[PicIdx].Width == 256) ratio = 16; else ratio = 32;
					break;
				case Keys.Left:
					PicIdx = (PicIdx - 1 + pi.Length) % pi.Length;
					if (bmps[PicIdx].Width == 256) ratio = 16; else ratio = 32;
					break;
				case Keys.Q:
					Close(); break;
				case Keys.F:
					FrameLine = !FrameLine; break;
				case Keys.O:
					if (!OpenFile()) return;
					break;
				default:
					return;
			}

			Refresh();
		}
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////	
	class Point3D
	{
		public float X;
		public float Y;
		public float Z;
	}

	class Point2D
	{
		public short point_id; //不准确
		public float U;//不准确
		public float V;//不准确

		public Point3D point;
	}

	class District
	{
		public byte num_lines;
		public byte pic_id;
		public Point2D[] map_points;
	}

	class Object
	{
		public string obj_name; //44字节
		public int start_district_id;
		public int end_district_id;
	}

	class PictureInfo
	{
		public int UNKNOWN;
		public int width;
		public int height;
		//public string pic_name; //32字节,only for comm3
		public Color[] color;
		public byte[] data;
	}
}