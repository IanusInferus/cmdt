using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace Comm_Y64
{
	public partial class Form1 : Form
	{
		Y64 y64;
		Bitmap bmp;
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Debug.WriteLine(Environment.TickCount);
			using (y64 = new Y64("F:/Commandos3/Data/Misiones/TV/TV.y64"))
			{
				Picture p = y64.GeneratePicture(0, 0);

				Debug.WriteLine(Environment.TickCount);
				bmp = p.DrawAllWithCrop(y64.rgb);
				bmp.Save("test.png", ImageFormat.Png);

				//p.SaveToFile("test.png", ImageFormat.Png, y64.rgb);

				Debug.WriteLine(Environment.TickCount);
			}
		}

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawImage(bmp, 0, 0);
		}

		private void Form1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 27)
				Close();
		}
	}
}