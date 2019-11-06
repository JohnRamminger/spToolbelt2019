using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;
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

        

        private void button5_Click(object sender, EventArgs e)
        {
            List<EventList> lists = new List<EventList>();
            KeywordQuery keywordQuery = new KeywordQuery(ctx);
            keywordQuery.QueryText = "ContentTypeId:0x0102*";
            keywordQuery.RowLimit = 500;
            keywordQuery.SelectProperties.Add("SPWebUrl");
            keywordQuery.SelectProperties.Add("ListID");
            SearchExecutor searchExecutor = new SearchExecutor(ctx);
            ClientResult<ResultTableCollection> results = searchExecutor.ExecuteQuery(keywordQuery);
            ctx.ExecuteQuery();
            
            foreach(ResultTable rt in results.Value)
            {
                foreach(IDictionary<string,object> result in rt.ResultRows)
                {
                    string cListID = result["ListID"].ToString();
                    string cWebUrl = result["SPWebUrl"].ToString();
                    EventList el = new EventList();
                    el.ListId = cListID;
                    el.SiteUrl = cWebUrl;
                    if (!HasItem(lists,cListID))
                    {
                        lists.Add(el);
                    }
                    if (!lists.Contains(el))
                    {
                        
                    }
                }
            }

            foreach (EventList eventList in lists)
            {
                ClientContext workCTX = new ClientContext(eventList.SiteUrl);
                workCTX.Credentials = ctx.Credentials;
                List lst = workCTX.Web.Lists.GetById(new Guid(eventList.ListId));
                ActivateEventFeature(workCTX, "bb635f49-78bb-406a-94c9-e28a5ac07375");

                lst.EnsureListHasContenttype(workCTX.Site, "atiEvent");
                Microsoft.SharePoint.Client.ContentType ct = lst.GetContentType("atiEvent");
                workCTX.Load(ct, t => t.Name, t => t.Id);
                workCTX.ExecuteQuery();
                CamlQuery oQuery = CamlQuery.CreateAllItemsQuery();
                ListItemCollection items = lst.GetItems(oQuery);
                workCTX.Load(items, i => i.Include(itm => itm.ContentType.Name, itm => itm["RecurrenceData"],itm=>itm["atiEventRecurrence"]));
                workCTX.ExecuteQuery();
                foreach (ListItem itm in items)
                {
                    try
                    {
                        bool bUpdate = false;
                        if (itm.ContentType.Name == "Event")
                        {
                            itm["ContentTypeId"] = ct.Id;
                            bUpdate = true;
                        }
                        if (itm["RecurrenceData"] != null)
                        {
                            if (itm["RecurrenceData"] != itm["atiEventRecurrence"])
                            {
                                itm["atiEventRecurrence"] = itm["RecurrenceData"];
                                bUpdate = true;
                            }
                        }


                        if (bUpdate)
                        {
                            itm.SystemUpdate();
                            workCTX.ExecuteQuery();
                        }


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
                lst.RemoveContentTypeByName("Event");
                lst.DisableContentTypes();
                workCTX.ExecuteQuery();
           }

            MessageBox.Show("done ");


        }

        private bool HasItem(List<EventList> lists, string cListID)
        {
            foreach (EventList eventList in lists)
            {
                if (eventList.ListId==cListID)
                {
                    return true;
                }
            }
            return false;
        }

        private void ActivateEventFeature(ClientContext ctxWork,string cFeatureID)
        {
            try
            {
                var features = ctxWork.Web.Features;
                ctxWork.Load(features);

                ctxWork.Load(features, fcol => fcol.Include(f => f.DisplayName, f => f.DefinitionId));
                


                ctxWork.ExecuteQuery();

                foreach (var item in features)
                {
                    System.Diagnostics.Trace.WriteLine(item.DisplayName);
                    if (item.DefinitionId == new Guid(cFeatureID))
                    {
                         return;
                    }
                }

                features.Add(new Guid(cFeatureID.ToUpper()), true,FeatureDefinitionScope.Web);
                ctxWork.ExecuteQuery();
            }
            catch (Exception ex)
            {
                
            }




        }
    }
    public class EventList
    {
        public string ListId { get; set; }
        public string SiteUrl { get; set; }
    }
}
