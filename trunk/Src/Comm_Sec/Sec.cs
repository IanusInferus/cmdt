using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using Microsoft.DirectX;

using Matrix2D = System.Drawing.Drawing2D.Matrix;

public class FPoint : IComparable<FPoint>, IEquatable<FPoint>
{
	//public int idx; //点序号
	
	public float x;
	public float y;


	public FPoint()
	{
		x = y = 0F;
	}

	public FPoint(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	public Vector2 ToVector2()
	{
		return new Vector2(x, y);
	}

	public bool Equals(FPoint other)
	{
		//!!!! 精确到小数点后4位
		long ax = (long)(x * 10000);
		long ay = (long)(y * 10000);
		long bx = (long)(other.x * 10000);
		long by = (long)(other.y * 10000);

		if (ax == bx && ay == by)
			return true;
		else
			return false;
	}

	public int CompareTo(FPoint other)
	{
		//经profiling，该函数是一个调用最为频繁的函数，因此为了加速，去掉了所有Math.Sign调用
		if (x > other.x)
			return 1;
		else if (x < other.x)
			return -1;

		if (y > other.y)
			return 1;
		else if (y < other.y)
			return -1;

		return 0;
	}

	public FPoint Clone()
	{
		return new FPoint(x, y);
	}

	public float Distance(FPoint other)
	{
		float tx = x - other.x;
		float ty = y - other.y;

		return (float)Math.Sqrt(tx * tx + ty * ty);	//导致最优解
		//return (tx * tx + ty * ty);				//导致非最优解
	}
}

public class Border
{
	public int from_idx;
	public FPoint from;

	public int to_idx;
	public FPoint to;

	public int belong_poly;
	public int neighbor_poly;

	public FPoint GetMid()
	{
		float x = (from.x + to.x) / 2;
		float y = (from.y + to.y) / 2;
		return new FPoint(x, y);
	}
}

public class Polygon
{
	public int idx; //补充，多边形编号

	public int num_borders;
	public byte[] attributes; //2*4byte
	public float tanX;
	public float tanY;
	public float M;
	public byte[] not_sure; //6*4byte
	public float minx;
	public float miny;
	public float minz;
	public float maxx;
	public float maxy;
	public float maxz;

	public int[] border_idxs;
	public Border[] borders; //num_borders个

	public bool enterable;

	//计算多边形面积
	float GetArea()
	{
		float result = 0.0F;
		for (int i = 0; i < num_borders; i++)
		{
			Border b = borders[i];
			result += (b.to.x - b.from.x) * (b.to.y + b.from.y) / 2;
		}

		return Math.Abs(result);
	}

	//计算多边形质心位置
	public FPoint GetCentroid()
	{
		float area = GetArea();

		FPoint fp = new FPoint();

		for (int i = 0; i < num_borders; i++)
		{
			Border b = borders[i];
			FPoint from = b.from;
			FPoint to = b.to;

			fp.x += (from.x + to.x) * (from.x * to.y - to.x * from.y);
			fp.y += (from.y + to.y) * (from.x * to.y - to.x * from.y);
		}

		fp.x /= 6 * area;
		fp.y /= 6 * area;

		if (fp.x < 0)
		{
			fp.x = -fp.x;
			fp.y = -fp.y;
		}

		return fp;
	}

	//判断某点是否处于多边形内
	public bool IsWithin(Vector2 test_p)
	{
		float prior = 1.0F;
		bool within = true;
		for (int i = 0; i < num_borders; i++)
		{
			Border test_b = borders[i];

			Vector2 from = test_b.to.ToVector2() - test_b.from.ToVector2();
			Vector2 to = test_p - test_b.from.ToVector2();

			float test = Vector2.Ccw(from, to);
			if (i == 0)
			{
				prior = test;
			}
			else
			{
				if ((prior > 0 && test < 0) || (prior < 0 && test > 0))
				{
					within = false;
					break;
				}
			}
		}
		return within;
	}
}

public class Grid
{
	public int num_polys;

