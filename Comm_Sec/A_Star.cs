using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Comm_Sec
{
	/////////////////////////////////////////////////////////////////////////////////////////////////////
	enum WaypointType { Normal, EdgePoint, Vertex };

	class Waypoint
	{
		public bool done; //��־�����Ա����Ƿ���Ŀ��λ��ֱ������

		public WaypointType type; //���Ա���ʹ�ö�̬��̫��

		public FPoint fp;
		public float G, H, F;
		public Waypoint parent;

		//-----------------------------------------------------------------------------------------------
		public Waypoint(FPoint fp, Waypoint parent, bool done)
		{
			this.type = WaypointType.Normal;
			this.done = done;

			this.parent = parent;
			this.fp = fp.Clone();

			if (parent == null)
				G = 0;
			else
				G = parent.G + fp.Distance(parent.fp);

			H = fp.Distance(fp_dst);

			F = G + H;
		}

		//-----------------------------------------------------------------------------------------------
		static FPoint fp_dst; //������...
		static public void SetDest(FPoint dst)
		{
			fp_dst = dst.Clone();
		}
	}

	//===============================================================================================
	class EdgeWaypoint : Waypoint
	{
		public Border border;

		public EdgeWaypoint(FPoint fp, Waypoint parent, Border border, bool done)
			: base(fp, parent, done)
		{
			this.type = WaypointType.EdgePoint;
			this.border = border;
		}
	}

	//===============================================================================================
	class VertexWaypoint : Waypoint
	{
		public Grid grid;

		public VertexWaypoint(FPoint fp, Waypoint parent, Grid grid, bool done)
			: base(fp, parent, done)
		{
			this.type = WaypointType.Vertex;
			this.grid = grid;
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////
	class WorkList
	{
		SortedDictionary<float, List<Waypoint>> fmap;	//����F->λ���б�:	multi-set �ɷ񣿣�
		SortedDictionary<FPoint, Waypoint> pmap;		//λ��pos->λ��:	unique-set

		public WorkList()
		{
			fmap = new SortedDictionary<float, List<Waypoint>>();
			pmap = new SortedDictionary<FPoint, Waypoint>();
		}

		public void Clear()
		{
			fmap.Clear();
			pmap.Clear();
		}

		public void Add(Waypoint w)
		{
			List<Waypoint> list;
			if (!fmap.ContainsKey(w.F))
			{
				list = new List<Waypoint>();
				fmap[w.F] = list;
			}
			else
			{
				list = fmap[w.F];
			}

			list.Add(w);

			FPoint pos = w.fp.Clone();
			pmap[pos] = w;
		}

		public void Delete(Waypoint w)
		{
			List<Waypoint> list;
			Debug.Assert(fmap.ContainsKey(w.F));

			list = fmap[w.F];
			int idx;
			for (idx = 0; idx < list.Count; idx++) //��...
			{
				if (list[idx].fp.Equals(w.fp))
					break;
			}
			Debug.Assert(idx != list.Count);

			list.RemoveAt(idx); //��...

			if (list.Count == 0)
				fmap.Remove(w.F);

			FPoint pos = new FPoint(w.fp.x, w.fp.y);
			pmap.Remove(pos); //ֵ�Ƚ�
		}

		public Waypoint GetLeastWaypoint() //��...ȡ����Сֵ��ô�鷳
		{
			float min = 0;
			Debug.Assert(fmap.Keys.Count != 0);

			foreach (float temp in fmap.Keys)
			{
				min = temp;
				break;
			}

			List<Waypoint> list = fmap[min];
			return list[0]; //��0����list.Count-1
		}

		public Waypoint GetWaypointFromXY(FPoint key) //OpenList��
		{
			Waypoint value = null;
			pmap.TryGetValue(key, out value);
			return value;
		}

		public bool IsEmpty()
		{
			return (pmap.Count == 0);
		}

		public bool IsContainsPos(FPoint key) //CloseList��
		{
			return pmap.ContainsKey(key);
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////
	class PathFinder
	{
		DisplayTopology topo;
		Polygon[] polys;

		//===============================================================================================
		WorkList open;
		WorkList close;

		//===============================================================================================
		public PathFinder(DisplayTopology topology)
		{
			topo = topology;
			polys = topo.polys;

			open = new WorkList();
			close = new WorkList();
		}

		//===============================================================================================
		Polygon dst_poly;	//Ŀ�����Σ������ж�EdgeWaypoint�Ƿ���Ŀ���ֱ������
		int[] dst_fpidx;	//Ŀ�����εĶ��������������ж�VertexWaypoint�Ƿ���Ŀ���ֱ������

		public Waypoint PathFinding(FPoint srcfp, FPoint dstfp)
		{
			//��㡢�յ���벻Ϊ��
			if (srcfp == null || dstfp == null) return null;

			//��֤��㡢�յ����ڶ���εĺϷ���
			int srcp = topo.SelectPolygon(srcfp.x, srcfp.y);
			int dstp = topo.SelectPolygon(dstfp.x, dstfp.y);
			if (srcp == -1 || dstp == -1 || !polys[srcp].enterable || !polys[dstp].enterable) return null;

			//����·���滮�յ�
			Waypoint.SetDest(dstfp);

			//����Ŀ������
			dst_poly = polys[dstp];

			//���Ŀ�����εĶ���
			dst_fpidx = new int[dst_poly.num_borders];
			for (int i = 0; i < dst_poly.num_borders; i++)
				dst_fpidx[i] = dst_poly.borders[i].from_idx;

			//----------------------------------------------------------------------------
			open.Clear();
			close.Clear();

			Waypoint start = new Waypoint(srcfp, null, false);
			close.Add(start); //!!!��֤open����ֻ��edgew��vertexw����λ��

			if (srcp == dstp)
			{
				Waypoint end = new Waypoint(dstfp, start, true);
				return end;
			}
			else
			{
				ExtendStartWaypoint(start, srcp);
			}

			//----------------------------------------------------------------------------
			Waypoint w;
			while (!open.IsEmpty())
			{
				w = open.GetLeastWaypoint();

				if (w.done) //���w��Ŀ���ֱ������
				{
					Waypoint end = new Waypoint(dstfp, w, true);
					return end;
				}

				open.Delete(w);
				close.Add(w);

				ExtendAdjacentWaypoints(w);
			}

			return null;
		}

		//===============================================================================================
		void ExtendAdjacentWaypoints(Waypoint w)
		{
			switch (w.type)
			{
				case WaypointType.EdgePoint:
					ExtendEdgeWaypoint((EdgeWaypoint)w);
					break;
				case WaypointType.Vertex:
					ExtendVertexWaypoint((VertexWaypoint)w);
					break;
				default:
					Debug.Assert(false);
					break;
			}
		}

		void ExtendStartWaypoint(Waypoint start, int srcp) //srcp: ��ʼ�������Ķ����
		{
			Polygon p = polys[srcp];
			Debug.Assert(p != dst_poly);	

			foreach (Border b in p.borders)
			{
				FPoint fp = b.GetMid();
				PushEdgeWaypoint(fp, b, start);

				PushVertexWaypoint(b.from, b.from_idx, start);
			}
		}

		void ExtendEdgeWaypoint(EdgeWaypoint w) //��չ���е�
		{
			int p1 = (w.border == null) ? -1 : w.border.belong_poly;
			int p2 = (w.border == null) ? -1 : w.border.neighbor_poly;

			if (p1 != -1 && polys[p1].enterable)
			{
				Polygon p = polys[p1];
				foreach (Border b in p.borders)
				{
					FPoint fp = b.GetMid();
					PushEdgeWaypoint(fp, b, w);

					PushVertexWaypoint(b.from, b.from_idx, w);
				}
			}

			if (p2 != -1 && polys[p2].enterable)
			{
				Polygon p = polys[p2];
				foreach (Border b in p.borders)
				{
					FPoint fp = b.GetMid();
					PushEdgeWaypoint(fp, b, w);

					PushVertexWaypoint(b.from, b.from_idx, w);
				}
			}
		}

		void ExtendVertexWaypoint(VertexWaypoint w)
		{
			//�жϸö���������Щ�����		
			List<Polygon> l = new List<Polygon>(8);
			foreach (Polygon p in w.grid.polygons)
			{
				if (p.enterable)
				{
					foreach (Border b in p.borders)
					{
						if (w.fp.Equals(b.from))
						{
							l.Add(p);
							break;
						}
					}
				}
			}

			//��һ����Щ����εĶ���ͱ��е�����
			foreach (Polygon p in l)
			{
				foreach (Border b in p.borders)
				{
					FPoint fp = b.GetMid();
					PushEdgeWaypoint(fp, b, w);

					PushVertexWaypoint(b.from, b.from_idx, w);
				}
			}
		}

		//===============================================================================================
		void PushEdgeWaypoint(FPoint fp, Border border, Waypoint parent) //ƾ��border�����ж�done��־
		{
			//��Close���У��˳�
			if (close.IsContainsPos(fp)) return;

			//��Open���з�
			Waypoint w = open.GetWaypointFromXY(fp);
			if (w != null)
			{
				//��Open���У����Ż���
				float d = fp.Distance(parent.fp);

				if (w.G > parent.G + d) //���Ż�
				{
					open.Delete(w);

					w.parent = parent;
					w.G = parent.G + d;
					w.F = w.G + w.H;

					open.Add(w);
				}
			}
			else
			{
				//����Open����
				int p1 = border.belong_poly;
				int p2 = border.neighbor_poly;
				Polygon po1 = (p1 == -1) ? null : polys[p1];
				Polygon po2 = (p2 == -1) ? null : polys[p2];
			
				bool done = (po1 == dst_poly || po2 == dst_poly) ? true : false;
				w = new EdgeWaypoint(fp, parent, border, done);

				open.Add(w);
			}
		}

		void PushVertexWaypoint(FPoint fp, int fp_idx, Waypoint parent) //ƾ��fp_idx�����ж�done��־
		{
			//��Close���У��˳�
			if (close.IsContainsPos(fp)) return;

			//��Open���з�
			Waypoint w = open.GetWaypointFromXY(fp);
			if (w != null)
			{
				//��Open���У����Ż���
				float d = fp.Distance(parent.fp);

				if (w.G > parent.G + d) //���Ż�
				{
					open.Delete(w);

					w.parent = parent;
					w.G = parent.G + d;
					w.F = w.G + w.H;

					open.Add(w);
				}
			}
			else
			{
				//����Open����
				int y_idx = (int)(fp.y / 64);
				int x_idx = (int)(fp.x / 64);
				Grid grid = topo.grids[y_idx, x_idx];

				bool done = false;
				foreach (int idx in dst_fpidx) //�᲻����?...
				{
					if (fp_idx == idx)
					{
						done = true;
						break;
					}
				}

				w = new VertexWaypoint(fp, parent, grid, done);

				open.Add(w);
			}
		}
	}
}