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
            oWorker.Complete += OWorker_Complete;
            oWorker.Start(textBox2.Text,"Update");
            
        }

        private void OWorker_Complete()
        {
            textBox1.Text += Environment.NewLine + "Complete";
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

        private void frmUpdateConfig_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            oWorker = new updateWorker();
            oWorker.Info += OWorker_Info;
            oWorker.Progress += OWorker_Progress;
            oWorker.Error += OWorker_Error;
            oWorker.Complete += OWorker_Complete;
            oWorker.Start(textBox2.Text, "Find");

        }
    }
}
