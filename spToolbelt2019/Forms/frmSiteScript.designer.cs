namespace spToolbelt2019
{
    partial class frmSiteScript
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
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtScript = new System.Windows.Forms.TextBox();
            this.txtCommands = new System.Windows.Forms.TextBox();
            this.chkProcessSubWebs = new System.Windows.Forms.CheckBox();
            this.chkAllSites = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblRunCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblRemaining = new System.Windows.Forms.ToolStripStatusLabel();
            this.chkTestMode = new System.Windows.Forms.CheckBox();
            this.cmdRunScript = new System.Windows.Forms.Button();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.cmdLoadScipt = new System.Windows.Forms.Button();
            this.chkSingleSite = new System.Windows.Forms.CheckBox();
            this.progressMain = new System.Windows.Forms.ToolStripProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Margin = new System.Windows.Forms.Padding(2);
            this.splitMain.Name = "splitMain";
            this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.chkProcessSubWebs);
            this.splitMain.Panel2.Controls.Add(this.chkAllSites);
            this.splitMain.Panel2.Controls.Add(this.statusStrip1);
            this.splitMain.Panel2.Controls.Add(this.chkTestMode);
            this.splitMain.Panel2.Controls.Add(this.cmdRunScript);
            this.splitMain.Panel2.Controls.Add(this.txtResults);
            this.splitMain.Panel2.Controls.Add(this.cmdLoadScipt);
            this.splitMain.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitMain_Panel2_Paint);
            this.splitMain.Size = new System.Drawing.Size(815, 417);
            this.splitMain.SplitterDistance = 98;
            this.splitMain.SplitterWidth = 3;
            this.splitMain.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtScript);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtCommands);
            this.splitContainer1.Size = new System.Drawing.Size(815, 98);
            this.splitContainer1.SplitterDistance = 271;
            this.splitContainer1.TabIndex = 1;
            // 
            // txtScript
            // 
            this.txtScript.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtScript.Location = new System.Drawing.Point(0, 0);
            this.txtScript.Margin = new System.Windows.Forms.Padding(2);
            this.txtScript.Multiline = true;
            this.txtScript.Name = "txtScript";
            this.txtScript.Size = new System.Drawing.Size(271, 98);
            this.txtScript.TabIndex = 0;
            // 
            // txtCommands
            // 
            this.txtCommands.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCommands.Location = new System.Drawing.Point(0, 0);
            this.txtCommands.Margin = new System.Windows.Forms.Padding(2);
            this.txtCommands.Multiline = true;
            this.txtCommands.Name = "txtCommands";
            this.txtCommands.Size = new System.Drawing.Size(540, 98);
            this.txtCommands.TabIndex = 2;
            // 
            // chkProcessSubWebs
            // 
            this.chkProcessSubWebs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkProcessSubWebs.AutoSize = true;
            this.chkProcessSubWebs.Location = new System.Drawing.Point(342, 276);
            this.chkProcessSubWebs.Margin = new System.Windows.Forms.Padding(2);
            this.chkProcessSubWebs.Name = "chkProcessSubWebs";
            this.chkProcessSubWebs.Size = new System.Drawing.Size(117, 17);
            this.chkProcessSubWebs.TabIndex = 9;
            this.chkProcessSubWebs.Text = "Processs Sub Sites";
            this.chkProcessSubWebs.UseVisualStyleBackColor = true;
            // 
            // chkAllSites
            // 
            this.chkAllSites.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkAllSites.AutoSize = true;
            this.chkAllSites.Location = new System.Drawing.Point(241, 275);
            this.chkAllSites.Margin = new System.Windows.Forms.Padding(2);
            this.chkAllSites.Name = "chkAllSites";
            this.chkAllSites.Size = new System.Drawing.Size(63, 17);
            this.chkAllSites.TabIndex = 8;
            this.chkAllSites.Text = "All Sites";
            this.chkAllSites.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblRemaining,
            this.lblRunCount,
            this.progressMain,
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 294);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(815, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 17);
            this.lblStatus.Text = "Status";
            this.lblStatus.Click += new System.EventHandler(this.lblStatus_Click);
            // 
            // lblRunCount
            // 
            this.lblRunCount.Name = "lblRunCount";
            this.lblRunCount.Size = new System.Drawing.Size(13, 17);
            this.lblRunCount.Text = "0";
            // 
            // lblRemaining
            // 
            this.lblRemaining.Name = "lblRemaining";
            this.lblRemaining.Size = new System.Drawing.Size(49, 17);
            this.lblRemaining.Text = "00:00:00";
            // 
            // chkTestMode
            // 
            this.chkTestMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkTestMode.AutoSize = true;
            this.chkTestMode.Location = new System.Drawing.Point(129, 275);
            this.chkTestMode.Margin = new System.Windows.Forms.Padding(2);
            this.chkTestMode.Name = "chkTestMode";
            this.chkTestMode.Size = new System.Drawing.Size(83, 17);
            this.chkTestMode.TabIndex = 6;
            this.chkTestMode.Text = "Test Mode?";
            this.chkTestMode.UseVisualStyleBackColor = true;
            // 
            // cmdRunScript
            // 
            this.cmdRunScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdRunScript.Location = new System.Drawing.Point(701, 266);
            this.cmdRunScript.Margin = new System.Windows.Forms.Padding(2);
            this.cmdRunScript.Name = "cmdRunScript";
            this.cmdRunScript.Size = new System.Drawing.Size(103, 30);
            this.cmdRunScript.TabIndex = 4;
            this.cmdRunScript.Text = "Run Script";
            this.cmdRunScript.UseVisualStyleBackColor = true;
            this.cmdRunScript.Click += new System.EventHandler(this.cmdRunScript_Click);
            // 
            // txtResults
            // 
            this.txtResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResults.Location = new System.Drawing.Point(5, 5);
            this.txtResults.Margin = new System.Windows.Forms.Padding(2);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResults.Size = new System.Drawing.Size(803, 257);
            this.txtResults.TabIndex = 3;
            // 
            // cmdLoadScipt
            // 
            this.cmdLoadScipt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdLoadScipt.Location = new System.Drawing.Point(5, 266);
            this.cmdLoadScipt.Margin = new System.Windows.Forms.Padding(2);
            this.cmdLoadScipt.Name = "cmdLoadScipt";
            this.cmdLoadScipt.Size = new System.Drawing.Size(103, 30);
            this.cmdLoadScipt.TabIndex = 2;
            this.cmdLoadScipt.Text = "Load Script";
            this.cmdLoadScipt.UseVisualStyleBackColor = true;
            this.cmdLoadScipt.Click += new System.EventHandler(this.cmdLoadScipt_Click);
            // 
            // chkSingleSite
            // 
            this.chkSingleSite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkSingleSite.AutoSize = true;
            this.chkSingleSite.Location = new System.Drawing.Point(241, 269);
            this.chkSingleSite.Margin = new System.Windows.Forms.Padding(2);
            this.chkSingleSite.Name = "chkSingleSite";
            this.chkSingleSite.Size = new System.Drawing.Size(112, 21);
            this.chkSingleSite.TabIndex = 8;
            this.chkSingleSite.Text = "Current Site Only";
            this.chkSingleSite.UseVisualStyleBackColor = true;
            // 
            // progressMain
            // 
            this.progressMain.Name = "progressMain";
            this.progressMain.Size = new System.Drawing.Size(100, 16);
            // 
            // frmSiteScript
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 417);
            this.Controls.Add(this.splitMain);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmSiteScript";
            this.Text = "Site Script";
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.TextBox txtScript;
        private System.Windows.Forms.CheckBox chkTestMode;
        private System.Windows.Forms.Button cmdRunScript;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.Button cmdLoadScipt;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtCommands;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.CheckBox chkAllSites;
        private System.Windows.Forms.CheckBox chkSingleSite;
        private System.Windows.Forms.CheckBox chkProcessSubWebs;
        private System.Windows.Forms.ToolStripStatusLabel lblRunCount;
        private System.Windows.Forms.ToolStripStatusLabel lblRemaining;
        private System.Windows.Forms.ToolStripProgressBar progressMain;
    }
}