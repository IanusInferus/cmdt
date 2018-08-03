using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace Comm_Sec3D
{
	public class FPoint
	{
		public int idx;
		public float x;
		public float y;
	}
	public class Border
	{
		public int idx;
		public FPoint from;
		public FPoint to;
		public int belong_district;		//自身所在区域
		public int neighbor_district;	//邻接区域
	}

	public class District
	{
		public int num_borders;
		
		public byte[] attributes; //2*4byte
		
		public float tanX;
		public float tanY;
		public float M;
		
		public byte[] not_sure; //24bytes or 16bytes
		
		public float minx;
		public float miny;
		public float minz;
		
		public float maxx;
		public float maxy;
		public float maxz;
		
		public Border[] borders; //num_borders个
	}

	public class Sec
	{
		public int num_points;
		public int num_borders;
		public int num_districts;
		public int num_xdistricts;

		public FPoint[] points;
		public Border[] borders;
		public District[] districts;

		public int version = 0;

		public Sec(string fn)
		{
			using (FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					int f1 = br.ReadInt32();
					int f2 = br.ReadInt32();

					if (f1 == 1 && f2 == 1)
						version = 2;
					else if (f1 == 2 && f2 == 1)
						version = 3;
					else
						Debug.Assert(false);

					while (br.ReadInt32() != 0x3150414d) ;

					/////////////////////////////////////////////////////////////////

					br.BaseStream.Seek(7 * 4, SeekOrigin.Current);

					/////////////////////////////////////////////////////////////////
					num_points = br.ReadInt32();
					num_borders = br.ReadInt32();
					num_districts = br.ReadInt32();
					num_xdistricts = br.ReadInt32();

					/////////////////////////////////////////////////////////////////

					points = new FPoint[num_points];
					for (int i = 0; i < num_points; i++)
					{
						FPoint fp = new FPoint();
						fp.idx = i;
						fp.x = br.ReadSingle();
						fp.y = br.ReadSingle();
						points[i] = fp;
					}

					CalculateMinMax(); //计算最大最小值

					/////////////////////////////////////////////////////////////////

					borders = new Border[num_borders];
					for (int i = 0; i < num_borders; i++)
					{
						Border bb = new Border();

						bb.idx = i;
						
						int idx = br.ReadInt32();
						bb.from = points[idx];

						idx = br.ReadInt32();
						bb.to = points[idx];

						bb.belong_district = br.ReadInt32();
						bb.neighbor_district = br.ReadInt32();

						br.ReadInt32();//DO NOT KNOW ABOUT IT YET
						
						borders[i] = bb;
					}

					/////////////////////////////////////////////////////////////////

					districts = new District[num_districts];
					for (int i = 0; i < num_districts; i++)
					{
						District dd = new District();

						dd.num_borders = br.ReadInt32();
						dd.attributes = br.ReadBytes(8);
						dd.tanX = br.ReadSingle();
						dd.tanY = br.ReadSingle();
						dd.M = br.ReadSingle();
						
						if (version==2)
							dd.not_sure = br.ReadBytes(24);
						else if (version==3)
							dd.not_sure = br.ReadBytes(16);

						dd.minx = br.ReadSingle();
						dd.miny = br.ReadSingle();
						dd.minz = br.ReadSingle();
						dd.maxx = br.ReadSingle();
						dd.maxy = br.ReadSingle();
						dd.maxz = br.ReadSingle();

						dd.borders = new Border[dd.num_borders];
						for (int j = 0; j < dd.num_borders; j++)
						{
							int idx = br.ReadInt32();
							dd.borders[j] = borders[idx];
						}
						districts[i] = dd;
					}
				}
			}
			ValidateSecData(); //这个修正非常强！效果极好！
		}

		void ValidateSecData()
		{
			int total = 0;
			for (int i = 0; i < num_districts; i++)
			{
				District d = districts[i];
				for (int j = 0; j < d.num_borders; j++)
				{
					if (d.borders[j].belong_district != i)
					{
						d.borders[j].belong_district = i;
						total++;
					}
				}
			}
			if (total != 0)
			{
				string msg = string.Format("调试信息：共发现{0}处不一致的数据.", total);
				Debug.WriteLine(msg);
			}
		}

		/////////////////////////////////////////////////////////////////
		/*
		//计算多边形面积
		float GetPolygonAreaOfDistrict(int idx)
		{
			District d = ld[idx];

			float result = 0.0F;
			for (int i = 0; i < d.num_borders; i++)
			{
				Border b = d.border[i];
				result += (b.to.x - b.from.x) * (b.to.y + b.from.y) / 2;
			}

			return Math.Abs(result);
		}

		//计算多边形质心位置
		FPoint GetPolyonCentroidOfDistrict(int idx)
		{
			float area = GetPolygonAreaOfDistrict(idx);

			FPoint fp = new FPoint();

			District d = ld[idx];
			for (int i = 0; i < d.num_borders; i++)
			{
				Border b = d.border[i];
				FPoint from = b.from;
				FPoint to = b.to;

				fp.x += (from.x + to.x) * (from.x * to.y - to.x * from.y);
				fp.y += (from.y + to.y) * (from.x * to.y - to.x * from.y);
			}

			fp.x /= 6 * area;
			fp.y /= 6 * area;

			if (fp.x < 0)
			{
				fp.x = -fp.x;
				fp.y = -fp.y;
			}

			return fp;
		}
		*/
		/////////////////////////////////////////////////////////////////

		public float minx;
		public float maxx;
		public float miny;
		public float maxy;
		public void CalculateMinMax()
		{
			maxx = float.NegativeInfinity;
			minx = float.PositiveInfinity;
			maxy = float.NegativeInfinity;
			miny = float.PositiveInfinity;

			for (int i = 0; i < num_points; i++)
			{
				if (points[i].x > maxx) maxx = points[i].x;
				if (points[i].x < minx) minx = points[i].x;
				if (points[i].y > maxy) maxy = points[i].y;
				if (points[i].y < miny) miny = points[i].y;
			}
		}
	}
}
