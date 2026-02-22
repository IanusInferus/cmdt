using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using System.Diagnostics;

namespace Comm_Sec
{
	class Vector2F
	{
		public float X;
		public float Y;

		static long scale = 10000L;

		public Vector2F()
		{
			X = Y = 0;
		}

		public Vector2F(float X, float Y)
		{
			this.X = X;
			this.Y = Y;
		}

		static public float Ccw(Vector2F left, Vector2F right)
		{
			long x1 = (long)(left.X * scale);
			long y1 = (long)(left.Y * scale);
			long x2 = (long)(right.X * scale);
			long y2 = (long)(right.Y * scale);

			long ccw = x1 * y2 - y1 * x2;

			return ((float)ccw / (float)scale);
		}

		static public float Dot(Vector2F left, Vector2F right)
		{
			long x1 = (long)(left.X * scale);
			long y1 = (long)(left.Y * scale);
			long x2 = (long)(right.X * scale);
			long y2 = (long)(right.Y * scale);

			long dot = x1 * x2 + y1 * y2;

			return ((float)dot / (float)scale);
		}
	}



	enum RESULT { TEST_BORDER, TEST_VERTEX, TEST_END };

	class ReturnInfo
	{
		public RESULT result;
		public FPoint p0;
		public Vector2 n0;
		public Polygon poly;
	}

	class LOS
	{
		public DisplayTopology topo;

		ReturnInfo ret;
		public LOS(DisplayTopology topo)
		{
			this.topo = topo;
			ret = new ReturnInfo();
		}

		public List<FPoint> lfp = new List<FPoint>();

		//========================================================================================================
		ReturnInfo TestVertexCall(FPoint p0, Vector2 n0)
		{
			ret.result = RESULT.TEST_VERTEX;
			ret.p0 = p0;
			ret.n0 = n0;
			ret.poly = null;
			return ret;
		}

		ReturnInfo TestBorderCall(FPoint p0, Vector2 n0, Polygon poly)
		{
			ret.result = RESULT.TEST_BORDER;
			ret.p0 = p0;
			ret.n0 = n0;
			ret.poly = poly;
			return ret;
		}

		ReturnInfo TestEndCall()
		{
			ret.result = RESULT.TEST_END;
			ret.p0 = null;
			ret.n0 = new Vector2();
			ret.poly = null;
			return ret;
		}

		//========================================================================================================
	
		public void Test(FPoint p0, Vector2 n0)
		{
			int y_idx = (int)(p0.y / 64);
			int x_idx = (int)(p0.x / 64);

			if (y_idx >= 0 && y_idx < topo.num_Y_grids && x_idx >= 0 && x_idx < topo.num_X_grids)
			{
				Grid g = topo.grids[y_idx, x_idx];
				
				for (int i = 0; i < g.num_polys; i++)
				{
					Polygon p = g.polygons[i];

					foreach (Border b in p.borders)
					{
						if (p0.Equals(b.from))
						{
							//shit，原来是个顶点
							return;
						}

						Vector2 d = GetV2FromBorder(b);
						Vector2 test = GetV2(p0) - GetV2(b.from);
						
						//decimal t1 = (decimal)test.X / (decimal)d.X;
						//decimal t2 = (decimal)test.Y / (decimal)d.Y;
						
						//以下全错！
						long dx = (long)(test.X * 10000) / (long)(d.X * 10000);
						long dy = (long)(test.Y * 10000) / (long)(d.Y * 10000);
						double ddx = ((double)dx) / 10000;

						if (dx == dy && ddx > 0 && ddx < 1) //b.from、p0、b.to三点共线
						{						
							//判断Vector2.Ccw(b,n0)的符号，belong/neighbor得到polygon
							Vector2 vb = GetV2FromBorder(b);						
							int poly_idx;
							if (Vector2.Ccw(vb, n0) > 0)
								poly_idx = b.belong_poly;
							else
								poly_idx = b.neighbor_poly;

							return;
						}
					}

					//if (g.polygons[i].IsWithin(test_p))
					//    g.poly_idxs[i];

				}
			}
		}

