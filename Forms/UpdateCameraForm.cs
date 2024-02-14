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
    public partial class UpdateCameraForm : Form
    {
        public StoreCollection Store { get; set; }
        public string cameraUrl = string.Empty;
        public UpdateCameraForm()
        {
            InitializeComponent();
            ShowCameraList();
        }
        public void ShowCameraList()
        {
            var store = new StoreCollection();
            Store = store;
            string url = Store.FirstOrDefault(x => x.accNum == SessionManager.StoreAccNum)?.CameraUrl;

            lvCamera.BeginUpdate();
            var selected = lvCamera.SelectedItems.OfType<ListViewItem>().FirstOrDefault()?.SubItems[0].Text;
            ListViewItem selectedItem = null;
            lvCamera.Clear();

            if (url != "")
            {
                string[] cameraUrlIds = url.Split(',');
                cameraUrl = url;
                if (cameraUrlIds.Length > 0)
                {
                    foreach (var Url in cameraUrlIds)
                    {
                        var plate = Url;
                        var item = lvCamera.Items.Add(plate);
                    }

                    lvCamera.EndUpdate();
                    selectedItem?.EnsureVisible();
                }
            }
            UpdateTotalCount();

        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbUrl.Text))
            {
                Store.UpdateCameraUrl(SessionManager.StoreAccNum, tbUrl.Text);
                cameraUrl = tbUrl.Text;
                this.Hide();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string urlIds = "";
            var selected = lvCamera.SelectedItems[0];
            var value = (string)selected.Text;
            if (MessageBox.Show($"Are you sure you want to delete subject '{value}'?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {

                lvCamera.Items.RemoveAt(selected.Index);
                foreach (var item in lvCamera.Items)
                {
                    urlIds += ((System.Windows.Forms.ListViewItem)item).Text + ",";
                }
                if (urlIds != "")
                {
                    urlIds = urlIds.Remove(urlIds.Length - 1);
                    cameraUrl = urlIds;

                    Store.UpdateCameraUrl(SessionManager.StoreAccNum, urlIds);
                }
                UpdateTotalCount();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string urlIds = "";
            if (!string.IsNullOrEmpty(tbUrl.Text))
            {
                foreach (var item in lvCamera.Items)
                {
                    urlIds += ((System.Windows.Forms.ListViewItem)item).Text + ",";
                }
                urlIds += tbUrl.Text;
                Store.UpdateCameraUrl(SessionManager.StoreAccNum, urlIds);
                tbUrl.Text = "";
                cameraUrl = urlIds;
                ShowCameraList();
            }
            btnAdd.Text = "Add";
            UpdateTotalCount();
        }

        private void lvCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDelete.Enabled = lvCamera.SelectedItems.Count > 0;
            btnEdit.Enabled = lvCamera.SelectedItems.Count > 0;

        }

        private void tbUrl_TextChanged(object sender, EventArgs e)
        {
            if (tbUrl.Text != "")
                btnAdd.Enabled = !string.IsNullOrWhiteSpace(tbUrl.Text);

        }
        private void UpdateTotalCount()
        {
            lblTotal.Text = "Subject Count: " + lvCamera.Items.Count.ToString();
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            string urlIds = "";
            var selected = lvCamera.SelectedItems[0];
            var value = (string)selected.Text;

            tbUrl.Text = value;
            btnAdd.Text = "Update";
            lvCamera.Items.RemoveAt(selected.Index);
            foreach (var item in lvCamera.Items)
            {
                urlIds += ((System.Windows.Forms.ListViewItem)item).Text + ",";
            }
            if (urlIds != "")
            {
                urlIds = urlIds.Remove(urlIds.Length - 1);
                cameraUrl = urlIds;

                Store.UpdateCameraUrl(SessionManager.StoreAccNum, urlIds);
            }


        }
    }
}
