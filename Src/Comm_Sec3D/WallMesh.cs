using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;

using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

namespace Comm_Sec3D
{
	////////////////////////////////////////////////////////////////////////////////////
	//垂直平面网格
	class WallMesh
	{
		Sec sec;
		
		////////////////////////////////////////////////////////////////////////////////////
		Color color;
		short[] idxbuf;									//面集合(索引),只能定义成short[]
		CustomVertex.PositionNormalColored[] vexbuf;	//顶点集合
		
		////////////////////////////////////////////////////////////////////////////////////
		public Mesh mesh;

		////////////////////////////////////////////////////////////////////////////////////
		public WallMesh(Sec sec, Color color)
		{
			this.sec = sec;
			this.color = color;

			GenerateAllVertexsAndFacets();
		}

		////////////////////////////////////////////////////////////////////////////////////
		private void GenerateAllVertexsAndFacets()
		{
			//计算所有垂直矩形的顶点集合
			//顶点数=边数* 4个顶点 * 边可能重复次数(1 or 2) * 垂直面可能重复次数(1 or 2)
			CustomVertex.PositionNormalColored[] v =
				new CustomVertex.PositionNormalColored[sec.num_borders * 4 * 2];

			//用于消除边重复的Hash Table
			Dictionary<ulong, bool> dict = new Dictionary<ulong, bool>(sec.num_borders);

			float epsilon = 0.5F; //不能小于0.2，否则Tu01ex.sec中三角面不正常
			int pos = 0;
			for (int i = 0; i < sec.num_borders; i++)
			{
				Border b = sec.borders[i];
				if (b.belong_district == -1 || b.neighbor_district == -1) continue; //特殊面，不画垂直面

				////////////////////////////////////////////////////////////////////////////////////////////
				//注意！不能用如下方法来消除边重复(例如RY和PT，缺面)
				//if (b.belong_district >= b.neighbor_district) continue; //面已经重复了，没有必要重绘

				//因此，只能采用集合薄记的方式来防止边重复
				bool ret;
				ulong x = (ulong)b.belong_district;
				ulong y = (ulong)b.neighbor_district;
				ulong key = (x << 32) | y;

				if (dict.TryGetValue(key, out ret))
					//已经登记过了，发生重复了
					continue;
				else
				{
					//没有登记过，登记进Hash Table
					key = (y << 32) | x;
					dict.Add(key, true);
				}
				////////////////////////////////////////////////////////////////////////////////////////////

				FPoint from = b.from;
				FPoint to = b.to;
				District belong = sec.districts[b.belong_district];
				District neighbor = sec.districts[b.neighbor_district];

				float z1 = from.x * belong.tanX + from.y * belong.tanY + belong.M;
				float z2 = to.x * belong.tanX + to.y * belong.tanY + belong.M;

				float z3 = from.x * neighbor.tanX + from.y * neighbor.tanY + neighbor.M;
				float z4 = to.x * neighbor.tanX + to.y * neighbor.tanY + neighbor.M;

				z1 = -z1;
				z2 = -z2;
				z3 = -z3;
				z4 = -z4;

				if (Math.Abs(z1 - z3) <= epsilon && Math.Abs(z2 - z4) <= epsilon) continue; //矩形两边高度均为0，无需画了

				v[pos++] = new CustomVertex.PositionNormalColored(from.x, from.y, z1, 0, 0, 0, color.ToArgb());
				v[pos++] = new CustomVertex.PositionNormalColored(to.x, to.y, z2, 0, 0, 0, color.ToArgb());
				v[pos++] = new CustomVertex.PositionNormalColored(to.x, to.y, z4, 0, 0, 0, color.ToArgb());
				v[pos++] = new CustomVertex.PositionNormalColored(from.x, from.y, z3, 0, 0, 0, color.ToArgb());
			}

			if (pos == 0)
			{
				vexbuf = null;
				return; //根本没有垂直面需要画，直接退出！
			}

			vexbuf = new CustomVertex.PositionNormalColored[pos];
			for (int i = 0; i < pos; i++)
				vexbuf[i] = v[i];

			v = null;//抛弃数组v

			//计算法线，矩形变成了三角形，或者上下边平行时，情况很微妙
			Vector3[] normals = new Vector3[pos / 4];
			for (int i = 0; i < pos / 4; i++)
			{
				CustomVertex.PositionNormalColored v0 = vexbuf[4 * i];
				CustomVertex.PositionNormalColored v1 = vexbuf[4 * i + 1];
				CustomVertex.PositionNormalColored v2 = vexbuf[4 * i + 2];
				CustomVertex.PositionNormalColored v3 = vexbuf[4 * i + 3];

				CustomVertex.PositionNormalColored[] tv = SimplifyQuadrangle(vexbuf, 4 * i, epsilon);
				normals[i] = CalculateNormalOfSimplifiedPolygon(tv);
			}

			for (int i = 0; i < pos; i++)
				vexbuf[i].Normal = normals[i / 4];

			int totalf = vexbuf.Length / 4 * 2; //三角面总数
			int totalv = vexbuf.Length;			//顶点总数

			//创建三角面集合
			idxbuf = new short[totalf * 3]; //注意，只能是short! WHY?

			pos = 0;
			for (int i = 0; i < totalf / 2; i++) //垂直多边形数为totalf/2
			{
				//分解成2个三角形, triangle fans
				for (int j = 0; j < 2; j++)
				{
					//v0, v1, v2三个点构成了一个左手三角形
					int v0 = 4 * i;
					int v1 = v0 + j + 1;
					int v2 = v1 + 1;

					idxbuf[pos++] = (short)v0;
					idxbuf[pos++] = (short)v1;
					idxbuf[pos++] = (short)v2;
				}
			}
		}

