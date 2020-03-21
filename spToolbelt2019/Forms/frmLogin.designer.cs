namespace spToolbelt2019
{
    partial class frmLogin
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
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.cboSites = new System.Windows.Forms.ComboBox();
            this.cmdLogin = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbAuthBasic = new System.Windows.Forms.RadioButton();
            this.rbAuthIntegrated = new System.Windows.Forms.RadioButton();
            this.rbAuthSPO = new System.Windows.Forms.RadioButton();
            this.rbAuthPNP = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Web Site";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 100);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Domain";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 71);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Password";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 45);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Username";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(86, 44);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(383, 20);
            this.txtUserName.TabIndex = 6;
            this.txtUserName.TextChanged += new System.EventHandler(this.TxtUserName_TextChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(86, 68);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(383, 20);
            this.txtPassword.TabIndex = 7;
            // 
            // txtDomain
            // 
            this.txtDomain.Location = new System.Drawing.Point(86, 98);
            this.txtDomain.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.Size = new System.Drawing.Size(254, 20);
            this.txtDomain.TabIndex = 8;
            // 
            // cboSites
            // 
            this.cboSites.FormattingEnabled = true;
            this.cboSites.Location = new System.Drawing.Point(86, 18);
            this.cboSites.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cboSites.Name = "cboSites";
            this.cboSites.Size = new System.Drawing.Size(383, 21);
            this.cboSites.TabIndex = 9;
            this.cboSites.Text = "https://clients.rammware.net/sites/sg";
            // 
            // cmdLogin
            // 
            this.cmdLogin.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdLogin.Location = new System.Drawing.Point(272, 119);
            this.cmdLogin.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmdLogin.Name = "cmdLogin";
            this.cmdLogin.Size = new System.Drawing.Size(98, 29);
            this.cmdLogin.TabIndex = 10;
            this.cmdLogin.Text = "Login";
            this.cmdLogin.UseVisualStyleBackColor = true;
            this.cmdLogin.Click += new System.EventHandler(this.cmdLogin_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(374, 119);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(98, 29);
            this.cmdCancel.TabIndex = 11;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbAuthPNP);
            this.groupBox1.Controls.Add(this.rbAuthSPO);
            this.groupBox1.Controls.Add(this.rbAuthIntegrated);
            this.groupBox1.Controls.Add(this.rbAuthBasic);
            this.groupBox1.Location = new System.Drawing.Point(31, 123);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(236, 63);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Authentication Mode";
            // 
            // rbAuthBasic
            // 
            this.rbAuthBasic.AutoSize = true;
            this.rbAuthBasic.Location = new System.Drawing.Point(15, 20);
            this.rbAuthBasic.Name = "rbAuthBasic";
            this.rbAuthBasic.Size = new System.Drawing.Size(51, 17);
            this.rbAuthBasic.TabIndex = 0;
            this.rbAuthBasic.TabStop = true;
            this.rbAuthBasic.Text = "Basic";
            this.rbAuthBasic.UseVisualStyleBackColor = true;
            // 
            // rbAuthIntegrated
            // 
            this.rbAuthIntegrated.AutoSize = true;
            this.rbAuthIntegrated.Location = new System.Drawing.Point(15, 37);
            this.rbAuthIntegrated.Name = "rbAuthIntegrated";
            this.rbAuthIntegrated.Size = new System.Drawing.Size(73, 17);
            this.rbAuthIntegrated.TabIndex = 1;
            this.rbAuthIntegrated.TabStop = true;
            this.rbAuthIntegrated.Text = "Integrated";
            this.rbAuthIntegrated.UseVisualStyleBackColor = true;
            // 
            // rbAuthSPO
            // 
            this.rbAuthSPO.AutoSize = true;
            this.rbAuthSPO.Location = new System.Drawing.Point(121, 19);
            this.rbAuthSPO.Name = "rbAuthSPO";
            this.rbAuthSPO.Size = new System.Drawing.Size(110, 17);
            this.rbAuthSPO.TabIndex = 2;
            this.rbAuthSPO.TabStop = true;
            this.rbAuthSPO.Text = "SharePoint Online";
            this.rbAuthSPO.UseVisualStyleBackColor = true;
            // 
            // rbAuthPNP
            // 
            this.rbAuthPNP.AutoSize = true;
            this.rbAuthPNP.Location = new System.Drawing.Point(121, 36);
            this.rbAuthPNP.Name = "rbAuthPNP";
            this.rbAuthPNP.Size = new System.Drawing.Size(47, 17);
            this.rbAuthPNP.TabIndex = 3;
            this.rbAuthPNP.TabStop = true;
            this.rbAuthPNP.Text = "PNP";
            this.rbAuthPNP.UseVisualStyleBackColor = true;
            // 
            // frmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 191);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdLogin);
            this.Controls.Add(this.cboSites);
            this.Controls.Add(this.txtDomain);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "frmLogin";
            this.Text = "Login";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmLogin_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.ComboBox cboSites;
        private System.Windows.Forms.Button cmdLogin;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbAuthPNP;
        private System.Windows.Forms.RadioButton rbAuthSPO;
        private System.Windows.Forms.RadioButton rbAuthIntegrated;
        private System.Windows.Forms.RadioButton rbAuthBasic;
    }
}