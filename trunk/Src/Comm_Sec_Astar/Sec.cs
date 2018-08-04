using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using Microsoft.DirectX;

namespace Comm_Sec_Astar
{
	public class FPoint
	{
		public float x;
		public float y;
		public Vector2 ToVector2()
		{
			return new Vector2(x, y);
		}
		public Vector3 ToVector3()
		{
			return new Vector3(x, y, 0);
		}
		static public FPoint FromVector2(Vector2 v2)
		{
			FPoint fp = new FPoint();
			fp.x = v2.X;
			fp.y = v2.Y;
			return fp;
		}
	}

	public class Edge
	{
		public FPoint from;
		public FPoint to;
		public int belong_poly;
		public int neighbor_poly;
		public bool wall;
	}

	public class Polygon
	{
		public int idx;
		public int num_edges;
		public byte[] attributes;	//2*4byte
		public float tanX;
		public float tanY;
		public float M;
		public byte[] not_sure;		//6*4byte
		public Vector3 min;
		public Vector3 max;
		public Vector3 min_bak;
		public Vector3 max_bak;
		public Edge[] edges;		//num_borders个

		public bool enterable;
		public FPoint centroid;
	}

	public class Entry
	{
		public Po po;
		public float distance;
	}

	public class Po
	{
		public int idx;
		public FPoint centroid;
		public List<Entry> neighbors;
	}

	public class Sec
	{
		public int num_points;
		public int num_edges;
		public int num_polys;
		public int num_xpolys;

		FPoint[] points;
		FPoint[] backup;

		Edge[] edges;
		Polygon[] polys;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		#region 暂时没有用到
		//Po[] po;
		
		float GetDistance(Polygon from, Polygon to)
		{
			float x = from.centroid.x - to.centroid.x;
			float y = from.centroid.y - to.centroid.y;

			return x * x + y * y;
		}

		Po[] GenPo() //生成用来A*的拓扑
		{
			Po[] p = new Po[num_polys];
			for (int i = 0; i < num_polys; i++)
			{
				p[i] = new Po();
				p[i].idx = i;
				p[i].centroid = polys[i].centroid;
				p[i].neighbors = new List<Entry>();
			}

			for (int i = 0; i < num_polys; i++)
			{
				for (int j = 0; j < polys[i].num_edges; j++)
				{
					Edge e = polys[i].edges[j];
					int to = e.neighbor_poly;
					if (to != -1 && polys[to].enterable)
					{
						Entry en = new Entry();
						en.po = p[to];
						en.distance = GetDistance(polys[i], polys[to]);
						p[i].neighbors.Add(en);
					}
				}
			}
			return p;
		}
		#endregion
		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		Geometry geo;
		
		public Sec(string fn)
		{
			using (FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					while (br.ReadInt32() != 0x3150414d) ;

					br.BaseStream.Seek(7 * 4, SeekOrigin.Current);

					/////////////////////////////////////////////////////////////////////////////////////////////////////////
					num_points = br.ReadInt32();
					num_edges = br.ReadInt32();
					num_polys = br.ReadInt32();
					num_xpolys = br.ReadInt32();

					/////////////////////////////////////////////////////////////////////////////////////////////////////////

					points = new FPoint[num_points];
					backup = new FPoint[num_points]; //backup是lp的值备份

					for (int i = 0; i < num_points; i++)
					{
						FPoint fp = new FPoint();
						fp.x = br.ReadSingle();
						fp.y = br.ReadSingle();
						points[i] = fp;

						FPoint fpbak = new FPoint();
						fpbak.x = fp.x;
						fpbak.y = fp.y;
						backup[i] = fpbak;
					}

					/////////////////////////////////////////////////////////////////////////////////////////////////////////
					edges = new Edge[num_edges];
					for (int i = 0; i < num_edges; i++)
					{
						Edge bb = new Edge();

						int idx = br.ReadInt32();
						bb.from = points[idx];

						idx = br.ReadInt32();
						bb.to = points[idx];

						bb.belong_poly = br.ReadInt32();
						bb.neighbor_poly = br.ReadInt32();

						int xx = br.ReadInt32();
						//Debug.Assert(xx == 0);

						edges[i] = bb;
					}

					/////////////////////////////////////////////////////////////////////////////////////////////////////////

					polys = new Polygon[num_polys];
					for (int i = 0; i < num_polys; i++)
					{
						Polygon dd = new Polygon();

						dd.idx = i;
						dd.num_edges = br.ReadInt32();
						dd.attributes = br.ReadBytes(8);
						dd.tanX = br.ReadSingle();
						dd.tanY = br.ReadSingle();
						dd.M = br.ReadSingle();
						dd.not_sure = br.ReadBytes(24);
						
						float x, y, z;
						x = br.ReadSingle();
						y = br.ReadSingle();
						z = br.ReadSingle();
						dd.min = new Vector3(x, y, z);
						dd.min_bak = new Vector3(x, y, z);

						x = br.ReadSingle();
						y = br.ReadSingle();
						z = br.ReadSingle();
						dd.max = new Vector3(x, y, z);
						dd.max_bak = new Vector3(x, y, z);

						dd.edges = new Edge[dd.num_edges];
						for (int j = 0; j < dd.num_edges; j++)
						{
							int idx = br.ReadInt32();
							dd.edges[j] = edges[idx];
						}

						int att = dd.attributes[4];
						if (att == 0x4 || att == 0x10 || att == 0x14 || att == 0x16)
							dd.enterable = false;
						else
							dd.enterable = true;

						if (dd.attributes[6] == 0x2)
							dd.enterable = false;

						polys[i] = dd;
					}

					//对backup进行标准化
					NormalizeAllPoints();

					//创建几何类
					geo = new Geometry(points, edges, polys);

					//更新所有质心
					geo.UpdateAllCentroid();
				}
			}

