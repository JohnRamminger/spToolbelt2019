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
    public partial class frmUpdateConfig : Form
    {
        updateWorker oWorker;
        public frmUpdateConfig()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            oWorker = new updateWorker();
            oWorker.Info += OWorker_Info;
            oWorker.Progress += OWorker_Progress;
            oWorker.Error += OWorker_Error;
            oWorker.Start(textBox2.Text);
        }

        private void OWorker_Error(string cExceptionMessage, string cLocation, string cMessage)
        {
            //textBox1.Text +=cExceptionMessage+" - "+cLocation+" - "+cMessage+ Environment.NewLine;
        }

        private void OWorker_Progress(string cProgress)
        {
            textBox1.Text += cProgress + Environment.NewLine;
        }

        private void OWorker_Info(string cInfo)
        {
            lblStatusInfo.Text = cInfo;
        }
    }
}
