using spc = Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using spToolbelt2019.Forms;

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

        private async Task  CheckForUpdates()
        {
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
            frmUpdateApp oUpdate = new frmUpdateApp();
            oUpdate.MdiParent = this;
            oUpdate.Show();

            

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

        private void scanConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUpdateConfig oFrm = new frmUpdateConfig();
            oFrm.MdiParent = this;
            oFrm.Show();
        }

        private void updateFromSQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUpdateSPSQL oFrm = new frmUpdateSPSQL(ctx);
            oFrm.MdiParent = this;
            oFrm.Show();
        }

        private void syncNavToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSyncNav sn = new frmSyncNav(ctx);
            sn.MdiParent = this;
            sn.Show();

        }

        private void metaDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMetaDataSync frmMDS = new frmMetaDataSync(ctx);
            frmMDS.MdiParent = this;
            frmMDS.Show();
        }
    }
}
