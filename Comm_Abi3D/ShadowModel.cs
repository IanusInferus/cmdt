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
		
		int model_idx;								//��ǰģ�ͱ��
		int color;									//������ɫ
		Vector3 n0;									//���߷���

		Vector3[] shadowv;							//����transv�������õ�X-Y��Ļ�ϵ�Ӱ�Ӷ��㼯��
		CustomVertex.PositionColored[] shadowarray; //���㻺������
		
		//-------------------------------------------------------------------------------------------------------
		public VertexBuffer shadowbuf;	//���㻺����
		public int totalf;				//ʵ��������������

		//-------------------------------------------------------------------------------------------------------
		public ShadowModel(ABI abi, AnimatedModel am, Vector3 n, int color)
		{
			this.abi = abi;
			this.am = am;
			this.color = color;

			n0 = n;
			n0.Normalize();
			Debug.Assert(Math.Abs(n0.Z) > float.Epsilon); //n.Z���ܵ���0

			model_idx = 0;

			CalculateAll();
		}

		//-------------------------------------------------------------------------------------------------------
		void CalculateAll()
		{
			//������Ӱ��������Ӱ����
			//�������ֻ��model_idx�Լ�am.transv�йأ���animation�޹�
			UpdateVertexSet(n0); //�������Ϊshadowv
			GenerateVertexBuf(color);
		}

		public void SetModel(Device device, int model)
		{
			model_idx = model % (abi.models.Length);

			if (shadowbuf != null)
			{
				CalculateAll();

				//ģ��һ���ı�Ļ��������ؽ����㻺��
				shadowbuf.Dispose();
				CreateVertexBuffer(device);
			}
		}

		public void Animation()
		{
			if (shadowbuf != null)
			{
				CalculateAll();

				//�����ؽ���ֱ�����ö��㻺��
				shadowbuf.SetData(shadowarray, 0, LockFlags.Discard);
			}
		}

		//-------------------------------------------------------------------------------------------------------
		//���ݷ����Դ��transv���㶥�㼯����X-Yƽ���ϵ�ͶӰ�㣬����Ӱ���㼯�ϣ����ս��Ϊshadow[]
		void UpdateVertexSet(Vector3 n0)
		{
			Vertex[] transv = am.transv; //BAD SMELL����̫����.......

			shadowv = new Vector3[transv.Length];
			for (int i = 0; i < transv.Length; i++)
			{
				Vector3 p0 = new Vector3(transv[i].X, transv[i].Y, transv[i].Z);
				float t = -p0.Z / n0.Z;
				shadowv[i] = p0 + t * n0;
			}
		}
		
		//-------------------------------------------------------------------------------------------------------
		void GenerateVertexBuf(int color) //��shadowд��󱸶��㻺��
		{
			totalf = 0; //��������Ŀ
			foreach (Polygon po in abi.models[model_idx].polygon)
				totalf += po.num_lines - 2;

			int totalv = 3 * totalf;  //������Ŀ
			shadowarray = new CustomVertex.PositionColored[totalv]; //���������ܶ����������������㼯

			int pos = 0;
			foreach (Polygon po in abi.models[model_idx].polygon)
			{
				//-------------------------------------------------------------------------------
				//ɸѡ������Ӱ�ӵ�����棬����Ӱ�Ӳ���˫��ģ��ٻ���������ѣ�Ч����������		
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
				for (int j = 0; j < po.num_lines - 2; j++) //�Ѷ����ת��Ϊ�����Σ��ŵ����㼯����
				{
					//ע��!ģ�ͼ��㶼������ϵ
					//�㷨�кܴ���ߵĿռ䣬��Ҫʵ����Ӱ������С������ֻ�������
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

			totalf = pos / 3; //ʵ����Ҫ���������
		}

		//-------------------------------------------------------------------------------------------------------
		public void CreateVertexBuffer(Device device)
		{
			shadowbuf = new VertexBuffer(
			   typeof(CustomVertex.PositionColored),	//��������
			   shadowarray.Length,						//ע�⣡�����ܵĶ����������
			   device,
			   Usage.WriteOnly | Usage.Dynamic,
			   CustomVertex.PositionColored.Format,		//�����ʽ
			   Pool.Default);

			shadowbuf.SetData(shadowarray, 0, LockFlags.Discard);
		}
	}
}