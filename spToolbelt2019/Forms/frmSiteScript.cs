using Microsoft.SharePoint.Client;
using spToolbelt2019Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace spToolbelt2019
{
    public partial class frmSiteScript : System.Windows.Forms.Form
    {
        ClientContext ctx;
        scriptWorker oWorker;
        scriptItems scriptWorkItems = null;
        public frmSiteScript()
        {
            InitializeComponent();
        }
        public frmSiteScript(ClientContext workContext)
        {
            InitializeComponent();
            ctx = workContext;
        }
        private void cmdLoadScipt_Click(object sender, EventArgs e)
        {
            OpenFileDialog oDlg = new OpenFileDialog() { Filter = "spToolbelt2019 Script File (.tbss)|*.tbss|All Files (*.*)|*.*", FilterIndex = 1 };
            DialogResult retVal = oDlg.ShowDialog();
            if (retVal==DialogResult.OK)
            {
                if (System.IO.File.Exists(oDlg.FileName))
                {
                    scriptWorkItems = new scriptItems();
                    string cLoadStatus = scriptWorkItems.LoadFromFile(oDlg.FileName);
                    txtResults.Text += cLoadStatus;
                    if (scriptWorkItems.IsValid)
                    {
                        txtScript.Text = scriptWorkItems.RawText;
                    }
                    else
                    {
                        foreach (scriptItem scriptWorkItem in scriptWorkItems)
                        {
                            txtResults.Text += string.Format("{0} {1} - {2}", Environment.NewLine, scriptWorkItem.RawLine, scriptWorkItem.Status);
                        }
                    }

                }
            }
        }

        private void cmdRunScript_Click(object sender, EventArgs e)
        {
            cmdRunScript.Text = "Cancel Script";
            if (oWorker == null)
            {
                oWorker = new scriptWorker();
                oWorker.ProcessSubWebs = chkProcessSubWebs.Checked;
                oWorker.Canceled += OWorker_Canceled;
                oWorker.Complete += OWorker_Complete;
                oWorker.Error += OWorker_Error;
                oWorker.Info += OWorker_Info;
                oWorker.Progress += OWorker_Progress;
                oWorker.Complete += OWorker_Complete1;
                oWorker.ReaminingTimeInfo += OWorker_ReaminingTimeInfo;
                oWorker.Start("SiteScript",ctx, scriptWorkItems, txtCommands.Text,chkAllSites.Checked);

            }


        }

        private void OWorker_ReaminingTimeInfo(string ReaminingTimeInfo)
        {
            progressMain.Maximum = oWorker.TotalItems;
            lblRemaining.Text = oWorker.RemainingTime;
            Int32 currentValue = Convert.ToInt32((oWorker.RunCount / oWorker.TotalItems) * 100);
            progressMain.Increment(1);
            

        }

        private void OWorker_Complete1()
        {
            txtResults.Text += "Complete!" + Environment.NewLine; ;
           
        }

        private void OWorker_Progress(string cProgress)
        {
            txtResults.Text += cProgress+Environment.NewLine;
        }

        private void OWorker_Info(string cInfo)
        {
            lblRunCount.Text = oWorker.RunCount.ToString();
            lblRemaining.Text = oWorker.RemainingTime;
            lblStatus.Text = cInfo;
        }

        private void OWorker_Error(string cExceptionMessage, string cLocation, string cMessage)
        {
            txtResults.Text += cExceptionMessage+" - "+cLocation+" - "+cMessage + Environment.NewLine;
        }

        private void OWorker_Complete()
        {
            cmdRunScript.Text = "Run Script";
        }

        private void OWorker_Canceled()
        {
         
        }

        private void splitMain_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblStatus_Click(object sender, EventArgs e)
        {

        }
    }
}
