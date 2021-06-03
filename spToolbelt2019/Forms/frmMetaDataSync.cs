using Microsoft.SharePoint.Client;
using spToolbelt2019Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace spToolbelt2019.Forms
{
    public partial class frmMetaDataSync : System.Windows.Forms.Form
    {
        ClientContext ctx;
        metadataWorker oWorker = new metadataWorker();
        public frmMetaDataSync(ClientContext workCTX)
        {
            ctx = workCTX;
            InitializeComponent();
        }

        private void frmMetaDataSync_Load(object sender, EventArgs e)
        {
            oWorker.Canceled += OWorker_Canceled;
            oWorker.Complete += OWorker_Complete;
            oWorker.Error += OWorker_Error;
            oWorker.Info += OWorker_Info;
            oWorker.Progress += OWorker_Progress;
            oWorker.ReaminingTimeInfo += OWorker_ReaminingTimeInfo;
        }

        private void OWorker_ReaminingTimeInfo(string ReaminingTimeInfo)
        {
            
        }

        private void OWorker_Progress(string cProgress)
        {
            lstResults.Items.Add(cProgress);
        }

        private void OWorker_Info(string cInfo)
        {
            
        }

        private void OWorker_Error(string cExceptionMessage, string cLocation, string cMessage)
        {
            
        }

        private void OWorker_Complete()
        {
            
        }

        private void OWorker_Canceled()
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            oWorker.Start(ctx);
        }
    }
}
