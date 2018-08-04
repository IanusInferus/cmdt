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
		ABI abi;				//ABI文件

		Matrix[] local_transf;	//骨骼本地坐标系中的旋转、平移
		Matrix[] global_tranf;	//用于将骨骼本地坐标系中的顶点变换到模型整体运动之后的世界坐标系
		bool[] boneused;		//骨骼是否使用的标记

		CustomVertex.PositionColoredTextured[] vexarray;  //顶点集合
		
		//////////////////////////////////////////////////////////////////////////////////////////////////////
		public Texture[] texture;	//贴图数组
		public VertexBuffer vexbuf; //顶点缓冲
		public Vertex[] transv;		//存储所有最终变换后的节点坐标，而abi文件里面保存的则是参考姿势
		public int[] txtoffset;		//顶点集合中需要切换texture的地方！		

		public float time;			//帧时间
		public int model_idx;		//模型索引
		public int animation_idx;	//动作索引

		public float radius;		//Mesh包络球半径
		
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
		//计算变换之后的顶点集合
		void CalculateAll()
		{
			//跟animation有关，跟model无关
			InitLocalTransformation();
			UpdateBoneHierarchyTransformation();

			//跟model有关，跟animation无关
			UpdateVertexSet();		//最终输出的就是transv
			GenerateVertexBuf();	//生成顶点集合
		}

		public void SetModel(Device device, int model)
		{
			model_idx = model % (abi.models.Length);

			if (vexbuf != null)
			{
				CalculateAll();

				//模型一旦改变，必须重建顶点缓冲
				vexbuf.Dispose();
				CreateVertexBuffer(device); //注意，此后必须相应地更新坐标轴！
			}
		}

		public void SetAnimation(Device device, int animation)
		{
			time = 0;
			animation_idx = animation % (abi.animations.Length);

			if (vexbuf != null)
			{
				CalculateAll();

				//无需重建顶点缓冲，直接设置顶点集合
				vexbuf.SetData(vexarray, 0, LockFlags.Discard);
			}
		}

		public void Animate(float delta)
		{
			time += delta;

			if (vexbuf != null)
			{
				CalculateAll();

				//无需重建顶点缓冲，直接设置顶点集合
				vexbuf.SetData(vexarray, 0, LockFlags.Discard);
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		#region 创建、销毁贴图
		public void CreateAllTextures(Device device)
		{
			Debug.WriteLine("[Texture] 开始创建...");
			texture = new Texture[abi.textureinfos.Length];
			for (int i = 0; i < texture.Length; i++)
			{
				Debug.WriteLine(String.Format("[Texture] {0}/{1}", i + 1, texture.Length));
				CreateSingleTextureFromPicInfo(device, i);
			}
			Debug.WriteLine("[Texture] 创建结束");
		}

		//这个贴图创建过程只考虑了透明色，没有考虑alpha贴图
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

					if (color == 0xffff00ff) //transparent key color! ARGB透明色:0xffff00ff
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
		#region 生成顶点缓冲
		void SortingPolygonByTexture(List<Polygon>[] lpoly)
		{
			for (int i = 0; i < abi.textureinfos.Length; i++)
				lpoly[i] = new List<Polygon>();

			foreach (Polygon po in abi.models[model_idx].polygon)
				lpoly[po.texture_id].Add(po);
		}

		void GenerateVertexBuf()
		{
			//筛选出所有使用同一材质的多边形
			List<Polygon>[] lpoly = new List<Polygon>[abi.textureinfos.Length];

			//根据材质来分拣一下多边形
			SortingPolygonByTexture(lpoly);

			int totalf = 0; //三角形数目
			foreach (Polygon po in abi.models[model_idx].polygon)
				totalf += po.num_lines - 2;

			int totalv = 3 * totalf;  //顶点数目
			vexarray = new CustomVertex.PositionColoredTextured[totalv];//创建顶点集合

			txtoffset = new int[abi.textureinfos.Length + 1];
			txtoffset[abi.textureinfos.Length] = totalv;

			int pos = 0;
			for (int i = 0; i < abi.textureinfos.Length; i++) //第i个贴图
			{
				txtoffset[i] = pos;
				foreach (Polygon po in lpoly[i]) //使用第i个贴图的所有多边形
				{
					Debug.Assert(po.texture_id == i);
					for (int j = 0; j < po.num_lines - 2; j++) //把多边形转换为三角形，放到顶点集合中
					{
						//注意!这里是根据变换后的模型来生成顶点集，而不是根据abi文件中的参考模型
						//注意!计算所得的模型数据依然是右手系的!所以在画出来的时候，必须先转换成左手系的
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

		bool first = true; //第一次创建标志
		public unsafe void CreateVertexBuffer(Device device)
		{

			vexbuf = new VertexBuffer(
			   typeof(CustomVertex.PositionColoredTextured),	//顶点类型
			   vexarray.Length,									//顶点个数
			   device,
			   Usage.WriteOnly | Usage.Dynamic,
			   CustomVertex.PositionColoredTextured.Format,		//顶点格式
			   Pool.Default);

			#region unsafe版本
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

			#region 托管版本
			vexbuf.SetData(vexarray, 0, LockFlags.Discard); //速度好像跟unsafe版本差不多，MDX中SetData实现本身似乎就是unsafe的
			#endregion

			if (first) //只有在第一次创建时，才计算模型的center和radius，即center/radius始终是参考模型的数据
			{
				CaculateBoundSphere();
				first = false;
			}
		}

		private void CaculateBoundSphere()
		{
			Vector3 center; //包络球体的中心位置将被抛弃

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
		#region 插值、更新骨骼的本地变换
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
			if (rta.num_keyframe == 1) //TODO: 没有考虑num_keyframe==0的情况
			{
				q = Quaternion_RH_To_LH(rta.rkf[0].rotate);
			}
			else
			{
				for (int i = 0; i < rta.num_keyframe - 1; i++)
				{
					fromrkf = rta.rkf[i];
					torkf = rta.rkf[i + 1];

					if (rtime >= fromrkf.timestamp && rtime <= torkf.timestamp) //哪两个关键帧之间？
					{
						scale = (rtime - fromrkf.timestamp) / (torkf.timestamp - fromrkf.timestamp);
						break;
					}
				}

				//无敌修改！将右手系的旋转数据转换成左手系的，以便于使用DX的左手系函数来进行计算
				//左手系相关的主要函数：RotationQuaternion和Slerp，而矩阵乘法、平移变换是与左右手系无关的
				//总之，这里的重点是：保持模型数据及其计算的右手系特性，利用DX的左手系函数来进行实质上的右手系计算
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
			if (trta.num_keyframe == 1) //TODO: 没有考虑num_keyframe==0的情况
			{
				t = trta.tkf[0].translate;
			}
			else
			{
				for (int i = 0; i < trta.num_keyframe - 1; i++) //BUG: 没有考虑num_keyframe==1的情况；未修正
				{
					fromtkf = trta.tkf[i];
					totkf = trta.tkf[i + 1];

					if (ttime >= fromtkf.timestamp && ttime <= totkf.timestamp) //哪两个关键帧之间？
					{
						scale = (ttime - fromtkf.timestamp) / (totkf.timestamp - fromtkf.timestamp);
						break;
					}
				}

				Vector3 fromt = fromtkf.translate;
				Vector3 tot = totkf.translate;
				t = (1 - scale) * fromt + scale * tot;//?? 暂时以jsm的为准
				//t = scale * fromt + (1-scale) * tot;//?? 居然两个效果都是一样的？
			}
		}

		public void InitLocalTransformation() //更新骨骼的本地变换
		{
			boneused = new bool[abi.num_bone];

			local_transf = new Matrix[abi.num_bone]; //每根骨头有一个local_transf
			global_tranf = new Matrix[abi.num_bone]; //每根骨头有一个global_tranf	

			Animation ani = abi.animations[animation_idx];
			for (int i = 0; i < ani.num_related_bone; i++)	//遍历所有相关的骨头
			{
				int bidx = ani.bae[i].bone_id;				//第几根骨头
				TransformTimeAxis tta = ani.bae[i].tta;		//该骨头对应的时间轴

				Quaternion outq;
				Vector3 outv;

				InterpolationOfLocalTransformation(tta, time, out outq, out outv);

				//注意：这里的左右手系相关的函数：RotationQuaternion
				//注意：这里的outq已经转换至左手系了，可以利用DX的左手系函数来进行实质上的右手系计算了

				#region 便于理解版本
				//Matrix x = Matrix.RotationQuaternion(outq); //!!!
				//Matrix y = Matrix.Translation(outv);
				//local_transf[bidx] = x * y; //先旋转，后平移	
				#endregion

				#region 优化计算版本
				Matrix x = Matrix.RotationQuaternion(outq); //!!!
				x.M41 += outv.X; //等价于先转转、再平移
				x.M42 += outv.Y; //等价于先转转、再平移
				x.M43 += outv.Z; //等价于先转转、再平移
				local_transf[bidx] = x; //先旋转，后平移	
				#endregion

				boneused[bidx] = true; //使用标记			
			}
		}
		#endregion

		//////////////////////////////////////////////////////////////////////////////////////////////////////////
		public void UpdateBoneHierarchyTransformation() //遍历bone层级，计算各个bone的累计matrix
		{
			for (int i = 0; i < abi.hierarchy.Length; i++)
			{
				if (!boneused[i]) continue; //当前动画没有使用该骨头，跳过

				int parent = abi.hierarchy[i].ParentIdx;
				BoneHierarchy bh = abi.hierarchy[i];
				if (parent == -1) //root bone,无需更新
				{
					#region 便于理解版本
					//global_tranf[i] = local_transf[i] * Matrix.Translation(bh.GlobalOffset);
					#endregion

					#region 优化计算版本
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

					#region 便于理解版本
					//global_tranf[i] = local_transf[i] * Matrix.Translation(dt) * global_tranf[parent];
					#endregion

					#region 优化计算版本
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
			//不冗余，必须根据model_idx重新生成
			transv = new Vertex[abi.models[model_idx].num_vertice];

			VidToBoneTable vt = abi.models[model_idx].vbt;

			for (int i = 0; i < abi.num_bone; i++) //遍历骨头
			{
				VidToBoneTableEntry entry = vt.entry[i];
				Vertex[] v = abi.models[model_idx].vertex; //原始顶点集合，不要修改！

				if (!boneused[i]) //这根骨头没有用到，跳过
				{
					for (int j = entry.StartVidx; j < entry.EndVidx; j++) //遍历该骨头所影响顶点
					{
						//直接不画这些无关的点
						transv[j] = new Vertex();
						transv[j].X = 0;
						transv[j].Y = 0;
						transv[j].Z = 0;
					}
					continue;
				}
				for (int j = entry.StartVidx; j < entry.EndVidx; j++) //遍历该骨头所影响顶点
				{
					Vector3 r = new Vector3(v[j].X, v[j].Y, v[j].Z); //参考位置
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
			//无敌修改！将右手系的旋转数据转换成左手系的，以便于使用DX的左手系函数来进行计算
			//左手系相关的主要函数：RotationQuaternion和Slerp，而矩阵乘法、平移变换是与左右手系无关的
			//总之，这里的重点是：保持模型数据及其计算的右手系特性，利用DX的左手系函数来进行实质上的右手系计算
			Quaternion ret = q;
			ret.X = -q.X;
			ret.Y = -q.Y;
			ret.Z = -q.Z;
			return ret;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////
		void PrintMatrix(Matrix mx) //调试用
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
