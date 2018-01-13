using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Diagnostics;

namespace Comm_Mbi3D
{
	class FreeRotateCamera //ʹ��ArcBall�㷨��������ת�����
	{
		public Matrix rotate;	//������ת����

		public float r;			//ʵ�ʰ뾶
		private float radius;	//�ο��뾶

		private bool updating;	//�Ϸ����±�־
		private Vector3 from, to;

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public FreeRotateCamera(float r)
		{
			rotate = Matrix.RotationY((float)Math.PI * 3 / 4) * Matrix.RotationX(-(float)Math.PI / 3F);

			radius = r;
			this.r = 2.5F * radius;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public Matrix GetViewMatrix()
		{
			//����ϵ��-Zָ���Լ���+Xָ�����ҷ���Y��ָ�����Ϸ�
			return Matrix.LookAtLH(		//view�任
				new Vector3(0, 0, -r),	//eye��cameraλ��-Z����
				new Vector3(0, 0, 0),	//at
				new Vector3(0, 1, 0));	//up��+YΪcamera���Ϸ�
		}

		public void IncreaseRadius(float d)
		{
			if (r + d <= 6 * radius) r += d; else r = 6 * radius;
		}

		public void DecreaseRadius(float d)
		{
			if (r - d >= 0.04 * radius) r -= d; else r = 0.04f * radius;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		//��֪���λ�á����ڿ�ߣ����Ӧ�İ���������
		//ע��X��Y����ά����ϵ���ӳ���ϵ������ļ�������View Matrix�ķ����趨��صģ�
		//��һ�㷨������NeHe��OpenGL tutorial�������ƽ���ArcBall�����acos��ʽ��Ƕ��ƺ����Ӿ�ȷ
		private void Hemisphere(float X, float Y, float W, float H, out Vector3 ret)
		{
			//�Ȱ�X��Y�ֱ�ӳ�䵽[-1,1]��Χ
			X = (X - W / 2) / (W / 2);
			Y = (H / 2 - Y) / (H / 2);

			//��Ӧ��View Matrix�ķ����趨
			float x = X;
			float y = Y;
			float length = x * x + y * y;

			float z;
			if (length > 1.0f)
			{
				//��xΪ0��Ȼ���ʵ�����y��z����֮����Ҫ��֤x^2+y^2+z^2=1
				float norm = 1.0f / (float)Math.Sqrt(length);
				x = x * norm;
				y = y * norm;
				z = 0.0f;
			}
			else
				z = (float)Math.Sqrt(1 - length);

			ret = new Vector3(x, y, -z); //���������-z����Ϊ�����λ��-z����
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		Matrix mm;

		public void StartRotate(int X, int Y, Size size)
		{
			Hemisphere(X, Y, size.Width, size.Height, out from);
			mm = rotate;		//��ʱ���浱ǰrotate

			updating = true;	//��ת��ʼ
		}

		public void UpdateRotateAccurately(int X, int Y, Size size)
		{
			//������StartRotate������UpdateRotate�������˳�
			if (!updating) return;

			Hemisphere(X, Y, size.Width, size.Height, out to);

			float dot = (float)Vector3.Dot(from, to);
			Vector3 axis = Vector3.Cross(from, to);

			//from��to������λʸ���Ĳ�ˡ����ƴװ��һ��Quaternion
			//���Quaternion���ñ�ʾ�˴�from��to��2���Ƕ���ת��һ��������ϸ��
			//��һ�㷨������NeHe��OpenGL tutorial�������ƽ���ArcBall
			//����㷨��acos��ʽ��Ƕ��ƺ����Ӿ�ȷ
			Quaternion q = new Quaternion(axis.X, axis.Y, axis.Z, dot);

			//��Quaternionת����Matrix
			Matrix m = Matrix.RotationQuaternion(q);

			//�Ȼ�����ת��Ȼ������ת���������ַ�����ÿ�θ���һ��С������ת�ķ�ʽ���ã���Ϊ������������
			rotate = mm * m;
		}

		public void EndRotate()
		{
			updating = false;	//��ת����
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public void Reset()
		{
			from = to = new Vector3();
			rotate = Matrix.RotationY((float)Math.PI * 3 / 4) * Matrix.RotationX(-(float)Math.PI / 3F);
			r = 2.5F * radius;
		}
	}
}