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
    public partial class LoginForm : Form
    {
        public LoginCollection Login { get; set; }
        public LoginForm()
        {
            InitializeComponent();

            var login = new LoginCollection();
            Login = login;


        }
        #region Public properties


        public string TargetValue { get; set; }

        #endregion
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (username.Text != "" && password.Text != "")
            {
                if (Login != null)
                {
                    var user = Login.FirstOrDefault(x => x.username == txtUsername.Text && x.password == txtPassword.Text);
                    if (user != null)
                    {
                        this.Hide();
                        SessionManager.Role = user.role;
                        SessionManager.StoreName = user.storeName;
                        SessionManager.CompanyAccNum = user.companyAcctNum;
                        SessionManager.CompanyName = user.companyName;
                        SessionManager.StoreAccNum = user.storeAcctNum;
                        var texting = ConfigurationManager.AppSettings["Texting"]; // fetching from the app config
                        if (texting == "Yes")
                        {
                            SendVerificationCode(user.cell);
                            VerificationCodeForm verification = new VerificationCodeForm();
                            verification.Show();
                        }
                        else
                        {
                            if (user.role.ToLower() == "siteadmin")
                            {
                                AdminForm admin = new AdminForm();
                                admin.Show();
                            }
                            else if (user.role.ToLower() == "companyadminuser")
                            {
                                StoreForm store = new StoreForm();
                                store.Show();
                            }
                            else if (user.role.ToLower() == "storeuser")
                            {
                                MainForm main = new MainForm();
                                main.Show();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password", "Xveillance 1.0");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid username or password", "Xveillance 1.0");
                }
            }
        }
        private void SendVerificationCode(string cell)
        {
            Random generator = new Random();
            String otp = generator.Next(0, 1000000).ToString("D6");

            // Set session data
            SessionManager.Otp = otp;

            var accountSid = ConfigurationManager.AppSettings["AccountSID"]; // fetching from the app config
            var authToken = ConfigurationManager.AppSettings["AuthToken"]; // fetching from the app config
            TwilioClient.Init(accountSid, authToken);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                var message = MessageResource.Create(
  body: "verification code : " + otp + "",
            from: new Twilio.Types.PhoneNumber(ConfigurationManager.AppSettings["from"]),
  to: new Twilio.Types.PhoneNumber(cell)
);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Xveillance 1.0.2");
            }



        }

        private void btnEnrollAdmin_Click(object sender, EventArgs e)
        {
            this.Hide();
            EnrollUser enroll = new EnrollUser();
            enroll.Show();
        }

        private void btnForgotPwd_Click(object sender, EventArgs e)
        {
            this.Hide();
            ForgotPassword forgot = new ForgotPassword();
            forgot.Show();
        }
    }
}
