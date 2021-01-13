using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EasyMC
{
	public class Refresh : Form
	{
		private IContainer components;

		public Refresh()
		{
			this.InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(800, 450);
			this.Text = "Refresh";
		}
	}
}