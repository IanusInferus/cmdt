using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Runtime.InteropServices;


namespace Comm_Abi3D
{
	static class Program
	{
		static Game app;

		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Application.Idle += new EventHandler(Application_Idle);

			string fn;
			if (args.Length == 1 && ValidateFileVersion(args[0]))
				fn = args[0];		
			else
				fn = Program.SelectSecFile();

			if (fn == null) return;

			app = new Game(fn);
			app.InitializeGraphics();
			System.Windows.Forms.Application.Run(app);
			app.Cleanup();

			//app = new Game(fn);
			//app.InitializeGraphics();
			//app.Show();
			//while (app.Created)
			//{
			//    app.Render();

			//    Thread.Sleep(1000 / 60);
			//    Application.DoEvents();
			//}
			//app.Cleanup();
		}

		static void Application_Idle(object sender, EventArgs e)
		{
			while (AppStillIdle)
			{
				app.Render();
				Thread.Sleep(1000 / 60);
			}
		}

		static public bool ValidateFileVersion(string fn)
		{
			if (!File.Exists(fn)) return false; //文件不存在,false
			
			int sign;
			short a;
			byte b, c;
			using (FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					sign = br.ReadInt32();
					a = br.ReadInt16();
					b = br.ReadByte();
					c = br.ReadByte();
				}
			}

			//LDMB 10 X 0
			//只有1050 or 1060才合法
			if (sign == 0x424d444c && a == 0x3031 && c == 0x30 && (b == 0x35 || b == 0x36)) return true;
			return false;
		}

		static public string SelectSecFile()
		{
			string fn;

			while (true)
			{
				OpenFileDialog ofg = new OpenFileDialog();

				ofg.Title = "Load .abi";
				ofg.Filter = "ABI files (*.abi)|*.abi";
				ofg.FilterIndex = 0;
				ofg.RestoreDirectory = true;

				if (ofg.ShowDialog() != DialogResult.OK)
					fn = null;
				else
					fn = ofg.FileName;

				if (fn == null || ValidateFileVersion(fn)) break;
			}

			return fn;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Message
		{
			public IntPtr hWnd;
			public uint msg;
			public IntPtr wParam;
			public IntPtr lParam;
			public uint time;
			public System.Drawing.Point p;
		}

		static private bool AppStillIdle
		{
			get
			{
				Message msg;
				return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
			}
		}

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern bool PeekMessage(
			out Message msg,
			IntPtr hWnd,
			uint messageFilterMin,
			uint messageFilterMax,
			uint flags);
	}
}