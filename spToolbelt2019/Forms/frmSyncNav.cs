using Microsoft.SharePoint.Client;
using System;
using spToolbelt2019Lib;
using spToolbelt2019lib.Code.Extensions;
using System.Windows;

namespace spToolbelt2019.Forms
{
    public partial class frmSyncNav : System.Windows.Forms.Form
    {
        ClientContext ctx;

        

        public frmSyncNav(ClientContext inputContext)
        {
            ctx = inputContext;
            InitializeComponent();
        }

        private void frmSyncNav_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            SyncNav(txtSourceSite.Text, txtTargetSite.Text);
            button1.Enabled = true;
        }

        private void SyncNav(string cSourceSite,string cTargetSite)   
        {
            ClientContext ctxSource = new ClientContext(cSourceSite);
            ctxSource.Credentials = ctx.Credentials;
            ClientContext ctxTarget = new ClientContext(cTargetSite);
            ctxTarget.Credentials = ctx.Credentials;

            List lstTarget = ctxTarget.Web.Lists.GetByTitle("NavigationItems");
            ctxTarget.Load(lstTarget);
            ctxTarget.ExecuteQuery();
            List lstSource = ctxSource.Web.Lists.GetByTitle("NavigationItems");
            CamlQuery oQuery = new CamlQuery
            {
                ViewXml = "<View><Query><OrderBy><FieldRef Name='niLevel'/><FieldRef Name='niSortOrder'/></OrderBy></Query><RowLimit>5000</RowLimit></View>"
            };



            ListItemCollection items = lstSource.GetItems(oQuery);
            ctxSource.Load(items, li=>li.Include(
                i=>i.HasUniqueRoleAssignments,
                i => i["Title"],
                i => i["niAriaText"],
                i => i["niCaption"],
                i => i["niLinkUrl"],
                i => i["niSiteID"],
                i => i["niStyles"],
                i => i["niToolTip"],
                i => i["niPosition"],
                i => i["niLinkType"],
                i => i["niEnabled"],
                i => i["niIndent"],
                i => i["niNewTab"],
                i => i["niLevel"],
                i =>i["niParentItem"],
                i => i["niSortOrder"]));
            ctxSource.Load(lstSource);
            ctxSource.ExecuteQuery();


            foreach(ListItem itm in items)
            {
                try
                {
                    listBox1.Items.Add("Working: " + itm["Title"].ToString());
                    ListItem tgtItem = lstTarget.GetListItemByTitle(itm["Title"].ToString());
                    if (tgtItem == null)
                    {
                        ListItemCreationInformation lici = new ListItemCreationInformation();
                        
                        tgtItem = lstTarget.AddItem(lici);
                        
                    }
                    SetFieldValue(tgtItem, itm, "Title");
                    SetFieldValue(tgtItem, itm, "niAriaText");
                    SetFieldValue(tgtItem, itm, "niCaption");
                    SetFieldValue(tgtItem, itm, "niLinkUrl");
                    SetFieldValue(tgtItem, itm, "niSiteID");
                    SetFieldValue(tgtItem, itm, "niSortOrder");
                    SetFieldValue(tgtItem, itm, "niLevel");
                    SetFieldValue(tgtItem, itm, "niNewTab");
                    SetFieldValue(tgtItem, itm, "niIndent");
                    SetFieldValue(tgtItem, itm, "niEnabled");
                    SetFieldValue(tgtItem, itm, "niLinkType");
                    SetFieldValue(tgtItem, itm, "niPosition");
                    SetFieldValue(tgtItem, itm, "niStyles");

                    
                    if (itm["niParentItem"] != null)
                    {

                        FieldLookupValue flv = itm["niParentItem"] as FieldLookupValue;
                        ListItem lookedUpItem = lstTarget.GetListItemByTitle(flv.LookupValue);
                        if (lookedUpItem != null)
                        {
                            FieldLookupValue saveFlv = new FieldLookupValue();
                            saveFlv.LookupId = lookedUpItem.Id;
                            tgtItem["niParentItem"] = saveFlv;
                        }
                    }
                    tgtItem.Update();
                    ctxTarget.ExecuteQuery();
                    if (itm.HasUniqueRoleAssignments)
                    {
                       SyncPemrissions(ctxTarget, tgtItem, itm);
                    }
                } catch (Exception ex)
                {
                    listBox1.Items.Add("Error work item: " + itm["Title"].ToString() + " - " + ex.Message);
                }
            }
            listBox1.Items.Add("Complete!");

        }

