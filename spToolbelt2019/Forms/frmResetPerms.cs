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
    public partial class frmResetPerms : System.Windows.Forms.Form
    {
        //ClientContext ctx;
        public frmResetPerms(/*ClientContext inContext*/)
        {
            //ctx = inContext;
            InitializeComponent();
        }


        private void frmResetPerms_Load(object sender, EventArgs e)
        {
            //ListCollection lists = ctx.Web.Lists;
            //ctx.Load(lists, lsts => lsts.Include(l=>l.Title,l => l.Hidden, l => l.HasUniqueRoleAssignments));
            //ctx.ExecuteQuery();
            //foreach (List list in lists)
            //{
            //    if (!list.Hidden && list.HasUniqueRoleAssignments)
            //    {
            //        listBox1.Items.Add(list.Title);
            //    }
            //}

        }

        private void SplitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
