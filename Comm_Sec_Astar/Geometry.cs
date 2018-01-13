using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.DirectX;

namespace Comm_Sec_Astar
{
	class WayPoint
	{
		public Vector2 inter; //射出点
		public int poly_idx;  //该wp与上一wp之间所穿越的多边形编号

		public WayPoint() { }
		public WayPoint(Vector2 inter, int poly_idx)
		{
			this.inter = inter;
			this.poly_idx = poly_idx;
		}
	}

	enum LOS_RESULT { CLEAR, BLOCKED, FAILED };

	//Belong_Status.Polygon		: 位于某多边形中
	//Belong_Status.Edge		: 位于某边上 
	//Belong_Status.Vertex		: 位于某顶点上
	//Belong_Status.OutOfPolygon: 在某多边形之外
	//Belong_Status.Invalid		: 在导航网格的合法范围之外
	enum Belong_Status { Polygon, Edge, Vertex, OutOfPolygon, Invalid };

	class Geometry
	{
		FPoint[] points;
		Edge[] edges;
		Polygon[] polys;

		public Geometry(FPoint[] v, Edge[] e, Polygon[] p)
		{
			points = v;
			edges = e;
			polys = p;
		}
		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		#region 快速检测点处于哪个多边形中
		
		//快速检测点的位置
		//- retobj: 要么返回一个多边形，要么返回一条边，要么返回一个点
		//- 返回值: true，成功检测出了点的位置；false，点处于不合法位置
		public Belong_Status GetPointLocation(Vector2 p,out Object retobj)
		{
			retobj = null;
			Belong_Status ret = Belong_Status.Invalid;

			for (int i = 0; i < polys.Length; i++)
			{
				Polygon poly = polys[i];
				if ((p.X >= poly.min.X && p.X <= poly.max.X) &&
					(p.Y >= poly.min.Y && p.Y <= poly.max.Y)) 
				{
					Object obj;
					Belong_Status result = PointPolygonRelationshipTest(p, poly, out obj);

					if (result == Belong_Status.Polygon)
					{
						ret = result; retobj = obj;
						Debug.WriteLine(string.Format("(点位置检测) P:{0}", poly.idx));
						break;
					}
					else if (result == Belong_Status.Edge)
					{
						ret = result; retobj = obj;

						Edge e = (Edge)obj;
						Debug.WriteLine(string.Format("(点位置检测) P:{0} E:({1}-{2})", poly.idx, e.belong_poly, e.neighbor_poly));
						break;
					}
					else if (result == Belong_Status.Vertex)
					{
						ret = result; retobj = obj;

						FPoint pos = (FPoint)obj;
						Debug.WriteLine(string.Format("(点位置检测) P:{0} V:({1},{2})", poly.idx, pos.x, pos.y));
						break;
					}
				}
			}
			return ret;
		}

		//点与单个多边形的关系检测
		//- 如果点在多边形内部，retobj返回该多边形
		//- 如果点在多边形的边上，retobj返回该边
		//- 如果点在多边形的顶点上，retobj返回该点
		//- 如果点在多边形之外，retobj返回null
		public Belong_Status PointPolygonRelationshipTest(Vector2 pos, Polygon poly,out Object retobj)
		{
			//默认情况：点处于多边形以内
			retobj = poly;
			Belong_Status result = Belong_Status.Polygon;

			foreach (Edge e in poly.edges)
			{
				Vector2 between = new Vector2(e.to.x - e.from.x, e.to.y - e.from.y);
				Vector2 from = new Vector2(pos.X - e.from.x, pos.Y - e.from.y);

				float test = Vector2.Ccw(between, from);

				if (FloatEqual(test, 0))						//在边上或在顶点上
				{
					Vector2 to = new Vector2(pos.X - e.to.x, pos.Y - e.to.y);

					if (FloatEqual(from.LengthSq(), 0))			//在顶点e.from上
					{
						retobj = e.from;
						result = Belong_Status.Vertex;
					} 
					else if (FloatEqual(to.LengthSq(), 0))		//在顶点e.to上
					{
						retobj = e.to;
						result = Belong_Status.Vertex;
					}
					else										//在边上
					{
						retobj = e;
						result = Belong_Status.Edge;
					}
					break;
				}
				if (test < 0)									//点处于polygon之外
				{
					retobj = null;
					result = Belong_Status.OutOfPolygon;
					break; 
				}
			}
			return result;
		}
		#endregion
		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		#region LOS视线检测相关

