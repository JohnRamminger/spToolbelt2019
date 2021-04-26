
using Microsoft.SharePoint.Client;
using System;


namespace spToolbelt2019lib.Code.Extensions
{
    public static class extGroup
    {
        public static void EnsureUser(this Group grp,Web oWeb,string cLoginName)
        {
            if (!grp.HasUser(oWeb,cLoginName))
            {
                grp.AddUser(oWeb, cLoginName);
            }
        }
        public static bool HasUser(this Group grp,Web oWeb,string cLoginName)
        {
            try
            {
                grp.Context.Load(grp, g => g.Users);
                grp.Context.ExecuteQuery();
                foreach(User usr in grp.Users)
                {
                    if (usr.LoginName==cLoginName)
                    {
                        return true;
                    }
                }
                
            } catch (Exception ex)
            {
                throw new Exception("Error finding: " + cLoginName + " - " + ex.Message);
            }


            return false;

        }

        public static void AddUser(this Group grp, Web oWeb, string cLoginName)
        {
            try
            {
                User oUser = oWeb.EnsureUser(cLoginName);
                grp.Users.AddUser(oUser);
                grp.Update();
                oWeb.Context.ExecuteQuery();
            } catch(Exception ex)
            {
                throw new Exception("Error adding: " + cLoginName + " - " + ex.Message);
            }

        }
    }
}
