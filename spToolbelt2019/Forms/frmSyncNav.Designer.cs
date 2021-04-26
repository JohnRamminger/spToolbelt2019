namespace spToolbelt2019.Forms
{
    partial class frmSyncNav
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
            this.txtSourceSite = new System.Windows.Forms.TextBox();
            this.txtTargetSite = new System.Windows.Forms.TextBox();
            this.S = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // txtSourceSite
            // 
            this.txtSourceSite.Location = new System.Drawing.Point(184, 29);
            this.txtSourceSite.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtSourceSite.Name = "txtSourceSite";
            this.txtSourceSite.Size = new System.Drawing.Size(1106, 31);
            this.txtSourceSite.TabIndex = 0;
            this.txtSourceSite.Text = "https://internal.ati.org/cfg";
            // 
            // txtTargetSite
            // 
            this.txtTargetSite.Location = new System.Drawing.Point(184, 98);
            this.txtTargetSite.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtTargetSite.Name = "txtTargetSite";
            this.txtTargetSite.Size = new System.Drawing.Size(1106, 31);
            this.txtTargetSite.TabIndex = 1;
            this.txtTargetSite.Text = "https://contracts.ati.org/sites/testnav";
            // 
            // S
            // 
            this.S.AutoSize = true;
            this.S.Location = new System.Drawing.Point(26, 35);
            this.S.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.S.Name = "S";
            this.S.Size = new System.Drawing.Size(123, 25);
            this.S.TabIndex = 2;
            this.S.Text = "Soruce Site";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 98);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Target Site";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1332, 25);
            this.button1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(224, 112);
            this.button1.TabIndex = 4;
            this.button1.Text = "Sync Navigation";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 25;
            this.listBox1.Location = new System.Drawing.Point(32, 162);
            this.listBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(1520, 679);
            this.listBox1.TabIndex = 5;
            // 
            // frmSyncNav
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1600, 865);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.S);
            this.Controls.Add(this.txtTargetSite);
            this.Controls.Add(this.txtSourceSite);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "frmSyncNav";
            this.Text = "frmSyncNav";
            this.Load += new System.EventHandler(this.frmSyncNav_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSourceSite;
        private System.Windows.Forms.TextBox txtTargetSite;
        private System.Windows.Forms.Label S;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
    }
}