			//给所有edge打上障碍标记
			foreach (Edge e in edges)
			{
				if (e.neighbor_poly == -1)
					e.wall = true;
				else
					if (!polys[e.neighbor_poly].enterable)
						e.wall = true;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		float b_minx, b_maxx;
		float b_miny, b_maxy;
		public void NormalizeAllPoints() //调整所有的backup
		{
			b_maxx = float.NegativeInfinity;
			b_minx = float.PositiveInfinity;
			b_maxy = float.NegativeInfinity;
			b_miny = float.PositiveInfinity;

			for (int i = 0; i < num_points; i++)
			{
				if (backup[i].x > b_maxx) b_maxx = backup[i].x;
				if (backup[i].x < b_minx) b_minx = backup[i].x;
				if (backup[i].y > b_maxy) b_maxy = backup[i].y;
				if (backup[i].y < b_miny) b_miny = backup[i].y;
			}

			for (int i = 0; i < num_points; i++)
			{
				backup[i].x += Math.Abs(b_minx);
				backup[i].y += Math.Abs(b_miny);
			}
		
			for (int i = 0; i < num_polys; i++)
			{
				Polygon poly = polys[i];
				poly.min_bak.X += Math.Abs(b_minx);
				poly.max_bak.X += Math.Abs(b_minx);
				poly.min_bak.Y += Math.Abs(b_miny);
				poly.max_bak.Y += Math.Abs(b_miny);
			}		
			
			b_maxx = (b_maxx + Math.Abs(b_minx));
			b_maxy = (b_maxy + Math.Abs(b_miny));

			b_minx = b_miny = 0;
		}

		//--------------------------------------------------------------------------------------------------------
		float minx, maxx;
		float miny, maxy;
		public void ZoomAllPoints(float n)//n倍放大
		{
			for (int i = 0; i < num_points; i++)
			{
				points[i].x = backup[i].x * n;
				points[i].y = backup[i].y * n;
			}

			for (int i = 0; i < num_polys; i++)
			{
				Polygon poly = polys[i];
				poly.min = poly.min_bak * n;
				poly.max = poly.max_bak * n;
			}	

			minx = b_minx * n;
			miny = b_miny * n;
			maxx = b_maxx * n;
			maxy = b_maxy * n;

			geo.UpdateAllCentroid();
		}
		
		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		List<WayPoint> wp;
		
		Vector2 fromv, tov;
		public Bitmap StartRay(float n)
		{
			ZoomAllPoints(n); //缩放！

			//在线段上，且沿着线段的用例
			//fromv = polys[107].edges[0].from.ToVector2();
			//tov = polys[107].edges[0].to.ToVector2();
			//Vector2 n0 = tov - fromv;
			//fromv = fromv + 0.2f * n0;

			//在线段上，但不沿着线段的用例
			//fromv = polys[107].edges[0].from.ToVector2();
			//tov = polys[107].edges[0].to.ToVector2();
			//Vector2 n0 = tov - fromv;
			//fromv = fromv + 0.2f * n0; ;
			//n0 = new Vector2(1, 0);
			//tov = fromv + 1f * n0;

			//在线段的两个点上的用例
			fromv = polys[107].edges[0].from.ToVector2();
			tov = polys[107].edges[0].to.ToVector2();

			//fromv = polys[107].centroid.ToVector2();
			//tov = polys[1].centroid.ToVector2();

			return DrawSketch(n, fromv, tov);
		}

		public bool UpdateSrc(Vector2 p0)
		{
			Vector2 v;
			Object obj;
			Belong_Status ret;

			int H = (int)maxy + 1;		
			v = new Vector2(p0.X, H - p0.Y);		
			
			ret = geo.GetPointLocation(v, out obj);
			if (ret == Belong_Status.Invalid) return false;
			if (ret == Belong_Status.Polygon)
			{
				Polygon p = (Polygon)obj;
				if (!p.enterable) return false;				
			}

			fromv = v;
			return true;
		}

		public bool UpdateDst(Vector2 p1)
		{
			Vector2 v;
			Object obj;
			Belong_Status ret;

			int H = (int)maxy + 1;
			v = new Vector2(p1.X, H - p1.Y);

			ret = geo.GetPointLocation(v, out obj);
			if (ret == Belong_Status.Invalid) return false;
			if (ret == Belong_Status.Polygon)
			{
				Polygon p = (Polygon)obj;
				if (!p.enterable) return false;
			}

			tov = v;
			return true;
		}

		public Bitmap Update(float n)
		{
			ZoomAllPoints(n); //缩放！
			Bitmap bmp = DrawSketch(n, fromv, tov);
			return bmp;
		}

		//---------------------------------------------------------------------------------------------------------
		public Bitmap DrawSketch(float n,Vector2 from, Vector2 to)
		{
			//我日，边排列的顺序是右手的，而顶点排列顺序是左手的，操！
			//LOS_RESULT ret = geo.LOS_Test(
			//    polys[254].edges[2].from.ToVector2(),
			//    polys[254].edges[1].from.ToVector2(),
			//    out wp);

			LOS_RESULT ret = geo.LOS_Test(from,to,out wp);

			Bitmap bmp = new Bitmap((int)maxx + 5, (int)maxy + 5, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			using (Graphics g = Graphics.FromImage(bmp))
			{
				Brush brush = new SolidBrush(Color.Black);
				g.FillRectangle(brush, new Rectangle(0, 0, (int)maxx + 5, (int)maxy + 5));
				brush.Dispose();

				int H = (int)maxy + 1;
				using (Pen pen = new Pen(Color.Red))
				{
					for (int i = 0; i < num_polys; i++)
						DrawPolygonEdges(g, pen, i);
				}

				using (Pen pen = new Pen(Color.White))
				{
					for (int i = 0; i < num_polys; i++)
						if (!polys[i].enterable)
							DrawPolygonEdges(g, pen, i);
				}

				Font font = new Font("Tahoma", 8);
				brush = new SolidBrush(Color.White);

				string msg = string.Format("Points:{0}  Edges:{1}  Polygons:{2}  xPolygons:{3}", num_points, num_edges, num_polys, num_xpolys);
				g.DrawString(msg, font, brush, 2, 2);

				font.Dispose();
				brush.Dispose();

				DrawRayPath(g);//把路径画出来！

				//把测试多边形的AABB画出来！
				//for (int i = 0; i < num_polys; i++) DrawAABB(g, i);
				DrawAABB(g, 1);
			}

			return bmp;
		}
		//---------------------------------------------------------------------------------------------------------
		void DrawRayPath(Graphics g)
		{
			if (wp == null) return;
			
			int H = (int)maxy + 1;

			using (Pen pen = new Pen(Color.Blue))
			{
				for (int i = 1; i < wp.Count; i++)
				{
					Vector2 from = wp[i].inter;
					Vector2 to = wp[i - 1].inter;

					g.DrawLine(pen, from.X, H - from.Y, to.X, H - to.Y);
				}

			}

			using (Pen pen = new Pen(Color.Yellow))
			{
				foreach (WayPoint w in wp)
				{
					Vector2 p = w.inter;
					g.DrawLine(pen, p.X - 2, H - p.Y + 2, p.X + 2, H - p.Y - 2);
					g.DrawLine(pen, p.X - 2, H - p.Y - 2, p.X + 2, H - p.Y + 2);
				}
			}
		}
		//---------------------------------------------------------------------------------------------------------
		public void DrawAllIndex(Bitmap bmp, int fontsize) //绘制所有多边形编号
		{
			using (Graphics g = Graphics.FromImage(bmp))
			{
				int H = (int)maxy + 1;

				Font font = new Font("Arial", fontsize);
				Brush brush = new SolidBrush(Color.LightGray);
				for (int i = 0; i < num_polys; i++)
				{
					FPoint fp;

					fp = polys[i].centroid;

					float x = fp.x;
					float y = H - fp.y;

					SizeF size = g.MeasureString(i.ToString(), font);

					g.DrawString(i.ToString(), font, brush, x - size.Width / 2, y - size.Height / 2);

					//绘制质心位置标记
					//g.DrawEllipse(new Pen(Color.LightBlue), x - 2, y - 2, 3, 3);
					//g.FillEllipse(new SolidBrush(Color.LightBlue), x - 2, y - 2, 3, 3);
				}

				font.Dispose();
				brush.Dispose();
			}
		}
		//---------------------------------------------------------------------------------------------------------
		public void DrawPolygonEdges(Graphics g, Pen pen, int idx) //画出一个多边形的所有边
		{
			Polygon d = polys[idx];

			int H = (int)maxy + 1;

			for (int i = 0; i < d.num_edges; i++)
			{
				FPoint from = d.edges[i].from;
				FPoint to = d.edges[i].to;
				g.DrawLine(pen, from.x, H - from.y, to.x, H - to.y);
			}
		}
		//---------------------------------------------------------------------------------------------------------
		public void DrawAABB(Graphics g, int idx)
		{
			int H = (int)maxy + 1;
			Polygon p = polys[idx];
			using (Pen pen = new Pen(Color.LightGreen))
			{
				Vector3 d = p.max - p.min;
				g.DrawRectangle(pen, p.min.X, H - p.max.Y, d.X, d.Y);
			}
		}
	}
}
