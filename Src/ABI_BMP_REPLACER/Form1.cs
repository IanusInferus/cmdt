using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Comm_ABI
{
	public partial class Form1 : Form
	{
		ABI abi;
		int idx;
		string aufgabe;
		string aufgabe_to;
		string filePath;
		List<string> back = new List<string>();
		string speichern;
		long position_old;
		byte[] ende;
		Graphics g;
		int[] act_width;
		int[] act_height;
		List<int> actives = new List<int>();
		int pos;


		public Form1()
		{
			InitializeComponent();

			button1.Enabled = false;
			button2.Enabled = true;
			button3.Enabled = false;
			button4.Enabled = false;
			button5.Enabled = false;
			button6.Enabled = false;

		}

		public void refresh()
		{

			ABI abis = new ABI(speichern);

			listBox1.Items.Clear();

			back = abis.get_name_list();

			for (int z = 0; z < back.Count; z++)
			{

				listBox1.Items.Add(back[z]);

			}

			int anzahl = back.Count();

			label6.Text = "Textures found";
			label3.Text = anzahl.ToString();

		}

		public void button1_Click(object sender, EventArgs e)
		{

			listBox1.Items.Clear();

			back = abi.get_name_list();

			for (int z = 0; z < back.Count; z++)
			{

				listBox1.Items.Add(back[z]);

			}

			int anzahl = back.Count();

			label6.Text = "Textures found";
			label3.Text = anzahl.ToString();

		}

		public void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{

			pictureBox1.Image = null;

			pictureBox1.Refresh();

			idx = listBox1.SelectedIndex;

			try
			{

				g = pictureBox1.CreateGraphics();
				Rectangle r1 = new Rectangle(0, 0, abi.textureinfo[idx].width, abi.textureinfo[idx].height);
				Rectangle r2 = new Rectangle(0, 0, abi.textureinfo[idx].width, abi.textureinfo[idx].height);

				g.DrawImage(abi.bmps[idx], r2, r1, GraphicsUnit.Pixel);

				label1.Text = abi.textureinfo[idx].width.ToString();
				label2.Text = abi.textureinfo[idx].height.ToString();


			}
			catch
			{

				//NIX

			}

			button3.Enabled = true;
			button6.Enabled = true;


		}

		public void button2_Click(object sender, EventArgs e)
		{

			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.InitialDirectory = "";
				openFileDialog.RestoreDirectory = true;
				openFileDialog.Filter = "abi files (*.abi)|*.abi|ABI files (*.ABI)|*.ABI";

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{

					filePath = openFileDialog.FileName;

					aufgabe = filePath;

					textBox1.Text = aufgabe;

					button1.Enabled = true;

					actives.Clear();
					back.Clear();

					abi = new ABI(aufgabe);

				}

			}

		}

		public void button3_Click(object sender, EventArgs e)
		{


			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.InitialDirectory = "";
				openFileDialog.RestoreDirectory = true;
				openFileDialog.Filter = "bmp files (*.bmp)|*.bmp|BMP files (*.BMP)|*.BMP";

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					filePath = openFileDialog.FileName;

					aufgabe_to = filePath;

					pictureBox2.Image = new Bitmap(aufgabe_to);

					if (pictureBox2.Image.Width > 512 || pictureBox2.Image.Height > 512)
					{

						pictureBox2.Image = null;

						textBox2.Text = "";

						MessageBox.Show("Please choose a .bmp File wih maximal 512 x 512 pixel.");

						return;

					}
					else
					{

						textBox2.Text = aufgabe_to;

						label4.Text = pictureBox2.Image.Width.ToString();
						label5.Text = pictureBox2.Image.Height.ToString();

						button5.Enabled = true;

					}


				}


			}


		}

		public void button5_Click(object sender, EventArgs e)
		{

			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.InitialDirectory = "";
				openFileDialog.RestoreDirectory = true;
				openFileDialog.Filter = "abi files (*.abi)|*.abi|ABI files (*.ABI)|*.ABI";

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					filePath = openFileDialog.FileName;

					speichern = filePath;

					textBox3.Text = speichern;

					button4.Enabled = true;

				}

			}

		}

		public Byte[] GetImageData(Bitmap sourceImage, out Int32 stride, Boolean collapseStride)
		{
			if (sourceImage == null)
				throw new ArgumentNullException("sourceImage", "Source image is null!");
			Int32 width = sourceImage.Width;
			Int32 height = sourceImage.Height;
			BitmapData sourceData = sourceImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, sourceImage.PixelFormat);
			stride = sourceData.Stride;
			Byte[] data;
			if (collapseStride)
			{
				Int32 actualDataWidth = ((Image.GetPixelFormatSize(sourceImage.PixelFormat) * width) + 7) / 8;
				Int64 sourcePos = sourceData.Scan0.ToInt64();
				Int32 destPos = 0;
				data = new Byte[actualDataWidth * height];
				for (Int32 y = 0; y < height; ++y)
				{
					Marshal.Copy(new IntPtr(sourcePos), data, destPos, actualDataWidth);
					sourcePos += stride;
					destPos += actualDataWidth;
				}
				stride = actualDataWidth;
			}
			else
			{
				data = new Byte[stride * height];
				Marshal.Copy(sourceData.Scan0, data, 0, data.Length);
			}
			sourceImage.UnlockBits(sourceData);
			return data;
		}

		public void anhaengen()
		{

			int laenge = abi.position_old;


			int von = File.ReadAllBytes(aufgabe).Length;

			using (FileStream fs = new FileStream(aufgabe, FileMode.Open, FileAccess.Read))
			{

				using (BinaryReader br = new BinaryReader(fs))
				{

					br.BaseStream.Seek(laenge, SeekOrigin.Begin);

					ende = br.ReadBytes(von);


				}

			}


			using (FileStream fs = new FileStream(speichern, FileMode.Open, FileAccess.Write))
			{

				using (BinaryWriter br = new BinaryWriter(fs))
				{

					br.BaseStream.Seek((int)position_old, SeekOrigin.Begin);

					br.Write(ende);

					br.Close();

				}

				fs.Close();

				fs.Dispose();

			}

		}

		public void ersetzen(string ims, int position)
        {

			int ddd = pos;
			idx = pos;

			actives.Add(ddd);

			pictureBox1.Image = (Bitmap)Bitmap.FromFile(ims);

			abi.bmps[idx] = (Bitmap)Bitmap.FromFile(ims);

			abi.textureinfo[idx].width = abi.bmps[ddd].Width;
			abi.textureinfo[idx].height = abi.bmps[ddd].Height;

			label1.Text = pictureBox2.Image.Width.ToString();
			label2.Text = pictureBox2.Image.Height.ToString();

			//HIER ERSETZEN STARTEN
			using (FileStream fs = new FileStream(speichern, FileMode.Open, FileAccess.Write))
			{
				using (BinaryWriter br = new BinaryWriter(fs))
				{

					br.Write(0x424d444c);
					br.Write(abi.version);

					//Zuerst vorgehen
					br.Write(BitConverter.GetBytes(abi.num_mesh));
					br.Write(BitConverter.GetBytes(abi.num_animation));
					br.Write(BitConverter.GetBytes(abi.num_texture));

					for (int i = 0; i < abi.textureinfo.Length; i++)
					{

						TextureInfo p = abi.textureinfo[i];

						if (i == position || actives.Contains(i))
						{

							br.Write(p.UNKNOWN);
							br.Write(abi.bmps[i].Width);
							br.Write(abi.bmps[i].Height);

							byte[] wert = Encoding.UTF8.GetBytes(p.name);
							Array.Resize(ref wert, 32);

							br.Write(wert);

							ColorPalette palettes = abi.bmps[i].Palette;

							for (int j = 0; j < 256; j++)
							{

								byte r = palettes.Entries[j].R;

								br.Write(r);

								byte g = palettes.Entries[j].G;

								br.Write(g);

								byte b = palettes.Entries[j].B;

								br.Write(b);

							}

							Int32 stride;
							byte[] rawData = GetImageData(abi.bmps[i], out stride, true);

							br.Write(rawData);

						}
						else
						{

							br.Write(p.UNKNOWN);
							br.Write(p.width);
							br.Write(p.height);

							byte[] wert = Encoding.UTF8.GetBytes(p.name);
							Array.Resize(ref wert, 32);

							br.Write(wert);

							Color[] palettes = p.palette;

							for (int j = 0; j < 256; j++)
							{

								byte r = palettes[j].R;

								br.Write(r);
								byte g = palettes[j].G;

								br.Write(g);
								byte b = palettes[j].B;

								br.Write(b);

								p.palette[j] = Color.FromArgb(r, g, b);

							}

							br.Write(p.data);

						}

					}

					position_old = br.BaseStream.Position;

					br.Close();

				}

				fs.Close();

				fs.Dispose();

			}


		}


		public void button4_Click(object sender, EventArgs e)
		{

			pos = listBox1.SelectedIndex;
			int posi = listBox1.SelectedIndex;

			ersetzen(aufgabe_to, posi);

			anhaengen();

		}

		public void button6_Click(object sender, EventArgs e)
		{

			int ddd = listBox1.SelectedIndex;

			using (SaveFileDialog openFileDialog = new SaveFileDialog())
			{
				openFileDialog.InitialDirectory = "";
				openFileDialog.RestoreDirectory = true;
				openFileDialog.Filter = "bmp files (*.bmp)|*.bmp|BMP files (*.BMP)|*.BMP";

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					//Get the path of specified file
					filePath = openFileDialog.FileName;

					optimized opt = new optimized();

					opt.sourceImage = (Bitmap)abi.bmps[ddd];
					opt.UpdateImages();

					opt.ergebnis.Save(filePath, ImageFormat.Bmp);

				}

			}

		}
	}

}