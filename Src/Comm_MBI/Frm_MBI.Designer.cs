namespace Comm_MBI
{
	partial class Frm_MBI
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
			// Frm_MBI
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlText;
			this.ClientSize = new System.Drawing.Size(257, 257);
			this.Name = "Frm_MBI";
			this.Text = ".MBI贴图浏览";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Frm_MBI_Paint);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Frm_MBI_KeyDown);
			this.Load += new System.EventHandler(this.Frm_MBI_Load);
			this.ResumeLayout(false);

		}

		#endregion



	}
}

