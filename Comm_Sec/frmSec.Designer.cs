namespace Comm_Sec
{
	partial class frmSec
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSec));
			this.SuspendLayout();
			// 
			// frmSec
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.DoubleBuffered = true;
			this.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmSec";
			this.Text = ".Sec Birdview";
			this.Load += new System.EventHandler(this.frmSec_Load);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmSec_MouseUp);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmSec_Paint);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmSec_MouseDown);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frmSec_KeyPress);
			this.Resize += new System.EventHandler(this.frmSec_Resize);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmSec_MouseMove);
			this.ResumeLayout(false);

		}

		#endregion
	}
}

