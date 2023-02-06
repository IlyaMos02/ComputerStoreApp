using NetTechLab3.Client;
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

namespace NetTechLab3.View
{
    public partial class AutorizationView : Form
    {               
        public AutorizationView()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            string name = txtName.Text;
            string password = txtPassword.Text;

            string send = "aut:" + name + ":" + password;

            string receive = SocketClient.Send(send);

            if(receive.Contains("True"))
            {
                MessageBox.Show("Autorization succes");
                btnContinue.Enabled = true;
            }
            else
            {
                MessageBox.Show("Autorization denied");
                btnContinue.Enabled = false;
            }
        }                     
        private void AutorizationView_Load(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }       

        private void btnContinue_Click(object sender, EventArgs e)
        {            
            MainView mainView = new MainView();
            mainView.Show();            
            this.Visible = false;
        }

        private void AutorizationView_FormClosed(object sender, FormClosedEventArgs e)
        {            
            Environment.Exit(1);
        }
    }
}
