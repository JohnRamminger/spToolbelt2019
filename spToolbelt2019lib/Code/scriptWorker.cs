using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.SharePoint.Client.Utilities;
using Microsoft.SharePoint.Client.WebParts;
using Microsoft.SharePoint.Client.WorkflowServices;
using Newtonsoft.Json;
using OfficeDevPnP.Core.Enums;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using spToolbelt2019lib;
using spToolbelt2019Lib.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Group = Microsoft.SharePoint.Client.Group;
using GroupCollection = Microsoft.SharePoint.Client.GroupCollection;

namespace spToolbelt2019Lib
{
    public class scriptWorker
    {
        #region Properties
        BackgroundWorker oWorker;
        public Stopwatch sw { get; set; }
        public int RunCount { get; set; }
        public int TotalItems { get; set; }
        StreamWriter oOutputFile = null;
        ClientContext ctx;
        string SiteList;
        string Command;
        List<string> spwebs = null;
        public bool ProcessSubWebs { get; set; }
        public bool bSingleSite { get; set; }


        public void UpdateProgress()
        {
            RunCount += 1;
            string cRemainingTime = RemainingTime;
            oWorker.ReportProgress(4, cRemainingTime);


        }


        public string RemainingTime
        {
            get
            {
                return GetRemaining(RunCount, TotalItems, sw.Elapsed);

                //if (RunCount == 0) return "";
                //    double RemainingSeconds = (sw.Elapsed.TotalSeconds / RunCount) * (TotalItems - RunCount);

                //int iHours = 0;
                //int iMinutes = 0;
                //int iSeconds = 0;

                //if (RemainingSeconds > 3600)
                //{
                //    iHours = Convert.ToInt32(RemainingSeconds / 3600);
                //    RemainingSeconds = RemainingSeconds - (iHours * 3600);
                //}
                //if (RemainingSeconds > 60)
                //{
                //    iMinutes = Convert.ToInt32(RemainingSeconds / 60);
                //    RemainingSeconds = RemainingSeconds - (iMinutes * 60);
                //}
                //iSeconds = Convert.ToInt32(RemainingSeconds);
                //return new TimeSpan(iHours, iMinutes, iSeconds).ToString();
            }

        }





        #endregion

        #region Events

        public delegate void CompleteHandler();
        public event CompleteHandler Complete;

        public delegate void CanceledHandler();
        public event CanceledHandler Canceled;

        public delegate void ProgressHandler(string cProgress);
        public event ProgressHandler Progress;

        public delegate void InfoHandler(string cInfo);
        public event InfoHandler Info;


        public delegate void ReaminingTimeHandler(string ReaminingTimeInfo);
        public event ReaminingTimeHandler ReaminingTimeInfo;


        public delegate void SiteInfoHandler(string cInfo);
        public event SiteInfoHandler SiteInfo;



        public delegate void WorkerErrorHandler(string cExceptionMessage, string cLocation, string cMessage);
        public event WorkerErrorHandler Error;



        #endregion

        #region Public Methods

        string cLogFile;

        scriptItems oWorkItems;
        public void Start(string cWorkerName, ClientContext workerCTX, scriptItems ScriptItems, string cCommand, bool bAllSites)
        {
            bSingleSite = !bAllSites;
            oWorkItems = ScriptItems;

            if (!bSingleSite)
            {
                SiteList = GetAllSiteCollections(workerCTX);
            }
            ctx = workerCTX;
            Command = cCommand;
            string cTempPath = Path.GetTempPath();
            string cLogPath = cTempPath + @"\spToolBelt";
            if (!Directory.Exists(cLogPath))
            {
                Directory.CreateDirectory(cLogPath);
            }
            string cLogName = string.Format(@"spToolBelt\" + cWorkerName + "-{0:yyyy-MM-dd_hh-mm-ss-tt}.log", DateTime.Now);
            string cLogFilename = Path.GetTempPath() + cLogName;
            cLogFile = cLogFilename;
            oOutputFile = new StreamWriter(cLogFilename);

            sw = new Stopwatch();
            if (oWorker == null) oWorker = new BackgroundWorker();

            if (!oWorker.IsBusy)
            {
                oWorker.WorkerReportsProgress = true;
                oWorker.WorkerSupportsCancellation = true;
                oWorker.RunWorkerCompleted += oWorker_RunWorkerCompleted;
                oWorker.ProgressChanged += oWorker_ProgressChanged;
                oWorker.DoWork += oWorker_DoWork;
                oWorker.RunWorkerAsync();
            }
        }

        private string GetAllSiteCollections(ClientContext workerCTX)
        {
            string cRetVal = "";
            try
            {
                Site oSite = workerCTX.Site;
                workerCTX.Load(oSite, os => os.Url);
                workerCTX.ExecuteQuery();
                string cAdminUrl = GetAdminUrl(workerCTX.Site.Url);
                SPOSitePropertiesEnumerable prop = null;
                ClientContext tenantCTX = new ClientContext(cAdminUrl)
                {
                    Credentials = workerCTX.Credentials
                };
                tenantCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };



                Tenant tenant = new Tenant(tenantCTX);
                prop = tenant.GetSiteProperties(0, true);
                tenantCTX.Load(prop);
                tenantCTX.ExecuteQuery();
                foreach (SiteProperties sp in prop)
                {
                    cRetVal += sp.Url + ";";
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "GetAllSiteCollections", "");
            }
            return cRetVal;
        }

        private static string GetAdminUrl(string url)
        {
            string cStartUrl = url.Substring(0, url.IndexOf("."));
            return cStartUrl + "-admin.sharepoint.com";

        }

        public void Cancel()
        {
            sw.Stop();
            oWorker.CancelAsync();
        }

        #endregion

        #region Worker Methods

        void oWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                ShowProgress("Log File Name:" + cLogFile);
                if (bSingleSite)
                {
                    Trace.WriteLine(ctx.Url);
                    WorkSite(ctx.Url);
                }
                else
                {
                    spwebs = new List<string>();
                    // TotalItems = GetSiteCount();
                    ShowProgress(string.Format("Found {0} webs ", spwebs.Count()));

                    //sw.Start();

                    foreach (string spweb in spwebs)
                    {
                        WorkSite(spweb);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "ProcessSites.oWorker_DoWork", "");
            }
        }

        private void WalkFolders(ClientContext pctx, Folder f)
        {
            try
            {
                ShowProgress("Folder: " + f.Name);


                //if (f.Name == "filename")
                {
                    pctx.Load(f.Files);
                    pctx.ExecuteQuery();
                    FileCollection fileCol = f.Files;
                    foreach (Microsoft.SharePoint.Client.File file in fileCol)
                    {
                        ShowProgress("File: " + file.Name);
                        //lstFile.Add(file.Name);
                    }
                }

                FolderCollection folders = f.Folders;
                pctx.Load(folders);
                pctx.ExecuteQuery();
                foreach (Folder folder in folders)
                {
                    WalkFolders(pctx, folder);
                }



            }
            catch (Exception ex)
            {
                ShowError(ex, "ProcessSites.WalkFolders", "");
            }
        }

