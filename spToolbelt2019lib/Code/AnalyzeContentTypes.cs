using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;

namespace spToolbelt2019Lib
{
    public class AnalyzeContentTypes
    { 
    

        public List<ContentTypeTracker> cttrackers;
        Web oWeb;
        #region Properties
        BackgroundWorker oWorker;
        ClientContext ctx;
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

        public delegate void WorkerErrorHandler(string cExceptionMessage, string cLocation, string cMessage);
        public event WorkerErrorHandler Error;



        #endregion

        #region Public Methods

        public void Start(ClientContext oWorkContext,Web oWorkWeb)
        {
            cttrackers = new List<ContentTypeTracker>();
            ctx = oWorkContext;
            oWeb = oWorkWeb;

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

        public void Cancel()
        {
            oWorker.CancelAsync();
        }

        #endregion

        #region Worker Methods

        void oWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            ScanContentTypes(oWeb);



        }

        private void ScanContentTypes(Web oWorkWeb)
        {
            try
            {
                ctx.Load(oWorkWeb);
                ctx.ExecuteQuery();
                ShowProgress(oWorkWeb.Url);
                WebCollection oWebs = oWorkWeb.Webs;
                ctx.Load(oWebs);
                ctx.ExecuteQuery();

                foreach (Web oWeb in oWebs)
                {
                    ScanContentTypes(oWeb);
                }

                ListCollection olists = oWorkWeb.Lists;
                ctx.Load(olists);
                ctx.ExecuteQuery();
                foreach (List olist in olists)
                {
                    try
                    {
                        ShowInfo(olist.Title);
                        CamlQuery oQuery = CamlQuery.CreateAllItemsQuery(2000);

                        ListItemCollection items = olist.GetItems(oQuery);
                        ctx.Load(items);
                        ctx.ExecuteQuery();
                        foreach (ListItem item in items)
                        {
                            ctx.Load(item.ContentType);
                            ctx.ExecuteQuery();
                            IncrementTracker(item.ContentType.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "frmAnalyzeContentTypes.ScanContentTypes", "Inside");
                    }

                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "frmAnalyzeContentTypes.ScanContentTypes",ex.StackTrace.ToString());
            }
        }


        private void IncrementTracker(string name)
        {
            try
            {
                ContentTypeTracker ctt = GetContentTypeTracker(name);
                if (ctt == null)
                {
                    ContentTypeTracker newtracker = new ContentTypeTracker() { ContentTypeName = name, UseCount = 1 };
                    cttrackers.Add(newtracker);
                }
                else ctt.UseCount++;
            }
            catch (Exception ex)
            {
                ShowError(ex, "frmAnalyzeContentTypes.IncrementTracker", "");
            }
        }

        private ContentTypeTracker GetContentTypeTracker(string name)
        {
            foreach (ContentTypeTracker ctt in cttrackers)
            {
                if (ctt.ContentTypeName == name) return ctt;
            }
            return null;
        }

        void oWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case -1:
                    string cParms = (string)e.UserState;
                    string[] parms = cParms.Split('|');
                    Error(parms[0], parms[1], parms[2]);
                    break;
                case 1:
                    Progress((string)e.UserState);
                    break;
                case 2:
                    Info((string)e.UserState);
                    break;
            }
        }

        void oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (oWorker.CancellationPending)
            {
                if (this.Canceled != null) Canceled();
                return;
            }
            if (this.Complete != null) Complete();
        }
        #endregion

        #region ProgressMethods

        void ShowProgress(string cProgress)
        {
            oWorker.ReportProgress(1, cProgress);
        }

        void ShowInfo(string cInfo)
        {
            oWorker.ReportProgress(2, cInfo);
        }

        void ShowError(Exception ex, string cLocation, string cMessage)
        {
            oWorker.ReportProgress(-1, String.Format("{0}|{1}|{2}", ex.Message, cLocation, cMessage));
        }

        #endregion

    }
}
