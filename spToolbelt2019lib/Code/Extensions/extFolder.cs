using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spToolbelt2019Lib
{
    public static class extFolder
    {

        public static bool HasFile(this Folder fld,string cFilename)
        {
            return (fld.GetFileByName(cFilename) != null);
        }

        public static Microsoft.SharePoint.Client.File GetFileByName(this Folder fld, string cFilename)
        {
            try
            {
                FileCollection oFiles = fld.Files;
                fld.Context.Load(oFiles);
                fld.Context.ExecuteQuery();
                foreach (File file in oFiles)
                {
                    if (file.Name.ToLower()==cFilename.ToLower())
                    {
                        return file;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in extFolder.GetFileByName -  " + ex.Message, ex);

            }
            return null;
        }
    }
}
