using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.SharePoint.Client;

namespace spToolbelt2019Lib
{
    public static class extWeb
    {

        public static void RemoveList(this Web workWeb, string cListName)
        {
            try
            {
                if (workWeb.HasList(cListName))
                {
                    List workList = workWeb.Lists.GetByTitle(cListName);
                    workWeb.Context.Load(workList);
                    workWeb.Context.ExecuteQuery();
                    workList.DeleteObject();
                    workWeb.Context.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Removing List from Site", ex);
            }
        }

        public static void RemoveContentType(this Web workWeb, string cContentType)
        {
            try
            {
                if(workWeb.HasContentType(cContentType))
                {
                    ContentType ct = workWeb.GetContentType(cContentType);
                    ct.DeleteObject();
                    workWeb.Context.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Removing Content Type ", ex);
            }
        }

        #region Field Operations


        public static void RemoveFieldsForGroup(this Web workWeb,string cGroupName)
        {
            try
            {
                List<Field> workFields = new List<Field>();
                FieldCollection flds = workWeb.Fields;
                workWeb.Context.Load(flds, fs => fs.Include(f => f.Group,f=>f.Title,f=>f.InternalName));
                workWeb.Context.ExecuteQuery();
                foreach(Field fld in flds)
                {
                    if (fld.Group.ToLower().Trim()==cGroupName.ToLower().Trim())
                    {
                        workFields.Add(fld);
                    }
                }
                string cFieldInternaName = "";
                foreach (var field in workFields)
                {
                    try
                    {
                        cFieldInternaName = field.InternalName;
                        field.DeleteObject();
                        workWeb.Context.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        string contentTypes = workWeb.GetContentTypesForColumn(cFieldInternaName);
                        System.Diagnostics.Trace.WriteLine(ex.Message + " " + field.Title);  
                    }

                }
        
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Removing Site Columns by Group", ex);                
            }
        }

        private static string GetContentTypesForColumn(this Web workWeb,string cInternalName)
        {
            string cIncluded = "";
            try
            {
                ContentTypeCollection cts = workWeb.ContentTypes;
                workWeb.Context.Load(cts);
                workWeb.Context.ExecuteQuery();
                foreach(ContentType ct in cts)
                {
                    FieldCollection ctFields = ct.Fields;
                    workWeb.Context.Load(ctFields, c => c.Include(cf => cf.InternalName));
                    workWeb.Context.ExecuteQuery();
                    foreach (Field field in ctFields)
                    {
                        if (field.InternalName==cInternalName)
                        {
                            cIncluded+=ct.Name+"; ";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            return cIncluded;
        }

        #endregion


        #region Guid

        public static bool HasGuid(this Web web, string cGuid)
        {
            try
            {
                web.Context.Load(web, l => l.Id);
                web.Context.ExecuteQuery();
                if (web.Id.ToString().ToLower() == cGuid.ToLower())
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
        public static bool ContainsGuidPart(this Web web, string cGuid)
        {
            try
            {
                web.Context.Load(web, l => l.Id);
                web.Context.ExecuteQuery();
                if (web.Id.ToString().ToLower().Contains(cGuid.ToLower()))
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

        #endregion

        #region Webs 
        
        public static List<Web> GetSubWebs(this Web web)
        {
            List<Web> oWebs = new List<Web>();
            try
            {
                oWebs.Add(web);
                WebCollection webs = web.Webs;
                web.Context.Load(webs);
                web.Context.ExecuteQuery();
                foreach (Web subweb in webs)
                {
                    List<Web> subwebs = GetSubWebs(subweb);
                    foreach (Web childweb in subwebs)
                    {
                        oWebs.Add(childweb);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in extWeb.GetSubWebs -  " + ex.Message, ex);
                
            }
            return oWebs;
        }
        #endregion

        #region Lists 





        public static void EnsureList(this Web web,string cListName,ListTemplateType templateType,string cDescription )
        {
            if (!web.HasList(cListName))
            {
                web.AddList(cListName, templateType, cDescription);
            }
        }

        public  static void AddList(this Web web,string cListName,ListTemplateType templateType,string cDescription)
        {
            try
            {
                string cInternalName = cListName.Replace(" ", "");
                ListCreationInformation lci = new ListCreationInformation() { TemplateType = (int)templateType, Description = cDescription, Title = cInternalName };
                List newList = web.Lists.Add(lci);
                web.Context.ExecuteQuery();
                web.Context.Load(newList);
                web.Context.ExecuteQuery();
                newList.Title = cListName;
                newList.Update();
                web.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.Addlist -  {0} - {1}", cListName, ex.Message), ex);

            }
        }

        public static bool HasList(this Web web, string cListName)
        {
            return (web.GetListObject(cListName)!=null);
        }

        public static List GetListObject(this Web web,string cListName)
        {
            try
            {
                ListCollection lists = web.Lists;
                web.Context.Load(lists);
                web.Context.ExecuteQuery();
                foreach (List list in lists)
                {
                    if (list.Title.ToLower()==cListName.ToLower())
                    {
                        list.Hidden = false;
                        list.Update();
                        web.Context.ExecuteQuery();
                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.GetListObject -  {0} - {1}", cListName, ex.Message), ex);

                
            }
            return null;
        }

        #endregion

        #region Site Columns


        #region Lookup


        public static void EnsureSiteColumnLookup(this Web web,string cInternalName, string cDisplayName, string cFieldDescription, string cGroup,string cListName, string cShowField)
        {
            try
            {
                if (!web.HasSiteColumn(cDisplayName))
                {
                    web.AddSiteColumnLookup(cInternalName,cDisplayName, cFieldDescription, cGroup,cListName,cShowField);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.EnsureSiteColumn -  {0} - {1}", cDisplayName, ex.Message), ex);
            }
        }

        public static void AddSiteColumnLookup(this Web web,string cInternalName,string cDisplayName, string cFieldDescription, string cGroup,string cListName,string cShowField)
        {
            try
            {

                List lstSource = web.Lists.GetByTitle(cListName);
                web.Context.Load(lstSource);
                web.Context.ExecuteQuery();
                System.Diagnostics.Trace.WriteLine(lstSource.Id.ToString());
                string strNewField = string.Format("<Field Type='Lookup'  DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' List='{4}' ShowField='{5}' />", cDisplayName, cInternalName, cGroup, cFieldDescription, lstSource.Id.ToString(), cShowField);
                web.Fields.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                web.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.AddSiteColumn -  {0} - {1}", cDisplayName, ex.Message), ex);
            }
        }
        #endregion

        #region SiteColumnUser


        public static void AddSiteColumnUser(this Web web, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup, bool bMultiUser, bool bAllowGroups)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName, cFieldTitle);
                string strNewField = string.Format("<Field Type='User'  UserSelectionMode='PeopleOnly' DisplayName ='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' />", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                if (bMultiUser && bAllowGroups)
                {
                    strNewField = string.Format("<Field Type='UserMulti' Mult='TRUE'  DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' />", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                }
                else if (bMultiUser)
                {
                    strNewField = string.Format("<Field Type='UserMulti' Mult='TRUE'  DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' />", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                }
                else if (bAllowGroups)
                {
                    strNewField = string.Format("<Field Type='User' UserSelectionMode='PeopleAndGroups'  DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' />", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                }
                web.Fields.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                web.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.AddSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }
        public static void EnsureSiteColumnUser(this Web web, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup, bool bMultiUser, bool bAllowGroups)
        {
            try
            {
                if (!web.HasSiteColumn(cFieldTitle))
                {
                    web.AddSiteColumnUser(cInternalName, cFieldTitle, cFieldDescription, cGroup, bMultiUser, bAllowGroups);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.EnsureSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        #endregion



        #region Text




        public static void EnsureSiteColumn(this Web web, string cInternalName, string cFieldTitle,string cFieldDescription, string cGroup)
        {
            try
            {
                if (!web.HasSiteColumn(cFieldTitle))
                {
                    web.AddSiteColumn(cInternalName,cFieldTitle, cFieldDescription, cGroup);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.EnsureSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddSiteColumn(this Web web, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName,cFieldTitle);

                string strNewField = string.Format("<Field Type='Text'  DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' />", cFieldTitle, cInternalName, cGroup,cFieldDescription);
                web.Fields.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                web.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.AddSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }
        #endregion

        #region Note


        public static void EnsureSiteColumnNote(this Web web, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                if (!web.HasSiteColumn(cFieldTitle))
                {
                    web.AddSiteColumnNote(cInternalName,cFieldTitle, cFieldDescription, cGroup);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.EnsureSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddSiteColumnNote(this Web web, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName,cFieldTitle);

                string strNewField = string.Format("<Field Type='Note' DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' />", cFieldTitle, cInternalName, cGroup,cFieldDescription);
                web.Fields.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                web.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.AddSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }
        #endregion
        
        #region DateTime


        public static void EnsureSiteColumnDateTime(this Web web, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                if (!web.HasSiteColumn(cFieldTitle))
                {
                    web.AddSiteColumnDateTime(cInternalName,cFieldTitle, cFieldDescription, cGroup);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.EnsureSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddSiteColumnDateTime(this Web web, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName,cFieldTitle);

                string strNewField = string.Format("<Field Type='DateTime' DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' />", cFieldTitle, cInternalName, cGroup,cFieldDescription);
                web.Fields.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                web.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.AddSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }
        #endregion

        #region Boolean


        public static void EnsureSiteColumnBoolean(this Web web, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                if (!web.HasSiteColumn(cFieldTitle))
                {
                    web.AddSiteColumnBoolean(cInternalName,cFieldTitle, cFieldDescription, cGroup);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.EnsureSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddSiteColumnBoolean(this Web web, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName,cFieldTitle);
                string strNewField = string.Format("<Field Type='Boolean' DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' ><Default>0</Default></Field>", cFieldTitle, cInternalName, cGroup,cFieldDescription);
                web.Fields.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                web.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.AddSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }
        #endregion

        #region Choice


        public static void EnsureSiteColumnChoice(this Web web, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup,string cChoices)
        {
            try
            {
                if (!web.HasSiteColumn(cFieldTitle))
                {
                    web.AddSiteColumnChoice(cInternalName, cFieldTitle, cFieldDescription, cGroup,cChoices);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.EnsureSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddSiteColumnChoice(this Web web, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup,string cChoices)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName,cFieldTitle);

                
                string strNewField = string.Format("<Field Type='Choice' DisplayName='{0}'  StaticName='{1}' Name='{1}'  Description='{2}' Group='{3}' ><CHOICES>", cFieldTitle, cInternalName,cFieldDescription,cGroup);

                string[] aChoices = cChoices.Split(';');
                foreach (string item in aChoices)
                {
                    strNewField += string.Format("<CHOICE>{0}</CHOICE>", item);
                }
                strNewField += "</CHOICES></Field>";
                web.Fields.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                web.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.AddSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }



        public static bool HasSiteColumn(this Web web,string cFieldTitle)
        {
            return (GetSiteColumn(web, cFieldTitle) != null);
        }
        public static Field GetSiteColumn(this Web web,string cFieldTitle)
        {
            try
            {
                
                FieldCollection fields = web.Fields;
                web.Context.Load(fields);
                web.Context.ExecuteQuery();
                foreach (Field fld in fields)
                {
                    if (fld.Title.ToLower()==cFieldTitle.ToLower())
                    {
                        return fld;
                    }
                    if (fld.InternalName.ToLower() == cFieldTitle.ToLower())
                    {
                        return fld;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.GetSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);

            }
            return null;
        }
        #endregion

        #region Boolean


        public static void EnsureSiteColumnNumber(this Web web,string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                if (!web.HasSiteColumn(cFieldTitle))
                {
                    web.AddSiteColumnNumber(cInternalName,cFieldTitle, cFieldDescription, cGroup);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.EnsureSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddSiteColumnNumber(this Web web, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName,cFieldTitle);
                string strNewField = string.Format("<Field Type='Number' DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' ><Default>0</Default></Field>", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                web.Fields.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                web.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.AddSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }
        #endregion

        #region Boolean


        public static void EnsureSiteColumnInteger(this Web web, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                if (!web.HasSiteColumn(cFieldTitle))
                {
                    web.AddSiteColumnInteger(cInternalName,cFieldTitle, cFieldDescription, cGroup);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.EnsureSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddSiteColumnInteger(this Web web, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName,cFieldTitle);
                string strNewField = string.Format("<Field Type='Number' DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' Decimals='0'><Default>0</Default></Field>", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                web.Fields.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                web.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.AddSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }
        #endregion

        #region Currency


        public static void EnsureSiteColumnCurrency(this Web web, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                if (!web.HasSiteColumn(cFieldTitle))
                {
                    web.AddSiteColumnCurrency(cInternalName,cFieldTitle, cFieldDescription, cGroup);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.EnsureSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddSiteColumnCurrency(this Web web, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName,cFieldTitle);
                string strNewField = string.Format("<Field Type='Currency' DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' ><Default>0</Default></Field>", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                web.Fields.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                web.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.AddSiteColumn -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        private static string GetInternalName(string cInternalName, string cFieldTitle)
        {

            string cResult = "";
            if (!string.IsNullOrEmpty(cInternalName))
            {
                cResult = cInternalName;
            } else
            {
                cResult = cFieldTitle;
            }
            try
            {
                cResult = cResult.Replace(" ", "");
                cResult = cResult.Replace("#", "");
                cResult = cResult.Replace("/", "");
                cResult = cResult.Replace("-", "");
                cResult = cResult.Replace("(", "");
                cResult = cResult.Replace(")", "");
                cResult = cResult.Replace("[", "");
                cResult = cResult.Replace("]", "");

            }
            catch (Exception ex)
            {
                throw new Exception("Error in extWeb.GetInternalName -  " + ex.Message, ex);
            }
            return cResult;
        }
        #endregion


        #endregion

        #region Context Types

        public static void EnsureContentTypeHasField(this Web web,  string cContentTypeName,string cFieldName)
        {
            if (!web.ContentTypeHasField(cContentTypeName,cFieldName))
            {
                web.AddFieldToContentType(cContentTypeName, cFieldName);
            }
        }

        public static void AddFieldToContentType(this Web web,string cContentTypeName,string cFieldName)
        {

            Field fld = web.GetSiteColumn(cFieldName);
            if (fld==null)
            {
                throw new Exception("Error in extWeb.AddFieldToContentType - Unable to get field "+cFieldName);
                
            }
            ContentType ct = web.GetContentType(cContentTypeName);
            if (ct==null)
            {
                throw new Exception("Error in extWeb.AddFieldToContentType - Unable to get content Type "+cContentTypeName);

            }
            try
            {
                ct.FieldLinks.Add(new FieldLinkCreationInformation { Field = fld });
                ct.Update(true);
                web.Context.ExecuteQuery();

            }
            catch (Exception ex)
            {
                throw new Exception("Error in extWeb.AddFieldToContentType -  " + ex.Message, ex);

            }




        }

        public static bool ContentTypeHasField(this Web web,string cContentTypeName,string cFieldName)
        {
            try
            {
                ContentType ct = web.GetContentType(cContentTypeName);
                FieldCollection flds = ct.Fields;
                web.Context.Load(flds);
                web.Context.ExecuteQuery();
                foreach (Field fld in flds)
                {
                    if (fld.Title.ToLower()==cFieldName.ToLower() || fld.InternalName.ToLower()==cFieldName.ToLower())
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.ContentTypeHasField -  {0} - {1}", cContentTypeName, cFieldName),ex);

            }
            return false;
        }



        public static bool HasContentType(this Web web,string cContentTypeName)
        {
            return (web.GetContentType(cContentTypeName)!=null);
        }
        public static ContentType GetContentType(this Web web,string cContentTypeName)
        {
            try
            {
                web.Context.ExecutingWebRequest += delegate (object sender2, WebRequestEventArgs e2)
                {
                    e2.WebRequestExecutor.WebRequest.UserAgent = "NONISV|RammWare|spToolbelt2019/1.0";
                };
                ContentTypeCollection cts = web.ContentTypes;
                web.Context.Load(cts);
                web.Context.ExecuteQuery();
                foreach (ContentType ct in cts)
                {
                    if (ct.Name.ToLower()==cContentTypeName.ToLower())
                    {
                        return ct;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extWeb.GetContentType -  {0} - {1}", cContentTypeName, ex.Message), ex);

            }
            return null;
        }


        public static void EnsureContentType(this Web web, string cContentTypeName,string cParentContentType,string cGroup)
        {
            if (!web.HasContentType(cContentTypeName)) web.AddContentType(cContentTypeName, cParentContentType,cGroup);
        }

        public static void AddContentType(this Web web,string cContentTypeName, string cParentContentType, string cGroup)
        {
            ContentType ct = web.GetContentType(cParentContentType);
            if (ct != null)
            {
                try
                {
                    ContentTypeCreationInformation ctci = new ContentTypeCreationInformation() { ParentContentType = ct, Name = cContentTypeName, Group = cGroup };

                    web.ContentTypes.Add(ctci);
                    web.Context.ExecuteQuery();

                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Error in extWeb.AddContentType -  {0} - {1}", cContentTypeName, ex.Message), ex);

                }
            }
            else
            {
                throw new Exception("Error in extWeb.AddContentType - Parent ContentType Not Found " + cParentContentType);

            }
           
        }






















        #endregion

        #region Folder

        public static Folder GetFolder(this FolderCollection fc,string cFoldername)
        {
            try
            {
                fc.Context.Load(fc);
                fc.Context.ExecuteQuery();
                foreach (Folder fld in fc)
                {
                    if (fld.Name.ToLower() == cFoldername.ToLower())
                        return fld;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in extWeb.GetFolder -  " + ex.Message, ex);

            }
            return null;
        }



        public static Folder EnsureFolder(this Web web,string cSourceFolder)
        {
            Folder fld = null;
            try
            {
                //string cWorkFolder="";
                
                FolderCollection fc = web.Folders;
                web.Context.Load(fc);
                web.Context.ExecuteQuery();

                string[] cFolders = cSourceFolder.Split('/');
                foreach (string cFolder in cFolders)
                {
                    fld = fc.GetFolder(cFolder);
                    if (fld!=null)
                    {
                        fc = fld.Folders;
                    } else
                    {
                        fc.Add(cFolder);
                        fc.Context.ExecuteQuery();
                        fld = fc.GetFolder(cFolder);
                        fc = fld.Folders;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in extWeb.EnsureFolder -  " + ex.Message, ex);
            }
            return fld;
        }
        
        #endregion
    }
}