		//---------------------------------------------------------------------------------------------------------
		//松弛疗法，微调pos，直到该点真正地处于某个多边形中为止
		//【遗留问题】没有考虑松弛过程中发生进入不可进入区域的情况
		Belong_Status RelaxPostion(ref Vector2 pos, out Polygon poly)
		{
			Vector2 p = pos;		

			Object obj = null;
			Belong_Status ret = GetPointLocation(p, out obj); //第一次
			
			if (ret == Belong_Status.Invalid)
			{
					poly = null;
					return ret; //不用松弛了，直接返回
			}

			Random rand = new Random((int)DateTime.Now.Ticks);
			bool ok = (ret == Belong_Status.Polygon);
			while (!ok)
			{
				p.X = pos.X + (float)(rand.NextDouble() - 0.5d) / 10f;
				p.Y = pos.Y + (float)(rand.NextDouble() - 0.5d) / 10f;
				ret = GetPointLocation(p, out obj);
				ok = (ret == Belong_Status.Polygon);
			}
			
			pos = p;
			poly = (Polygon)obj;
			return ret;
		}

		//---------------------------------------------------------------------------------------------------------
		//from:起点；to:终点；waypoint:所有视线途经位点
		public LOS_RESULT LOS_Test(Vector2 from, Vector2 to, out List<WayPoint> waypoint)
		{
			Belong_Status ret;
			Debug.WriteLine("----------------------------------------------------");
			
			////////////////////////////////////////////////////////////////////////////////////////
			Polygon dstpoly;
			Debug.Write("(目标) ");		
			ret = RelaxPostion(ref to, out dstpoly); //松弛目标点	
			if (ret != Belong_Status.Polygon || !dstpoly.enterable)
			{
				waypoint = null;
				return LOS_RESULT.FAILED;  //目标点不合法
			}

			int dstidx = dstpoly.idx; //关键！目标点所处的多边形
			
			////////////////////////////////////////////////////////////////////////////////////////
			Polygon srcpoly;
			Debug.Write("(源点) ");
			ret = RelaxPostion(ref from, out srcpoly); //松弛源点	
			if (ret != Belong_Status.Polygon || !srcpoly.enterable)
			{
				waypoint = null;
				return LOS_RESULT.FAILED; //源点不合法
			}
			
			////////////////////////////////////////////////////////////////////////////////////////
			w = new List<WayPoint>();
			w.Add(new WayPoint(from, -1)); //这是对的！
			PrintDebugPosition(-1, from);

			////////////////////////////////////////////////////////////////////////////////////////	
			Vector2 n0 = to - from;
			n0.Normalize();
			
			////////////////////////////////////////////////////////////////////////////////////////
			LOS_RESULT los=LOS_RESULT.FAILED;

			if (!FloatEqual(n0.LengthSq(),0))  //from/to两点不容许重合
			{
				Debug.Assert(ret == Belong_Status.Polygon);			
				los = StartLOS_P(n0, from, srcpoly, dstidx); //此刻from在srcpoly内部
			}

			if (los == LOS_RESULT.CLEAR)
			{
				w.Add(new WayPoint(to, dstidx));
				PrintDebugPosition(dstidx, to);
			}

			Debug.WriteLine(string.Format("({0}) 共计 {1} 个位点", los.ToString(),w.Count));
			Debug.WriteLine("----------------------------------------------------");

			waypoint = w;
			return los;
		}

		//---------------------------------------------------------------------------------------------------------
		List<WayPoint> w;

