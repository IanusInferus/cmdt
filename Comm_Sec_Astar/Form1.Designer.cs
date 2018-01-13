namespace Comm_Sec_Astar
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
			this.SuspendLayout();
			// 
			// frmSec
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Name = "frmSec";
			this.Text = "2D Sec A*";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmSec_Paint);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frmSec_KeyPress);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmSec_MouseDown);
			this.Load += new System.EventHandler(this.frmSec_Load);
			this.ResumeLayout(false);

		}

		#endregion
	}
}

