using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Windows.Forms;
using spc = Microsoft.SharePoint.Client;

namespace spToolbelt2019
{
    public partial class frmLogin : System.Windows.Forms.Form
    {

        StringCollection scSites = null;

        public bool UserLoggedIn = false;
        public spc.ClientContext LoginContext = null;
        public frmLogin()
        {
            InitializeComponent();
        }

        private void EnableUI(bool bEnable)
        {
            try
            {
                chkIsSharePointOnline.Enabled = bEnable;
                chkIntegrated.Enabled = bEnable;
                txtDomain.Enabled = bEnable;
                txtPassword.Enabled = bEnable;
                txtUserName.Enabled = bEnable;
                cmdLogin.Enabled = bEnable;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

        }



        private void LoadSettings()
        {
            scSites = Properties.Settings.Default.SharePointSites;
          
            if (scSites == null) scSites = new StringCollection();
            cboSites.Items.Clear();
            foreach (System.String item in scSites)
            {
                cboSites.Items.Add(item);
            }
            chkIntegrated.Checked = Properties.Settings.Default.UserIntegratedSecurity;
            chkIsSharePointOnline.Checked = Properties.Settings.Default.SharePointOnline;
            cboSites.Text = Properties.Settings.Default.LastUrl;
            txtUserName.Text = Properties.Settings.Default.LastUserName;
            txtDomain.Text = Properties.Settings.Default.LastDomain;
            txtPassword.Text = Properties.Settings.Default.LastPassword;
        }

        private void SaveSettings()
        {

            Properties.Settings.Default.UserIntegratedSecurity= chkIntegrated.Checked;
            Properties.Settings.Default.SharePointOnline= chkIsSharePointOnline.Checked;

            Properties.Settings.Default.SharePointSites = scSites;
            Properties.Settings.Default.LastUrl = cboSites.Text;
            Properties.Settings.Default.LastUserName = txtUserName.Text;
            Properties.Settings.Default.LastPassword = txtPassword.Text;
            Properties.Settings.Default.LastDomain = txtDomain.Text;


            Properties.Settings.Default.Save();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }



        private void button2_Click(object sender, EventArgs e)
        {
            return;
        }

        private void cmdLogin_Click(object sender, EventArgs e)
        {
            try
            {
                EnableUI(false);
                if (!scSites.Contains(cboSites.Text)) scSites.Add(cboSites.Text);
                LoginContext = new Microsoft.SharePoint.Client.ClientContext(cboSites.Text);
                if (chkIntegrated.Checked)
                {

                }
                else
                {
                    if (chkIsSharePointOnline.Checked)
                    {
                        SecureString password = new SecureString();

                        foreach (char c in txtPassword.Text.ToCharArray()) password.AppendChar(c);


                        LoginContext.Credentials = new spc.SharePointOnlineCredentials(txtUserName.Text, password);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(txtDomain.Text))
                        {
                            LoginContext.Credentials = new NetworkCredential(txtUserName.Text, txtPassword.Text, txtDomain.Text);
                        }
                        else
                        {
                            LoginContext.Credentials = new NetworkCredential(txtUserName.Text, txtPassword.Text);
                        }
                    }
                }

                LoginContext.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };

                spc.Web oRootWeb = LoginContext.Site.RootWeb;
                LoginContext.Load(oRootWeb);
                LoginContext.ExecuteQuery();
                System.Diagnostics.Trace.WriteLine(oRootWeb.Title);
                UserLoggedIn = true;
                SaveSettings();
            }
            catch (Exception ex)
            {
                ErrorHandling.LogError(ex, "frmLogin.cmdLogin_Click", "");
            }
           


        }

        private void TxtUserName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