		///////////////////////////     UTILITYs     ///////////////////////////////
		//计算简化后的三角形或四边形的法线
		Vector3 CalculateNormalOfSimplifiedPolygon(CustomVertex.PositionNormalColored[] poly)
		{
			Debug.Assert(poly.Length >= 3); //不能计算直线的法线

			Vector3 from = poly[1].Position - poly[0].Position;
			Vector3 to = poly[2].Position - poly[1].Position;

			Vector3 n = Vector3.Cross(from, to);
			n.Normalize();

			return n;
		}

		//将四边形简化为线、三角形或者是四边形
		CustomVertex.PositionNormalColored[] SimplifyQuadrangle(CustomVertex.PositionNormalColored[] poly4, int offset, float epsilon)
		{
			//poly4为左手系
			CustomVertex.PositionNormalColored v0 = poly4[offset];
			CustomVertex.PositionNormalColored v1 = poly4[offset + 1];
			CustomVertex.PositionNormalColored v2 = poly4[offset + 2];
			CustomVertex.PositionNormalColored v3 = poly4[offset + 3];

			int total = 4;
			bool[] m = new bool[4] { true, true, true, true };

			if (Math.Abs(v0.Z - v3.Z) < epsilon)
			{
				//合并0－3
				total--;
				m[0] = false;
			}
			if (Math.Abs(v1.Z - v2.Z) < epsilon)
			{
				//合并1－2
				total--;
				m[1] = false;
			}

			CustomVertex.PositionNormalColored[] v = new CustomVertex.PositionNormalColored[total];
			int pos = 0;
			for (int i = 0; i < 4; i++)
				if (m[i]) v[pos++] = poly4[offset + i];

			return v; //简化成线、三角形或者是四边形
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void CreateWallMesh(Device device)
		{
			if (vexbuf == null)
			{
				Debug.WriteLine("[WallMesh] 空数据"); //此时根本无需存在任何垂直面需要绘制
				mesh = null; 
				return;
			}
			
			int totalf = vexbuf.Length / 4 * 2; //三角面总数
			int totalv = vexbuf.Length;			//顶点总数

			//创建网格对象
			mesh = new Mesh(totalf, totalv, MeshFlags.Dynamic, CustomVertex.PositionNormalColored.Format, device);

			//设置mesh
			mesh.SetVertexBufferData(vexbuf, LockFlags.None);
			mesh.SetIndexBufferData(idxbuf, LockFlags.None);
			DEBUG_PrintMeshInfo("[WallMesh] 没有优化");

			//优化顶点数量，删除多余顶点，只有几乎绝对重复的顶点才进行融接
			WeldEpsilons epsilon = new WeldEpsilons();
			epsilon.Diffuse = 0.01F;
			epsilon.Position = 0.01F;
			epsilon.Normal = 0.01F;
			mesh.WeldVertices(WeldEpsilonsFlags.WeldPartialMatches, epsilon, null, null);
			DEBUG_PrintMeshInfo("[WallMesh] 顶点优化");

			//针对cache命中率进行优化排序
			int[] adjacency = new int[mesh.NumberFaces * 3];
			mesh.GenerateAdjacency(0.01F, adjacency);
			mesh.OptimizeInPlace(MeshFlags.OptimizeVertexCache | MeshFlags.OptimizeCompact, adjacency);

			//将mesh变成是writeonly的，加快效率！
			Mesh m = mesh.Clone(MeshFlags.WriteOnly, CustomVertex.PositionNormalColored.Format, device);
			mesh.Dispose();
			mesh = m;
		}

		void DEBUG_PrintMeshInfo(string s)
		{
			Debug.Write(s);
			string msg = string.Format("\t v:{0}\tf:{1}", mesh.NumberVertices, mesh.NumberFaces);
			Debug.WriteLine(msg);
		}
	}
}