using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;

namespace spToolbelt2019.Forms
{
    public partial class frmTest : System.Windows.Forms.Form
    {
        ClientContext ctx;
        public frmTest(ClientContext workCTX)
        {
            ctx = workCTX;
            InitializeComponent();
        }

        private void FrmTest_Load(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {

            ProvisioningTemplateCreationInformation ptci = new ProvisioningTemplateCreationInformation(ctx.Web);

            // Create FileSystemConnector, so that we can store composed files somewhere temporarily 
            ptci.FileConnector = new FileSystemConnector(@"c:\temp\pnpprovisioningdemo","");
            ptci.PersistBrandingFiles = true;
            ptci.ProgressDelegate = delegate (String message, Int32 progress, Int32 total)
            {

                // Use this to simply output progress to the console application UI
                Console.WriteLine("{0:00}/{1:00} - {2}", progress, total, message);
            };

            
            ProvisioningTemplate template = ctx.Web.GetProvisioningTemplate(ptci);




        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Guid guid = new Guid("e374875e-06b6-11e0-b0fa-57f5dfd72085");
            ctx.Site.Features.Add(guid, true, FeatureDefinitionScope.None);
            ctx.ExecuteQuery();
        }
    }
}
