using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;

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

				this.center = center;
				this.radius = radius;
				this.first = false;
			}
			else
			{
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
			device.SetRenderState(RenderState.FillMode, FillMode.Solid);
			device.SetTexture(0, null);

			device.SetStreamSource(0, linebuf, 0, CustomVertex.PositionColored.SizeBytes);
			device.DrawPrimitives(PrimitiveType.LineList, 0, 3);

			for (int i = 0; i < 3; i++)
			{
				device.SetStreamSource(0, arrows[i].vexbuf, 0, CustomVertex.PositionColored.SizeBytes);
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

				linebuf = new VertexBuffer(
					device,
					6 * CustomVertex.PositionColored.SizeBytes,
					Usage.WriteOnly,
					CustomVertex.PositionColored.Format,
					Pool.Default);

				using (DataStream ds = linebuf.Lock(0, 0, LockFlags.None))
					ds.WriteRange(linearray);
				linebuf.Unlock();
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

				vexbuf = new VertexBuffer(
					device,
					vex.Length * CustomVertex.PositionColored.SizeBytes,
					Usage.WriteOnly,
					CustomVertex.PositionColored.Format,
					Pool.Default);

				using (DataStream ds = vexbuf.Lock(0, 0, LockFlags.None))
					ds.WriteRange(v);
				vexbuf.Unlock();
			}

			if (idxbuf == null)
			{
				idxbuf = new IndexBuffer(device, idx.Length * sizeof(short), Usage.WriteOnly, Pool.Default, true);
				using (DataStream ds = idxbuf.Lock(0, 0, LockFlags.None))
					ds.WriteRange(idx);
				idxbuf.Unlock();
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
