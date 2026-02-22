using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;

namespace Comm_Abi3D
{
	class ShadowModel
	{
		ABI abi;
		AnimatedModel am;
		
		int model_idx;								//当前模型编号
		int color;									//顶点颜色
		Vector3 n0;									//光线方向

		Vector3[] shadowv;							//根据transv计算所得的X-Y屏幕上的影子顶点集合
		CustomVertex.PositionColored[] shadowarray; //顶点缓冲数据
		
		//-------------------------------------------------------------------------------------------------------
		public VertexBuffer shadowbuf;	//顶点缓冲区
		public int totalf;				//实际三角形总面数

		//-------------------------------------------------------------------------------------------------------
		public ShadowModel(ABI abi, AnimatedModel am, Vector3 n, int color)
		{
			this.abi = abi;
			this.am = am;
			this.color = color;

			n0 = n;
			n0.Normalize();
			Debug.Assert(Math.Abs(n0.Z) > float.Epsilon); //n.Z不能等于0

			model_idx = 0;

			CalculateAll();
		}

		//-------------------------------------------------------------------------------------------------------
		void CalculateAll()
		{
			//计算阴影并生成阴影顶点
			//计算过程只和model_idx以及am.transv有关，与animation无关
			UpdateVertexSet(n0); //最终输出为shadowv
			GenerateVertexBuf(color);
		}

		public void SetModel(Device device, int model)
		{
			model_idx = model % (abi.models.Length);

			if (shadowbuf != null)
			{
				CalculateAll();

				//模型一旦改变的话，必须重建顶点缓冲
				shadowbuf.Dispose();
				CreateVertexBuffer(device);
			}
		}

		public void Animation()
		{
			if (shadowbuf != null)
			{
				CalculateAll();

				//无需重建，直接设置顶点缓冲
				shadowbuf.SetData(shadowarray, 0, LockFlags.Discard);
			}
		}

		//-------------------------------------------------------------------------------------------------------
		//根据方向光源和transv计算顶点集合在X-Y平面上的投影点，即阴影顶点集合，最终结果为shadow[]
		void UpdateVertexSet(Vector3 n0)
		{
			Vertex[] transv = am.transv; //BAD SMELL，不太优雅.......

			shadowv = new Vector3[transv.Length];
			for (int i = 0; i < transv.Length; i++)
			{
				Vector3 p0 = new Vector3(transv[i].X, transv[i].Y, transv[i].Z);
				float t = -p0.Z / n0.Z;
				shadowv[i] = p0 + t * n0;
			}
		}
		
		//-------------------------------------------------------------------------------------------------------
		void GenerateVertexBuf(int color) //将shadow写入后备顶点缓冲
		{
			totalf = 0; //三角形数目
			foreach (Polygon po in abi.models[model_idx].polygon)
				totalf += po.num_lines - 2;

			int totalv = 3 * totalf;  //顶点数目
			shadowarray = new CustomVertex.PositionColored[totalv]; //按照最大可能顶点数量来创建顶点集

			int pos = 0;
			foreach (Polygon po in abi.models[model_idx].polygon)
			{
				//-------------------------------------------------------------------------------
				//筛选出所有影子的向光面，即让影子不是双面的，少画几个面而已，效率提升不大		
				int idx0, idx1, idx2;
				idx0 = po.map_points[0].vertex_id;
				idx1 = po.map_points[1].vertex_id;
				idx2 = po.map_points[2].vertex_id;

				Vector2 a = new Vector2(
					shadowv[idx1].X - shadowv[idx0].X,
					shadowv[idx1].Y - shadowv[idx0].Y
					);

				Vector2 b = new Vector2(
					shadowv[idx2].X - shadowv[idx1].X,
					shadowv[idx2].Y - shadowv[idx1].Y
					);

				if (Vector2.Ccw(b, a) < 0) continue;

				//-------------------------------------------------------------------------------
				for (int j = 0; j < po.num_lines - 2; j++) //把多边形转换为三角形，放到顶点集合中
				{
					//注意!模型计算都是右手系
					//算法有很大提高的空间，需要实现阴影面数最小化，且只有向光面
					int idx = po.map_points[0].vertex_id;
					shadowarray[pos].X = shadowv[idx].X;
					shadowarray[pos].Y = shadowv[idx].Y;
					shadowarray[pos].Z = shadowv[idx].Z;
					shadowarray[pos].Color = color;
					++pos;

					idx = po.map_points[j + 1].vertex_id;
					shadowarray[pos].X = shadowv[idx].X;
					shadowarray[pos].Y = shadowv[idx].Y;
					shadowarray[pos].Z = shadowv[idx].Z;
					shadowarray[pos].Color = color;
					++pos;

					idx = po.map_points[j + 2].vertex_id;
					shadowarray[pos].X = shadowv[idx].X;
					shadowarray[pos].Y = shadowv[idx].Y;
					shadowarray[pos].Z = shadowv[idx].Z;
					shadowarray[pos].Color = color;
					++pos;
				}
			}

			totalf = pos / 3; //实际需要画的向光面
		}

		//-------------------------------------------------------------------------------------------------------
		public void CreateVertexBuffer(Device device)
		{
			shadowbuf = new VertexBuffer(
			   typeof(CustomVertex.PositionColored),	//顶点类型
			   shadowarray.Length,						//注意！最大可能的顶点个数！！
			   device,
			   Usage.WriteOnly | Usage.Dynamic,
			   CustomVertex.PositionColored.Format,		//顶点格式
			   Pool.Default);

			shadowbuf.SetData(shadowarray, 0, LockFlags.Discard);
		}
	}
}