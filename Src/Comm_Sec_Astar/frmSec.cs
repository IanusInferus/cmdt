using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.DirectX;

namespace Comm_Sec_Astar
{
	public partial class frmSec : Form
	{
		Sec sec;
		Bitmap bmp;
		
		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		public frmSec()
		{
			InitializeComponent();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		private void frmSec_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawImage(bmp, 0, 0);
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		float zoom = 3.5F;			//1-16，共32级缩放,delta=0.5
		int   fontsize = 6;			//2-12磅字体
		bool  fontdisplay = false;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		private void frmSec_Load(object sender, EventArgs e)
		{
			OpenFileDialog ofg = new OpenFileDialog();

			ofg.Title = "Load .sec";
			ofg.Filter = "Sec files (*.sec)|*.sec";
			ofg.FilterIndex = 0;
			ofg.RestoreDirectory = true;

			string fn;
			if (ofg.ShowDialog() == DialogResult.OK)
			{
				fn = ofg.FileName;
				sec = new Sec(fn);
				GenOriginalSketch(1 / zoom, fontsize, fontdisplay); //重置视线
			}
			else
				Close();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		private void frmSec_KeyPress(object sender, KeyPressEventArgs e)
		{
			switch (e.KeyChar)
			{
				case 'q':
					Close();
					return;
				case 'x':
					if (zoom + 0.5 <= 16) zoom += 0.5F;
					break;
				case 'z':
					if (zoom - 0.5 > 0.5) zoom -= 0.5F;
					break;
				case '=':
					if (fontsize + 1 <= 12) fontsize++;
					break;
				case '-':
					if (fontsize - 1 >= 2) fontsize--;
					break;
				case 'f':
					fontdisplay = !fontdisplay;
					break;
				case 'o':
					OpenFileDialog ofg = new OpenFileDialog();

					ofg.Title = "Load .sec";
					ofg.Filter = "Sec files (*.sec)|*.sec";
					ofg.FilterIndex = 0;
					ofg.RestoreDirectory = true;

					if (ofg.ShowDialog() == DialogResult.OK)
					{
						sec = new Sec(ofg.FileName);
						GenOriginalSketch(1 / zoom, fontsize, fontdisplay);  //重置视线
						Refresh();
					}
					return;
				case 's':
					SaveFileDialog sfg = new SaveFileDialog();

					sfg.Title = "Save to PNG";
					sfg.Filter = "PNG files (*.png)|*.png";
					sfg.FilterIndex = 0;
					sfg.RestoreDirectory = true;

					if (sfg.ShowDialog() == DialogResult.OK)
						bmp.Save(sfg.FileName, ImageFormat.Png);
					else
						return;
					break;
				default:
					return;
			}
			if (sec != null)
			{
				GenOriginalSketch(1 / zoom, fontsize, fontdisplay);
				Refresh();
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		private void frmSec_MouseDown(object sender, MouseEventArgs e) //更新视线
		{
			bool updated = false;

			Vector2 v = new Vector2(e.X, e.Y);
			if (e.Button == MouseButtons.Left) //左键设置终点
				updated = sec.UpdateDst(v);
			else //其他键设置起始点
				updated = sec.UpdateSrc(v);

			if (updated)
			{
				if (bmp != null) bmp.Dispose();
				bmp = sec.Update(1 / zoom);
				if (fontdisplay) sec.DrawAllIndex(bmp, fontsize);

				this.Width = bmp.Width + 5;
				this.Height = bmp.Height + 32;
				Refresh();
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		private void GenOriginalSketch(float n, int fontsize, bool fontdisplay) //重置视线
		{
			if (bmp != null) bmp.Dispose();

			bmp = sec.StartRay(1 / zoom);

			if (fontdisplay)
				sec.DrawAllIndex(bmp, fontsize);

			this.Width = bmp.Width + 5;
			this.Height = bmp.Height + 32;
		}
	}
}