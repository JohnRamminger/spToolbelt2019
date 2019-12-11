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
using spToolbelt2019Lib;

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

        private void WalkCustomActions()
        {
            Web ww = ctx.Web;
            UserCustomActionCollection ucas = ww.UserCustomActions;
            ctx.Load(ucas);
            ctx.Load(ww);
            ctx.ExecuteQuery();
            foreach (UserCustomAction uca in ucas)
            {
                ctx.Load(uca);
                ctx.ExecuteQuery();
                System.Diagnostics.Trace.WriteLine(uca.Name);

                uca.ClientSideComponentProperties = "{\"FooterMessage\":\"This Site is External Facing.\"}";
                uca.Update();
                ctx.ExecuteQuery();
                
                
            }



        }
        
        private void ApplyTheme(ClientContext workCTX,string cFilename,string cLocalFile)
        {
            try
            {
                Web ww = workCTX.Web;
                ww.EnsureList("SiteAssets", ListTemplateType.DocumentLibrary, "");
                List lstSiteAssets = ww.Lists.GetByTitle("SiteAssets");
                workCTX.Load(lstSiteAssets);
                workCTX.ExecuteQuery();
                lstSiteAssets.EnsureFile(workCTX, cFilename, cLocalFile);
                string cWebfile = "/SiteAssets/" + cFilename;
                ListItem workItem = lstSiteAssets.GetListItemByFileName(workCTX, cFilename);
                workCTX.Load(workItem, wf => wf.File.ServerRelativeUrl);
                workCTX.ExecuteQuery();
                cWebfile = workItem.File.ServerRelativeUrl;
                
                ww.ApplyTheme(cWebfile, null, null, true);
                workCTX.ExecuteQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                ApplyTheme(ctx, "ATIMain.spcolor", @"c:\code\ati\scripts\ATIMain.spcolor");
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ApplyTheme(ctx, "ATISecondary.spcolor", @"c:\code\ati\scripts\ATISecondary.spcolor");

        }

        private void Button4_Click(object sender, EventArgs e)
        {
            WalkCustomActions();
        }
    }
}