        private void WorkSite(string item)
        {
            string cLastUrl = "";
            try
            {
                ClientContext workCTX = new ClientContext(item) { Credentials = ctx.Credentials };

                workCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                foreach (scriptItem oWorkItem in oWorkItems)
                {
                    if (oWorkItem.GetParm("url") != cLastUrl)
                    {
                        workCTX = GetClientContext(ctx, oWorkItem.GetParm("url"));
                        cLastUrl = oWorkItem.GetParm("url");
                    }
                    try
                    {
                        Trace.WriteLine(oWorkItem.Command);
                        switch (oWorkItem.Command.ToLower())
                        {
                            case "ensure-targetfolders":
                                EnsureTargetFolders(workCTX, item, oWorkItem);
                                break;
                            case "ensure-targetfields":
                                EnsureTargetFields(workCTX, item, oWorkItem);
                                break;
                            case "find-everyone":
                                FindEveryOne(workCTX);
                                break;
                            case "poll-accessrequests":
                                PollAccessRequests(oWorkItem);
                                break;
                            case "enable-accessrequestsearch":
                                EnableAccessRequestSearch(workCTX, item, oWorkItem);
                                break;

                            case "fix-encryptedtext":
                                FixEncryptedText(workCTX, item, oWorker);
                                break;
                            case "find-customizations":
                                FindCustomizations(workCTX, item, oWorkItem);
                                break;
                            case "update-userinformation":
                                UpdateUserInformation(oWorkItem);
                                break;
                            case "set-sitereadonly":
                                SetSiteReadOnly();
                                break;
                            case "update-sitepermissions":
                                UpdateSitePermissions(oWorkItem);
                                break;
                            case "ensure-sitegallery":
                                EnsureSiteGallery(workCTX, item, oWorkItem);
                                break;
                            case "ensure-versioningenabled":
                                EnsureVersioningEnabled(workCTX, oWorkItem);
                                break;

                            case "ensure-childsite":
                                EnsureChildSite(workCTX, oWorkItem);
                                break;
                            case "show-brokenpermissions":
                                ShowBrokenPermissions(workCTX, oWorkItem);
                                break;
                            case "clear-list":
                                ClearList(oWorkItem);

                                break;
                            case "import-list":
                                ImportList(workCTX, oWorkItem);
                                break;
                            case "copy-list":
                                CopyList(oWorkItem);
                                break;
                            case "list-webparts":
                                EnumWebParts(workCTX, workCTX.Site.RootWeb);
                                break;
                            case "sync-sitecollectionfolder":
                                string cSourceSite = oWorkItem.GetParm("SourceSite");
                                string cSourceFolder = oWorkItem.GetParm("SourceFolder");

                                ShowProgress(string.Format("Syncing Site Folder: {0} - {1} to {2}", cSourceSite, cSourceFolder, item));
                                SyncSiteFolder(cSourceSite, cSourceFolder, item);

                                break;
                            case "remove-listandcontenttype":
                                RemoveListAndContentType(workCTX, oWorkItem);

                                break;

                            case "remove-contenttypefromlist":
                                ShowProgress(string.Format("Remove Content Type From List: {0} - {1}", oWorkItem.GetParm("ListName"), oWorkItem.GetParm("ContentType")));

                                List lstRemove = workCTX.Web.Lists.GetByTitle(oWorkItem.GetParm("ListName"));
                                workCTX.Load(lstRemove);
                                lstRemove.RemoveContentTypeFromList(oWorkItem.GetParm("ContentType"));

                                break;
                            case "disable-contenttypes":
                                ShowProgress(string.Format("Ensure Content Type In List: {0} - {1}", oWorkItem.GetParm("ListName"), oWorkItem.GetParm("ContentType")));

                                List lst = workCTX.Web.Lists.GetByTitle(oWorkItem.GetParm("ListName"));

                                workCTX.Load(lst);
                                workCTX.ExecuteQuery();
                                lst.DisableContentTypes();
                                break;
                            case "download-images":

                                DownloadImages(workCTX, oWorkItem);
                                break;
                            case "ensure-listlookup":
                                EnsureListLookup(workCTX, oWorkItem);

                                break;
                            case "ensure-listhascontenttype":
                                ShowProgress(string.Format("Ensure Content Type In List: {0} - {1}", oWorkItem.GetParm("ListName"), oWorkItem.GetParm("ContentType")));

                                List lst2 = workCTX.Web.Lists.GetByTitle(oWorkItem.GetParm("ListName"));

                                workCTX.Load(lst2);
                                workCTX.ExecuteQuery();
                                lst2.EnsureListHasContenttype(workCTX.Site, oWorkItem.GetParm("ContentType"));
                                break;

                            case "ensure-list":
                                ShowProgress("Working List: " + oWorkItem.GetParm("ListName"));

                                ListTemplateType oTemplateType = GetTemplateType(oWorkItem.GetParm("Type"));
                                workCTX.Web.EnsureList(oWorkItem.GetParm("ListName"), oTemplateType, oWorkItem.GetParm("Description"));
                                break;

                            case "attach-workflow":
                                ShowProgress("Working List: " + oWorkItem.GetParm("ListName"));

                                List lstAttach = workCTX.Web.Lists.GetByTitle(oWorkItem.GetParm("ListName"));
                                workCTX.Load(lstAttach);
                                workCTX.ExecuteQuery();
                                lstAttach.attachWorkflow(workCTX, oWorkItem.GetParm("workflowtemplate"));
                                break;

                            case "provision-list":
                                ShowProgress("Working List: " + oWorkItem.GetParm("ListName"));
                                //
                                ListTemplateType oProvTemplateType = GetTemplateType(oWorkItem.GetParm("Type"));
                                workCTX.Web.EnsureList(oWorkItem.GetParm("ListName"), oProvTemplateType, oWorkItem.GetParm("Description"));
                                ShowProgress(string.Format("Ensure Content Type In List: {0} - {1}", oWorkItem.GetParm("ListName"), oWorkItem.GetParm("ContentType")));

                                List lstProvision = workCTX.Web.Lists.GetByTitle(oWorkItem.GetParm("ListName"));
                                workCTX.Load(lstProvision);
                                workCTX.ExecuteQuery();
                                lstProvision.EnsureListHasContenttype(workCTX.Site, oWorkItem.GetParm("ContentType"));
                                ShowProgress(string.Format("Remove Content Type From List: {0} - {1}", oWorkItem.GetParm("ListName"), oWorkItem.GetParm("DefaultContentType")));
                                lstProvision.RemoveContentTypeFromList(oWorkItem.GetParm("DefaultContentType"));
                                break;
                            case "ensure-contenttypefield":
                                ShowProgress(string.Format("Working Content Field Type: {0}-{1}", oWorkItem.GetParm("ContentType"), oWorkItem.GetParm("Fieldname")));
                                //
                                workCTX.Web.EnsureContentTypeHasField(oWorkItem.GetParm("ContentType"), oWorkItem.GetParm("FieldName"));
                                break;

                            case "remove-list":
                                string cListName = oWorkItem.GetParm("listname");
                                ShowProgress("Removing List: " + cListName);
                                //
                                workCTX.Web.RemoveList(cListName);
                                break;
                            case "remove-contenttype":
                                string cContentTypeName = oWorkItem.GetParm("contenttype");
                                ShowProgress("Removing Content Type: " + cContentTypeName);
                                //
                                workCTX.Web.RemoveContentType(cContentTypeName);
                                break;

                            case "remove-sitecolumnsbygroup":
                                string cGroupName = oWorkItem.GetParm("group");
                                ShowProgress("Removing Columns for Group: " + cGroupName);

                                workCTX.Web.RemoveFieldsForGroup(cGroupName);
                                break;
                            case "ensure-listandcontenttype":
                                EnsureListandContentType(workCTX, oWorkItem);
                                break;

                            case "ensure-contenttype":
                                ShowProgress("Working Content Type: " + oWorkItem.GetParm("Name"));

                                workCTX.Web.EnsureContentType(oWorkItem.GetParm("Name"), oWorkItem.GetParm("ParentName"), oWorkItem.GetParm("Group"));
                                break;
                            case "set-fielddefaultvalue":
                                ShowProgress("Working Default Value: " + oWorkItem.GetParm("fieldname"));


                                SetDefaultValue(workCTX, oWorkItem);

                                break;

                            case "save-template":
                                ShowProgress("Working Required Filed: " + oWorkItem.GetParm("fieldname"));


                                SavePNPTemplate(workCTX, oWorkItem);

                                break;



                            case "set-fieldrequired":
                                ShowProgress("Working Required Filed: " + oWorkItem.GetParm("fieldname"));


                                SetRequiredField(workCTX, oWorkItem);

                                break;


                            case "ensure-sitecolumnuser":
                                ShowProgress("Working Site Coloumn: " + oWorkItem.GetParm("Title"));

                                bool bMultiUser = oWorkItem.GetParmBool("multiuser");
                                bool bAllowGroups = oWorkItem.GetParmBool("alllowgroups");

                                workCTX.Web.EnsureSiteColumnUser(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"), bMultiUser, bAllowGroups);
                                break;
                            case "ensure-sitecolumn":
                                ShowProgress("Working Site Coloumn: " + oWorkItem.GetParm("Title"));

                                workCTX.Web.EnsureSiteColumn(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"));
                                break;

                            case "ensure-sitecolumninteger":
                                ShowProgress("Working Site Coloumn: " + oWorkItem.GetParm("Title"));

                                workCTX.Web.EnsureSiteColumnInteger(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"));
                                break;

                            case "ensure-sitecolumndatetime":
                                ShowProgress("Working Site Coloumn: " + oWorkItem.GetParm("Title"));

                                workCTX.Web.EnsureSiteColumnDateTime(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"));
                                break;
                            case "ensure-sitecolumncurrency":
                                ShowProgress("Working Site Coloumn: " + oWorkItem.GetParm("Title"));

                                workCTX.Web.EnsureSiteColumnCurrency(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"));
                                break;
                            case "ensure-sitecolumnchoice":
                                ShowProgress("Working Site Coloumn: " + oWorkItem.GetParm("Title"));

                                workCTX.Web.EnsureSiteColumnChoice(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("choices"));
                                break;
                            case "ensure-sitecolumnboolean":
                                ShowProgress("Working Site Coloumn: " + oWorkItem.GetParm("Title"));

                                workCTX.Web.EnsureSiteColumnBoolean(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"));
                                break;

                            case "ensure-sitecolumnnote":
                                ShowProgress("Working Site Coloumn: " + oWorkItem.GetParm("Title"));

                                workCTX.Web.EnsureSiteColumnNote(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"));
                                break;
                            case "ensure-sitecolumnnumber":
                                ShowProgress("Working Site Coloumn: " + oWorkItem.GetParm("Title"));

                                workCTX.Web.EnsureSiteColumnNumber(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"));
                                break;

                            case "ensure-sitecolumnlookup":
                                ShowProgress("Working Site Coloumn: " + oWorkItem.GetParm("displayname"));

                                workCTX.Web.EnsureSiteColumnLookup(oWorkItem.GetParm("internalname"), oWorkItem.GetParm("displayname"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"), oWorkItem.GetParm("ListName"), oWorkItem.GetParm("ShowField"));
                                break;



                            case "ensure-fieldinteger":
                            case "ensure-fielddatetime":
                            case "ensure-fieldcurrency":
                            case "ensure-fieldchoice":
                            case "ensure-fieldboolean":
                            case "ensure-fieldnnumber":
                            case "ensure-fieldnote":
                            case "ensure-field":
                                ShowProgress("Working Field: " + oWorkItem.GetParm("Title"));
                                Utility.EnsureListField(ctx, oWorkItem);
                                break;

                            case "find-unpublishedfiles":
                                FindUnpublishedFiles(item);
                                break;
                            case "find-uniqueid":
                                FindUniqueID(oWorkItem, item);
                                break;

                            case "update-homepagereferences":
                                UpdateHomePageReferences(item);

                                break;
                            case "upload-pagelayout":
                                UploadPageLayout(oWorkItem, item);
                                break;

                            case "update-sitephoto":
                            case "update-siteimage":
                                UpdateSiteImage(oWorkItem, item);
                                break;
                            case "RenameInternalGroups":
                                RenameInternalGroups(item);

                                break;

                            case "WalkSites":
                                WalkAllSites();
                                break;
                            case "ContentTypeFields":
                                ShowContentTypeFields("https://havertys.sharepoint.com/sites/contentTypeHub");
                                break;
                            case "UpdateSearch":
                                NavigationNodeCollection oSearchNav = workCTX.Web.LoadSearchNavigation();

                                //workCTX.Web.DeleteAllNavigationNodes(NavigationType.SearchNav);
                                //workCTX.ExecuteQuery();
                                workCTX.Web.SetPropertyBagValue("SRCH_ENH_FTR_URL_WEB", @"https://havertys.sharepoint.com/Sites/SearchCenter/Pages/results.aspx");
                                workCTX.Web.SetPropertyBagValue("SRCH_ENH_FTR_URL", @"https://havertys.sharepoint.com/Sites/SearchCenter/Pages/results.aspx");
                                workCTX.ExecuteQuery();

                                EnsureNavNode(workCTX, "Everything", "https://havertys.sharepoint.com/Sites/SearchCenter/Pages/results.aspx");
                                EnsureNavNode(workCTX, "KB", "https://havertys.sharepoint.com/Sites/SearchCenter/Pages/kbresults.aspx");
                                EnsureNavNode(workCTX, "Bulletin", "https://havertys.sharepoint.com/Sites/SearchCenter/Pages/bulletinresults.aspx");
                                RemoveNavNode(workCTX, "Warranty");
                                EnsureNavNode(workCTX, "Product", "https://havertys.sharepoint.com/Sites/SearchCenter/Pages/warrantyresults.aspx");
                                EnsureNavNode(workCTX, "Training", "https://havertys.sharepoint.com/Sites/SearchCenter/Pages/trainingresults.aspx");
                                EnsureNavNode(workCTX, "User Guides", "https://havertys.sharepoint.com/Sites/SearchCenter/Pages/ugresults.aspx");
                                EnsureNavNode(workCTX, "Directory", "https://havertys.sharepoint.com/Sites/SearchCenter/Pages/dirresults.aspx");

                                workCTX.ExecuteQuery();




                                break;

                            case "update-inventorylistitems":
                                UpdateInventoryListItems(workCTX, item, oWorkItem);
                                break;


                            case "update-inventory":
                                UpdateInventory(workCTX, item, oWorkItem);
                                break;

                            case "update-inventoryfromdatabase":
                                UpdateInventoryFromDatabase(workCTX, item, oWorkItem);
                                break;

                            default:
                                ShowProgress("Unrecognized command!" + oWorkItem.Command);
                                break;
                        }

                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "ProcessSites.WorkSite - " + item, "");
                    }

                }




            }
            catch (Exception ex)
            {
                ShowError(ex, "ProcessSites.WorkSite - " + item, "");
            }

        }

        private void DownloadImages(ClientContext workCTX, scriptItem oWorkItem)
        {
            List lstImages = workCTX.Web.Lists.GetByTitle(oWorkItem.GetParm("libraryname"));
            workCTX.Load(lstImages);

            ListItemCollection items = lstImages.GetAllItems();
            workCTX.Load(items, i => i.Include(itm => itm.File.ServerRelativeUrl, itm => itm.File.Name));
            workCTX.ExecuteQuery();

            string cLocalFolder = oWorkItem.GetParm("localfolder");
            if (!Directory.Exists(cLocalFolder))
            {
                Directory.CreateDirectory(cLocalFolder);
            }

            foreach (ListItem itm in items)
            {
                try
                {
                    // THIS IS THE LINE THAT CAUSES ISSUES!!!!!!!!
                    using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(workCTX, itm.File.ServerRelativeUrl))
                    {
                        // Combine destination folder with filename -- don't concatenate
                        // it's just wrong!
                        var filePath = Path.Combine(cLocalFolder, itm.File.Name);
                        // Erase existing files, cause that's how I roll
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        // Create the file
                        using (var fileStream = System.IO.File.Create(filePath))
                        {
                            fileInfo.Stream.CopyTo(fileStream);
                        }
                    }
                }
                catch (Exception ex)
                {

                }


            }


        }

        private void EnsureListLookup(ClientContext workCTX, scriptItem oWorkItem)
        {
            try
            {
                ShowProgress("Working Site Coloumn: " + oWorkItem.GetParm("displayname"));

                workCTX.Web.EnsureSiteColumnLookup(oWorkItem.GetParm("internalname"), oWorkItem.GetParm("displayname"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"), oWorkItem.GetParm("lookupList"), oWorkItem.GetParm("ShowField"));
                workCTX.Web.EnsureContentTypeHasField(oWorkItem.GetParm("ctName"), oWorkItem.GetParm("internalname"));

            }
            catch (Exception ex)
            {
                ShowError(ex, "EnsureListLookup", ex.Message);
            }
        }

        private void RemoveListAndContentType(ClientContext workCTX, scriptItem oWorkItem)
        {
            try
            {
                string cListName = oWorkItem.GetParm("listname");
                ShowProgress("Removing List: " + cListName);

                workCTX.Web.RemoveList(cListName);
                string cContentTypeName = oWorkItem.GetParm("ctname");
                ShowProgress("Removing Content Type: " + cContentTypeName);
                workCTX.Web.RemoveContentType(cContentTypeName);
            }
            catch (Exception ex)
            {
                ShowError(ex, "RemoveListAndContentType", ex.Message);
            }
        }

        private void SavePNPTemplate(ClientContext workCTX, scriptItem oWorkItem)
        {
            try
            {
                string appPath = @"C:\temp";
                string cTemplateName = oWorkItem.GetParm("template");
                var web = workCTX.Web;
                var ptci = new ProvisioningTemplateCreationInformation(web)
                {
                    IncludeHiddenLists = false,
                    IncludeAllClientSidePages = false,
                    PersistBrandingFiles = true,
                    IncludeSearchConfiguration = false,

                };
                ptci.MessagesDelegate += (msg, mtype) =>
                 {
                     ShowProgress($"   - {msg}");
                 };
                ptci.ProgressDelegate += (msg, step, total) =>
                {
                    ShowProgress($"{step}|{total} - {msg}");
                };
                var template = web.GetProvisioningTemplate(ptci);
                var provider = new XMLFileSystemTemplateProvider(appPath, "");
                provider.SaveAs(template, cTemplateName);
            }
            catch (Exception ex)
            {
                ShowError(ex, "SavePNPTemplate", "");
            }

        }
        private void EnsureListandContentType(ClientContext workCTX, scriptItem oWorkItem)
        {
            try
            {
                ShowProgress("Working Content Type: " + oWorkItem.GetParm("ctname"));
                workCTX.Web.EnsureContentType(oWorkItem.GetParm("ctName"), oWorkItem.GetParm("ctParent"), oWorkItem.GetParm("Group"));
                ShowProgress("Working List: " + oWorkItem.GetParm("ListName"));
                ListTemplateType oListTemplateType = GetTemplateType(oWorkItem.GetParm("ListType"));
                workCTX.Web.EnsureList(oWorkItem.GetParm("ListName"), oListTemplateType, oWorkItem.GetParm("Description"));
                ShowProgress(string.Format("Ensure Content Type In List: {0} - {1}", oWorkItem.GetParm("ListName"), oWorkItem.GetParm("ctName")));
                List lstWork = workCTX.Web.Lists.GetByTitle(oWorkItem.GetParm("ListName"));
                workCTX.Load(lstWork);
                workCTX.ExecuteQuery();
                lstWork.EnsureListHasContenttype(workCTX.Site, oWorkItem.GetParm("ctName"));
                ShowProgress(string.Format("Remove Content Type From List: {0} - {1}", oWorkItem.GetParm("ListName"), oWorkItem.GetParm("ContentType")));
                lstWork.RemoveContentTypeFromList(oWorkItem.GetParm("ctParent"));
                ShowProgress(string.Format("Ensure Content Type In List: {0} - {1}", oWorkItem.GetParm("ListName"), oWorkItem.GetParm("ContentType")));
                lstWork.DisableContentTypes();

            }
            catch (Exception ex)
            {
                ShowError(ex, "EnsureListandContentType", ex.Message);

            }
        }
        private void SetDefaultValue(ClientContext workCTX, scriptItem oWorkItem)
        {
            try
            {
                List lstWork = workCTX.Web.Lists.GetByTitle(oWorkItem.GetParm("listname"));
                Field fld = lstWork.Fields.GetByInternalNameOrTitle(oWorkItem.GetParm("fieldname"));
                workCTX.Load(fld, f => f.Required);
                workCTX.ExecuteQuery();
                string cDefault = oWorkItem.GetParm("defaultvalue");

                fld.DefaultValue = cDefault;
                fld.Update();
                workCTX.ExecuteQuery();

            }
            catch (Exception ex)
            {
                ShowError(ex, "SetDefaultValue", "");
            }

        }

        private void SetRequiredField(ClientContext workCTX, scriptItem oWorkItem)
        {
            try
            {
                List lstWork = workCTX.Web.Lists.GetByTitle(oWorkItem.GetParm("listname"));
                Field fld = lstWork.Fields.GetByInternalNameOrTitle(oWorkItem.GetParm("fieldname"));
                workCTX.Load(fld, f => f.Required);
                workCTX.ExecuteQuery();
                bool bRequired = oWorkItem.GetParmBool("required");
                if (fld.Required != bRequired)
                {
                    fld.Required = bRequired;
                    fld.Update();
                    workCTX.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "SetRequiredField", "");
            }
        }

        private void SetSiteReadOnly()
        {
            SetWebReadOnly(ctx, ctx.Web);

        }

        private void SetWebReadOnly(ClientContext workCTX, Web web)
        {
            WebCollection webs = web.Webs;
            workCTX.Load(webs, w => w.Include(wi => wi.HasUniqueRoleAssignments, wi => wi.Url));
            workCTX.ExecuteQuery();
            foreach (Web childweb in webs)
            {

                ShowInfo("Working: " + childweb.Url);
                SetWebReadOnly(ctx, childweb);
            }
            ListCollection lists = web.Lists;
            workCTX.Load(lists, lst => lst.Include(l => l.Title, l => l.HasUniqueRoleAssignments, l => l.Hidden, l => l.Title));
            workCTX.ExecuteQuery();
            foreach (List list in lists)
            {
                if (!list.Hidden && list.Title != "MicroFeed")
                {
                    ShowInfo("Working Items: " + list.Title);
                    SetListReadOnly(ctx, list);



                    if (list.HasUniqueRoleAssignments)
                    {
                        ShowInfo("Working List: " + list.Title);
                        MakeListReadOnly(workCTX, list);
                    }
                }
            }

            if (web.HasUniqueRoleAssignments)
            {
                MakeWebReadOnly(workCTX, web);
            }




        }

        private void MakeWebReadOnly(ClientContext workCTX, Web web)
        {
            try
            {
                ShowProgress("Working Web: " + web.Url);

                RoleAssignmentCollection rac = web.RoleAssignments;
                workCTX.Load(rac, ri => ri.Include(r => r.Member.PrincipalType, r => r.Member.LoginName));
                workCTX.ExecuteQuery();
                List<string> groups = new List<string>();
                List<string> users = new List<string>();
                List<RoleAssignment> removeItems = new List<RoleAssignment>();
                foreach (RoleAssignment ra in rac)
                {
                    if (ra.Member.PrincipalType == PrincipalType.SharePointGroup)
                    {
                        groups.Add(ra.Member.LoginName);
                    }
                    if (ra.Member.PrincipalType == PrincipalType.User)
                    {
                        users.Add(ra.Member.LoginName);
                    }
                    removeItems.Add(ra);
                }

                foreach (RoleAssignment roleAssignment in removeItems)
                {
                    roleAssignment.DeleteObject();
                }
                web.Update();
                workCTX.ExecuteQuery();
                foreach (string user in users)
                {
                    EnsureUserPermissionInWeb(workCTX, web, user, RoleType.Reader);
                }
                foreach (string group in groups)
                {
                    EnsureGroupPermissionInWeb(workCTX, web, group, RoleType.Reader);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "MakeWebReadOnly", "");
            }

        }






        private void MakeListReadOnly(ClientContext workCTX, List list)
        {
            try
            {
                RoleAssignmentCollection rac = list.RoleAssignments;
                workCTX.Load(rac, ri => ri.Include(r => r.Member.PrincipalType, r => r.Member.LoginName));
                workCTX.ExecuteQuery();
                List<string> groups = new List<string>();
                List<string> users = new List<string>();
                List<RoleAssignment> removeItems = new List<RoleAssignment>();
                foreach (RoleAssignment ra in rac)
                {
                    if (ra.Member.PrincipalType == PrincipalType.SharePointGroup)
                    {
                        groups.Add(ra.Member.LoginName);
                    }
                    if (ra.Member.PrincipalType == PrincipalType.User)
                    {
                        users.Add(ra.Member.LoginName);
                    }
                    removeItems.Add(ra);
                }

                foreach (RoleAssignment roleAssignment in removeItems)
                {
                    roleAssignment.DeleteObject();
                }
                list.Update();
                workCTX.ExecuteQuery();
                foreach (string user in users)
                {
                    EnsureUserPermissionInList(workCTX, list, user, RoleType.Reader);
                }
                foreach (string group in groups)
                {
                    EnsureGroupPermissionInList(workCTX, list, group, RoleType.Reader);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "MakeListReadOnly", "");
            }

        }

        private void MakeItemReadOnly(ClientContext workCTX, ListItem li, string cListTitle)
        {
            try
            {

                RoleAssignmentCollection rac = li.RoleAssignments;
                workCTX.Load(rac, ri => ri.Include(r => r.Member.PrincipalType, r => r.Member.LoginName));
                workCTX.ExecuteQuery();
                List<string> groups = new List<string>();
                List<string> users = new List<string>();
                List<RoleAssignment> removeItems = new List<RoleAssignment>();
                foreach (RoleAssignment ra in rac)
                {
                    if (ra.Member.PrincipalType == PrincipalType.SharePointGroup)
                    {
                        groups.Add(ra.Member.LoginName);
                    }
                    if (ra.Member.PrincipalType == PrincipalType.User)
                    {
                        users.Add(ra.Member.LoginName);
                    }
                    removeItems.Add(ra);
                }

                foreach (RoleAssignment roleAssignment in removeItems)
                {
                    roleAssignment.DeleteObject();
                }
                li.SystemUpdate();

                workCTX.ExecuteQuery();
                foreach (string user in users)
                {
                    EnsureUserPermissionInItem(workCTX, li, user, RoleType.Reader);
                }
                foreach (string group in groups)
                {
                    EnsureGroupPermissionInItem(workCTX, li, group, RoleType.Reader);
                }


            }
            catch (Exception ex)
            {
                ShowError(ex, "MakeItemReadOnly", "");
            }

        }

        private void SetListReadOnly(ClientContext workCTX, List list)
        {
            CamlQuery oQuery = CamlQuery.CreateAllItemsQuery();
            ListItemCollection items = list.GetItems(oQuery);
            workCTX.Load(items, itms => itms.Include(i => i.Id, i => i.HasUniqueRoleAssignments));
            workCTX.ExecuteQuery();
            foreach (ListItem li in items)
            {
                if (li.HasUniqueRoleAssignments)
                {
                    MakeItemReadOnly(workCTX, li, list.Title);
                }
            }

        }



        private void FindEveryOne(ClientContext workCTX)
        {
            try
            {
                System.Threading.Thread.Sleep(3000);
                GroupCollection oGroups = workCTX.Site.RootWeb.SiteGroups;
                workCTX.Load(workCTX.Site, itm => itm.Url);
                workCTX.ExecuteQuery();

                workCTX.Load(oGroups, grps => grps.Include(grp => grp.Title));
                workCTX.ExecuteQuery();
                foreach (var group in oGroups)
                {
                    bool bComplete = false;
                    while (!bComplete)
                    {
                        try
                        {
                            UserCollection oUsers = group.Users;
                            workCTX.Load(oUsers, usrs => usrs.Include(usr => usr.Title));
                            workCTX.ExecuteQuery();
                            foreach (var user in oUsers)
                            {
                                if (user.Title.Contains("Everyone") && !group.Title.Contains("Style Resource"))
                                {
                                    ShowProgress(workCTX.Site.Url + " | " + group.Title + " | contains the user Everyone");
                                    break;
                                }
                            }
                            bComplete = true;
                        }
                        catch (Exception ex)
                        {
                            ShowError(ex, "FindEveryOne", ex.Message);
                            System.Threading.Thread.Sleep(5000);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "FindEveryOne", ex.Message);
                System.Threading.Thread.Sleep(5000);
                FindEveryOne(workCTX);
            }

        }

        private void FindEveryOne(ClientContext workCTX, scriptItem oWorkItem)
        {
            throw new NotImplementedException();
        }

        Int32 iRequestCount = 0;

        private void PollAccessRequests(scriptItem workItem)
        {
            try
            {
                string saveUrl = workItem.GetParm("SaveContext");
                DateTime dtStart = DateTime.Now;
                ShowProgress("Polling Access Requests");
                ClientContext workCTX = new ClientContext(saveUrl);
                workCTX.Credentials = ctx.Credentials;
                workCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                List lstSites = workCTX.Web.Lists.GetByTitle("spmiSites");
                CamlQuery oQuery = new CamlQuery();
                oQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='UniquePermissions' /><Value Type='Boolean'>1</Value></Eq></Where></Query></View>";
                ListItemCollection items = lstSites.GetItems(oQuery);
                workCTX.Load(items, i => i.Include(itm => itm["SiteUrl"]));
                workCTX.ExecuteQuery();
                foreach (ListItem listItem in items)
                {
                    try
                    {

                        ShowInfo(listItem["SiteUrl"].ToString());
                        WalkAccessRequests(listItem["SiteUrl"].ToString());
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "", "");
                    }
                }
                TimeSpan ts = DateTime.Now.Subtract(dtStart);
                ShowProgress("Processing " + iRequestCount + " took: " + ts.TotalMinutes.ToString());

            }
            catch (Exception ex)
            {
                ShowError(ex, "PollAccessRequests", "");
            }
        }

        private void WalkAccessRequests(string cSiteUrl)
        {
            try
            {
                ClientContext siteCTX = new ClientContext(cSiteUrl);
                siteCTX.Credentials = ctx.Credentials;
                siteCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                if (siteCTX.Web.HasList("Access Requests"))
                {

                    CamlQuery oQuery = CamlQuery.CreateAllItemsQuery();
                    List lstAccessRequests = siteCTX.Web.Lists.GetByTitle("Access Requests");
                    //FieldCollection flds = lstAccessRequests.Fields;
                    //siteCTX.Load(flds, fs => fs.Include(f => f.InternalName));
                    //siteCTX.ExecuteQuery();
                    //foreach (Field fld in flds)
                    //{
                    //    System.Diagnostics.Trace.WriteLine(fld.InternalName);
                    //}
                    ListItemCollection items = lstAccessRequests.GetItems(oQuery);
                    siteCTX.Load(items, li => li.Include(itm => itm["RequestId"],
                                                            itm => itm["Title"],
                                                            itm => itm["RequestedFor"],
                                                            itm => itm["RequestedForDisplayName"],
                                                            itm => itm["RequestedForUserId"],
                                                            itm => itm["RequestedWebId"],
                                                            itm => itm["RequestedListId"],
                                                            itm => itm["RequestedListItemId"]));
                    siteCTX.ExecuteQuery();
                    foreach (ListItem listItem in items)
                    {
                        try
                        {
                            string cRequestType = "Web";
                            string cWebUrl = "";
                            string cListName = "";
                            string WebId = listItem["RequestedWebId"].ToString();
                            string ListId = listItem["RequestedListId"].ToString();
                            string ListItemId = listItem["RequestedListItemId"].ToString();


                            Guid gWeb = new Guid(WebId);
                            Web oSourceWeb = siteCTX.Site.OpenWebById(gWeb);
                            siteCTX.Load(oSourceWeb, sw => sw.Url);
                            siteCTX.ExecuteQuery();
                            cWebUrl = oSourceWeb.Url;
                            string cRequestedFor = listItem["RequestedFor"].ToString();
                            string cUserName = cRequestedFor.Substring(cRequestedFor.LastIndexOf("|") + 1);
                            if (!ListId.Contains("0000"))
                            {
                                Guid gList = new Guid(ListId);
                                List lstRequested = oSourceWeb.Lists.GetById(gList);
                                siteCTX.Load(lstRequested);
                                siteCTX.ExecuteQuery();
                                cListName = lstRequested.Title;
                                cRequestType = "List";
                            }
                            if (!ListItemId.Contains("0000"))
                            {
                                cRequestType = "Item";
                            }

                            User oRequestedFor = null;

                            var result = Microsoft.SharePoint.Client.Utilities.Utility.ResolvePrincipal(ctx, ctx.Web, cUserName, Microsoft.SharePoint.Client.Utilities.PrincipalType.User, Microsoft.SharePoint.Client.Utilities.PrincipalSource.All, null, true);
                            ctx.ExecuteQuery();
                            if (result != null)
                            {
                                oRequestedFor = ctx.Web.EnsureUser(result.Value.LoginName);
                                ctx.Load(oRequestedFor);
                                ctx.ExecuteQuery();
                            }


                            //System.Diagnostics.Trace.WriteLine("Requested For:" + listItem["RequestedFor"].ToString());
                            //System.Diagnostics.Trace.WriteLine("Requested For DisplayName:" + listItem["RequestedForDisplayName"].ToString());
                            //System.Diagnostics.Trace.WriteLine("Requested For User Id" + listItem["RequestedForUserId"].ToString());
                            //System.Diagnostics.Trace.WriteLine("RequestId: " + listItem["RequestId"].ToString());
                            ////System.Diagnostics.Trace.WriteLine("RequestedListId" + listItem["RequestedListId"].ToString());
                            //System.Diagnostics.Trace.WriteLine("RequestedListItemId:" + listItem["RequestedListItemId"].ToString());
                            string cRequestId = listItem["RequestId"].ToString();
                            List lstAllAccessRequests = ctx.Web.Lists.GetByTitle("spmiAccessRequests");
                            ListItem liAccessRequest = GetRequestItem(ctx, cRequestId);
                            if (liAccessRequest == null)
                            {
                                ListItemCreationInformation lici = new ListItemCreationInformation();
                                liAccessRequest = lstAllAccessRequests.AddItem(lici);

                                liAccessRequest["Title"] = cRequestId;
                                //liAccessRequest.Update();
                                //ctx.ExecuteQuery();
                                liAccessRequest["SiteUrl"] = cWebUrl;
                                if (!String.IsNullOrEmpty(cListName))
                                {
                                    liAccessRequest["ListName"] = cListName;
                                }
                                if (!ListItemId.Contains("0000"))
                                {
                                    liAccessRequest["ListItemID"] = ListItemId;
                                }


                                liAccessRequest["RequestFor"] = oRequestedFor;
                                liAccessRequest["AccessRequestType"] = cRequestType;

                                liAccessRequest.Update();
                                ctx.ExecuteQuery();

                            }

                            iRequestCount++;
                        }
                        catch (Exception ex)
                        {
                            ShowError(ex, "", "");
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                ShowError(ex, "WalkAccessRequests: " + cSiteUrl, "");
            }
        }

        private ListItem GetRequestItem(ClientContext ctx, string cGuid)
        {
            try
            {
                List lstAllAccessRequests = ctx.Web.Lists.GetByTitle("spmiAccessRequests");
                string uilQuery = "<View>><Where><Eq><FieldRef Name='Title'/><Value Type='Text'>" + cGuid + "</Value></Eq></Where></Query></View>";
                CamlQuery oQuery = new CamlQuery();
                oQuery.ViewXml = uilQuery;
                ListItemCollection itms = lstAllAccessRequests.GetItems(oQuery);
                ctx.Load(itms);
                ctx.ExecuteQuery();
                foreach (ListItem listItem in itms)
                {
                    if (listItem["Title"].ToString().ToLower() == cGuid.ToLower())
                    {
                        return listItem;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "GetRequestItem - " + cGuid, "");
            }
            return null;
        }

        private void EnableAccessRequestSearch(ClientContext workCTX, string item, scriptItem oWorkItem)
        {
            EnableARSearch(workCTX, workCTX.Web);
        }

        private void EnableARSearch(ClientContext workCTX, Web web)
        {
            try
            {
                workCTX.Load(web, ww => ww.Url);
                workCTX.ExecuteQuery();
                ShowInfo(web.Url);
                WebCollection oWebs = web.Webs;
                workCTX.Load(oWebs);
                workCTX.ExecuteQuery();
                foreach (Web childweb in oWebs)
                {
                    EnableARSearch(workCTX, childweb);
                }



                if (web.HasList("Access Requests"))
                {
                    ShowProgress(web.Url);
                    //List lstAccessRequests = web.Lists.GetByTitle("Access Requests");
                    //lstAccessRequests.NoCrawl = false;
                    //lstAccessRequests.ReIndexList();
                    //lstAccessRequests.Update();
                    //workCTX.ExecuteQuery();

                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "EnableARSearch", "");
            }
        }

        private void FixEncryptedText(ClientContext workCTX, string item, BackgroundWorker oWorker)
        {
            FixEncryptedText(workCTX, workCTX.Web);
        }

        private void FixEncryptedText(ClientContext workCTX, Web oWorkWeb)
        {

            try
            {
                workCTX.Load(oWorkWeb, ww => ww.Url);
                workCTX.ExecuteQuery();
                ShowInfo(oWorkWeb.Url);
                WebCollection oWebs = oWorkWeb.Webs;
                workCTX.Load(oWebs);
                workCTX.ExecuteQuery();
                foreach (Web web in oWebs)
                {
                    FixEncryptedText(workCTX, web);
                }


                List libSiteAssets = workCTX.Web.Lists.GetByTitle("Site Assets");
                Folder oWorkFolder = libSiteAssets.RootFolder;
                Microsoft.SharePoint.Client.File oWorkFile = oWorkFolder.GetFileByName("EncryptedText.html");
                if (oWorkFile != null)
                {
                    string fileName = "EncryptedText.html";

                    using (var fs = new FileStream(@"c:\temp\EncryptedText.html", FileMode.Open))
                    {
                        var fi = new FileInfo(fileName);

                        workCTX.Load(libSiteAssets.RootFolder);
                        workCTX.ExecuteQuery();
                        var fileUrl = String.Format("{0}/{1}", libSiteAssets.RootFolder.ServerRelativeUrl, fi.Name);

                        Microsoft.SharePoint.Client.File.SaveBinaryDirect(workCTX, fileUrl, fs, true);
                    }






                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "FixEncryptedText: " + oWorkWeb.Url, "");
            }



        }

        private void FindCustomizations(ClientContext workCTX, string item, scriptItem oWorkItem)
        {
            ShowInfo(item);
            WebCollection oWebs = workCTX.Web.Webs;
            workCTX.Load(oWebs);
            workCTX.ExecuteQuery();
            foreach (var web in oWebs)
            {
                FindSiteCustimizations(workCTX, web);
            }

        }

        private void FindSiteCustimizations(ClientContext workCTX, Web oWorkWeb)
        {
            try
            {
                //workCTX.Load(oWorkWeb, ww => ww.Url);
                //workCTX.ExecuteQuery();
                WebCollection oWebs = oWorkWeb.Webs;
                workCTX.Load(oWebs);
                workCTX.ExecuteQuery();
                foreach (var web in oWebs)
                {
                    FindSiteCustimizations(workCTX, web);
                }

                if (oWorkWeb.HasList("Site Assets"))
                {
                    List lstSiteAssets = oWorkWeb.Lists.GetByTitle("Site Assets");
                    Folder oRootFolder = lstSiteAssets.RootFolder;
                    workCTX.Load(oRootFolder);
                    workCTX.ExecuteQuery();
                    List<Microsoft.SharePoint.Client.File> ofiles = GetFiles(workCTX, oRootFolder);
                    foreach (var file in ofiles)
                    {
                        workCTX.Load(file, ff => ff.ServerRelativeUrl);
                        workCTX.ExecuteQuery();
                        ShowProgress(file.ServerRelativeUrl);

                    }

                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "FindSiteCustomizations", "");
            }

        }

        private List<Microsoft.SharePoint.Client.File> GetFiles(ClientContext workCTX, Folder oWorkFolder)
        {
            List<Microsoft.SharePoint.Client.File> oJSFiles = new List<Microsoft.SharePoint.Client.File>();
            try
            {
                FolderCollection ofolders = oWorkFolder.Folders;
                workCTX.Load(ofolders);
                workCTX.ExecuteQuery();
                foreach (var folder in ofolders)
                {
                    List<Microsoft.SharePoint.Client.File> oChildFiles = GetFiles(workCTX, folder);
                    foreach (var file in oChildFiles)
                    {

                        oJSFiles.Add(file);
                    }
                }

                FileCollection oFiles = oWorkFolder.Files;
                workCTX.Load(oFiles);
                workCTX.ExecuteQuery();
                foreach (var file in oFiles)
                {
                    if (file.Name.ToLower().Contains(".js") || file.Name.ToLower().Contains(".htm"))
                    {
                        if (file.Name != "EncryptedText.html")
                        {
                            oJSFiles.Add(file);
                        }

                    }
                }



            }
            catch (Exception ex)
            {
                ShowError(ex, "GetFiles", "");
            }

            return oJSFiles;
        }

        private void UpdateUserInformation(scriptItem workItem)
        {
            try
            {
                string saveUrl = workItem.GetParm("SaveContext");
                ClientContext workCTX = new ClientContext(saveUrl);
                workCTX.Credentials = ctx.Credentials;
                workCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                string uilQuery = "<View><OrderBy Override='TRUE' ><FieldRef Name = 'ID' /></OrderBy><Query><Where><Eq><FieldRef Name='Title'/><Value Type='Text'>User Information List</Value></Eq></Where></Query><RowLimit>1500</RowLimit></View>";
                CamlQuery oQuery = new CamlQuery();
                oQuery.ViewXml = uilQuery;
                List lstLists = workCTX.Web.Lists.GetByTitle("spmiLists");
                ListItemCollection itms = lstLists.GetItems(oQuery);
                List lstSites = workCTX.Web.Lists.GetByTitle("spmiSites");
                workCTX.Load(itms, i => i.Include(itm => itm["SiteID"]));
                workCTX.ExecuteQuery();

                foreach (var listItem in itms)
                {
                    try
                    {
                        int SiteID = Convert.ToInt32(listItem["SiteID"].ToString());
                        ListItem itmSite = lstLists.GetItemById(SiteID);
                        workCTX.Load(itmSite, i => i["SiteUrl"]);
                        workCTX.ExecuteQuery();

                        ProcessUserList(workCTX, itmSite["SiteUrl"].ToString());
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "UpdateUserInformation - Site Doesn't Exist!", "");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "UpdateUserInformation", "");
            }
        }

        private void ProcessUserList(ClientContext workCTX, string cSiteUrl)
        {
            try
            {
                string peopleQuery = "<View><Query><Where><BeginsWith><FieldRef Name='ContentTypeId' /><Value Type='Text'>0x010A00C551C88244FFAC438415E970D4A5B281</Value></BeginsWith></Where></ Query><RowLimit>5000</RowLimit></View>";
                ShowProgress(cSiteUrl);
                ClientContext ulContext = new ClientContext(cSiteUrl);
                ulContext.Credentials = workCTX.Credentials;
                ulContext.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                List lstUserInfo = ulContext.Web.Lists.GetByTitle("User Information List");
                CamlQuery oQuery = new CamlQuery();
                oQuery.ViewXml = peopleQuery;
                ListItemCollection itms = lstUserInfo.GetItems(oQuery);
                ulContext.Load(itms);
                ulContext.ExecuteQuery();

                foreach (var itm in itms)
                {
                    ulContext.Load(itm);
                    ulContext.ExecuteQuery();
                    System.Diagnostics.Trace.WriteLine(itm["Title"]);
                }






            }
            catch (Exception ex)
            {
                ShowError(ex, "ProcessUserList", "");

            }
        }

        private void UpdateSitePermissions(scriptItem workItem)
        {
            string saveUrl = workItem.GetParm("SaveContext");
            ClientContext workCTX = new ClientContext(saveUrl);
            workCTX.Credentials = ctx.Credentials;
            workCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
            {
                e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
            };


            string viewXML = "<View><Query><Where><Eq><FieldRef Name = 'spmiUniquePermissions' /><Value Type = 'Boolean'>1</Value></Eq></Where></Query><RowLimit>5000</RowLimit></View>";
            try
            {

                List lst = workCTX.Web.Lists.GetByTitle("spmiSites");

                workCTX.Load(lst);
                workCTX.ExecuteQuery();
                CamlQuery oQuery = new CamlQuery();
                oQuery.ViewXml = viewXML;
                ListItemCollection items = lst.GetItems(oQuery);
                workCTX.Load(items, its => its.Include(i => i["spmiSiteUrl"], i => i.Id, i => i["spmiPermissions"]));
                sw.Start();
                workCTX.ExecuteQuery();
                TotalItems = items.Count;
                foreach (var lstItem in items)
                {
                    try
                    {
                        RunCount++;
                        ShowInfo(RunCount + " of " + TotalItems);
                        string cSiteURL = lstItem["spmiSiteUrl"].ToString();

                        ShowInfo("Processing Permissions for:" + cSiteURL);
                        string cPermissions = GetPermissions(cSiteURL, workCTX);
                        ListItem saveItem = GetSiteItemToUpdate(workCTX, cSiteURL);
                        saveItem["spmiPermissions"] = cPermissions;
                        saveItem["spmiPermissionsLastScan"] = DateTime.Now;
                        saveItem.Update();
                        workCTX.ExecuteQuery();

                        UpdateListPermissions(workCTX, cSiteURL, lstItem.Id);
                        UpdateListItemPermissions(workCTX, cSiteURL, lstItem.Id);

                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "Update Site Permissions", "");
                    }


                }
                sw.Stop();
            }
            catch (Exception ex)
            {
                ShowError(ex, "Update Site Permissions", "");
            }

        }

        private void UpdateListItemPermissions(ClientContext workCTX, string cSiteUrl, int iParentID)
        {
            try
            {

                ClientContext webCTX = new ClientContext(cSiteUrl);
                webCTX.Credentials = ctx.Credentials;
                webCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };


                string viewXML = "<View><Query><Where><Eq><FieldRef Name = 'spmiSiteID' /><Value Type = 'Number'>" + iParentID + "</Value></Eq></Where></Query><RowLimit>5000</RowLimit></View>";

                List lstSave = workCTX.Web.Lists.GetByTitle("spmiItems");
                workCTX.Load(lstSave);

                List lst = workCTX.Web.Lists.GetByTitle("spmiLists");
                workCTX.Load(lst);
                workCTX.ExecuteQuery();
                CamlQuery oQuery = new CamlQuery();
                oQuery.ViewXml = viewXML;
                ListItemCollection items = lst.GetItems(oQuery);
                workCTX.Load(items, itms => itms.Include(itm => itm["spmiPermissions"], itm => itm["Title"], itm => itm.Id));


                workCTX.ExecuteQuery();
                foreach (var lstItem in items)
                {
                    try
                    {
                        string cList = lstItem["Title"].ToString();
                        ShowInfo("Processing Item Permissions for:" + cList);
                        List itmLst = webCTX.Web.Lists.GetByTitle(cList);
                        webCTX.Load(itmLst, il => il.Id);
                        webCTX.ExecuteQuery();
                        CamlQuery oItemQuery = CamlQuery.CreateAllItemsQuery();
                        ListItemCollection permItems = itmLst.GetItems(oItemQuery);
                        webCTX.Load(permItems, itms => itms.Include(pi => pi["GUID"], pi => pi.HasUniqueRoleAssignments, pi => pi.Id));
                        webCTX.ExecuteQuery();
                        foreach (var listItem in permItems)
                        {
                            if (listItem.HasUniqueRoleAssignments)
                            {
                                webCTX.Load(listItem);
                                webCTX.ExecuteQuery();
                                ShowInfo(listItem.Id + " has unique permissions");
                                string cItemPermissions = GetListItemPermissions(webCTX, listItem);
                                SaveItemPermissions(workCTX, cList, lstSave, lstItem.Id, listItem, cItemPermissions);
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "Update List Item Permissions", "");
                    }


                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "Update Site Permissions", "");
            }
        }

        private void SaveItemPermissions(ClientContext workCTX, string cSourceList, List saveList, int listID, ListItem itm, string cItemPermissions)
        {
            try
            {
                string itmGuid = itm["GUID"].ToString();
                string viewXML = "<View><Query><Where><Eq><FieldRef Name = 'Title' /><Value Type = 'Text'>" + itmGuid + "</Value></Eq></Where></Query><RowLimit>5000</RowLimit></View>";
                List lst = workCTX.Web.Lists.GetByTitle("spmiItems");
                workCTX.Load(lst);
                workCTX.ExecuteQuery();
                CamlQuery oQuery = new CamlQuery();
                oQuery.ViewXml = viewXML;
                ListItemCollection items = lst.GetItems(oQuery);
                workCTX.Load(items);
                workCTX.ExecuteQuery();
                foreach (var item in items)
                {
                    item["Permissions"] = cItemPermissions;
                    item.Update();
                    workCTX.ExecuteQuery();
                    return;
                }
                ListItemCreationInformation lici = new ListItemCreationInformation();
                ListItem newitem = saveList.AddItem(lici);
                newitem["ListTitle"] = cSourceList;
                newitem["ListID"] = listID;
                newitem["Title"] = itm["GUID"].ToString();
                newitem["ItemID"] = itm.Id;
                newitem["Permissions"] = cItemPermissions;
                newitem.Update();
                workCTX.ExecuteQuery();
            }
            catch (Exception ex)
            {
                ShowError(ex, "Save Item Permission", "");
            }
        }

        private void UpdateListPermissions(ClientContext workCTX, string cSiteUrl, int iParentID)
        {
            try
            {

                ClientContext webCTX = new ClientContext(cSiteUrl);
                webCTX.Credentials = ctx.Credentials;
                webCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                string viewXML = "<View><Query><Where><And><Eq><FieldRef Name = 'spmiUniquePermissions' /><Value Type = 'Boolean'>1</Value></Eq><Eq><FieldRef Name = 'spmiSiteID' /><Value Type = 'Number'>" + iParentID + "</Value></Eq></And></Where></Query><RowLimit>5000</RowLimit></View>";
                viewXML = "<View><Query><Where><Eq><FieldRef Name = 'spmiSiteID' /><Value Type = 'Text'>" + iParentID + "</Value></Eq></Where></Query><RowLimit>5000</RowLimit></View>";
                List lst = workCTX.Web.Lists.GetByTitle("spmiLists");
                workCTX.Load(lst);
                workCTX.ExecuteQuery();
                CamlQuery oQuery = new CamlQuery();
                oQuery.ViewXml = viewXML;
                ListItemCollection items = lst.GetItems(oQuery);
                workCTX.Load(items, itms => itms.Include(itm => itm["spmiPermissions"], itm => itm["Title"]));


                workCTX.ExecuteQuery();
                foreach (var lstItem in items)
                {
                    try
                    {
                        ShowProgress("Processing Permissions for:" + lstItem["Title"].ToString());
                        if (lstItem["spmiPermissions"] == null)
                        {
                            string cPermissions = GetListPermissions(webCTX, lstItem["Title"].ToString());
                            lstItem["spmiPermissions"] = cPermissions;
                            lstItem["spmiPermissionsLastScan"] = DateTime.Now;
                            lstItem.Update();
                            workCTX.ExecuteQuery();
                        }

                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "Update List Permissions", "");
                    }


                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "Update Site Permissions", "");
            }

        }

        private string GetBasePermissions(ClientContext workCTX, RoleAssignmentCollection roles)
        {
            try
            {
                PermItemSet oPermSet = new PermItemSet();
                oPermSet.Groups = new List<PermItem>();
                oPermSet.Users = new List<PermItem>();

                foreach (RoleAssignment item in roles)
                {
                    workCTX.Load(item, i => i.Member);
                    workCTX.ExecuteQuery();
                    RoleDefinitionBindingCollection roledefs = item.RoleDefinitionBindings;
                    workCTX.Load(roledefs);
                    workCTX.ExecuteQuery();
                    foreach (RoleDefinition roledef in roledefs)
                    {
                        try
                        {
                            if (!roledef.Name.Contains("Limited"))
                            {


                                if (item.Member.PrincipalType == PrincipalType.User)
                                {
                                    PermItem opi = new PermItem();
                                    opi.PermItemLevel = roledef.Name;

                                    User oUser = workCTX.Web.EnsureUser(item.Member.LoginName);
                                    workCTX.Load(oUser);
                                    workCTX.ExecuteQuery();

                                    opi.PermItemName = item.Member.Title + ":" + oUser.Email;
                                    opi.PermItemType = "Users";
                                    oPermSet.Users.Add(opi);
                                }
                                else
                                {
                                    PermItem opi = GetPermItem(workCTX, roledef.Name, item.Member.Title);
                                    if (opi != null)
                                    {
                                        oPermSet.Groups.Add(opi);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowError(ex, "Processing Role Permissions", "");
                        }
                    }
                }


                string retVal = JsonConvert.SerializeObject(oPermSet);

                System.Diagnostics.Trace.WriteLine(retVal);
                return retVal;

            }
            catch (Exception ex)
            {
                ShowError(ex, "GetBasePermissions", "");
            }
            return "";
        }

        private string GetListItemPermissions(ClientContext workCTX, ListItem itm)
        {
            try
            {
                RoleAssignmentCollection roles = itm.RoleAssignments;
                workCTX.Load(roles);
                workCTX.ExecuteQuery();
                return GetBasePermissions(workCTX, roles);

            }
            catch (Exception ex)
            {
                ShowError(ex, "GetItemPermissions", "");
            }
            return "";

        }

        private string GetListPermissions(ClientContext workCTX, string cListName)
        {
            try
            {

                List lst = workCTX.Web.Lists.GetByTitle(cListName);
                workCTX.Load(lst);
                RoleAssignmentCollection roles = lst.RoleAssignments;
                workCTX.Load(roles);
                workCTX.ExecuteQuery();
                return GetBasePermissions(workCTX, roles);
            }
            catch (Exception ex)
            {
                ShowError(ex, "GetPermissions - " + cListName, "");
            }
            return "";

        }

        private string GetPermissions(string cSiteUrl, ClientContext saveContext)
        {
            try
            {
                ClientContext ctxWorkSite = new ClientContext(cSiteUrl);
                ctxWorkSite.Credentials = ctx.Credentials;
                ctxWorkSite.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spcToolBelt/1.0";
                };
                RoleAssignmentCollection roles = ctxWorkSite.Web.RoleAssignments;
                ctxWorkSite.Load(roles);
                ctxWorkSite.ExecuteQuery();
                return GetBasePermissions(ctxWorkSite, roles);
            }
            catch (Exception ex)
            {
                ShowError(ex, "GetPermissions - " + cSiteUrl, "");
            }
            return "";
        }
        private void UpdateSitePermissions(ClientContext ctxWorkSite, ClientContext saveContext, string cSiteUrl, PermItemSet oPermSet)
        {
            List<FieldUserValue> oUsers = new List<FieldUserValue>();
            try
            {
                foreach (PermItem usrItem in oPermSet.Users)
                {
                    if (usrItem.PermItemLevel.ToUpper().Contains("FULL CONTROL"))
                    {
                        FieldUserValue oVal = BuildUsersValue(saveContext, usrItem);
                        if (oVal != null) oUsers.Add(oVal);

                    }
                }
                foreach (PermItem grpItem in oPermSet.Groups)
                {
                    if (grpItem.PermItemLevel.ToUpper().Contains("FULL CONTROL"))
                    {
                        foreach (PermItem usrItem in grpItem.Members)
                        {
                            if (usrItem.PermItemLevel.ToUpper().Contains("FULL CONTROL"))
                            {

                                FieldUserValue oVal = BuildUsersValue(saveContext, usrItem);
                                if (oVal != null) oUsers.Add(oVal);
                            }
                        }
                    }
                }
                ListItem oListItem = GetSiteItemToUpdate(saveContext, cSiteUrl);

                oListItem["FullControlUsers"] = oUsers;
                oListItem["FullControlUsersCount"] = oUsers.Count;

                oListItem.Update();
                saveContext.ExecuteQuery();

            }
            catch (Exception ex)
            {
                ShowError(ex, "UpdateSitePermissions = " + cSiteUrl, "");
            }
        }

        private FieldUserValue BuildUsersValue(ClientContext saveContext, PermItem usrItem)
        {
            try
            {
                if (usrItem.LoginName.ToLower().Contains("authority"))
                {
                    System.Diagnostics.Trace.WriteLine(" Where are we?");

                }
                User oWorkUser = saveContext.Web.EnsureUser(usrItem.LoginName);
                saveContext.Load(oWorkUser);
                saveContext.ExecuteQuery();
                if (oWorkUser.LoginName.ToLower().Contains("rolemanager"))
                {
                    return null;
                }
                if (oWorkUser.Title.ToLower().Contains("system account") || oWorkUser.Title.ToLower().Contains("rolemanager"))
                {
                    return null;
                }
                FieldUserValue oVal = new FieldUserValue();
                oVal.LookupId = oWorkUser.Id;

                return oVal;

            }
            catch (Exception ex)
            {
                ShowError(ex, "BuildUsersValue ", "");
            }
            return null;
        }

        private ListItem GetSiteItemToUpdate(ClientContext saveContext, string cSiteUrl)
        {
            ListItem itm = null;
            try
            {
                string viewXML = "<View><Query><Where><Eq><FieldRef Name = 'spmiSiteUrl' /><Value Type = 'Text'>" + cSiteUrl + "</Value></Eq></Where></Query><RowLimit>5</RowLimit></View>";

                List lst = saveContext.Web.Lists.GetByTitle("spmiSites");
                saveContext.Load(lst);
                saveContext.ExecuteQuery();
                CamlQuery oQuery = new CamlQuery();
                oQuery.ViewXml = viewXML;
                ListItemCollection items = lst.GetItems(oQuery);
                saveContext.Load(items);
                saveContext.ExecuteQuery();
                foreach (var lstItem in items)
                {
                    return lstItem;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "GetSiteItemToUpdate = " + cSiteUrl, "");

            }
            return itm;
        }

        private PermItem GetPermItem(ClientContext ctxWorkSite, string rolename, string title)
        {
            PermItem opi = new PermItem();
            opi.PermItemType = "Group";
            opi.PermItemLevel = rolename;
            opi.PermItemName = title;
            opi.Members = new List<PermItem>();
            try
            {

                Group oGroup = GetGroup(ctxWorkSite, title);
                if (oGroup != null)
                {
                    UserCollection grpUsers = oGroup.Users;
                    ctxWorkSite.Load(grpUsers);
                    ctxWorkSite.ExecuteQuery();
                    if (grpUsers.Count == 0) return null;
                    foreach (User grpUser in grpUsers)
                    {
                        PermItem upi = new PermItem();
                        upi.LoginName = grpUser.LoginName;
                        upi.PermItemLevel = rolename;
                        upi.PermItemName = grpUser.Title + ":" + grpUser.Email;
                        upi.PermItemType = "User";
                        opi.Members.Add(upi);
                    }
                }
                if (opi.Members == null) return null;
                if (opi.Members.Count > 0)
                {
                    return opi;
                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "GetPermItem = " + title, "");
            }
            return null;
        }

        private string GetMemberDetails(ClientContext ctxWorkSite, string title)
        {
            try
            {
                string cDetails = title + " = [";

                Group oGroup = GetGroup(ctxWorkSite, title);
                if (oGroup != null)
                {
                    UserCollection grpUsers = oGroup.Users;
                    ctxWorkSite.Load(grpUsers);
                    ctxWorkSite.ExecuteQuery();
                    foreach (User grpUser in grpUsers)
                    {
                        cDetails += grpUser.Title + ":" + grpUser.Email + ";";
                    }
                    cDetails += "]";
                    return cDetails;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "GetMemberDetails: " + title, "");
            }
            return string.Format("[{0}]", title);

        }

        private Group GetGroup(ClientContext ctxWorkSite, string title)
        {
            Web oRootWeb = ctxWorkSite.Site.RootWeb;
            ctxWorkSite.Load(oRootWeb);
            ctxWorkSite.ExecuteQuery();
            GroupCollection grps = ctxWorkSite.Site.RootWeb.SiteGroups;
            ctxWorkSite.Load(grps);
            ctxWorkSite.ExecuteQuery();
            foreach (Group grp in grps)
            {
                if (grp.Title == title) return grp;
            }
            return null;
        }


        private PermissionLevel GetPermLevel(List<PermissionLevel> oLevels, string title)
        {
            foreach (PermissionLevel oLevel in oLevels)
            {
                if (oLevel.Name == title) return oLevel;
            }
            PermissionLevel newLevel = new PermissionLevel(title, "");
            oLevels.Add(newLevel);
            return newLevel;
        }



        private void EnsureSiteGallery(ClientContext workCTX, string item, scriptItem oWorkItem)
        {
            try
            {
                string cWalkUrl = oWorkItem.GetParm("walkcontext");
                ClientContext walkContext = new ClientContext(cWalkUrl);
                walkContext.Credentials = workCTX.Credentials;
                walkContext.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                walkContext.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };


                walkContext.Load(walkContext.Web, ww => ww.Title, ww => ww.Url, ww => ww.Id);
                walkContext.ExecuteQuery();
                ShowInfo(walkContext.Web.Title + " - " + walkContext.Web.Url);
                ClientContext saveContext = new ClientContext(oWorkItem.GetParm("SaveContext"));
                saveContext.Credentials = workCTX.Credentials;
                saveContext.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                EnsureSiteGallery(walkContext, saveContext, walkContext.Web, -1, oWorkItem);
            }
            catch (Exception ex)
            {
                ShowError(ex, "EnsureSiteGallery -" + item, "");
            }

        }

        public bool IsSkipSite(scriptItem oWorkItem, string cUrl)
        {
            try
            {
                if (oWorkItem.parms.ContainsKey("skipkeys"))
                {
                    string cSkipKeys = oWorkItem.parms["skipkeys"];
                    if (!string.IsNullOrEmpty(cSkipKeys))
                    {
                        string[] aSkipKeys = cSkipKeys.Split(';');
                        foreach (var aSkipKey in aSkipKeys)
                        {
                            if (cUrl.ToLower().Contains(aSkipKey.ToLower()))
                            {
                                return true;
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "IsSkipSite: " + cUrl, "");
            }

            return false;
        }
        private void EnsureSiteGallery(ClientContext workCTX, ClientContext saveContext, Web oWorkWeb, int iParentID, scriptItem oWorkItem)
        {
            try
            {
                workCTX.Load(workCTX.Site, s => s.Url);
                workCTX.Load(oWorkWeb, ww => ww.RequestAccessEmail, ww => ww.Title, ww => ww.Url, ww => ww.Id, ww => ww.Webs, ww => ww.HasUniqueRoleAssignments, ww => ww.LastItemModifiedDate, ww => ww.LastItemUserModifiedDate);
                workCTX.ExecuteQuery();


                if (IsSkipSite(oWorkItem, oWorkWeb.Url))
                {
                    return;
                }
                ShowInfo(oWorkWeb.Title + " - " + oWorkWeb.Url);
                bool bHasChildren = oWorkWeb.Webs.Count > 0;

                int iUserCount = 0;

                if (oWorkWeb.HasList("User Information List"))
                {
                    iUserCount = GetUserCount(workCTX);
                }

                int iSiteID = EnsureSiteInGallery(saveContext, oWorkWeb.Title, oWorkWeb.Url, oWorkWeb.Id.ToString(), iParentID, bHasChildren, oWorkWeb.HasUniqueRoleAssignments, oWorkWeb.LastItemUserModifiedDate, iUserCount);
                if (iSiteID > 0)
                {
                    WebCollection oWebs = oWorkWeb.Webs;
                    workCTX.Load(oWebs, webs => webs.Include(ww => ww.Url));
                    workCTX.ExecuteQuery();
                    foreach (var web in oWebs)
                    {
                        try
                        {
                            string token = web.Url.Substring(0, web.Url.IndexOf("/", 10));
                            if (!token.Contains("-"))
                            {
                                EnsureSiteGallery(workCTX, saveContext, web, iSiteID, oWorkItem);
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowError(ex, "EnsureSiteGallery - Inside -" + oWorkWeb.Url, "");

                        }
                    }
                    ListCollection lists = oWorkWeb.Lists;

                    workCTX.Load(lists, lsts => lsts.Include(l => l.HasUniqueRoleAssignments, l => l.Hidden, l => l.Id, l => l.Title, l => l.ItemCount, l => l.LastItemModifiedDate, l => l.LastItemUserModifiedDate));
                    workCTX.ExecuteQuery();


                    foreach (var list in lists)
                    {
                        bool bSkip = false;
                        if (list.Hidden) bSkip = true;
                        if (list.ItemCount == 0) bSkip = true;
                        if (list.Title.ToLower() == "user information list") bSkip = true;
                        if (list.Title.ToLower() == "access requests") bSkip = true;
                        if (!bSkip)
                        {
                            int iListID = EnsureListInGallery(saveContext, iSiteID, list, oWorkWeb.Url);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "EnsureSiteGallery -" + oWorkWeb.Url, "");
            }
        }

        private int GetUserCount(ClientContext workCTX)
        {
            try
            {
                int iCount = 0;
                List lstUsers = workCTX.Site.RootWeb.Lists.GetByTitle("User Information List");
                workCTX.Load(lstUsers);
                ListItemCollection items = lstUsers.GetItems(CamlQuery.CreateAllItemsQuery());
                workCTX.Load(items, itms => itms.Include(itm => itm.FieldValuesAsText, itm => itm["Title"], itm => itm["EMail"]));
                workCTX.ExecuteQuery();
                foreach (var item in items)
                {
                    if (item["EMail"] != null)
                    {
                        iCount++;
                    }
                }
                return iCount;
            }
            catch (Exception ex)
            {
                ShowError(ex, "GetUserCount", "");
            }
            return -1;
        }

        private int EnsureListInGallery(ClientContext saveContext, int iSiteID, List list, string cSiteUrl)
        {
            string viewXML = "<View><Query><Where><Eq><FieldRef Name = 'spmiListID' /><Value Type = 'Text'>" + list.Id + "</Value></Eq></Where></Query><RowLimit>10</RowLimit></View>";
            try
            {

                List lst = saveContext.Web.Lists.GetByTitle("spmiLists");
                saveContext.Load(lst);
                saveContext.ExecuteQuery();
                CamlQuery oQuery = new CamlQuery();
                oQuery.ViewXml = viewXML;
                ListItemCollection items = lst.GetItems(oQuery);
                saveContext.Load(items);
                saveContext.ExecuteQuery();
                int iListID = 0;

                foreach (var lstItem in items)
                {
                    if (lstItem["Title"].ToString() == list.Title)
                    {
                        iListID = lstItem.Id;
                    }
                }
                ListItem listItem = null;
                if (iListID == 0)
                {
                    ListItemCreationInformation lici = new ListItemCreationInformation();
                    listItem = lst.AddItem(lici);
                    listItem["Title"] = list.Title;
                    listItem["spmiSiteUrl"] = cSiteUrl;
                    listItem["spmiSiteID"] = iSiteID;
                    listItem["spmiListID"] = list.Id;
                    listItem["spmiParentSite"] = iSiteID;
                }
                else
                {
                    listItem = lst.GetItemById(iListID);
                    saveContext.Load(listItem);
                    saveContext.ExecuteQuery();
                }
                listItem["spmiLastScan"] = DateTime.Now;
                listItem["spmiUniquePermissions"] = list.HasUniqueRoleAssignments;
                listItem["spmiLastItemModified"] = list.LastItemModifiedDate;
                listItem["spmiItemCount"] = list.ItemCount;
                listItem.Update();
                saveContext.ExecuteQuery();
                return listItem.Id;
            }
            catch (Exception ex)
            {

                ShowError(ex, "EnsureListInGallery " + list.Title + " - " + list.Id, "");
            }
            return -1;


        }

        private int EnsureSiteInGallery(ClientContext saveContext, string title, string url, string siteID, int iParentID, bool bHasChildren, bool bHasUniquePerms, DateTime dtLastUserDate, int iUserCount)
        {
            string viewXML = "<View><Query><Where><Eq><FieldRef Name = 'spmiSiteUrl' /><Value Type = 'Text'>" + url + "</Value></Eq></Where></Query><RowLimit>10</RowLimit></View>";
            try
            {

                List lst = saveContext.Web.Lists.GetByTitle("spmiSites");
                saveContext.Load(lst);
                saveContext.ExecuteQuery();
                CamlQuery oQuery = new CamlQuery();
                oQuery.ViewXml = viewXML;
                ListItemCollection items = lst.GetItems(oQuery);
                saveContext.Load(items, si => si.Include(i => i["spmiSiteUrl"], i => i.Id));
                saveContext.ExecuteQuery();
                int iFoundID = 0;
                foreach (var listItem in items)
                {
                    string cWorkUrl = listItem["spmiSiteUrl"].ToString();
                    if (listItem["spmiSiteUrl"].ToString().ToLower() == url.ToString().ToLower())
                    {
                        iFoundID = listItem.Id;
                    }
                }
                ListItem itmSite = null;
                if (iFoundID == 0)
                {
                    ListItemCreationInformation lici = new ListItemCreationInformation();
                    itmSite = lst.AddItem(lici);
                    itmSite["Title"] = title;
                    itmSite["spmiSiteUrl"] = url;
                }
                else
                {
                    itmSite = lst.GetItemById(iFoundID);
                    saveContext.Load(itmSite);
                    saveContext.ExecuteQuery();
                }

                itmSite["spmiUserCount"] = iUserCount;
                itmSite["spmiSiteID"] = siteID;
                itmSite["spmiSiteUrl"] = url;
                itmSite["spmiLastUserItemModified"] = dtLastUserDate;

                if (bHasChildren)
                {
                    itmSite["spmiHasSubSites"] = 1;
                }
                else
                {
                    itmSite["spmiHasSubSites"] = 0;
                }
                itmSite["spmiLastScan"] = DateTime.Now;
                itmSite["spmiUniquePermissions"] = bHasUniquePerms;
                if (iParentID > 0)
                {
                    itmSite["spmiParentSite"] = iParentID;
                }
                itmSite.Update();
                saveContext.ExecuteQuery();
                return itmSite.Id;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    ShowError(ex.InnerException, "EnsureSiteInGallery " + url, "");
                }
                else
                {
                    ShowError(ex, "EnsureSiteInGallery " + url, "");
                }
            }
            return -1;
        }


        private void EnsureVersioningEnabled(ClientContext workCTX, scriptItem oWorkItem)
        {
            EnabledVersioningForLists(workCTX, workCTX.Site.RootWeb);
        }

        private void EnabledVersioningForLists(ClientContext workCTX, Web workWeb)
        {
            try
            {
                workCTX.Load(workWeb, w => w.Title, w => w.Url);
                workCTX.ExecuteQuery();
                ShowProgress(workWeb.Title + " - " + workWeb.Url);
                WebCollection webs = workWeb.Webs;
                workCTX.Load(webs);
                workCTX.ExecuteQuery();
                foreach (Web oWeb in webs)
                {
                    EnabledVersioningForLists(workCTX, oWeb);
                }

                ListCollection olists = workWeb.Lists;
                workCTX.Load(olists);
                workCTX.ExecuteQuery();
                foreach (List list in olists)
                {
                    try
                    {
                        ShowInfo("Working List - " + list.Title);
                        workCTX.Load(list, l => l.EnableVersioning, l => l.Hidden);
                        workCTX.ExecuteQuery();
                        if (!list.Hidden && !list.EnableVersioning)
                        {
                            list.EnableVersioning = true;
                            list.EnableMinorVersions = false;
                            list.Update();
                            workCTX.ExecuteQuery();
                            ShowInfo("Updated Versions for: " + list.Title);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "scriptWorker.EnabledVersioningForList - " + list.Title, "");
                    }
                }


            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.EnabledVersioningForLists", "");
            }
        }

        private void EnsureChildSite(ClientContext workCTX, scriptItem oWorkItem)
        {
            try
            {
                string cParentSite = oWorkItem.GetParm("parentsite");
                string cSiteUrl = oWorkItem.GetParm("siteurl");
                string cSiteTitle = oWorkItem.GetParm("sitetitle");
                string cSiteTemplate = oWorkItem.GetParm("sitetemplate");

            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.EnsureChildSite", "");
            }
        }

        private void ShowBrokenPermissions(ClientContext workCTX, scriptItem oWorkItem)
        {
            try
            {
                ListCollection oLists = workCTX.Web.Lists;
                workCTX.Load(oLists);
                workCTX.ExecuteQuery();
                foreach (List lst in oLists)
                {
                    ShowUniqueItems(workCTX, lst);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.ShowBrokenPermissions", "");
            }
        }

        private void ShowUniqueItems(ClientContext workCTX, List lst)
        {
            try
            {
                workCTX.Load(lst);
                workCTX.ExecuteQuery();
                ShowProgress(lst.Title + " - " + lst.ItemCount);

                CamlQuery oQuery = CamlQuery.CreateAllItemsQuery();
                ListItemCollection items = lst.GetItems(oQuery);
                workCTX.Load(items);
                workCTX.ExecuteQuery();
                foreach (ListItem item in items)
                {
                    workCTX.Load(item, X => X.HasUniqueRoleAssignments);
                    workCTX.ExecuteQuery();
                    if (item.HasUniqueRoleAssignments)
                    {
                        ShowPerms(workCTX, item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.ShowUniqueItems", "");
            }
        }

        private void ShowPerms(ClientContext workCTX, ListItem itm)
        {
            try
            {

                RoleAssignmentCollection roles = itm.RoleAssignments;
                workCTX.Load(roles);
                workCTX.ExecuteQuery();
                ShowProgress("Permissions for: " + itm.Id + " " + itm["FileLeafRef"].ToString());
                foreach (RoleAssignment item in roles)
                {
                    workCTX.Load(item, x => x.Member);
                    workCTX.ExecuteQuery();

                    RoleDefinitionBindingCollection roledefs = item.RoleDefinitionBindings;
                    workCTX.Load(roledefs);
                    workCTX.ExecuteQuery();
                    foreach (RoleDefinition roledef in roledefs)
                    {
                        if (!roledef.Name.Contains("Limited"))
                        {
                            ShowProgress("   " + roledef.Name + " " + item.Member.Title);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.ShowPerms", "");
            }
        }


        private void ClearList(scriptItem oWorkItem)
        {
            try
            {
                string cSourceSite = oWorkItem.GetParm("sourcesite");
                string cSourceList = oWorkItem.GetParm("sourcelist");
                string cSourceUser = oWorkItem.GetParm("sourceuser");
                string cSourcePassword = oWorkItem.GetParm("sourcepassword");
                string cSourceIsOffice365 = oWorkItem.GetParm("sourceoffice365");

                ClientContext srcCTX = new ClientContext(cSourceSite);
                if (cSourceIsOffice365.ToLower() == "true")
                {
                    SecureString password = new SecureString();
                    foreach (char c in cSourcePassword.ToCharArray()) password.AppendChar(c);
                    srcCTX.Credentials = new SharePointOnlineCredentials(cSourceUser, password);

                }
                else
                {
                    srcCTX.Credentials = new NetworkCredential(cSourceUser, cSourcePassword);
                }

                srcCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                List SourceList = srcCTX.Web.Lists.GetByTitle(cSourceList);
                srcCTX.Load(SourceList);
                srcCTX.ExecuteQuery();
                int iBatch = 0;
                while (SourceList.ItemCount > 0)
                {
                    try
                    {
                        {
                            iBatch++;
                            ShowInfo("Processing Batch:" + iBatch);

                            CamlQuery oQuery = new CamlQuery();
                            oQuery.ViewXml = "<View><RowLimit>250</RowLimit></View>";
                            // oQuery = CamlQuery.CreateAllItemsQuery();
                            ListItemCollection items = SourceList.GetItems(oQuery);

                            srcCTX.Load(items);
                            srcCTX.ExecuteQuery();



                            foreach (ListItem item in items.ToList())
                            {
                                item.DeleteObject();
                            }

                            try
                            {
                                srcCTX.ExecuteQuery();

                            }
                            catch (Exception ex)
                            {
                                ShowError(ex, "scriptWorker.ClearList-Inside", "");

                            }



                            srcCTX.Load(SourceList);
                            srcCTX.ExecuteQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "scriptWorker.ClearList", "");
                    }

                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.CopyList", "");
            }
        }


        private Dictionary<string, int> GetHeaders(string cFileName)
        {
            Dictionary<string, int> hi = new Dictionary<string, int>();
            StreamReader oFile = new StreamReader(cFileName);
            string cHeader = oFile.ReadLine();
            string[] aHeader = cHeader.Split(',');
            int i = 0;
            foreach (string item in aHeader)
            {
                try
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        hi.Add(aHeader[i], i);
                    }
                    i++;
                }
                catch (Exception ex)
                {
                    ShowError(ex, "", "");
                }

            }
            return hi;
        }

        private void ImportList(ClientContext workCTX, scriptItem oWorkItem)
        {
            try
            {
                string cFileName = oWorkItem.GetParm("filename");
                string cListName = oWorkItem.GetParm("listname");
                string cListKey = oWorkItem.GetParm("listkey");
                string cFileKey = oWorkItem.GetParm("filekey");
                string cFieldSettings = oWorkItem.GetParm("fieldsettings");

                List lst = workCTX.Web.Lists.GetByTitle(cListName);
                workCTX.Load(lst);
                workCTX.ExecuteQuery();

                Dictionary<string, string> fields = new Dictionary<string, string>();
                string[] afields = cFieldSettings.Split(';');
                foreach (string item in afields)
                {
                    string[] info = item.Split('`');
                    fields.Add(info[0], info[1]);
                }


                if (System.IO.File.Exists(cFileName))
                {
                    Dictionary<string, int> headerInfo = GetHeaders(cFileName);


                    using (StreamReader reader = new StreamReader(cFileName))
                    {
                        string line;
                        reader.ReadLine();

                        while ((line = reader.ReadLine()) != null)
                        {
                            try
                            {
                                //Define pattern
                                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                                //Separating columns to array
                                string[] values = CSVParser.Split(line);

                                string cKeyValue = GetRowValue(headerInfo, cFileKey, values);
                                ListItem itm = lst.GetListItemByField(workCTX, cListKey, "Text", cKeyValue);
                                if (itm == null)
                                {
                                    ListItemCreationInformation lici = new ListItemCreationInformation();
                                    itm = lst.AddItem(lici);
                                    itm[cListKey] = cKeyValue;

                                }
                                foreach (KeyValuePair<string, string> fld in fields)
                                {
                                    string cValue = GetRowValue(headerInfo, fld.Value, values);
                                    itm[fld.Key] = cValue;
                                }
                                itm.Update();
                                workCTX.ExecuteQuery();

                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }




                }

                //List SourceList = srcCTX.Web.Lists.GetByTitle(cSourceList);
                //srcCTX.Load(SourceList);
                //srcCTX.ExecuteQuery();
                //List TargetList = tgtCTX.Web.Lists.GetByTitle(cTargetList);
                //tgtCTX.Load(TargetList);
                //tgtCTX.ExecuteQuery();

                //if (FieldSettingsValid(SourceList, TargetList, cFieldSettings))
                //{
                //    SourceList.SyncList(tgtCTX, cTargetList, cFieldSettings, new DateTime(1970, 1, 1));

                //    // CopyList(srcCTX, SourceList, tgtCTX, TargetList, cFieldSettings,cQuery,bLargeList);
                //}
                //else
                //{
                //    ShowProgress("Field Settings Do Not Match for:" + SourceList.Title);
                //}

            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.CopyList", "");
            }
        }

        private string GetRowValue(Dictionary<string, int> headerInfo, string cFileKey, string[] values)
        {
            try
            {
                int iFieldIndex = GetFieldIndex(headerInfo, cFileKey);
                return values[iFieldIndex];

            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.GetRowValue", "");
            }
            return "";
        }

        private int GetFieldIndex(Dictionary<string, int> headerInfo, string cFileKey)
        {
            foreach (KeyValuePair<string, int> item in headerInfo)
            {
                if (item.Key == cFileKey)
                {
                    return item.Value;
                }
            }
            return -1;
        }

        private void CopyList(scriptItem oWorkItem)
        {
            try
            {
                string cSourceSite = oWorkItem.GetParm("sourcesite");
                string cSourceList = oWorkItem.GetParm("sourcelist");
                string cTargetSite = oWorkItem.GetParm("targetsite");
                string cTargetList = oWorkItem.GetParm("targetlist");
                string cFieldSettings = oWorkItem.GetParm("fieldsettings");

                ClientContext srcCTX = new ClientContext(cSourceSite);
                ClientContext tgtCTX = new ClientContext(cTargetSite);
                srcCTX.Credentials = ctx.Credentials;
                tgtCTX.Credentials = ctx.Credentials;
                List SourceList = srcCTX.Web.Lists.GetByTitle(cSourceList);
                srcCTX.Load(SourceList);
                srcCTX.ExecuteQuery();
                List TargetList = tgtCTX.Web.Lists.GetByTitle(cTargetList);
                tgtCTX.Load(TargetList);
                tgtCTX.ExecuteQuery();

                if (FieldSettingsValid(SourceList, TargetList, cFieldSettings))
                {
                    SourceList.SyncList(tgtCTX, cTargetList, cFieldSettings, new DateTime(1970, 1, 1));

                    // CopyList(srcCTX, SourceList, tgtCTX, TargetList, cFieldSettings,cQuery,bLargeList);
                }
                else
                {
                    ShowProgress("Field Settings Do Not Match for:" + SourceList.Title);
                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.CopyList", "");
            }
        }

        private void CopyItem(List SourceList, ClientContext tgtCTX, List TargetList, ListItem oSourceItem, string cFieldSettings)
        {
            try
            {
                ListItemCreationInformation lici = new ListItemCreationInformation();
                ListItem oNewItem = TargetList.AddItem(lici);
                //DisableWorkflowAssociations(tgtCTX, TargetList, true);
                string[] aFieldSettings = cFieldSettings.Split(';');
                foreach (string aFieldSetting in aFieldSettings)
                {
                    try
                    {
                        string[] aFldSet = aFieldSetting.Split(':');

                        string cTargetFieldInternalName = TargetList.Fields.GetField(aFldSet[0]).InternalName;
                        if (!aFldSet[1].Contains("'"))
                        {
                            string cSourceInternalName = SourceList.Fields.GetField(aFldSet[1]).InternalName;
                            if (oSourceItem[cSourceInternalName] != null)
                            {
                                oNewItem[cTargetFieldInternalName] = oSourceItem[cSourceInternalName];
                                System.Diagnostics.Trace.WriteLine(cTargetFieldInternalName + " = " + oSourceItem[cSourceInternalName].ToString());
                            }
                        }
                        else
                        {
                            oNewItem[cTargetFieldInternalName] = aFldSet[1].Replace("'", "");
                            System.Diagnostics.Trace.WriteLine(cTargetFieldInternalName + " = " + aFldSet[1].Replace("'", ""));
                        }
                        oNewItem.Update();
                        tgtCTX.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "scriptWorker.CopyItem", "");
                    }

                }
                //StartWorkflow(tgtCTX, oNewItem, "ImportItem");

                //DisableWorkflowAssociations(tgtCTX, TargetList, false);



                oNewItem.Update();
                tgtCTX.ExecuteQuery();

                ShowInfo(oNewItem.Id.ToString());

                RunCount++;

            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.CopyItem", "");
            }
        }

        private void DisableWorkflowAssociations(ClientContext tgtCTX, List targetList, bool bDisable)
        {
            try
            {
                tgtCTX.Load(targetList, tl => tl.WorkflowAssociations);
                tgtCTX.ExecuteQuery();
                for (int i = 0; i < targetList.WorkflowAssociations.Count; i++)
                {
                    targetList.WorkflowAssociations[0].Enabled = !bDisable;
                    targetList.WorkflowAssociations[0].Update();
                    tgtCTX.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.DisableWorkflowAssociations", "");
            }
        }

        private void StartWorkflow(ClientContext workCTX, ListItem itm, string cWorkflowName)
        {
            try
            {

                var workflowServicesManager = new WorkflowServicesManager(workCTX, workCTX.Web);
                var workflowInteropService = workflowServicesManager.GetWorkflowInteropService();
                var workflowSubscriptionService = workflowServicesManager.GetWorkflowSubscriptionService();
                var workflowDeploymentService = workflowServicesManager.GetWorkflowDeploymentService();
                var workflowInstanceService = workflowServicesManager.GetWorkflowInstanceService();

                var publishedWorkflowDefinitions = workflowDeploymentService.EnumerateDefinitions(true);
                workCTX.Load(publishedWorkflowDefinitions);
                workCTX.ExecuteQuery();

                var def = from defs in publishedWorkflowDefinitions
                          where defs.DisplayName == cWorkflowName
                          select defs;

                WorkflowDefinition workflow = def.FirstOrDefault();

                if (workflow != null)
                {


                    // get all workflow associations
                    var workflowAssociations = workflowSubscriptionService.EnumerateSubscriptionsByDefinition(workflow.Id);
                    workCTX.Load(workflowAssociations);
                    workCTX.ExecuteQuery();

                    // find the first association
                    var firstWorkflowAssociation = workflowAssociations.First();

                    // start the workflow
                    var startParameters = new Dictionary<string, object>();


                    ShowProgress("Starting workflow for item: " + itm.Id);
                    workflowInstanceService.StartWorkflowOnListItem(firstWorkflowAssociation, itm.Id, startParameters);
                    workCTX.ExecuteQuery();

                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.StartWorkflow", "");
            }

        }




        private void CopyList(ClientContext srcCTX, List SourceList, ClientContext tgtCTX, List TargetList, string cFieldSettings)
        {
            bool bLargeList = true;
            string cQuery = "";
            try
            {
                if (bLargeList)
                {
                    TotalItems = SourceList.ItemCount;
                    int iItemCount = SourceList.ItemCount;
                    for (int i = 1; i < iItemCount; i++)
                    {
                        try
                        {
                            ListItem itmSource = SourceList.GetItemById(i);
                            srcCTX.Load(itmSource);
                            srcCTX.ExecuteQuery();

                            CopyItem(SourceList, tgtCTX, TargetList, itmSource, cFieldSettings);
                        }
                        catch (Exception ex)
                        {
                            ShowError(ex, "scriptWorker.CopyList", "");
                        }
                    }

                }
                else
                {


                    CamlQuery oQuery = null;
                    if (string.IsNullOrEmpty(cQuery))
                    {
                        oQuery = CamlQuery.CreateAllItemsQuery();
                    }
                    else
                    {
                        oQuery = CamlQuery.CreateAllItemsQuery();
                    }
                    ListItemCollection oSourceItems = SourceList.GetItems(oQuery);
                    srcCTX.Load(oSourceItems);
                    srcCTX.ExecuteQuery();
                    TotalItems = oSourceItems.Count;
                    foreach (ListItem oSourceItem in oSourceItems)
                    {
                        try
                        {
                            srcCTX.Load(oSourceItem);
                            srcCTX.ExecuteQuery();
                            CopyItem(SourceList, tgtCTX, TargetList, oSourceItem, cFieldSettings);
                        }
                        catch (Exception ex)
                        {
                            ShowError(ex, "scriptWorker.CopyList", "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.CopyList", "");
            }
        }

        private bool FieldSettingsValid(List SourceList, List TargetList, string cFieldSettings)
        {
            StringBuilder sbFieldResults = new StringBuilder();
            try
            {
                string[] aFieldSettings = cFieldSettings.Split(';');
                foreach (string aFieldSetting in aFieldSettings)
                {
                    if (aFieldSetting.Contains(":"))
                    {
                        string[] aFldSet = aFieldSetting.Split(':');
                        if (!TargetList.Fields.HasField(aFldSet[0]))
                        {
                            sbFieldResults.AppendLine(string.Format("{0} is missing: {1}", TargetList.Title, aFldSet[0]));
                        }
                        if (!aFldSet[1].Contains("'"))
                        {
                            if (!SourceList.Fields.HasField(aFldSet[1]))
                            {
                                sbFieldResults.AppendLine(string.Format("{0} is missing: {1}", SourceList.Title, aFldSet[1]));
                            }
                        }
                    }
                }


                if (string.IsNullOrEmpty(sbFieldResults.ToString()))
                {
                    return true;
                }
                else
                {
                    ShowProgress("Field Exceptions: " + sbFieldResults.ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {

                ShowError(ex, "scriptWorker.FieldSettingsValid", sbFieldResults.ToString());
            }

            return false;
        }

        private void EnumWebParts(ClientContext workCTX, Web oWorkWeb)
        {
            try
            {
                if (ProcessSubWebs)
                {
                    WebCollection webs = oWorkWeb.Webs;
                    workCTX.Load(webs);
                    workCTX.ExecuteQuery();
                    foreach (Web item in webs)
                    {
                        EnumWebParts(workCTX, item);
                    }
                }
                Folder fldPages = oWorkWeb.RootFolder.Folders.GetFolder("Pages");
                if (fldPages != null)
                {
                    FileCollection files = fldPages.Files;
                    workCTX.Load(files);
                    workCTX.ExecuteQuery();
                    foreach (Microsoft.SharePoint.Client.File file in files)
                    {
                        if (file.Name.ToLower().Contains(".aspx"))
                        {
                            EnnumWebParts(workCTX, file);
                        }
                    }

                }
                Folder fldSitePages = oWorkWeb.RootFolder.Folders.GetFolder("SitePages");
                if (fldSitePages != null)
                {
                    FileCollection files = fldSitePages.Files;
                    workCTX.Load(files);
                    workCTX.ExecuteQuery();
                    foreach (Microsoft.SharePoint.Client.File file in files)
                    {
                        if (file.Name.ToLower().Contains(".aspx"))
                        {
                            EnnumWebParts(workCTX, file);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.EnumWebParts", "");
            }
        }

        private void EnnumWebParts(ClientContext workCTX, Microsoft.SharePoint.Client.File file)
        {
            workCTX.Load(file, f => f.ServerRelativeUrl);
            workCTX.ExecuteQuery();
            ShowProgress("Listing Web Parts for: " + file.ServerRelativeUrl);


            LimitedWebPartManager limitedWebPartManager = file.GetLimitedWebPartManager(PersonalizationScope.Shared);

            workCTX.Load(limitedWebPartManager.WebParts,
                wps => wps.Include(
                wp => wp.WebPart.Title, wp => wp.WebPart.Properties));

            workCTX.ExecuteQuery();

            if (limitedWebPartManager.WebParts.Count > 0)
            {

                foreach (WebPartDefinition webPart in limitedWebPartManager.WebParts)
                {
                    ShowProgress(string.Format("{0} - {1}", file.ServerRelativeUrl, webPart.WebPart.Title));

                    //Dictionary<string,object> props =  webPart.WebPart.Properties.FieldValues;
                    //foreach (KeyValuePair<string, object> prop in props)
                    //{
                    //    Trace.WriteLine(prop.Key+" - +prop.Value.ToString());
                    //}

                    //Trace.WriteLine("----------------------------");

                }
            }




        }

        private void SyncSiteFolder(string cSourceSite, string cSourceFolder, string item)
        {
            try
            {
                ClientContext srcContext = new ClientContext(cSourceSite) { Credentials = ctx.Credentials };
                ClientContext tgtContext = new ClientContext(item) { Credentials = ctx.Credentials };
                srcContext.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                tgtContext.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                Folder srcFolder = srcContext.Web.EnsureFolder(cSourceFolder);
                Folder tgtFolder = tgtContext.Web.EnsureFolder(cSourceFolder);

                SyncFolder(srcContext, tgtContext, srcFolder, tgtFolder);


            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("permission"))
                {
                    ShowProgress("Unable to Get Permission for: " + item);
                }
                else
                {
                    ShowError(ex, "scriptWorker.SyncSiteFolder", "");
                }
            }
        }

        private void SyncFolder(ClientContext srcCTX, ClientContext tgtCTX, Folder srcFolder, Folder tgtFolder)
        {
            try
            {
                FolderCollection folders = srcFolder.Folders;
                srcFolder.Context.Load(folders);
                srcFolder.Context.ExecuteQuery();
                foreach (Folder folder in folders)
                {
                    try
                    {
                        Folder tgtWorkFolder = tgtFolder.Folders.GetFolder(folder.Name);
                        if (tgtWorkFolder == null)
                        {
                            tgtFolder.Folders.Add(folder.Name);
                            tgtCTX.ExecuteQuery();
                            tgtWorkFolder = tgtFolder.Folders.GetFolder(folder.Name);
                        }
                        SyncFolder(srcCTX, tgtCTX, folder, tgtWorkFolder);
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "scriptWorker.SyncFolder", "Inside Folders");
                    }
                }
                FileCollection oSrcFiles = srcFolder.Files;
                srcCTX.Load(oSrcFiles);
                srcCTX.ExecuteQuery();
                foreach (Microsoft.SharePoint.Client.File srcFile in oSrcFiles)
                {
                    Trace.WriteLine(srcFile.Name);
                    if (!(srcFolder.ServerRelativeUrl.ToLower().Contains("display templates") && srcFile.Name.ToLower().EndsWith(".js")))
                    {
                        try
                        {
                            srcFolder.Context.Load(srcFile, f => f.TimeLastModified);
                            srcFolder.Context.ExecuteQuery();
                            Microsoft.SharePoint.Client.File tgtFile = tgtFolder.GetFile(srcFile.Name);
                            if (tgtFile == null)
                            {
                                string nLocation = string.Format("{0}/{1}", tgtFolder.ServerRelativeUrl, srcFile.Name);
                                FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(srcCTX, srcFile.ServerRelativeUrl);
                                Microsoft.SharePoint.Client.File.SaveBinaryDirect(tgtCTX, nLocation, fileInfo.Stream, true);
                                ShowProgress("Added File:" + srcFile.Name);
                            }
                            else
                            {
                                if (srcFile.TimeLastModified > tgtFile.TimeLastModified)
                                {
                                    string nLocation = string.Format("{0}/{1}", tgtFolder.ServerRelativeUrl, srcFile.Name);
                                    FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(srcCTX, srcFile.ServerRelativeUrl);
                                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(tgtCTX, nLocation, fileInfo.Stream, true);

                                    ShowProgress("Updated File:" + tgtFile.Name);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.ToLower().Contains("unauthorized"))
                            {
                                ShowProgress("Unable to Get Permission for: " + tgtFolder.ServerRelativeUrl);
                                break;
                            }
                            else
                            {
                                ShowError(ex, "scriptWorker.SyncFolder", "Inside Files");
                            }
                        }
                    }
                }




            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.SyncFolder", "");
            }

        }

        private static ListTemplateType GetTemplateType(string cTypeName)
        {
            ListTemplateType ltt = ListTemplateType.GenericList;
            switch (cTypeName)
            {
                case "Calendar":
                    ltt = ListTemplateType.Events;
                    break;
                case "Tasks":
                    ltt = ListTemplateType.Tasks;
                    break;
                case "Issues":
                    ltt = ListTemplateType.IssueTracking;
                    break;
                case "Picture":
                    ltt = ListTemplateType.PictureLibrary;
                    break;
                case "Document":

                case "DocumentLibrary":
                    ltt = ListTemplateType.DocumentLibrary;
                    break;
                case "Generic":
                case "Custom":
                    ltt = ListTemplateType.GenericList;
                    break;
                default:
                    break;
            }

            return ltt;
            //switch (tp)
            //{
            //    case ListTemplateType.InvalidType:

            //        break;
            //    case ListTemplateType.NoListTemplate:

            //        break;
            //    case ListTemplateType.GenericList:

            //        break;
            //    case ListTemplateType.DocumentLibrary:

            //        break;
            //    case ListTemplateType.Survey:

            //        break;
            //    case ListTemplateType.Links:

            //        break;
            //    case ListTemplateType.Announcements:

            //        break;
            //    case ListTemplateType.Contacts:

            //        break;
            //    case ListTemplateType.Events:

            //        break;
            //    case ListTemplateType.Tasks:

            //        break;
            //    case ListTemplateType.DiscussionBoard:

            //        break;
            //    case ListTemplateType.PictureLibrary:

            //        break;
            //    case ListTemplateType.DataSources:

            //        break;
            //    case ListTemplateType.WebTemplateCatalog:

            //        break;
            //    case ListTemplateType.UserInformation:

            //        break;
            //    case ListTemplateType.WebPartCatalog:

            //        break;
            //    case ListTemplateType.ListTemplateCatalog:

            //        break;
            //    case ListTemplateType.XMLForm:

            //        break;
            //    case ListTemplateType.MasterPageCatalog:

            //        break;
            //    case ListTemplateType.NoCodeWorkflows:

            //        break;
            //    case ListTemplateType.WorkflowProcess:

            //        break;
            //    case ListTemplateType.WebPageLibrary:

            //        break;
            //    case ListTemplateType.CustomGrid:

            //        break;
            //    case ListTemplateType.SolutionCatalog:

            //        break;
            //    case ListTemplateType.NoCodePublic:

            //        break;
            //    case ListTemplateType.ThemeCatalog:

            //        break;
            //    case ListTemplateType.DesignCatalog:

            //        break;
            //    case ListTemplateType.AppDataCatalog:

            //        break;
            //    case ListTemplateType.DataConnectionLibrary:

            //        break;
            //    case ListTemplateType.WorkflowHistory:

            //        break;
            //    case ListTemplateType.GanttTasks:

            //        break;
            //    case ListTemplateType.HelpLibrary:

            //        break;
            //    case ListTemplateType.AccessRequest:

            //        break;
            //    case ListTemplateType.TasksWithTimelineAndHierarchy:

            //        break;
            //    case ListTemplateType.MaintenanceLogs:

            //        break;
            //    case ListTemplateType.Meetings:

            //        break;
            //    case ListTemplateType.Agenda:

            //        break;
            //    case ListTemplateType.MeetingUser:

            //        break;
            //    case ListTemplateType.Decision:

            //        break;
            //    case ListTemplateType.MeetingObjective:

            //        break;
            //    case ListTemplateType.TextBox:

            //        break;
            //    case ListTemplateType.ThingsToBring:

            //        break;
            //    case ListTemplateType.HomePageLibrary:

            //        break;
            //    case ListTemplateType.Posts:

            //        break;
            //    case ListTemplateType.Comments:

            //        break;
            //    case ListTemplateType.Categories:

            //        break;
            //    case ListTemplateType.Facility:

            //        break;
            //    case ListTemplateType.Whereabouts:

            //        break;
            //    case ListTemplateType.CallTrack:

            //        break;
            //    case ListTemplateType.Circulation:

            //        break;
            //    case ListTemplateType.Timecard:

            //        break;
            //    case ListTemplateType.Holidays:

            //        break;
            //    case ListTemplateType.IMEDic:

            //        break;
            //    case ListTemplateType.ExternalList:

            //        break;
            //    case ListTemplateType.MySiteDocumentLibrary:

            //        break;
            //    case ListTemplateType.IssueTracking:

            //        break;
            //    case ListTemplateType.AdminTasks:

            //        break;
            //    case ListTemplateType.HealthRules:

            //        break;
            //    case ListTemplateType.HealthReports:

            //        break;
            //    case ListTemplateType.DeveloperSiteDraftApps:

            //        break;
            //    case ListTemplateType.AccessApp:

            //        break;
            //    case ListTemplateType.AlchemyMobileForm:

            //        break;
            //    case ListTemplateType.AlchemyApprovalWorkflow:

            //        break;
            //    case ListTemplateType.SharingLinks:

            //        break;
            //    case ListTemplateType.HashtagStore:

            //        break;

            //}
        }

        private static ClientContext GetClientContext(ClientContext ctx, string cUrl)
        {
            ClientContext oNewContext;
            if (cUrl.ToLower() == "{currentsite}")
            {
                cUrl = ctx.Url;
            }
            if (cUrl.ToLower().Contains("sharepoint.com"))
            {
                //ClientContext oNewContext = new ClientContext(cUrl) { Credentials = ctx.Credentials };
                OfficeDevPnP.Core.AuthenticationManager am = new OfficeDevPnP.Core.AuthenticationManager();
                oNewContext = am.GetWebLoginClientContext(cUrl, null);

                oNewContext.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
            }
            else
            {
                oNewContext = new ClientContext(cUrl);
                oNewContext.Credentials = ctx.Credentials;

            }
            return oNewContext;
        }

        private void FindUnpublishedFiles(string item)
        {
            try
            {
                ClientContext ctxWork = new ClientContext(item) { Credentials = ctx.Credentials };
                ctxWork.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                List<Web> oWebs = ctxWork.Site.RootWeb.GetSubWebs();
                TotalItems = oWebs.Count;
                RunCount = 0;

                ShowProgress("Working: " + item);
                ShowUnPublishedFiles(ctxWork.Site.RootWeb, "Master Page Gallery");



                foreach (Web oWeb in oWebs)
                {
                    try
                    {
                        RunCount++;
                        ctxWork.Load(oWeb, ow => ow.Url);
                        ctxWork.ExecuteQuery();
                        ShowInfo("Working:" + oWeb.Url);

                        if (oWeb.HasList("Pages"))
                        {
                            ShowUnPublishedFiles(oWeb, "Pages");
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "scriptWorker.FindUnpublishedFiles", "");
                    }

                }


            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.FindUniqueID", "");
            }

        }

        private void ShowUnPublishedFiles(Web workWeb, string cListName)
        {
            try
            {
                List lstWork = workWeb.Lists.GetByTitle(cListName);
                workWeb.Context.Load(lstWork);
                workWeb.Context.ExecuteQuery();
                Folder wrkFolder = lstWork.RootFolder;

                ShowUnPublishedFiles(wrkFolder);



            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.ShowUnPublishedFiles", "");
            }


        }

        private void ShowUnPublishedFiles(Folder wrkFolder)
        {
            try
            {
                wrkFolder.Context.Load(wrkFolder, f => f.ServerRelativeUrl);
                wrkFolder.Context.ExecuteQuery();

                ShowInfo("Working: " + wrkFolder.ServerRelativeUrl);
                FolderCollection folders = wrkFolder.Folders;
                wrkFolder.Context.Load(folders);
                wrkFolder.Context.ExecuteQuery();
                foreach (Folder folder in folders)
                {
                    ShowUnPublishedFiles(folder);

                }
                FileCollection files = wrkFolder.Files;
                wrkFolder.Context.Load(files);
                wrkFolder.Context.ExecuteQuery();

                foreach (Microsoft.SharePoint.Client.File item in files)
                {
                    try
                    {
                        wrkFolder.Context.Load(item, f => f.ServerRelativeUrl, f => f.Level);
                        wrkFolder.Context.ExecuteQuery();

                        if (item.Level != FileLevel.Published)
                        {
                            if (item.Level == FileLevel.Draft)
                            {
                                if (!item.Name.Contains(".js"))
                                {
                                    item.Publish("Published By script");
                                    wrkFolder.Context.ExecuteQuery();
                                }
                            }
                            else
                            {
                                ShowProgress("File Is Checked Out: " + item.ServerRelativeUrl);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "scriptWorker.ShowUnPublishedFiles", "Inside");
                    }
                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.ShowUnPublishedFiles", "");
            }
        }

        private void FindUniqueID(scriptItem oWorkItem, string item)
        {

            try
            {
                string cGuid = oWorkItem.parms["guid"];
                ClientContext ctxWork = new ClientContext(item) { Credentials = ctx.Credentials };
                ctxWork.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                List<Web> oWebs = ctxWork.Site.RootWeb.GetSubWebs();
                TotalItems = oWebs.Count;
                RunCount = 0;

                foreach (Web oWeb in oWebs)
                {
                    try
                    {
                        RunCount++;
                        ctxWork.Load(oWeb, ow => ow.Url);
                        ctxWork.ExecuteQuery();
                        ShowProgress("Working:" + oWeb.Url);

                        ListCollection lists = oWeb.Lists;
                        ctxWork.Load(lists);
                        ctxWork.ExecuteQuery();
                        foreach (List list in lists)
                        {
                            ctxWork.Load(list, l => l.Title, l => l.Id);
                            ctxWork.ExecuteQuery();
                            ShowInfo("Processing: " + list.Title);
                            if (list.ContainsGuidPart(cGuid))
                            {
                                ShowProgress(string.Format("Guid Found in: {0}  - {1}", oWeb.Url, list.Title));
                            }
                            else
                            {
                                CamlQuery oAllItems = CamlQuery.CreateAllItemsQuery();
                                ListItemCollection items = list.GetItems(oAllItems);
                                ctxWork.Load(items);
                                ctxWork.ExecuteQuery();
                                foreach (ListItem childitem in items)
                                {
                                    if (childitem.ContainsGuidPart(cGuid))
                                    {
                                        ShowProgress(string.Format("Found Find in: {0}  - {1} @ id =  {2}", oWeb.Url, list.Title, childitem.Id));

                                    }
                                }



                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "scriptWorker.FindUniqueID", "");
                    }

                }


            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.FindUniqueID", "");
            }
        }

        private void UpdateHomePageReferences(ClientContext workCTX, Web workWeb)
        {
            try
            {
                workCTX.Load(workWeb, ww => ww.Url);
                workCTX.ExecuteQuery();
                ShowInfo(workWeb.Url);
                WebCollection webs = workWeb.Webs;
                workCTX.Load(webs);
                workCTX.ExecuteQuery();
                foreach (Web web in webs)
                {
                    UpdateHomePageReferences(workCTX, web);
                }

                NavigationNodeCollection nodes = workWeb.Navigation.TopNavigationBar;
                workCTX.Load(nodes);
                workCTX.ExecuteQuery();
                foreach (NavigationNode item in nodes)
                {
                    if (item.Url.ToLower().Contains("home.aspx") || item.Url.ToLower().Contains("default.aspx"))
                    {
                        ShowProgress(string.Format("TopNav:{0} - {1}", item.Title, item.Url));
                    }
                }

                nodes = workWeb.Navigation.QuickLaunch;
                workCTX.Load(nodes);
                workCTX.ExecuteQuery();
                foreach (NavigationNode item in nodes)
                {
                    if (item.Url.ToLower().Contains("home.aspx") || item.Url.ToLower().Contains("default.aspx"))
                    {
                        ShowProgress(string.Format("QuickLaunch:{0} - {1}", item.Title, item.Url));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.UpdateHomePageReferences", "");
            }
        }
        private void UpdateHomePageReferences(string item)
        {
            try
            {
                ClientContext workCTX = new ClientContext(item) { Credentials = ctx.Credentials };
                workCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                UpdateHomePageReferences(workCTX, workCTX.Site.RootWeb);

            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.UpdateHomePageReferences", "");
            }
        }

        private void UploadPageLayout(scriptItem oWorkItem, string item)
        {
            try
            {
                ClientContext workCTX = new ClientContext(item) { Credentials = ctx.Credentials };
                workCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                string cFileName = oWorkItem.parms["filelocation"];
                UploadFile(workCTX, "Master Page Gallery", "Article Page", cFileName, item);

            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.UploadPageLayout", "");
            }
        }

        private void UploadFile(ClientContext workCTX, string cListName, string cContentType, string cFileLoc, string cSiteCol)
        {
            try
            {

                List lstDocLib = workCTX.Web.Lists.GetByTitle(cListName);
                workCTX.Load(lstDocLib);
                workCTX.ExecuteQuery();

                using (var fs = new FileStream(cFileLoc, FileMode.Open))
                {
                    var fi = new FileInfo(cFileLoc);
                    workCTX.Load(lstDocLib.RootFolder, rf => rf.ServerRelativeUrl);
                    workCTX.ExecuteQuery();
                    var fileUrl = String.Format("{0}/{1}", lstDocLib.RootFolder.ServerRelativeUrl, fi.Name);
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(workCTX, fileUrl, fs, true);
                    ListItem itm = GetItemByName(lstDocLib, fi.Name);
                    if (itm != null)
                    {
                        SetContentType(workCTX, itm, cContentType);
                    }
                    ShowProgress(string.Format("Uploaded: {0} to => {1}", fi.Name, cSiteCol));

                }


            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.UploadFile " + cSiteCol, "");
            }
        }

        private void SetContentType(ClientContext workCTX, ListItem itm, string cContentType)
        {
            try
            {
                ContentTypeId ctid = GetContentTypeId(workCTX, cContentType);
                if (ctid != null)
                {
                    itm["ContentTypeId"] = ctid;
                    itm.Update();
                    workCTX.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.SetContentType", "");
            }
        }

        private ContentTypeId GetContentTypeId(ClientContext workCTX, string cContentType)
        {
            try
            {
                ContentTypeCollection cts = workCTX.Site.RootWeb.ContentTypes;
                workCTX.Load(cts);
                workCTX.ExecuteQuery();
                foreach (ContentType ct in cts)
                {
                    if (ct.Name == cContentType)
                    {
                        workCTX.Load(ct, contenttype => contenttype.Id);
                        workCTX.ExecuteQuery();
                        return ct.Id;
                    }
                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.GetContentTypeId", "");
            }
            return null;
        }

        private ListItem GetItemByName(List lstDocLib, string name)
        {
            try
            {
                CamlQuery oQuery = new CamlQuery() { ViewXml = string.Format("<Where><Eq><FieldRef Name = 'FileLeafRef' /><Value Type = 'File' > {0} </Value></Eq></Where>", name) };
                ListItemCollection items = lstDocLib.GetItems(oQuery);
                lstDocLib.Context.Load(items);
                lstDocLib.Context.ExecuteQuery();
                foreach (ListItem item in items)
                {
                    if (item["FileLeafRef"].ToString() == name)
                        return item;
                }


            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.GetItemByName", "");
            }
            return null;
        }

        private void UpdateSiteImage(scriptItem oWorkItem, string item)
        {
            try
            {
                string cImageUrl = oWorkItem.parms["imageurl"];
                ClientContext oWorkCTX = new ClientContext(item) { Credentials = ctx.Credentials };
                oWorkCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                UpdateWebLogos(oWorkCTX, oWorkCTX.Site.RootWeb, cImageUrl);
            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.UpdateSiteImage", "");
            }

        }

        private void UpdateWebLogos(ClientContext oWorkCTX, Web workWeb, string cImageUrl)
        {
            try
            {
                oWorkCTX.Load(workWeb, rw => rw.SiteLogoUrl, rw => rw.Url);
                oWorkCTX.ExecuteQuery();
                if (workWeb.SiteLogoUrl != cImageUrl)
                {
                    workWeb.SiteLogoUrl = cImageUrl;
                    workWeb.Update();
                    oWorkCTX.ExecuteQuery();
                    ShowProgress("UpdateSiteImage for: " + workWeb.Url);
                }
                WebCollection oWebs = workWeb.Webs;
                oWorkCTX.Load(oWebs);
                oWorkCTX.ExecuteQuery();
                foreach (Web oWeb in oWebs)
                {
                    UpdateWebLogos(oWorkCTX, oWeb, cImageUrl);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "scriptWorker.UpdateWebLogos", "");
            }


        }

        private void RenameInternalGroups(string item)
        {
            try
            {
                ShowProgress("Working Site:" + item);
                ClientContext wrkCTX = new ClientContext(item) { Credentials = ctx.Credentials };
                wrkCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                GroupCollection grps = wrkCTX.Site.RootWeb.SiteGroups;
                wrkCTX.Load(grps);
                wrkCTX.ExecuteQuery();
                foreach (Group grp in grps)
                {
                    try
                    {
                        ShowInfo("Working: " + grp.Title);
                        if ((grp.Title.ToLower().EndsWith("members") || grp.Title.ToLower().EndsWith("visitors") || grp.Title.ToLower().EndsWith("owners"))
                            && !grp.Title.ToLower().StartsWith("spg"))
                        {
                            grp.Title = "spg" + grp.Title;
                            grp.Update();
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "RenameInternalGroups: " + grp.Title, "");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "RenameInternalGroups: " + item, "");
            }
        }

        private void WalkAllSites()
        {
            SPOSitePropertiesEnumerable prop = null;
            ClientContext tenantCTX = new ClientContext("https://havertys-admin.sharepoint.com") { Credentials = ctx.Credentials };
            tenantCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
            {
                e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
            };
            Tenant tenant = new Tenant(tenantCTX);
            prop = tenant.GetSiteProperties(0, true);
            tenantCTX.Load(prop);
            tenantCTX.ExecuteQuery();
            foreach (SiteProperties sp in prop)
            {
                ShowProgress("Working: " + sp.Url);
                WalkSiteCollections(sp.Url);

            }
        }

        private void WalkSiteCollections(string url)
        {
            try
            {
                ShowInfo(url);
                ClientContext workCTX = new ClientContext(url) { Credentials = ctx.Credentials };
                workCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                WebCollection webs = workCTX.Site.RootWeb.Webs;
                workCTX.Load(webs);
                workCTX.ExecuteQuery();
                foreach (Web web in webs)
                {
                    WalkChildSites(web);
                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "ProcessSites.WalkSiteCollections", "");
            }
        }

        private void WalkChildSites(Web web)
        {
            try
            {
                ShowInfo("Child: " + web.Url);

                WebCollection webs = web.Webs;
                web.Context.Load(webs);
                web.Context.ExecuteQuery();
                foreach (Web childweb in webs)
                {
                    try
                    {
                        WalkChildSites(childweb);
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "ProcessSites.WalkChildSites", web.Url);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "ProcessSites.WalkChildSites - Outside: ", web.Url);
            }
        }

        private bool HasNavNode(ClientContext workCTX, string Title)
        {
            try
            {
                NavigationNode oNode = workCTX.Web.Navigation.GetNodeById(1040);
                NavigationNodeCollection oNodes = oNode.Children;
                workCTX.Load(oNode);
                workCTX.Load(oNodes);
                workCTX.ExecuteQuery();
                foreach (NavigationNode item in oNodes)
                {
                    if (item.Title == Title) return true;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "HasNavNode", "");

            }
            return false;
        }

        private void EnsureNavNode(ClientContext workCTX, string Title, string cUrl)
        {
            try
            {
                if (!HasNavNode(workCTX, Title))
                {
                    AddNavNode(workCTX, Title, cUrl);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "EnsureNavNode", "");
            }
        }

        private void AddNavNode(ClientContext workCTX, string Title, string cUrl)
        {
            try
            {
                Uri target = new Uri(cUrl, UriKind.Absolute);
                workCTX.Web.AddNavigationNode(Title, target, string.Empty, NavigationType.SearchNav, false);

            }
            catch (Exception ex)
            {
                ShowError(ex, "ProcessSites.AddNavNode", "");
            }
        }


        private void RemoveNavNode(ClientContext workCTX, string Title)
        {
            try
            {
                NavigationNode oNode = workCTX.Web.Navigation.GetNodeById(1040);
                NavigationNodeCollection oNodes = oNode.Children;
                workCTX.Load(oNode);
                workCTX.Load(oNodes);
                workCTX.ExecuteQuery();
                foreach (NavigationNode item in oNodes)
                {
                    if (item.Title == Title) item.DeleteObject();
                    workCTX.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "ProcessSites.AddNavNode", "");
            }
        }

        private Uri BuildTargetUri(string cPath)
        {
            try
            {
                ClientContext tempCTX = new ClientContext("https://havertys.sharepoint.com/sites/searchcenter") { Credentials = ctx.Credentials };
                tempCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                string cShortName = cPath.Substring(cPath.LastIndexOf('/') + 1);
                List<Microsoft.SharePoint.Client.File> oFiles = tempCTX.Web.FindFiles(cShortName);

                foreach (Microsoft.SharePoint.Client.File item in oFiles)
                {
                    Trace.WriteLine(item.ServerRelativeUrl);
                    Uri fileURI = new Uri(item.ServerRelativeUrl, UriKind.Relative);
                    return fileURI;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "ProcessSites.BuildTargetUri", "");
            }
            return null;
        }

        private void ShowContentTypeFields(string cSite)
        {

            try
            {
                ClientContext workCtx = new ClientContext(cSite) { Credentials = ctx.Credentials };
                workCtx.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                ContentTypeCollection cts = workCtx.Web.ContentTypes;
                workCtx.Load(cts);
                workCtx.ExecuteQuery();
                foreach (ContentType ct in cts)
                {
                    if (ct.Group == "Havertys")
                    {
                        workCtx.Load(ct);
                        workCtx.ExecuteQuery();
                        FieldCollection flds = ct.Fields;
                        workCtx.Load(flds);
                        workCtx.ExecuteQuery();
                        foreach (Field fld in flds)
                        {
                            ShowInfo(string.Format("Field: {0} - {1}", ct.Name, fld.InternalName));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "ProcessSites.ShowContentTypeFields", "");
            }
        }

        private int GetSiteCount()
        {
            ShowProgress("Getting Site Information");
            int iSiteCount = 0;
            try
            {
                string[] sites = SiteList.Split(';');
                foreach (string item in sites)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        int iCount = GetSiteCount(item);
                        if (iCount > 0) iSiteCount += iCount;
                    }
                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "ProcessSites.GetSiteCounts", "");
            }
            return iSiteCount;
        }

        private int GetSiteCount(string item)
        {
            try
            {
                if (!spwebs.Contains(item))
                {
                    spwebs.Add(item);
                }

                if (ProcessSubWebs)
                {
                    int iSiteCount = 0;
                    ClientContext workCTX = new ClientContext(item) { Credentials = ctx.Credentials };
                    workCTX.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                    {
                        e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                    };
                    WebCollection webs = workCTX.Site.RootWeb.Webs;
                    workCTX.Load(webs);
                    workCTX.ExecuteQuery();

                    iSiteCount += GetSiteCount(workCTX, workCTX.Site.RootWeb);

                    foreach (Web web in webs)
                    {
                        workCTX.Load(web);
                        workCTX.ExecuteQuery();

                        if (!web.Url.Contains("HavertysNavigation") && !web.Url.Contains("havertys-") && !spwebs.Contains(web.Url))
                        {
                            spwebs.Add(web.Url);
                        }
                        iSiteCount += GetSiteCount(workCTX, web);
                    }
                    return iSiteCount;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "ProcessSites.GetSiteCount::" + item, "");
            }
            return -1;

        }

        private int GetSiteCount(ClientContext workCTX, Web workWeb)
        {
            try
            {
                workCTX.Load(workWeb);
                workCTX.ExecuteQuery();

                if (!workWeb.Url.Contains("HavertysNavigation") && !workWeb.Url.Contains("havertys-") && !spwebs.Contains(workWeb.Url))
                {
                    spwebs.Add(workWeb.Url);
                }

                int iSiteCount = 0;
                WebCollection webs = workWeb.Webs;
                workCTX.Load(webs);
                workCTX.ExecuteQuery();

                foreach (Web web in webs)
                {
                    workCTX.Load(web);
                    workCTX.ExecuteQuery();

                    if (!web.Url.Contains("HavertysNavigation") && !web.Url.Contains("havertys-") && !spwebs.Contains(web.Url))
                    {
                        spwebs.Add(web.Url);
                    }
                    iSiteCount += GetSiteCount(workCTX, web);
                }
                return iSiteCount;
            }
            catch (Exception ex)
            {
                ShowError(ex, "ProcessSites.GetSiteCount - Inside Web", "");
            }
            return -1;

        }


        void oWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case -1:
                    string cParms = (string)e.UserState;
                    string[] parms = cParms.Split('|');
                    if (Error != null) Error(parms[0], parms[1], parms[2]);
                    break;
                case 1:
                    if (Progress != null) Progress((string)e.UserState);
                    break;
                case 2:
                    if (Info != null) Info((string)e.UserState);
                    break;
                case 3:
                    if (SiteInfo != null) SiteInfo((string)e.UserState);
                    break;
                case 4:
                    if (ReaminingTimeInfo != null) ReaminingTimeInfo((string)e.UserState);
                    break;
            }
        }

        void oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            sw.Stop();
            if (oWorker.CancellationPending)
            {
                if (Canceled != null) Canceled();
                return;
            }
            if (Complete != null) Complete();
            oOutputFile.Flush();
            oOutputFile.Close();
        }



        private static Folder GetListItemFolder(ListItem listItem)
        {

            var folderUrl = (string)listItem["FileDirRef"];
            var parentFolder = listItem.ParentList.ParentWeb.GetFolderByServerRelativeUrl(folderUrl);
            listItem.Context.Load(parentFolder);
            listItem.Context.ExecuteQuery();
            return parentFolder;
        }

        private void ProcessLibrary(ClientContext workCTX, ClientContext updateCTX, string cListName)
        {
            ContentType ctFolder = workCTX.Web.GetContentType("Folder");
            ContentType ct = workCTX.Web.GetContentType("CIS Document");
            workCTX.Load(ct);
            workCTX.Load(ctFolder);
            workCTX.ExecuteQuery();
            List lst = workCTX.Web.Lists.GetByTitle(cListName);

            List lstUpdate = updateCTX.Web.Lists.GetByTitle(cListName);
            updateCTX.Load(lstUpdate);
            updateCTX.ExecuteQuery();

            workCTX.Load(lst);
            workCTX.ExecuteQuery();
            lst.EnsureListHasContenttype(workCTX.Site, "CIS Document");
            CamlQuery oQuery = CamlQuery.CreateAllItemsQuery();
            ListItemCollection items = lst.GetItems(oQuery);
            workCTX.Load(items);
            workCTX.ExecuteQuery();

            ShowProgress(cListName + " = " + items.Count);
            foreach (var listItem in items)
            {
                try
                {
                    workCTX.Load(listItem, l => l["FileLeafRef"], l => l["Title"], l => l.ContentType.Name, l => l["FileDirRef"]);
                    workCTX.ExecuteQuery();
                    Folder parentFolder = GetListItemFolder(listItem);
                    if (listItem["Title"] != null)
                    {
                        if (listItem["Title"].ToString().Contains("FS") && listItem["Title"].ToString().Contains("FS"))
                        {
                            listItem["ContentTypeId"] = ctFolder.Id;
                        }
                    }
                    if (listItem["FileLeafRef"] != null)
                    {
                        if (listItem["FileLeafRef"].ToString().Contains("FS") && listItem["FileLeafRef"].ToString().Contains("FS"))
                        {
                            listItem["ContentTypeId"] = ctFolder.Id;
                        }
                    }
                    if (listItem.ContentType.Name == "Document")
                    {

                        listItem["ContentTypeId"] = ct.Id;
                    }
                    if (parentFolder != null)
                    {
                        if (parentFolder.Name != null)
                        {
                            if (parentFolder.Name != cListName)
                            {
                                ListItem itmUpdate = lstUpdate.GetItemById(listItem.Id);
                                updateCTX.Load(itmUpdate);
                                updateCTX.ExecuteQuery();


                                itmUpdate["cisFolder"] = parentFolder.Name;

                                string cName = GetCISName(parentFolder.Name);
                                string cNumber = GetCISNumber(parentFolder.Name);
                                string cType = GetCISType(parentFolder.Name);
                                itmUpdate["cisLocationName"] = cName;
                                itmUpdate["cisLocationNo"] = cNumber;
                                itmUpdate["cisLocationType"] = cType;
                                itmUpdate.Update();
                                updateCTX.ExecuteQuery();

                                // ProcessItem(listItem, parentFolder.Name);
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    ShowError(ex, "WorkLib", ex.Message);
                }
            }

        }

        private void ProcessLibraryItem(ClientContext workCTX, ClientContext updateCTX, string cListName)
        {
            ContentType ctFolder = workCTX.Web.GetContentType("Folder");
            ContentType ct = workCTX.Web.GetContentType("CIS Document");
            workCTX.Load(ct);
            workCTX.Load(ctFolder);
            workCTX.ExecuteQuery();
            List lst = workCTX.Web.Lists.GetByTitle(cListName);
            workCTX.Load(lst);
            workCTX.ExecuteQuery();


            var field = workCTX.CastTo<FieldChoice>(lst.Fields.GetByInternalNameOrTitle("Document Type"));
            workCTX.Load(field, f => f.Choices);
            workCTX.ExecuteQuery();


            CamlQuery oQuery = CamlQuery.CreateAllItemsQuery();
            ListItemCollection items = lst.GetItems(oQuery);
            workCTX.Load(items, itms => itms.Include(itm => itm.ContentType.Name, itm => itm.Id, itm => itm["FileDirRef"], itm => itm["FileLeafRef"], itm => itm["cisDocumentType"], itm => itm["cisLocationNo"], itm => itm["cisLocationName"], itm => itm["cisLocationType"], itm => itm["cisFolder"]));
            workCTX.ExecuteQuery();

            ShowProgress(cListName + " = " + items.Count);
            foreach (var listItem in items)
            {
                try
                {
                    bool bUpdate = false;
                    if (listItem["cisLocationName"] != null)
                    {
                        if (listItem["cisLocationName"].ToString().StartsWith("/"))
                        {
                            listItem["cisLocationName"] = listItem["cisLocationName"].ToString().Substring(1);
                            bUpdate = true;
                        }
                    }
                    //if (listItem.ContentType.Name=="Folder")
                    //{
                    //    if (listItem["FileLeafRef"].ToString().Contains("."))
                    //    {
                    //        listItem["ContentTypeId"] = ct.Id;
                    //        bUpdate = true;
                    //    }

                    //}



                    foreach (string item in field.Choices)
                    {
                        if (listItem["cisDocumentType"] == null && listItem["FileLeafRef"].ToString().ToLower().Contains(item.ToLower()))
                        {
                            listItem["cisDocumentType"] = item;
                            bUpdate = true;
                            break;
                        }
                    }
                    if (listItem["cisLocationNo"] != null)
                    {
                        if (listItem["cisLocationNo"].ToString().ToLower().Contains("grid") || listItem["cisLocationNo"].ToString().ToLower().Contains("table"))
                        {
                            listItem["cisLocationNo"] = "";
                            bUpdate = true;
                        }
                    }
                    if (listItem["cisFolder"] != null && listItem["cisFolder"].ToString().ToLower().Contains("grid"))
                    {

                        string cPath = listItem["FileDirRef"].ToString();
                        string cLastFolder = cPath.Substring(cPath.LastIndexOf('/'));
                        cPath = cPath.Replace(cLastFolder, "");
                        string cFolderName = cPath.Substring(cPath.LastIndexOf('/'));

                        if (listItem["cisLocationNo"] == null)
                        {
                            listItem["cisLocationNo"] = GetCISNumber(cFolderName);
                            bUpdate = true;
                        }
                        if (listItem["cisLocationName"] == null)
                        {
                            listItem["cisLocationName"] = GetCISName(cFolderName);
                            bUpdate = true;
                        }
                        if (listItem["cisLocationType"] == null)
                        {
                            listItem["cisLocationType"] = GetCISType(cFolderName);
                            bUpdate = true;
                        }

                    }

                    if (listItem["cisFolder"] != null && !listItem["cisFolder"].ToString().ToLower().Contains("grid data"))
                    {
                        ShowInfo(listItem["cisFolder"].ToString());
                        if (listItem["cisLocationNo"] == null)
                        {
                            listItem["cisLocationNo"] = GetCISNumber(listItem["cisFolder"].ToString());
                            bUpdate = true;
                        }
                        if (listItem["cisLocationName"] == null)
                        {
                            listItem["cisLocationName"] = GetCISName(listItem["cisFolder"].ToString());
                            bUpdate = true;
                        }
                        if (listItem["cisLocationType"] == null)
                        {
                            listItem["cisLocationType"] = GetCISType(listItem["cisFolder"].ToString());
                            bUpdate = true;
                        }
                    }
                    if (bUpdate)
                    {
                        listItem.Update();
                        workCTX.ExecuteQuery();
                    }

                }
                catch (Exception ex)
                {
                    ShowError(ex, "WorkLib", ex.Message);
                }
            }

        }


        private string GetCISName(string name)
        {
            try
            {
                ShowInfo(name);
                string cName = "";
                if (name.Contains("FSU"))
                {
                    cName = name.Substring(0, name.IndexOf("FSU")).Trim();
                }
                if (name.Contains("FSR"))
                {
                    cName = name.Substring(0, name.IndexOf("FSR"));
                }
                if (name.Contains("DTO"))
                {
                    cName = name.Substring(0, name.IndexOf("DTO"));
                }
                if (name.Contains("Other"))
                {
                    cName = name.Substring(0, name.IndexOf("Other"));
                }

                if (name.Contains("In-Line"))
                {
                    cName = name.Substring(0, name.IndexOf("In-Line"));
                }
                return cName;
            }
            catch (Exception ex)
            {
                ShowError(ex, "", ex.Message);
            }
            return "";
        }

        private string GetCISNumber(string name)
        {
            try
            {
                string cNumber = "";
                cNumber = name.Substring(name.LastIndexOf(" ")).Trim();
                if (cNumber.StartsWith("_"))
                {
                    cNumber = cNumber.Substring(1);
                }
                if (cNumber.ToLower() == "data")
                    return "";
                return cNumber;


            }
            catch (Exception ex)
            {
                ShowError(ex, "", ex.Message);
            }
            return "";
        }

        private string GetCISType(string name)
        {
            try
            {
                string cType = "";
                if (name.Contains("FSU"))
                {
                    cType = "FSU";
                }
                if (name.Contains("FSR"))
                {
                    cType = "FSR";
                }
                if (name.Contains("DTO"))
                {
                    cType = "DTO";
                }
                if (name.Contains("Other"))
                {
                    cType = "Other";
                }

                if (name.Contains("In-Line"))
                {
                    cType = "In-Line";
                }
                return cType;
            }
            catch (Exception ex)
            {
                ShowError(ex, "", ex.Message);
            }
            return "";
        }

        private void ProcessItem(ListItem listItem, string name)
        {
            try
            {
                ShowInfo(name);
                string cName = "";
                string cType = "";
                string cNumber = "";
                if (name.Contains("FSU"))
                {
                    cName = name.Substring(0, name.IndexOf("FSU")).Trim();
                    cType = "FSU";
                    cNumber = name.Substring(name.LastIndexOf(" ")).Trim();
                    if (cNumber.StartsWith("_"))
                    {
                        cNumber = cNumber.Substring(1);
                    }
                }
                if (name.Contains("FSR"))
                {
                    cName = name.Substring(0, name.IndexOf("FSR"));
                    cType = "FSR";
                    cNumber = name.Substring(name.LastIndexOf(" ")).Trim();
                    if (cNumber.StartsWith("_"))
                    {
                        cNumber = cNumber.Substring(1);
                    }
                }
                if (name.Contains("DTO"))
                {
                    cName = name.Substring(0, name.IndexOf("DTO"));
                    cType = "DTO";
                    cNumber = name.Substring(name.LastIndexOf(" ")).Trim();
                    if (cNumber.StartsWith("_"))
                    {
                        cNumber = cNumber.Substring(1);
                    }
                }
                if (name.Contains("Other"))
                {
                    cName = name.Substring(0, name.IndexOf("Other"));
                    cType = "Other";
                    cNumber = name.Substring(name.LastIndexOf(" ")).Trim();
                    if (cNumber.StartsWith("_"))
                    {
                        cNumber = cNumber.Substring(1);
                    }
                }

                if (name.Contains("In-Line"))
                {
                    cName = name.Substring(0, name.IndexOf("In-Line"));
                    cType = "In-Line";
                    cNumber = name.Substring(name.LastIndexOf(" ")).Trim();
                    if (cNumber.StartsWith("_"))
                    {
                        cNumber = cNumber.Substring(1);
                    }
                }


                listItem["cisLocationName"] = cName;
                listItem["cisLocationNo"] = cNumber;
                listItem["cisLocationType"] = cType;
                listItem.Update();
                listItem.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                ShowError(ex, "WorkItem", ex.Message);
            }



        }


        #endregion

        #region ProgressMethods
        void ShowSiteInfo(string cSiteInfo)
        {
            oWorker.ReportProgress(3, cSiteInfo);
            oOutputFile.WriteLine("SiteInfo: " + cSiteInfo);
        }
        void ShowProgress(string cProgress)
        {
            oWorker.ReportProgress(1, cProgress);
            oOutputFile.WriteLine("Progress: " + cProgress);
        }

        void ShowInfo(string cInfo)
        {
            oWorker.ReportProgress(2, cInfo);
            oOutputFile.WriteLine("Info: " + cInfo);

        }

        void ShowError(Exception ex, string cLocation, string cMessage)
        {

            string cOutput = String.Format("{0}|{1}|{2}", ex.Message, cLocation, cMessage);
            if (ex.InnerException != null)
            {
                cOutput += "|InnerException|" + ex.InnerException.Message;
            }
            oWorker.ReportProgress(-1, cOutput);
            oOutputFile.WriteLine("Error: " + cOutput);
        }

        #endregion



        private void EnsureUserPermissionInWeb(ClientContext workCTX, Web web, string cUserName, RoleType role)
        {
            try
            {
                RoleAssignmentCollection roleAssignments = web.RoleAssignments;
                workCTX.Load(roleAssignments, ra => ra.Include(rd => rd.Member.LoginName, rd => rd.Member.Title, rd => rd.Member.PrincipalType, rd => rd.RoleDefinitionBindings));
                workCTX.ExecuteQuery();
                foreach (RoleAssignment ra in roleAssignments)
                {
                    if (ra.Member.LoginName == cUserName)
                    {
                        return;
                    }
                }
                var user_group = web.EnsureUser(cUserName);
                var roleDefBindCol = new RoleDefinitionBindingCollection(workCTX);
                roleDefBindCol.Add(web.RoleDefinitions.GetByType(role));
                roleAssignments.Add(user_group, roleDefBindCol);
                workCTX.Load(roleAssignments);
                web.Update();
                workCTX.ExecuteQuery();
            }
            catch (Exception ex)
            {
                ShowError(ex, "EnsureGroupPermissionInWeb", "");
            }


        }

        private void EnsureUserPermissionInList(ClientContext workCTX, List list, string cUserName, RoleType role)
        {
            try
            {
                RoleAssignmentCollection roleAssignments = list.RoleAssignments;
                workCTX.Load(roleAssignments, ra => ra.Include(rd => rd.Member.LoginName, rd => rd.Member.Title, rd => rd.Member.PrincipalType, rd => rd.RoleDefinitionBindings));
                workCTX.ExecuteQuery();
                foreach (RoleAssignment ra in roleAssignments)
                {
                    if (ra.Member.LoginName == cUserName)
                    {
                        return;
                    }
                }
                var user_group = list.ParentWeb.EnsureUser(cUserName);
                var roleDefBindCol = new RoleDefinitionBindingCollection(workCTX);
                roleDefBindCol.Add(list.ParentWeb.RoleDefinitions.GetByType(role));
                roleAssignments.Add(user_group, roleDefBindCol);
                workCTX.Load(roleAssignments);
                list.Update();
                workCTX.ExecuteQuery();
            }
            catch (Exception ex)
            {
                ShowError(ex, "EnsureUserPermissionInList", "");
            }


        }


        private void EnsureUserPermissionInItem(ClientContext workCTX, ListItem item, string cUserName, RoleType role)
        {
            try
            {
                RoleAssignmentCollection roleAssignments = item.RoleAssignments;
                workCTX.Load(roleAssignments, ra => ra.Include(rd => rd.Member.LoginName, rd => rd.Member.Title, rd => rd.Member.PrincipalType, rd => rd.RoleDefinitionBindings));
                workCTX.ExecuteQuery();
                foreach (RoleAssignment ra in roleAssignments)
                {
                    if (ra.Member.LoginName == cUserName)
                    {
                        return;
                    }
                }
                var user_group = item.ParentList.ParentWeb.EnsureUser(cUserName);
                var roleDefBindCol = new RoleDefinitionBindingCollection(workCTX);
                roleDefBindCol.Add(item.ParentList.ParentWeb.RoleDefinitions.GetByType(role));
                roleAssignments.Add(user_group, roleDefBindCol);
                workCTX.Load(roleAssignments);
                item.SystemUpdate();
                workCTX.ExecuteQuery();
            }
            catch (Exception ex)
            {
                ShowError(ex, "EnsureGroupPermissionInWeb", "");
            }
        }



        private void EnsureGroupPermissionInWeb(ClientContext workCTX, Web web, string cGroupName, RoleType role)
        {
            try
            {
                RoleAssignmentCollection roleAssignments = web.RoleAssignments;
                workCTX.Load(roleAssignments, ra => ra.Include(rd => rd.Member.Title, rd => rd.Member.PrincipalType, rd => rd.RoleDefinitionBindings));
                workCTX.ExecuteQuery();
                foreach (RoleAssignment ra in roleAssignments)
                {
                    if (ra.Member.Title == cGroupName)
                    {
                        return;
                    }
                }
                var user_group = web.SiteGroups.GetByName(cGroupName);
                var roleDefBindCol = new RoleDefinitionBindingCollection(workCTX);
                roleDefBindCol.Add(web.RoleDefinitions.GetByType(role));
                roleAssignments.Add(user_group, roleDefBindCol);
                workCTX.Load(roleAssignments);
                web.Update();
                workCTX.ExecuteQuery();
            }
            catch (Exception ex)
            {
                ShowError(ex, "EnsureGroupPermissionInWeb", "");
            }


        }

        private void EnsureGroupPermissionInList(ClientContext workCTX, List list, string cGroupName, RoleType role)
        {
            try
            {
                RoleAssignmentCollection roleAssignments = list.RoleAssignments;
                workCTX.Load(roleAssignments, ra => ra.Include(rd => rd.Member.Title, rd => rd.Member.PrincipalType, rd => rd.RoleDefinitionBindings));
                workCTX.ExecuteQuery();
                foreach (RoleAssignment ra in roleAssignments)
                {
                    if (ra.Member.Title == cGroupName)
                    {
                        return;
                    }
                }
                var user_group = list.ParentWeb.SiteGroups.GetByName(cGroupName);
                var roleDefBindCol = new RoleDefinitionBindingCollection(workCTX);
                roleDefBindCol.Add(list.ParentWeb.RoleDefinitions.GetByType(role));
                roleAssignments.Add(user_group, roleDefBindCol);
                workCTX.Load(roleAssignments);
                list.Update();
                workCTX.ExecuteQuery();
            }
            catch (Exception ex)
            {
                ShowError(ex, "EnsureGroupPermissionInWeb", "");
            }


        }


        private void EnsureGroupPermissionInItem(ClientContext workCTX, ListItem item, string cGroupName, RoleType role)
        {
            try
            {

                RoleAssignmentCollection roleAssignments = item.RoleAssignments;
                workCTX.Load(roleAssignments, ra => ra.Include(rd => rd.Member.Title, rd => rd.Member.PrincipalType, rd => rd.RoleDefinitionBindings));
                workCTX.ExecuteQuery();
                foreach (RoleAssignment ra in roleAssignments)
                {
                    if (ra.Member.Title == cGroupName)
                    {
                        return;
                    }
                }
                var user_group = item.ParentList.ParentWeb.SiteGroups.GetByName(cGroupName);
                var roleDefBindCol = new RoleDefinitionBindingCollection(workCTX);
                roleDefBindCol.Add(item.ParentList.ParentWeb.RoleDefinitions.GetByType(role));
                roleAssignments.Add(user_group, roleDefBindCol);
                workCTX.Load(roleAssignments);
                item.SystemUpdate();
                workCTX.ExecuteQuery();
            }
            catch (Exception ex)
            {
                ShowError(ex, "EnsureGroupPermissionInWeb", "");
            }
        }




        private void ListWebParts(ClientContext workCTX, string item, scriptItem oWorkItem)
        {
            try
            {
                string cPageUrl = oWorkItem.GetParm("pagename");

                var page = workCTX.Web.GetFileByServerRelativeUrl(cPageUrl);
                var wpm = page.GetLimitedWebPartManager(PersonalizationScope.Shared);
                workCTX.Load(wpm, w => w.WebParts);
                workCTX.ExecuteQuery();
                Console.WriteLine(wpm.WebParts.Count);
                foreach (WebPartDefinition wp in wpm.WebParts)
                {
                    workCTX.Load(wp, wpi => wpi.WebPart);
                    workCTX.ExecuteQuery();
                    System.Diagnostics.Trace.WriteLine(wp.ToString());





                    //var client = new WebPartPagesWebService();
                    //client.Url = siteRootAddress + "/_vti_bin/Webpartpages.asmx";
                    //client.Credentials = credential;
                    //// webPartId is a property of WebPart Defenition from the above code
                    //var webPartXmlString = client.GetWebPart2(pageAddress,
                    //                                               webPartId,
                    //                                               Storage.Shared,
                    //                                               SPWebServiceBehavior.Version3);

                    //var webPartNode = XElement.Parse(webPartXmlString);

                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "ListWebParts", ex.Message);
            }
        }


        private void UpdateInventory(ClientContext workCTX, string item, scriptItem oWorkItem)
        {
            try
            {

                string saveContextUrl = oWorkItem.GetParm("saveurl");
                string scanContextUrl = oWorkItem.GetParm("scanurl");
                ClientContext saveContext = new ClientContext(saveContextUrl);
                saveContext.Credentials = workCTX.Credentials;
                ShowProgress("Gathering Scan Information");
                ClientContext scanContext = new ClientContext(scanContextUrl);

                if (oWorkItem.GetParmBool("CurrentCredentials") == true)
                {
                    ShowProgress("Using Current Credentials");
                }
                else
                {
                    ShowProgress("Using Login Credentials");
                    scanContext.Credentials = ctx.Credentials;
                }



                Int32 workCount = GetItemsToProcess(scanContext.Site.RootWeb);
                TotalItems = workCount;
                ShowProgress(workCount + " items to process");
                WorkGallerySite(workCTX, oWorkItem);
            }
            catch (Exception ex)
            {
                ShowError(ex, "Work Sites for Gallery -" + item, "");
            }

        }

        private string GetRemaining(int iCount, int totalitemcount, TimeSpan elapsed)
        {
            try
            {

                double avgSpeed = elapsed.TotalMilliseconds / iCount;
                double estRemaining = avgSpeed * (totalitemcount - iCount);
                TimeSpan time = TimeSpan.FromSeconds(Convert.ToInt64(estRemaining / 1000));
                string str = time.ToString(@"hh\:mm");
                return str;
            }
            catch (Exception ex)
            {
                ShowError(ex, "GetRemaining", "");
            }
            return "";
        }


        private void UpdateInventoryListItems(ClientContext workCTX, string item, scriptItem oWorkItem)
        {
            try
            {

                string saveContextUrl = oWorkItem.GetParm("saveurl");
                //string scanContextUrl = oWorkItem.GetParm("scanurl");
                ClientContext saveContext = new ClientContext(saveContextUrl);
                saveContext.Credentials = workCTX.Credentials;
                ShowProgress("Gathering Scan Information");
                //ClientContext scanContext = new ClientContext(scanContextUrl);
                //scanContext.Credentials = ctx.Credentials;

                CamlQuery oQueryAll = CamlQuery.CreateAllItemsQuery();
                ListItemCollection items = saveContext.Web.Lists.GetByTitle("spmiLists").GetItems(oQueryAll);
                saveContext.Load(items, l => l.Include(li => li["spmiListID"], li => li["spmiSiteUrl"], li => li["spmiItemCount"], li => li["Title"]));
                saveContext.ExecuteQuery();


                foreach (ListItem listItem in items)
                {
                    TotalItems += Convert.ToInt32(listItem["spmiItemCount"].ToString());
                }
                ShowProgress("Processing " + TotalItems + " items");
                RunCount = 1;
                //string lastUrl = "";
                ClientContext scanContext = ctx;

                List<Task> tasks = new List<Task>();

                foreach (ListItem listItem in items)
                {
                    try
                    {
                        //if (listItem["spmiSiteUrl"].ToString() != lastUrl)
                        //{
                        //    string cSiteUrl = listItem["spmiSiteUrl"].ToString();
                        //    scanContext = new ClientContext(cSiteUrl);
                        //    scanContext.Credentials = ctx.Credentials;
                        //    lastUrl = listItem["spmiSiteUrl"].ToString();
                        //    ShowInfo("Scanning List: " + listItem["Title"].ToString()+ " in Site: " + cSiteUrl);
                        //}
                        string cSiteUrl = listItem["spmiSiteUrl"].ToString();
                        string cListID = listItem["spmiListID"].ToString();

                        //tasks.Add(Task.Run(() =>
                        //{
                        //ScanListitems(scanContext, saveContext, listItem["spmiListID"].ToString());
                        ScanListItemsAsync(cSiteUrl, saveContextUrl, cListID, ctx.Credentials);
                        /*}))*/
                        ;
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "UpdateInventoryListItems inside: " + item, "");
                    }
                }
                Task.WaitAll(tasks.ToArray());
                System.Diagnostics.Trace.WriteLine("Done with Items!");
            }
            catch (Exception ex)
            {
                ShowError(ex, "UpdateInventoryListItems" + item, "");
            }
        }

        private void ScanListItemsAsync(string cSiteUrl, string saveContextUrl, string cListID, ICredentials credentials)
        {
            try
            {
                using (ClientContext saveContext = new ClientContext(saveContextUrl))
                {
                    saveContext.Credentials = credentials;
                    using (ClientContext scanContext = new ClientContext(cSiteUrl))
                    {
                        scanContext.Credentials = credentials;
                        ScanListitems(scanContext, saveContext, cListID);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "ScanListItemsAsync: " + cSiteUrl, "");
            }



        }

        private void ScanListitems(ClientContext scanContext, ClientContext saveContext, string cListId)
        {
            try
            {
                List workList = scanContext.Web.Lists.GetById(new Guid(cListId));
                scanContext.Load(workList);
                scanContext.ExecuteQuery();

                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View Scope='RecursiveAll'><RowLimit>5000</RowLimit><ViewFields><FieldRef Name='ID' /></ViewFields></View>";

                List<ListItem> items = new List<ListItem>();
                do
                {
                    ListItemCollection listItemCollection = workList.GetItems(camlQuery);

                    scanContext.Load(listItemCollection);//, l => l.Include(li => li.File, li => li.FieldValuesAsText, li => li.HasUniqueRoleAssignments, li => li.DisplayName, li => li["Title"], li => li["FileLeafRef"], li => li["GUID"], li => li["File_x0020_Size"]));


                    scanContext.ExecuteQuery();

                    //Adding the current set of ListItems in our single buffer
                    items.AddRange(listItemCollection);
                    //Reset the current pagination info
                    camlQuery.ListItemCollectionPosition = listItemCollection.ListItemCollectionPosition;

                } while (camlQuery.ListItemCollectionPosition != null);
                foreach (ListItem li in items)
                {
                    if (workList.BaseTemplate == 101)
                    {
                        scanContext.Load(li, itm => itm.File.ServerRelativeUrl, itm => itm.File.Name, itm => itm.DisplayName, itm => itm.ContentType, itm => itm["File_x0020_Size"], itm => itm["GUID"], itm => itm.File, itm => itm.FieldValuesAsText, itm => itm.HasUniqueRoleAssignments);
                    }
                    else
                    {
                        scanContext.Load(li, itm => itm.DisplayName, itm => itm["GUID"], itm => itm.ContentType, itm => itm["Title"], itm => itm.FieldValuesAsText, itm => itm.HasUniqueRoleAssignments);
                    }
                    scanContext.ExecuteQuery();
                    if (li.ContentType.Name != "Folder")
                    {
                        SaveItemInfo(saveContext, li, cListId, (workList.BaseTemplate == 101));
                    }
                    UpdateProgress();
                }



            }
            catch (Exception ex)
            {
                ShowError(ex, "ScanListitems" + cListId, "");
            }
        }



        private void SaveItemInfo(ClientContext saveContext, ListItem itm, string cListId, bool isFile)
        {
            try
            {
                string itmGuid = itm["GUID"].ToString();
                string viewXML = "<View><Query><Where><Eq><FieldRef Name = 'spmiItemGuid' /><Value Type = 'Text'>" + itmGuid + "</Value></Eq></Where></Query><RowLimit>5000</RowLimit></View>";
                CamlQuery oQuery = new CamlQuery();
                oQuery.ViewXml = viewXML;
                List lst = saveContext.Web.Lists.GetByTitle("spmiItems");
                ListItemCollection items = lst.GetItems(oQuery);
                saveContext.Load(items);
                saveContext.ExecuteQuery();
                if (items.Count == 0)
                {
                    ListItemCreationInformation lici = new ListItemCreationInformation();
                    ListItem workItem = lst.AddItem(lici);
                    if (isFile)
                    {
                        if (itm["File_x0020_Size"] != null)
                        {

                            string cFileSize = itm["File_x0020_Size"].ToString();
                            if (!string.IsNullOrEmpty(cFileSize))
                            {
                                int fileSize = Convert.ToInt32(cFileSize);
                                workItem["spmiItemSize"] = fileSize;
                            }
                        }
                        workItem["spmiItemType"] = "File";
                        workItem["spmiItemUrl"] = itm.File.ServerRelativeUrl;
                        workItem["Title"] = itm.File.Name;
                    }
                    else
                    {
                        workItem["Title"] = itm.DisplayName;
                    }
                    workItem["spmiListID"] = cListId;
                    workItem["spmiItemData"] = JsonConvert.SerializeObject(itm.FieldValuesAsText.FieldValues);
                    //workItem["spmiParentFolderUrl"] = itm.File.foldeworkFolder.ServerRelativeUrl
                    workItem.Update();
                    saveContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "SaveItemInfo -", "");
            }

        }

        private Int32 GetItemsToProcess(Web oWeb)
        {
            oWeb.Context.Load(oWeb, w => w.ServerRelativeUrl);
            oWeb.Context.ExecuteQuery();
            ShowSiteInfo("Working: " + oWeb.ServerRelativeUrl);
            Int32 runningCount = 1;
            try
            {
                WebCollection webs = oWeb.Webs;
                oWeb.Context.Load(webs);
                oWeb.Context.ExecuteQuery();
                foreach (Web web in webs)
                {
                    runningCount += GetItemsToProcess(web);
                }
                ListCollection lsts = oWeb.Lists;
                oWeb.Context.Load(lsts, ls => ls.Include(l => l.Title), ls => ls.Include(l => l.BaseTemplate), ls => ls.Include(l => l.ItemCount), ls => ls.Where(l => l.Hidden == false && l.IsCatalog == false));
                oWeb.Context.ExecuteQuery();
                foreach (List list in lsts)
                {
                    runningCount = runningCount + 1;// +list.ItemCount;
                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "GetItemsToProcess -" + oWeb.ServerRelativeUrl, "");
            }

            return runningCount;
        }
        private void WorkGallerySite(ClientContext workCTX, scriptItem oWorkItem)
        {
            try
            {

                string saveContextUrl = oWorkItem.GetParm("saveurl");
                ClientContext saveContext = new ClientContext(saveContextUrl);
                saveContext.Credentials = workCTX.Credentials;


                ClientContext walkContext = new ClientContext(oWorkItem.GetParm("scanurl"));
                walkContext.Credentials = workCTX.Credentials;
                walkContext.Load(walkContext.Web, ww => ww.Title, ww => ww.ServerRelativeUrl, ww => ww.Id);
                walkContext.ExecuteQuery();
                ShowInfo(walkContext.Web.Title + " - " + walkContext.Web.ServerRelativeUrl);
                ShowProgress("Walking Site: " + oWorkItem.GetParm("scanurl"));


                EnsureSiteGallery(walkContext, saveContext, walkContext.Web, -1, oWorkItem);
            }
            catch (Exception ex)
            {
                ShowError(ex, "Work Site for Gallery -", "");
            }

        }

        private void UpdateInventoryFromDatabase(ClientContext workCTX, string item, scriptItem oWorkItem)
        {
            try
            {

                ClientContext saveContext = GetClientContext(ctx, oWorkItem.GetParm("saveurl"));
                saveContext.Credentials = workCTX.Credentials;

                List<SiteInfo> siItems = SQLDataAccess.LoadData<SiteInfo>("select * from SiteInfo", new Dictionary<string, object>(), "Default");
                List<ListInfo> liItems = SQLDataAccess.LoadData<ListInfo>("select * from ListInfo", new Dictionary<string, object>(), "Default");
                TotalItems = siItems.Count + liItems.Count;
                RunCount = 0;
                sw.Start();
                List spmiSites = saveContext.Web.Lists.GetByTitle("spmiSites");
                List spmiLists = saveContext.Web.Lists.GetByTitle("spmiLists");

                foreach (SiteInfo si in siItems)
                {
                    RunCount++;
                    try
                    {
                        ShowInfo("Processing Site: " + si.SiteUrl);
                        Int32 iParentID = 0;
                        ListItem workParent = spmiSites.GetListItemByField(saveContext, "spmiSiteID", "Text", si.ParentSiteID);
                        if (workParent != null)
                        {
                            iParentID = workParent.Id;
                        }

                        UpdateSiteInfo(saveContext, spmiSites, si, iParentID);
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "Error Processing Site: " + si.SiteUrl, "");
                    }
                    UpdateProgress();
                }
                foreach (ListInfo li in liItems)
                {
                    RunCount++;
                    try
                    {

                        Int32 iParentID = 0;
                        ListItem parentItem = spmiSites.GetListItemByField(saveContext, "spmiSiteID", "Text", li.SiteID);
                        if (parentItem != null)
                        {
                            iParentID = parentItem.Id;
                        }
                        else
                        {
                            System.Diagnostics.Trace.WriteLine(li.SiteUrl + "/" + li.Title);
                        }
                        UpdateListInfo(saveContext, spmiLists, li, iParentID);
                        ShowInfo("Processing List: " + li.SiteUrl + "/" + li.Title);
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "Error Processing List: " + li.SiteUrl + "/" + li.Title, "");
                    }
                    UpdateProgress();
                }
            }

            catch (Exception ex)
            {
                ShowError(ex, "Update Inventory from Database", "");
            }
        }

        private void UpdateListInfo(ClientContext saveContext, List spmiLists, ListInfo li, Int32 iParentID)
        {
            try
            {
                ListItem itm = spmiLists.GetListItemByField(saveContext, "spmiListID", "Text", li.ListID);
                if (itm == null)
                {
                    ListItemCreationInformation lici = new ListItemCreationInformation();
                    itm = spmiLists.AddItem(lici);
                    itm["spmiListID"] = li.ListID;
                    itm.Update();
                    saveContext.ExecuteQuery();
                }
                itm["spmiItemCount"] = li.ItemCount;
                itm["spmiLastItemModified"] = li.LastItemModified;
                itm["spmiLastScan"] = li.LastScan;
                //itm["spmiLastUserItemModified"] = li.LastUserItemModified;
                //itm["spmiParentSite"] =li.p

                if (iParentID > 0)
                {
                    FieldLookupValue lv = new FieldLookupValue();
                    lv.LookupId = iParentID;
                    itm["spmiParentSite"] = lv;
                }


                itm["spmiPermissions"] = li.Permissions;
                itm["spmiPermissionsLastScan"] = li.LastScan;
                itm["spmiSiteID"] = li.SiteID;
                itm["spmiSiteUrl"] = li.SiteUrl;
                itm["Title"] = li.Title;
                itm["spmiUniquePermissions"] = li.UniquePermissions;

                itm.Update();
                saveContext.ExecuteQuery();

            }
            catch (Exception ex)
            {
                ShowError(ex, "UpdateListInfo", "");
            }
        }

        private void UpdateSiteInfo(ClientContext saveContext, List spmiSites, SiteInfo si, Int32 iParentID)
        {
            try
            {
                ListItem itm = spmiSites.GetListItemByField(saveContext, "spmiSiteID", "Text", si.SiteID);
                if (itm == null)
                {
                    ListItemCreationInformation lici = new ListItemCreationInformation();
                    itm = spmiSites.AddItem(lici);
                    itm["spmiSiteID"] = si.SiteID;
                    itm["spmiSiteUrl"] = si.SiteUrl;
                    itm["Title"] = si.Title;
                    itm.Update();
                    saveContext.ExecuteQuery();
                }

                ////itm["spmiAccessRequestEmail"] = si.AccessRequestEmail;
                ////itm["spmiAdministrators"] = si.Adminsitrators;
                ////itm["spmiFullControlUsers"] = si.FullControlUsers;
                ////itm["spmiFullControlUsersCount"]=si.FullControlUserCount;
                itm["spmiHasSubSites"] = si.HasSubSites;
                itm["spmiLastItemModified"] = si.LastItemModified;
                itm["spmiLastScan"] = si.LastScan;
                itm["spmiLastUserItemModified"] = si.LastUserItemModified;

                if (iParentID > 0)
                {
                    FieldLookupValue lv = new FieldLookupValue();
                    lv.LookupId = iParentID;
                    itm["spmiParentSite"] = lv;
                }

                itm["spmiPermissions"] = si.Permissions;
                itm["spmiPermissionsLastScan"] = si.LastScan;
                ////itm["spmiRequestApprovers"] = si.RequestApprovers;


                itm["Title"] = si.Title;
                itm["spmiUniquePermissions"] = si.UniquePermissions;
                itm["spmiUserCount"] = si.UserCount;


                itm.Update();
                saveContext.ExecuteQuery();
            }
            catch (Exception ex)
            {
                ShowError(ex, "UpdateSiteInfo", "");
            }
        }


        private void EnsureTargetFields(ClientContext workCTX, string item, scriptItem oWorkItem)
        {
            try
            {
                string viewXML = "<View><Query><OrderBy><FieldRef Name='spmiSiteUrl' Ascending='TRUE'/></OrderBy><Where><And><IsNotNull><FieldRef Name='spmiTargetAction' /></IsNotNull><Neq><FieldRef Name = 'spmiTargetAction' /><Value Type = 'Text'>none</Value></Neq></And></Where></Query><RowLimit>5000</RowLimit></View>";
                List lstLists = ctx.Web.Lists.GetByTitle("spmiLists");
                CamlQuery oListQuery = new CamlQuery();
                oListQuery.ViewXml = viewXML;
                ListItemCollection listitems = lstLists.GetItems(oListQuery);
                ctx.Load(listitems, itms => itms.Include(i => i.FieldValuesAsText, i => i.Id, i => i["Title"], i => i["spmiSiteUrl"], i => i["TargetList"], i => i["spmiTargetAction"], i => i["spmiTargetLocation"], i => i["spmiSiteID"]));
                ctx.ExecuteQuery();
                RunCount = 0;
                TotalItems = listitems.Count;
                sw.Start();
                List<ListInfo> listData = new List<ListInfo>();
                foreach (ListItem itm in listitems)
                {
                    ListInfo li = BuildListInfo(itm);
                    ShowInfo(li.SiteUrl + "/" + li.Title);
                    UpdateProgress();
                    ClientContext srcCTX = new ClientContext(li.SiteUrl);
                    srcCTX.Credentials = ctx.Credentials;
                    string cTargetUrl = li.TargetLocation.Substring(0, li.TargetLocation.LastIndexOf("/"));
                    string cTargetList = li.TargetLocation.Substring(li.TargetLocation.LastIndexOf("/") + 1);

                    ClientContext tgtCTX = new ClientContext(li.SiteUrl);
                    tgtCTX.Credentials = ctx.Credentials;
                    FieldCollection tgtFields = tgtCTX.Web.AvailableFields;
                    List tgtList = tgtCTX.Web.Lists.GetByTitle(itm["TargetList"].ToString());
                    tgtCTX.Load(tgtList);
                    tgtCTX.Load(tgtFields);
                    tgtCTX.ExecuteQuery();

                    FieldCollection srcFields = srcCTX.Web.Lists.GetByTitle(li.Title).Fields;
                    srcCTX.Load(srcFields, sfs => sfs.Include(sf => sf.Id, sf => sf.InternalName, sf => sf.Description, sf => sf.Hidden, sf => sf.Title, sf => sf.TypeAsString, sf => sf.Group));
                    srcCTX.ExecuteQuery();
                    foreach (Field srcFld in srcFields)
                    {
                        try
                        {
                            if (srcFld.InternalName == "Title" && srcFld.Title != srcFld.InternalName)
                            {
                                System.Diagnostics.Trace.WriteLine("");
                            }
                            if (srcFld.Hidden == false && ProcessField(srcFld.InternalName) == true)
                            {
                                if (!tgtList.HasField(srcFld.Title))
                                {
                                    if (!tgtFields.HasField(srcFld.Title))
                                    {
                                        EnsureSourceFieldInTargetWeb(srcCTX, srcFld, tgtFields);
                                    }
                                    Field workField = tgtFields.GetByInternalNameOrTitle(srcFld.InternalName);
                                    tgtCTX.Load(workField);
                                    tgtCTX.ExecuteQuery();

                                    if (!tgtList.Fields.HasField(workField.Id))
                                    {
                                        tgtList.Fields.Add(workField);
                                        tgtList.Update();
                                        tgtCTX.ExecuteQuery();
                                    }

                                    workField = tgtList.Fields.GetByInternalNameOrTitle(srcFld.InternalName);
                                    tgtCTX.Load(workField);
                                    tgtCTX.ExecuteQuery();

                                    if (workField.Title != srcFld.Title)
                                    {
                                        if (workField.Title == "Title")
                                        {
                                            tgtList.Fields.AddField(srcFld.Title, srcFld.Title, srcFld.Description, srcFld.Group);
                                        }
                                        else
                                        {
                                            workField.Title = srcFld.Title;
                                            workField.Update();
                                            tgtList.Update();
                                            tgtCTX.ExecuteQuery();

                                        }

                                    }


                                }



                            }

                        }
                        catch (Exception ex)
                        {
                            ShowError(ex, "Ensure-TargetFields - " + srcFld.Title, "");
                        }


                    }





                }


            }
            catch (Exception ex)
            {
                ShowError(ex, "", "");

            }




        }

        private bool ProcessField(string cFieldName)
        {
            bool bRetVal = true;
            switch (cFieldName)
            {
                case "Attachments":
                    bRetVal = false;
                    break;
            }
            return bRetVal;
        }

        private void EnsureSourceFieldInTargetWeb(ClientContext srcCTX, Field srcFld, FieldCollection tgtFields)
        {
            try
            {
                switch (srcFld.TypeAsString)
                {
                    case "Text":
                        tgtFields.EnsureField(srcFld.InternalName, srcFld.Title, srcFld.Description, srcFld.Group);
                        break;
                    case "Number":
                        tgtFields.EnsureFieldNumber(srcFld.InternalName, srcFld.Title, srcFld.Description, srcFld.Group);
                        break;
                    case "Boolean":
                        tgtFields.EnsureFieldBoolean(srcFld.InternalName, srcFld.Title, srcFld.Description, srcFld.Group);
                        break;
                    case "Choice":
                        FieldChoice fc = (FieldChoice)srcFld;
                        srcCTX.Load(fc);
                        srcCTX.ExecuteQuery();

                        tgtFields.EnsureFieldChoice(srcFld.InternalName, srcFld.Title, srcFld.Description, srcFld.Group, fc.Choices);
                        break;
                    case "Currency":
                        tgtFields.EnsureFieldCurrency(srcFld.InternalName, srcFld.Title, srcFld.Description, srcFld.Group);
                        break;
                    case "DateTime":
                        tgtFields.EnsureFieldDateTime(srcFld.InternalName, srcFld.Title, srcFld.Description, srcFld.Group);
                        break;
                    case "Note":
                        tgtFields.EnsureFieldNote(srcFld.InternalName, srcFld.Title, srcFld.Description, srcFld.Group);
                        break;

                    case "Computed":
                        FieldComputed fcomp = (FieldComputed)srcFld;
                        srcCTX.Load(fcomp, f => f.TypeAsString, f => f.Title, f => f.InternalName, f => f.Description, f => f.Group);
                        srcCTX.ExecuteQuery();

                        string cCalculation = "";
                        tgtFields.EnsureFieldComputed(fcomp.TypeAsString, srcFld.InternalName, srcFld.Title, srcFld.Description, srcFld.Group, cCalculation);
                        break;

                    case "URL":
                        tgtFields.EnsureFieldUrl(srcFld.TypeAsString, srcFld.InternalName, srcFld.Title, srcFld.Description, srcFld.Group);
                        break;

                    case "Integer":
                        tgtFields.EnsureFieldInteger(srcFld.InternalName, srcFld.Title, srcFld.Description, srcFld.Group);
                        break;
                    case "HTML":
                        tgtFields.EnsureFieldHTML(srcFld.TypeAsString, srcFld.InternalName, srcFld.Title, srcFld.Description, srcFld.Group);
                        break;



                    default:
                        System.Diagnostics.Trace.WriteLine(srcFld.TypeAsString);
                        break;


                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "", "");
            }
        }

        private ListInfo BuildListInfo(ListItem itm)
        {

            ListInfo li = new ListInfo();
            try
            {
                li.SiteUrl = itm["spmiSiteUrl"].ToString();
                li.Title = itm["Title"].ToString();
                li.SiteID = itm["spmiSiteID"].ToString();
                if (itm["spmiTargetAction"] != null)
                {
                    li.TargetAction = itm["spmiTargetAction"].ToString();
                }
                if (itm["spmiTargetLocation"] != null)
                {
                    li.TargetLocation = itm["spmiTargetLocation"].ToString();
                }
                li.Id = itm.Id;
                li.TargetList = itm["TargetList"].ToString();

            }
            catch (Exception ex)
            {
                ShowError(ex, "Build ListInfo", "");
            }
            return li;
        }
        private void EnsureTargetFolders(ClientContext workCTX, string item, scriptItem oWorkItem)
        {
            try
            {
                string viewXML = "<View><Query><OrderBy><FieldRef Name='spmiSiteUrl' Ascending='TRUE'/></OrderBy><Where><And><IsNotNull><FieldRef Name='spmiTargetAction' /></IsNotNull><Neq><FieldRef Name = 'spmiTargetAction' /><Value Type = 'Text'>none</Value></Neq></And></Where></Query><RowLimit>5000</RowLimit></View>";
                List lstLists = ctx.Web.Lists.GetByTitle("spmiLists");
                CamlQuery oListQuery = new CamlQuery();
                oListQuery.ViewXml = viewXML;
                ListItemCollection listitems = lstLists.GetItems(oListQuery);
                ctx.Load(listitems, itms => itms.Include(i => i.FieldValuesAsText, i => i.Id, i => i["Title"], i => i["spmiSiteUrl"], i => i["TargetList"], i => i["spmiTargetAction"], i => i["spmiTargetLocation"], i => i["spmiSiteID"]));
                ctx.ExecuteQuery();
                List<ListInfo> listData = new List<ListInfo>();
                foreach (ListItem itm in listitems)
                {
                    try
                    {
                        ListInfo li = BuildListInfo(itm);
                        //ClientContext srcCTX = new ClientContext(li.SiteUrl);
                        //srcCTX.Credentials = ctx.Credentials;
                        string cTargetUrl = li.TargetLocation.Substring(0, li.TargetLocation.LastIndexOf("/"));
                        string cTargetList = li.TargetLocation.Substring(li.TargetLocation.LastIndexOf("/") + 1);

                        ClientContext tgtCTX = new ClientContext(li.TargetLocation);
                        tgtCTX.Credentials = ctx.Credentials;
                        string cListName = itm["TargetList"].ToString();
                        if (!tgtCTX.Web.HasList(cListName))
                        {
                            tgtCTX.Web.EnsureList(cListName, ListTemplateType.DocumentLibrary, "");
                        }


                        List tgtList = tgtCTX.Web.Lists.GetByTitle(itm["TargetList"].ToString());
                        tgtCTX.Load(tgtList, tls => tls.RootFolder);
                        tgtCTX.ExecuteQuery();
                        string cFolderTitle = itm["Title"].ToString();
                        cFolderTitle = cFolderTitle.Replace("/", "-");
                        tgtList.RootFolder.EnsureFolder(cFolderTitle);
                        ShowProgress("Working Folder:" + cFolderTitle);
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "EnsureTargetFolders - inside", "");

                    }
                }


            }
            catch (Exception ex)
            {
                ShowError(ex, "EnsureTargetFolders", "");

            }


        }


    }
}
