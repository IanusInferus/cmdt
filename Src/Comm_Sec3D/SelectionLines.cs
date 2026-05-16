using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;

using SlimDX;
using SlimDX.Direct3D9;

namespace Comm_Sec3D
{
	//高亮显示的面选择线框
	public class SelectionLines : IDisposable
	{
		CustomVertex.PositionColored[] pvexs;
		////////////////////////////////////////////////////////////////////////////////////
		public VertexBuffer vertexbuf;

		public int NumberOfLines
		{
			get { return pvexs.Length / 2; }
		}

		////////////////////////////////////////////////////////////////////////////////////
		Polygon poly;
		public SelectionLines(Polygon poly)
		{
			this.poly = poly;

			GenerateSelectionLines();
		}

		////////////////////////////////////////////////////////////////////////////////////
		private void GenerateSelectionLines()
		{
			pvexs = new CustomVertex.PositionColored[poly.vertex.Length * 2];

			int pos = 0;
			for (int i = 0; i < poly.vertex.Length; i++)
			{
				CustomVertex.PositionNormalColored from = poly.vertex[i];
				CustomVertex.PositionNormalColored to = poly.vertex[(i + 1) % poly.vertex.Length];

				pvexs[pos++] = new CustomVertex.PositionColored(from.Position, Color.Aqua.ToArgb()); ;
				pvexs[pos++] = new CustomVertex.PositionColored(to.Position, Color.Aqua.ToArgb()); ;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////
		public void CreateSelectionLines(Device device)
		{
			vertexbuf = new VertexBuffer(
				device,
				pvexs.Length * CustomVertex.PositionColored.SizeBytes,
				Usage.WriteOnly,
				CustomVertex.PositionColored.Format,
				Pool.Default);

			using (DataStream ds = vertexbuf.Lock(0, 0, LockFlags.None))
				ds.WriteRange(pvexs);
			vertexbuf.Unlock();
		}
		////////////////////////////////////////////////////////////////////////////////////
		#region IDisposable Members

		private bool _alreadyDisposed = false;

		~SelectionLines()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(true);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			// 不要多次处理
			if (_alreadyDisposed)
				return;
			if (isDisposing)
			{
				//此处释放受控资源
				vertexbuf.Dispose(); 
			}
			//此处释放非受控资源。设置被处理过标记
			_alreadyDisposed = true;
		}

		#endregion
	}
}