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
    public partial class EnrollUser : Form
    {
        public EnrollUserCollection User { get; set; }
        public EnrollUser()
        {
            InitializeComponent();
            var user = new EnrollUserCollection();
            User = user;
        }
        private void btnEnrollUser_Click(object sender, EventArgs e)
        {
            var value = (tbUsername.Text);
            if (string.IsNullOrWhiteSpace(value))
            {
                System.Media.SystemSounds.Beep.Play();
            }
            if (User.Contains(value))
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }

            User.Add(value, tbPassword.Text, tbCell.Text, tbAddress.Text);
            MessageBox.Show("Enroll successfully");
            tbUsername.Text = "";
            tbPassword.Text = "";
            tbCell.Text = "";
            tbAddress.Text = "";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm login = new LoginForm();
            login.Show();
        }

        private void tbUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbCell_TextChanged(object sender, EventArgs e)
        {
            if(tbUsername.Text != null && tbPassword.Text != null && tbCell != null)
            {
                btnEnrollUser.Enabled = true;
            }
        }
    }
}
