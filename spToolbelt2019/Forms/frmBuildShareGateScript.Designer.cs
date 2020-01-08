namespace spToolbelt2019.Forms
{
    partial class frmBuildShareGateScript
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
            this.cmdProcessLists = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpSites = new System.Windows.Forms.TabPage();
            this.chkSelectAllSites = new System.Windows.Forms.CheckBox();
            this.cmdSelectSiteTemplate = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSiteOutput = new System.Windows.Forms.TextBox();
            this.cmdProcessSites = new System.Windows.Forms.Button();
            this.txtSiteTemplate = new System.Windows.Forms.TextBox();
            this.lvSites = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpLists = new System.Windows.Forms.TabPage();
            this.chkSelectAllLists = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTemplate = new System.Windows.Forms.Label();
            this.txtListOutput = new System.Windows.Forms.TextBox();
            this.txtListTemplate = new System.Windows.Forms.TextBox();
            this.lvLists = new System.Windows.Forms.ListView();
            this.chUrl = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chTargetAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chTargetLocation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tpSites.SuspendLayout();
            this.tpLists.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Working Folder";
            // 
            // cmdProcessLists
            // 
            this.cmdProcessLists.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdProcessLists.Location = new System.Drawing.Point(533, 145);
            this.cmdProcessLists.Name = "cmdProcessLists";
            this.cmdProcessLists.Size = new System.Drawing.Size(75, 23);
            this.cmdProcessLists.TabIndex = 6;
            this.cmdProcessLists.Text = "Process";
            this.cmdProcessLists.UseVisualStyleBackColor = true;
            this.cmdProcessLists.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tpSites);
            this.tabControl1.Controls.Add(this.tpLists);
            this.tabControl1.Location = new System.Drawing.Point(15, 39);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(619, 200);
            this.tabControl1.TabIndex = 7;
            // 
            // tpSites
            // 
            this.tpSites.Controls.Add(this.chkSelectAllSites);
            this.tpSites.Controls.Add(this.cmdSelectSiteTemplate);
            this.tpSites.Controls.Add(this.label3);
            this.tpSites.Controls.Add(this.label5);
            this.tpSites.Controls.Add(this.txtSiteOutput);
            this.tpSites.Controls.Add(this.cmdProcessSites);
            this.tpSites.Controls.Add(this.txtSiteTemplate);
            this.tpSites.Controls.Add(this.lvSites);
            this.tpSites.Location = new System.Drawing.Point(4, 22);
            this.tpSites.Name = "tpSites";
            this.tpSites.Padding = new System.Windows.Forms.Padding(3);
            this.tpSites.Size = new System.Drawing.Size(611, 174);
            this.tpSites.TabIndex = 0;
            this.tpSites.Text = "Sites";
            this.tpSites.UseVisualStyleBackColor = true;
            // 
            // chkSelectAllSites
            // 
            this.chkSelectAllSites.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkSelectAllSites.AutoSize = true;
            this.chkSelectAllSites.Location = new System.Drawing.Point(7, 127);
            this.chkSelectAllSites.Name = "chkSelectAllSites";
            this.chkSelectAllSites.Size = new System.Drawing.Size(70, 17);
            this.chkSelectAllSites.TabIndex = 17;
            this.chkSelectAllSites.Text = "Select All";
            this.chkSelectAllSites.UseVisualStyleBackColor = true;
            this.chkSelectAllSites.CheckedChanged += new System.EventHandler(this.chkSelectAllSites_CheckedChanged);
            // 
            // cmdSelectSiteTemplate
            // 
            this.cmdSelectSiteTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdSelectSiteTemplate.Location = new System.Drawing.Point(530, 122);
            this.cmdSelectSiteTemplate.Name = "cmdSelectSiteTemplate";
            this.cmdSelectSiteTemplate.Size = new System.Drawing.Size(75, 23);
            this.cmdSelectSiteTemplate.TabIndex = 16;
            this.cmdSelectSiteTemplate.Text = "Select";
            this.cmdSelectSiteTemplate.UseVisualStyleBackColor = true;
            this.cmdSelectSiteTemplate.Click += new System.EventHandler(this.cmdSelectSiteTemplate_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(271, 151);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Output";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(268, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Template";
            // 
            // txtSiteOutput
            // 
            this.txtSiteOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtSiteOutput.Location = new System.Drawing.Point(328, 147);
            this.txtSiteOutput.Name = "txtSiteOutput";
            this.txtSiteOutput.Size = new System.Drawing.Size(199, 20);
            this.txtSiteOutput.TabIndex = 12;
            // 
            // cmdProcessSites
            // 
            this.cmdProcessSites.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdProcessSites.Location = new System.Drawing.Point(530, 146);
            this.cmdProcessSites.Name = "cmdProcessSites";
            this.cmdProcessSites.Size = new System.Drawing.Size(75, 23);
            this.cmdProcessSites.TabIndex = 13;
            this.cmdProcessSites.Text = "Process";
            this.cmdProcessSites.UseVisualStyleBackColor = true;
            this.cmdProcessSites.Click += new System.EventHandler(this.cmdProcessSites_Click);
            // 
            // txtSiteTemplate
            // 
            this.txtSiteTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtSiteTemplate.Location = new System.Drawing.Point(331, 124);
            this.txtSiteTemplate.Name = "txtSiteTemplate";
            this.txtSiteTemplate.Size = new System.Drawing.Size(196, 20);
            this.txtSiteTemplate.TabIndex = 11;
            // 
            // lvSites
            // 
            this.lvSites.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSites.CheckBoxes = true;
            this.lvSites.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.lvSites.Location = new System.Drawing.Point(6, 9);
            this.lvSites.Name = "lvSites";
            this.lvSites.Size = new System.Drawing.Size(599, 106);
            this.lvSites.TabIndex = 10;
            this.lvSites.UseCompatibleStateImageBehavior = false;
            this.lvSites.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Url";
            this.columnHeader1.Width = 500;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Title";
            this.columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Target Action";
            this.columnHeader3.Width = 200;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Target Location";
            this.columnHeader4.Width = 200;
            // 
            // tpLists
            // 
            this.tpLists.Controls.Add(this.chkSelectAllLists);
            this.tpLists.Controls.Add(this.button1);
            this.tpLists.Controls.Add(this.label4);
            this.tpLists.Controls.Add(this.lblTemplate);
            this.tpLists.Controls.Add(this.txtListOutput);
            this.tpLists.Controls.Add(this.cmdProcessLists);
            this.tpLists.Controls.Add(this.txtListTemplate);
            this.tpLists.Controls.Add(this.lvLists);
            this.tpLists.Location = new System.Drawing.Point(4, 22);
            this.tpLists.Name = "tpLists";
            this.tpLists.Padding = new System.Windows.Forms.Padding(3);
            this.tpLists.Size = new System.Drawing.Size(611, 174);
            this.tpLists.TabIndex = 1;
            this.tpLists.Text = "Lists";
            this.tpLists.UseVisualStyleBackColor = true;
            // 
            // chkSelectAllLists
            // 
            this.chkSelectAllLists.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkSelectAllLists.AutoSize = true;
            this.chkSelectAllLists.Location = new System.Drawing.Point(9, 119);
            this.chkSelectAllLists.Name = "chkSelectAllLists";
            this.chkSelectAllLists.Size = new System.Drawing.Size(70, 17);
            this.chkSelectAllLists.TabIndex = 11;
            this.chkSelectAllLists.Text = "Select All";
            this.chkSelectAllLists.UseVisualStyleBackColor = true;
            this.chkSelectAllLists.CheckedChanged += new System.EventHandler(this.chkSelectAllLists_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(533, 116);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 21);
            this.button1.TabIndex = 10;
            this.button1.Text = "Select";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(274, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Output";
            // 
            // lblTemplate
            // 
            this.lblTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTemplate.AutoSize = true;
            this.lblTemplate.Location = new System.Drawing.Point(271, 120);
            this.lblTemplate.Name = "lblTemplate";
            this.lblTemplate.Size = new System.Drawing.Size(51, 13);
            this.lblTemplate.TabIndex = 8;
            this.lblTemplate.Text = "Template";
            // 
            // txtListOutput
            // 
            this.txtListOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtListOutput.Location = new System.Drawing.Point(331, 145);
            this.txtListOutput.Name = "txtListOutput";
            this.txtListOutput.Size = new System.Drawing.Size(199, 20);
            this.txtListOutput.TabIndex = 2;
            this.txtListOutput.Text = "Output.ps1";
            // 
            // txtListTemplate
            // 
            this.txtListTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtListTemplate.Location = new System.Drawing.Point(328, 119);
            this.txtListTemplate.Name = "txtListTemplate";
            this.txtListTemplate.Size = new System.Drawing.Size(199, 20);
            this.txtListTemplate.TabIndex = 1;
            // 
            // lvLists
            // 
            this.lvLists.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvLists.CheckBoxes = true;
            this.lvLists.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chUrl,
            this.chTitle,
            this.chTargetAction,
            this.chTargetLocation});
            this.lvLists.Location = new System.Drawing.Point(9, 6);
            this.lvLists.Name = "lvLists";
            this.lvLists.Size = new System.Drawing.Size(599, 104);
            this.lvLists.TabIndex = 0;
            this.lvLists.UseCompatibleStateImageBehavior = false;
            this.lvLists.View = System.Windows.Forms.View.Details;
            // 
            // chUrl
            // 
            this.chUrl.Text = "Url";
            this.chUrl.Width = 500;
            // 
            // chTitle
            // 
            this.chTitle.Text = "Title";
            this.chTitle.Width = 200;
            // 
            // chTargetAction
            // 
            this.chTargetAction.Text = "Target Action";
            this.chTargetAction.Width = 200;
            // 
            // chTargetLocation
            // 
            this.chTargetLocation.Text = "Target Location";
            this.chTargetLocation.Width = 200;
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(101, 13);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(320, 20);
            this.txtOutputFolder.TabIndex = 2;
            this.txtOutputFolder.Text = "C:\\temp\\Migrate";
            // 
            // frmBuildShareGateScript
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 251);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOutputFolder);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmBuildShareGateScript";
            this.Text = "Build Inventory Script";
            this.Load += new System.EventHandler(this.frmBuildShareGateScript_Load);
            this.tabControl1.ResumeLayout(false);
            this.tpSites.ResumeLayout(false);
            this.tpSites.PerformLayout();
            this.tpLists.ResumeLayout(false);
            this.tpLists.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdProcessLists;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpSites;
        private System.Windows.Forms.TabPage tpLists;
        private System.Windows.Forms.TextBox txtListOutput;
        private System.Windows.Forms.TextBox txtListTemplate;
        private System.Windows.Forms.ListView lvLists;
        private System.Windows.Forms.ColumnHeader chUrl;
        private System.Windows.Forms.ColumnHeader chTitle;
        private System.Windows.Forms.ColumnHeader chTargetAction;
        private System.Windows.Forms.ColumnHeader chTargetLocation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSiteOutput;
        private System.Windows.Forms.Button cmdProcessSites;
        private System.Windows.Forms.TextBox txtSiteTemplate;
        private System.Windows.Forms.ListView lvSites;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTemplate;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button cmdSelectSiteTemplate;
        private System.Windows.Forms.CheckBox chkSelectAllSites;
        private System.Windows.Forms.CheckBox chkSelectAllLists;
    }
}