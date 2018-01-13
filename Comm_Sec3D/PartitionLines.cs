//TO THINK: �᲻����ָ��������������߿���Ҫ���Ƶ������

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;

using Microsoft.DirectX.Direct3D;

namespace Comm_Sec3D
{
	////////////////////////////////////////////////////////////////////////////////////
	//ˮƽ����ָ��߿�
	class PartitionLines
	{
		CustomVertex.PositionColored[] pvexs;
		////////////////////////////////////////////////////////////////////////////////////
		public VertexBuffer vertexbuf;

		public int NumberOfLines
		{
			get { return pvexs.Length / 2; }
		}
		////////////////////////////////////////////////////////////////////////////////////

		Color color;
		public PartitionLines(Polygon[] polys, Color color)
		{
			this.color = color;
			GenerateAllPartitionLines(polys);
		}
		////////////////////////////////////////////////////////////////////////////////////
		private void GenerateAllPartitionLines(Polygon[] p)
		{
			int total = 0; //���б��ܺ�(�˴�û���ظ���)
			for (int i = 0; i < p.Length; i++)
				total += p[i].vertex.Length;

			pvexs = new CustomVertex.PositionColored[total * 2];

			int pos = 0;
			for (int i = 0; i < p.Length; i++)
			{
				Polygon pi = p[i];
				for (int j = 0; j < pi.vertex.Length; j++)
				{
					CustomVertex.PositionNormalColored from = pi.vertex[j];
					CustomVertex.PositionNormalColored to = pi.vertex[(j + 1) % pi.vertex.Length];

					pvexs[pos++] = new CustomVertex.PositionColored(from.X, from.Y, from.Z, color.ToArgb());
					pvexs[pos++] = new CustomVertex.PositionColored(to.X, to.Y, to.Z, color.ToArgb());
				}
			}
		}
		////////////////////////////////////////////////////////////////////////////////////
		public void CreatePartitionLinesVertexBuffer(Device device)
		{
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

			string msg = String.Format("[Lines] û���Ż�\t v:{0}", pvexs.Length);
			Debug.WriteLine(msg);
		}
	}
}
