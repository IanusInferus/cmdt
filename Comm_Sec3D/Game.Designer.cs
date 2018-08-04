namespace Comm_Sec3D
{
	partial class Game
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Game));
			this.SuspendLayout();
			// 
			// Game
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
			this.ClientSize = new System.Drawing.Size(800, 600);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Game";
			this.Text = "3D .Sec Viewer";
			this.Deactivate += new System.EventHandler(this.Game_Deactivate);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Game_Paint);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Game_MouseClick);
			this.Resize += new System.EventHandler(this.Game_Resize);
			this.Shown += new System.EventHandler(this.Game_Shown);
			this.DoubleClick += new System.EventHandler(this.Game_DoubleClick);
			this.Activated += new System.EventHandler(this.Game_Activated);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Game_MouseUp);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Game_KeyDown);
			this.ResizeEnd += new System.EventHandler(this.Game_ResizeEnd);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Game_MouseDown);
			this.ResumeLayout(false);

		}

		#endregion
	}
}

