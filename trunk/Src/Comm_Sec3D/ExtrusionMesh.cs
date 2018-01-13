//TO THINK: �᲻����ָ��������ڼ�ѹƽ����Ҫ���Ƶ������

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
	//��ѹƽ������
	public class Polygon
	{
		public CustomVertex.PositionNormalColored[] vertex; //����ϵ����
		public Polygon(CustomVertex.PositionNormalColored[] v)
		{
			vertex = v;
		}

		//����������XOYƽ����ͶӰ�����
		float GetAreaOfXY()
		{
			float result = 0.0F;
			for (int i = 0; i < vertex.Length; i++)
			{
				CustomVertex.PositionNormalColored from = vertex[i];
				CustomVertex.PositionNormalColored to = vertex[(i + 1) % vertex.Length];
				result += (to.X - from.X) * (to.Y + from.Y) / 2;
			}

			return -result; //ע��������⣡
		}

		//������������λ�ã����е�Z����Ϊ���ж���Z����ľ�ֵ
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

			//ע��������⣡
			x /= 6 * area;
			y /= 6 * area;				
			z /= vertex.Length;

			return new Vector3(x,y,z);
		}
	}

	////////////////////////////////////////////////////////////////////////////////////			
	//��ѹƽ������
	class ExtrusionMesh
	{
		////////////////////////////////////////////////////////////////////////////////////			
		Sec sec;
		short[] idxbuf;									//�漯��(����)��ֻ�ܶ����short[]
		CustomVertex.PositionNormalColored[] vexbuf;	//���㼯��

		Color color;	//Ĭ�϶�����ɫ(�����е���������ɫʱ)
		bool att_color; //�Ƿ���Ҫ����������ɫ
		
		////////////////////////////////////////////////////////////////////////////////////
		public Mesh mesh;

		////////////////////////////////////////////////////////////////////////////////////
		public ExtrusionMesh(Sec sec, Color color, bool att_color)
		{
			this.sec = sec;
			this.color = color;
			this.att_color = att_color;

			//�������еĶ��㼯�ϼ�������
			GenerateAllVertexsAndFacets();
		}
		////////////////////////////////////////////////////////////////////////////////////
		public Polygon[] polys; //���еĶ��������
		int[] start_triangle;	//�����-��ʼ��������ӳ�����֪����α�ţ�������ʼ��������

		//�������еĶ��㼯�ϼ�������
		private void GenerateAllVertexsAndFacets()
		{
			polys = GenerateAllPolygonWithVertexNormal();

			int totalf = 0;	//����������
			int totalv = 0;	//�ܶ�����
			for (int i = 0; i < polys.Length; i++)
			{
				totalf += polys[i].vertex.Length - 2;
				totalv += polys[i].vertex.Length;
			}

			//�������㼯��
			vexbuf = new CustomVertex.PositionNormalColored[totalv];
			int pos = 0;
			int[] poly_idx = new int[polys.Length];//ÿ������ε���ʼλ��
			for (int i = 0; i < polys.Length; i++)
			{
				poly_idx[i] = pos;
				for (int j = 0; j < polys[i].vertex.Length; j++)
					vexbuf[pos++] = polys[i].vertex[j];
			}

			//���������漯��
			idxbuf = new short[totalf * 3]; //ע�⣬ֻ����short! WHY?

			//��ʼ����ʼ�����α����������
			//��֪polygon��ţ��ٲ��Polygon����ʼ�����α��
			start_triangle = new int[polys.Length + 1];

			pos = 0;
			for (int i = 0; i < polys.Length; i++)
			{
				//�ֽ��(polys[i].vertex.Length - 2)��������, triangle fans
				start_triangle[i] = pos / 3;
				
				for (int j = 0; j < polys[i].vertex.Length - 2; j++)
				{
					//baseptr, v1, v2�����㹹����һ������������
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
		#region �������ж���μ��䶥�㷨��
		Polygon[] GenerateAllPolygonWithVertexNormal()
		{
			Polygon[] p = new Polygon[sec.num_districts];

			//���������еĶ���xyz����
			for (int i = 0; i < sec.num_districts; i++)
				p[i] = GeneratePolyPos(i);

			//���������ж���ķ��߷���
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

				normals[i] = Vector3.Cross(A, B); //�����˾�ȻҲ�����ֵģ�faint
				normals[i].Normalize(); //�����׼��
			}

			for (int i = 0; i < poly.Length; i++)
				for (int j = 0; j < poly[i].vertex.Length; j++)
					poly[i].vertex[j].Normal = normals[i];
		}

		Polygon GeneratePolyPos(int idx)
		{
			District d = sec.districts[idx];

			CustomVertex.PositionNormalColored[] v = new CustomVertex.PositionNormalColored[d.num_borders];

			for (int i = d.num_borders - 1; i >= 0; i--) //����ϵ->����ϵ
			{
				Border b = d.borders[i];
				float z = b.to.x * d.tanX + b.to.y * d.tanY + d.M;

				//�����-z�ǽ�����ϵ->����ϵ
				v[d.num_borders - 1 - i] = new CustomVertex.PositionNormalColored();
				v[d.num_borders - 1 - i].Position = new Vector3(b.to.x, b.to.y, -z);

				if (att_color)
				{
					//��������ĵ�����������ɫ
					ColoredByCategory(d.attributes[0], ref v[d.num_borders - 1 - i]);
					ColoredBySubCategory(d.attributes[1], ref v[d.num_borders - 1 - i]);

					//���������Ƿ�ɽ�������ɫ
					ColoredByEnterable(d.attributes[4], ref v[d.num_borders - 1 - i]);

					//����������������ɫ
					ColoredByIllumination(d.attributes[5], ref v[d.num_borders - 1 - i]);

					//�Ա�Ե�ϵĿ��������ɫ
					if (d.attributes[6] == 0x2)
						v[d.num_borders - 1 - i].Color = 0x25;//�ǳ������ɫ
				}
				else
					v[d.num_borders - 1 - i].Color = color.ToArgb();
			}

			return new Polygon(v);
		}
		#endregion
		////////////////////////////////////////////////////////////////////////////////////
		#region ���ڵ������Խ��ж�����ɫ
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
				case 0: //½��
					vex.Color = Color.DarkOrange.ToArgb();
					break;
				case 1: //ѩ��
					vex.Color = Color.White.ToArgb();
					break;
				case 2: //��ˮ
					vex.Color = Color.Blue.ToArgb();
					break;
				case 3: //ǳˮ
					vex.Color = Color.LightBlue.ToArgb();
					break;
				case 4: //��
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
				case 0: //ɳ�ӻ�������
					vex.Color = Color.DarkOrange.ToArgb();
					break;
				case 1: //�ݵ�
					vex.Color = Color.LimeGreen.ToArgb();
					break;
				case 2://·
					vex.Color = Color.Silver.ToArgb();
					break;
				case 3://�����ж�
					break;
				case 4://��
					break;
				case 5://ľ�ʵ���
					vex.Color = Color.Gold.ToArgb();
					break;
				case 6://��ɳ���Σ��ӱߵľӶ࣬�����ж�
					vex.Color = Color.Khaki.ToArgb();
					break;
				case 7://ѩ��
					vex.Color = Color.White.ToArgb();
					break;
				case 8://��
					break;
				case 9://�Ӻӱ���ʯ�������������ж�
					vex.Color = Color.Linen.ToArgb();
					break;
				case 10://��
					break;
				case 11: //���ʵ��Σ������ˣ���¥��
					vex.Color = Color.DarkSlateGray.ToArgb();
					break;
				case 12: //��
					break;
				case 13: //ǳˮб��
					vex.Color = Color.CornflowerBlue.ToArgb();
					break;
				case 14: //��ˮ
					vex.Color = Color.DarkBlue.ToArgb();
					break;
				case 15: //��ʯ
					vex.Color = Color.Gainsboro.ToArgb();
					break;
			}

		}
		#endregion
		////////////////////////////////////////////////////////////////////////////////////
		#region ���ݶ��㼯��������������Mesh�����Ż�֮
		public void CreateExtrusionMesh(Device device)
		{
			int totalf = idxbuf.Length / 3;	//����������
			int totalv = vexbuf.Length;	//�ܶ�����

			//�����������
			mesh = new Mesh(totalf, totalv, MeshFlags.Dynamic, CustomVertex.PositionNormalColored.Format, device);

			//����mesh
			mesh.SetVertexBufferData(vexbuf, LockFlags.None);
			mesh.SetIndexBufferData(idxbuf, LockFlags.None);
			DEBUG_PrintMeshInfo("[Mesh] û���Ż�");
			
			//�Ż�����������ɾ�����ඥ�㣬ֻ�м��������ظ��ĵ��ɾ��
			WeldEpsilons epsilon = new WeldEpsilons();
			epsilon.Diffuse = 0.01F;
			epsilon.Position = 0.01F;
			epsilon.Normal = 0.01F;
			mesh.WeldVertices(WeldEpsilonsFlags.WeldPartialMatches, epsilon, null, null);
			DEBUG_PrintMeshInfo("[Mesh] �����Ż�");
			
			//�Ż�mesh������OptimizeInPlace�Զ�����SubSet����
			int[] adjacency = new int[mesh.NumberFaces * 3];
			mesh.GenerateAdjacency(0.01F, adjacency);
			mesh.OptimizeInPlace(MeshFlags.OptimizeVertexCache | MeshFlags.OptimizeCompact, adjacency);

			//��mesh�����writeonly�ģ��ӿ�Ч�ʣ�
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
		#region Picking System���
		
		//��֪ƥ�������εı��Ϊidx������������Ķ���α��
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
			int idx = -1; //-1����û��ƥ�䣬����idxΪƥ����������
			float min_z = float.PositiveInfinity; //���ڱȽ϶��ƥ���������Z���

			Matrix trans = device.Transform.World * device.Transform.View * device.Transform.Projection; //������ȡ
			
			//��������������...����Ч�ʱȽϵͣ���Ҫ����취
			for (int i = 0; i < idxbuf.Length / 3; i++)
			{
				IntersectInformation hitlocation;

				int v0_idx = idxbuf[3 * i];
				int v1_idx = idxbuf[3 * i + 1];
				int v2_idx = idxbuf[3 * i + 2];

				Vector3 v0 = vexbuf[v0_idx].Position;
				Vector3 v1 = vexbuf[v1_idx].Position;
				Vector3 v2 = vexbuf[v2_idx].Position;

				//����picking ray�������εĽ����Ƿ��ཻ
				bool result = Geometry.IntersectTri(
					v0, v1, v2,
					raypos,
					raydir,
					out hitlocation);
				
				if (result) //�������ཻ��
				{				
					//����world�µĽ��㣬����ת����projection windows�������������
					Vector3 p = v0 + hitlocation.U * (v1 - v0) + hitlocation.V * (v2 - v0);
					p.TransformCoordinate(trans);
					
					if (p.Z < min_z) //�Ƚ�Z depth��ѡȡ�����С��������
					{
						idx = i;
						min_z = p.Z;
					}
				}
			}

			if (idx != -1) //���������ƥ��
			{
				//idxΪz�����С��ƥ���������ţ�����������Ӧ�Ķ���α��
				int polyid = FindPolygonByTriangleIdx(idx);
				
				//���ز��ҵ��Ķ���α��
				Debug.Assert(polyid != -1);			
				return polyid;
			}
			else
				return -1; //���򣬷���-1��������ƥ������
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
