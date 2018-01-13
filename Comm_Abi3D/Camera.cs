using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Comm_Abi3D
{
	class Camera
	{
		//挤压Mesh的球面的原始半径
		float radius;

		////////////////////////////////////////////////////////////////////////////////////////
		//球面系中的Camera半径和方位角
		float R;
		float alpha;
		float beta;

		////////////////////////////////////////////////////////////////////////////////////////
		public Camera(float radius)
		{
			this.radius = radius;

			R = radius;
			alpha = (float)(Math.PI * 3 / 4);	//纬度：XZ平面与Y夹角（左手） [-pi/2,+pi/2]映射到[0:+pi]
			beta = (float)(Math.PI / 4);		//经度：Z与OP投影的夹角（左手） 0:2pi
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
			//将球面坐标转换成world直角坐标
			//注意，下面坐标的计算是以+Y为camera的正上方，左手系
			double t = R * Math.Cos(alpha - Math.PI / 2);
			float y = (float)(R * Math.Sin(alpha - Math.PI / 2));
			float z = (float)(t * Math.Cos(beta));
			float x = (float)(t * Math.Sin(beta));

			device.Transform.View = Matrix.LookAtLH(		//view变换
				new Vector3((float)x, (float)y, (float)z),	//camera所在的world位置
				new Vector3(0, 0, 0),						//camera正对world原点
				new Vector3(0, 1, 0));						//camera以+Y为正上方
		}

		////////////////////////////////////////////////////////////////////////////////////////
		//半径 Radius
		public void IncreaseRadius(float d)
		{
			if (R + d <= 8 * radius) R += d; else R = 8 * radius;
		}
		public void DecreaseRadius(float d)
		{
			if (R - d >= 0.02 * radius) R -= d; else R = 0.05F * radius;
		}

		////////////////////////////////////////////////////////////////////////////////////////
		//纬度 Latitude
		public void IncreaseLatitude(float d)
		{
			if (alpha + d <= Math.PI) alpha += d; else alpha = (float)Math.PI - 0.001F;
		}
		public void DecreaseLatitude(float d)
		{
			if (alpha - d >= 0) alpha -= d; else alpha = 0.001F;
		}

		////////////////////////////////////////////////////////////////////////////////////////
		//经度 Longitude
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
