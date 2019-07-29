using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace spToolbelt2019.Forms
{
    public partial class frmBuildShareGateScript : System.Windows.Forms.Form
    {
        //ClientContext ctx;
        public frmBuildShareGateScript(/*ClientContext inCTX*/)
        {
            //ctx = inCTX;
            InitializeComponent();
        }

        private void frmBuildShareGateScript_Load(object sender, EventArgs e)
        {
            //comboBox1.Items.Clear();
            //ListCollection lists = ctx.Web.Lists;
            //ctx.Load(lists, lsts => lsts.Include(l => l.Hidden,l=>l.Title,l=>l.Fields));
            //ctx.ExecuteQuery();
            //foreach (List list in lists)
            //{
            //    if (!list.Hidden)
            //    {
            //        foreach (Field fld in list.Fields)
            //        {
            //            if (fld.Title.ToLower().Contains("targeturl"))
            //            {
            //                comboBox1.Items.Add(list.Title);
            //            }
            //        }
            //    }
            //}




        }

        private void BuildScript(string cOutFile)
        {
            //if (!cOutFile.ToLower().EndsWith(".ps1"))
            //{
            //    cOutFile += ".ps1";
            //}
            //StreamWriter oOutfile = new StreamWriter(cOutFile);
            //oOutfile.WriteLine("Import-Module ShareGate");
            //oOutfile.WriteLine("$copysettings = New-CopySettings -OnContentItemExists IncrementalUpdate");
            //oOutfile.WriteLine("$Pwd = ConvertTo-SecureString \"vqpjfxbcqdfmrnqb\" -AsPlainText -Force");
          
            //List lst = ctx.Web.Lists.GetByTitle(comboBox1.Text);
            //CamlQuery oQuery = CamlQuery.CreateAllItemsQuery();
            //ListItemCollection items = lst.GetItems(oQuery);
            //ctx.Load(items, itms => itms.Include(i => i.Id, i => i["Title"],  i => i["SourceURL"], i => i["TargetURL"]));
            //ctx.ExecuteQuery();

            //foreach (ListItem listItem in items)
            //{
            //    oOutfile.WriteLine("#--------------------------------------------------------------------");
            //    string cLoc = @"C:\Temp\Log-" + listItem.Id + "-" + listItem["TargetURL"].ToString().Replace("/", "-") + listItem["Title"].ToString().Replace("/", "-").Replace(" ", "");
            //    oOutfile.WriteLine("$CopyResultFile" + listItem.Id + @" = """ + cLoc + "\"");
            //    string cSourceUrl = txtSourceBase.Text + listItem["SourceURL"].ToString();
            //    string cDestUrl = txtTargetBase.Text + listItem["TargetURL"].ToString();
            //    oOutfile.WriteLine("$srcSite" + listItem.Id + " = Connect-Site -Url " + cSourceUrl + " -username \"john.ramminger@cfacorp.onmicrosoft.com\" -password $Pwd");
            //    oOutfile.WriteLine("$tgtSite" + listItem.Id + " = Connect-Site -Url " + cDestUrl + " -username \"john.ramminger@cfacorp.onmicrosoft.com\"  -password $Pwd");
            //    oOutfile.WriteLine("$copyResult" + listItem.Id + " = Copy-List -Name \"" + listItem["Title"].ToString() + "\" -SourceSite $srcSite" + listItem.Id + " -DestinationSite $tgtSite" + listItem.Id + " -InsaneMode -CopySettings $copysettings ");
            //    oOutfile.WriteLine("Export-Report $copyResult" + listItem.Id + " -Path $CopyResultFile" + listItem.Id + " -Overwrite");
            //}
            //oOutfile.Flush();
            //oOutfile.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //saveFileDialog1.InitialDirectory = @"C:\";
            //saveFileDialog1.Title = "Save PowerShell Script";
            //saveFileDialog1.CheckPathExists = true;
            //saveFileDialog1.DefaultExt = "ps1";
            //saveFileDialog1.Filter = "PowerShell Script (*.ps1)|*.ps1";
            ////saveFileDialog1.FilterIndex = 0;
            ////saveFileDialog1.RestoreDirectory = true;



            //if (saveFileDialog1.ShowDialog() == DialogResult.OK)

            //{
            //    BuildScript(saveFileDialog1.FileName);
            //}
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
