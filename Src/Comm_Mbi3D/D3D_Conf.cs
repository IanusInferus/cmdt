using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using SlimDX;
using SlimDX.Direct3D9;

namespace Comm_Mbi3D
{
	class D3DConfiguration
	{
		static Direct3D d3d;
		static DisplayMode mode;
		static Capabilities caps;

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		static D3DConfiguration()
		{
			d3d = new Direct3D();
			mode = d3d.Adapters[0].CurrentDisplayMode;
			caps = d3d.GetDeviceCaps(0, DeviceType.Hardware);
		}

		public static Direct3D D3D { get { return d3d; } }

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		static public Format GetAppropriateDepthFormat()
		{
			Format df;

			if (d3d.CheckDepthStencilMatch(0, DeviceType.Hardware, mode.Format, Format.A8R8G8B8, Format.D24X8))
				df = Format.D24X8;
			else if (d3d.CheckDepthStencilMatch(0, DeviceType.Hardware, mode.Format, Format.A8R8G8B8, Format.D16))
				df = Format.D16;
			else
			{
				MessageBox.Show("缺乏最基本的16位色深支持，程序无法继续", "显卡配置警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
				throw new Direct3D9Exception("显卡配置警告：缺乏最基本的16位色深支持，强制退出");
			}
			return df;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		static public bool SupportsHardwareVertexProcessing()
		{
			return (caps.DeviceCaps & DeviceCaps.HWTransformAndLight) != 0;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		static public void GetAppropriateMultiSampleType(out MultisampleType type, out int quality)
		{
			type = MultisampleType.None;
			quality = 0;

			if (d3d.CheckDeviceMultisampleType(0, DeviceType.Hardware, Format.A8R8G8B8, true, MultisampleType.NonMaskable, out quality))
				type = MultisampleType.NonMaskable;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		static public TextureFilter GetAppropriateTextureMagFilter()
		{
			if ((caps.TextureFilterCaps & FilterCaps.MagLinear) != 0)
				return TextureFilter.Linear;
			else if ((caps.TextureFilterCaps & FilterCaps.MagPoint) != 0)
				return TextureFilter.Point;
			else
			{
				MessageBox.Show("缺乏最基本的贴图过滤支持，程序无法继续", "显卡配置警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
				throw new Direct3D9Exception("显卡配置警告：缺乏最基本的贴图过滤支持，强制退出");
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		static public TextureFilter GetAppropriateTextureMinFilter()
		{
			if ((caps.TextureFilterCaps & FilterCaps.MinAnisotropic) != 0)
				return TextureFilter.Anisotropic;
			else if ((caps.TextureFilterCaps & FilterCaps.MinLinear) != 0)
				return TextureFilter.Linear;
			else if ((caps.TextureFilterCaps & FilterCaps.MinPoint) != 0)
				return TextureFilter.Point;
			else
			{
				MessageBox.Show("缺乏最基本的贴图过滤支持，程序无法继续", "显卡配置警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
				throw new Direct3D9Exception("显卡配置警告：缺乏最基本的贴图过滤支持，强制退出");
			}
		}
	}
}
