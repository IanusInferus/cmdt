using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace Comm_Y64
{
	public class Y64 : IDisposable
	{
		private long[,] ptr_pics; //����ͼƬ���ļ�λ�ã�ptr_pics[num_view,num_pic_per_view]
		public YCrCbPalette palette;
		public RGBPalette rgb;

		public int num_view; //�ӽ���
		public int num_pic_per_view; //ÿһ�ӽǵ�ͼƬ��
		public int version; //�汾��Ϣ

		FileStream fs;
		BinaryReader br;

		public Y64(string fn) //ok
		{
			fs = new FileStream(fn, FileMode.Open, FileAccess.Read);
			br = new BinaryReader(fs);

			//���Y74��6�ֽ��ļ���־46 43 44 45 00 00
			int id = br.ReadInt32();
			Debug.Assert(id == 0x45444346);
			id = br.ReadInt16();
			Debug.Assert(id == 0);

			//�汾��־��commandos 2 or commandos 3
			version = br.ReadInt16();
			Debug.Assert(version >= 2 && version <= 4);
			if (version > 2) version = 3;

			//���YCrCbɫ���RGBɫ��
			if (version == 2)
				palette = new YCrCbPalette(br, 48);
			else
				palette = new YCrCbPalette(br, 113);
			rgb = new RGBPalette(palette);

			//����ӽ�����ÿ�ӽǵ�ͼƬ��
			num_view = br.ReadInt32();
			num_pic_per_view = br.ReadInt32();

			//���ÿһͼƬ����ʼλ��
			ptr_pics = new long[num_view, num_pic_per_view];
			for (int i = 0; i < num_view; i++)
				for (int j = 0; j < num_pic_per_view; j++)
					ptr_pics[i, j] = br.ReadInt32();
		}

		public Picture GeneratePicture(int view, int idx)
		{
			Debug.Assert(view >= 0 && view < num_view);
			Debug.Assert(idx >= 0 && idx < num_pic_per_view);

			//����ָ����ͼƬ
			return new Picture(br, ptr_pics[view, idx], version);
		}

		#region IDisposable Pattern

		private bool disposed = false;
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					// Dispose managed resources here.
					br.Close();
					fs.Close();
					fs.Dispose();
				}
				// Call the appropriate methods to clean up unmanaged resources here.
			}
			disposed = true;
		}
		~Y64()
		{
			Dispose(false);
		}

		#endregion
	}

	public class Picture
	{
		public Block[,] blocks; //Block[y,x]

		int zoom;				//���ŵȼ�
		public int width;		//���ؿ�
		public int height;		//���ظ�
		int num_block;			//block����������extra block��������null block������Ч������
		int num_xblock;			//extra block���� 

		long address;			//ͼƬ��Y64�ļ��е���ʼλ��

		public Picture(BinaryReader br, long address, int version)
		{
			br.BaseStream.Seek(address, SeekOrigin.Begin);
			this.address = address;

			//��ȡ���ŵȼ������ؿ��ߡ���Чblock��extrablock����
			zoom = br.ReadInt32();
			width = br.ReadInt32();
			height = br.ReadInt32();
			num_block = br.ReadInt32();
			num_xblock = br.ReadInt32();

			//������������ʵ�������64*64ͼ����
			//ע��˴�width��height��һ���ܱ�64����
			int W = (width >> 6) + ((width & 63) != 0 ? 1 : 0);
			int H = (height >> 6) + ((height & 63) != 0 ? 1 : 0);

			//����ÿһ��64*64ͼ�����ļ��е�����λ��
			long[,] ptr_block = new long[H, W];
			switch (version)
			{
				case 2://commandos2: �˴�Ϊ���16λ������Ҫ����һ���������
					for (int h = 0; h < H; h++)
						for (int w = 0; w < W; w++)
						{
							int idx = br.ReadUInt16();
							if (idx != 0xffff)			//0xffffΪnull block���
								ptr_block[h, w] = address + 20 + 2 * W * H + 16 * num_xblock + idx * 3592;
							else
								ptr_block[h, w] = 0;	//null block
						}
					break;
				case 3: //commandos3: �˴�Ϊ���32λ�ļ�λ��ָ�룬����������
					for (int h = 0; h < H; h++)
						for (int w = 0; w < W; w++)
							ptr_block[h, w] = br.ReadInt32();	//commandos3������null block�����
					break;
				default:
					Debug.Assert(false); break;
			}

			//����ÿһ��64*64ͼ��
			blocks = new Block[H, W];
			for (int h = 0; h < H; h++)
				for (int w = 0; w < W; w++)
					if (ptr_block[h, w] != 0)
						blocks[h, w] = new Block(br, ptr_block[h, w]);
					else
						blocks[h, w] = null;
		}

		#region ͼƬ���ơ��������

		//���ٻ���Bitmap�������вü���ͼƬ���߷ֱ���W��H��������
		public unsafe Bitmap DrawAllWithoutCrop(RGBPalette rgb)
		{
			//width��height��һ���ܱ�64����
			int W = (width >> 6) + ((width & 63) != 0 ? 1 : 0);
			int H = (height >> 6) + ((height & 63) != 0 ? 1 : 0);

			Bitmap bmp = new Bitmap(W * 64, H * 64, PixelFormat.Format32bppArgb);

			BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, W * 64, H * 64), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

			IntPtr ptr = bmpdata.Scan0;			//Bitmap����ָ��
			int pitch = bmpdata.Stride / 4;		//һ�ж��ٸ�����,������һ�ж����ֽ�!

			//���λ���ÿһ��64*64ͼ��
			for (int h = 0; h < H; h++)
				for (int w = 0; w < W; w++)
				{
					int* line = (int*)ptr + h * 64 * pitch + w * 64; //��λblock

					Block b = blocks[h, w];
					if (b == null) //�÷ۺ�ɫ�������е�null block
					{
						int pink = Color.Fuchsia.ToArgb();
						for (int y = 0; y < 64; y++, line += pitch)
							for (int x = 0; x < 64; x++)
								line[x] = pink;//��ɫ���
					}
					else
					{
						//���λ���ÿһ��8*8��ͼ��
						for (int sh = 0; sh < 8; sh++)
							for (int sw = 0; sw < 8; sw++)
							{
								SubBlock sb = b.subs[sh, sw];
								int* p = line + sh * 8 * pitch + sw * 8; //��λsubblock
								//���λ���8*8��ͼ���е�ÿһ������
								for (int y = 0; y < 8; y++, p += pitch)
									for (int x = 0; x < 8; x++)
										p[x] = (int)rgb.RGB32[sb.pixels[y, x]];
							}
					}
				}

			bmp.UnlockBits(bmpdata);

			return bmp;
		}

		//���ٻ���Bitmap����ͬʱ���вü��������Ż�
		public unsafe Bitmap DrawAllWithCrop(RGBPalette rgb)
		{
			//width��height��һ���ܱ�64����
			int W = (width >> 6) + ((width & 63) != 0 ? 1 : 0);
			int H = (height >> 6) + ((height & 63) != 0 ? 1 : 0);

			Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

			BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

			IntPtr ptr = bmpdata.Scan0;			//Bitmap����ָ��
			int pitch = bmpdata.Stride / 4;		//һ�ж��ٸ�����,������һ�ж����ֽ�!

			//���λ���ÿһ��64*64ͼ��
			for (int h = 0; h < H; h++)
				for (int w = 0; w < W; w++)
				{
					int* line = (int*)ptr + h * 64 * pitch + w * 64; //��λblock

					Block b = blocks[h, w];

					if (b == null) //�÷ۺ�ɫ�������е�null block
					{
						int pink = Color.Fuchsia.ToArgb();

						int aby = h * 64;
						for (int y = 0; y < 64; y++, aby++)
						{
							if (aby >= height) break;

							int abx = w * 64;
							for (int x = 0; x < 64; x++, abx++)
							{
								if (abx >= width) break;
								line[x] = pink;//��ɫ���
							}
							line += pitch;
						}
						continue; //��������һ��ͼ��
					}

					//���λ���ÿһ��8*8��ͼ��
					for (int sh = 0; sh < 8; sh++)
					{
						for (int sw = 0; sw < 8; sw++)
						{
							SubBlock sb = b.subs[sh, sw];
							int* p = line + sh * 8 * pitch + sw * 8; //��λsubblock

							//���λ���8*8��ͼ���е�ÿһ������
							int aby = h * 64 + sh * 8;
							for (int y = 0; y < 8; y++, aby++)
							{
								if (aby >= height) break;

								int abx = w * 64 + sw * 8;
								for (int x = 0; x < 8; x++, abx++)
								{
									if (abx >= width) break;
									p[x] = (int)rgb.RGB32[sb.pixels[y, x]];
								}
								p += pitch;
							}
						}
					}
				}

			bmp.UnlockBits(bmpdata);

			return bmp;
		}

		//��ָ����ʽ����ͼƬ
		public void SaveToFile(string fn, ImageFormat format, RGBPalette rgb)
		{
			Bitmap bmp = DrawAllWithCrop(rgb);
			bmp.Save(fn, format);
			bmp.Dispose();

			//��DrawAll���ɵ�W*64-H*64������ʵ�ߴ�Crop��ò�ƴ��ۻ��Ǻܸߣ�����ö����Draw��ʵ��
			//Bitmap dest = bmp.Clone(new Rectangle(0, 0, width, height), PixelFormat.Format32bppArgb);		
			//bmp.Dispose();

			//dest.Save(fn, format);
			//dest.Dispose();
		}

		#endregion
	}

	public class Block
	{
		public SubBlock[,] subs;	//SubBlock[y,x]
		long address;				//64*64ͼ����Y64�ļ��е���ʼλ��

		public Block(BinaryReader br, long address)
		{
			this.address = address;
			br.BaseStream.Seek(address, SeekOrigin.Begin);

			//����Seperator������һ����0xffffffffffffffff!
			br.BaseStream.Seek(8, SeekOrigin.Current);

			subs = new SubBlock[8, 8];
			for (int h = 0; h < 8; h++)
				for (int w = 0; w < 8; w++)
				{
					//���μ����8*8��ͼ�����ʼλ�ã������ɸ���ͼ��
					long off = address + 8 + 56 * (h * 8 + w); //ÿ��subblock��56�ֽ�
					subs[h, w] = new SubBlock(br, off);
				}
		}
	}

	public class SubBlock
	{
		public int[,] pixels;	//pixels[y,x]
		long address;			//8*8��ͼ����ļ���ʼλ��

		public SubBlock(BinaryReader br, long address)
		{
			this.address = address;
			br.BaseStream.Seek(address, SeekOrigin.Begin);

			//ÿһ��pixelsԪ�ؾ�ΪYCbCr�������ĺϳ�ʸ��
			//�����Ϊ�Ƕ�RGBɫ�������
			pixels = new int[8, 8];

			DecodeDS0(br); //����Y�����ĸ�4λ
			br.ReadInt16();
			DecodeDS1(br); //����Cb��Cr����
			DecodeDS2(br); //����Y�����ĵ�4λ
		}

		#region δ�Ż���DS0��DS1�������
		/*
		public void DecodeDS0(BinaryReader br) //δ�Ż���ԭʼ�汾
		{
			int[,] DS0=new int[2,2];

			for (int y=1;y>=0;y--) //����
			{
				int t=br.ReadByte();
				DS0[y,0] = t >> 4;
				DS0[y,1] = t & 0xf;
			}

			for (int y = 0; y < 8; y++)
				for (int x = 0; x < 8; x++)
					pixels[y, x] |= (DS0[y / 4, x / 4] << 4);
		}

		public void DecodeDS1(BinaryReader br) //δ�Ż���ԭʼ�汾
		{
			byte[] DS1 = br.ReadBytes(20);
			for (int j = 0; j < 5; j++) //ÿ4�ֽڷ�ת
			{
				byte t;
				int i = j * 4;
				t = DS1[i]; DS1[i] = DS1[i + 3]; DS1[i + 3] = t;
				t = DS1[i + 1]; DS1[i + 1] = DS1[i + 2]; DS1[i + 2] = t;
			}

			int[,] ds1 = new int[4, 4];

			for (int y = 0; y < 4; y++)
			{
				int A, B, C, D, E;
				A = DS1[y * 5];
				B = DS1[y * 5 + 1];
				C = DS1[y * 5 + 2];
				D = DS1[y * 5 + 3];
				E = DS1[y * 5 + 4];

				//A           B        C       D        E
				//76543210 76543210 76543210 76543210 76543210 
				//98765432 10987654 32109876 54321098 76543210
				//a          b          c          d 
				ds1[y, 0] = (A << 2) | (B >> 6);
				ds1[y, 1] = ((B & 0x3f) << 4) | (C >> 4);
				ds1[y, 2] = ((C & 0xf) << 6) | (D >> 2);
				ds1[y, 3] = ((D & 0x3) << 8) | E;
			}
			for (int y = 0; y < 8; y++)
				for (int x = 0; x < 8; x++)
					pixels[y, x] |= (ds1[y / 2, x / 2] << 8);
		}
		*/
		#endregion

		public void DecodeDS0(BinaryReader br) //DS0������ٰ�
		{
			ushort DS0 = br.ReadUInt16();//2�ֽڷ�ת

			int i, j;
			for (int y = 0; y < 8; y++)
				for (int x = 0; x < 8; x++)
				{
					//i = (y / 4 * 2 + x / 4) * 4;
					i = (((y >> 1) & 14) + (x >> 2)) << 2; //����ʽ�ȼ�
					j = ((DS0 << i) & 0xffff) >> 12;

					pixels[y, x] |= j << 4;
				}
		}

		public void DecodeDS1(BinaryReader br) //��΢�Ż���DS1�������
		{
			byte[] DS1 = br.ReadBytes(20);

			//4���ֽ�4���ֽ�һ��ת
			for (int j = 0; j < 5; j++) //ÿһ��4�ֽڷ�ת��Ҫ2�α�������
			{
				int i = j << 2;
				DS1[i] ^= DS1[i + 3]; DS1[i + 3] ^= DS1[i]; DS1[i] ^= DS1[i + 3];
				DS1[i + 1] ^= DS1[i + 2]; DS1[i + 2] ^= DS1[i + 1]; DS1[i + 1] ^= DS1[i + 2];
			}

			//5���ֽ�5���ֽ�һ���ϣ����ϳ�����10bits
			int A, B, C, D, E;
			int[] Ret = new int[4];
			for (int i = 0; i < 4; i++)
			{
				A = DS1[i * 5];
				B = DS1[i * 5 + 1];
				C = DS1[i * 5 + 2];
				D = DS1[i * 5 + 3];
				E = DS1[i * 5 + 4];

				Ret[0] = (A << 2) | (B >> 6);
				Ret[1] = ((B & 0x3f) << 4) | (C >> 4);
				Ret[2] = ((C & 0xf) << 6) | (D >> 2);
				Ret[3] = ((D & 0x3) << 8) | E;

				for (int j = 0; j < 2; j++)
					for (int x = 0; x < 8; x++)
						pixels[(i << 1) + j, x] |= (Ret[x >> 1] << 8);
			}
		}

		public void DecodeDS2(BinaryReader br) //�����ر��Ż�
		{
			int t;
			for (int y = 0; y < 8; y++)
				for (int i = 3; i >= 0; i--) //ÿ4�ֽڷ�ת!
				{
					t = br.ReadByte();
					pixels[y, i << 1] |= (t >> 4);
					pixels[y, (i << 1) + 1] |= (t & 0xf);
				}
		}
	}

	public class YCrCbPalette
	{
		public byte[] Y; //8bit
		public byte[] Cb; //5bit
		public byte[] Cr; //5bit;

		long address;     //palette DB���ļ��е���ʼλ��

		public YCrCbPalette(BinaryReader br, long address)
		{
			this.address = address;
			br.BaseStream.Seek(address, SeekOrigin.Begin);

			Y = br.ReadBytes(256);
			Cb = br.ReadBytes(32);
			Cr = br.ReadBytes(32);
		}
	}

	public class RGBPalette
	{
		public uint[] RGB32 = new uint[(1 << 18)]; //����1M Bytes

		//RGBɫ���Ǹ���YCrCbPalette�е���ɫ����ɫ�����ɵ�
		public RGBPalette(YCrCbPalette ycc)
		{
			for (int y = 0; y < 256; y++)
				for (int cr = 0; cr < 32; cr++)
					for (int cb = 0; cb < 32; cb++)
						//��YCbCr�������ĺϳ�ʸ����ΪRGBɫ������
						RGB32[(cb << 13) | (cr << 8) | y] = YCbCr2RGB(ycc.Y[y], ycc.Cb[cb], ycc.Cr[cr]);
		}

		public uint YCbCr2RGB(int Y, int Cb, int Cr) //��YCbCrת����ARGB
		{
			int R, G, B;
			R = (int)(Y + 1.402 * (Cr - 128));
			G = (int)(Y - 0.34414 * (Cb - 128) - 0.71414 * (Cr - 128));
			B = (int)(Y + 1.772 * (Cb - 128));

			if (R < 0) R = 0;
			if (G < 0) G = 0;
			if (B < 0) B = 0;

			if (R > 255) R = 255;
			if (G > 255) G = 255;
			if (B > 255) B = 255;

			return ((uint)R << 16) | ((uint)G << 8) | (uint)B | 0xFF000000;
		}
	}
}