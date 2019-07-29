using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spToolbelt2019Lib
{
    public static class extListItem
    {

        public static bool HasGuid(this ListItem itm, string cGuid)
        {
            try
            {
                itm.Context.Load(itm);
                itm.Context.ExecuteQuery();
                if (itm["UniqueId"].ToString().ToLower() == cGuid.ToLower())
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in extList.HasGuid -  " + ex.Message, ex);
            }
            return false;
        }
        public static bool ContainsGuidPart(this ListItem itm, string cGuid)
        {
            try
            {
                
                itm.Context.Load(itm);
                itm.Context.ExecuteQuery();
                if (itm["UniqueId"].ToString().ToLower().Contains(cGuid.ToLower()))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in extList.ContainsGuidPart -  " + ex.Message, ex);
            }
            return false;
        }



    }
}