		//调用该函数时，在大多数情况下，p0在target的内部、边或点上
		LOS_RESULT StartLOS_P(Vector2 n0, Vector2 p0, Polygon target, int dstidx)
		{
			LOS_RESULT los = LOS_RESULT.FAILED;

			if (target.idx == dstidx) 
			{
				los = LOS_RESULT.CLEAR;
			}
			else //保证src与dst点不处于同一多边形
			{
				int i;
				for (i = 0; i < target.num_edges; i++)
				{
					Edge edge = target.edges[i];

					Vector2 from = edge.from.ToVector2() - p0;
					Vector2 to = edge.to.ToVector2() - p0;
					from.Normalize();
					to.Normalize();

					float ra = Vector2.Ccw(from, n0);
					float rb = Vector2.Ccw(to, n0);

					bool ta = FloatEqual(ra, 0); //V cross V = 零矢量 
					bool tb = FloatEqual(rb, 0);

					/*
					//特殊情况1：穿出视线与多边形某边完全重合，计算精度导致
					if (ta && tb) //from/to/p0-n0三者共线
					{
						if (edge.neighbor_poly == dstidx || edge.belong_poly==dstidx )
						{
							los = LOS_RESULT.CLEAR;
							break;
						}
						else
						{
							Vector2 pos = new Vector2();

							float da = Vector2.Dot(from, n0);
							float db = Vector2.Dot(to, n0);

							if (FloatEqual(da, 1))			//from与n0同向
								pos = edge.from.ToVector2();
							else if (FloatEqual(db, 1))		//to与n0同向
								pos = edge.to.ToVector2();
							else
								Debug.Assert(false);		//应该不可能

							w.Add(new WayPoint(pos, edge.belong_poly)); //最好取e.belong_poly，取e.neighbor_poly有可能==-1
							PrintDebugPosition(edge.belong_poly, pos);

							los = StartLOS_V(n0, pos, dstidx);
							break;
						}
					}

					//特殊情况2：穿出点正好是多边形的一个顶点
					//if (ta || tb) ：这句话导致死循环，它没有排除射入边的情况
					if ((ta && rb < 0) || (ra > 0 && tb)) //ta || tb 且保证边为穿出边
					{
						Vector2 pos = new Vector2();
						if (ta) pos = edge.from.ToVector2();
						if (tb) pos = edge.to.ToVector2();

						w.Add(new WayPoint(pos, edge.belong_poly)); //最好取e.belong_poly，取e.neighbor_poly有可能==-1
						PrintDebugPosition(edge.belong_poly, pos);

						los = StartLOS_V(n0, pos, dstidx);
						break;
					}
					*/

					//普通情况：穿出点在多边形的边上
					if (ra > 0 && rb < 0) //找到穿出边，即from在n0左边，to在n0右边
					{
						//在确信射线与穿出边相交后，求其交点
						Vector2 pos = CrossedRayLineSegIntersection(n0, p0, edge.from.ToVector2(), edge.to.ToVector2());

						w.Add(new WayPoint(pos, edge.belong_poly));
						PrintDebugPosition(edge.belong_poly, pos);

						if (!edge.wall) //穿出边是可以穿透的么？
						{
							Polygon newtarget = polys[edge.neighbor_poly];
							if (newtarget.idx == dstidx)
								los = LOS_RESULT.CLEAR;
							else
								los = StartLOS_P(n0, pos, newtarget, dstidx); //ok
						}
						else
							los = LOS_RESULT.BLOCKED;

						break; //只有一根穿出边，找到了就over了
					}
				}
				Debug.Assert(i != target.num_edges); //确信调用该函数时，p0始终在target的内部、边上或顶点上
			}

			return los;
		}

		//LOS_RESULT StartLOS_V(Vector2 n0,Vector2 p0, int dstidx)
		//{
		//    LOS_RESULT los = LOS_RESULT.FAILED;

		//    Vector2 pos = 0.005f * n0 + p0; //不得已的点探测，距离必须取得足够小(最小的地图为100*10O)
			
		//    Object obj;
		//    Belong_Status ret = GetPointLocation(pos, out obj); //检测探测点的位置

