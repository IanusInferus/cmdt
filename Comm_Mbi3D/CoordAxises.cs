using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Comm_Mbi3D
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	public class CoordAxies
	{
		AxisArrow[] arrows;							//所有的坐标箭头	
		VertexBuffer linebuf;						//坐标轴顶点
		CustomVertex.PositionColored[] linearray;	//三根线

		bool first;									//首次创建标志
		float radius;								//始终保存参考模型的center
		Vector3 center;								//始终保存参考模型的center

		static float SCALE = 1f;					//坐标轴长度的scale

		public CoordAxies()
		{
			first = true;

			arrows = new AxisArrow[3];
			arrows[0] = new AxisArrow(Color.Red);
			arrows[1] = new AxisArrow(Color.Green);
			arrows[2] = new AxisArrow(Color.Blue);
		}

		public void CreateAllBuffer(Device device, Vector3 center, float radius)
		{
			if (first)
			{
				CreateAxieLinesBuffer(device, center, radius);

				arrows[0].CreateAllBuffer(device, AxisArrow.Point_To_X_Axis(center, radius, SCALE));
				arrows[1].CreateAllBuffer(device, AxisArrow.Point_To_Y_Axis(center, radius, SCALE));
				arrows[2].CreateAllBuffer(device, AxisArrow.Point_To_Z_Axis(center, radius, SCALE));

				//只有第一次创建时才更新center,radius
				this.center = center;
				this.radius = radius;
				this.first = false;
			}
			else
			{
				//非首次创建，则始终使用参考模型的center、radius
				CreateAxieLinesBuffer(device, this.center, this.radius);

				arrows[0].CreateAllBuffer(device, AxisArrow.Point_To_X_Axis(this.center, this.radius, SCALE));
				arrows[1].CreateAllBuffer(device, AxisArrow.Point_To_Y_Axis(this.center, this.radius, SCALE));
				arrows[2].CreateAllBuffer(device, AxisArrow.Point_To_Z_Axis(this.center, this.radius, SCALE));
			}
		}

		public void DisposeAllBuffer()
		{
			for (int i = 0; i < 3; i++)
				arrows[i].DisposeAllBuffer();

			DisposeAxieLinesBuffer();
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public void Render(Device device)
		{
			device.VertexFormat = CustomVertex.PositionColored.Format;
			device.RenderState.FillMode = FillMode.Solid;
			device.SetTexture(0, null);

			device.SetStreamSource(0, linebuf, 0);
			device.DrawPrimitives(PrimitiveType.LineList, 0, 3);

			for (int i = 0; i < 3; i++)
			{
				device.SetStreamSource(0, arrows[i].vexbuf, 0);
				device.Indices = arrows[i].idxbuf;
				device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 5, 0, 6);
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		void DisposeAxieLinesBuffer()
		{
			if (linebuf != null)
			{
				linebuf.Dispose();
				linebuf = null;
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		void CreateAxieLinesBuffer(Device device, Vector3 center, float radius)
		{
			if (linebuf == null)
			{
				linearray = new CustomVertex.PositionColored[6];

				int pos = 0;
				linearray[pos].X = linearray[pos].Y = linearray[pos].Z = 0;
				linearray[pos].Color = Color.Red.ToArgb();
				pos++;
				linearray[pos].X = radius * SCALE;
				linearray[pos].Y = 0;
				linearray[pos].Z = 0;
				linearray[pos].Color = Color.Red.ToArgb();
				pos++;

				linearray[pos].X = linearray[pos].Y = linearray[pos].Z = 0;
				linearray[pos].Color = Color.Green.ToArgb();
				pos++;
				linearray[pos].X = 0;
				linearray[pos].Y = radius * SCALE;
				linearray[pos].Z = 0;
				linearray[pos].Color = Color.Green.ToArgb();
				pos++;

				linearray[pos].X = linearray[pos].Y = linearray[pos].Z = 0;
				linearray[pos].Color = Color.Blue.ToArgb();
				pos++;
				linearray[pos].X = 0;
				linearray[pos].Y = 0;
				linearray[pos].Z = radius * SCALE;
				linearray[pos].Color = Color.Blue.ToArgb();
				pos++;

				for (int i = 0; i < 6; i++)
				{
					linearray[i].X += center.X;
					linearray[i].Y += center.Y;
					linearray[i].Z += center.Z;
				}

				//由于是一次性创建，并不是每一帧都修改，因此使用静态顶点缓冲
				linebuf = new VertexBuffer(
					   typeof(CustomVertex.PositionColored),	//顶点类型
					   6,				   						//顶点个数
					   device,
					   Usage.WriteOnly,
					   CustomVertex.PositionColored.Format,		//顶点格式
					   Pool.Default);

				linebuf.SetData(linearray, 0, LockFlags.None);
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	class AxisArrow
	{
		public VertexBuffer vexbuf;
		CustomVertex.PositionColored[] vex;

		short[] idx;
		public IndexBuffer idxbuf;

		public AxisArrow(Color color)
		{
			float R = 0.8f, S = 2.4f;
			int c = color.ToArgb();

			int pos = 0;
			vex = new CustomVertex.PositionColored[5];
			vex[pos++] = new CustomVertex.PositionColored(R, R, 0, c);
			vex[pos++] = new CustomVertex.PositionColored(R, -R, 0, c);
			vex[pos++] = new CustomVertex.PositionColored(-R, -R, 0, c);
			vex[pos++] = new CustomVertex.PositionColored(-R, R, 0, c);
			vex[pos++] = new CustomVertex.PositionColored(0, 0, S, c);

			pos = 0;
			idx = new short[18];
			idx[pos++] = 4; idx[pos++] = 0; idx[pos++] = 3;
			idx[pos++] = 4; idx[pos++] = 1; idx[pos++] = 0;
			idx[pos++] = 4; idx[pos++] = 2; idx[pos++] = 1;
			idx[pos++] = 4; idx[pos++] = 3; idx[pos++] = 2;
			idx[pos++] = 0; idx[pos++] = 1; idx[pos++] = 2;
			idx[pos++] = 0; idx[pos++] = 2; idx[pos++] = 3;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public void CreateAllBuffer(Device device, Matrix trans)
		{
			if (vexbuf == null)
			{
				CustomVertex.PositionColored[] v = new CustomVertex.PositionColored[5];

				for (int i = 0; i < 5; i++)
				{
					v[i] = vex[i];
					v[i].Position = Vector3.TransformCoordinate(vex[i].Position, trans);
				}

				//由于是一次性创建，并不是每一帧都修改，因此使用静态顶点缓冲
				vexbuf = new VertexBuffer(
				   typeof(CustomVertex.PositionColored),	//顶点类型
				   vex.Length,								//顶点个数
				   device,
				   Usage.WriteOnly,
				   CustomVertex.PositionColored.Format,		//顶点格式
				   Pool.Default);

				vexbuf.SetData(v, 0, LockFlags.None);
			}

			if (idxbuf == null)
			{
				//由于是一次性创建，并不是每一帧都修改，因此使用静态索引缓冲
				idxbuf = new IndexBuffer(typeof(short), idx.Length, device, Usage.WriteOnly, Pool.Default);
				idxbuf.SetData(idx, 0, LockFlags.None);
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public void DisposeAllBuffer()
		{
			if (vexbuf != null) { vexbuf.Dispose(); vexbuf = null; }
			if (idxbuf != null) { idxbuf.Dispose(); idxbuf = null; }
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		static public Matrix Point_To_X_Axis(Vector3 center, float radius, float scale)
		{
			Matrix m = Matrix.Scaling(radius / 30f, radius / 30f, radius / 30f);
			m *= Matrix.RotationY((float)(Math.PI / 2));
			m *= Matrix.Translation(center);
			m.M41 += scale * radius;
			return m;
		}

		static public Matrix Point_To_Y_Axis(Vector3 center, float radius, float scale)
		{
			Matrix m = Matrix.Scaling(radius / 30f, radius / 30f, radius / 30f);
			m *= Matrix.RotationX((float)(-Math.PI / 2));
			m *= Matrix.Translation(center);
			m.M42 += scale * radius;
			return m;
		}

		static public Matrix Point_To_Z_Axis(Vector3 center, float radius, float scale)
		{
			Matrix m = Matrix.Scaling(radius / 30f, radius / 30f, radius / 30f);
			m *= Matrix.Translation(center);
			m.M43 += scale * radius;
			return m;
		}
	}
}