		//========================================================================================================
		public void LOS_Engine(FPoint from, FPoint to)
		{
			int srcp = topo.SelectPolygon(from.x, from.y);
			int dstp = topo.SelectPolygon(to.x, to.y);

			if (srcp == -1 || dstp == -1) return;
			if (!topo.polys[srcp].enterable || !topo.polys[dstp].enterable) return;

			lfp.Add(from);

			if (srcp == dstp)
			{
				lfp.Add(to);
				return;
			}		

			Vector2 n0 = GetV2(from, to);
			n0.Normalize();

			TestBorderCall(from, n0, topo.polys[srcp]);
			do
			{
				switch (ret.result)
				{
					case RESULT.TEST_BORDER:
						TestVertexPolygon(ret.p0, ret.n0, ret.poly);
						break;
					case RESULT.TEST_VERTEX:
						TestPureVertex(ret.p0, ret.n0);
						break;
				}

				//以下是检测LOS是否可终止
				Polygon dst = topo.polys[dstp];

				switch (ret.result)
				{
					case RESULT.TEST_BORDER:
						if (ret.poly == dst)
						{
							lfp.Add(to);
							return;
						}
						break;
					case RESULT.TEST_VERTEX:
						FPoint fp = lfp[lfp.Count - 1];
						foreach (Border b in dst.borders)
						{
							if (fp.Equals(b.from))
							{
								lfp.Add(to);
								return;
							}
						}
						break;
				}
			} while (ret.result != RESULT.TEST_END);
		}

		public void Simple_LOS_Engine(FPoint p0, Vector2 n0)
		{
			TestVertexCall(p0, n0);
			do
			{
				switch (ret.result)
				{
					case RESULT.TEST_BORDER:
						TestVertexPolygon(ret.p0, ret.n0, ret.poly);
						break;
					case RESULT.TEST_VERTEX:
						TestPureVertex(ret.p0, ret.n0);
						break;
				}
			} while (ret.result != RESULT.TEST_END);
		}

		//========================================================================================================
		//1. p0位于poly之内，包括顶点上和边上
		//2. n0必须归一化	
		public ReturnInfo TestVertexPolygon(FPoint p0, Vector2 n0, Polygon poly)
		{
			Debug.Assert(poly.enterable);

			//测试1：相交于顶点否
			for (int i = 0; i < poly.num_borders; i++)
			{
				Border b = poly.borders[i];
				if (!b.to.Equals(p0))
				{
					Vector2 n1 = GetV2(b.to, p0);
					n1.Normalize();

					if (n0 == n1) //可靠否？
					{
						lfp.Add(b.to);

						return TestVertexCall(b.to, n0);
					}
				}
			}

			//测试2：相交于边否
			for (int i = 0; i < poly.num_borders; i++)
			{
				Border b = poly.borders[i];

				if (!b.from.Equals(p0) && !b.to.Equals(p0))
				{
					Vector2 from = GetV2(p0, b.from);
					Vector2 to = GetV2(p0, b.to);
					from.Normalize();
					to.Normalize();

					float ra = Vector2.Ccw(from, n0); //+Z轴面朝屏幕内
					float rb = Vector2.Ccw(to, n0);

					if (ra > 0 && rb < 0) //找到穿出边，该判断是正确的，看MSDN中的Vector2.Ccw释义
					{
						Vector2 p1 = CrossedRayLineSegIntersection(n0, GetV2(p0), GetV2(b.from), GetV2(b.to));
						lfp.Add(GetFP(p1));

						if (b.neighbor_poly != -1)
						{
							Polygon next_poly = topo.polys[b.neighbor_poly];
							if (next_poly.enterable)
								return TestBorderCall(GetFP(p1), n0, next_poly);
						}

						return TestEndCall();
					}
				}
			}

			Debug.Assert(false); //错误检测，不可能运行到这里
			return TestEndCall();
		}

