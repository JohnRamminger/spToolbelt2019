using spc = Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using spToolbelt2019.Forms;
using Squirrel;
using System.Threading.Tasks;
using System.Diagnostics;

namespace spToolbelt2019
{

    public partial class frmMain : Form
    {
        spc.ClientContext ctx;
        public frmMain()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            EnableUI(false);
           
            AddVersionNumber();
        }

        private void AddVersionNumber()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.Text += $"v.{versionInfo.ProductVersion}";// "SharePoint Toolbelt: Version ("+versionInfo.ProductVersion+")";
        }

        private void  CheckForUpdates()
        {
            using (var manager = new UpdateManager("https://rammware.s3.us-east-2.amazonaws.com/spToolBelt2019","spToolBelt2019"))
            {
                 manager.UpdateApp();
                MessageBox.Show("Update Check Complete!");
            }
        }


        private void EnableUI(bool bEnableUI)
        {
            try
            {
                mnuSiteScript.Enabled = bEnableUI;
                tbSiteScript.Enabled = bEnableUI;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetLogin();
        }

        private void GetLogin()
        {
            try
            {
                frmLogin oLogin = new frmLogin();
                DialogResult result = oLogin.ShowDialog();
                if (result == DialogResult.OK)
                {
                    ctx = oLogin.LoginContext;
                }
                oLogin.Dispose();
                EnableUI(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);       
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            GetLogin();
        }

        private void tbSiteScript_Click(object sender, EventArgs e)
        {
            ShowSiteScript();
        }

        private void ShowSiteScript()
        {
            frmSiteScript oSiteScript = new frmSiteScript(ctx);
            oSiteScript.MdiParent = this;
            oSiteScript.Show();
        }

        private void mnuSiteScript_Click(object sender, EventArgs e)
        {
            ShowSiteScript();
        }


        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAboutScreen();
        }

        private void ShowAboutScreen()
        {
            Forms.frmAbout oAbout = new Forms.frmAbout();
            oAbout.MdiParent = this;
            oAbout.Show();
        }


        private void shareGateScriptBuilderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.frmBuildShareGateScript fb = new frmBuildShareGateScript(ctx);
            fb.MdiParent = this;
            fb.Show();
        }

        private void TestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTest frmtest = new frmTest(ctx);
            frmtest.MdiParent = this;
            frmtest.Show();
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var manager = new UpdateManager("https://rammware.s3.us-east-2.amazonaws.com/spToolBelt2019", "spToolBelt2019"))
            {
                
                manager.UpdateApp();
                MessageBox.Show("Update Check Complete!");
            }


        }

        private void viewLogFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string cTempPath = Path.GetTempPath();
            string cLogPath = cTempPath + @"\spToolBelt";
            if (!Directory.Exists(cLogPath))
            {
                Directory.CreateDirectory(cLogPath);
            }
            Process.Start(cLogPath);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Forms.frmBuildShareGateScript fb = new frmBuildShareGateScript(ctx);
            fb.MdiParent = this;
            fb.Show();
        }
    }
}
