using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D9;

namespace Comm_Abi3D
{
	// Shim replicating Microsoft.DirectX.Direct3D.CustomVertex.* on top of SlimDX.
	static class CustomVertex
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct PositionColored
		{
			public float X;
			public float Y;
			public float Z;
			public int Color;

			public const VertexFormat Format = VertexFormat.Position | VertexFormat.Diffuse;
			public static readonly int SizeBytes = Marshal.SizeOf(typeof(PositionColored));

			public PositionColored(float x, float y, float z, int color)
			{
				X = x; Y = y; Z = z; Color = color;
			}

			public PositionColored(Vector3 position, int color)
			{
				X = position.X; Y = position.Y; Z = position.Z; Color = color;
			}

			public Vector3 Position
			{
				get { return new Vector3(X, Y, Z); }
				set { X = value.X; Y = value.Y; Z = value.Z; }
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct PositionColoredTextured
		{
			public float X;
			public float Y;
			public float Z;
			public int Color;
			public float Tu;
			public float Tv;

			public const VertexFormat Format = VertexFormat.Position | VertexFormat.Diffuse | VertexFormat.Texture1;
			public static readonly int SizeBytes = Marshal.SizeOf(typeof(PositionColoredTextured));

			public PositionColoredTextured(float x, float y, float z, int color, float u, float v)
			{
				X = x; Y = y; Z = z; Color = color; Tu = u; Tv = v;
			}

			public Vector3 Position
			{
				get { return new Vector3(X, Y, Z); }
				set { X = value.X; Y = value.Y; Z = value.Z; }
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct PositionNormalColored
		{
			public float X;
			public float Y;
			public float Z;
			public float Nx;
			public float Ny;
			public float Nz;
			public int Color;

			public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Diffuse;
			public static readonly int SizeBytes = Marshal.SizeOf(typeof(PositionNormalColored));

			public PositionNormalColored(float x, float y, float z, float nx, float ny, float nz, int color)
			{
				X = x; Y = y; Z = z; Nx = nx; Ny = ny; Nz = nz; Color = color;
			}

			public Vector3 Position
			{
				get { return new Vector3(X, Y, Z); }
				set { X = value.X; Y = value.Y; Z = value.Z; }
			}

			public Vector3 Normal
			{
				get { return new Vector3(Nx, Ny, Nz); }
				set { Nx = value.X; Ny = value.Y; Nz = value.Z; }
			}
		}
	}
}
