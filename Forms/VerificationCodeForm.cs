using Neurotec.Samples.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neurotec.Samples.Forms
{
    public partial class VerificationCodeForm : Form
    {
        public VerificationCodeForm()
        {
            InitializeComponent();
        }

        private void verifyOtp_Click(object sender, EventArgs e)
        {
            // Access session data
            string otp = SessionManager.Otp;
            string role = SessionManager.Role;
            //otp = "12345";
            if (otp == tbOtp.Text)
            {
                if (role != null)
                {
                    this.Hide();
                    if (role.ToLower() == "siteadmin")
                    {
                        AdminForm admin = new AdminForm();
                        admin.Show();
                    }
                    else if (role.ToLower() == "companyadminuser")
                    {
                        StoreForm store = new StoreForm();
                        store.Show();
                    }
                    else if(role.ToLower() == "storeuser")
                    {
                        MainForm main = new MainForm();
                        main.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid Otp", "Xveillance 1.0");
            }
        }
    }
}
