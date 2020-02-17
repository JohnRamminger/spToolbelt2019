using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace spToolbelt2019Lib
{
    public class scriptItem
    {
        public int Order { get; set; }
        public string RawLine { get; set; }
        public string Command { get; set; }
        public bool Verified { get; set; }
        public Dictionary<string,string> parms { get; set; }
        public string Url { get; set; }
        public bool TestMode { get; set; }
        public string Status { get; set; }

        public scriptItem(string cRawLine)
        {
            try
            {

                if (cRawLine.Contains("(") && cRawLine.Contains(")"))
                {
                    LoadItem(cRawLine);
                    Verified = ItemValid();
                }
                else
                {
                    throw new Exception("Script Item not implemented correctly.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in Constructor: {0} - {1}", cRawLine, ex.Message));
            }

        }


        public string GetParm(string cParmKey)
        {
            foreach (KeyValuePair<string, string> parm in parms)
            {
                if (parm.Key.ToLower() == cParmKey.ToLower())
                    return parm.Value;
            }
            return "";
        }
        private bool ItemValid()
        {
            bool bRetVal = true;
            try
            {
                switch (Command)
                {
                    case "save-template":
                        if (parms.ContainsKey("url") &&
                            parms.ContainsKey("templatefile"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;


                        break;
                    case "ensure-targetfolders":
                        bRetVal = true;
                        break;

                    case "ensure-targetfields":
                        bRetVal = true;



                        break;
                    case "update-inventorylistitems":
                        if (parms.ContainsKey("scanurl") &&
                            parms.ContainsKey("saveurl"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;


                        
                    case "update-inventoryfromdatabase":
                        if (parms.ContainsKey("saveurl"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;


                    case "update-inventory":
                        if (parms.ContainsKey("scanurl") &&
                            parms.ContainsKey("saveurl"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;

                    case "set-sitereadonly":
                    case "find-everyone":
                    case "enable-accessrequestsearch":
                    case "fix-encryptedtext":
                    case "update-userinformation":
                    case "update-sitepermissions":
                    case "poll-accessrequests":
                        bRetVal = true;
                        break;
                    case "update-listpermissions":
                        bRetVal = true;
                        break;
                    case "find-customizations":
                        bRetVal = true;
                        break;
                    case "remove-contenttype":
                        if (parms.ContainsKey("url") &&
                            parms.ContainsKey("contenttype"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;

                    case "remove-list":
                        if (parms.ContainsKey("url") &&
                            parms.ContainsKey("listname"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;

                    case "remove-sitecolumnsbygroup":
                        if (parms.ContainsKey("url") &&
                            parms.ContainsKey("group"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;
                    case "set-fielddefaultvalue":
                        if (parms.ContainsKey("url") &&
                            parms.ContainsKey("listname") &&
                            parms.ContainsKey("fieldname") &&
                            parms.ContainsKey("defaultvalue"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;


                    case "set-fieldrequired":
                        if (parms.ContainsKey("url") &&
                            parms.ContainsKey("listname") &&
                            parms.ContainsKey("fieldname") &&
                            parms.ContainsKey("required"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;

                    case "navigation-list":
                    case "sync-list":
                        if (parms.ContainsKey("url") &&
                            parms.ContainsKey("listname") &&
                            parms.ContainsKey("targeturl") &&
                            parms.ContainsKey("syncfields"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;


                    case "attach-workflow":
                        if (parms.ContainsKey("url") &&
                            parms.ContainsKey("listname") &&
                            parms.ContainsKey("workflowtemplate"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;



                    case "provision-list":
                        if (parms.ContainsKey("url") &&
                            parms.ContainsKey("listname") &&
                            parms.ContainsKey("type") &&
                            parms.ContainsKey("contenttype") &&
                            parms.ContainsKey("defaultcontenttype"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;


                    case "ensure-childsite":
                        if (parms.ContainsKey("parentsite") &&
                                          parms.ContainsKey("siteurl") &&
                                          parms.ContainsKey("sitetitle") &&
                                          parms.ContainsKey("sitetemplate"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;
                    case "show-brokenpermissions":
                        if (parms.ContainsKey("targetsite"))
                            bRetVal = true;
                        break;
                    
                    case "clear-list":
                        if (parms.ContainsKey("sourcesite") &&
                            parms.ContainsKey("sourcelist") &&
                            parms.ContainsKey("sourceuser") &&
                            parms.ContainsKey("sourcepassword"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;
                    case "update-url":
                        bRetVal = true;
                        break;
                    case "copy-list":
                        if (parms.ContainsKey("sourcesite") &&
                            parms.ContainsKey("sourcelist") &&
                            parms.ContainsKey("targetsite") &&
                            parms.ContainsKey("targetlist") &&
                            parms.ContainsKey("fieldsettings"))
                        {

                            bRetVal = true;
                        } else
                        {
                            bRetVal = false;
                        }
                        break;
                    case "list-webparts":
                        bRetVal = true;
                        break;

                    case "sync-sitecollectionfolder":
                        if (parms.ContainsKey("sourcesite") && parms.ContainsKey("sourcefolder"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;

                    case "remove-contenttypefromlist":
                        if (parms.ContainsKey("url") && parms.ContainsKey("listname") && parms.ContainsKey("contenttype"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;
                    case "disable-contenttypes":
                        if (parms.ContainsKey("url") && parms.ContainsKey("listname"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;

                        

                    case "ensure-listhascontenttype":
                        if (parms.ContainsKey("url") && parms.ContainsKey("listname") && parms.ContainsKey("contenttype"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;
                    case "ensure-list":
                        if (parms.ContainsKey("url") && parms.ContainsKey("listname") && parms.ContainsKey("type"))
                        {
                            bRetVal = true;
                        } else
                        {
                            bRetVal = false;
                        }
                        break;

                    case "find-unpublishedfiles":
                        bRetVal = true;
                        break;
                    case "find-uniqueid":
                        if (parms.ContainsKey("guid") || parms.ContainsKey("Guid"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;
                    case "update-homepagereferences":
                        bRetVal = true;
                        break;
                    case "ensure-sitegallery":
                        if (parms.ContainsKey("savecontext") && parms.ContainsKey("walkcontext"))
                         { 
                            bRetVal = true;
                        }
                        else
                        {

                            bRetVal = false;
                        }
                        break;
                    case "upload-pagelayout":
                        if (parms.ContainsKey("filelocation"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                        }
                        break;
                    case "update-sitephoto":
                        if (parms.ContainsKey("imageurl"))
                        {
                            bRetVal = true;
                        } else
                        {
                            bRetVal = false;
                        }

                            break;
                    case "ensure-splist":
                        if (parms.ContainsKey("title") && parms.ContainsKey("template"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                            Status += "Ensure-SPList requires the template and title parameters.";
                        }
                        break;
                    case "ensure-spsite":
                        if (parms.ContainsKey("siteurl") && parms.ContainsKey("title"))
                        {
                            bRetVal = true;
                        } 
                        else
                        {
                            bRetVal = false;
                            Status += "Ensure-SPSite requires the URL and Title Parameterss.";
                        }
                        break;
                    case "ensure-contenttype":
                    case "ensure-spcontenttype":
                        if (parms.ContainsKey("name") && parms.ContainsKey("parentname") && parms.ContainsKey("group"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                           bRetVal = false;
                            Status += "Ensure-ContentType requires the Name, Parent Name and Group  Parameters.";
                        }
                        break;
                    case "ensure-contenttypefield":
                        if (parms.ContainsKey("fieldname") && parms.ContainsKey("contenttype") && parms.ContainsKey("url"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                            Status += "Ensure-ContentTypeField  requires the Url, ContentType and FieldName parameters.";
                        }
                        break;

                    case "ensure-sitecolumn":
                    case "ensure-sitecolumnuser":
                    case "ensure-sitecolumnboolean":
                    case "ensure-sitecolumncurrency":
                    case "ensure-sitecolumndatetime":
                    case "ensure-sitecolumninteger":
                    case "ensure-sitecolumnnote":
                    case "ensure-sitecolumnnumber":
                        if (parms.ContainsKey("url") && parms.ContainsKey("title") && parms.ContainsKey("internalname"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                            Status += "Ensure-SiteColumn items require  Url,Title, and internalname  fields..";
                        }
                        break;

                    case "ensure-sitecolumnchoice":
                        if (parms.ContainsKey("url") && parms.ContainsKey("title") && parms.ContainsKey("internalname") && parms.ContainsKey("choices"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                            Status += "Ensure-SiteColumnChoice require Url,Title,internalname and Choices fields..";
                        }
                        break;

                    case "ensure-field":
                    case "ensure-fieldboolean":
                    case "ensure-fieldcurrency":
                    case "ensure-fielddatetime":
                    case "ensure-fieldinteger":
                    case "ensure-fieldnnote":
                    case "ensure-fieldnumber":
                        if (parms.ContainsKey("url") && parms.ContainsKey("title") && parms.ContainsKey("internalname") && parms.ContainsKey("listname"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                            Status += "Ensure-SiteColumn intems require  Url,Title, and internalname  fields..";
                        }
                        break;

                    case "ensure-fieldchoice":
                        if (parms.ContainsKey("url") && parms.ContainsKey("title") && parms.ContainsKey("internalname") && parms.ContainsKey("choices")  && parms.ContainsKey("listname"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                            Status += "Ensure-SiteColumnChoice require Url,Title,internalname and Choices fields..";
                        }


                        break;


                    case "ensure-sitecolumnlookup":
                        if (parms.ContainsKey("url") && parms.ContainsKey("internalname") && parms.ContainsKey("displayname") && parms.ContainsKey("listname") && parms.ContainsKey("showfield"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                            Status += "Ensure-SiteColumnLookup require Url,DisplayName,InternalName,ListName,ShowField";
                        }
                        break;

                    case "ensure-versioningenabled":
                        if (parms.ContainsKey("url"))
                        {
                            bRetVal = true;
                        }
                        else
                        {
                            bRetVal = false;
                            Status += "Ensure-VersioningEnabled requires Url";
                        }


                        break;





                    default:
                        break;

                }
                Verified = true;
                return bRetVal;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                Status = "Error in Verify: " + ex.Message;
                Verified = false;
            }
            return false;
        }

        private void LoadItem(string cRawLine)
        {
            try
            {
                if (string.IsNullOrEmpty(cRawLine)) return;
                    RawLine = cRawLine;
                Command = cRawLine.Substring(0, cRawLine.IndexOf("(")).ToLower();
                string cTempParms = cRawLine.Substring(cRawLine.IndexOf("(") + 1);
                cTempParms = cTempParms.Substring(0, cTempParms.Length - 1);
                string[] aParms = cTempParms.Split(',');
                parms = new Dictionary<string, string>();
                foreach (string aParm in aParms)
                {
                    try
                    {
                        if (aParm.Contains('='))
                        {
                            string[] cWorkSplit = aParm.Split('=');
                            parms.Add(cWorkSplit[0].ToLower(), cWorkSplit[1]);
                            if (cWorkSplit[0].ToLower() == "testmode" && (cWorkSplit[1].ToLower() == "true" || (cWorkSplit[1] == "1")))
                            {
                                TestMode = true;
                            }
                            else TestMode = false;
                        }
                    }
                    catch (Exception ex)
                    {

                        Status += string.Format("Error Loading Some or All Parameters: {0}{1}", ex.Message, Environment.NewLine);                        
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error Loading Item: {0} - {1}", cRawLine, ex.Message));
            }
        }

        public bool GetParmBool(string cParmKey)
        {
            foreach (KeyValuePair<string, string> parm in parms)
            {
                if (parm.Value.ToLower() == "true")
                    return true;
            }
            return false;
        }


    }
}
