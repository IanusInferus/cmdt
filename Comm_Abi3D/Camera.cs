using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Comm_Abi3D
{
	class Camera
	{
		//��ѹMesh�������ԭʼ�뾶
		float radius;

		////////////////////////////////////////////////////////////////////////////////////////
		//����ϵ�е�Camera�뾶�ͷ�λ��
		float R;
		float alpha;
		float beta;

		////////////////////////////////////////////////////////////////////////////////////////
		public Camera(float radius)
		{
			this.radius = radius;

			R = radius;
			alpha = (float)(Math.PI * 3 / 4);	//γ�ȣ�XZƽ����Y�нǣ����֣� [-pi/2,+pi/2]ӳ�䵽[0:+pi]
			beta = (float)(Math.PI / 4);		//���ȣ�Z��OPͶӰ�ļнǣ����֣� 0:2pi
		}

		////////////////////////////////////////////////////////////////////////////////////////
		public void ResetRadius()
		{
			R = radius;
			alpha = (float)(Math.PI * 3 / 4);
			beta = (float)(Math.PI / 4);
		}

		////////////////////////////////////////////////////////////////////////////////////////
		public void SetViewTransform(Device device)
		{
			//����������ת����worldֱ������
			//ע�⣬��������ļ�������+YΪcamera�����Ϸ�������ϵ
			double t = R * Math.Cos(alpha - Math.PI / 2);
			float y = (float)(R * Math.Sin(alpha - Math.PI / 2));
			float z = (float)(t * Math.Cos(beta));
			float x = (float)(t * Math.Sin(beta));

			device.Transform.View = Matrix.LookAtLH(		//view�任
				new Vector3((float)x, (float)y, (float)z),	//camera���ڵ�worldλ��
				new Vector3(0, 0, 0),						//camera����worldԭ��
				new Vector3(0, 1, 0));						//camera��+YΪ���Ϸ�
		}

		////////////////////////////////////////////////////////////////////////////////////////
		//�뾶 Radius
		public void IncreaseRadius(float d)
		{
			if (R + d <= 8 * radius) R += d; else R = 8 * radius;
		}
		public void DecreaseRadius(float d)
		{
			if (R - d >= 0.02 * radius) R -= d; else R = 0.05F * radius;
		}

		////////////////////////////////////////////////////////////////////////////////////////
		//γ�� Latitude
		public void IncreaseLatitude(float d)
		{
			if (alpha + d <= Math.PI) alpha += d; else alpha = (float)Math.PI - 0.001F;
		}
		public void DecreaseLatitude(float d)
		{
			if (alpha - d >= 0) alpha -= d; else alpha = 0.001F;
		}

		////////////////////////////////////////////////////////////////////////////////////////
		//���� Longitude
		public void IncreaseLongitude(float d)
		{
			beta = (float)((beta + d) % (2 * Math.PI));
		}
		public void DecreaseLongitude(float d)
		{
			beta = (float)((beta - d) % (2 * Math.PI));
		}
	}
}
