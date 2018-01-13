using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Comm_Mbi3D
{
	class D3DConfiguration
	{
		static DisplayMode mode;
		static Caps caps;

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		static D3DConfiguration()
		{
			mode = Manager.Adapters[0].CurrentDisplayMode;
			caps = Manager.GetDeviceCaps(0, DeviceType.Hardware);
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		static public DepthFormat GetAppropriateDepthFormat()
		{
			DepthFormat df;

			if (Manager.CheckDepthStencilMatch(0, DeviceType.Hardware, mode.Format, Format.A8R8G8B8, DepthFormat.D24X8))
				df = DepthFormat.D24X8;
			else if (Manager.CheckDepthStencilMatch(0, DeviceType.Hardware, mode.Format, Format.A8R8G8B8, DepthFormat.D16))
				df = DepthFormat.D16;
			else
			{
				MessageBox.Show("ȱ���������16λɫ��֧�֣������޷�����", "�Կ����þ���", MessageBoxButtons.OK, MessageBoxIcon.Error);
				throw new Direct3DXException("�Կ����þ��棺ȱ���������16λɫ��֧�֣�ǿ���˳�");
			}
			return df;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		static public bool SupportsHardwareVertexProcessing()
		{
			return caps.DeviceCaps.SupportsHardwareTransformAndLight;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		static public void GetAppropriateMultiSampleType(out MultiSampleType type, out int quality)
		{
			type = MultiSampleType.None;
			quality = 0;

			int result;
			if (Manager.CheckDeviceMultiSampleType(0, DeviceType.Hardware, Format.A8R8G8B8, true, MultiSampleType.NonMaskable, out result, out quality))
				type = MultiSampleType.NonMaskable;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		static public TextureFilter GetAppropriateTextureMagFilter()
		{
			if (caps.TextureFilterCaps.SupportsMagnifyLinear)
				return TextureFilter.Linear;
			else if (caps.TextureFilterCaps.SupportsMagnifyPoint)
				return TextureFilter.Point;
			else
			{
				MessageBox.Show("ȱ�����������ͼ����֧�֣������޷�����", "�Կ����þ���", MessageBoxButtons.OK, MessageBoxIcon.Error);
				throw new Direct3DXException("�Կ����þ��棺ȱ�����������ͼ����֧�֣�ǿ���˳�");
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		static public TextureFilter GetAppropriateTextureMinFilter()
		{
			if (caps.TextureFilterCaps.SupportsMinifyAnisotropic)
				return TextureFilter.Anisotropic;
			else if (caps.TextureFilterCaps.SupportsMinifyLinear)
				return TextureFilter.Linear;
			else if (caps.TextureFilterCaps.SupportsMinifyPoint)
				return TextureFilter.Point;
			else
			{
				MessageBox.Show("ȱ�����������ͼ����֧�֣������޷�����", "�Կ����þ���", MessageBoxButtons.OK, MessageBoxIcon.Error);
				throw new Direct3DXException("�Կ����þ��棺ȱ�����������ͼ����֧�֣�ǿ���˳�");
			}
		}
	}
}