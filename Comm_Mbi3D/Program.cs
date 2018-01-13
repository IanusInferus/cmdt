using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Runtime.InteropServices;

namespace Comm_Mbi3D
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
			if (args.Length == 1 && File.Exists(args[0]))
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

		static public string SelectSecFile()
		{
			OpenFileDialog ofg = new OpenFileDialog();

			ofg.Title = "Load .mbi";
			ofg.Filter = "MBI files (*.mbi)|*.mbi";
			ofg.FilterIndex = 0;
			ofg.RestoreDirectory = true;

			string fn;
			if (ofg.ShowDialog() != DialogResult.OK)
				fn = null;
			else
				fn = ofg.FileName;

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
		
		[System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool PeekMessage(
            out Message msg,
            IntPtr hWnd,
            uint messageFilterMin,
            uint messageFilterMax,
            uint flags);
    }
}