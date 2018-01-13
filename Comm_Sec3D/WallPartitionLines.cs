using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;

using Microsoft.DirectX.Direct3D;

namespace Comm_Sec3D
{
	////////////////////////////////////////////////////////////////////////////////////
	//垂直区域分割线框
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
		private void GenerateAllWallPartitionLines() //存在着冗余
		{
			//用于消除border重复的Hash Table
			Dictionary<ulong, bool> border_dict = new Dictionary<ulong, bool>(sec.num_borders);

			CustomVertex.PositionColored[] v = new CustomVertex.PositionColored[sec.num_borders * 2 * 2];

			int pos = 0;
			for (int i = 0; i < sec.num_borders; i++)
			{
				Border b = sec.borders[i];

				if (b.belong_district == -1 || b.neighbor_district == -1) continue;

				//不能使用如下这种剔除边重复的方式，否则SBex、PTex(潜艇库口、航母尾部)不正常
				//if (b.belong_district >= b.neighbor_district) continue;

				//为什么只能用薄记方式来消除边重复？这是因为，.sec中，并不是每条border都会来回出现两次的
				bool ret;
				ulong x = (ulong)b.belong_district;
				ulong y = (ulong)b.neighbor_district;
				ulong key = (x << 32) | y;

				if (border_dict.TryGetValue(key, out ret))
					//已经登记过了，重复了
					continue;
				else
				{
					//没有登记过，登记进Hash Table
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
				pvexs = null; //根本不存在垂直线需要画，直接退出！
				return;
			}

			pvexs = new CustomVertex.PositionColored[pos];

			for (int i = 0; i < pvexs.Length; i++)
				pvexs[i] = v[i];
		}
		////////////////////////////////////////////////////////////////////////////////////
		public void CreateWallPartitionLinesVertexBuffer(Device device)
		{
			//注意! 此处的pvexs.Length可能等于0! 此时，根本不存在垂直区域网格
			if (pvexs == null)
			{
				Debug.WriteLine("[WallPartitionLines] 空数据"); 
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

			string msg = String.Format("[WallLines] 没有优化\t v:{0}", pvexs.Length);
			Debug.WriteLine(msg);
		}
	}
}