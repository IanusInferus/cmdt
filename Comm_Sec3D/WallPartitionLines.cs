using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;

using Microsoft.DirectX.Direct3D;

namespace Comm_Sec3D
{
	////////////////////////////////////////////////////////////////////////////////////
	//��ֱ����ָ��߿�
	class WallPartitionLines
	{
		Sec sec;

		CustomVertex.PositionColored[] pvexs;
		////////////////////////////////////////////////////////////////////////////////////
		public VertexBuffer vertexbuf;

		public int NumberOfLines
		{
			get { return pvexs.Length / 2; }
		}
		////////////////////////////////////////////////////////////////////////////////////
		Color color;
		public WallPartitionLines(Sec sec, Color color)
		{
			this.sec = sec;
			this.color = color;

			GenerateAllWallPartitionLines();
		}
		////////////////////////////////////////////////////////////////////////////////////
		private void GenerateAllWallPartitionLines() //����������
		{
			//��������border�ظ���Hash Table
			Dictionary<ulong, bool> border_dict = new Dictionary<ulong, bool>(sec.num_borders);

			CustomVertex.PositionColored[] v = new CustomVertex.PositionColored[sec.num_borders * 2 * 2];

			int pos = 0;
			for (int i = 0; i < sec.num_borders; i++)
			{
				Border b = sec.borders[i];

				if (b.belong_district == -1 || b.neighbor_district == -1) continue;

				//����ʹ�����������޳����ظ��ķ�ʽ������SBex��PTex(Ǳͧ��ڡ���ĸβ��)������
				//if (b.belong_district >= b.neighbor_district) continue;

				//Ϊʲôֻ���ñ��Ƿ�ʽ���������ظ���������Ϊ��.sec�У�������ÿ��border�������س������ε�
				bool ret;
				ulong x = (ulong)b.belong_district;
				ulong y = (ulong)b.neighbor_district;
				ulong key = (x << 32) | y;

				if (border_dict.TryGetValue(key, out ret))
					//�Ѿ��Ǽǹ��ˣ��ظ���
					continue;
				else
				{
					//û�еǼǹ����Ǽǽ�Hash Table
					key = (y << 32) | x;
					border_dict.Add(key, true);
				}

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

				if (Math.Abs(z1 - z3) > 0.05F)
				{
					v[pos++] = new CustomVertex.PositionColored(from.x, from.y, z1, color.ToArgb());
					v[pos++] = new CustomVertex.PositionColored(from.x, from.y, z3, color.ToArgb());
				}
				if (Math.Abs(z2 - z4) > 0.05F)
				{
					v[pos++] = new CustomVertex.PositionColored(to.x, to.y, z2, color.ToArgb());
					v[pos++] = new CustomVertex.PositionColored(to.x, to.y, z4, color.ToArgb());
				}
			}

			if (pos == 0)
			{
				pvexs = null; //���������ڴ�ֱ����Ҫ����ֱ���˳���
				return;
			}

			pvexs = new CustomVertex.PositionColored[pos];

			for (int i = 0; i < pvexs.Length; i++)
				pvexs[i] = v[i];
		}
		////////////////////////////////////////////////////////////////////////////////////
		public void CreateWallPartitionLinesVertexBuffer(Device device)
		{
			//ע��! �˴���pvexs.Length���ܵ���0! ��ʱ�����������ڴ�ֱ��������
			if (pvexs == null)
			{
				Debug.WriteLine("[WallPartitionLines] ������"); 
				vertexbuf = null;
				return;
			}

			vertexbuf = new VertexBuffer(
				typeof(CustomVertex.PositionColored),
				pvexs.Length,
				device,
				Usage.WriteOnly,
				CustomVertex.PositionColored.Format,
				Pool.Default);

			CustomVertex.PositionColored[] v = (CustomVertex.PositionColored[])vertexbuf.Lock(0, 0);
			for (int i = 0; i < pvexs.Length; i++)
				v[i] = pvexs[i];

			vertexbuf.Unlock();

			string msg = String.Format("[WallLines] û���Ż�\t v:{0}", pvexs.Length);
			Debug.WriteLine(msg);
		}
	}
}