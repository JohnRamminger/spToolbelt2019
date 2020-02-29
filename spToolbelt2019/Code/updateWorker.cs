using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace spToolbelt2019
{
    public class updateWorker
    {
        #region Properties
        BackgroundWorker oWorker;
        string cStartLoc = "";
        string cCommand = "";
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

        public void Start(string cLocation,string command)
        {
            cCommand = command;
            cStartLoc = cLocation;
            if (oWorker == null)
            {
                oWorker = new BackgroundWorker();
            }

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
            if (cCommand=="Update")
            { 
               DirectoryInfo di = new DirectoryInfo(cStartLoc);
                List<string> locations = ScanDrive(di,"*.config");
                //UpdateConfigFile(fi.FullName);
            } 
            if (cCommand=="Find")
            {
                DirectoryInfo di = new DirectoryInfo(cStartLoc);
                List<string> locations = ScanDrive(di, "*anglesharp.dll");

            }
        }

        private List<string> ScanDrive(DirectoryInfo di,string cPattern)
        {
            ShowInfo(di.FullName);
            List<string> foundFiles = new List<string>();
            try
            {
                foreach (DirectoryInfo diChild in di.GetDirectories())
                {
                    List<string> childFound = ScanDrive(diChild, cPattern);
                    foreach (string item in childFound)
                    {
                        foundFiles.Add(item);
                    }

                }
                foreach (FileInfo fi in di.GetFiles(cPattern))
                {
                    foundFiles.Add(fi.FullName);
                    string cFileText = File.ReadAllText(fi.FullName);
                    //if (fi.FullName.ToLower().Contains("web.config"))
                    {
        
                    }
                    ShowProgress(fi.FullName);
                    //if (cFileText.ToLower().Contains("anglesharp"))
                    //{
                    //    ShowProgress(fi.FullName);
                    //}

                }

            }
            catch (Exception ex)
            {
                ShowError(ex, "ScanDrive", di.FullName);
            }
            return foundFiles;
        }

        private void UpdateConfigFile(string cFileName)
        {
            try
            {
                EnsureDependentAssembly(cFileName, "AngleSharp", "", "", "");


                string savedir = @"c:\temp\configFileUpdates";
                if (!Directory.Exists(savedir))
                {
                    Directory.CreateDirectory(savedir);
                }
                //string cRestoreFile = cFileName + ".bak";
                //File.Copy(cRestoreFile, cFileName,true);
                //return;
                string cSafeName = cFileName.Replace(":", "-").Replace("\\", "-");
                File.Copy(cFileName, savedir + @"\" + cSafeName);
                File.Copy(cFileName, cFileName + ".bak");

                //string configFile = SPUtility.GetGenericSetupPath("TEMPLATE").ToLower().Replace("\\template", "\\bin") + "\\OWSTIMER.EXE.CONFIG";
                //string XMLData = System.IO.File.ReadAllText(configFile, Encoding.UTF8);
                XmlDocument config = new XmlDocument();
                config.Load(cFileName);

                XmlNode cfg = config.SelectSingleNode("configuration");
                if (cfg == null)
                {
                    cfg = config.CreateNode(XmlNodeType.Element, "configuration", "");
                    config.AppendChild(cfg);
                }



                XmlNode runtime = config.SelectSingleNode("configuration/runtime");
                if (runtime==null)
                {
                    runtime = config.CreateNode(XmlNodeType.Element, "runtime", "");
                    config.SelectSingleNode("configuration").AppendChild(runtime);
                }



                //ensure assemblyBinding exists
                XmlNode assemblyBinding = config.SelectSingleNode("configuration/runtime/*[local-name()='assemblyBinding' and namespace-uri()='urn:schemas-microsoft-com:asm.v1']");

                if (assemblyBinding == null)
                {
                    assemblyBinding = config.CreateNode(XmlNodeType.Element, "assemblyBinding", "urn:schemas-microsoft-com:asm.v1");
                    config.SelectSingleNode("configuration/runtime").AppendChild(assemblyBinding);
                }

                //Delete old entrees if exist
                XmlElement current = assemblyBinding.FirstChild as XmlElement;
                while (current != null)
                {
                    XmlElement elmToRemove = null;
                    if (current.FirstChild != null)
                    {
                        var asmIdn = (current.FirstChild as XmlElement);
                        if (asmIdn.GetAttribute("name").ToLower().Equals("anglesharp"))
                            elmToRemove = current;
                    }
                    current = current.NextSibling as XmlElement;

                    if (elmToRemove != null)
                        assemblyBinding.RemoveChild(elmToRemove);
                }

                XmlElement dependentAssembly = null;
                if (dependentAssembly == null)//create it
                {
                    dependentAssembly = config.CreateElement("dependentAssembly");
                    dependentAssembly.InnerXml = "<assemblyIdentity name = \"AngleSharp\" publicKeyToken = \"e83494dcdc6d31ea\" culture = \"neutral\" />" +
                                                 "<bindingRedirect oldVersion = \"0.0.0.0-0.13.0.0\" newVersion = \"0.13.0.0\" />";
                    assemblyBinding.AppendChild(dependentAssembly);
                }


                config.LoadXml(config.OuterXml.Replace("xmlns=\"\"", ""));
                config.Save(cFileName);
            }
            catch (Exception ex) {
                ShowError(ex, "UpdateConfigFile", ex.Message);
            }



        }



        private void EnsureDependentAssembly(string cFileName,string cAssemblyName,string cPublicToken,string cLowerVerions,string cHigherVersion)
        {
            try
            {
                File.Copy(cFileName, cFileName + ".bak");
                XmlDocument config = new XmlDocument();
                config.Load(cFileName);

                XmlNode cfg = config.SelectSingleNode("configuration");
                if (cfg == null)
                {
                    cfg = config.CreateNode(XmlNodeType.Element, "configuration", "");
                    config.AppendChild(cfg);
                }



                XmlNode runtime = config.SelectSingleNode("configuration/runtime");
                if (runtime == null)
                {
                    runtime = config.CreateNode(XmlNodeType.Element, "runtime", "");
                    config.SelectSingleNode("configuration").AppendChild(runtime);
                }



                //ensure assemblyBinding exists
                XmlNode assemblyBinding = config.SelectSingleNode("configuration/runtime/*[local-name()='assemblyBinding' and namespace-uri()='urn:schemas-microsoft-com:asm.v1']");

                if (assemblyBinding == null)
                {
                    assemblyBinding = config.CreateNode(XmlNodeType.Element, "assemblyBinding", "urn:schemas-microsoft-com:asm.v1");
                    config.SelectSingleNode("configuration/runtime").AppendChild(assemblyBinding);
                }

                //Delete old entrees if exist
                XmlElement current = assemblyBinding.FirstChild as XmlElement;
                while (current != null)
                {
                    XmlElement elmToRemove = null;
                    if (current.FirstChild != null)
                    {
                        var asmIdn = (current.FirstChild as XmlElement);
                        if (asmIdn.GetAttribute("name").ToLower().Equals(cAssemblyName.ToLower()))
                            elmToRemove = current;
                    }
                    current = current.NextSibling as XmlElement;

                    if (elmToRemove != null)
                        assemblyBinding.RemoveChild(elmToRemove);
                }

                XmlElement dependentAssembly = null;
                if (dependentAssembly == null)//create it
                {
                    dependentAssembly = config.CreateElement("dependentAssembly");
                    dependentAssembly.InnerXml = "<assemblyIdentity name = \""+cAssemblyName.ToLower()+"\" publicKeyToken = \""+cPublicToken+"\" culture = \"neutral\" />" +
                                                 "<bindingRedirect oldVersion = \""+cLowerVerions+"-"+cHigherVersion+"\" newVersion = \""+cHigherVersion+"\" />";
                    assemblyBinding.AppendChild(dependentAssembly);
                }


                config.LoadXml(config.OuterXml.Replace("xmlns=\"\"", ""));
                config.Save(cFileName);
            }
            catch (Exception ex)
            {
                ShowError(ex, "UpdateConfigFile", ex.Message);
            }


        }


        private void UpdateConfigFileOld(string cFileName)
        {
            try
            {
                string savedir = @"c:\temp\configFileUpdates";
                if (!Directory.Exists(savedir))
                {
                    Directory.CreateDirectory(savedir);
                }
                //string cRestoreFile = cFileName + ".bak";
                //File.Copy(cRestoreFile, cFileName,true);
                //return;
                string cSafeName = cFileName.Replace(":", "-").Replace("\\", "-");
                File.Copy(cFileName, savedir + @"\" + cSafeName);
                File.Copy(cFileName, cFileName + ".bak");

                //string configFile = SPUtility.GetGenericSetupPath("TEMPLATE").ToLower().Replace("\\template", "\\bin") + "\\OWSTIMER.EXE.CONFIG";
                //string XMLData = System.IO.File.ReadAllText(configFile, Encoding.UTF8);
                XmlDocument config = new XmlDocument();
                config.Load(cFileName);

                XmlNode cfg = config.SelectSingleNode("configuration");
                if (cfg == null)
                {
                    cfg = config.CreateNode(XmlNodeType.Element, "configuration", "");
                    config.AppendChild(cfg);
                }



                XmlNode runtime = config.SelectSingleNode("configuration/runtime");
                if (runtime == null)
                {
                    runtime = config.CreateNode(XmlNodeType.Element, "runtime", "");
                    config.SelectSingleNode("configuration").AppendChild(runtime);
                }



                //ensure assemblyBinding exists
                XmlNode assemblyBinding = config.SelectSingleNode("configuration/runtime/*[local-name()='assemblyBinding' and namespace-uri()='urn:schemas-microsoft-com:asm.v1']");

                if (assemblyBinding == null)
                {
                    assemblyBinding = config.CreateNode(XmlNodeType.Element, "assemblyBinding", "urn:schemas-microsoft-com:asm.v1");
                    config.SelectSingleNode("configuration/runtime").AppendChild(assemblyBinding);
                }

                //Delete old entrees if exist
                XmlElement current = assemblyBinding.FirstChild as XmlElement;
                while (current != null)
                {
                    XmlElement elmToRemove = null;
                    if (current.FirstChild != null)
                    {
                        var asmIdn = (current.FirstChild as XmlElement);
                        if (asmIdn.GetAttribute("name").ToLower().Equals("anglesharp"))
                            elmToRemove = current;
                    }
                    current = current.NextSibling as XmlElement;

                    if (elmToRemove != null)
                        assemblyBinding.RemoveChild(elmToRemove);
                }

                XmlElement dependentAssembly = null;
                if (dependentAssembly == null)//create it
                {
                    dependentAssembly = config.CreateElement("dependentAssembly");
                    dependentAssembly.InnerXml = "<assemblyIdentity name = \"AngleSharp\" publicKeyToken = \"e83494dcdc6d31ea\" culture = \"neutral\" />" +
                                                 "<bindingRedirect oldVersion = \"0.0.0.0-0.13.0.0\" newVersion = \"0.13.0.0\" />";
                    assemblyBinding.AppendChild(dependentAssembly);
                }


                config.LoadXml(config.OuterXml.Replace("xmlns=\"\"", ""));
                config.Save(cFileName);
            }
            catch (Exception ex)
            {
                ShowError(ex, "UpdateConfigFile", ex.Message);
            }


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
