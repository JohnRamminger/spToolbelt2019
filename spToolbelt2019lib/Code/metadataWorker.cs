using LumenWorks.Framework.IO.Csv;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.SharePoint.Client.Utilities;
using Microsoft.SharePoint.Client.WebParts;
using Microsoft.SharePoint.Client.WorkflowServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class metadataWorker
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

        #region ProgressMethods
        void ShowSiteInfo(string cSiteInfo)
        {
            oWorker.ReportProgress(3, cSiteInfo);
            oOutputFile.WriteLine("SiteInfo: " + cSiteInfo);
        }
        void ShowProgress(string cProgress)
        {
            oWorker.ReportProgress(1, cProgress);
            //oOutputFile.WriteLine("Progress: " + cProgress);
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
        public void Start(ClientContext workerCTX)
        {
            ctx = workerCTX;
            if (oWorker == null) oWorker = new BackgroundWorker();

            if (!oWorker.IsBusy)
            {
                oWorker.WorkerReportsProgress = true;
                oWorker.WorkerSupportsCancellation = true;
                //oWorker.RunWorkerCompleted += oWorker_RunWorkerCompleted;
                //oWorker.ProgressChanged += oWorker_ProgressChanged;
                oWorker.DoWork += oWorker_DoWork;
                oWorker.RunWorkerAsync();
            }

        }



        #endregion

        #region Worker Methods

        private KeywordQuery GetQuery(string cQuery,string cProp,int StartRpw)
        {
            KeywordQuery kq = new KeywordQuery(ctx);
            try
            {
                kq.QueryText = cQuery;

                kq.StartRow = 0;
                kq.RowLimit = 500;
                kq.RowsPerPage = 500;
                kq.Timeout = 60000;

                string[] mp = cProp.Split(',');
                foreach (string p in mp)
                {
                    kq.SelectProperties.Add(p);
                }
                kq.TrimDuplicates = false;
            } catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            return kq;
        }

        private ResultTable GetResults(KeywordQuery kq)
        {
            try
            { 
            SearchExecutor se = new SearchExecutor(ctx);
            ClientResult<ResultTableCollection> results = se.ExecuteQuery(kq);
            ctx.ExecuteQuery();
            return results.Value.FirstOrDefault(v => v.TableType.Equals(KnownTableTypes.RelevantResults));
            } catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return null;
            }
        }


        void oWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int StartRow = 0;
            try
            {
                string mp = "Title,Path,SPSiteURL,ListItemID,ListID,UniqueID,Language";
                KeywordQuery kq = GetQuery("ContentType:gbBook", mp, 0);
                ResultTable firstResults = GetResults(kq);
                List<dynamic> items = new List<dynamic>();
                items = UpdateItems(items, firstResults,mp);




                //if (firstResults.RowCount != firstResults.TotalRows)
                //{
                //    while ((StartRow + 500) < firstResults.TotalRows)
                //    {
                //        StartRow += 500;
                //        ShowProgress("Starting At: " + StartRow);
                //        KeywordQuery nestedkq = GetQuery("ContentType:gbBook", "Title;Path", StartRow);
                //        ResultTable nestedResults = GetResults(kq);
                //        items = UpdateItems(items, nestedResults,mp);
                //    }

                //}


                System.Diagnostics.Trace.WriteLine(firstResults.TotalRows);
                List lst = ctx.Web.Lists.GetByTitle("BookInfo");
                ctx.Load(lst);
                ctx.ExecuteQuery();
                foreach (IDictionary<string,object> item in firstResults.ResultRows)
                {

                    string id = item["UniqueID"].ToString();

                    ListItem itm = lst.GetItemByField(id, "ItemUniqueId");
                    if (itm==null)
                    {
                        ListItemCreationInformation lici = new ListItemCreationInformation();
                        ListItem newItem = lst.AddItem(lici);
                        newItem["ItemUniqueId"] = id;
                    }
                    itm["Title"] = item["Title"].ToString();
                    itm["Language"] = item["Language"].ToString();
                    itm.Update();
                    ctx.ExecuteQuery();

                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "metadataWorker.oWorker_DoWork", "");
            }
        }

        private List<dynamic> UpdateItems(List<dynamic> items, ResultTable rt,string mp)
        {
            foreach (IDictionary<string, object> item in rt.ResultRows)
            {
                dynamic mi = new JObject();
                string[] props = mp.Split(',');
                foreach(string p in props)
                {
                    if (item[p] != null)
                    {
                        mi[p] = item[p].ToString();
                    }
                }
                items.Add(mi);
                System.Diagnostics.Trace.WriteLine(item["Path"].ToString());
            }
            return items;
        }




        #endregion


    }
}
