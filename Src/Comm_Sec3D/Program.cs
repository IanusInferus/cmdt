using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Comm_Sec3D
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			string fn = Program.SelectSecFile();
			if (fn == null) return;

            Game app=new Game(fn);

			app.InitializeGraphics();
			app.Show();

            while (app.Created)
            {
				app.RenderScene();

				Thread.Sleep(1000/60);
				Application.DoEvents();
            }

			app.CleanupGraphics();
		}

		static public string SelectSecFile()
		{
			OpenFileDialog ofg = new OpenFileDialog();

			ofg.Title = "Load .sec";
			ofg.Filter = "Sec files (*.sec)|*.sec";
			ofg.FilterIndex = 0;
			ofg.RestoreDirectory = true;

			string fn;
			if (ofg.ShowDialog() != DialogResult.OK)
				fn = null;
			else
				fn = ofg.FileName;

			return fn;
		}
	}
}