		//========================================================================================================
		//前提：p0位于多边形顶点上
		public ReturnInfo TestPureVertex(FPoint p0, Vector2 n0)
		{
			int y_idx = (int)(p0.y / 64);
			int x_idx = (int)(p0.x / 64);
			Grid grid = topo.grids[y_idx, x_idx];

			//测试1：与某多边形的某边重合否
			for (int i = 0; i < grid.num_polys; i++)
			{
				Polygon p = grid.polygons[i];
				for (int j = 0; j < p.num_borders; j++)
				{
					Border b = p.borders[j];
					if (b.from.Equals(p0)) //远端就是b.to
					{
						Vector2 bn = GetV2FromBorder(b);
						bn.Normalize();

						if (bn == n0) //重合
						{
							Polygon neighbor_p = (b.neighbor_poly == -1) ? null : topo.polys[b.neighbor_poly];
							if (neighbor_p != null && !p.enterable && !neighbor_p.enterable) break; //排除一种情况：某边两侧的多边形均不可进入
							if (neighbor_p == null && !p.enterable) break;//排除一种情况：某边一侧的多边形不可进入，一侧不存在多边形

							lfp.Add(b.to);

							return TestVertexCall(b.to, n0);
						}

						break;
					}
				}
			}

			//测试2：(p0,n0)属于哪个多边形？
			for (int i = 0; i < grid.num_polys; i++)
			{
				Polygon p = grid.polygons[i];
				for (int j = 0; j < p.num_borders; j++)
				{
					Border b = p.borders[j];
					if (b.from.Equals(p0))
					{
						Border b1 = b;
						Border b2 = p.borders[(j + p.num_borders - 1) % p.num_borders];
						Debug.Assert(b1.from_idx == b2.to_idx);

						Vector2 vb1 = GetV2FromBorder(b1);
						Vector2 vb2 = -GetV2FromBorder(b2);
						vb1.Normalize();
						vb2.Normalize();

						float sb1 = Vector2.Ccw(n0, vb1);
						float sb2 = Vector2.Ccw(n0, vb2);

						if ((sb1 > 0 && sb2 < 0) || (sb1 < 0 && sb2 > 0))
						{
							float z = Vector2.Dot((vb1 + vb2), n0); //根据两边的角评分线与n0的方向来判断
							if (z > 0)
							{
								int p_idx = grid.poly_idxs[i]; //位于多边形grid.polygons_idx[i]内！
								Polygon p_ref = topo.polys[p_idx];

								if (p_ref.enterable)
									return TestBorderCall(p0, n0, p_ref);

								return TestEndCall();
							}
						}
					}
				}
			}

			//Debug.Assert(false); 
			return TestEndCall();
		}

		//========================================================================================================
		public Vector2 GetV2FromBorder(Border b)
		{
			float y = b.to.y - b.from.y;
			float x = b.to.x - b.from.x;

			return new Vector2(x, y);
		}

		public Vector2 GetV2(FPoint from, FPoint to)
		{
			return new Vector2(to.x - from.x, to.y - from.y);
		}

		public Vector2 GetV2(FPoint fp)
		{
			return new Vector2(fp.x, fp.y);
		}

		public FPoint GetFP(Vector2 v)
		{
			return new FPoint(v.X, v.Y);
		}

		//注意！这个函数有个假设前提，那就是输入的射线和线段必相交，函数本身不负责测试ray和line平行、重合的情况
		//明确的说，这个函数求的是两条线的交点，而没有考虑射线不射线的问题
		Vector2 CrossedRayLineSegIntersection(Vector2 n, Vector2 p, Vector2 from, Vector2 to)
		{
			//公式来自与Wordware - 3D Math Primer for Graphics and Game Development pp.282
			float a1, b1, d1;
			float a2, b2, d2;

			a1 = n.Y;
			b1 = -n.X;
			d1 = (p.X * n.Y) - (p.Y * n.X);

			a2 = (to.Y - from.Y);
			b2 = -(to.X - from.X);
			d2 = a2 * from.X + b2 * from.Y;

			//a1*x+b1*y=d1和a2*x+b2*y=d2联立并求解
			float x, y;
			x = (b2 * d1 - b1 * d2) / (a1 * b2 - a2 * b1);
			y = (a1 * d2 - a2 * d1) / (a1 * b2 - a2 * b1);

			return new Vector2(x, y);
		}
	}
}
