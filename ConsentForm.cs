using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace EasyMC
{
	public class ConsentForm : Form
	{
		private Client client;

		private IContainer components;

		private Button button1;

		private Label label1;

		private Button button2;

		private Label label2;

		public ConsentForm(Client client)
		{
			this.InitializeComponent();
			this.client = client;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("I have read the terms and I agree to them.", "Confirm agreement", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
			{
				Application.Exit();
				return;
			}
			Consent.SaveConsent();
			base.Hide();
			this.client.initialize();
			this.client.Show();
			this.client.fetchSettings();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void ConsentForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Application.Exit();
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(12, 266);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(202, 42);
            this.button1.TabIndex = 0;
            this.button1.Text = "I agree, continue";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(683, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Please read and agree to the following terms berfore you can continue";
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(514, 266);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(179, 42);
            this.button2.TabIndex = 2;
            this.button2.Text = "Abort";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(669, 183);
            this.label2.TabIndex = 3;
            this.label2.Text = "Hi dont use  my source I just made ui but this dont work. get real one at easymc." +
    "io";
            // 
            // ConsentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 323);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConsentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EasyMC Client - Terms";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConsentForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
	}
}