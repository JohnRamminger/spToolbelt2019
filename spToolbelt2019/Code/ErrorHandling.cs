using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace spToolbelt2019
{
    public static class ErrorHandling
    {
        public static void LogError(Exception ex,string cLocation,string cUserMessage)
        {
            try
            {
                string cMessage = "An Error Occured at: " + cLocation + " " + cUserMessage + " " + ex.Message;
                System.Diagnostics.Trace.WriteLine(cMessage);
                MessageBox.Show(cMessage);
                using (StreamWriter sw = new StreamWriter(ApplicationDeployment.CurrentDeployment.DataDirectory + @"\ErrorLog.txt", true))
                {
                    sw.WriteLine(cMessage);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch 
            {

            }
        }
    }
}
