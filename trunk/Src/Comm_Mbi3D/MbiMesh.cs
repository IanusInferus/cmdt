using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;

namespace Comm_Mbi3D
{
	class MbiMesh
	{
		public Vector3 center;		//Mesh包络球中心
		public float radius;		//Mesh包络球半径

		public Texture[] texture;	//贴图数组

		public VertexBuffer vexbuf;
		public int[] txtoffset; //顶点集合中需要切换texture的地方！		

		private CustomVertex.PositionColoredTextured[] vexarray;  //顶点集合

		private MBI mbi; //MBI文件

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public MbiMesh(MBI mbi)
		{
			this.mbi = mbi;
			SortingPolygonsByTexture();
			GenVertexBuf();
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		/*
		private bool Validate()
		{
			for (int i = 0; i < mbi.objects.Length; i++) //遍历对象
			{
				Object obj = mbi.objects[i];
				int pic_id = mbi.districts[obj.start_district_id].pic_id;
				for (int j = obj.start_district_id + 1; j <= obj.end_district_id; j++) //遍历对象所拥有的多边形
				{
					District dis = mbi.districts[j];
					if (dis.pic_id != pic_id) 
						return false;
				}
			}
			return true;
		}
		*/

		List<Polygon>[] lpoly;

		private void SortingPolygonsByTexture()
		{
			lpoly = new List<Polygon>[mbi.txtinfos.Length];

			for (int i = 0; i < mbi.txtinfos.Length; i++)
				lpoly[i] = new List<Polygon>();

			for (int i = 0; i < mbi.polygons.Length; i++)
			{
				Polygon poly = mbi.polygons[i];
				int idx = poly.texture_id;
				lpoly[idx].Add(poly);
			}
		}

		private void GenVertexBuf()
		{
			int totalf = 0; //三角形数目
			for (int i = 0; i < mbi.polygons.Length; i++)
			{
				Polygon di = mbi.polygons[i];
				totalf += di.num_lines - 2;
			}

			int totalv = 3 * totalf;  //顶点数目
			vexarray = new CustomVertex.PositionColoredTextured[totalv];//创建顶点集合

			txtoffset = new int[mbi.txtinfos.Length + 1];
			txtoffset[mbi.txtinfos.Length] = totalv;

			int pos = 0;
			for (int i = 0; i < mbi.txtinfos.Length; i++) //第i个贴图
			{
				txtoffset[i] = pos;
				foreach (Polygon poly in lpoly[i]) //使用第i个贴图的所有多边形
				{
					Debug.Assert(poly.texture_id == i);
					for (int j = 0; j < poly.num_lines - 2; j++) //把多边形转换为三角形，放到顶点集合中
					{
						//注意：以下数据都是右手系的，因此在显示时，必须进行RHtoLH转换
						vexarray[pos].X = poly.map_points[0].vertex.X;
						vexarray[pos].Y = poly.map_points[0].vertex.Y;
						vexarray[pos].Z = poly.map_points[0].vertex.Z;
						vexarray[pos].Tu = poly.map_points[0].U;
						vexarray[pos].Tv = poly.map_points[0].V;
						vexarray[pos].Color = Color.White.ToArgb();
						pos++;

						vexarray[pos].X = poly.map_points[j + 1].vertex.X;
						vexarray[pos].Y = poly.map_points[j + 1].vertex.Y;
						vexarray[pos].Z = poly.map_points[j + 1].vertex.Z;
						vexarray[pos].Tu = poly.map_points[j + 1].U;
						vexarray[pos].Tv = poly.map_points[j + 1].V;
						vexarray[pos].Color = Color.White.ToArgb();
						pos++;

						vexarray[pos].X = poly.map_points[j + 2].vertex.X;
						vexarray[pos].Y = poly.map_points[j + 2].vertex.Y;
						vexarray[pos].Z = poly.map_points[j + 2].vertex.Z;
						vexarray[pos].Tu = poly.map_points[j + 2].U;
						vexarray[pos].Tv = poly.map_points[j + 2].V;
						vexarray[pos].Color = Color.White.ToArgb();
						pos++;
					}
				}
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public void CreateVertexBuffer(Device device)
		{
			vexbuf = new VertexBuffer(
			   typeof(CustomVertex.PositionColoredTextured),	//顶点类型
			   vexarray.Length,									//顶点个数
			   device,
			   Usage.WriteOnly | Usage.Dynamic,
			   CustomVertex.PositionColoredTextured.Format,		//顶点格式
			   Pool.Default);

			vexbuf.SetData(vexarray, 0, LockFlags.Discard);

			CaculateBoundSphere();
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public void CreateAllTextures(Device device)
		{
			Debug.WriteLine("[Texture] 开始创建...");
			texture = new Texture[mbi.txtinfos.Length];
			for (int i = 0; i < texture.Length; i++)
			{
				Debug.WriteLine(String.Format("[Texture] {0}/{1}", i + 1, texture.Length));
				CreateSingleTexture(device, i, mbi.texturetype[i]);
			}
			Debug.WriteLine("[Texture] 创建结束");
		}

		//根据不同的贴图类型type来创建贴图
		private unsafe void CreateSingleTexture(Device device, int idx, byte type)
		{
			TextureInfo txt = mbi.txtinfos[idx];
			texture[idx] = new Texture(device, txt.width, txt.height, 0, 0, Format.A8R8G8B8, Pool.Managed);
			Texture t = texture[idx];

			SurfaceDescription s = t.GetLevelDescription(0);
			uint* pData = (uint*)t.LockRectangle(0, LockFlags.None).InternalData.ToPointer();

			int pos = 0;
			for (int i = 0; i < s.Width; i++)
				for (int j = 0; j < s.Height; j++)
				{
					int pal = txt.data[pos++];
					uint color = txt.color[pal];

					switch (type)	//判断贴图类型
					{
						case 0:		//普通贴图
						case 1:		//transparent key color! ARGB透明色:0xffff00ff
							if (color == 0xffff00ff)
								*pData++ = (uint)0x0;
							else
								*pData++ = (uint)color;
							break;
						case 2:		//Alpha光晕效果
							*pData++ = (uint)((((color & 0xff) / 4) << 24) | 0xffffff);
							break;
						case 4:		//大理石地板反射效果(其实是透明效果)
							*pData++ = (uint)((0xE0 << 24) | (color & 0xffffff));
							break;
						default:	//强烈的视觉警告，未知的新贴图类型!
							*pData++ = 0xffff00ff;
							break;
					}
				}

			t.UnlockRectangle(0);
		}

		public void DisposeAllTextures()
		{
			if (texture == null) return;
			for (int i = 0; i < texture.Length; i++)
				if (texture[i] != null)
				{
					texture[i].Dispose();
					texture[i] = null;
				}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		private void CaculateBoundSphere()
		{
			GraphicsStream vertexData = vexbuf.Lock(0, 0, LockFlags.NoOverwrite);
			radius = Geometry.ComputeBoundingSphere(
				vertexData,
				vexarray.Length,
				CustomVertex.PositionColoredTextured.Format,
				out center);
			vexbuf.Unlock();
		}
	}
}
