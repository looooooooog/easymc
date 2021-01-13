using EasyMC.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyMC
{
	public class Client : Form
	{
		private EasyMC.Settings settings;

		private string activeSession;

		private IContainer components;

		private FlowLayoutPanel altsList;

		private Panel loginPanel;

		private LinkLabel linkLabel1;

		private Label label1;

		private TextBox tokenInput;

		private Button loginButton;

		private Button mojangButton;

		private Label label3;

		private Panel infoPanel;

		private Button redeemButton;

		private PictureBox renderPictureBox;

		private Label mcNameLabel;

		private Button activeButton;

		private Label statusLabel;

		private Label label5;

		private Button refreshButton;

		private Button buttonRemove;

		private Label label2;

		private TextBox tokenInfoInput;

		private Label refreshInfoLabel;
        private Button easymcButton;
        private Label playInfoLabel;

		public Client()
		{
			this.InitializeComponent();
		}

		private void activeButton_Click(object sender, EventArgs e)
		{
			if (!(sender is Button))
			{
				return;
			}
			Button button = (Button)sender;
			if (button.Tag == null || !(button.Tag is string))
			{
				return;
			}
			Session session = Sessions.getSessions().Single<Session>((Session sess) => sess.session == (string)button.Tag);
			if (session == null)
			{
				return;
			}
			Accounts.setActiveProfile(session.session, session.mcName, session.uuid, session.userId);
			this.renderSessions();
		}

		private void alt_Click(object sender, EventArgs e)
		{
			this.activeSession = ((Control)sender).Tag.ToString();
			this.renderSessions();
		}

		private void button1_Click(object sender, EventArgs e)
		{
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
		}

		private void buttonRefresh_Click(object sender, EventArgs e)
		{
			Process.Start("https://easymc.io/renew");
		}

		private void buttonRemove_Click(object sender, EventArgs e)
		{
			if (!(sender is Button))
			{
				return;
			}
			Button button = (Button)sender;
			if (button.Tag == null || !(button.Tag is string))
			{
				return;
			}
			Session session = Sessions.getSessions().Single<Session>((Session sess) => sess.session == (string)button.Tag);
			if (session == null)
			{
				return;
			}
			if (MessageBox.Show(string.Concat("Do you really want to remove ", session.mcName, " from the list?"), "EasyMC Client", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
			{
				this.activeSession = null;
				Sessions.removeSession(session.session);
				Accounts.removeActiveProfile();
				this.renderSessions();
			}
		}

		private void Client_Load(object sender, EventArgs e)
		{
			if (Consent.HasConsent())
			{
				this.initialize();
				return;
			}
			base.Hide();
			(new ConsentForm(this)).Show();
		}

		private void Client_Shown(object sender, EventArgs e)
		{
			if (Consent.HasConsent())
			{
				this.fetchSettings();
				return;
			}
			base.Hide();
		}

		private void Collapse()
		{
			base.Height = 170;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void easymcButton_Click(object sender, EventArgs e)
		{
			this.easymcButton.Text = "Loading";
			this.easymcButton.Enabled = false;
			string defaultRuntimeDir = Launcher.getDefaultRuntimeDir();
			if (defaultRuntimeDir != null && Launcher.getJres(defaultRuntimeDir).Count < 1)
			{
				defaultRuntimeDir = null;
			}
			if (defaultRuntimeDir == null)
			{
				defaultRuntimeDir = Launcher.getRuntimeDir();
				if (defaultRuntimeDir == null)
				{
					MessageBox.Show("Please select a valid Minecraft Launcher!\nEasyMC only works with the original Minecraft.", "EasyMC Client", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					this.UpdateForm();
					return;
				}
				if (defaultRuntimeDir == "abort")
				{
					this.UpdateForm();
					return;
				}
			}
			ArrayList jres = Launcher.getJres(defaultRuntimeDir);
			Certs.exportCerts();
			foreach (object jre in jres)
			{
				string runtime = Certs.importToRuntime((string)jre);
				if (runtime == null)
				{
					continue;
				}
				MessageBox.Show(string.Concat("Could not install certificates in runtime!\n", runtime), "EasyMC Client", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				this.UpdateForm();
				return;
			}
			string system = Certs.importToSystem();
			if (system != null)
			{
				MessageBox.Show(string.Concat("Could not install certificates in system!\n", system), "EasyMC Client", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				this.UpdateForm();
				return;
			}
			string str = Hosts.removeHosts();
			if (str != null)
			{
				MessageBox.Show(string.Concat("Could not remove hosts!\n", str), "EasyMC Client", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				this.UpdateForm();
				return;
			}
			str = Hosts.writeHosts(this.settings.authServer);
			if (str != null)
			{
				MessageBox.Show(string.Concat("Could not write hosts!\n", str), "EasyMC Client", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				this.UpdateForm();
				return;
			}
			this.UpdateForm();
		}

		private void Expand()
		{
			base.Height = 505;
		}

		public void fetchSettings()
		{
			Task.Run(() => {
				this.settings = new EasyMC.Settings(Config.API_URL);
				string str = this.settings.fetch();
				if (str != null)
				{
					base.Invoke(new MethodInvoker(() => {
						base.Hide();
						MessageBox.Show(string.Concat("Could not connect to EasyMC, please try again later!\n\n", str), "EasyMC Client", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						Application.Exit();
					}));
					return;
				}
				if (this.settings.version != Config.VERSION)
				{
					base.Invoke(new MethodInvoker(() => {
						base.Hide();
						if (MessageBox.Show("Please update the EasyMC Client.\nDo you want to update now?", "EasyMC Client", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
						{
							Process.Start("https://easymc.io/client");
						}
						Application.Exit();
					}));
					return;
				}
				ImageCache.HEAD_URL = this.settings.headUrl;
				ImageCache.RENDER_URL = this.settings.renderUrl;
				this.UpdateForm();
				
			});
		}

		public void initialize()
		{
			base.Show();
			this.Text = string.Concat("EasyMC Client - v", Config.VERSION);
			
			this.infoPanel.Hide();
			this.Collapse();
			if ((new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator))
			{
				return;
			}
			MessageBox.Show("Please run the EasyMC Client as administrator!", "EasyMC Client", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			Application.Exit();
		}

		private void InitializeComponent()
		{
            this.altsList = new System.Windows.Forms.FlowLayoutPanel();
            this.loginPanel = new System.Windows.Forms.Panel();
            this.loginButton = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.tokenInput = new System.Windows.Forms.TextBox();
            this.mojangButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.infoPanel = new System.Windows.Forms.Panel();
            this.refreshInfoLabel = new System.Windows.Forms.Label();
            this.refreshButton = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.activeButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tokenInfoInput = new System.Windows.Forms.TextBox();
            this.renderPictureBox = new System.Windows.Forms.PictureBox();
            this.mcNameLabel = new System.Windows.Forms.Label();
            this.playInfoLabel = new System.Windows.Forms.Label();
            this.redeemButton = new System.Windows.Forms.Button();
            this.easymcButton = new System.Windows.Forms.Button();
            this.loginPanel.SuspendLayout();
            this.infoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.renderPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // altsList
            // 
            this.altsList.AutoScroll = true;
            this.altsList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.altsList.Location = new System.Drawing.Point(13, 175);
            this.altsList.Name = "altsList";
            this.altsList.Size = new System.Drawing.Size(199, 276);
            this.altsList.TabIndex = 2;
            // 
            // loginPanel
            // 
            this.loginPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.loginPanel.Controls.Add(this.loginButton);
            this.loginPanel.Controls.Add(this.linkLabel1);
            this.loginPanel.Controls.Add(this.label1);
            this.loginPanel.Controls.Add(this.tokenInput);
            this.loginPanel.Location = new System.Drawing.Point(218, 136);
            this.loginPanel.Name = "loginPanel";
            this.loginPanel.Size = new System.Drawing.Size(407, 315);
            this.loginPanel.TabIndex = 3;
            // 
            // loginButton
            // 
            this.loginButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(130)))), ((int)(((byte)(108)))));
            this.loginButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(130)))), ((int)(((byte)(108)))));
            this.loginButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loginButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginButton.ForeColor = System.Drawing.Color.White;
            this.loginButton.Location = new System.Drawing.Point(143, 137);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(136, 41);
            this.loginButton.TabIndex = 5;
            this.loginButton.Text = "Redeem";
            this.loginButton.UseVisualStyleBackColor = false;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(130)))), ((int)(((byte)(108)))));
            this.linkLabel1.Location = new System.Drawing.Point(115, 279);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(208, 20);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Get Alt Token on EasyMC.io";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(139, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enter Alt Token";
            // 
            // tokenInput
            // 
            this.tokenInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tokenInput.Location = new System.Drawing.Point(48, 88);
            this.tokenInput.Name = "tokenInput";
            this.tokenInput.Size = new System.Drawing.Size(319, 29);
            this.tokenInput.TabIndex = 0;
            // 
            // mojangButton
            // 
            this.mojangButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(130)))), ((int)(((byte)(108)))));
            this.mojangButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(130)))), ((int)(((byte)(108)))));
            this.mojangButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mojangButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mojangButton.ForeColor = System.Drawing.Color.White;
            this.mojangButton.Location = new System.Drawing.Point(337, 59);
            this.mojangButton.Name = "mojangButton";
            this.mojangButton.Size = new System.Drawing.Size(303, 60);
            this.mojangButton.TabIndex = 4;
            this.mojangButton.Text = "Mojang";
            this.mojangButton.UseVisualStyleBackColor = false;
            this.mojangButton.Click += new System.EventHandler(this.mojangButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(140, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(368, 31);
            this.label3.TabIndex = 5;
            this.label3.Text = "Choose authentication server";
            // 
            // infoPanel
            // 
            this.infoPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.infoPanel.Controls.Add(this.refreshInfoLabel);
            this.infoPanel.Controls.Add(this.refreshButton);
            this.infoPanel.Controls.Add(this.buttonRemove);
            this.infoPanel.Controls.Add(this.statusLabel);
            this.infoPanel.Controls.Add(this.label5);
            this.infoPanel.Controls.Add(this.activeButton);
            this.infoPanel.Controls.Add(this.label2);
            this.infoPanel.Controls.Add(this.tokenInfoInput);
            this.infoPanel.Controls.Add(this.renderPictureBox);
            this.infoPanel.Controls.Add(this.mcNameLabel);
            this.infoPanel.Controls.Add(this.playInfoLabel);
            this.infoPanel.Location = new System.Drawing.Point(219, 136);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(407, 315);
            this.infoPanel.TabIndex = 6;
            // 
            // refreshInfoLabel
            // 
            this.refreshInfoLabel.AutoSize = true;
            this.refreshInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refreshInfoLabel.ForeColor = System.Drawing.Color.White;
            this.refreshInfoLabel.Location = new System.Drawing.Point(147, 208);
            this.refreshInfoLabel.Name = "refreshInfoLabel";
            this.refreshInfoLabel.Size = new System.Drawing.Size(251, 40);
            this.refreshInfoLabel.TabIndex = 14;
            this.refreshInfoLabel.Text = "Please renew the Account in order\r\nto play with it again.";
            this.refreshInfoLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.refreshInfoLabel.Visible = false;
            // 
            // refreshButton
            // 
            this.refreshButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(130)))), ((int)(((byte)(108)))));
            this.refreshButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(130)))), ((int)(((byte)(108)))));
            this.refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refreshButton.ForeColor = System.Drawing.Color.White;
            this.refreshButton.Location = new System.Drawing.Point(157, 166);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(226, 33);
            this.refreshButton.TabIndex = 13;
            this.refreshButton.Text = "Renew Account";
            this.refreshButton.UseVisualStyleBackColor = false;
            this.refreshButton.Visible = false;
            this.refreshButton.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonRemove.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRemove.ForeColor = System.Drawing.Color.White;
            this.buttonRemove.Location = new System.Drawing.Point(308, 69);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(75, 33);
            this.buttonRemove.TabIndex = 12;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = false;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.ForeColor = System.Drawing.Color.White;
            this.statusLabel.Location = new System.Drawing.Point(209, 82);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(15, 20);
            this.statusLabel.TabIndex = 11;
            this.statusLabel.Text = "-";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(153, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 20);
            this.label5.TabIndex = 10;
            this.label5.Text = "Status:";
            // 
            // activeButton
            // 
            this.activeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(130)))), ((int)(((byte)(108)))));
            this.activeButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(130)))), ((int)(((byte)(108)))));
            this.activeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.activeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.activeButton.ForeColor = System.Drawing.Color.White;
            this.activeButton.Location = new System.Drawing.Point(157, 262);
            this.activeButton.Name = "activeButton";
            this.activeButton.Size = new System.Drawing.Size(226, 33);
            this.activeButton.TabIndex = 7;
            this.activeButton.Text = "Login";
            this.activeButton.UseVisualStyleBackColor = false;
            this.activeButton.Visible = false;
            this.activeButton.Click += new System.EventHandler(this.activeButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(153, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "Alt-Token";
            // 
            // tokenInfoInput
            // 
            this.tokenInfoInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tokenInfoInput.Location = new System.Drawing.Point(157, 136);
            this.tokenInfoInput.Name = "tokenInfoInput";
            this.tokenInfoInput.ReadOnly = true;
            this.tokenInfoInput.Size = new System.Drawing.Size(226, 24);
            this.tokenInfoInput.TabIndex = 8;
            // 
            // renderPictureBox
            // 
            this.renderPictureBox.Location = new System.Drawing.Point(15, 25);
            this.renderPictureBox.Name = "renderPictureBox";
            this.renderPictureBox.Size = new System.Drawing.Size(120, 270);
            this.renderPictureBox.TabIndex = 7;
            this.renderPictureBox.TabStop = false;
            // 
            // mcNameLabel
            // 
            this.mcNameLabel.AutoSize = true;
            this.mcNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mcNameLabel.ForeColor = System.Drawing.Color.White;
            this.mcNameLabel.Location = new System.Drawing.Point(151, 27);
            this.mcNameLabel.Name = "mcNameLabel";
            this.mcNameLabel.Size = new System.Drawing.Size(78, 29);
            this.mcNameLabel.TabIndex = 6;
            this.mcNameLabel.Text = "Name";
            // 
            // playInfoLabel
            // 
            this.playInfoLabel.AutoSize = true;
            this.playInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playInfoLabel.ForeColor = System.Drawing.Color.White;
            this.playInfoLabel.Location = new System.Drawing.Point(166, 208);
            this.playInfoLabel.Name = "playInfoLabel";
            this.playInfoLabel.Size = new System.Drawing.Size(208, 40);
            this.playInfoLabel.TabIndex = 16;
            this.playInfoLabel.Text = "This Alt is set as active, start\r\nyour Minecraft to use it.";
            this.playInfoLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.playInfoLabel.Visible = false;
            // 
            // redeemButton
            // 
            this.redeemButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(130)))), ((int)(((byte)(108)))));
            this.redeemButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(130)))), ((int)(((byte)(108)))));
            this.redeemButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.redeemButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.redeemButton.ForeColor = System.Drawing.Color.White;
            this.redeemButton.Location = new System.Drawing.Point(13, 136);
            this.redeemButton.Name = "redeemButton";
            this.redeemButton.Size = new System.Drawing.Size(199, 33);
            this.redeemButton.TabIndex = 6;
            this.redeemButton.Text = "Redeem Token";
            this.redeemButton.UseVisualStyleBackColor = false;
            this.redeemButton.Click += new System.EventHandler(this.redeemButton_Click);
            // 
            // easymcButton
            // 
            this.easymcButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(130)))), ((int)(((byte)(108)))));
            this.easymcButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(130)))), ((int)(((byte)(108)))));
            this.easymcButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.easymcButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.easymcButton.ForeColor = System.Drawing.Color.White;
            this.easymcButton.Location = new System.Drawing.Point(13, 59);
            this.easymcButton.Name = "easymcButton";
            this.easymcButton.Size = new System.Drawing.Size(303, 60);
            this.easymcButton.TabIndex = 0;
            this.easymcButton.Text = "EasyMC";
            this.easymcButton.UseVisualStyleBackColor = false;
            this.easymcButton.Click += new System.EventHandler(this.easymcButton_Click);
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.ClientSize = new System.Drawing.Size(641, 466);
            this.Controls.Add(this.redeemButton);
            this.Controls.Add(this.easymcButton);
            this.Controls.Add(this.mojangButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.altsList);
            this.Controls.Add(this.loginPanel);
            this.Controls.Add(this.infoPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Client";
            this.Text = "EasyMC Client";
            this.Load += new System.EventHandler(this.Client_Load);
            this.Shown += new System.EventHandler(this.Client_Shown);
            this.loginPanel.ResumeLayout(false);
            this.loginPanel.PerformLayout();
            this.infoPanel.ResumeLayout(false);
            this.infoPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.renderPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		private void label4_Click(object sender, EventArgs e)
		{
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://easymc.io");
		}

		private void loginButton_Click(object sender, EventArgs e)
		{
			if (Sessions.getSessions().Count<Session>() >= 20)
			{
				MessageBox.Show("You can only have a maximum of 20 Accounts at the same time!", "EasyMC Client", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.tokenInput.TextLength != 20)
			{
				MessageBox.Show("The Alt-Token has to be 20 characters long!", "EasyMC Client", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			this.loginButton.Enabled = false;
			this.loginButton.Text = "Loading ...";
			this.loginButton.BackColor = Color.FromArgb(23, 71, 62);
			this.tokenInput.Enabled = false;
			string text = this.tokenInput.Text;
			dynamic obj = Api.RedeemToken(text);
			if (obj is string)
			{
				this.loginButton.Enabled = true;
				this.loginButton.Text = "Redeem";
				this.loginButton.BackColor = Color.FromArgb(21, 130, 108);
				this.tokenInput.Enabled = true;
				this.tokenInput.Text = "";
				MessageBox.Show(obj, "EasyMC Client", 0);
				return;
			}
			Dictionary<string, dynamic> strs = (Dictionary<string, object>)obj;
			if (!strs.ContainsKey("session") || !strs.ContainsKey("mcName") || !strs.ContainsKey("uuid"))
			{
				this.loginButton.Enabled = true;
				this.loginButton.Text = "Redeem";
				this.loginButton.BackColor = Color.FromArgb(21, 130, 108);
				this.tokenInput.Enabled = true;
				this.tokenInput.Text = "";
				MessageBox.Show("Could not login!", "EasyMC Client", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			Accounts.setActiveProfile(strs["session"], strs["mcName"], strs["uuid"], strs["userId"]);
			Sessions.addSession(strs["session"], strs["mcName"], strs["uuid"], strs["userId"], text);
			this.activeSession = (string)strs["session"];
			this.renderSessions();
			this.loginButton.Enabled = true;
			this.loginButton.Text = "Redeem";
			this.loginButton.BackColor = Color.FromArgb(21, 130, 108);
			this.tokenInput.Enabled = true;
			this.tokenInput.Text = "";
			MessageBox.Show("Successfully logged in as " + strs["mcName"] + "\n\nTo play with the account now, please restart your Minecraft.", "EasyMC Client", 0);
		}

		private void mojangButton_Click(object sender, EventArgs e)
		{
			this.mojangButton.Text = "Loading";
			this.mojangButton.Enabled = false;
			Accounts.removeActiveProfile();
			string str = Hosts.removeHosts();
			if (str == null)
			{
				this.UpdateForm();
				MessageBox.Show("Authentication server changed to Mojang!", "EasyMC Client", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}
			MessageBox.Show(string.Concat("Could not remove hosts!\n", str), "EasyMC Client", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			this.UpdateForm();
		}

		private void redeemButton_Click(object sender, EventArgs e)
		{
			this.activeSession = null;
			this.renderSessions();
		}

		private void renderSessions()
		{
			base.Invoke(new MethodInvoker(() => {
				List<Session> sessions = Sessions.getSessions();
				if (sessions.Count < 1)
				{
					Label label = new Label()
					{
						Text = "No alts added",
						ForeColor = Color.White,
						Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, FontStyle.Italic)
					};
					this.loginPanel.Show();
					this.infoPanel.Hide();
					this.redeemButton.Enabled = false;
					this.redeemButton.BackColor = Color.FromArgb(23, 71, 62);
					this.altsList.Controls.Clear();
					this.altsList.Controls.Add(label);
					return;
				}
				int value = this.altsList.VerticalScroll.Value;
				this.altsList.Controls.Clear();
				this.altsList.VerticalScroll.Visible = false;
				string activeProfile = Accounts.getActiveProfile();
				Dictionary<string, bool> status = SessionStatus.getStatus((
					from sess in sessions
					select sess.session).ToList<string>());
				Dictionary<string, Control> strs = new Dictionary<string, Control>();
				foreach (Session session in sessions)
				{
					Panel panel = new Panel();
					if (session.session != this.activeSession)
					{
						panel.BackColor = Color.FromArgb(32, 32, 32);
					}
					else
					{
						panel.BackColor = Color.FromArgb(50, 50, 50);
					}
					if (sessions.Count <= 5)
					{
						panel.Width = 193;
					}
					else
					{
						panel.Width = 175;
					}
					panel.Height = 45;
					panel.BorderStyle = BorderStyle.FixedSingle;
					panel.Cursor = Cursors.Hand;
					panel.Click += new EventHandler(this.alt_Click);
					panel.Tag = session.session;
					PictureBox pictureBox = new PictureBox()
					{
						Width = 32,
						Height = 32,
						Location = new Point(8, 6)
					};
					pictureBox.Click += new EventHandler(this.alt_Click);
					pictureBox.Tag = session.session;
					pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
					Task.Run(() => {
						Bitmap head = ImageCache.getHead(session.uuid);
						base.Invoke(new MethodInvoker(() => pictureBox.Image = head));
					});
					Label label1 = new Label()
					{
						Location = new Point(45, 14),
						Text = session.mcName,
						ForeColor = Color.White,
						Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, FontStyle.Regular)
					};
					label1.Click += new EventHandler(this.alt_Click);
					label1.Tag = session.session;
					if (activeProfile != null && activeProfile == session.session)
					{
						PictureBox point = new PictureBox()
						{
							Width = 26,
							Height = 26,
							Image = Resources.check,
							SizeMode = PictureBoxSizeMode.StretchImage
						};
						point.Click += new EventHandler(this.alt_Click);
						point.Tag = session.session;
						point.Location = new Point(panel.Width - point.Width - 12, 8);
						panel.Controls.Add(point);
					}
					panel.Controls.Add(label1);
					panel.Controls.Add(pictureBox);
					strs[session.session] = panel;
				}
				if (activeProfile != null && strs.ContainsKey(activeProfile))
				{
					this.altsList.Controls.Add(strs[activeProfile]);
					strs.Remove(activeProfile);
				}
				foreach (KeyValuePair<string, Control> str in strs)
				{
					this.altsList.Controls.Add(str.Value);
				}
				this.altsList.VerticalScroll.Value = value;
				this.altsList.PerformLayout();
				Session session1 = (this.activeSession == null ? null : sessions.Single<Session>((Session sess) => sess.session == this.activeSession));
				if (session1 == null)
				{
					this.loginPanel.Show();
					this.infoPanel.Hide();
					this.redeemButton.Enabled = false;
					this.redeemButton.BackColor = Color.FromArgb(23, 71, 62);
					return;
				}
				this.loginPanel.Hide();
				this.infoPanel.Show();
				this.redeemButton.Enabled = true;
				this.redeemButton.BackColor = Color.FromArgb(21, 130, 108);
				this.mcNameLabel.Text = session1.mcName;
				this.tokenInfoInput.Text = session1.token;
				this.buttonRemove.Tag = session1.session;
				this.activeButton.Tag = session1.session;
				this.refreshInfoLabel.Hide();
				this.playInfoLabel.Hide();
				this.activeButton.Hide();
				this.refreshButton.Hide();
				if (!status.ContainsKey(session1.session))
				{
					this.statusLabel.Text = "-";
					this.statusLabel.ForeColor = Color.White;
				}
				else if (status[session1.session])
				{
					this.statusLabel.Text = "Expired";
					this.statusLabel.ForeColor = Color.DarkOrange;
					this.refreshInfoLabel.Show();
					this.refreshButton.Show();
				}
				else if (activeProfile != session1.session)
				{
					this.activeButton.Show();
					this.statusLabel.Text = "Valid";
					this.statusLabel.ForeColor = Color.DarkGreen;
				}
				else
				{
					this.playInfoLabel.Show();
					this.statusLabel.Text = "Active";
					this.statusLabel.ForeColor = Color.DarkGreen;
				}
				Task.Run(() => {
					Bitmap render = ImageCache.getRender(session1.uuid);
					base.Invoke(new MethodInvoker(() => this.renderPictureBox.Image = render));
				});
			}));
		}

		private void UpdateForm()
		{
			if (!Hosts.hasHosts())
			{
				base.Invoke(new MethodInvoker(() => {
					
					this.easymcButton.Text = "EasyMC";
					this.mojangButton.Text = "Mojang";
					this.easymcButton.Enabled = true;
					this.mojangButton.Enabled = false;
					this.easymcButton.BackColor = Color.FromArgb(21, 130, 108);
					this.easymcButton.FlatAppearance.BorderColor = Color.FromArgb(21, 130, 108);
					this.easymcButton.FlatAppearance.BorderSize = 1;
					this.mojangButton.BackColor = Color.FromArgb(23, 71, 62);
					this.mojangButton.FlatAppearance.BorderColor = Color.FromArgb(21, 130, 50);
					this.mojangButton.FlatAppearance.BorderSize = 4;
					this.Collapse();
				}));
				return;
			}
			if (!Hosts.matchesHosts(this.settings.authServer))
			{
				Hosts.removeHosts();
				Hosts.writeHosts(this.settings.authServer);
			}
			base.Invoke(new MethodInvoker(() => {
				
				this.easymcButton.Text = "EasyMC";
				this.mojangButton.Text = "Mojang";
				this.easymcButton.Enabled = false;
				this.mojangButton.Enabled = true;
				this.easymcButton.BackColor = Color.FromArgb(23, 71, 62);
				this.easymcButton.FlatAppearance.BorderColor = Color.FromArgb(21, 130, 50);
				this.easymcButton.FlatAppearance.BorderSize = 4;
				this.mojangButton.BackColor = Color.FromArgb(21, 130, 108);
				this.mojangButton.FlatAppearance.BorderColor = Color.FromArgb(21, 130, 108);
				this.mojangButton.FlatAppearance.BorderSize = 1;
				this.renderSessions();
				this.Expand();
			}));
		}
	}
}