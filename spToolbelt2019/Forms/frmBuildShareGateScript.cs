using HandlebarsDotNet;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using spToolbelt2019lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace spToolbelt2019.Forms
{
    public partial class frmBuildShareGateScript : System.Windows.Forms.Form
    {
        ClientContext ctx;
        public frmBuildShareGateScript(ClientContext inCTX)
        {
            ctx = inCTX;
            InitializeComponent();
        }

        private void GetData()
        {

            try
            {
                string viewXML = "<View><Query><OrderBy><FieldRef Name='spmiSiteUrl' Ascending='TRUE'/></OrderBy><Where><And><IsNotNull><FieldRef Name='spmiTargetAction' /></IsNotNull><Neq><FieldRef Name = 'spmiTargetAction' /><Value Type = 'Text'>none</Value></Neq></And></Where></Query><RowLimit>5000</RowLimit></View>";

                List lstSites = ctx.Web.Lists.GetByTitle("spmiSites");
                CamlQuery oQuery = new CamlQuery();
                oQuery.ViewXml = viewXML;
                ListItemCollection items = lstSites.GetItems(oQuery);
                ctx.Load(items, itms => itms.Include(i => i.FieldValuesAsText, i => i["Title"], i => i["spmiSiteUrl"], i => i["spmiTargetAction"], i => i["spmiTargetLocation"], i => i["spmiSiteID"]));
                ctx.ExecuteQuery();
                List<SiteInfo> siteData = new List<SiteInfo>();
                foreach (ListItem itm in items)
                {
                    SiteInfo si = new SiteInfo();
                    si.SiteUrl = itm["spmiSiteUrl"].ToString();
                    si.Title = itm["Title"].ToString();
                    si.SiteID = itm["spmiSiteID"].ToString();
                    if (itm["spmiTargetAction"] != null)
                    {
                        si.TargetAction = itm["spmiTargetAction"].ToString();
                    }
                    if (itm["spmiTargetLocation"] != null)
                    {
                        si.TargetLocation = itm["spmiTargetLocation"].ToString();
                    }
                    siteData.Add(si);
                }
                //string template = "{{#each this}}{{Title}}{{/each}}";
                //Func<object, string> compiledTemplate = Handlebars.Compile(template);
                //string templateOutput = compiledTemplate(siteData);
                string viewListXML = "<View><Query><OrderBy><FieldRef Name='spmiSiteUrl' Ascending='TRUE'/></OrderBy><Where><And><IsNotNull><FieldRef Name='spmiTargetAction' /></IsNotNull><Neq><FieldRef Name = 'spmiTargetAction' /><Value Type = 'Text'>none</Value></Neq></And></Where></Query><RowLimit>5000</RowLimit></View>";


                List lstLists = ctx.Web.Lists.GetByTitle("spmiLists");
                CamlQuery oListQuery = new CamlQuery();
                oListQuery.ViewXml = viewListXML;
                ListItemCollection workItems = lstLists.GetItems(oListQuery);
                ctx.Load(workItems, itms => itms.Include(i => i.FieldValuesAsText, i => i["spmiListType"], i => i["spmiRootFolderUrl"], i => i["Title"], i => i["TargetList"], i => i.Id, i => i["spmiSiteUrl"], i => i["spmiTargetAction"], i => i["spmiTargetLocation"], i => i["spmiSiteID"]));
                ctx.ExecuteQuery();
                List<ListInfo> listData = new List<ListInfo>();
                foreach (ListItem itm in workItems)
                {
                    ListInfo li = new ListInfo();
                    li.Id = itm.Id;
                    li.TargetList = GetFieldValue(itm, "TargetList");
                    li.ListType = GetFieldValue(itm, "spmiListType");
                    li.RootFolderUrl = GetFieldValue(itm, "spmiRootFolderUrl");
                    li.SiteUrl = GetFieldValue(itm, "spmiSiteUrl");
                    li.Title = itm["Title"].ToString();
                    li.SiteID = GetFieldValue(itm, "spmiSiteID");
                    li.TargetAction = GetFieldValue(itm, "spmiTargetAction");
                    li.TargetLocation = GetFieldValue(itm, "spmiTargetLocation");

                    listData.Add(li);
                }

                foreach (SiteInfo si in siteData)
                {
                    ListViewItem lvitem = lvSites.Items.Add(si.SiteUrl);
                    lvitem.Tag = si;
                    lvitem.SubItems.Add(si.Title);
                    lvitem.SubItems.Add(si.TargetAction);
                    lvitem.SubItems.Add(si.TargetLocation);

                }

                foreach (ListInfo li in listData)
                {
                    ListViewItem lvitem = lvLists.Items.Add(li.Title);
                    lvitem.Tag = li;
                    lvitem.SubItems.Add(li.SiteUrl);
                    lvitem.SubItems.Add(li.TargetAction);
                    lvitem.SubItems.Add(li.TargetLocation);

                }
            } catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }


        }

        private string GetFieldValue(ListItem itm, string cFieldName)
        {
            try
            {
                if (itm[cFieldName]!=null)
                {
                    return itm[cFieldName].ToString();
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " - " + cFieldName);
            }
            return "";
        }

        private void frmBuildShareGateScript_Load(object sender, EventArgs e)
        {
            GetData();
        }

       
        private void button1_Click(object sender, EventArgs e)
        {
            GetData();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtListTemplate.Text))
            {
                MessageBox.Show("Please Select a File");
                return;
            }

            if (!Directory.Exists(txtOutputFolder.Text))
            {
                Directory.CreateDirectory(txtOutputFolder.Text);
            }

            string cTemplate = System.IO.File.ReadAllText(txtListTemplate.Text);
            List<ListInfo> items = GetSelectLists();


            foreach (ListInfo lst in items)
            {
                using (ClientContext workCTX = new ClientContext(lst.TargetLocation))
                {
                    try
                    {
                        lst.Title = lst.Title.Replace("/", "-");
                        workCTX.Credentials = ctx.Credentials;
                        List workList = workCTX.Web.Lists.GetByTitle(lst.TargetList);
                        workCTX.Load(workList);
                        workCTX.ExecuteQuery();
                        workList.RootFolder.Folders.Add(lst.Title);
                        workList.Update();
                        workCTX.ExecuteQuery();
                    } catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                    }
                }
            }


            Func<object, string> compiledTemplate = Handlebars.Compile(cTemplate);
            string templateOutput = compiledTemplate(items);
            string cOutputPath = txtOutputFolder.Text + @"\" + txtListOutput.Text;
            System.IO.File.WriteAllText(cOutputPath, templateOutput);

            MessageBox.Show("Complete!");




        }

        private List<ListInfo> GetSelectLists()
        {
            List<ListInfo> items = new List<ListInfo>();
            foreach(ListViewItem lvi in lvLists.Items)
            {
                if (lvi.Checked)
                {
                    items.Add((ListInfo)lvi.Tag);
                }
            }
            return items;
        }

        private List<SiteInfo> GetSelectSites()
        {
            List<SiteInfo> items = new List<SiteInfo>();
            foreach (ListViewItem lvi in lvSites.Items)
            {
                if (lvi.Checked)
                {
                    items.Add((SiteInfo)lvi.Tag);
                }
            }
            return items;
        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            string cFilename = GetFilename("Inventory Migration Template (.migrationtemplate)|*.migrationtemplate|All Files (*.*)|*.*");
            txtListTemplate.Text = cFilename;
        }

        private string GetFilename(string cFilter)
        {
            OpenFileDialog oDlg = new OpenFileDialog() { Filter = cFilter, FilterIndex = 1 };
            DialogResult retVal = oDlg.ShowDialog();
            if (retVal == DialogResult.OK)
            {
                if (System.IO.File.Exists(oDlg.FileName))
                {
                    return oDlg.FileName;
                }
            }
            return "";
        }

        private void cmdSelectSiteTemplate_Click(object sender, EventArgs e)
        {
            string cFilename = GetFilename("Inventory Migration Template (.migrationtemplate)|*.migrationtemplate|All Files (*.*)|*.*");
            txtSiteTemplate.Text = cFilename;
        }

        private void chkSelectAllLists_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkLists = (CheckBox)sender;
            if (chkLists.Checked)
            {
                foreach(ListViewItem lvi in lvLists.Items)
                {
                    lvi.Selected = true;
                    lvi.Checked = true;
                }
            } else
            {
                foreach (ListViewItem lvi in lvLists.Items)
                {
                    lvi.Selected = false;
                    lvi.Checked = false;
                }

            }
        }

        private void chkSelectAllSites_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkLists = (CheckBox)sender;
            if (chkLists.Checked)
            {
                foreach (ListViewItem lvi in lvSites.Items)
                {
                    lvi.Selected = true;
                    lvi.Checked = true;
                }
            }
            else
            {
                foreach (ListViewItem lvi in lvSites.Items)
                {
                    lvi.Selected = false;
                    lvi.Checked = false;
                }

            }

        }

        private void cmdProcessSites_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSiteTemplate.Text))
            {
                MessageBox.Show("Please Select a File");
                return;
            }

            if (!Directory.Exists(txtOutputFolder.Text))
            {
                Directory.CreateDirectory(txtOutputFolder.Text);
            }

            string cTemplate = System.IO.File.ReadAllText(txtSiteTemplate.Text);
            List<SiteInfo> items = GetSelectSites();
            Func<object, string> compiledTemplate = Handlebars.Compile(cTemplate);
            string templateOutput = compiledTemplate(items);
            string cOutputPath = txtOutputFolder.Text + @"\" + txtSiteOutput.Text;
            System.IO.File.WriteAllText(cOutputPath, templateOutput);

            MessageBox.Show("Complete!");





        }

        private void button2_Click_1(object sender, EventArgs e)
        {

            ClearCurrentList();
        }



        private void ClearCurrentList()
        {

            try
            {
                string viewXML = "<View><Query><OrderBy><FieldRef Name='spmiSiteUrl' Ascending='TRUE'/></OrderBy><Where><IsNotNull><FieldRef Name='spmiTargetAction' /></IsNotNull></Where></Query><RowLimit>5000</RowLimit></View>";

                List lstSites = ctx.Web.Lists.GetByTitle("spmiLists");
                CamlQuery oQuery = new CamlQuery();
                oQuery.ViewXml = viewXML;
                ListItemCollection items = lstSites.GetItems(oQuery);
                ctx.Load(items);
                ctx.ExecuteQuery();
                foreach (ListItem itm in items)
                {
                    try
                    {
                        itm["spmiTargetAction"] = null;
                        itm["spmiTargetLocation"] = null;
                        itm["TargetList"] = null;
                        itm.Update();
                        ctx.ExecuteQuery();
                    } 
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                    }
                }
                MessageBox.Show("Items Cleared!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }


        }


    }
}
