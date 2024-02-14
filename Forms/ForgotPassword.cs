using Neurotec.Samples.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Neurotec.Samples.Forms
{
    public partial class ForgotPassword : Form
    {
        public LoginCollection Login { get; set; }
        public ForgotPassword()
        {
            InitializeComponent();
            var login = new LoginCollection();
            Login = login;
        }

        private void btnGetPassword_Click(object sender, EventArgs e)
        {
            var user = Login.FirstOrDefault(x => x.cell == tbCell.Text);
            if (user != null)
            {
                var password =user.password;
                SendVerificationCode(user.cell,password);
                MessageBox.Show("Password sent to the cell");
                this.Hide();
                LoginForm login = new LoginForm();
                login.Show();
            }
            else
            {
                MessageBox.Show("Invalid cell");
            }
        }
        private void SendVerificationCode(string cell,string pwd)
        {
            var accountSid = ConfigurationManager.AppSettings["AccountSID"]; // fetching from the app config
            var authToken = ConfigurationManager.AppSettings["AuthToken"]; // fetching from the app config
            TwilioClient.Init(accountSid, authToken);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                var message = MessageResource.Create(
  body: "Your password is  : " + pwd + "",
            from: new Twilio.Types.PhoneNumber(ConfigurationManager.AppSettings["From"]),
  to: new Twilio.Types.PhoneNumber(cell)
);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }
        private void ForgotPassword_Load(object sender, EventArgs e)
        {

        }

        private void tbCell_TextChanged(object sender, EventArgs e)
        {
            if(tbCell.Text != null)
            {
                btnGetPassword.Enabled = true;
            }
        }

      
    }
}
