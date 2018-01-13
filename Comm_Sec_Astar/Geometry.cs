using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.DirectX;

namespace Comm_Sec_Astar
{
	class WayPoint
	{
		public Vector2 inter; //�����
		public int poly_idx;  //��wp����һwp֮������Խ�Ķ���α��

		public WayPoint() { }
		public WayPoint(Vector2 inter, int poly_idx)
		{
			this.inter = inter;
			this.poly_idx = poly_idx;
		}
	}

	enum LOS_RESULT { CLEAR, BLOCKED, FAILED };

	//Belong_Status.Polygon		: λ��ĳ�������
	//Belong_Status.Edge		: λ��ĳ���� 
	//Belong_Status.Vertex		: λ��ĳ������
	//Belong_Status.OutOfPolygon: ��ĳ�����֮��
	//Belong_Status.Invalid		: �ڵ�������ĺϷ���Χ֮��
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
		#region ���ټ��㴦���ĸ��������
		
		//���ټ����λ��
		//- retobj: Ҫô����һ������Σ�Ҫô����һ���ߣ�Ҫô����һ����
		//- ����ֵ: true���ɹ������˵��λ�ã�false���㴦�ڲ��Ϸ�λ��
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
						Debug.WriteLine(string.Format("(��λ�ü��) P:{0}", poly.idx));
						break;
					}
					else if (result == Belong_Status.Edge)
					{
						ret = result; retobj = obj;

						Edge e = (Edge)obj;
						Debug.WriteLine(string.Format("(��λ�ü��) P:{0} E:({1}-{2})", poly.idx, e.belong_poly, e.neighbor_poly));
						break;
					}
					else if (result == Belong_Status.Vertex)
					{
						ret = result; retobj = obj;

						FPoint pos = (FPoint)obj;
						Debug.WriteLine(string.Format("(��λ�ü��) P:{0} V:({1},{2})", poly.idx, pos.x, pos.y));
						break;
					}
				}
			}
			return ret;
		}

		//���뵥������εĹ�ϵ���
		//- ������ڶ�����ڲ���retobj���ظö����
		//- ������ڶ���εı��ϣ�retobj���ظñ�
		//- ������ڶ���εĶ����ϣ�retobj���ظõ�
		//- ������ڶ����֮�⣬retobj����null
		public Belong_Status PointPolygonRelationshipTest(Vector2 pos, Polygon poly,out Object retobj)
		{
			//Ĭ��������㴦�ڶ��������
			retobj = poly;
			Belong_Status result = Belong_Status.Polygon;

			foreach (Edge e in poly.edges)
			{
				Vector2 between = new Vector2(e.to.x - e.from.x, e.to.y - e.from.y);
				Vector2 from = new Vector2(pos.X - e.from.x, pos.Y - e.from.y);

				float test = Vector2.Ccw(between, from);

				if (FloatEqual(test, 0))						//�ڱ��ϻ��ڶ�����
				{
					Vector2 to = new Vector2(pos.X - e.to.x, pos.Y - e.to.y);

					if (FloatEqual(from.LengthSq(), 0))			//�ڶ���e.from��
					{
						retobj = e.from;
						result = Belong_Status.Vertex;
					} 
					else if (FloatEqual(to.LengthSq(), 0))		//�ڶ���e.to��
					{
						retobj = e.to;
						result = Belong_Status.Vertex;
					}
					else										//�ڱ���
					{
						retobj = e;
						result = Belong_Status.Edge;
					}
					break;
				}
				if (test < 0)									//�㴦��polygon֮��
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
		#region LOS���߼�����

		//---------------------------------------------------------------------------------------------------------
		//�ɳ��Ʒ���΢��pos��ֱ���õ������ش���ĳ���������Ϊֹ
		//���������⡿û�п����ɳڹ����з������벻�ɽ�����������
		Belong_Status RelaxPostion(ref Vector2 pos, out Polygon poly)
		{
			Vector2 p = pos;		

			Object obj = null;
			Belong_Status ret = GetPointLocation(p, out obj); //��һ��
			
			if (ret == Belong_Status.Invalid)
			{
					poly = null;
					return ret; //�����ɳ��ˣ�ֱ�ӷ���
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
		//from:��㣻to:�յ㣻waypoint:��������;��λ��
		public LOS_RESULT LOS_Test(Vector2 from, Vector2 to, out List<WayPoint> waypoint)
		{
			Belong_Status ret;
			Debug.WriteLine("----------------------------------------------------");
			
			////////////////////////////////////////////////////////////////////////////////////////
			Polygon dstpoly;
			Debug.Write("(Ŀ��) ");		
			ret = RelaxPostion(ref to, out dstpoly); //�ɳ�Ŀ���	
			if (ret != Belong_Status.Polygon || !dstpoly.enterable)
			{
				waypoint = null;
				return LOS_RESULT.FAILED;  //Ŀ��㲻�Ϸ�
			}

			int dstidx = dstpoly.idx; //�ؼ���Ŀ��������Ķ����
			
			////////////////////////////////////////////////////////////////////////////////////////
			Polygon srcpoly;
			Debug.Write("(Դ��) ");
			ret = RelaxPostion(ref from, out srcpoly); //�ɳ�Դ��	
			if (ret != Belong_Status.Polygon || !srcpoly.enterable)
			{
				waypoint = null;
				return LOS_RESULT.FAILED; //Դ�㲻�Ϸ�
			}
			
			////////////////////////////////////////////////////////////////////////////////////////
			w = new List<WayPoint>();
			w.Add(new WayPoint(from, -1)); //���ǶԵģ�
			PrintDebugPosition(-1, from);

			////////////////////////////////////////////////////////////////////////////////////////	
			Vector2 n0 = to - from;
			n0.Normalize();
			
			////////////////////////////////////////////////////////////////////////////////////////
			LOS_RESULT los=LOS_RESULT.FAILED;

			if (!FloatEqual(n0.LengthSq(),0))  //from/to���㲻�����غ�
			{
				Debug.Assert(ret == Belong_Status.Polygon);			
				los = StartLOS_P(n0, from, srcpoly, dstidx); //�˿�from��srcpoly�ڲ�
			}

			if (los == LOS_RESULT.CLEAR)
			{
				w.Add(new WayPoint(to, dstidx));
				PrintDebugPosition(dstidx, to);
			}

			Debug.WriteLine(string.Format("({0}) ���� {1} ��λ��", los.ToString(),w.Count));
			Debug.WriteLine("----------------------------------------------------");

			waypoint = w;
			return los;
		}

		//---------------------------------------------------------------------------------------------------------
		List<WayPoint> w;

		//���øú���ʱ���ڴ��������£�p0��target���ڲ����߻����
		LOS_RESULT StartLOS_P(Vector2 n0, Vector2 p0, Polygon target, int dstidx)
		{
			LOS_RESULT los = LOS_RESULT.FAILED;

			if (target.idx == dstidx) 
			{
				los = LOS_RESULT.CLEAR;
			}
			else //��֤src��dst�㲻����ͬһ�����
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

					bool ta = FloatEqual(ra, 0); //V cross V = ��ʸ�� 
					bool tb = FloatEqual(rb, 0);

					/*
					//�������1����������������ĳ����ȫ�غϣ����㾫�ȵ���
					if (ta && tb) //from/to/p0-n0���߹���
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

							if (FloatEqual(da, 1))			//from��n0ͬ��
								pos = edge.from.ToVector2();
							else if (FloatEqual(db, 1))		//to��n0ͬ��
								pos = edge.to.ToVector2();
							else
								Debug.Assert(false);		//Ӧ�ò�����

							w.Add(new WayPoint(pos, edge.belong_poly)); //���ȡe.belong_poly��ȡe.neighbor_poly�п���==-1
							PrintDebugPosition(edge.belong_poly, pos);

							los = StartLOS_V(n0, pos, dstidx);
							break;
						}
					}

					//�������2�������������Ƕ���ε�һ������
					//if (ta || tb) ����仰������ѭ������û���ų�����ߵ����
					if ((ta && rb < 0) || (ra > 0 && tb)) //ta || tb �ұ�֤��Ϊ������
					{
						Vector2 pos = new Vector2();
						if (ta) pos = edge.from.ToVector2();
						if (tb) pos = edge.to.ToVector2();

						w.Add(new WayPoint(pos, edge.belong_poly)); //���ȡe.belong_poly��ȡe.neighbor_poly�п���==-1
						PrintDebugPosition(edge.belong_poly, pos);

						los = StartLOS_V(n0, pos, dstidx);
						break;
					}
					*/

					//��ͨ������������ڶ���εı���
					if (ra > 0 && rb < 0) //�ҵ������ߣ���from��n0��ߣ�to��n0�ұ�
					{
						//��ȷ�������봩�����ཻ�����佻��
						Vector2 pos = CrossedRayLineSegIntersection(n0, p0, edge.from.ToVector2(), edge.to.ToVector2());

						w.Add(new WayPoint(pos, edge.belong_poly));
						PrintDebugPosition(edge.belong_poly, pos);

						if (!edge.wall) //�������ǿ��Դ�͸��ô��
						{
							Polygon newtarget = polys[edge.neighbor_poly];
							if (newtarget.idx == dstidx)
								los = LOS_RESULT.CLEAR;
							else
								los = StartLOS_P(n0, pos, newtarget, dstidx); //ok
						}
						else
							los = LOS_RESULT.BLOCKED;

						break; //ֻ��һ�������ߣ��ҵ��˾�over��
					}
				}
				Debug.Assert(i != target.num_edges); //ȷ�ŵ��øú���ʱ��p0ʼ����target���ڲ������ϻ򶥵���
			}

			return los;
		}

		//LOS_RESULT StartLOS_V(Vector2 n0,Vector2 p0, int dstidx)
		//{
		//    LOS_RESULT los = LOS_RESULT.FAILED;

		//    Vector2 pos = 0.005f * n0 + p0; //�����ѵĵ�̽�⣬�������ȡ���㹻С(��С�ĵ�ͼΪ100*10O)
			
		//    Object obj;
		//    Belong_Status ret = GetPointLocation(pos, out obj); //���̽����λ��

		//    switch (ret)
		//    {
		//        case Belong_Status.Invalid: //̽��㴦�ڲ��Ϸ�λ�ã�LOST��ֹ��ok
		//            los = LOS_RESULT.BLOCKED; //or failed?
		//            break;
		//        case Belong_Status.Polygon: //���ܲ���ȷ���������һ��������ˣ�����LOST
		//            {
		//                Polygon target = (Polygon)obj;
		//                if (target.idx == dstidx)
		//                    los = LOS_RESULT.CLEAR; //�Ѿ��ﵽĿ�������ˣ�LOST��ֹ
		//                else
		//                    los = StartLOS_P(n0, pos, target, dstidx); //֮���Բ���p0������Ϊpos�ڶ�����ڲ�������һ��
		//                break;
		//            }
		//        case Belong_Status.Edge:	//���ܲ���ȷ��̽��㴦��һ����e�ϣ���ԭ���ĵ�Ϊһ������v
		//            {
		//                //���ڵ������ǣ�ԭ���ĵ�vһ����e�Ķ����𣿣����ڵ�̽��Ĳ���ȷ�ԣ����м���ľ��ȣ������۶Ϻ��ѱ�֤
		//                //����ֻ����������ͨ�������ȡ����n0������e��Զ���Ķ˵㣬Ȼ����������⣬��Ĭ�ϼ���v��e��һ���˵�
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
		//                    if (FloatEqual(test, 1))		// 1: to-from��n0ͬ��
		//                        p = e.to.ToVector2();
		//                    else if (FloatEqual(test, -1))	//-1: to-from��n0����
		//                        p = e.from.ToVector2();
		//                    else							//һ�㲻Ӧ�÷�����С�����¼�,���ȷʵ�����ˣ���ֻ�����ǵ�̽�ⲻ׼ȷ�򾫶��������
		//                    {
		//                        Debug.WriteLine("[����ȷ�¼�A]");
		//                        los = StartLOS_V(n0, pos, dstidx);	//�ݴ�....����̽��...���������⡿��һPOSԽ����Ŀ�����ô�죿
		//                        break;
		//                    }

		//                    //���������⡿�����֤һ�£�������e.belong_polyΪ-1�����
		//                    w.Add(new WayPoint(p, e.belong_poly)); //ֻ��ȡe.belong_poly��ȡe.neighbor_poly�п���==-1
		//                    PrintDebugPosition(e.belong_poly, pos);
		//                    los = StartLOS_V(n0, p, dstidx);
		//                }
		//                break;
		//            }
		//        case Belong_Status.Vertex:	//���Բ���ȷ��С�����¼���һ�ε�̽��֮���ٴ�������һ�����㣬ֻ�ܲ�ȡ����ȷ��������̽��
		//            {
		//                Debug.WriteLine("[����ȷ�¼�B]");
		//                los = StartLOS_V(n0, pos, dstidx); //���������⡿��һPOSԽ����Ŀ�����ô�죿
		//                break;
		//            }
		//    }

		//    return los;
		//}
	
		//---------------------------------------------------------------------------------------------------------
		//ע�⣡��������и�����ǰ�ᣬ�Ǿ�����������ߺ��߶α��ཻ�����������������ray��lineƽ�С��غϵ����
		//��ȷ��˵�������������������ߵĽ��㣬��û�п������߲����ߵ�����
		Vector2 CrossedRayLineSegIntersection(Vector2 n, Vector2 p, Vector2 from, Vector2 to)
		{
			//��ʽ������Wordware - 3D Math Primer for Graphics and Game Development pp.282
			float a1, b1, d1;
			float a2, b2, d2;

			a1 = n.Y;
			b1 = -n.X;
			d1 = (p.X * n.Y) - (p.Y * n.X);

			a2 = (to.Y - from.Y);
			b2 = -(to.X - from.X);
			d2 = a2 * from.X + b2 * from.Y;

			//a1*x+b1*y=d1��a2*x+b2*y=d2���������
			float x, y;
			x = (b2 * d1 - b1 * d2) / (a1 * b2 - a2 * b1);
			y = (a1 * d2 - a2 * d1) / (a1 * b2 - a2 * b1);

			return new Vector2(x, y);
		}
		
		//---------------------------------------------------------------------------------------------------------
		#endregion
		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		#region �����������������
		public void UpdateAllCentroid()
		{
			for (int i = 0; i < polys.Length; i++)
				polys[i].centroid = GetPolyonCentroid(i);
		}
		
		//�����������
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

		//������������λ��
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
		#region ���ú���
		bool FloatEqual(float a, float b)
		{
			return ((a > (b - 0.00001f)) && (a < (b + 0.00001f)));
			//return ((a >= (b - float.Epsilon)) && (a <= (b + float.Epsilon))); //������>=��<=������a==b==1.0ʱ����ȷ
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
			return new Vector2(v3.X, v3.Y); //ֱ�Ӷ���Z
		}
		#endregion
	}
}
