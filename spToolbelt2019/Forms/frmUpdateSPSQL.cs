using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace spToolbelt2019.Forms
{
    public partial class frmUpdateSPSQL : System.Windows.Forms.Form
    {
        ClientContext workCTX;  
        public frmUpdateSPSQL(ClientContext ctx)
        {
            InitializeComponent();
            workCTX = ctx;
        }

        private void frmUpdateSPSQL_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Lists = "";
            ListCollection l = workCTX.Web.Lists;
            workCTX.Load(l);
            workCTX.ExecuteQuery();
            foreach(List lst in l)
            {
                Lists += lst.Title + Environment.NewLine;
            }
            MessageBox.Show(Lists);
        }
    }
}
