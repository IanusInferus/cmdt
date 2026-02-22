//TO THINK: 会不会出现根本不存在挤压平面需要绘制的情况？

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
	//挤压平面多边形
	public class Polygon
	{
		public CustomVertex.PositionNormalColored[] vertex; //左手系排列
		public Polygon(CustomVertex.PositionNormalColored[] v)
		{
			vertex = v;
		}

		//计算多边形在XOY平面上投影的面积
		float GetAreaOfXY()
		{
			float result = 0.0F;
			for (int i = 0; i < vertex.Length; i++)
			{
				CustomVertex.PositionNormalColored from = vertex[i];
				CustomVertex.PositionNormalColored to = vertex[(i + 1) % vertex.Length];
				result += (to.X - from.X) * (to.Y + from.Y) / 2;
			}

			return -result; //注意符号问题！
		}

		//计算多边形质心位置，其中的Z坐标为所有顶点Z坐标的均值
		public Vector3 GetCentroid()
		{
			float area = GetAreaOfXY();
			float x = 0;
			float y = 0;
			float z = 0;

			for (int i = 0; i < vertex.Length; i++)
			{
				CustomVertex.PositionNormalColored from = vertex[i];
				CustomVertex.PositionNormalColored to = vertex[(i + 1) % vertex.Length];

				x += (from.X + to.X) * (from.X * to.Y - to.X * from.Y);
				y += (from.Y + to.Y) * (from.X * to.Y - to.X * from.Y);
				z += from.Z;
			}

			//注意符号问题！
			x /= 6 * area;
			y /= 6 * area;				
			z /= vertex.Length;

			return new Vector3(x,y,z);
		}
	}

	////////////////////////////////////////////////////////////////////////////////////			
	//挤压平面网格
	class ExtrusionMesh
	{
		////////////////////////////////////////////////////////////////////////////////////			
		Sec sec;
		short[] idxbuf;									//面集合(索引)，只能定义成short[]
		CustomVertex.PositionNormalColored[] vexbuf;	//顶点集合

		Color color;	//默认顶点颜色(不进行地形属性着色时)
		bool att_color; //是否需要根据属性着色
		
		////////////////////////////////////////////////////////////////////////////////////
		public Mesh mesh;

		////////////////////////////////////////////////////////////////////////////////////
		public ExtrusionMesh(Sec sec, Color color, bool att_color)
		{
			this.sec = sec;
			this.color = color;
			this.att_color = att_color;

			//计算所有的顶点集合及面索引
			GenerateAllVertexsAndFacets();
		}
		////////////////////////////////////////////////////////////////////////////////////
		public Polygon[] polys; //所有的多边形数据
		int[] start_triangle;	//多边形-起始三角面编号映射表，已知多边形编号，查其起始三角面编号

		//计算所有的顶点集合及面索引
		private void GenerateAllVertexsAndFacets()
		{
			polys = GenerateAllPolygonWithVertexNormal();

			int totalf = 0;	//总三角面数
			int totalv = 0;	//总顶点数
			for (int i = 0; i < polys.Length; i++)
			{
				totalf += polys[i].vertex.Length - 2;
				totalv += polys[i].vertex.Length;
			}

			//创建顶点集合
			vexbuf = new CustomVertex.PositionNormalColored[totalv];
			int pos = 0;
			int[] poly_idx = new int[polys.Length];//每个多边形的起始位置
			for (int i = 0; i < polys.Length; i++)
			{
				poly_idx[i] = pos;
				for (int j = 0; j < polys[i].vertex.Length; j++)
					vexbuf[pos++] = polys[i].vertex[j];
			}

			//创建三角面集合
			idxbuf = new short[totalf * 3]; //注意，只能是short! WHY?

			//初始化起始三角形编号索引数组
			//已知polygon编号，速查该Polygon的起始三角形编号
			start_triangle = new int[polys.Length + 1];

			pos = 0;
			for (int i = 0; i < polys.Length; i++)
			{
				//分解成(polys[i].vertex.Length - 2)个三角形, triangle fans
				start_triangle[i] = pos / 3;
				
				for (int j = 0; j < polys[i].vertex.Length - 2; j++)
				{
					//baseptr, v1, v2三个点构成了一个左手三角形
					int v0 = poly_idx[i];
					int v1 = v0 + j + 1;
					int v2 = v1 + 1;

					idxbuf[pos++] = (short)v0;
					idxbuf[pos++] = (short)v1;
					idxbuf[pos++] = (short)v2;
				}
			}

			start_triangle[polys.Length] = pos;
		}
		////////////////////////////////////////////////////////////////////////////////////
		#region 计算所有多边形及其顶点法线
		Polygon[] GenerateAllPolygonWithVertexNormal()
		{
			Polygon[] p = new Polygon[sec.num_districts];

			//仅计算所有的顶点xyz坐标
			for (int i = 0; i < sec.num_districts; i++)
				p[i] = GeneratePolyPos(i);

			//仅计算所有顶点的法线方向
			GenerateAllNormalsOfPolys(p);

			return p;
		}

		void GenerateAllNormalsOfPolys(Polygon[] poly)
		{
			Vector3[] normals = new Vector3[poly.Length];

			for (int i = 0; i < poly.Length; i++)
			{
				Polygon p = poly[i];

				CustomVertex.PositionNormalColored v0 = p.vertex[0];
				CustomVertex.PositionNormalColored v1 = p.vertex[1];
				CustomVertex.PositionNormalColored v2 = p.vertex[2];

				Vector3 A = new Vector3(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);
				Vector3 B = new Vector3(v0.X - v1.X, v0.Y - v1.Y, v0.Z - v1.Z);

				normals[i] = Vector3.Cross(A, B); //这个叉乘居然也是左手的？faint
				normals[i].Normalize(); //必须标准化
			}

			for (int i = 0; i < poly.Length; i++)
				for (int j = 0; j < poly[i].vertex.Length; j++)
					poly[i].vertex[j].Normal = normals[i];
		}

		Polygon GeneratePolyPos(int idx)
		{
			District d = sec.districts[idx];

			CustomVertex.PositionNormalColored[] v = new CustomVertex.PositionNormalColored[d.num_borders];

			for (int i = d.num_borders - 1; i >= 0; i--) //右手系->左手系
			{
				Border b = d.borders[i];
				float z = b.to.x * d.tanX + b.to.y * d.tanY + d.M;

				//这里的-z是将右手系->左手系
				v[d.num_borders - 1 - i] = new CustomVertex.PositionNormalColored();
				v[d.num_borders - 1 - i].Position = new Vector3(b.to.x, b.to.y, -z);

				if (att_color)
				{
					//根据区域的地形特征来着色
					ColoredByCategory(d.attributes[0], ref v[d.num_borders - 1 - i]);
					ColoredBySubCategory(d.attributes[1], ref v[d.num_borders - 1 - i]);

					//根据区域是否可进入来着色
					ColoredByEnterable(d.attributes[4], ref v[d.num_borders - 1 - i]);

					//根据区域亮度来着色
					ColoredByIllumination(d.attributes[5], ref v[d.num_borders - 1 - i]);

					//对边缘上的框架区域着色
					if (d.attributes[6] == 0x2)
						v[d.num_borders - 1 - i].Color = 0x25;//非常深的蓝色
				}
				else
					v[d.num_borders - 1 - i].Color = color.ToArgb();
			}

			return new Polygon(v);
		}
		#endregion
		////////////////////////////////////////////////////////////////////////////////////
		#region 基于地形属性进行顶点着色
		void ColoredByIllumination(int brightness, ref CustomVertex.PositionNormalColored vex)
		{
			float m = (255 - brightness) / (float)255;
			Color c = Color.FromArgb(vex.Color);
			int red = (int)(c.R * m);
			int green = (int)(c.G * m);
			int blue = (int)(c.B * m);

			vex.Color = Color.FromArgb(red, green, blue).ToArgb();
		}

		void ColoredByEnterable(int enterable, ref CustomVertex.PositionNormalColored vex)
		{
			float m = 0F;
			Color c = Color.FromArgb(vex.Color);
			int red = (int)(c.R * m);
			int green = (int)(c.G * m);
			int blue = (int)(c.B * m);

			if (enterable == 0x4)
				vex.Color = Color.FromArgb(red, green, blue).ToArgb();
			if (enterable == 0x10)
				vex.Color = Color.FromArgb(red, green, blue).ToArgb();
			if (enterable == 0x14)
				vex.Color = Color.FromArgb(red, green, blue).ToArgb();
			if (enterable == 0x16)
				vex.Color = Color.FromArgb(red, green, blue).ToArgb();
		}

		void ColoredByCategory(int category, ref CustomVertex.PositionNormalColored vex)
		{
			switch (category)
			{
				case 0: //陆地
					vex.Color = Color.DarkOrange.ToArgb();
					break;
				case 1: //雪地
					vex.Color = Color.White.ToArgb();
					break;
				case 2: //深水
					vex.Color = Color.Blue.ToArgb();
					break;
				case 3: //浅水
					vex.Color = Color.LightBlue.ToArgb();
					break;
				case 4: //无
					vex.Color = Color.RoyalBlue.ToArgb();
					break;
				default:
					Debug.Assert(false);
					break;
			}
		}

		void ColoredBySubCategory(int sub_category, ref CustomVertex.PositionNormalColored vex)
		{
			switch (sub_category)
			{
				case 0: //沙子或者土壤
					vex.Color = Color.DarkOrange.ToArgb();
					break;
				case 1: //草地
					vex.Color = Color.LimeGreen.ToArgb();
					break;
				case 2://路
					vex.Color = Color.Silver.ToArgb();
					break;
				case 3://不好判断
					break;
				case 4://无
					break;
				case 5://木质地面
					vex.Color = Color.Gold.ToArgb();
					break;
				case 6://泥沙地形，河边的居多，不好判断
					vex.Color = Color.Khaki.ToArgb();
					break;
				case 7://雪地
					vex.Color = Color.White.ToArgb();
					break;
				case 8://无
					break;
				case 9://从河边卵石到土壤，不好判断
					vex.Color = Color.Linen.ToArgb();
					break;
				case 10://无
					break;
				case 11: //铁质地形，铁栏杆，铁楼梯
					vex.Color = Color.DarkSlateGray.ToArgb();
					break;
				case 12: //无
					break;
				case 13: //浅水斜坡
					vex.Color = Color.CornflowerBlue.ToArgb();
					break;
				case 14: //深水
					vex.Color = Color.DarkBlue.ToArgb();
					break;
				case 15: //卵石
					vex.Color = Color.Gainsboro.ToArgb();
					break;
			}

		}
		#endregion
		////////////////////////////////////////////////////////////////////////////////////
		#region 根据顶点集及其面索引创建Mesh，并优化之
		public void CreateExtrusionMesh(Device device)
		{
			int totalf = idxbuf.Length / 3;	//总三角面数
			int totalv = vexbuf.Length;	//总顶点数

			//创建网格对象
			mesh = new Mesh(totalf, totalv, MeshFlags.Dynamic, CustomVertex.PositionNormalColored.Format, device);

			//设置mesh
			mesh.SetVertexBufferData(vexbuf, LockFlags.None);
			mesh.SetIndexBufferData(idxbuf, LockFlags.None);
			DEBUG_PrintMeshInfo("[Mesh] 没有优化");
			
			//优化顶点数量，删除多余顶点，只有几乎绝对重复的点才删除
			WeldEpsilons epsilon = new WeldEpsilons();
			epsilon.Diffuse = 0.01F;
			epsilon.Position = 0.01F;
			epsilon.Normal = 0.01F;
			mesh.WeldVertices(WeldEpsilonsFlags.WeldPartialMatches, epsilon, null, null);
			DEBUG_PrintMeshInfo("[Mesh] 顶点优化");
			
			//优化mesh，并由OptimizeInPlace自动计算SubSet个数
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
		#endregion
		////////////////////////////////////////////////////////////////////////////////////
		#region Picking System相关
		
		//已知匹配三角形的编号为idx，求出其所属的多边形编号
		int FindPolygonByTriangleIdx(int idx)
		{
			if (idx < 0 && idx >= idxbuf.Length) return -1;
			for (int i = 0; i < polys.Length; i++)
				if ((idx >= start_triangle[i]) && (idx < start_triangle[i+1]))
					return i;
			return -1;
		}

		public int MatchPicking(Device device, Vector3 raypos, Vector3 raydir)
		{
			int idx = -1; //-1代表没有匹配，否则，idx为匹配三角面编号
			float min_z = float.PositiveInfinity; //用于比较多个匹配三角面的Z深度

			Matrix trans = device.Transform.World * device.Transform.View * device.Transform.Projection; //常数提取
			
			//遍历所有三角形...这里效率比较低，需要想想办法
			for (int i = 0; i < idxbuf.Length / 3; i++)
			{
				IntersectInformation hitlocation;

				int v0_idx = idxbuf[3 * i];
				int v1_idx = idxbuf[3 * i + 1];
				int v2_idx = idxbuf[3 * i + 2];

				Vector3 v0 = vexbuf[v0_idx].Position;
				Vector3 v1 = vexbuf[v1_idx].Position;
				Vector3 v2 = vexbuf[v2_idx].Position;

				//测试picking ray与三角形的交点是否相交
				bool result = Geometry.IntersectTri(
					v0, v1, v2,
					raypos,
					raydir,
					out hitlocation);
				
				if (result) //线面是相交的
				{				
					//计算world下的交点，将其转换至projection windows，并测试其深度
					Vector3 p = v0 + hitlocation.U * (v1 - v0) + hitlocation.V * (v2 - v0);
					p.TransformCoordinate(trans);
					
					if (p.Z < min_z) //比较Z depth，选取深度最小的三角形
					{
						idx = i;
						min_z = p.Z;
					}
				}
			}

			if (idx != -1) //如果存在着匹配
			{
				//idx为z深度最小的匹配三角面编号，根据其查出对应的多边形编号
				int polyid = FindPolygonByTriangleIdx(idx);
				
				//返回查找到的多边形编号
				Debug.Assert(polyid != -1);			
				return polyid;
			}
			else
				return -1; //否则，返回-1，不存在匹配多边形
		}
		#endregion
		////////////////////////////////////////////////////////////////////////////////////
		public float CaculateBoundSphere(out Vector3 center)
		{
			GraphicsStream gs = mesh.LockVertexBuffer(LockFlags.None);

			float radius = Geometry.ComputeBoundingSphere(
				gs,
				mesh.NumberVertices,
				mesh.VertexFormat,
				out center);

			mesh.UnlockVertexBuffer();

			return radius;
		}
	}
}
