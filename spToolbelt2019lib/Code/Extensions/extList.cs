﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Workflow;


namespace spToolbelt2019Lib
{
    public static class ExtList
    {
        public static void EnsureDefaultViewFields(this List lst,  string cViewFields)
        {
            EnsureViewFields(lst, lst.DefaultView, cViewFields);
        }

        public static void EnsureViewFields(this List lst,View workView,string cViewFields)
        {
            try
            {
                string[] vwFields = cViewFields.Split(';');
                lst.Context.Load(workView, v => v.ViewFields);
                lst.Context.ExecuteQuery();
                workView.ViewFields.RemoveAll();
                foreach (string vwField in vwFields)
                {
                    try
                    {
                        if (!workView.ViewFields.Contains(vwField))
                        {
                            workView.ViewFields.Add(vwField);
                            workView.Update();
                            lst.Context.ExecuteQuery();
                        }
                    } catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                    }
                }
                //workView.ViewFields.Add("Editor");
                //workView.ViewFields.Add("Modified");
                workView.Update();
                lst.Context.ExecuteQuery();
            } catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            
        }



        public static void SetListExperience(this List lst, ListExperience exp)
        {
            try
            {
                
                lst.ListExperienceOptions = exp;
                lst.Update();
                lst.Context.ExecuteQuery();

            } catch (Exception ex)
            {
                throw new Exception("An error occurred in SetListExperience for: " + lst.Title + " " + ex.Message);
            }
        }



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


        public static ListItemCollection GetAllItems(this List workList)
        {
            ListItemCollection items;
            try
           
            {
                CamlQuery oQuery = CamlQuery.CreateAllItemsQuery();
                items = workList.GetItems(oQuery);
                workList.Context.Load(items);
                workList.Context.ExecuteQuery();
                
            }
            catch (Exception ex)
            {
                throw new Exception("Error with list: " + workList.Title + " - " + ex.Message);
            }
            return items;
        }


        public static ListItemCollection GetModifiedItems(this List workList, DateTime lastRun)
        {
            ListItemCollection items;
            try
            {
                string datetime = lastRun.ToString("yyyy-MM-ddTHH:mm:ssZ");//change the Time to your date time value
                CamlQuery oQuery = new CamlQuery
                {
                    ViewXml = "<View><Query><Where><Gt><FieldRef Name='Modified'/><Value Type='DateTime'>" + datetime + "</Value></Gt></Where></Query><RowLimit>5000</RowLimit></View>"
                };

                items = workList.GetItems(oQuery);
                workList.Context.Load(items);
                workList.Context.ExecuteQuery();

                
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in GetModifiedItems for: " + workList.Title + " " + ex.Message);
            }
            return items;
        }


