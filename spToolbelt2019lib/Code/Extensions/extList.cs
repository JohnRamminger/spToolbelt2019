using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Workflow;


namespace spToolbelt2019Lib
{
    public static class extList
    {
        #region Guid

        public static bool HasGuid(this List lst,string cGuid)
        {
            try
            {
                lst.Context.Load(lst, l => l.Id);
                lst.Context.ExecuteQuery();
                if (lst.Id.ToString().ToLower() == cGuid.ToLower())
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
        public static bool ContainsGuidPart(this List lst, string cGuid)
        {
            try
            {
                lst.Context.Load(lst, l => l.Id);
                lst.Context.ExecuteQuery();
                if (lst.Id.ToString().ToLower().Contains(cGuid.ToLower()))
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


        public static bool HasItemByTitle(this List lst,string cTitle)
        {
            string viewXML = "<View><Query><Where><Eq><FieldRef Name = 'Title' /><Value Type = 'Text'>"+cTitle+"</Value></Eq></Where></Query><RowLimit>5000</RowLimit></View>";
            CamlQuery oQuery = new CamlQuery();
            oQuery.ViewXml = viewXML;

            try
            {
                ListItemCollection items = lst.GetItems(oQuery);
                lst.Context.Load(items);
                lst.Context.ExecuteQuery();
                if (items.Count > 0)
                    return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);  
            }
            return false;


        }

        public static bool HasField(this List lst,string cFieldName)
        {
            try
            {
                FieldCollection flds = lst.Fields;
                lst.Context.Load(flds, fs => fs.Include(f => f.Title, f => f.InternalName));
                lst.Context.ExecuteQuery();
                foreach (Field field in flds)
                {
                    if (field.InternalName == cFieldName || field.Title == cFieldName)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            return false;
        }

        public static ListItem  GetListItemByTitle(this List lst,ClientContext wctx,string cItemTitle)
        {
            try
            {
                wctx.Load(lst);
                wctx.ExecuteQuery();
                string viewXML = "<View><Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>"+cItemTitle+"</Value></Eq></Where></Query></View>";
                CamlQuery oQuery = CamlQuery.CreateAllItemsQuery();
                oQuery.ViewXml = viewXML;
                ListItemCollection oItems = lst.GetItems(oQuery);
                wctx.Load(oItems);
                wctx.ExecuteQuery();
                foreach (ListItem listItem in oItems)
                {
                    if (listItem["Title"].ToString() == cItemTitle)
                    {
                        return listItem;
                    }
                }
                //if (oItems.Count>0)
                //{
                //    return oItems[0];
                //}
            }
            catch (Exception ex)
            {
                if (ex.InnerException!=null)
                {
                    System.Diagnostics.Trace.WriteLine(ex.InnerException.Message);
                }
                
            }
            return null;
        }



        public static ListItem GetItemByFileNam(this List lst,  string cFileName)
        {
            try
            {
                lst.Context.Load(lst);
                lst.Context.ExecuteQuery();
                string viewXML = "<View><Query><Where><Eq><FieldRef Name = 'FileLeafRef' /><Value Type = 'File'>" + cFileName + "</Value></Eq></Where></Query><RowLimit>5000</RowLimit></View>";
                CamlQuery oQuery = new CamlQuery
                {
                    //CamlQuery oQuery = CamlQuery.CreateAllItemsQuery();
                    ViewXml = viewXML
                };// CamlQuery.CreateAllItemsQuery();
                ListItemCollection oItems = lst.GetItems(oQuery);
                lst.Context.Load(oItems,itms=>itms.Include(i=>i["FileLeafRef"]));
                lst.Context.ExecuteQuery();
                foreach (ListItem listItem in oItems)
                {
                    if (listItem["FileLeafRef"].ToString() == cFileName)
                    {
                        return listItem;
                    }
                }
                //if (oItems.Count>0)
                //{
                //    return oItems[0];
                //}
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Trace.WriteLine(ex.InnerException.Message);
                }

            }
            return null;
        }

        public static ListItem GetListItemByFileName(this List lst, ClientContext wctx, string cItemTitle)
        {
            try
            {
                wctx.Load(lst);
                wctx.ExecuteQuery();
                string viewXML = "<View><Query><Where><Eq><FieldRef Name = 'FileLeafRef' /><Value Type = 'File'>" + cItemTitle + "</Value></Eq></Where></Query><RowLimit>5000</RowLimit></View>";
                CamlQuery oQuery = CamlQuery.CreateAllItemsQuery();
                oQuery.ViewXml = viewXML;
                ListItemCollection oItems = lst.GetItems(oQuery);
                wctx.Load(oItems);
                wctx.ExecuteQuery();
                foreach (ListItem listItem in oItems)
                {
                    if (listItem["FileLeafRef"].ToString() == cItemTitle)
                    {
                        return listItem;
                    }
                }
                //if (oItems.Count>0)
                //{
                //    return oItems[0];
                //}
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Trace.WriteLine(ex.InnerException.Message);
                }

            }
            return null;
        }


        public static WorkflowTemplate GetWorkflowTemplate(Web oWeb,string cTemplateName)
        {
            try
            {
                WorkflowTemplateCollection wts = oWeb.WorkflowTemplates;
                oWeb.Context.Load(wts);
                oWeb.Context.ExecuteQuery();
                foreach (WorkflowTemplate wt in wts)
                {
                    System.Diagnostics.Trace.WriteLine(wt.Name);
                    if (wt.Name.ToLower() == cTemplateName.ToLower())
                        return wt;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            return null;
        }


        public static void attachWorkflow(this List lst,ClientContext ctx,string cTemplateName)
        {
            
            try
            {
                Web web = lst.ParentWeb;

                ctx.Load(web, w => w.Url);
                ctx.ExecuteQuery();
                System.Diagnostics.Trace.WriteLine(web.Url);
              
                WorkflowTemplate wt = GetWorkflowTemplate(web,cTemplateName);
                
                WorkflowAssociationCreationInformation wfc = new WorkflowAssociationCreationInformation();
                wfc.HistoryList = web.Lists.GetByTitle("Workflow History");
                wfc.Name = cTemplateName;
                wfc.TaskList = web.Lists.GetByTitle("Workflow Tasks");
                wfc.Template = wt;
                WorkflowAssociation wf = lst.WorkflowAssociations.Add(wfc);
                wf.AllowManual = false; // is never updated
                wf.AutoStartChange = true; // is never
                wf.AutoStartCreate = true; // is never updated
                wf.Enabled = true; // is never updated
                                   //string assocData = GetAssociationXml(); // internal method
                                   // wf.AssociationData = assocData; // is never updated
                wf.Update();
                ctx.Load(wf);
                ctx.ExecuteQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }


        #region 

        public static void RemoveContentTypeFromList(this List lst, string cContentTypeName)
        {
            try
            {
                if (lst.HasContentType(cContentTypeName))
                {
                    ContentType ct = lst.GetContentType(cContentTypeName);
                    lst.Context.Load(ct);
                    lst.Context.ExecuteQuery();
                    ct.DeleteObject();
                    lst.Update();
                    lst.Context.ExecuteQuery();


                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in extList.RemoveContentTypeFromList -  " + ex.Message, ex);

            }
        }
        public static void EnsureListHasContenttype(this List lst, Site oSite, string cContentTypeName)
        {
            if (!lst.HasContentType(cContentTypeName))
            {
                lst.AddContentType(oSite,cContentTypeName);       
            }
        }


        public static void DisableContentTypes(this List lst)
        {
            try
            {
                lst.ContentTypesEnabled = false;
                lst.Update();
                lst.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in extList.DisableContentTypes -  " + ex.Message, ex);
            }
        }

        public static void AddContentType(this List lst, Site oSite, string cContentTypeName)
        {
            try
            {
                lst.ContentTypesEnabled = true;
                lst.Update();
                lst.Context.ExecuteQuery();
                
                ContentType ct = oSite.RootWeb.GetContentType(cContentTypeName);
                lst.ContentTypes.AddExistingContentType(ct);
                lst.Update();
                lst.Context.ExecuteQuery();
                
            }
            catch (Exception ex)
            {
                throw new Exception("Error in extList.AddContentType -  " + ex.Message, ex);
            }
        }
        public static bool HasContentType(this List lst,string cContentTypeName)
        {
            return (lst.GetContentType(cContentTypeName) != null);
        }

        public static View GetView(this List lst, string cViewName)
        {
            try
            {
                View tgtView = lst.Views.GetByTitle(cViewName);
                lst.Context.Load(tgtView);
                lst.Context.ExecuteQuery();
                return tgtView;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            return null;
        }



        public static bool HasView(this List lst,string cViewName)
        {
            try
            {
                if (lst.GetView(cViewName) != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            return false;
        }
        public static ContentType GetContentType(this List lst,string cContentTypeName)
        {
            try
            {
                ContentTypeCollection cts = lst.ContentTypes;
                lst.Context.Load(cts);
                lst.Context.ExecuteQuery();
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
                throw new Exception("Error in extList.GetContentType -  " + ex.Message, ex);

            }
            return null;
        }
        #endregion
    }
}