		//    switch (ret)
		//    {
		//        case Belong_Status.Invalid: //探测点处于不合法位置，LOST中止，ok
		//            los = LOS_RESULT.BLOCKED; //or failed?
		//            break;
		//        case Belong_Status.Polygon: //可能不精确：检测是下一个多边形了，继续LOST
		//            {
		//                Polygon target = (Polygon)obj;
		//                if (target.idx == dstidx)
		//                    los = LOS_RESULT.CLEAR; //已经达到目标多边形了，LOST中止
		//                else
		//                    los = StartLOS_P(n0, pos, target, dstidx); //之所以不用p0，是因为pos在多边形内部，保险一点
		//                break;
		//            }
		//        case Belong_Status.Edge:	//可能不精确：探测点处于一个边e上，且原来的点为一个顶点v
		//            {
		//                //现在的问题是：原来的点v一定是e的顶点吗？！由于点探测的不精确性，还有计算的精度，上述论断很难保证
		//                //这里只考虑了最普通的情况：取射线n0方向上e边远处的端点，然后继续顶点检测，即默认假设v是e的一个端点
		//                Edge e = (Edge)obj;
		//                if (e.belong_poly == dstidx || e.neighbor_poly == dstidx)
		//                {
		//                    los = LOS_RESULT.CLEAR;
		//                }
		//                else
		//                {
		//                    Vector2 between = new Vector2(e.to.x - e.from.x, e.to.y - e.from.y);
		//                    between.Normalize();
		//                    float test = Vector2.Dot(between, n0);

		//                    Vector2 p = new Vector2();
		//                    if (FloatEqual(test, 1))		// 1: to-from与n0同向
		//                        p = e.to.ToVector2();
		//                    else if (FloatEqual(test, -1))	//-1: to-from与n0反向
		//                        p = e.from.ToVector2();
		//                    else							//一般不应该发生的小概率事件,如果确实发生了，那只可能是点探测不准确或精度误差所致
		//                    {
		//                        Debug.WriteLine("[不精确事件A]");
		//                        los = StartLOS_V(n0, pos, dstidx);	//容错....继续探测...【遗留问题】万一POS越过了目标点怎么办？
		//                        break;
		//                    }

		//                    //【遗留问题】最好验证一下，不存在e.belong_poly为-1的情况
		//                    w.Add(new WayPoint(p, e.belong_poly)); //只能取e.belong_poly，取e.neighbor_poly有可能==-1
		//                    PrintDebugPosition(e.belong_poly, pos);
		//                    los = StartLOS_V(n0, p, dstidx);
		//                }
		//                break;
		//            }
		//        case Belong_Status.Vertex:	//绝对不精确：小概率事件，一次点探测之后再次碰到另一个顶点，只能采取不精确处理，继续探测
		//            {
		//                Debug.WriteLine("[不精确事件B]");
		//                los = StartLOS_V(n0, pos, dstidx); //【遗留问题】万一POS越过了目标点怎么办？
		//                break;
		//            }
		//    }

		//    return los;
		//}
	
		//---------------------------------------------------------------------------------------------------------
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
		
		//---------------------------------------------------------------------------------------------------------
		#endregion
		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		#region 计算多边形面积与质心
		public void UpdateAllCentroid()
		{
			for (int i = 0; i < polys.Length; i++)
				polys[i].centroid = GetPolyonCentroid(i);
		}
		
		//计算多边形面积
		float GetPolygonArea(int idx)
		{
			Polygon d = polys[idx];

			float result = 0.0F;
			for (int i = 0; i < d.num_edges; i++)
			{
				Edge b = d.edges[i];
				result += (b.to.x - b.from.x) * (b.to.y + b.from.y) / 2;
			}

			return Math.Abs(result);
		}

		//计算多边形质心位置
		FPoint GetPolyonCentroid(int idx)
		{
			float area = GetPolygonArea(idx);

			FPoint fp = new FPoint();

			Polygon d = polys[idx];
			for (int i = 0; i < d.num_edges; i++)
			{
				Edge b = d.edges[i];
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
		#endregion
		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		#region 常用函数
		bool FloatEqual(float a, float b)
		{
			return ((a > (b - 0.00001f)) && (a < (b + 0.00001f)));
			//return ((a >= (b - float.Epsilon)) && (a <= (b + float.Epsilon))); //必须是>=和<=，否则a==b==1.0时不正确
		}

		void PrintDebugPosition(int polyidx,Vector2 pos)
		{
			Debug.WriteLine(string.Format("[{0}] \t X:{1} \t Y:{2}", polyidx, pos.X, pos.Y));
		}

		Vector3 ConvertV2toV3(Vector2 v2)
		{
			return new Vector3(v2.X, v2.Y, 0);
		}

		Vector2 ConvertV3toV2(Vector3 v3)
		{
			return new Vector2(v3.X, v3.Y); //直接丢弃Z
		}
		#endregion
	}
}