        public static void SyncList(this List srcList, ClientContext ctxTarget, string cListName, string cSyncFields, DateTime dtLastRun)
        {

            try
            {
                List tgtList = ctxTarget.Web.Lists.GetByTitle(cListName);
                ctxTarget.Load(tgtList, l => l.ItemCount);
                ctxTarget.ExecuteQuery();
                srcList.Context.Load(srcList, s => s.ItemCount,s=>s.HasUniqueRoleAssignments);
                srcList.Context.ExecuteQuery();

                ListItemCollection workItems = null;

                if (srcList.ItemCount != tgtList.ItemCount)
                {
                    workItems = srcList.GetAllItems();
                } else
                {
                    workItems = srcList.GetModifiedItems(dtLastRun);
                }
                foreach (ListItem item in workItems)
                {
                    try
                    {
                        srcList.Context.Load(item);
                        srcList.Context.ExecuteQueryRetry();

                        ListItem tgtItem = tgtList.GetListItemByTitle(item["Title"].ToString());
                        if (tgtItem == null)
                        {
                            ListItemCreationInformation lici = new ListItemCreationInformation();
                            tgtItem = tgtList.AddItem(lici);
                            tgtItem["Title"] = item["Title"];
                            tgtItem.Update();
                        }

                        string[] aFields = cSyncFields.Split(';');
                        foreach(string cField in aFields)
                        {
                            try
                            {
                                if (cField.Contains("`"))
                                {
                                    string[] cFieldInfo = cField.Split('`');
                                 
                                    
                                    if (item[cFieldInfo[1]]!=null)
                                    {
                                        string cText = item[cFieldInfo[1]].ToString();
                                        if (cText.Contains("/Departments/Marketing/MarketingStore"))
                                        {
                                            if (cFieldInfo[0].ToLower().Contains("image"))
                                            {
                                                cText = cText.Replace("/Departments/Marketing/MarketingStore", "");
                                                string cUrl = cText.Substring(cText.IndexOf("<img"));
                                                 cUrl = cUrl.Substring(cUrl.IndexOf("src") + 5);
                                                Int32 iEnd = cUrl.IndexOf("\"");
                                                cUrl = cUrl.Substring(0, iEnd);
                                                cUrl = cUrl.Replace("/Catalog%20Images/", "https://sandbox.rammware.net/sites/Sebia/BrochureImages/");
                                                tgtItem[cFieldInfo[0]] = cUrl;
                                            }
                                            else
                                            {
                                                cText = cText.Replace("/Departments/Marketing/MarketingStore", "");
                                                string cUrl = cText.Substring(cText.IndexOf("href") + 6);
                                                Int32 iEnd = cUrl.IndexOf("\"");
                                                cUrl = cUrl.Substring(0, iEnd);
                                                tgtItem[cFieldInfo[0]] = cUrl;

                                            }
                                        } else
                                        {
                                            tgtItem[cFieldInfo[0]] = item[cFieldInfo[1]];
                                        }
                                    }
                                }
                                else
                                {
                                    tgtItem[cField.Trim()] = item[cField.Trim()];
                                }
                            } catch  (Exception ex)
                            {
                                System.Diagnostics.Trace.WriteLine(ex.Message);
                            }
                            tgtItem.Update();
                            ctxTarget.ExecuteQuery();

                        }
                        tgtItem.Update();
                        srcList.Context.Load(item, i => i.HasUniqueRoleAssignments, i => i.RoleAssignments);
                        srcList.Context.ExecuteQuery();
                        if (item.HasUniqueRoleAssignments)
                        {
                            tgtItem.BreakRoleInheritance(false, true);
                            tgtItem.Update();
                            ctxTarget.ExecuteQuery();
                            SyncPemrissions(ctxTarget, tgtItem, item);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("An error occured in SyncList for (Inside): " + srcList.Title + " " + ex.Message);
                    }
                }
            } catch(Exception ex)
            {
                throw new Exception("An error occured in SyncList for: " + srcList.Title + " " + ex.Message);
            }
            
        }

        private static void SyncPemrissions(ClientContext ctxTarget, ListItem tgtItem, ListItem item)
        {
            try
            {
                item.Context.Load(item, i => i.RoleAssignments);
                item.Context.ExecuteQuery();
                foreach(RoleAssignment ra in item.RoleAssignments)
                {
                    try
                    {
                        item.Context.Load(ra.Member);
                        item.Context.ExecuteQuery();
                        if (!HasRole(tgtItem, ra.Member))
                        {
                            tgtItem.RoleAssignments.Add(ra.Member, ra.RoleDefinitionBindings);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to Add User: " + ra.Member.LoginName+" - "+ex.Message);
                    }

                }
            } catch(Exception ex)
            {
                throw new Exception("An error occured in SyncPemrissions for: " + item.Id + " " + ex.Message);
            }
        }

        private static bool HasRole(ListItem tgtItem, Principal member)
        {
            try
            {
                tgtItem.Context.Load(tgtItem.RoleAssignments);
                tgtItem.Context.ExecuteQuery();
                foreach(RoleAssignment ra in tgtItem.RoleAssignments)
                {
                    tgtItem.Context.Load(ra.Member);
                    tgtItem.Context.ExecuteQuery();
                    if (ra.Member.LoginName==member.LoginName)
                    {
                        return true;
                    }
                }
            } catch (Exception)
            {
                throw new Exception("Error with Roles");

            }
            return false;
        }

        public static bool HasItemByTitle(this List lst,string cTitle)
        {
            string viewXML = "<View><Query><Where><Eq><FieldRef Name = 'Title' /><Value Type = 'Text'>"+cTitle+"</Value></Eq></Where></Query><RowLimit>5000</RowLimit></View>";
            CamlQuery oQuery = new CamlQuery
            {
                ViewXml = viewXML
            };

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

        public static bool HasItemByField(this List lst, string cValue,string cField)
        {
            string viewXML = "<View><Query><Where><Eq><FieldRef Name = '"+cField+"' /><Value Type = 'Text'>" + cValue + "</Value></Eq></Where></Query><RowLimit>5000</RowLimit></View>";
            CamlQuery oQuery = new CamlQuery
            {
                ViewXml = viewXML
            };

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


        public static ListItem GetItemByField(this List lst, string cValue, string cField)
        {
            string viewXML = "<View><Query><Where><Eq><FieldRef Name = '" + cField + "' /><Value Type = 'Text'>" + cValue + "</Value></Eq></Where></Query><RowLimit>5000</RowLimit></View>";
            CamlQuery oQuery = new CamlQuery
            {
                ViewXml = viewXML
            };

            try
            {
                ListItemCollection items = lst.GetItems(oQuery);
                lst.Context.Load(items);
                lst.Context.ExecuteQuery();
                if (items.Count > 0)
                {
                    return items[0];
                }
                    
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            return null;


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


        public static ListItem GetListItemByField(this List lst, ClientContext wctx, string cFieldName,string cFieldType,string cFieldValue)
        {
            try
            {
                wctx.Load(lst);
                wctx.ExecuteQuery();
                string viewXML = "<View><Query><Where><Eq><FieldRef Name='"+cFieldName+"' /><Value Type='"+cFieldType+"'>" + cFieldValue + "</Value></Eq></Where></Query></View>";
                CamlQuery oQuery = CamlQuery.CreateAllItemsQuery();
                oQuery.ViewXml = viewXML;
                ListItemCollection oItems = lst.GetItems(oQuery);
                wctx.Load(oItems);
                wctx.ExecuteQuery();
                foreach (ListItem listItem in oItems)
                {
                    if (listItem[cFieldName].ToString() == cFieldValue)
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

        public static ListItem GetListItemByTitle(this List lst, string cItemTitle)
        {
            try
            {
                lst.Context.Load(lst);
                lst.Context.ExecuteQuery();
                string viewXML = "<View><Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + cItemTitle + "</Value></Eq></Where></Query></View>";
                CamlQuery oQuery = CamlQuery.CreateAllItemsQuery();
                oQuery.ViewXml = viewXML;
                ListItemCollection oItems = lst.GetItems(oQuery);
                lst.Context.Load(oItems);
                lst.Context.ExecuteQuery();
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
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Trace.WriteLine(ex.InnerException.Message);
                }

            }
            return null;
        }

        public static ListItem GetItemByFileName(this List lst,  string cFileName)
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

                WorkflowAssociationCreationInformation wfc = new WorkflowAssociationCreationInformation
                {
                    HistoryList = web.Lists.GetByTitle("Workflow History"),
                    Name = cTemplateName,
                    TaskList = web.Lists.GetByTitle("Workflow Tasks"),
                    Template = wt
                };
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

        public static View GetDefaultView(this List lst)
        {
            ViewCollection views = lst.Views;
            lst.Context.Load(views,v=>v.Include(vw=>vw.DefaultView,vw=>vw.ViewFields));
            lst.Context.ExecuteQuery();
            foreach(View vw in views)
            {
                if (vw.DefaultView)
                {
                    return vw; 
                }
            }
            return null;
        }

        public static View GetView(this List lst, string cViewName)
        {
            try
            {
                View tgtView = lst.Views.GetByTitle(cViewName);
                lst.Context.Load(tgtView,vw=>vw.ViewFields);
                lst.Context.ExecuteQuery();
                return tgtView;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return lst.GetDefaultView();
            }
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
        
        public static void EnsureFile(this List lstName,ClientContext workContext,string cFileName,string cLocalPath)
        {
            ListItem fileItem = lstName.GetListItemByFileName(workContext, cFileName);
            if (fileItem==null)
            {

                Folder folder = lstName.RootFolder;
                FileCreationInformation fci = new FileCreationInformation
                {
                    Content = System.IO.File.ReadAllBytes(cLocalPath),
                    Url = cFileName,
                    Overwrite = true
                };
                File fileToUpload = folder.Files.Add(fci);
                workContext.Load(fileToUpload);
                workContext.ExecuteQuery();

            }
        }
        
        
    }
}
