using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;

namespace Comm_Abi3D
{
	class AnimatedModel
	{
		ABI abi;				//ABI�ļ�

		Matrix[] local_transf;	//������������ϵ�е���ת��ƽ��
		Matrix[] global_tranf;	//���ڽ�������������ϵ�еĶ���任��ģ�������˶�֮�����������ϵ
		bool[] boneused;		//�����Ƿ�ʹ�õı��

		CustomVertex.PositionColoredTextured[] vexarray;  //���㼯��
		
		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public Texture[] texture;	//��ͼ����
		public VertexBuffer vexbuf; //���㻺��
		public Vertex[] transv;		//�洢�������ձ任��Ľڵ����꣬��abi�ļ����汣������ǲο�����
		public int[] txtoffset;		//���㼯������Ҫ�л�texture�ĵط���		

		public float time;			//֡ʱ��
		public int model_idx;		//ģ������
		public int animation_idx;	//��������

		public float radius;		//Mesh������뾶
		
		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public AnimatedModel(ABI abi)
		{
			this.abi = abi;

			time = 0f;
			model_idx = 0;
			animation_idx = 0;

			CalculateAll();
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		//����任֮��Ķ��㼯��
		void CalculateAll()
		{
			//��animation�йأ���model�޹�
			InitLocalTransformation();
			UpdateBoneHierarchyTransformation();

			//��model�йأ���animation�޹�
			UpdateVertexSet();		//��������ľ���transv
			GenerateVertexBuf();	//���ɶ��㼯��
		}

		public void SetModel(Device device, int model)
		{
			model_idx = model % (abi.models.Length);

			if (vexbuf != null)
			{
				CalculateAll();

				//ģ��һ���ı䣬�����ؽ����㻺��
				vexbuf.Dispose();
				CreateVertexBuffer(device); //ע�⣬�˺������Ӧ�ظ��������ᣡ
			}
		}

		public void SetAnimation(Device device, int animation)
		{
			time = 0;
			animation_idx = animation % (abi.animations.Length);

			if (vexbuf != null)
			{
				CalculateAll();

				//�����ؽ����㻺�壬ֱ�����ö��㼯��
				vexbuf.SetData(vexarray, 0, LockFlags.Discard);
			}
		}

		public void Animate(float delta)
		{
			time += delta;

			if (vexbuf != null)
			{
				CalculateAll();

				//�����ؽ����㻺�壬ֱ�����ö��㼯��
				vexbuf.SetData(vexarray, 0, LockFlags.Discard);
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		#region ������������ͼ
		public void CreateAllTextures(Device device)
		{
			Debug.WriteLine("[Texture] ��ʼ����...");
			texture = new Texture[abi.textureinfos.Length];
			for (int i = 0; i < texture.Length; i++)
			{
				Debug.WriteLine(String.Format("[Texture] {0}/{1}", i + 1, texture.Length));
				CreateSingleTextureFromPicInfo(device, i);
			}
			Debug.WriteLine("[Texture] ��������");
		}

		//�����ͼ��������ֻ������͸��ɫ��û�п���alpha��ͼ
		private unsafe void CreateSingleTextureFromPicInfo(Device device, int idx)
		{
			TextureInfo pic = abi.textureinfos[idx];
			texture[idx] = new Texture(device, pic.width, pic.height, 0, 0, Format.A8R8G8B8, Pool.Managed);
			Texture t = texture[idx];

			SurfaceDescription s = t.GetLevelDescription(0);
			uint* pData = (uint*)t.LockRectangle(0, LockFlags.None).InternalData.ToPointer();

			int pos = 0;
			for (int i = 0; i < s.Width; i++)
				for (int j = 0; j < s.Height; j++)
				{
					int pal = pic.data[pos++];
					uint color = pic.palette[pal];

					if (color == 0xffff00ff) //transparent key color! ARGB͸��ɫ:0xffff00ff
						*pData++ = (uint)0x0;
					else
						*pData++ = (uint)color;
				}

			t.UnlockRectangle(0);
		}

		public void DisposeAllTextures()
		{
			if (texture == null) return;
			for (int i = 0; i < texture.Length; i++)
				if (texture[i] != null)
				{
					texture[i].Dispose();
					texture[i] = null;
				}
		}
		#endregion

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		#region ���ɶ��㻺��
		void SortingPolygonByTexture(List<Polygon>[] lpoly)
		{
			for (int i = 0; i < abi.textureinfos.Length; i++)
				lpoly[i] = new List<Polygon>();

			foreach (Polygon po in abi.models[model_idx].polygon)
				lpoly[po.texture_id].Add(po);
		}

		void GenerateVertexBuf()
		{
			//ɸѡ������ʹ��ͬһ���ʵĶ����
			List<Polygon>[] lpoly = new List<Polygon>[abi.textureinfos.Length];

			//���ݲ������ּ�һ�¶����
			SortingPolygonByTexture(lpoly);

			int totalf = 0; //��������Ŀ
			foreach (Polygon po in abi.models[model_idx].polygon)
				totalf += po.num_lines - 2;

			int totalv = 3 * totalf;  //������Ŀ
			vexarray = new CustomVertex.PositionColoredTextured[totalv];//�������㼯��

			txtoffset = new int[abi.textureinfos.Length + 1];
			txtoffset[abi.textureinfos.Length] = totalv;

			int pos = 0;
			for (int i = 0; i < abi.textureinfos.Length; i++) //��i����ͼ
			{
				txtoffset[i] = pos;
				foreach (Polygon po in lpoly[i]) //ʹ�õ�i����ͼ�����ж����
				{
					Debug.Assert(po.texture_id == i);
					for (int j = 0; j < po.num_lines - 2; j++) //�Ѷ����ת��Ϊ�����Σ��ŵ����㼯����
					{
						//ע��!�����Ǹ��ݱ任���ģ�������ɶ��㼯�������Ǹ���abi�ļ��еĲο�ģ��
						//ע��!�������õ�ģ��������Ȼ������ϵ��!�����ڻ�������ʱ�򣬱�����ת��������ϵ��
						int idx = po.map_points[0].vertex_id;
						vexarray[pos].X = transv[idx].X;
						vexarray[pos].Y = transv[idx].Y;
						vexarray[pos].Z = transv[idx].Z;
						vexarray[pos].Tu = po.map_points[0].U;
						vexarray[pos].Tv = po.map_points[0].V;
						vexarray[pos].Color = Color.White.ToArgb();
						pos++;

						idx = po.map_points[j + 1].vertex_id;
						vexarray[pos].X = transv[idx].X;
						vexarray[pos].Y = transv[idx].Y;
						vexarray[pos].Z = transv[idx].Z;
						vexarray[pos].Tu = po.map_points[j + 1].U;
						vexarray[pos].Tv = po.map_points[j + 1].V;
						vexarray[pos].Color = Color.White.ToArgb();
						pos++;

						idx = po.map_points[j + 2].vertex_id;
						vexarray[pos].X = transv[idx].X;
						vexarray[pos].Y = transv[idx].Y;
						vexarray[pos].Z = transv[idx].Z;
						vexarray[pos].Tu = po.map_points[j + 2].U;
						vexarray[pos].Tv = po.map_points[j + 2].V;
						vexarray[pos].Color = Color.White.ToArgb();
						pos++;
					}
				}
			}
		}

		bool first = true; //��һ�δ�����־
		public unsafe void CreateVertexBuffer(Device device)
		{

			vexbuf = new VertexBuffer(
			   typeof(CustomVertex.PositionColoredTextured),	//��������
			   vexarray.Length,									//�������
			   device,
			   Usage.WriteOnly | Usage.Dynamic,
			   CustomVertex.PositionColoredTextured.Format,		//�����ʽ
			   Pool.Default);

			#region unsafe�汾
			//int count = vexarray.Length;

			//GraphicsStream vb = vexbuf.Lock(
			//    0,
			//    sizeof(CustomVertex.PositionColoredTextured) * count,
			//    LockFlags.Discard);

			//CustomVertex.PositionColoredTextured* pvb =
			//    (CustomVertex.PositionColoredTextured*)vb.InternalDataPointer;

			//for (int i = 0; i < count; i++)
			//{
			//    pvb->X = vexarray[i].X;
			//    pvb->Y = vexarray[i].Y;
			//    pvb->Z = vexarray[i].Z;
			//    pvb->Tu = vexarray[i].Tu;
			//    pvb->Tv = vexarray[i].Tv;
			//    pvb++;
			//}

			//vexbuf.Unlock();
			#endregion

			#region �йܰ汾
			vexbuf.SetData(vexarray, 0, LockFlags.Discard); //�ٶȺ����unsafe�汾��࣬MDX��SetDataʵ�ֱ����ƺ�����unsafe��
			#endregion

			if (first) //ֻ���ڵ�һ�δ���ʱ���ż���ģ�͵�center��radius����center/radiusʼ���ǲο�ģ�͵�����
			{
				CaculateBoundSphere();
				first = false;
			}
		}

		private void CaculateBoundSphere()
		{
			Vector3 center; //�������������λ�ý�������

			GraphicsStream vertexData = vexbuf.Lock(0, 0, LockFlags.NoOverwrite);
			radius = Geometry.ComputeBoundingSphere(
				vertexData,
				vexarray.Length,
				CustomVertex.PositionColoredTextured.Format,
				out center);
			vexbuf.Unlock();
		}
		#endregion

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		#region ��ֵ�����¹����ı��ر任
		void InterpolationOfLocalTransformation(TransformTimeAxis tta, float time, out Quaternion q, out Vector3 t)
		{
			RotateTimeAxis rta = tta.rta;
			float scale = 0f;
			float rtime;

			if (rta.rkf[rta.num_keyframe - 1].timestamp == 0)
				rtime = 0f;
			else
				rtime = time % (rta.rkf[rta.num_keyframe - 1].timestamp);

			RotateKeyFrame fromrkf = null, torkf = null;
			if (rta.num_keyframe == 1) //TODO: û�п���num_keyframe==0�����
			{
				q = Quaternion_RH_To_LH(rta.rkf[0].rotate);
			}
			else
			{
				for (int i = 0; i < rta.num_keyframe - 1; i++)
				{
					fromrkf = rta.rkf[i];
					torkf = rta.rkf[i + 1];

					if (rtime >= fromrkf.timestamp && rtime <= torkf.timestamp) //�������ؼ�֮֡�䣿
					{
						scale = (rtime - fromrkf.timestamp) / (torkf.timestamp - fromrkf.timestamp);
						break;
					}
				}

				//�޵��޸ģ�������ϵ����ת����ת��������ϵ�ģ��Ա���ʹ��DX������ϵ���������м���
				//����ϵ��ص���Ҫ������RotationQuaternion��Slerp��������˷���ƽ�Ʊ任����������ϵ�޹ص�
				//��֮��������ص��ǣ�����ģ�����ݼ�����������ϵ���ԣ�����DX������ϵ����������ʵ���ϵ�����ϵ����
				Quaternion fromq = Quaternion_RH_To_LH(fromrkf.rotate);
				Quaternion toq = Quaternion_RH_To_LH(torkf.rotate);
				q = Quaternion.Slerp(fromq, toq, scale); //!!
			}

			//////////////////////////////////////////////////////////////////////////////////////////////////////////
			TranslateTimeAxis trta = tta.trta;
			float ttime;

			if (trta.tkf[trta.num_keyframe - 1].timestamp == 0)
				ttime = 0;
			else
				ttime = time % (trta.tkf[trta.num_keyframe - 1].timestamp);

			TranslateKeyFrame fromtkf = null, totkf = null;
			if (trta.num_keyframe == 1) //TODO: û�п���num_keyframe==0�����
			{
				t = trta.tkf[0].translate;
			}
			else
			{
				for (int i = 0; i < trta.num_keyframe - 1; i++) //BUG: û�п���num_keyframe==1�������δ����
				{
					fromtkf = trta.tkf[i];
					totkf = trta.tkf[i + 1];

					if (ttime >= fromtkf.timestamp && ttime <= totkf.timestamp) //�������ؼ�֮֡�䣿
					{
						scale = (ttime - fromtkf.timestamp) / (totkf.timestamp - fromtkf.timestamp);
						break;
					}
				}

				Vector3 fromt = fromtkf.translate;
				Vector3 tot = totkf.translate;
				t = (1 - scale) * fromt + scale * tot;//?? ��ʱ��jsm��Ϊ׼
				//t = scale * fromt + (1-scale) * tot;//?? ��Ȼ����Ч������һ���ģ�
			}
		}

		public void InitLocalTransformation() //���¹����ı��ر任
		{
			boneused = new bool[abi.num_bone];

			local_transf = new Matrix[abi.num_bone]; //ÿ����ͷ��һ��local_transf
			global_tranf = new Matrix[abi.num_bone]; //ÿ����ͷ��һ��global_tranf	

			Animation ani = abi.animations[animation_idx];
			for (int i = 0; i < ani.num_related_bone; i++)	//����������صĹ�ͷ
			{
				int bidx = ani.bae[i].bone_id;				//�ڼ�����ͷ
				TransformTimeAxis tta = ani.bae[i].tta;		//�ù�ͷ��Ӧ��ʱ����

				Quaternion outq;
				Vector3 outv;

				InterpolationOfLocalTransformation(tta, time, out outq, out outv);

				//ע�⣺�����������ϵ��صĺ�����RotationQuaternion
				//ע�⣺�����outq�Ѿ�ת��������ϵ�ˣ���������DX������ϵ����������ʵ���ϵ�����ϵ������

				#region �������汾
				//Matrix x = Matrix.RotationQuaternion(outq); //!!!
				//Matrix y = Matrix.Translation(outv);
				//local_transf[bidx] = x * y; //����ת����ƽ��	
				#endregion

				#region �Ż�����汾
				Matrix x = Matrix.RotationQuaternion(outq); //!!!
				x.M41 += outv.X; //�ȼ�����תת����ƽ��
				x.M42 += outv.Y; //�ȼ�����תת����ƽ��
				x.M43 += outv.Z; //�ȼ�����תת����ƽ��
				local_transf[bidx] = x; //����ת����ƽ��	
				#endregion

				boneused[bidx] = true; //ʹ�ñ��			
			}
		}
		#endregion

		//////////////////////////////////////////////////////////////////////////////////////////////////////////
		public void UpdateBoneHierarchyTransformation() //����bone�㼶���������bone���ۼ�matrix
		{
			for (int i = 0; i < abi.hierarchy.Length; i++)
			{
				if (!boneused[i]) continue; //��ǰ����û��ʹ�øù�ͷ������

				int parent = abi.hierarchy[i].ParentIdx;
				BoneHierarchy bh = abi.hierarchy[i];
				if (parent == -1) //root bone,�������
				{
					#region �������汾
					//global_tranf[i] = local_transf[i] * Matrix.Translation(bh.GlobalOffset);
					#endregion

					#region �Ż�����汾
					global_tranf[i] = local_transf[i];
					global_tranf[i].M41 += bh.GlobalOffset.X;
					global_tranf[i].M42 += bh.GlobalOffset.Y;
					global_tranf[i].M43 += bh.GlobalOffset.Z;
					#endregion
				}
				else
				{
					BoneHierarchy bph = abi.hierarchy[parent];
					Vector3 dt = bh.GlobalOffset - bph.GlobalOffset;

					#region �������汾
					//global_tranf[i] = local_transf[i] * Matrix.Translation(dt) * global_tranf[parent];
					#endregion

					#region �Ż�����汾
					global_tranf[i] = local_transf[i];
					global_tranf[i].M41 += dt.X;
					global_tranf[i].M42 += dt.Y;
					global_tranf[i].M43 += dt.Z;
					global_tranf[i] *= global_tranf[parent];
					#endregion
				}
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////
		public void UpdateVertexSet()
		{
			//�����࣬�������model_idx��������
			transv = new Vertex[abi.models[model_idx].num_vertice];

			VidToBoneTable vt = abi.models[model_idx].vbt;

			for (int i = 0; i < abi.num_bone; i++) //������ͷ
			{
				VidToBoneTableEntry entry = vt.entry[i];
				Vertex[] v = abi.models[model_idx].vertex; //ԭʼ���㼯�ϣ���Ҫ�޸ģ�

				if (!boneused[i]) //�����ͷû���õ�������
				{
					for (int j = entry.StartVidx; j < entry.EndVidx; j++) //�����ù�ͷ��Ӱ�춥��
					{
						//ֱ�Ӳ�����Щ�޹صĵ�
						transv[j] = new Vertex();
						transv[j].X = 0;
						transv[j].Y = 0;
						transv[j].Z = 0;
					}
					continue;
				}
				for (int j = entry.StartVidx; j < entry.EndVidx; j++) //�����ù�ͷ��Ӱ�춥��
				{
					Vector3 r = new Vector3(v[j].X, v[j].Y, v[j].Z); //�ο�λ��
					r -= abi.hierarchy[i].GlobalOffset;
					r = Vector3.TransformCoordinate(r, global_tranf[i]);

					transv[j] = new Vertex();
					transv[j].X = r.X;
					transv[j].Y = r.Y;
					transv[j].Z = r.Z;
				}
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		Quaternion Quaternion_RH_To_LH(Quaternion q)
		{
			//�޵��޸ģ�������ϵ����ת����ת��������ϵ�ģ��Ա���ʹ��DX������ϵ���������м���
			//����ϵ��ص���Ҫ������RotationQuaternion��Slerp��������˷���ƽ�Ʊ任����������ϵ�޹ص�
			//��֮��������ص��ǣ�����ģ�����ݼ�����������ϵ���ԣ�����DX������ϵ����������ʵ���ϵ�����ϵ����
			Quaternion ret = q;
			ret.X = -q.X;
			ret.Y = -q.Y;
			ret.Z = -q.Z;
			return ret;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////
		void PrintMatrix(Matrix mx) //������
		{
			String a = String.Format("{0:F3}\t{1:F3}\t{2:F3}\t{3:F3}", mx.M11, mx.M12, mx.M13, mx.M14);
			String b = String.Format("{0:F3}\t{1:F3}\t{2:F3}\t{3:F3}", mx.M21, mx.M22, mx.M23, mx.M24);
			String c = String.Format("{0:F3}\t{1:F3}\t{2:F3}\t{3:F3}", mx.M31, mx.M32, mx.M33, mx.M34);
			String d = String.Format("{0:F3}\t{1:F3}\t{2:F3}\t{3:F3}", mx.M41, mx.M42, mx.M43, mx.M44);

			Debug.WriteLine("-------------------------------------------");
			Debug.WriteLine(a); Debug.WriteLine(b);
			Debug.WriteLine(c); Debug.WriteLine(d);
		}
	}
}
