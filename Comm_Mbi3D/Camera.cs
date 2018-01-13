using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Diagnostics;

namespace Comm_Mbi3D
{
	class FreeRotateCamera //使用ArcBall算法的自由旋转摄像机
	{
		public Matrix rotate;	//积累旋转矩阵

		public float r;			//实际半径
		private float radius;	//参考半径

		private bool updating;	//合法更新标志
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
			//左手系：-Z指向自己、+X指向正右方、Y轴指向正上方
			return Matrix.LookAtLH(		//view变换
				new Vector3(0, 0, -r),	//eye，camera位于-Z轴上
				new Vector3(0, 0, 0),	//at
				new Vector3(0, 1, 0));	//up，+Y为camera正上方
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
		//已知鼠标位置、窗口宽高，求对应的半球面坐标
		//注意X、Y与三维坐标系轴的映射关系！这里的计算是与View Matrix的方向设定相关的！
		//这一算法来自于NeHe的OpenGL tutorial，其名称叫做ArcBall，其比acos方式求角度似乎更加精确
		private void Hemisphere(float X, float Y, float W, float H, out Vector3 ret)
		{
			//先把X和Y分别映射到[-1,1]范围
			X = (X - W / 2) / (W / 2);
			Y = (H / 2 - Y) / (H / 2);

			//对应到View Matrix的方向设定
			float x = X;
			float y = Y;
			float length = x * x + y * y;

			float z;
			if (length > 1.0f)
			{
				//令x为0，然后适当缩放y、z，总之就是要保证x^2+y^2+z^2=1
				float norm = 1.0f / (float)Math.Sqrt(length);
				x = x * norm;
				y = y * norm;
				z = 0.0f;
			}
			else
				z = (float)Math.Sqrt(1 - length);

			ret = new Vector3(x, y, -z); //这里必须是-z，因为摄像机位于-z轴上
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		Matrix mm;

		public void StartRotate(int X, int Y, Size size)
		{
			Hemisphere(X, Y, size.Width, size.Height, out from);
			mm = rotate;		//临时保存当前rotate

			updating = true;	//旋转开始
		}

		public void UpdateRotateAccurately(int X, int Y, Size size)
		{
			//必须先StartRotate，方可UpdateRotate，否则退出
			if (!updating) return;

			Hemisphere(X, Y, size.Width, size.Height, out to);

			float dot = (float)Vector3.Dot(from, to);
			Vector3 axis = Vector3.Cross(from, to);

			//from、to两个单位矢量的叉乘、点乘拼装成一个Quaternion
			//这个Quaternion正好表示了从from到to的2倍角度旋转这一动作，仔细看
			//这一算法来自于NeHe的OpenGL tutorial，其名称叫做ArcBall
			//这个算法比acos方式求角度似乎更加精确
			Quaternion q = new Quaternion(axis.X, axis.Y, axis.Z, dot);

			//将Quaternion转换成Matrix
			Matrix m = Matrix.RotationQuaternion(q);

			//先基础旋转，然后再旋转增量，这种方法比每次更新一个小增量旋转的方式更好，因为消除了误差积累
			rotate = mm * m;
		}

		public void EndRotate()
		{
			updating = false;	//旋转结束
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