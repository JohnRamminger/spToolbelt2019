using Remotion.Implementation;
using Squirrel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace spToolbelt2019.Forms
{
    public partial class frmUpdateApp : Form
    {
        public frmUpdateApp()
        {
            InitializeComponent();
            
        }


        //private async void UpdateApp()
        //{
        //    try
        //    {
        //        using (var manager = new UpdateManager(textBox1.Text, "spToolBelt2019"))
        //        {
        //            try
        //            {
        //                var releaseEntry = await manager.UpdateApp();
        //                lblNewVersion.Text = releaseEntry.Version.ToString();
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show("Inside: '"+ex.Message);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }


        //}

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            cmdUpdate.Enabled = false;
            var x = UpdateApp();
            cmdUpdate.Enabled = true;
        }

        private void frmUpdateApp_Load(object sender, EventArgs e)
        {
            
        }



        public async Task UpdateApp()
        {
            string updatePath = ConfigurationManager.AppSettings["UpdatePathFolder"];
            string packageId = ConfigurationManager.AppSettings["PackageID"];

            if (string.IsNullOrEmpty(updatePath))
            {
                updatePath = "https://deploy.rammware.net/sptoolbelt2019";
            }
            if (string.IsNullOrEmpty(packageId))
            {
                packageId = "spToolBelt2019";
            }

            using (var mgr = new UpdateManager(updatePath, packageId))
            {
                var updates = await mgr.CheckForUpdate();
                if (updates.ReleasesToApply.Any())
                {
                    var lastVersion = updates.ReleasesToApply.OrderBy(x => x.Version).Last();
                    await mgr.DownloadReleases(new[] { lastVersion });
                    await mgr.ApplyReleases(updates);
                    await mgr.UpdateApp();

                    MessageBox.Show("The application has been updated – please close and restart.");
                }
                else
                {
                    MessageBox.Show("No Updates are available at this time.");
                }
            }
        }

        public static void OnAppUpdate(Version version)
        {
            // Could use this to do stuff here too.
        }

        public static void OnInitialInstall(Version version)
        {
            var exePath = Assembly.GetEntryAssembly().Location;
            string appName = Path.GetFileName(exePath);

            var updatePath = ConfigurationManager.AppSettings["UpdatePathFolder"];
            var packageId = ConfigurationManager.AppSettings["PackageID"];

            if (string.IsNullOrEmpty(updatePath))
            {
                updatePath = "https://deploy.rammware.net/sptoolbelt2019";
            }
            if (string.IsNullOrEmpty(packageId))
            {
                packageId = "spToolBelt2019";
            }


            using (var mgr = new UpdateManager(updatePath, packageId))
            {

                // Create Desktop and Start Menu shortcuts
                
                mgr.CreateShortcutsForExecutable(appName,ShortcutLocation.Desktop, false);
            }
        }

        public static void OnAppUninstall(Version version)
        {
            var exePath = Assembly.GetEntryAssembly().Location;
            string appName = Path.GetFileName(exePath);

            var updatePath = ConfigurationManager.AppSettings["UpdatePathFolder"];
            var packageId = ConfigurationManager.AppSettings["PackageID"];

            if (string.IsNullOrEmpty(updatePath))
            {
                updatePath = "https://deploy.rammware.net/sptoolbelt2019";
            }
            if (string.IsNullOrEmpty(packageId))
            {
                packageId = "spToolBelt2019";
            }



            using (var mgr = new UpdateManager(updatePath, packageId))
            {
                // Remove Desktop and Start Menu shortcuts
                mgr.RemoveShortcutsForExecutable(appName,ShortcutLocation.Desktop);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebClient wc = new WebClient();
            string theTextFile = wc.DownloadString("https://deploy.rammware.net/sptoolbelt2019/RELEASES");

            // print out page source
            MessageBox.Show(theTextFile);
            textBox1.Text = theTextFile;
        }
    }
}
