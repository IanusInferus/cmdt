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
		public bool done; //标志：用以表征是否与目标位置直接相邻

		public WaypointType type; //用以避免使用多态，太慢

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
		static FPoint fp_dst; //颇郁闷...
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
		SortedDictionary<float, List<Waypoint>> fmap;	//代价F->位点列表:	multi-set 可否？？
		SortedDictionary<FPoint, Waypoint> pmap;		//位置pos->位点:	unique-set

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
			for (idx = 0; idx < list.Count; idx++) //慢...
			{
				if (list[idx].fp.Equals(w.fp))
					break;
			}
			Debug.Assert(idx != list.Count);

			list.RemoveAt(idx); //慢...

			if (list.Count == 0)
				fmap.Remove(w.F);

			FPoint pos = new FPoint(w.fp.x, w.fp.y);
			pmap.Remove(pos); //值比较
		}

		public Waypoint GetLeastWaypoint() //日...取个最小值这么麻烦
		{
			float min = 0;
			Debug.Assert(fmap.Keys.Count != 0);

			foreach (float temp in fmap.Keys)
			{
				min = temp;
				break;
			}

			List<Waypoint> list = fmap[min];
			return list[0]; //是0还是list.Count-1
		}

		public Waypoint GetWaypointFromXY(FPoint key) //OpenList用
		{
			Waypoint value = null;
			pmap.TryGetValue(key, out value);
			return value;
		}

		public bool IsEmpty()
		{
			return (pmap.Count == 0);
		}

		public bool IsContainsPos(FPoint key) //CloseList用
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
		Polygon dst_poly;	//目标多边形，用于判断EdgeWaypoint是否与目标点直接相邻
		int[] dst_fpidx;	//目标多边形的顶点索引，用于判断VertexWaypoint是否与目标点直接相邻

		public Waypoint PathFinding(FPoint srcfp, FPoint dstfp)
		{
			//起点、终点必须不为空
			if (srcfp == null || dstfp == null) return null;

			//验证起点、终点所在多边形的合法性
			int srcp = topo.SelectPolygon(srcfp.x, srcfp.y);
			int dstp = topo.SelectPolygon(dstfp.x, dstfp.y);
			if (srcp == -1 || dstp == -1 || !polys[srcp].enterable || !polys[dstp].enterable) return null;

			//设置路径规划终点
			Waypoint.SetDest(dstfp);

			//设置目标多边形
			dst_poly = polys[dstp];

			//标记目标多边形的顶点
			dst_fpidx = new int[dst_poly.num_borders];
			for (int i = 0; i < dst_poly.num_borders; i++)
				dst_fpidx[i] = dst_poly.borders[i].from_idx;

			//----------------------------------------------------------------------------
			open.Clear();
			close.Clear();

			Waypoint start = new Waypoint(srcfp, null, false);
			close.Add(start); //!!!保证open里面只有edgew和vertexw两类位点

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

				if (w.done) //如果w与目标点直接相邻
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

		void ExtendStartWaypoint(Waypoint start, int srcp) //srcp: 起始点所属的多边形
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

		void ExtendEdgeWaypoint(EdgeWaypoint w) //扩展边中点
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
			//判断该顶点属于哪些多边形		
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

			//逐一将这些多边形的顶点和边中点添入
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
		void PushEdgeWaypoint(FPoint fp, Border border, Waypoint parent) //凭借border可以判断done标志
		{
			//在Close表中，退出
			if (close.IsContainsPos(fp)) return;

			//在Open表中否？
			Waypoint w = open.GetWaypointFromXY(fp);
			if (w != null)
			{
				//在Open表中，可优化否？
				float d = fp.Distance(parent.fp);

				if (w.G > parent.G + d) //可优化
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
				//不在Open表中
				int p1 = border.belong_poly;
				int p2 = border.neighbor_poly;
				Polygon po1 = (p1 == -1) ? null : polys[p1];
				Polygon po2 = (p2 == -1) ? null : polys[p2];
			
				bool done = (po1 == dst_poly || po2 == dst_poly) ? true : false;
				w = new EdgeWaypoint(fp, parent, border, done);

				open.Add(w);
			}
		}

		void PushVertexWaypoint(FPoint fp, int fp_idx, Waypoint parent) //凭借fp_idx可以判断done标志
		{
			//在Close表中，退出
			if (close.IsContainsPos(fp)) return;

			//在Open表中否？
			Waypoint w = open.GetWaypointFromXY(fp);
			if (w != null)
			{
				//在Open表中，可优化否？
				float d = fp.Distance(parent.fp);

				if (w.G > parent.G + d) //可优化
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
				//不在Open表中
				int y_idx = (int)(fp.y / 64);
				int x_idx = (int)(fp.x / 64);
				Grid grid = topo.grids[y_idx, x_idx];

				bool done = false;
				foreach (int idx in dst_fpidx) //会不会慢?...
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