        private void SetFieldValue(ListItem tgtItem, ListItem itm, string cInternalFieldName)
        {
            try
            {
                tgtItem[cInternalFieldName] = itm[cInternalFieldName];
                tgtItem.Update();
                tgtItem.Context.ExecuteQuery();
            } catch (Exception ex)
            {
                MessageBox.Show("Eror on field:" + cInternalFieldName + " - " + ex.Message);
            }
        }

        private  void SyncPemrissions(ClientContext ctxTarget, ListItem tgtItem, ListItem item)
        {
            try
            {
                ctxTarget.Load(tgtItem,a=>a.HasUniqueRoleAssignments);
                ctxTarget.ExecuteQuery();
                if (!tgtItem.HasUniqueRoleAssignments)
                {
                    tgtItem.BreakRoleInheritance(false, true);
                }
                item.Context.Load(item, i => i.RoleAssignments);
                item.Context.ExecuteQuery();
                foreach (RoleAssignment ra in item.RoleAssignments)
                {
                    try
                    {
                        item.Context.Load(ra.Member);
                        item.Context.ExecuteQuery();
                        if (!HasRole(ctxTarget,tgtItem, ra.Member))
                        {

                            var roleDefBindCol = new RoleDefinitionBindingCollection(ctxTarget);
                            roleDefBindCol.Add(tgtItem.ParentList.ParentWeb.RoleDefinitions.GetByType(RoleType.Reader));
                            if (ra.Member.PrincipalType.ToString() != "SharePointGroup")
                            {
                                var user_group = tgtItem.ParentList.ParentWeb.EnsureUser(ra.Member.LoginName);
                                tgtItem.RoleAssignments.Add(user_group, roleDefBindCol);
                            }
                            else
                            {
                                tgtItem.ParentList.ParentWeb.EnsureGroup(ra.Member.LoginName);

                                var newgroup = tgtItem.ParentList.ParentWeb.SiteGroups.GetByName(ra.Member.LoginName);
                                var srcgroup = item.ParentList.ParentWeb.SiteGroups.GetByName(ra.Member.LoginName);
                                item.Context.Load(srcgroup, sg => sg.Users);
                                item.Context.ExecuteQuery();
                                foreach (User usr in srcgroup.Users)
                                {
                                    try
                                    {
                                        newgroup.EnsureUser(tgtItem.ParentList.ParentWeb,usr.LoginName);
                                    } catch (Exception ex)
                                    {
                                        System.Diagnostics.Trace.WriteLine(ex.Message);
                                    }
                                }

                                var roleDefBindCol2 = new RoleDefinitionBindingCollection(ctxTarget);
                                roleDefBindCol2.Add(tgtItem.ParentList.ParentWeb.RoleDefinitions.GetByType(RoleType.Reader));

                                tgtItem.RoleAssignments.Add(newgroup, roleDefBindCol2);
                            }
                            
                            tgtItem.Update();
                            ctxTarget.ExecuteQuery();




                            
                        }
                    }
                    catch (Exception ex)
                    {
                        string cMessage = "Unable to Add User: " + ra.Member.LoginName + " - " + ex.Message;
                        listBox1.Items.Add(cMessage);
                    }

                }
            }
            catch (Exception ex)
            {
                string cMessage = "An error occured in SyncPemrissions for: " + item.Id + " " + ex.Message;
                listBox1.Items.Add(cMessage);
            }
        }

        private  bool HasRole(ClientContext permctx, ListItem tgtItem, Principal member)
        {
            try
            {
                permctx.Load(tgtItem.RoleAssignments);
                permctx.ExecuteQuery();
                foreach (RoleAssignment ra in tgtItem.RoleAssignments)
                {
                    permctx.Load(ra.Member);
                    permctx.ExecuteQuery();
                    if (ra.Member.LoginName == member.LoginName)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                string cMessage = "An error occured in HasRole for: " + member.LoginName+ " " + ex.Message;
                listBox1.Items.Add(cMessage);

            }
            return false;
        }

    }



}
