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
	//��ֱƽ������
	class WallMesh
	{
		Sec sec;
		
		////////////////////////////////////////////////////////////////////////////////////
		Color color;
		short[] idxbuf;									//�漯��(����),ֻ�ܶ����short[]
		CustomVertex.PositionNormalColored[] vexbuf;	//���㼯��
		
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
			//�������д�ֱ���εĶ��㼯��
			//������=����* 4������ * �߿����ظ�����(1 or 2) * ��ֱ������ظ�����(1 or 2)
			CustomVertex.PositionNormalColored[] v =
				new CustomVertex.PositionNormalColored[sec.num_borders * 4 * 2];

			//�����������ظ���Hash Table
			Dictionary<ulong, bool> dict = new Dictionary<ulong, bool>(sec.num_borders);

			float epsilon = 0.5F; //����С��0.2������Tu01ex.sec�������治����
			int pos = 0;
			for (int i = 0; i < sec.num_borders; i++)
			{
				Border b = sec.borders[i];
				if (b.belong_district == -1 || b.neighbor_district == -1) continue; //�����棬������ֱ��

				////////////////////////////////////////////////////////////////////////////////////////////
				//ע�⣡���������·������������ظ�(����RY��PT��ȱ��)
				//if (b.belong_district >= b.neighbor_district) continue; //���Ѿ��ظ��ˣ�û�б�Ҫ�ػ�

				//��ˣ�ֻ�ܲ��ü��ϱ��ǵķ�ʽ����ֹ���ظ�
				bool ret;
				ulong x = (ulong)b.belong_district;
				ulong y = (ulong)b.neighbor_district;
				ulong key = (x << 32) | y;

				if (dict.TryGetValue(key, out ret))
					//�Ѿ��Ǽǹ��ˣ������ظ���
					continue;
				else
				{
					//û�еǼǹ����Ǽǽ�Hash Table
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

				if (Math.Abs(z1 - z3) <= epsilon && Math.Abs(z2 - z4) <= epsilon) continue; //�������߸߶Ⱦ�Ϊ0�����軭��

				v[pos++] = new CustomVertex.PositionNormalColored(from.x, from.y, z1, 0, 0, 0, color.ToArgb());
				v[pos++] = new CustomVertex.PositionNormalColored(to.x, to.y, z2, 0, 0, 0, color.ToArgb());
				v[pos++] = new CustomVertex.PositionNormalColored(to.x, to.y, z4, 0, 0, 0, color.ToArgb());
				v[pos++] = new CustomVertex.PositionNormalColored(from.x, from.y, z3, 0, 0, 0, color.ToArgb());
			}

			if (pos == 0)
			{
				vexbuf = null;
				return; //����û�д�ֱ����Ҫ����ֱ���˳���
			}

			vexbuf = new CustomVertex.PositionNormalColored[pos];
			for (int i = 0; i < pos; i++)
				vexbuf[i] = v[i];

			v = null;//��������v

			//���㷨�ߣ����α���������Σ��������±�ƽ��ʱ�������΢��
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

			int totalf = vexbuf.Length / 4 * 2; //����������
			int totalv = vexbuf.Length;			//��������

			//���������漯��
			idxbuf = new short[totalf * 3]; //ע�⣬ֻ����short! WHY?

			pos = 0;
			for (int i = 0; i < totalf / 2; i++) //��ֱ�������Ϊtotalf/2
			{
				//�ֽ��2��������, triangle fans
				for (int j = 0; j < 2; j++)
				{
					//v0, v1, v2�����㹹����һ������������
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
		//����򻯺�������λ��ı��εķ���
		Vector3 CalculateNormalOfSimplifiedPolygon(CustomVertex.PositionNormalColored[] poly)
		{
			Debug.Assert(poly.Length >= 3); //���ܼ���ֱ�ߵķ���

			Vector3 from = poly[1].Position - poly[0].Position;
			Vector3 to = poly[2].Position - poly[1].Position;

			Vector3 n = Vector3.Cross(from, to);
			n.Normalize();

			return n;
		}

		//���ı��μ�Ϊ�ߡ������λ������ı���
		CustomVertex.PositionNormalColored[] SimplifyQuadrangle(CustomVertex.PositionNormalColored[] poly4, int offset, float epsilon)
		{
			//poly4Ϊ����ϵ
			CustomVertex.PositionNormalColored v0 = poly4[offset];
			CustomVertex.PositionNormalColored v1 = poly4[offset + 1];
			CustomVertex.PositionNormalColored v2 = poly4[offset + 2];
			CustomVertex.PositionNormalColored v3 = poly4[offset + 3];

			int total = 4;
			bool[] m = new bool[4] { true, true, true, true };

			if (Math.Abs(v0.Z - v3.Z) < epsilon)
			{
				//�ϲ�0��3
				total--;
				m[0] = false;
			}
			if (Math.Abs(v1.Z - v2.Z) < epsilon)
			{
				//�ϲ�1��2
				total--;
				m[1] = false;
			}

			CustomVertex.PositionNormalColored[] v = new CustomVertex.PositionNormalColored[total];
			int pos = 0;
			for (int i = 0; i < 4; i++)
				if (m[i]) v[pos++] = poly4[offset + i];

			return v; //�򻯳��ߡ������λ������ı���
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void CreateWallMesh(Device device)
		{
			if (vexbuf == null)
			{
				Debug.WriteLine("[WallMesh] ������"); //��ʱ������������κδ�ֱ����Ҫ����
				mesh = null; 
				return;
			}
			
			int totalf = vexbuf.Length / 4 * 2; //����������
			int totalv = vexbuf.Length;			//��������

			//�����������
			mesh = new Mesh(totalf, totalv, MeshFlags.Dynamic, CustomVertex.PositionNormalColored.Format, device);

			//����mesh
			mesh.SetVertexBufferData(vexbuf, LockFlags.None);
			mesh.SetIndexBufferData(idxbuf, LockFlags.None);
			DEBUG_PrintMeshInfo("[WallMesh] û���Ż�");

			//�Ż�����������ɾ�����ඥ�㣬ֻ�м��������ظ��Ķ���Ž����ڽ�
			WeldEpsilons epsilon = new WeldEpsilons();
			epsilon.Diffuse = 0.01F;
			epsilon.Position = 0.01F;
			epsilon.Normal = 0.01F;
			mesh.WeldVertices(WeldEpsilonsFlags.WeldPartialMatches, epsilon, null, null);
			DEBUG_PrintMeshInfo("[WallMesh] �����Ż�");

			//���cache�����ʽ����Ż�����
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
	}
}