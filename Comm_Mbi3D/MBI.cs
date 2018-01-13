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
		public byte[] texturetype; //��ͼ���ͱ�ǣ���������ͼ��ţ���ֵΪ��ǰ��ͼ����

		public Version version = Version.Commandos2;

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		string filename; //�ļ�ȫ����������չ����·��
		string nameonly; //�������ļ���������չ����·��

		public MBI(string fn)
		{
			filename = fn; //����ļ�ȫ��

			string dir, ext;
			SplitFileName(filename, out dir, out nameonly, out ext); //����ļ�������ext��·��

			ReadAll();			//��ȡ����mbi��������
			MarkTextureType();	//���������ͼ������
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		void ReadAll()
		{
			//��Dragon_UNPAcker�������������Ǵ���ģ���Ҫ������������ѹ.mbi�ļ�
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
						Debug.Assert(false); //�ļ����ͳ���

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
							polygons[i].attribute = 0; //���ã�
							polygons[i].num_lines = br.ReadByte();
						}
						else if (version == Version.Commandos3)
						{
							byte n = br.ReadByte();
							polygons[i].attribute = (byte)(n >> 4);					
							
							byte test = (byte)(n >> 4);
							Debug.Assert(test == 0 || test == 1 || test == 2 || test == 4); //����δ֪����ͼ����

							polygons[i].num_lines = (byte)(n & 0xf);
						}

						polygons[i].texture_id = br.ReadByte();

						polygons[i].map_points = new Point2D[polygons[i].num_lines];
						for (int j = 0; j < polygons[i].num_lines; j++)
						{
							polygons[i].map_points[j] = new Point2D();
							polygons[i].map_points[j].vertex_id = br.ReadInt16();
							polygons[i].map_points[j].U = br.ReadInt16() / 4096f; //����4096!
							polygons[i].map_points[j].V = br.ReadInt16() / 4096f; //����4096!

							polygons[i].map_points[j].vertex = vertice[polygons[i].map_points[j].vertex_id];
						}
					}

					int num_objects = br.ReadInt32();
					objects = new Object[num_objects];
					for (int i = 0; i < num_objects; i++)
					{
						objects[i] = new Object();

						objects[i].obj_name = ConvertBytesToString(br.ReadBytes(44));
						Debug.WriteLine(String.Format("[��������] {0}", objects[i].obj_name));

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
							txtinfos[i].texture_name = ConvertBytesToString(br.ReadBytes(32)); //ֻ��comm3����Ҫ��
							Debug.WriteLine(String.Format("[��ͼ����] {0}", txtinfos[i].texture_name));
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

					Debug.WriteLine(String.Format("[�ļ�ָ��] {0}", fs.Position));
				}
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		void MarkTextureType() //���������ͼ������
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
			GenerateAllBitmaps();	//������ͼ���ͣ�������ͼ��Bitmaps
			ExportTexture(txtdir);	//������Bitmaps����ΪPNG
			ExportObj(objfn);		//����Wavefront OBJ�ļ�					
			ExportMtl(mtlfn);		//����Wavefront MTL�ļ�

			DisposAllBitmaps();		//��β�������ͷ�����λͼ��Դ
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		List<Polygon>[] lpoly;

		//�����ж���ΰ�����ͼ��������
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

			List<string> v = new List<string>();  //���㼯��
			List<string> vt = new List<string>(); //ӳ��㼯��
			List<string> f = new List<string>();  //�漯��

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
						vt.Add(string.Format("vt  {0} {1}", poly.map_points[j].U, 1 - poly.map_points[j].V)); //���գ����"1-"��ֱ�����ˣ���
					
					StringBuilder s=new StringBuilder();
					s.Append("f  ");
					for (int j = poly.num_lines - 1; j >= 0; j--)
						s.AppendFormat("{0}/{1} ", poly.map_points[j].vertex_id + 1, pos++);
					
					f.Add(s.ToString());
				}
			}

			//���û��ָ��Obj�ļ�������ʹ��Ĭ���ļ���
			if (objfn == null) objfn = GetObjFileName();

			using (TextWriter tw = new StreamWriter(objfn))
			{
				tw.WriteLine(string.Format("mtllib {0}.mtl\n", this.nameonly));

				tw.WriteLine(string.Format("# ��������: {0}\n", v.Count));
				foreach (string s in v) tw.WriteLine(s);	//������ж��㼯��

				tw.WriteLine(string.Format("# ��ͼӳ�������: {0}\n", vt.Count));
				foreach (string s in vt) tw.WriteLine(s);	//���������ͼӳ��㼯��

				tw.WriteLine(string.Format("# ���������: {0}\n", f.Count));
				foreach (string s in f) tw.WriteLine(s);	//������ж���μ���
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		void ExportMtl(string mtlfn)
		{
			//���û��ָ��Mtl�ļ�������ʹ��Ĭ���ļ���
			if (mtlfn == null) mtlfn = GetMtlFileName();

			using (TextWriter tw = new StreamWriter(mtlfn))
			{
				for (int i = 0; i < txtinfos.Length; i++)	//���ÿһ����������Ӧ����ͼ
				{
					tw.WriteLine("newmtl {0}", i);
					tw.WriteLine("illum 0");				
					tw.WriteLine("map_Kd {0}.png", nameonly + "_" + i);
					//tw.WriteLine("Ka 0.2 0.2 0.2"); //ambient, �����⣬����ӹ���
					//tw.WriteLine("Kd 0.8 0.8 0.8"); //diffuse, �����䣬��ֱ�ӹ���
					tw.WriteLine("Kd 1 1 1"); //diffuse, �����䣬��ֱ�ӹ���
					tw.WriteLine();
				}
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		void ExportTexture(string txtdir)
		{
			//���û��ָ����ͼĿ¼����ʹ��Ĭ��Ŀ¼
			if (txtdir == null) txtdir = GetTextureDirName();

			//��֤��ͼĿ¼һ������
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

				bmps[i] = new Bitmap(p.width, p.height,PixelFormat.Format32bppArgb); //ע�⣬����Я��Alphaͨ��

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
								case 0:  //��ͨ��ͼ
								case 1:  //͸��ɫ��ͼ
									if (p.color[idx] == 0xffff00ff)
										*(start + x) = 0x0; //alphaΪ0����Ϊ��ɫ������Ϊ��ɫ������3DS max��Ӧ��͸������ͼʱ���б�Ե��
									else
										*(start + x) = p.color[idx];//��͸��
									break;
								case 2:  //Alpha������ͼ
									uint a = p.color[idx] & 0xff;
									//*(start + x) = ((a >> 2) << 24) | (a << 16) | (a << 8) | a;
									*(start + x) = ((a >> 2) << 24) | 0xffffff; //�˴���Ϊ����ʾһ��
									break;
								case 4:  //����ʯ������ͼ
									*(start + x) = (uint)((0xE0 << 24) | (p.color[idx] & 0xffffff));
									break;
								default: //�Ӿ����棬δ֪����ͼ����!
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

		string GetObjFileName()	//Ĭ��Obj�ļ���
		{
			string dir, name, ext;
			SplitFileName(filename, out dir, out name, out ext);
			return dir + @"\" + name + ".obj";
		}

		string GetMtlFileName()	//Ĭ��Mtl�ļ���
		{
			string dir, name, ext;
			SplitFileName(filename, out dir, out name, out ext);
			return dir + @"\" + name + ".mtl";
		}

		string GetTextureDirName() //Ĭ����ͼĿ¼
		{
			string dir, name, ext;
			SplitFileName(filename, out dir, out name, out ext);

			//return dir + @"\" + name; //"F:\MBI2\RY02"��ʽ��Ŀ¼
			return dir + @"\maps"; //��ǰ·���µ�maps��Ŀ¼��
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
		public short vertex_id;	//׼ȷ
		public float U;			//׼ȷ��/4096f�õ�Tu (16*256=32*128=4096)
		public float V;			//׼ȷ��/4996f�õ�Tv (16*256=32*128=4096)

		public Vertex vertex;
	}

	class Polygon
	{
		public byte num_lines;
		public byte attribute;	//only for comm3��0��1��2���ֱ��ʾ��ͨ��͸��������Ч��
		public byte texture_id;
		public Point2D[] map_points;
	}

	class Object
	{
		public string obj_name;	//44�ֽ�
		public int start_polygon_id;
		public int end_polygon_id;
	}

	class TextureInfo
	{
		public int UNKNOWN;
		public int width;
		public int height;
		public string texture_name;	//32�ֽ�,only for comm3
		public uint[] color;	//A8R8G8B8
		public byte[] data;
	}
}