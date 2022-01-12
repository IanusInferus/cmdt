using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace Comm_ABI
{
	class ABI
	{
		public int num_mesh;
		public int num_animation;
		public int version;
		public int num_texture;
		public int haupt;
		public int einer;
		public int zweier;
		public int position_old;

		public TextureInfo[] textureinfo;
		public Bitmap[] bmps;

		public int num_dress;
		public int num_bone;
		public Dress[] dress;

		public BoneHierarchy[] hierarchy;
		public List<string> liste_namen = new List<string>();

		public ABI(string filename)
		{

			using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					br.ReadInt32();
					version = br.ReadInt32();

					num_mesh = br.ReadInt32();
					num_animation= br.ReadInt32();
					num_texture = br.ReadInt32();

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

					position_old = (int)br.BaseStream.Position;
					
					dress = new Dress[num_dress];
					for (int i = 0; i < num_dress; i++)
					{

						Console.WriteLine("STATUS: " + i);

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

							}
							
							poly.texture_id = br.ReadByte();

							poly.map_points = new Point[poly.num_lines];
							for (int k = 0; k < poly.num_lines; k++)
							{
								Point p = poly.map_points[k] = new Point();
								p.vertex_id = br.ReadInt16();

								einer = br.ReadInt16();
								zweier = br.ReadInt16();

								p.U = einer / 4096f * textureinfo[poly.texture_id].width*2;
								p.V = zweier / 4096f * textureinfo[poly.texture_id].width*2;

								p.vertex = d.vertex[p.vertex_id];
							}
						}

						d.vbt = new VidToBoneTable();
						d.vbt.entry = new VidToBoneTableEntry[num_bone];
						for (int j = 0; j < num_bone; j++)
						{
							d.vbt.entry[j] = new VidToBoneTableEntry();
							d.vbt.entry[j].StartVidx = br.ReadInt32();
							d.vbt.entry[j].EndVidx = br.ReadInt32();
						}
					}
					
					hierarchy = new BoneHierarchy[num_bone];

					for (int i = 0; i < num_bone;i++ )
					{
						hierarchy[i] = new BoneHierarchy();
						hierarchy[i].ParentIdx = br.ReadInt32();
						hierarchy[i].b = new Vector3
						{
							x = br.ReadSingle(),
							y = br.ReadSingle(),
							z = br.ReadSingle()
						};
						hierarchy[i].NodeName = ConvertBytesToString(br.ReadBytes(32));
						hierarchy[i].UNKNOWN = br.ReadInt32();

					}

				}

				Array.Clear(dress, 0, dress.Length);
				Array.Clear(hierarchy, 0, hierarchy.Length);

				GenerateAllBitmaps();
			}

			

		}

		public int anzahl_textures()
		{

			return num_texture;

		}

		public List<string> get_name_list()
		{

			liste_namen.Clear();

			for (int u = 0; u < num_texture; u++)
            {

				liste_namen.Add(textureinfo[u].name);

			}

			return liste_namen;

		}

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

		public string ConvertBytesToString(byte[] buf)
		{
			int i;
			for (i = 0; i < buf.Length; i++)
				if (buf[i] == 0) break;

			string s = Encoding.ASCII.GetString(buf, 0, i);
			return s;
		}
	}

	class TextureInfo
	{
		public int UNKNOWN;
		public int width;
		public int height;
		public string name;
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

	class Vector3
	{
		public float x;
		public float y;
		public float z;
	}

	class BoneHierarchy 
	{
		public int		ParentIdx;
		public Vector3	b;
		public String	NodeName;
		public int		UNKNOWN;
	}
}