	public int[] poly_idxs;
	public Polygon[] polygons;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Comm_Sec
{
	class GeoTopology
	{
		public int num_points;
		public int num_borders;
		public int num_polys;
		public int num_xpolys;

		public FPoint[] points;
		public Border[] borders;
		public Polygon[] polys;

		public int num_X_grids;
		public int num_Y_grids;
		public Grid[,] grids;

		public float minx, miny, maxx, maxy;

		public GeoTopology(string fn)
		{
			using (FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					while (br.ReadInt32() != 0x3150414d) ;

					br.BaseStream.Seek(7 * 4, SeekOrigin.Current);

					num_points = br.ReadInt32();
					num_borders = br.ReadInt32();
					num_polys = br.ReadInt32();
					num_xpolys = br.ReadInt32();

					//------------------------------------------------------------------------------
					points = new FPoint[num_points];

					maxx = maxy = float.NegativeInfinity;
					minx = miny = float.PositiveInfinity;
					for (int i = 0; i < num_points; i++)
					{
						FPoint dst = new FPoint();
						
						//dst.idx = i;
						
						dst.x = br.ReadSingle();
						dst.y = br.ReadSingle();

						if (dst.x > maxx) maxx = dst.x;
						if (dst.x < minx) minx = dst.x;
						if (dst.y > maxy) maxy = dst.y;
						if (dst.y < miny) miny = dst.y;

						points[i] = dst;
					}

					//------------------------------------------------------------------------------
					borders = new Border[num_borders];
					for (int i = 0; i < num_borders; i++)
					{
						Border dst = new Border();

						dst.from_idx = br.ReadInt32();
						dst.to_idx = br.ReadInt32();

						dst.from = points[dst.from_idx];
						dst.to = points[dst.to_idx];

						dst.belong_poly = br.ReadInt32();
						dst.neighbor_poly = br.ReadInt32();

						int not_sure = br.ReadInt32(); //未用

						borders[i] = dst;
					}

					//------------------------------------------------------------------------------
					polys = new Polygon[num_polys];
					for (int i = 0; i < num_polys; i++)
					{
						Polygon dst = new Polygon();

						dst.idx = i;

						dst.num_borders = br.ReadInt32();
						dst.attributes = br.ReadBytes(8);
						dst.tanX = br.ReadSingle();
						dst.tanY = br.ReadSingle();
						dst.M = br.ReadSingle();
						dst.not_sure = br.ReadBytes(24);

						dst.minx = br.ReadSingle();
						dst.miny = br.ReadSingle();
						dst.minz = br.ReadSingle();
						dst.maxx = br.ReadSingle();
						dst.maxy = br.ReadSingle();
						dst.maxz = br.ReadSingle();

						dst.border_idxs = new int[dst.num_borders];
						dst.borders = new Border[dst.num_borders];
						for (int j = 0; j < dst.num_borders; j++)
						{
							int idx = br.ReadInt32();
							dst.border_idxs[j] = idx;
							dst.borders[j] = borders[idx];
						}

						int att = dst.attributes[4];
						if (att == 0x4 || att == 0x10 || att == 0x14 || att == 0x16)
							dst.enterable = false;
						else
							dst.enterable = true;

						if (dst.attributes[6] == 0x2)
							dst.enterable = false;

						polys[i] = dst;
					}

					//------------------------------------------------------------------------------
					while (br.ReadInt32() != 0x48415332) ;
					br.ReadInt32();	//grids中polygon数量的总和，未用
					br.ReadInt32(); //用途未知

					num_X_grids = br.ReadInt32();
					num_Y_grids = br.ReadInt32();

					grids = new Grid[num_Y_grids, num_X_grids];
					for (int y = 0; y < num_Y_grids; y++)
					{
						for (int x = 0; x < num_X_grids; x++)
						{
							Grid g = new Grid();
							g.num_polys = br.ReadInt32();
							g.poly_idxs = new int[g.num_polys];
							g.polygons = new Polygon[g.num_polys];

							for (int i = 0; i < g.num_polys; i++)
							{
								int idx = br.ReadInt32();
								g.poly_idxs[i] = idx;
								g.polygons[i] = polys[idx];
							}

							grids[y, x] = g;
						}
					}
				}
			}
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////
	class DisplayTopology
	{
		GeoTopology geo;

		public int num_points;
		public int num_borders;
		public int num_polys;
		public int num_xpolys;

		public FPoint[] points;
		public Border[] borders;
		public Polygon[] polys;

		public int num_X_grids;
		public int num_Y_grids;
		public Grid[,] grids;

		//-----------------------------------------------------------------------------------------------
		public DisplayTopology(GeoTopology geo)
		{
			this.geo = geo;

			num_points = geo.num_points;
			num_borders = geo.num_borders;
			num_polys = geo.num_polys;
			num_xpolys = geo.num_xpolys;

			//------------------------------------------------------------------------------
			points = new FPoint[num_points];
			for (int i = 0; i < num_points; i++)
			{
				FPoint dst = new FPoint();
				FPoint src = geo.points[i];

				//dst.idx = src.idx;

				dst.x = src.x;
				dst.y = src.y;
				points[i] = dst;
			}

			//------------------------------------------------------------------------------
			borders = new Border[num_borders];
			for (int i = 0; i < num_borders; i++)
			{
				Border dst = new Border();
				Border src = geo.borders[i];

				dst.from_idx = src.from_idx;
				dst.to_idx = src.to_idx;

				dst.from = points[dst.from_idx];
				dst.to = points[dst.to_idx];

				dst.belong_poly = src.belong_poly;
				dst.neighbor_poly = src.neighbor_poly;

				borders[i] = dst;
			}

			//------------------------------------------------------------------------------
			polys = new Polygon[num_polys];
			for (int i = 0; i < num_polys; i++)
			{
				Polygon dst = new Polygon();
				Polygon src = geo.polys[i];

				dst.idx = src.idx;

				dst.num_borders = src.num_borders;
				dst.tanX = src.tanX;
				dst.tanY = src.tanY;
				dst.M = src.M;
				dst.attributes = (byte[])src.attributes.Clone();
				dst.not_sure = (byte[])src.not_sure.Clone();

				dst.minx = src.minx; dst.miny = src.miny; dst.minz = src.minz;
				dst.maxx = src.maxx; dst.maxy = src.maxy; dst.maxz = src.maxz;

				dst.border_idxs = new int[dst.num_borders];
				dst.borders = new Border[dst.num_borders];
				for (int j = 0; j < dst.num_borders; j++)
				{
					dst.border_idxs[j] = src.border_idxs[j];
					dst.borders[j] = borders[dst.border_idxs[j]];
				}

				dst.enterable = src.enterable;

				polys[i] = dst;
			}

			//------------------------------------------------------------------------------
			num_X_grids = geo.num_X_grids;
			num_Y_grids = geo.num_Y_grids;

			grids = new Grid[num_Y_grids, num_X_grids];
			for (int y = 0; y < num_Y_grids; y++)
			{
				for (int x = 0; x < num_X_grids; x++)
				{
					Grid src = geo.grids[y, x];
					Grid dst = new Grid();

					dst.num_polys = src.num_polys;
					dst.poly_idxs = (int[])src.poly_idxs.Clone();

					dst.polygons = new Polygon[dst.num_polys];
					for (int i = 0; i < dst.num_polys; i++)
						dst.polygons[i] = polys[dst.poly_idxs[i]];

					grids[y, x] = dst;
				}
			}

			NormalizeAllPoints();

			ValidateSecData();
		}

		//-----------------------------------------------------------------------------------------------
		void ValidateSecData()	//消除不一致现象
		{
			int total = 0;
			for (int i = 0; i < this.num_polys; i++)
			{
				Polygon p = polys[i];
				for (int j = 0; j < p.num_borders; j++)
				{
					if (p.borders[j].belong_poly != i)
					{
						p.borders[j].belong_poly = i;
						total++;
					}
				}
			}
			if (total != 0)
			{
				string msg = string.Format("调试信息：共发现{0}处不一致的数据.", total);
				Debug.WriteLine(msg);
			}
		}

		//-----------------------------------------------------------------------------------------------
		public float minx, maxx;
		public float miny, maxy;
		void NormalizeAllPoints()	//归一化所有顶点集
		{
			minx = geo.minx; miny = geo.miny;
			maxx = geo.maxx; maxy = geo.maxy;

			for (int i = 0; i < num_points; i++)
			{
				points[i].x -= minx;
				points[i].y -= miny;
			}

			maxx -= minx; maxy -= miny;
			minx = miny = 0;
		}

		//-----------------------------------------------------------------------------------------------
		public int SelectPolygon(float x, float y) //x,y:都是显示拓扑坐标系
		{
			int y_idx = (int)(y / 64);
			int x_idx = (int)(x / 64);

			if (y_idx >= 0 && y_idx < num_Y_grids && x_idx >= 0 && x_idx < num_X_grids)
			{
				Grid g = grids[y_idx, x_idx];
				Vector2 test_p = new Vector2(x, y);

				for (int i = 0; i < g.num_polys; i++)
					if (g.polygons[i].IsWithin(test_p))
						return g.poly_idxs[i];
			}

			return -1;
		}

		//-----------------------------------------------------------------------------------------------
		Color GetPolygonColor(Polygon poly)
		{
			Color c = Color.White;

			//按照地形子类别点亮
			int category = poly.attributes[0];
			switch (category)
			{
				case 0: //陆地
					c = Color.DarkOrange;
					break;
				case 1: //雪地
					c = Color.White;
					break;
				case 2: //深水
					c = Color.Blue;
					break;
				case 3: //浅水
					c = Color.LightBlue;
					break;
				case 4: //无
					c = Color.RoyalBlue;
					break;
				default:
					Debug.Assert(false);
					break;
			}

			//按照地形子类别点亮
			int sub_category = poly.attributes[1];
			switch (sub_category)
			{
				case 0: //沙子或者土壤
					c = Color.DarkOrange;
					break;
				case 1: //草地
					c = Color.LimeGreen;
					break;
				case 2://路
					c = Color.Silver;
					break;
				case 3://不好判断
					break;
				case 4://无
					break;
				case 5://木质地面
					c = Color.Gold;
					break;
				case 6://泥沙地形，河边的居多，不好判断
					c = Color.Khaki;
					break;
				case 7://雪地
					c = Color.White;
					break;
				case 8://无
					break;
				case 9://从河边卵石到土壤，不好判断
					c = Color.Linen;
					break;
				case 10://无
					break;
				case 11: //铁质地形，铁栏杆，铁楼梯
					c = Color.DarkSlateGray;
					break;
				case 12: //无
					break;
				case 13: //浅水斜坡
					c = Color.CornflowerBlue;
					break;
				case 14: //深水
					c = Color.DarkBlue;
					break;
				case 15: //卵石
					c = Color.Gainsboro;
					break;
			}

			//按照可进入性点亮
			if (!poly.enterable)
			{
				c = Color.Black;
			}

			//按照环境亮度点亮
			int brightness = poly.attributes[5];
			float m = (255 - brightness) / (float)255;
			int red = (int)(c.R * m);
			int green = (int)(c.G * m);
			int blue = (int)(c.B * m);
			c = Color.FromArgb(red, green, blue);

			//点亮边缘上的框架区域
			if (poly.attributes[6] == 0x2)
				c = Color.FromArgb(255, 0, 0, 0x25); //非常深的蓝色

			return c;
		}

		void DrawColoredPolygon(Graphics g, Polygon poly) //g: Bitmap坐标系
		{
			PointF[] ps = new PointF[poly.num_borders];

			for (int i = 0; i < poly.num_borders; i++)
			{
				Border b = poly.borders[i];
				ps[i] = new PointF(b.from.x, b.from.y);
			}

			Color c = GetPolygonColor(poly);

			using (Brush brush = new SolidBrush(c))
			{
				g.FillPolygon(brush, ps);
			}
		}

		public void DrawSketch(Graphics g, float zoom, bool show_idx, bool colored) //g: Bitmap坐标系
		{
			int W = (int)Math.Ceiling(maxx * zoom);
			int H = (int)Math.Ceiling(maxy * zoom);

			using (Brush brush = new SolidBrush(Color.Black))
				g.FillRectangle(brush, new Rectangle(0, 0, W + 1, H + 1)); //必须+1

			//从显示拓扑坐标系(笛卡尔)->Bmp设备坐标系
			Matrix2D trans = new Matrix2D(1, 0, 0, -1, 0, maxy); //Y翻转
			trans.Scale(zoom, zoom, MatrixOrder.Append);		 //再缩放
			g.Transform = trans;

			//绘制所有多边形，根据属性着色
			if (colored)
				for (int i = 0; i < num_polys; i++)
					DrawColoredPolygon(g, polys[i]);

			using (Pen pen = new Pen(Color.FromArgb(150, Color.Red)))
			{
				for (int i = 0; i < num_borders; i++)
				{
					FPoint from = borders[i].from;
					FPoint to = borders[i].to;
					g.DrawLine(pen, from.x, from.y, to.x, to.y);
				}
			}

			using (Pen pen = new Pen(Color.White))
			{
				for (int i = 0; i < num_polys; i++)
					if (!polys[i].enterable)
					{
						Polygon p = polys[i];
						for (int j = 0; j < p.num_borders; j++)
						{
							FPoint from = p.borders[j].from;
							FPoint to = p.borders[j].to;
							g.DrawLine(pen, from.x, from.y, to.x, to.y);
						}
					}
			}

			//------------------------------------------------------------------------
			if (show_idx)
			{
				g.ResetTransform();

				using (Font font = new Font("Tahoma", 8.5F * zoom))
				using (Brush brush = new SolidBrush(Color.White))
				{
					PointF[] pos = { new PointF(0, 0) };
					for (int i = 0; i < num_polys; i++)
					{
						FPoint fp = polys[i].GetCentroid();

						pos[0].X = fp.x;
						pos[0].Y = fp.y;

						//从显示拓扑坐标系->Bmp设备坐标系
						trans.TransformPoints(pos);

						SizeF size = g.MeasureString(i.ToString(), font);
						pos[0].X -= size.Width / 2;
						pos[0].Y -= size.Height / 2;

						g.DrawString(i.ToString(), font, brush, pos[0]);
					}

				}
			}

		}

		//-----------------------------------------------------------------------------------------------
		public Bitmap GetSketchBitmap(float zoom, bool show_idx, bool colored)	//zoom: 放大倍率，用的是disp_topo坐标系
		{
			int W = (int)Math.Ceiling(maxx * zoom);
			int H = (int)Math.Ceiling(maxy * zoom);
			Bitmap bmp = new Bitmap(W + 1, H + 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb); //必须+1

			using (Graphics g = Graphics.FromImage(bmp))
			{
				DrawSketch(g, zoom, show_idx, colored);
			}
			return bmp;
		}

		//-----------------------------------------------------------------------------------------------
		public void DisplaySelectedPolygon(Graphics g, Matrix2D trans_D2C, int idx) //g: Client窗口设备坐标系
		{
			if (idx == -1) return;

			g.ResetTransform();
			g.SmoothingMode = SmoothingMode.HighQuality;

			using (Pen pen = new Pen(Color.Blue, 4F))
			{
				//显示from->to顶点走向
				pen.EndCap = LineCap.ArrowAnchor;
				
				PointF[] pos = { new PointF(), new PointF() };

				Polygon p = polys[idx];
				for (int i = 0; i < p.num_borders; i++)
				{
					FPoint from = p.borders[i].from;
					FPoint to = p.borders[i].to;

					pos[0].X = from.x;
					pos[0].Y = from.y;
					pos[1].X = to.x;
					pos[1].Y = to.y;

					//显示拓扑坐标系->窗口客户区坐标系
					trans_D2C.TransformPoints(pos);

					g.DrawLine(pen, pos[0], pos[1]);
				}
			}

			//显示边序号
			using (Font font = new Font("Tahoma", 9))
			using (Brush brush = new SolidBrush(Color.Yellow))
			{
				PointF[] pos = { new PointF() };

				Polygon p = polys[idx];
				for (int i = 0; i < p.num_borders; i++)
				{
					FPoint fp = p.borders[i].GetMid();

					pos[0].X = fp.x;
					pos[0].Y = fp.y;

					//坐标系级调整
					trans_D2C.TransformPoints(pos);

					//像素级调整
					SizeF size = g.MeasureString(i.ToString(), font);
					pos[0].X -= size.Width / 2;
					pos[0].Y -= size.Height / 2;

					g.DrawString(i.ToString(), font, brush, pos[0]);
				}
			}

			using (Font font = new Font("Tahoma", 12))
			using (Brush brush = new SolidBrush(Color.GreenYellow))
			{
				FPoint fp = polys[idx].GetCentroid();

				//坐标系级调整
				PointF[] pos = { new PointF(fp.x, fp.y) };
				trans_D2C.TransformPoints(pos);

				//像素级调整
				SizeF size = g.MeasureString(idx.ToString(), font);
				pos[0].X -= size.Width / 2;
				pos[0].Y -= size.Height / 2;

				g.DrawString(idx.ToString(), font, brush, pos[0]);
			}

			g.SmoothingMode = SmoothingMode.Default;
		}

		//-----------------------------------------------------------------------------------------------
		public void DisplayGrids(Graphics g, Matrix2D trans_D2C, float zoom) //g: Client窗口设备坐标系
		{
			g.Transform = trans_D2C;

			using (Pen pen = new Pen(Color.FromArgb(80, 180, 180, 180)))
			{
				for (int y = 0; y < num_Y_grids + 1; y++)
					g.DrawLine(pen, 0, y << 6, num_X_grids << 6, y << 6);
				for (int x = 0; x < num_X_grids + 1; x++)
					g.DrawLine(pen, x << 6, 0, x << 6, num_Y_grids << 6);
			}

			g.ResetTransform();

			using (Font font = new Font("Tahoma", 24 * zoom))
			using (Brush brush = new SolidBrush(Color.GreenYellow))
			{
				PointF[] pos ={ new PointF() };
				for (int x = 0; x < num_X_grids; x++)
				{
					//坐标系级调整
					pos[0].X = (x << 6) + 32;
					pos[0].Y = 0;
					trans_D2C.TransformPoints(pos);

					//像素级调整
					SizeF size = g.MeasureString(x.ToString(), font);
					pos[0].X -= size.Width / 2;

					g.DrawString(x.ToString(), font, brush, pos[0]);
				}

				for (int y = 0; y < num_Y_grids; y++)
				{
					//坐标系级调整
					pos[0].X = 0;
					pos[0].Y = (y << 6) + 32;
					trans_D2C.TransformPoints(pos);

					//像素级调整
					SizeF size = g.MeasureString(y.ToString(), font);
					pos[0].X -= size.Width;
					pos[0].Y -= size.Height / 2;

					g.DrawString(y.ToString(), font, brush, pos[0]);
				}
			}
		}

		public void DisplayLOS(Graphics g, Matrix2D trans_D2C, List<FPoint> losw) //g: Client窗口设备坐标系
		{
			g.ResetTransform();
			g.SmoothingMode = SmoothingMode.HighQuality;

			if (losw == null) return;
			if (losw.Count == 0) return;

			PointF[] pos = new PointF[losw.Count];
			for (int i = 0; i < losw.Count; i++)
			{
				pos[i] = new PointF();
				pos[i].X = losw[i].x;
				pos[i].Y = losw[i].y;
			}

			trans_D2C.TransformPoints(pos);

			using (Pen line_pen = new Pen(Color.DeepPink, 3F))
			using (Pen dot_pen = new Pen(Color.Yellow, 2f))
			{
				line_pen.EndCap = LineCap.ArrowAnchor;
				for (int i = 1; i < losw.Count; i++)
				{
					g.DrawLine(line_pen, pos[i - 1], pos[i]);
				}

				for (int i = 0; i < losw.Count; i++)
				{
					g.DrawLine(dot_pen, pos[i].X - 3, pos[i].Y - 3, pos[i].X + 3, pos[i].Y + 3);
					g.DrawLine(dot_pen, pos[i].X - 3, pos[i].Y + 3, pos[i].X + 3, pos[i].Y - 3);
				}
			}
		}


		//-----------------------------------------------------------------------------------------------
		public void DisplayMessage(Graphics g) //g: Client窗口设备坐标系
		{
			g.ResetTransform();

			using (Font font = new Font("Tahoma", 10))
			using (Brush brush = new SolidBrush(Color.White))
			{
				string msg = string.Format("Points:{0}     Borders:{1}     Districts:{2}     xDistricts:{3}", num_points, num_borders, num_polys, num_xpolys);
				g.DrawString(msg, font, brush, 2, 2);
			}
		}

		//-----------------------------------------------------------------------------------------------
		public void DisplayWaypoints(Graphics g, Matrix2D trans_D2C, Waypoint w) //g: Client窗口设备坐标系
		{
			if (w != null)
			{
				g.SmoothingMode = SmoothingMode.HighQuality;

				using (Pen pen = new Pen(Color.Yellow, 3f))
				using (Brush brush = new SolidBrush(Color.Yellow))
				{
					pen.EndCap = LineCap.ArrowAnchor; //很酷...

					Waypoint c = w;
					Waypoint p = w.parent;

					//绘制路径
					while (p != null)
					{
						PointF[] pos = { new PointF(p.fp.x, p.fp.y), new PointF(c.fp.x, c.fp.y) };
						trans_D2C.TransformPoints(pos);

						//路径起点为一个圆点
						if (p.parent == null)
							pen.StartCap = LineCap.RoundAnchor;

						g.DrawLine(pen, pos[0], pos[1]);

						c = p;
						p = c.parent;
					}

					//绘制位点
					//g.Transform = trans_D2C;

					//c = w;
					//do
					//{
					//    g.FillRectangle(brush, c.fp.x - 5, c.fp.y - 5, 10, 10);
					//    c = c.parent;
					//} while (c != null);

					//g.ResetTransform();
				}

				g.SmoothingMode = SmoothingMode.Default;
			}
		}
	}
}