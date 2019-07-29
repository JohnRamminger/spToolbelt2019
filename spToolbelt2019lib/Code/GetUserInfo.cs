using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace spToolbelt2019Lib
{
    public class GetUserInfo
    {
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

        public delegate void UserInfoHandler(User oUser);
        public event UserInfoHandler UserInfo;



        #endregion

        #region Public Methods

        public void Start(ClientContext workCtx)
        {
            ctx = workCtx;
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
            try
            {
                GroupCollection grps = ctx.Site.RootWeb.SiteGroups;
                ctx.Load(grps);
                ctx.ExecuteQuery();
                foreach (Group grp in grps)
                {
                    ShowInfo(grp.Title);
                    UserCollection oUsers = grp.Users;
                    ctx.Load(oUsers);
                    ctx.ExecuteQuery();
                    foreach (User oUser in oUsers)
                    {
                        ctx.Load(oUser);
                        ctx.ExecuteQuery();
                        oWorker.ReportProgress(-100, oUser);
                    }
                }


            }
            catch (Exception ex)
            {
                ShowError(ex, "GetUserInfo.oWorker_DoWork", "");
            }
        }

        void oWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case -100:
                    if (UserInfo!=null) UserInfo((User)e.UserState);
                    break;
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
