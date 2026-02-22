using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Comm_ABI
{
	class ABI
	{
		public int num_mesh;
		public int num_animation;

		public int num_texture;

		public TextureInfo[] textureinfo;
		public Bitmap[] bmps;

		public int num_dress;
		public int num_bone;
		public Dress[] dress;

		public BoneHierarchy[] hierarchy;

		///////////////////////////////////////////////////////////////////////////////////////////////////////////
		public ABI(string filename)
		{
			using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					Debug.Assert( br.ReadInt32() == 0x424d444c);
					//Debug.Assert(br.ReadInt32() == 0x30363031); //1060
					br.ReadInt32(); //实验1050等版本，1050的失败

					num_mesh = br.ReadInt32();
					num_animation= br.ReadInt32();
					num_texture = br.ReadInt32();

					///////////////////////////////////////////////////////////////////////////////////////////////////////////
					//所有贴图
					textureinfo = new TextureInfo[num_texture];				
					for (int i = 0; i < num_texture; i++)
					{
						textureinfo[i] = new TextureInfo();
						textureinfo[i].UNKNOWN = br.ReadInt32();
						textureinfo[i].width = br.ReadInt32();
						textureinfo[i].height = br.ReadInt32();
						textureinfo[i].name = ConvertBytesToString(br.ReadBytes(32));

						textureinfo[i].palette = new Color[256];
						for (int j = 0; j < 256; j++)
						{
							byte r = br.ReadByte();
							byte g = br.ReadByte();
							byte b = br.ReadByte();

							textureinfo[i].palette[j] = Color.FromArgb(r, g, b);
						}

						textureinfo[i].data = br.ReadBytes(textureinfo[i].width * textureinfo[i].height);
					}

					///////////////////////////////////////////////////////////////////////////////////////////////////////////
					//着装数据
					br.ReadByte();//未知标志，并不恒为1
					//Debug.Assert(br.ReadByte() == 1);

					num_dress = br.ReadInt32();
					num_bone = br.ReadInt32();

					dress = new Dress[num_dress];
					for (int i = 0; i < num_dress; i++)
					{
						Dress d = dress[i] = new Dress();					
						d.num_vertice = br.ReadInt32();
						d.num_polygon = br.ReadInt32();
						d.name = ConvertBytesToString(br.ReadBytes(32));

						d.vertex = new Vertex[d.num_vertice];
						for (int j = 0; j < d.num_vertice; j++)
						{
							Vertex v = d.vertex[j] = new Vertex();
							v.X = br.ReadSingle();
							v.Y = br.ReadSingle();
							v.Z = br.ReadSingle();
						}

						d.polygon = new Polygon[d.num_polygon];
						for (int j = 0; j < d.num_polygon; j++)
						{
							Polygon poly = d.polygon[j] = new Polygon();
							poly.num_lines = br.ReadByte();
							
							if (poly.num_lines != 3 && poly.num_lines != 4)
							{
								//的确存在num_lines超过3/4的情况，比方说tiger.abi，num_lines就有为6的情况
								//throw new Exception();
							}
							
							poly.texture_id = br.ReadByte();

							poly.map_points = new Point[poly.num_lines];
							for (int k = 0; k < poly.num_lines; k++)
							{
								Point p = poly.map_points[k] = new Point();
								p.vertex_id = br.ReadInt16();
								p.U = br.ReadInt16() / 4096f * textureinfo[poly.texture_id].width*2; //验证了除以4096映射至0-1之间是对的
								p.V = br.ReadInt16() / 4096f * textureinfo[poly.texture_id].width*2; //这里我们扩大了两倍，以便于显示

								p.vertex = d.vertex[p.vertex_id];
							}
						}

						d.vbt = new VidToBoneTable();
						d.vbt.entry = new VidToBoneTableEntry[num_bone];
						for (int j = 0; j < num_bone; j++)
						{
							d.vbt.entry[j] = new VidToBoneTableEntry();
							d.vbt.entry[j].StartVidx = br.ReadInt32();
							d.vbt.entry[j].EndVidx = br.ReadInt32(); //要不要-1?
						}
					}
					
					///////////////////////////////////////////////////////////////////////////////////////////////////////////
					//骨骼继承结构
					hierarchy = new BoneHierarchy[num_bone];

					for (int i = 0; i < num_bone;i++ )
					{
						hierarchy[i] = new BoneHierarchy();
						hierarchy[i].ParentIdx = br.ReadInt32();
						hierarchy[i].b = new Vector3(
							br.ReadSingle(),
							br.ReadSingle(),
							br.ReadSingle());
						hierarchy[i].NodeName = ConvertBytesToString(br.ReadBytes(32));
						hierarchy[i].UNKNOWN = br.ReadInt32();

						Debug.WriteLine(String.Format("{0} - [{1}]:{2}", i, hierarchy[i].ParentIdx, hierarchy[i].NodeName));
						Debug.Assert(hierarchy[i].UNKNOWN == 0);
					}

					//Debug.WriteLine(fs.Position);
				}

				GenerateAllBitmaps(); //创建所有位图
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////
		public void GenerateAllBitmaps()
		{
			bmps = new Bitmap[textureinfo.Length];

			for (int i = 0; i < textureinfo.Length; i++)
			{
				TextureInfo p = textureinfo[i];
				bmps[i] = new Bitmap(p.width, p.height);

				BitmapData bmpdata = bmps[i].LockBits(new Rectangle(0, 0, bmps[i].Width, bmps[i].Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				IntPtr ptr = bmpdata.Scan0;
				int pitch = bmpdata.Stride / 4;

				unsafe
				{
					int t = 0;
					int* start = (int*)ptr;
					for (int y = 0; y < p.height; y++)
					{
						for (int x = 0; x < p.width; x++)
						{
							int idx = p.data[t++];
							if (idx == 0xfe)
								//*(start + x) = Color.Fuchsia.ToArgb();
								*(start + x) = Color.Black.ToArgb();
							else
								*(start + x) = p.palette[idx].ToArgb();
						}
						start += pitch;
					}
				}

				bmps[i].UnlockBits(bmpdata);
			}
		}

		private string ConvertBytesToString(byte[] buf)
		{
			int i;
			for (i = 0; i < buf.Length; i++)
				if (buf[i] == 0) break;

			string s = Encoding.ASCII.GetString(buf, 0, i);
			return s;
		}
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	class TextureInfo
	{
		public int UNKNOWN;
		public int width;
		public int height;
		public string name; //32字节
		public Color[] palette;
		public byte[] data;
	}

	class Vertex
	{
		public float X;
		public float Y;
		public float Z;
	}

	class Point
	{
		public short vertex_id;
		public float U;
		public float V;
		
		public Vertex vertex;
	}

	class Polygon
	{
		public byte num_lines;
		public byte texture_id;
		public Point[] map_points;
	}

	class Dress
	{
		public int num_vertice;
		public int num_polygon;
		public string name;

		public Vertex[] vertex;
		public Polygon[] polygon;

		public VidToBoneTable vbt;
	}

	class VidToBoneTable
	{
		public VidToBoneTableEntry[] entry;
	}

	class VidToBoneTableEntry
	{
		public int StartVidx;
		public int EndVidx;
	}

	class BoneHierarchy 
	{
		public int		ParentIdx;	//4字节
		public Vector3	b;			//12字节
		public String	NodeName;	//32字节
		public int		UNKNOWN;	//4字节
	}
}
