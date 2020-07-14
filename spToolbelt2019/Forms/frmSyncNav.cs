using Microsoft.SharePoint.Client;
using System;
using spToolbelt2019Lib;

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
            CamlQuery oQuery = CamlQuery.CreateAllItemsQuery();
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
                listBox1.Items.Add("Working: " + itm["Title"].ToString());
                ListItem tgtItem = lstTarget.GetListItemByTitle(itm["Title"].ToString());
                if (tgtItem==null)
                {
                    ListItemCreationInformation lici = new ListItemCreationInformation();
                    tgtItem = lstTarget.AddItem(lici);
                    tgtItem["Title"] = itm["Title"];
                }

                tgtItem["niAriaText"] = itm["niAriaText"];
                tgtItem["niCaption"] = itm["niCaption"];
                tgtItem["niLinkUrl"] = itm["niLinkUrl"];
                tgtItem["niSiteID"] = itm["niSiteID"];
                tgtItem["niStyles"] = itm["niStyles"];
                tgtItem["niToolTip"] = itm["niToolTip"];
                tgtItem["niPosition"] = itm["niPosition"];
                tgtItem["niLinkType"] = itm["niLinkType"];
                tgtItem["niEnabled"] = itm["niEnabled"];
                tgtItem["niIndent"] = itm["niIndent"];
                tgtItem["niNewTab"] = itm["niNewTab"];
                tgtItem["niLevel"] = itm["niLevel"];
                tgtItem["niSortOrder"] = itm["niSortOrder"];

                if (itm["niParentItem"] != null)
                {

                    FieldLookupValue flv = itm["niParentItem"] as FieldLookupValue; 
                    ListItem lookedUpItem = lstTarget.GetListItemByTitle(flv.LookupValue);
                    FieldLookupValue saveFlv = new FieldLookupValue();
                    saveFlv.LookupId = lookedUpItem.Id;
                    tgtItem["niParentItem"] = saveFlv;
                }
                tgtItem.Update();
                ctxTarget.ExecuteQuery();
                if (itm.HasUniqueRoleAssignments)
                {
                    SyncPemrissions(ctxTarget, tgtItem, itm);
                }
            }
            listBox1.Items.Add("Complete!");

        }

        private  void SyncPemrissions(ClientContext ctxTarget, ListItem tgtItem, ListItem item)
        {
            try
            {
                item.Context.Load(item, i => i.RoleAssignments);
                item.Context.ExecuteQuery();
                foreach (RoleAssignment ra in item.RoleAssignments)
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

        private  bool HasRole(ListItem tgtItem, Principal member)
        {
            try
            {
                tgtItem.Context.Load(tgtItem.RoleAssignments);
                tgtItem.Context.ExecuteQuery();
                foreach (RoleAssignment ra in tgtItem.RoleAssignments)
                {
                    tgtItem.Context.Load(ra.Member);
                    tgtItem.Context.ExecuteQuery();
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
