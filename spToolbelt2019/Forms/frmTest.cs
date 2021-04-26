using System;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Pages;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SharePoint.Client;
using spToolbelt2019Lib;
using Microsoft.SharePoint.Client.Workflow;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using ClientSidePage = OfficeDevPnP.Core.Pages.ClientSidePage;
using Newtonsoft.Json.Linq;
using CanvasControl = OfficeDevPnP.Core.Pages.CanvasControl;
using System.IO;
using spToolbelt2019lib;
using Newtonsoft.Json;
using LinqToExcel.Extensions;

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

        private void AddSampleData()
        {
            List lst = ctx.Web.Lists.GetByTitle("ReportingPeriods");
            
            InsertSampleData(lst, "2011 Period 01", "Pd_1FY11");
            InsertSampleData(lst, "2018 Period 11", "Pd_11FY18");
            InsertSampleData(lst, "2019 Period 03", "Pd_3FY19");
            InsertSampleData(lst, "2018 Period 10", "Pd_10FY18");
            InsertSampleData(lst, "2019 Period 10", "Pd_10FY19");
            InsertSampleData(lst, "2019 Period 08", "Pd_8FY19");
            InsertSampleData(lst, "2019 Period 01", "Pd_1FY19");
            InsertSampleData(lst, "2019 Period 02", "Pd_2FY19");
            InsertSampleData(lst, "2018 Period 06", "Pd_6FY18");
            InsertSampleData(lst, "2018 Period 12", "Pd_12FY18");
            InsertSampleData(lst, "2018 Period 07", "Pd_7FY18");
            InsertSampleData(lst, "2019 Period 09", "Pd_9FY19");
            InsertSampleData(lst, "2018 Period 04", "Pd_4FY18");
            InsertSampleData(lst, "2018 Period 09", "Pd_9FY18");
            InsertSampleData(lst, "2019 Period 06", "Pd_6FY19");
            InsertSampleData(lst, "2018 Period 08", "Pd_8FY18");
            InsertSampleData(lst, "2019 Period 07", "Pd_7FY19");
            InsertSampleData(lst, "2018 Period 01", "Pd_1FY18");
            InsertSampleData(lst, "2018 Period 05", "Pd_5FY18");
            InsertSampleData(lst, "2019 Period 04", "Pd_4FY19");
            InsertSampleData(lst, "2018 Period 03", "Pd_3FY18");
            InsertSampleData(lst, "2019 Period 05", "Pd_5FY19");
            InsertSampleData(lst, "2017 Period 12", "Pd_12FY17");
            InsertSampleData(lst, "2017 Period 09", "Pd_9FY17");
            InsertSampleData(lst, "2017 Period 11", "Pd_11FY17");
            InsertSampleData(lst, "2017 Period 14", "Pd_14FY17");
            InsertSampleData(lst, "2017 Period 10", "Pd_10FY17");
            InsertSampleData(lst, "2017 Period 03", "Pd_3FY17");
            InsertSampleData(lst, "2018 Period 02", "Pd_2FY18");
            InsertSampleData(lst, "2017 Period 06", "Pd_6FY17");
            InsertSampleData(lst, "2017 Period 04", "Pd_4FY17");
            InsertSampleData(lst, "2017 Period 08", "Pd_8FY17");
            InsertSampleData(lst, "2017 Period 15", "Pd_15FY17");
            InsertSampleData(lst, "2017 Period 13", "Pd_13FY17");
            InsertSampleData(lst, "2017 Period 05", "Pd_5FY17");
            InsertSampleData(lst, "2017 Period 07", "Pd_7FY17");
            InsertSampleData(lst, "2017 Period 01", "Pd_1FY17");
            InsertSampleData(lst, "2017 Period 02", "Pd_2FY17");
            InsertSampleData(lst, "2013 Period 12", "Pd_12FY13");
            InsertSampleData(lst, "2013 Period 05", "Pd_5FY13");
            InsertSampleData(lst, "2013 Period 11", "Pd_11FY13");
            InsertSampleData(lst, "2013 Period 09", "Pd_9FY13");
            InsertSampleData(lst, "2013 Period 10", "Pd_10FY13");
            InsertSampleData(lst, "2013 Period 08", "Pd_8FY13");
            InsertSampleData(lst, "2013 Period 01", "Pd_1FY13");
            InsertSampleData(lst, "2013 Period 02", "Pd_2FY13");
            InsertSampleData(lst, "2013 Period 04", "Pd_4FY13");
            InsertSampleData(lst, "2013 Period 06", "Pd_6FY13");
            InsertSampleData(lst, "2013 Period 03", "Pd_3FY13");
            InsertSampleData(lst, "2013 Period 07", "Pd_7FY13");
            InsertSampleData(lst, "2014 Period 03", "Pd_3FY14");
            InsertSampleData(lst, "2014 Period 04", "Pd_4FY14");
            InsertSampleData(lst, "2014 Period 01", "Pd_1FY14");
            InsertSampleData(lst, "2015 Period 01", "Pd_1FY15");
            InsertSampleData(lst, "2014 Period 07", "Pd_7FY14");
            InsertSampleData(lst, "2014 Period 08", "Pd_8FY14");
            InsertSampleData(lst, "2014 Period 02", "Pd_2FY14");
            InsertSampleData(lst, "2014 Period 06", "Pd_6FY14");
            InsertSampleData(lst, "2014 Period 09", "Pd_9FY14");
            InsertSampleData(lst, "2014 Period 11", "Pd_11FY14");
            InsertSampleData(lst, "2014 Period 05", "Pd_5FY14");
            InsertSampleData(lst, "2016 Period 12", "Pd_12FY16");
            InsertSampleData(lst, "2014 Period 12", "Pd_12FY14");
            InsertSampleData(lst, "2015 Period 03", "Pd_3FY15");
            InsertSampleData(lst, "2015 Period 04", "Pd_4FY15");
            InsertSampleData(lst, "2014 Period 10", "Pd_10FY14");
            InsertSampleData(lst, "2016 Period 01", "Pd_1FY16");
            InsertSampleData(lst, "2015 Period 05", "Pd_5FY15");
            InsertSampleData(lst, "2016 Period 07", "Pd_7FY16");
            InsertSampleData(lst, "2010 Period 01", "Pd_1FY10");
            InsertSampleData(lst, "2016 Period 11", "Pd_11FY16");
            InsertSampleData(lst, "2016 Period 10", "Pd_10FY16");
            InsertSampleData(lst, "2016 Period 03", "Pd_3FY16");
            InsertSampleData(lst, "2016 Period 04", "Pd_4FY16");
            InsertSampleData(lst, "2016 Period 08", "Pd_8FY16");
            InsertSampleData(lst, "2016 Period 02", "Pd_2FY16");
            InsertSampleData(lst, "2019 Period 11", "Pd_11FY19");
            InsertSampleData(lst, "2015 Period 11", "Pd_11FY15");
            InsertSampleData(lst, "2016 Period 05", "Pd_5FY16");
            InsertSampleData(lst, "2015 Period 06", "Pd_6FY15");
            InsertSampleData(lst, "2016 Period 09", "Pd_9FY16");
            InsertSampleData(lst, "2015 Period 10", "Pd_10FY15");
            InsertSampleData(lst, "2015 Period 02", "Pd_2FY15");
            InsertSampleData(lst, "2016 Period 06", "Pd_6FY16");


        }

        private void InsertSampleData(List lst, string cTitle, string cPeriod)
        {
            ListItemCreationInformation lici = new ListItemCreationInformation();
            ListItem li = lst.AddItem(lici);
            li["Title"] = cTitle;
            li["Period"] = cPeriod;
            li.Update();
            lst.Context.ExecuteQuery();
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

        private void ApplyTheme(ClientContext workCTX, string cFilename, string cLocalFile)
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
            // List<EventList> lists = new List<EventList>();
            // KeywordQuery keywordQuery = new KeywordQuery(ctx);
            // keywordQuery.QueryText = "ContentTypeId:0x0102*";
            // keywordQuery.RowLimit = 500;
            // keywordQuery.SelectProperties.Add("SPWebUrl");
            // keywordQuery.SelectProperties.Add("ListID");
            // SearchExecutor searchExecutor = new SearchExecutor(ctx);
            // ClientResult<ResultTableCollection> results = searchExecutor.ExecuteQuery(keywordQuery);
            // ctx.ExecuteQuery();

            // foreach(ResultTable rt in results.Value)
            // {
            //     foreach(IDictionary<string,object> result in rt.ResultRows)
            //     {
            //         string cListID = result["ListID"].ToString();
            //         string cWebUrl = result["SPWebUrl"].ToString();
            //         EventList el = new EventList();
            //         el.ListId = cListID;
            //         el.SiteUrl = cWebUrl;
            //         if (!HasItem(lists,cListID))
            //         {
            //             lists.Add(el);
            //         }
            //         if (!lists.Contains(el))
            //         {

            //         }
            //     }
            // }

            // foreach (EventList eventList in lists)
            // {
            //     ClientContext workCTX = new ClientContext(eventList.SiteUrl);
            //     workCTX.Credentials = ctx.Credentials;
            //     List lst = workCTX.Web.Lists.GetById(new Guid(eventList.ListId));
            //     ActivateEventFeature(workCTX, "bb635f49-78bb-406a-94c9-e28a5ac07375");

            //     lst.EnsureListHasContenttype(workCTX.Site, "atiEvent");
            //     Microsoft.SharePoint.Client.ContentType ct = lst.GetContentType("atiEvent");
            //     workCTX.Load(ct, t => t.Name, t => t.Id);
            //     workCTX.ExecuteQuery();
            //     CamlQuery oQuery = CamlQuery.CreateAllItemsQuery();
            //     ListItemCollection items = lst.GetItems(oQuery);
            //     workCTX.Load(items, i => i.Include(itm => itm.ContentType.Name, itm => itm["RecurrenceData"],itm=>itm["atiEventRecurrence"]));
            //     workCTX.ExecuteQuery();
            //     foreach (ListItem itm in items)
            //     {
            //         try
            //         {
            //             bool bUpdate = false;
            //             if (itm.ContentType.Name == "Event")
            //             {
            //                 itm["ContentTypeId"] = ct.Id;
            //                 bUpdate = true;
            //             }
            //             if (itm["RecurrenceData"] != null)
            //             {
            //                 if (itm["RecurrenceData"] != itm["atiEventRecurrence"])
            //                 {
            //                     itm["atiEventRecurrence"] = itm["RecurrenceData"];
            //                     bUpdate = true;
            //                 }
            //             }


            //             if (bUpdate)
            //             {
            //                 itm.SystemUpdate();
            //                 workCTX.ExecuteQuery();
            //             }


            //         }
            //         catch (Exception ex)
            //         {
            //             MessageBox.Show("Error: " + ex.Message);
            //         }
            //     }
            //     lst.RemoveContentTypeByName("Event");
            //     lst.DisableContentTypes();
            //     workCTX.ExecuteQuery();
            //}

            // MessageBox.Show("done ");


        }

        private bool HasItem(List<EventList> lists, string cListID)
        {
            foreach (EventList eventList in lists)
            {
                if (eventList.ListId == cListID)
                {
                    return true;
                }
            }
            return false;
        }

        private void ActivateEventFeature(ClientContext ctxWork, string cFeatureID)
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

                features.Add(new Guid(cFeatureID.ToUpper()), true, FeatureDefinitionScope.Web);
                ctxWork.ExecuteQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            UpdateFile("Home.aspx");
        }

        private ClientSideComponent GetComponent(ClientSidePage page,string cAlias)
        {
            var comps = page.AvailableClientSideComponents();
            foreach (ClientSideComponent csc in comps)
            {
                dynamic data = JObject.Parse(csc.Manifest);
                string alias = data.alias;
                if (alias == cAlias)
                {
                    return csc;
                }
            }



                return null;
        }

        private void UpdateFile(string cFileName)
        {
            try
            {
                
                ClientSidePage page = ClientSidePage.Load(ctx, "Home.aspx");
                ClientSidePage newPage = new ClientSidePage(ctx, ClientSidePageLayoutType.Home);

                foreach (CanvasControl ctrl in page.Controls)
                {
                    System.Diagnostics.Trace.WriteLine(ctrl.ToString());
                    newPage.Controls.Add(ctrl);
                }
                
                newPage.Save("NewHome.aspx");
                ctx.ExecuteQuery();

                

                
                ClientSideComponent c = GetComponent(page, "EtcContentDisplayWebPart");

                if (c!=null)
                {
                    ClientSideWebPart csWebPart = new ClientSideWebPart(c);
                    page.Controls.Add(csWebPart);
                    page.Save();
                    ctx.ExecuteQuery();
                    
                }


                
                
                //foreach(ClientSideComponent csc in comps)
                //{
                //    dynamic data = JObject.Parse(csc.Manifest);
                //    string alias = data.alias;
                //    string componentType = data.componentType;
                //    string isInternal = data.isInternal;


                //    System.Diagnostics.Trace.WriteLine(componentType +" - "+isInternal+" - "+alias);
                    
                    


                //    if (csc.Manifest.Contains("Content Display"))
                //    {
                        
                //        System.Diagnostics.Trace.WriteLine(csc.Name+" -"+csc.Status+" - "+csc.ManifestType);
                //    }
                    
                        
                //}
                System.Diagnostics.Trace.WriteLine("");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            List lstDocs = ctx.Web.Lists.GetByTitle("CandidateDocuments");
            List lstCandidates = ctx.Web.Lists.GetByTitle("Candidates");
            CamlQuery oQuery = CamlQuery.CreateAllItemsQuery();
            ListItemCollection items = lstDocs.GetItems(oQuery);
            ctx.Load(items, i => i.Include(itm => itm["ItemParentID"],itm=>itm["Candidate"],itm=>itm["FileLeafRef"]));
            ctx.ExecuteQuery();
            foreach(ListItem itm in items)
            {
                try
                { 
                if (itm["ItemParentID"]==null || itm["Candidate"]==null)
                {
                    string Key = itm["FileLeafRef"].ToString().Substring(0, 6);
                    ListItem itmCandidate = lstCandidates.GetListItemByTitle(Key);
                    itm["ItemParentID"] = itmCandidate.Id;
                    itm["Candidate"] = itmCandidate.Id;
                    itm.Update();
                    ctx.ExecuteQuery();


                }
                } catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                }
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            string cLoc = @"C:\temp\desc\SiteInfo07242020";
            StreamWriter oWriter = new StreamWriter(@"c:\temp\DeSCWorkflows.csv");
            oWriter.WriteLine("Site,List,Last Updated,Workflow Name");
            if (System.IO.Directory.Exists(cLoc))
            {
                DirectoryInfo di = new DirectoryInfo(cLoc);
                foreach (FileInfo fi in di.GetFiles("*.json"))
                {
                    infoSite si = JsonConvert.DeserializeObject<infoSite>(System.IO.File.ReadAllText(fi.FullName));
                    foreach (infoList li in si.Lists)
                    {
                        if (li.workflows!=null)
                        {
                            if (li.workflows.Count > 0)
                            {
                                foreach (infoWorkflow wf in li.workflows)
                                {
                                    if (!wf.Name.ToLower().Contains("previous"))
                                    {
                                        oWriter.WriteLine(si.SiteUrl + "," + DeComma(li.ListTitle) + "," + li.LastItemModified.ToShortDateString() + "," + DeComma(wf.Name));
                                    }
                                }
                            }
                        }
                    }

                }
            }
            MessageBox.Show("Done");
            oWriter.Flush();
            oWriter.Close();

        }

        private string DeComma(string inputStr)
        {
            while (inputStr.Contains(","))
            {
                inputStr=inputStr.Replace(",", "-");
            }
            return inputStr;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string cLoc = @"C:\temp\DeWorkflow\SiteInfo07152020";
            StreamWriter oWriter = new StreamWriter(@"c:\temp\DominionLists.csv");
            oWriter.WriteLine("Site,List,Last Updated,ItemCount");
            if (System.IO.Directory.Exists(cLoc))
            {

                DirectoryInfo di = new DirectoryInfo(cLoc);
                foreach (FileInfo fi in di.GetFiles("*.json"))
                {
                    infoSite si = JsonConvert.DeserializeObject<infoSite>(System.IO.File.ReadAllText(fi.FullName));
                    foreach (infoList li in si.Lists)
                    {
                        if (li.ListItemCount > 0)
                        {
                            oWriter.WriteLine(DeComma(si.SiteUrl) + "," + DeComma(li.ListTitle) + "," + li.LastItemModified.ToShortDateString() + "," + li.ListItemCount);
                        }
                    }

                }
            }
            MessageBox.Show("Done");
            oWriter.Flush();
            oWriter.Close();


        }

        private void button7_Click(object sender, EventArgs e)
        {
            txtResults.Text = "";
            button7.Enabled = false;
            string cLoc = @"C:\temp\DeWorkflow\SiteInfo07152020";
            if (System.IO.Directory.Exists(cLoc))
            {
                DirectoryInfo di = new DirectoryInfo(cLoc);
                foreach (FileInfo fi in di.GetFiles("*.json"))
                {
                    infoSite si = JsonConvert.DeserializeObject<infoSite>(System.IO.File.ReadAllText(fi.FullName));
                    foreach(string cFeature in si.Features)
                    {
                        if (cFeature.ToLower().Contains(txtFeatureId.Text.ToLower()))
                        {
                            txtResults.Text += si.SiteUrl + Environment.NewLine;
                        }
                    }
                }
            }
            button7.Enabled = true;






        }

        private void button8_Click(object sender, EventArgs e)
        {
            txtResults.Text = "";
            button8.Enabled = false;
            string cLoc = @"C:\temp\DeWorkflow\SiteInfo07152020";
            if (System.IO.Directory.Exists(cLoc))
            {
                DirectoryInfo di = new DirectoryInfo(cLoc);
                foreach (FileInfo fi in di.GetFiles("*.json"))
                {
                    infoSite si = JsonConvert.DeserializeObject<infoSite>(System.IO.File.ReadAllText(fi.FullName));
                    foreach(infoList li in si.Lists)
                    {
                        foreach(infoField fld in li.fields)
                        {
                            if (fld.FieldTitle.ToLower().Contains(txtFeatureId.Text.ToLower()) || fld.FieldName.ToLower().Contains(txtFeatureId.Text.ToLower()))
                            {
                                txtResults.Text += si.SiteUrl + " - " + li.ListTitle + " - " + li.LastItemModified.ToShortDateString()+Environment.NewLine;
                            }
                        }
                    }
                }
            }
            button8.Enabled = true;


        }

        private void button9_Click(object sender, EventArgs e)
        {
            txtResults.Text = "";
            button9.Enabled = false;
            string cLoc = @"C:\temp\DeWorkflow\SiteInfo07152020";
            if (System.IO.Directory.Exists(cLoc))
            {
                DirectoryInfo di = new DirectoryInfo(cLoc);
                foreach (FileInfo fi in di.GetFiles("*.json"))
                {

                    infoSite si = JsonConvert.DeserializeObject<infoSite>(System.IO.File.ReadAllText(fi.FullName));
                    foreach(infoPage pg in si.PageFiles)
                    {
                        foreach(infoWebPart wp in pg.WebParts)
                        {
                            if (wp.Title.Contains(txtFeatureId.Text))
                            {
                                txtResults.Text += si.SiteUrl + " - " + pg.Url + " - " + wp.Title + Environment.NewLine;
                            }
                        }
                        
                    }
                }
            }
            button9.Enabled = true;


        }

        private void cmdWorkflowFind_Click(object sender, EventArgs e)
        {
            txtResults.Text = "";
        cmdWorkflowFind.Enabled = false;
            string cLoc = @"C:\temp\DeWorkflow\SiteInfo07152020";
            if (System.IO.Directory.Exists(cLoc))
            {
                DirectoryInfo di = new DirectoryInfo(cLoc);
                foreach (FileInfo fi in di.GetFiles("*.json"))
                {

                    infoSite si = JsonConvert.DeserializeObject<infoSite>(System.IO.File.ReadAllText(fi.FullName));
                    foreach(infoList lst in si.Lists)
                    {
                        if (lst.workflows != null)
                        {
                            foreach (infoWorkflow wf in lst.workflows)
                            {
                                if (wf.Name.Contains(txtFeatureId.Text.ToLower()))
                                {
                                    txtResults.Text += si.SiteUrl + " - " + lst.ListTitle + " - " + wf + Environment.NewLine;
                                }
                            }
                        }
                    }
                    
                    
                    
                    foreach (infoPage pg in si.PageFiles)
                    {
                        foreach (infoWebPart wp in pg.WebParts)
                        {
                            if (wp.Title.Contains(txtFeatureId.Text))
                            {
                                txtResults.Text += si.SiteUrl + " - " + pg.Url + " - " + wp.Title + Environment.NewLine;
                            }
                        }

                    }
                }
            }
            cmdWorkflowFind.Enabled = true;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            WorkflowAssociationCollection wfs = ctx.Web.WorkflowAssociations;
            ctx.Load(wfs);
            ctx.ExecuteQuery();
            foreach(WorkflowAssociation wf in wfs)
            {
                System.Diagnostics.Trace.WriteLine(wf.Name);
            }

            ListCollection lsts = ctx.Web.Lists;
            ctx.Load(lsts, l => l.Include(lst => lst.Title, lst => lst.WorkflowAssociations));
            ctx.ExecuteQuery();
            foreach (List lst in lsts)
            {
                 wfs = lst.WorkflowAssociations;
                ctx.Load(wfs);
                ctx.ExecuteQuery();
                if (lst.WorkflowAssociations.Count > 0)
                {
                    foreach(WorkflowAssociation wf in lst.WorkflowAssociations)
                    {
                        System.Diagnostics.Trace.WriteLine(wf.Name);
                    }
                }
            }
        }



        private void fieldtest()
        {
            List lstDocs = ctx.Web.Lists.GetByTitle("Documents");
            Microsoft.SharePoint.Client.Field fld = lstDocs.Fields.GetByInternalNameOrTitle("Title");
            ctx.Load(lstDocs);
            ctx.Load(fld);
            ctx.ExecuteQuery();
            fld.SetProperty("securedTo", "John");
            ctx.ExecuteQuery();


            
        }



        
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            Microsoft.SharePoint.Client.ContentType ctDoc = ctx.Web.ContentTypes.GetById("0x0101");
            ctx.Load(ctDoc);
            ctx.ExecuteQuery();

            ContentTypeCreationInformation ctci = new ContentTypeCreationInformation();
            ctci.Description = "test";
            ctci.Group = "Test";
            ctci.ParentContentType = ctDoc;
            ctci.Name = RandomString(10);

            ctx.Web.ContentTypes.Add(ctci);
            ctx.ExecuteQuery();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            fieldtest();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            List lstTracks = ctx.Web.Lists.GetByTitle("misTracks");
            lstTracks.DefaultDisplayFormUrl = "https://rammworks.sharepoint.com/sites/MIS/SitePages/test.aspx";
            lstTracks.DefaultEditFormUrl = "https://rammworks.sharepoint.com/sites/MIS/SitePages/test.aspx";
            lstTracks.DefaultNewFormUrl = "https://rammworks.sharepoint.com/sites/MIS/SitePages/test.aspx";
            lstTracks.Update();
            ctx.ExecuteQuery();
        }

        private void button5_Click_11(object sender, EventArgs e)
        {
            Web oWeb = ctx.Web;
            ctx.Load(oWeb);
            ctx.Load(oWeb.UserCustomActions,u=>u.Include(uc=>uc.ClientSideComponentProperties));
            ctx.ExecuteQuery();
            
            foreach (UserCustomAction uca in oWeb.UserCustomActions)
            {
                System.Diagnostics.Trace.WriteLine(uca.ClientSideComponentProperties);
            }
        }
    }

    public class EventList
    {
        public string ListId { get; set; }
        public string SiteUrl { get; set; }
    }
}
