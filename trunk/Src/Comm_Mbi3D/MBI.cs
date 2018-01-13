using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace Comm_Mbi3D
{
	class MBI
	{
		public enum Version { Commandos2, Commandos3 };

		public Vertex[] vertice;
		public Object[] objects;
		public Polygon[] polygons;
		public TextureInfo[] txtinfos;
		public byte[] texturetype; //贴图类型标记，索引是贴图编号，其值为当前贴图类型

		public Version version = Version.Commandos2;

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		string filename; //文件全名，包括扩展名和路径
		string nameonly; //单独的文件名，无扩展名和路径

		public MBI(string fn)
		{
			filename = fn; //获得文件全名

			string dir, ext;
			SplitFileName(filename, out dir, out nameonly, out ext); //获得文件名，无ext和路径

			ReadAll();			//读取所有mbi数据内容
			MarkTextureType();	//标记所有贴图的类型
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		void ReadAll()
		{
			//用Dragon_UNPAcker读出来的数据是错误的，不要用这个软件来解压.mbi文件
			using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					int v = br.ReadInt32();
					if (v == 0x4d424931)
						version = Version.Commandos2;
					else if (v == 0x4d424932)
						version = Version.Commandos3;
					else
						Debug.Assert(false); //文件类型出错！

					int num_vertice = br.ReadInt32();
					int num_polygons = br.ReadInt32();

					vertice = new Vertex[num_vertice];
					for (int i = 0; i < num_vertice; i++)
					{
						vertice[i] = new Vertex();
						vertice[i].X = br.ReadSingle();
						vertice[i].Y = br.ReadSingle();
						vertice[i].Z = br.ReadSingle();
					}

					polygons = new Polygon[num_polygons];
					for (int i = 0; i < num_polygons; i++)
					{
						polygons[i] = new Polygon();
						if (version == Version.Commandos2)
						{
							polygons[i].attribute = 0; //无用！
							polygons[i].num_lines = br.ReadByte();
						}
						else if (version == Version.Commandos3)
						{
							byte n = br.ReadByte();
							polygons[i].attribute = (byte)(n >> 4);					
							
							byte test = (byte)(n >> 4);
							Debug.Assert(test == 0 || test == 1 || test == 2 || test == 4); //发现未知的贴图类型

							polygons[i].num_lines = (byte)(n & 0xf);
						}

						polygons[i].texture_id = br.ReadByte();

						polygons[i].map_points = new Point2D[polygons[i].num_lines];
						for (int j = 0; j < polygons[i].num_lines; j++)
						{
							polygons[i].map_points[j] = new Point2D();
							polygons[i].map_points[j].vertex_id = br.ReadInt16();
							polygons[i].map_points[j].U = br.ReadInt16() / 4096f; //除以4096!
							polygons[i].map_points[j].V = br.ReadInt16() / 4096f; //除以4096!

							polygons[i].map_points[j].vertex = vertice[polygons[i].map_points[j].vertex_id];
						}
					}

					int num_objects = br.ReadInt32();
					objects = new Object[num_objects];
					for (int i = 0; i < num_objects; i++)
					{
						objects[i] = new Object();

						objects[i].obj_name = ConvertBytesToString(br.ReadBytes(44));
						Debug.WriteLine(String.Format("[对象名称] {0}", objects[i].obj_name));

						objects[i].start_polygon_id = br.ReadInt32();
						objects[i].end_polygon_id = br.ReadInt32() - 1;
					}

					int num_textures = br.ReadInt32();
					txtinfos = new TextureInfo[num_textures];
					for (int i = 0; i < num_textures; i++)
					{
						txtinfos[i] = new TextureInfo();
						txtinfos[i].UNKNOWN = br.ReadInt32();
						txtinfos[i].width = br.ReadInt32();
						txtinfos[i].height = br.ReadInt32();

						if (version == Version.Commandos3)
						{
							txtinfos[i].texture_name = ConvertBytesToString(br.ReadBytes(32)); //只有comm3才需要！
							Debug.WriteLine(String.Format("[贴图名称] {0}", txtinfos[i].texture_name));
						}
						txtinfos[i].color = new uint[256];
						for (int j = 0; j < 256; j++)
						{
							byte r = br.ReadByte();
							byte g = br.ReadByte();
							byte b = br.ReadByte();

							txtinfos[i].color[j] = (uint)((0xff << 24) | (r << 16) | (g << 8) | b);
						}

						txtinfos[i].data = br.ReadBytes(txtinfos[i].width * txtinfos[i].height);
					}

					Debug.WriteLine(String.Format("[文件指针] {0}", fs.Position));
				}
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		void MarkTextureType() //标记所有贴图的类型
		{
			texturetype = new byte[txtinfos.Length];
			for (int i = 0; i < polygons.Length; i++)
			{
				Polygon poly = polygons[i];
				if (poly.attribute != 0)
					texturetype[poly.texture_id] = poly.attribute;
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		public void ExportToObjFile(string objfn,string mtlfn,string txtdir)
		{
			GenerateAllBitmaps();	//根据贴图类型，生成贴图的Bitmaps
			ExportTexture(txtdir);	//将所有Bitmaps导出为PNG
			ExportObj(objfn);		//导出Wavefront OBJ文件					
			ExportMtl(mtlfn);		//导出Wavefront MTL文件

			DisposAllBitmaps();		//收尾工作，释放所有位图资源
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		List<Polygon>[] lpoly;

		//将所有多边形按照贴图索引排序
		void SortingPolygonsByTextureID()
		{
			lpoly = new List<Polygon>[txtinfos.Length];

			for (int i = 0; i < txtinfos.Length; i++)
				lpoly[i] = new List<Polygon>();

			for (int i = 0; i < polygons.Length; i++)
			{
				Polygon poly = polygons[i];
				int idx = poly.texture_id;
				lpoly[idx].Add(poly);
			}
		}

		void ExportObj(string objfn)
		{
			SortingPolygonsByTextureID();

			List<string> v = new List<string>();  //顶点集合
			List<string> vt = new List<string>(); //映射点集合
			List<string> f = new List<string>();  //面集合

			for (int i = 0; i < vertice.Length; i++)
				v.Add(string.Format("v  {0} {1} {2}", vertice[i].X, vertice[i].Y, vertice[i].Z));

			int pos = 1;
			for (int i = 0; i < txtinfos.Length; i++)
			{
				f.Add(string.Format("usemtl  {0}", i));
				f.Add(string.Format("g  {0}", i));

				foreach (Polygon poly in lpoly[i])
				{
					for (int j = poly.num_lines - 1; j >= 0; j--)
						vt.Add(string.Format("vt  {0} {1}", poly.map_points[j].U, 1 - poly.map_points[j].V)); //我日！这个"1-"简直害死人！！
					
					StringBuilder s=new StringBuilder();
					s.Append("f  ");
					for (int j = poly.num_lines - 1; j >= 0; j--)
						s.AppendFormat("{0}/{1} ", poly.map_points[j].vertex_id + 1, pos++);
					
					f.Add(s.ToString());
				}
			}

			//如果没有指定Obj文件名，则使用默认文件名
			if (objfn == null) objfn = GetObjFileName();

			using (TextWriter tw = new StreamWriter(objfn))
			{
				tw.WriteLine(string.Format("mtllib {0}.mtl\n", this.nameonly));

				tw.WriteLine(string.Format("# 顶点数量: {0}\n", v.Count));
				foreach (string s in v) tw.WriteLine(s);	//输出所有顶点集合

				tw.WriteLine(string.Format("# 贴图映射点数量: {0}\n", vt.Count));
				foreach (string s in vt) tw.WriteLine(s);	//输出所有贴图映射点集合

				tw.WriteLine(string.Format("# 多边形数量: {0}\n", f.Count));
				foreach (string s in f) tw.WriteLine(s);	//输出所有多边形集合
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		void ExportMtl(string mtlfn)
		{
			//如果没有指定Mtl文件名，则使用默认文件名
			if (mtlfn == null) mtlfn = GetMtlFileName();

			using (TextWriter tw = new StreamWriter(mtlfn))
			{
				for (int i = 0; i < txtinfos.Length; i++)	//输出每一个材质所对应的贴图
				{
					tw.WriteLine("newmtl {0}", i);
					tw.WriteLine("illum 0");				
					tw.WriteLine("map_Kd {0}.png", nameonly + "_" + i);
					//tw.WriteLine("Ka 0.2 0.2 0.2"); //ambient, 环境光，即间接光照
					//tw.WriteLine("Kd 0.8 0.8 0.8"); //diffuse, 漫反射，即直接光照
					tw.WriteLine("Kd 1 1 1"); //diffuse, 漫反射，即直接光照
					tw.WriteLine();
				}
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		void ExportTexture(string txtdir)
		{
			//如果没有指定贴图目录，则使用默认目录
			if (txtdir == null) txtdir = GetTextureDirName();

			//保证贴图目录一定存在
			DirectoryInfo di = new DirectoryInfo(txtdir);
			if (!di.Exists) Directory.CreateDirectory(txtdir);

			for (int i = 0; i < txtinfos.Length; i++)
			{
				string n = string.Format(@"{0}\{1}_{2}.png", txtdir, nameonly, i);
				bmps[i].Save(n, ImageFormat.Png);
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		Bitmap[] bmps;
		
		void DisposAllBitmaps()
		{
			for (int i = 0; i < bmps.Length; i++)
				bmps[i].Dispose();

			bmps = null;
			GC.Collect();
		}

		void GenerateAllBitmaps()
		{
			bmps = new Bitmap[txtinfos.Length];

			for (int i = 0; i < txtinfos.Length; i++)
			{			
				TextureInfo p = txtinfos[i];

				bmps[i] = new Bitmap(p.width, p.height,PixelFormat.Format32bppArgb); //注意，必须携带Alpha通道

				BitmapData bmpdata = bmps[i].LockBits(new Rectangle(0, 0, bmps[i].Width, bmps[i].Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				IntPtr ptr = bmpdata.Scan0;
				int pitch = bmpdata.Stride / 4;

				unsafe
				{
					int t = 0;
					uint* start = (uint*)ptr;
					for (int y = 0; y < p.height; y++)
					{
						for (int x = 0; x < p.width; x++)
						{
							int idx = p.data[t++];
							switch (texturetype[i])
							{
								case 0:  //普通贴图
								case 1:  //透明色贴图
									if (p.color[idx] == 0xffff00ff)
										*(start + x) = 0x0; //alpha为0，且为黑色（必须为黑色，否则3DS max里应用透明度贴图时会有边缘）
									else
										*(start + x) = p.color[idx];//不透明
									break;
								case 2:  //Alpha光晕贴图
									uint a = p.color[idx] & 0xff;
									//*(start + x) = ((a >> 2) << 24) | (a << 16) | (a << 8) | a;
									*(start + x) = ((a >> 2) << 24) | 0xffffff; //此处改为与显示一致
									break;
								case 4:  //大理石反射贴图
									*(start + x) = (uint)((0xE0 << 24) | (p.color[idx] & 0xffffff));
									break;
								default: //视觉警告，未知的贴图类型!
									*(start + x) = 0xffff00ff;
									break;
							}
						}
						start += pitch;
					}
				}

				bmps[i].UnlockBits(bmpdata);
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		string ConvertBytesToString(byte[] buf)
		{		
			int i;
			for (i = 0; i < buf.Length; i++) 
				if (buf[i] == 0) break;

			string s = Encoding.ASCII.GetString(buf, 0, i);
			return s;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		void SplitFileName(string filename, out string dir, out string name, out string ext)
		{
			FileInfo fi = new FileInfo(filename);
			dir = fi.DirectoryName;
			ext = fi.Extension;
			Debug.Assert(ext.ToUpper() == ".MBI");
			name = fi.Name.Substring(0, fi.Name.Length - 4);
		}

		string GetObjFileName()	//默认Obj文件名
		{
			string dir, name, ext;
			SplitFileName(filename, out dir, out name, out ext);
			return dir + @"\" + name + ".obj";
		}

		string GetMtlFileName()	//默认Mtl文件名
		{
			string dir, name, ext;
			SplitFileName(filename, out dir, out name, out ext);
			return dir + @"\" + name + ".mtl";
		}

		string GetTextureDirName() //默认贴图目录
		{
			string dir, name, ext;
			SplitFileName(filename, out dir, out name, out ext);

			//return dir + @"\" + name; //"F:\MBI2\RY02"形式的目录
			return dir + @"\maps"; //当前路径下的maps子目录下
		}
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	class Vertex
	{
		public float X;
		public float Y;
		public float Z;
	}

	class Point2D
	{
		public short vertex_id;	//准确
		public float U;			//准确：/4096f得到Tu (16*256=32*128=4096)
		public float V;			//准确：/4996f得到Tv (16*256=32*128=4096)

		public Vertex vertex;
	}

	class Polygon
	{
		public byte num_lines;
		public byte attribute;	//only for comm3：0、1、2，分别表示普通、透明、阳光效果
		public byte texture_id;
		public Point2D[] map_points;
	}

	class Object
	{
		public string obj_name;	//44字节
		public int start_polygon_id;
		public int end_polygon_id;
	}

	class TextureInfo
	{
		public int UNKNOWN;
		public int width;
		public int height;
		public string texture_name;	//32字节,only for comm3
		public uint[] color;	//A8R8G8B8
		public byte[] data;
	}
}