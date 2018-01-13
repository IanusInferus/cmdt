using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Comm_Abi3D
{
	class ABI
	{
		public string filename;

		public int version;
		public int num_timeaxis;
		public int num_animation;

		public int num_texture;

		public TextureInfo[] textureinfos;
		public Bitmap[] bmps;

		public int num_model;
		public int num_bone;
		public Model[] models;

		public BoneHierarchy[] hierarchy;

		public TransformTimeAxis[] timeaxises;	//num_mesh个
		public Animation[] animations;			//num_animation个

		///////////////////////////////////////////////////////////////////////////////////////////////////////////
		public ABI(string filename)
		{
			this.filename = filename;
			FileInfo fi = new FileInfo(filename);
			this.filename = fi.Name;

			using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					int sign = br.ReadInt32();;
					Debug.Assert(sign == 0x424d444c);

					version=int.Parse(ConvertBytesToString(br.ReadBytes(4)));
					Debug.Assert(version == 1060 || version == 1050, "此ABI文件的版本不是1050或1060!");
					
					num_timeaxis = br.ReadInt32();
					num_animation= br.ReadInt32();
					num_texture = br.ReadInt32();

					///////////////////////////////////////////////////////////////////////////////////////////////////////////
					//所有贴图
					textureinfos = new TextureInfo[num_texture];				
					for (int i = 0; i < num_texture; i++)
					{
						textureinfos[i] = new TextureInfo();
						textureinfos[i].UNKNOWN = br.ReadInt32();
						textureinfos[i].width = br.ReadInt32();
						textureinfos[i].height = br.ReadInt32();
						textureinfos[i].name = ConvertBytesToString(br.ReadBytes(32));

						textureinfos[i].palette = new uint[256];
						for (int j = 0; j < 256; j++)
						{
							uint r = br.ReadByte();
							uint g = br.ReadByte();
							uint b = br.ReadByte();

							textureinfos[i].palette[j] = 0xff000000 | (r << 16) | (g << 8) | b;
						}

						textureinfos[i].data = br.ReadBytes(textureinfos[i].width * textureinfos[i].height);
					}

					///////////////////////////////////////////////////////////////////////////////////////////////////////////
					//模型数据
					if (version!=1050)
						br.ReadByte();//未知标志，并不恒为1，奇怪的是，1050格式里面没有这个自己，而1060却有
					//Debug.Assert(br.ReadByte() == 1);

					num_model = br.ReadInt32();
					num_bone = br.ReadInt32();

					models = new Model[num_model];
					for (int i = 0; i < num_model; i++)
					{
						Model d = models[i] = new Model();					
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
								p.U = br.ReadInt16() / 4096f;
								p.V = br.ReadInt16() / 4096f;
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
						hierarchy[i].GlobalOffset = new Vector3(
							br.ReadSingle(),
							br.ReadSingle(),
							br.ReadSingle());
						hierarchy[i].NodeName = ConvertBytesToString(br.ReadBytes(32));
						hierarchy[i].UNKNOWN = br.ReadInt32();

						Debug.WriteLine(String.Format("{0} - [{1}]:{2}", i, hierarchy[i].ParentIdx, hierarchy[i].NodeName));
						Debug.Assert(hierarchy[i].UNKNOWN == 0);
					}
					
					///////////////////////////////////////////////////////////////////////////////////////////////////////////
					//关键帧、时间轴相关结构
					timeaxises = new TransformTimeAxis[num_timeaxis];

					for (int i = 0; i < num_timeaxis; i++)
					{
						TransformTimeAxis ta = timeaxises[i] = new TransformTimeAxis();
						ta.trta = new TranslateTimeAxis();
						ta.rta = new RotateTimeAxis();
						
						TranslateTimeAxis tta=ta.trta;
						tta.num_keyframe = br.ReadInt32();
						tta.tkf=new TranslateKeyFrame[tta.num_keyframe];
						for (int j = 0; j < tta.num_keyframe; j++)
						{
							tta.tkf[j] = new TranslateKeyFrame();
							tta.tkf[j].timestamp=br.ReadByte();
							tta.tkf[j].translate = new Vector3(br.ReadInt16() / 256f, br.ReadInt16() / 256f, br.ReadInt16() / 256f);
						}

						RotateTimeAxis rta = ta.rta;
						rta.num_keyframe = br.ReadInt32();
						rta.rkf = new RotateKeyFrame[rta.num_keyframe];
						for (int j = 0; j < rta.num_keyframe; j++)
						{
							rta.rkf[j] = new RotateKeyFrame();
							rta.rkf[j].timestamp = br.ReadByte();
							rta.rkf[j].rotate = new Quaternion(br.ReadInt16() / 32768f, br.ReadInt16() / 32768f, br.ReadInt16() / 32768f, br.ReadInt16() / 32768f);
						}
					}

					///////////////////////////////////////////////////////////////////////////////////////////////////////////
					//动画行为定义结构
					animations = new Animation[num_animation];

					for (int i = 0; i < num_animation; i++)
					{
						Animation ani = animations[i] = new Animation();
						ani.name = ConvertBytesToString(br.ReadBytes(0x3c));
						ani.num_related_bone = br.ReadInt32();

						ani.bae = new BoneAnimationEntry[ani.num_related_bone];
						for (int j = 0; j < ani.num_related_bone; j++)
						{
							BoneAnimationEntry bae = ani.bae[j] = new BoneAnimationEntry();
							bae.bone_id = br.ReadInt32();
							bae.transform_time_axis_idx = br.ReadInt32();
							bae.tta = timeaxises[bae.transform_time_axis_idx];
						}
					}

					Debug.WriteLine("已读长度: "+fs.Position.ToString());
					Debug.WriteLine("文件长度: "+fs.Length.ToString());
				}

				//GenerateAllBitmaps(); //创建所有位图
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////
		public void GenerateAllBitmaps()
		{
			bmps = new Bitmap[textureinfos.Length];

			for (int i = 0; i < textureinfos.Length; i++)
			{
				TextureInfo p = textureinfos[i];
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
								*(start + x) = (int)p.palette[idx];
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
		public uint[] palette;
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
	}

	class Polygon
	{
		public byte num_lines;
		public byte texture_id;
		public Point[] map_points;
	}

	class Model
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
		public Vector3	GlobalOffset;			//12字节
		public String	NodeName;	//32字节
		public int		UNKNOWN;	//4字节
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	class TranslateKeyFrame //平移相关的关键帧
	{
		public int timestamp;
		public Vector3 translate;
	}

	class TranslateTimeAxis //平移相关关键帧时间轴
	{
		public int num_keyframe;
		public TranslateKeyFrame[] tkf;
	}
	//---------------------------------------------------------------------------------------------------------
	class RotateKeyFrame //旋转相关的关键帧
	{
		public int timestamp;
		public Quaternion rotate;
	}

	class RotateTimeAxis //旋转相关的关键帧时间轴
	{
		public int num_keyframe;
		public RotateKeyFrame[] rkf;
	}
	//---------------------------------------------------------------------------------------------------------
	class TransformTimeAxis //平移和旋转结合起来，变换相关的关键帧时间轴
	{
		public TranslateTimeAxis trta;
		public RotateTimeAxis rta;
	}
	//---------------------------------------------------------------------------------------------------------
	class BoneAnimationEntry  //动作相关的骨骼及其对应的关键帧时间轴
	{
		public int bone_id;					//哪一根骨骼
		public int transform_time_axis_idx;	//对应哪个关键帧时间轴

		public TransformTimeAxis tta;			//辅助
	}

	class Animation //动作
	{
		public string name;
		public int num_related_bone;
		public BoneAnimationEntry[] bae; //重复num_related_bone次
	}
}
