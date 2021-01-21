namespace spToolbelt2019.Forms
{
    partial class frmUpdateApp
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
            this.lblCurrentVersion = new System.Windows.Forms.Label();
            this.lblNewVersion = new System.Windows.Forms.Label();
            this.cmdUpdate = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblCurrentVersion
            // 
            this.lblCurrentVersion.AutoSize = true;
            this.lblCurrentVersion.Location = new System.Drawing.Point(166, 23);
            this.lblCurrentVersion.Name = "lblCurrentVersion";
            this.lblCurrentVersion.Size = new System.Drawing.Size(0, 13);
            this.lblCurrentVersion.TabIndex = 2;
            // 
            // lblNewVersion
            // 
            this.lblNewVersion.AutoSize = true;
            this.lblNewVersion.Location = new System.Drawing.Point(166, 70);
            this.lblNewVersion.Name = "lblNewVersion";
            this.lblNewVersion.Size = new System.Drawing.Size(0, 13);
            this.lblNewVersion.TabIndex = 3;
            // 
            // cmdUpdate
            // 
            this.cmdUpdate.Location = new System.Drawing.Point(233, 12);
            this.cmdUpdate.Name = "cmdUpdate";
            this.cmdUpdate.Size = new System.Drawing.Size(75, 23);
            this.cmdUpdate.TabIndex = 4;
            this.cmdUpdate.Text = "Update";
            this.cmdUpdate.UseVisualStyleBackColor = true;
            this.cmdUpdate.Click += new System.EventHandler(this.cmdUpdate_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(233, 59);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Releases";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(215, 133);
            this.textBox1.TabIndex = 6;
            // 
            // frmUpdateApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 157);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmdUpdate);
            this.Controls.Add(this.lblNewVersion);
            this.Controls.Add(this.lblCurrentVersion);
            this.Name = "frmUpdateApp";
            this.Text = "Update App";
            this.Load += new System.EventHandler(this.frmUpdateApp_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblCurrentVersion;
        private System.Windows.Forms.Label lblNewVersion;
        private System.Windows.Forms.Button cmdUpdate;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
    }
}