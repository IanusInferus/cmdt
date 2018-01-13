using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Comm_ABI
{
	public partial class Form1 : Form
	{
		ABI abi;
		int idx;

		public Form1()
		{
			InitializeComponent();

			abi = new ABI("f:/ABI/CONDUC.ABI");
		}

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			Rectangle r1 = new Rectangle(0, 0, abi.textureinfo[idx].width, abi.textureinfo[idx].height);
			Rectangle r2 = new Rectangle(0, 0, abi.textureinfo[idx].width*2, abi.textureinfo[idx].height*2);

			g.DrawImage(abi.bmps[idx], r2, r1, GraphicsUnit.Pixel);

			Pen pen = new Pen(Color.Red);
			foreach (Dress d in abi.dress)
			{
				foreach (Polygon po in d.polygon)
				{
					if (po.texture_id == idx)
					{
						for (int k = 0; k < po.num_lines; k++)
						{
							int xxx = (k + 1) % po.num_lines;
							g.DrawLine(pen, po.map_points[k].U, po.map_points[k].V, po.map_points[xxx].U, po.map_points[xxx].V);
						}
					}
				}
			}
		}

		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Right:
				case Keys.Space:
					idx = (idx + 1) % abi.bmps.Length;
					break;
				case Keys.Left:
					idx = (idx - 1 + abi.bmps.Length) % abi.bmps.Length;
					break;
				case Keys.Q:
				case Keys.Escape:
					Close();
					break;
			}
			Text = abi.textureinfo[idx].name;
			this.Refresh();
		}
	}
}