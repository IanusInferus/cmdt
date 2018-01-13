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
		public Vector3 center;		//Mesh����������
		public float radius;		//Mesh������뾶

		public Texture[] texture;	//��ͼ����

		public VertexBuffer vexbuf;
		public int[] txtoffset; //���㼯������Ҫ�л�texture�ĵط���		

		private CustomVertex.PositionColoredTextured[] vexarray;  //���㼯��

		private MBI mbi; //MBI�ļ�

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
			for (int i = 0; i < mbi.objects.Length; i++) //��������
			{
				Object obj = mbi.objects[i];
				int pic_id = mbi.districts[obj.start_district_id].pic_id;
				for (int j = obj.start_district_id + 1; j <= obj.end_district_id; j++) //����������ӵ�еĶ����
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
			int totalf = 0; //��������Ŀ
			for (int i = 0; i < mbi.polygons.Length; i++)
			{
				Polygon di = mbi.polygons[i];
				totalf += di.num_lines - 2;
			}

			int totalv = 3 * totalf;  //������Ŀ
			vexarray = new CustomVertex.PositionColoredTextured[totalv];//�������㼯��

			txtoffset = new int[mbi.txtinfos.Length + 1];
			txtoffset[mbi.txtinfos.Length] = totalv;

			int pos = 0;
			for (int i = 0; i < mbi.txtinfos.Length; i++) //��i����ͼ
			{
				txtoffset[i] = pos;
				foreach (Polygon poly in lpoly[i]) //ʹ�õ�i����ͼ�����ж����
				{
					Debug.Assert(poly.texture_id == i);
					for (int j = 0; j < poly.num_lines - 2; j++) //�Ѷ����ת��Ϊ�����Σ��ŵ����㼯����
					{
						//ע�⣺�������ݶ�������ϵ�ģ��������ʾʱ���������RHtoLHת��
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
			   typeof(CustomVertex.PositionColoredTextured),	//��������
			   vexarray.Length,									//�������
			   device,
			   Usage.WriteOnly | Usage.Dynamic,
			   CustomVertex.PositionColoredTextured.Format,		//�����ʽ
			   Pool.Default);

			vexbuf.SetData(vexarray, 0, LockFlags.Discard);

			CaculateBoundSphere();
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public void CreateAllTextures(Device device)
		{
			Debug.WriteLine("[Texture] ��ʼ����...");
			texture = new Texture[mbi.txtinfos.Length];
			for (int i = 0; i < texture.Length; i++)
			{
				Debug.WriteLine(String.Format("[Texture] {0}/{1}", i + 1, texture.Length));
				CreateSingleTexture(device, i, mbi.texturetype[i]);
			}
			Debug.WriteLine("[Texture] ��������");
		}

		//���ݲ�ͬ����ͼ����type��������ͼ
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

					switch (type)	//�ж���ͼ����
					{
						case 0:		//��ͨ��ͼ
						case 1:		//transparent key color! ARGB͸��ɫ:0xffff00ff
							if (color == 0xffff00ff)
								*pData++ = (uint)0x0;
							else
								*pData++ = (uint)color;
							break;
						case 2:		//Alpha����Ч��
							*pData++ = (uint)((((color & 0xff) / 4) << 24) | 0xffffff);
							break;
						case 4:		//����ʯ�ذ巴��Ч��(��ʵ��͸��Ч��)
							*pData++ = (uint)((0xE0 << 24) | (color & 0xffffff));
							break;
						default:	//ǿ�ҵ��Ӿ����棬δ֪������ͼ����!
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
