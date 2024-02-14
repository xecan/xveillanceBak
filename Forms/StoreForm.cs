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
    public partial class StoreForm : Form
    {
        #region Constants

        private const int PageSize = 50;

        #endregion

        #region Private fields

        private string _searchTerm = string.Empty;
        private string _searchAcctNum = string.Empty;
        private bool IsStoreEdit = false;

        #endregion
        public StoreForm()
        {
            InitializeComponent();
            string role = SessionManager.Role;
            if (role != null)
            {
                if (role.ToLower() == "companyadminuser")
                {
                    this.Text += "              Company Name : " + SessionManager.CompanyName;
                }
            }
            var store = new StoreCollection();
            Store = store;
            var company = new CompanyCollection();
            Company = company;
            //GetCellPhone();
            ShowList();
            UpdateTotalCount();
        }
        #region Public properties

        public StoreCollection Store { get; set; }
        public CompanyCollection Company { get; set; }

        public string TargetValue { get; set; }

        #endregion

        #region Private methods

        private void UpdateTotalCount()
        {
            lblTotal.Text = $"Subject Count: {Store.GetCount()}";
        }

        private IEnumerable<StoreRecord> GetCellPhone()
        {

            if (!string.IsNullOrWhiteSpace(_searchTerm) && IsStoreEdit==false)
            {
                var term = _searchTerm.ToUpperInvariant();
                var acctNum = _searchAcctNum.ToUpperInvariant();

                return Store.Where(x => x.name.Contains(term)).OrderBy(x => x.name);
            }
            //if (!string.IsNullOrWhiteSpace(_searchAcctNum))
            //{
            //	var term = _searchTerm.ToUpperInvariant();
            //	var acctNum = _searchAcctNum.ToUpperInvariant();

            //	return Store.Where(x => x.name.Contains(term) && x.accNum.Contains(acctNum)).OrderBy(x => x.name);
            //}
            return Store.OrderBy(x => x.name);
        }

        private void ShowList()
        {
            listViewCellPhone.BeginUpdate();

            var selected = listViewCellPhone.SelectedItems.OfType<ListViewItem>().FirstOrDefault()?.SubItems[0].Text;
            ListViewItem selectedItem = null;
            listViewCellPhone.Clear();
            foreach (var plateRecord in GetCellPhone())
            {
                var plate = string.IsNullOrEmpty(plateRecord.name) ? plateRecord.name : $"{plateRecord.name} - {plateRecord.accNum}";
                var item = listViewCellPhone.Items.Add(plate);
                item.Tag = plateRecord.accNum;
                if (selected == plate)
                {
                    item.Selected = true;
                    selectedItem = item;
                }
            }

            listViewCellPhone.EndUpdate();
            listViewCellPhone.Refresh();
            selectedItem?.EnsureVisible();
        }

        private string CleanCellPhone(string value)
        {
            return tbSearchAndAdd.Text?.Trim().ToUpperInvariant() ?? string.Empty;
        }
        private string CleanAcctNum(string value)
        {
            return tbAcct.Text?.Trim().ToUpperInvariant() ?? string.Empty;
        }

        #endregion

        #region Private form events



        private void TbSearchTextChanged(object sender, EventArgs e)
        {
            _searchTerm = CleanCellPhone(tbSearchAndAdd.Text);
            //btnAdd.Enabled = !string.IsNullOrWhiteSpace(_searchTerm);
            ShowList();
        }

        private void TbAcctTextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbSearchAndAdd.Text) && !string.IsNullOrWhiteSpace(tbAcct.Text))
            {
                _searchAcctNum = CleanAcctNum(tbAcct.Text);
                btnAdd.Enabled = !string.IsNullOrWhiteSpace(_searchAcctNum);
                ShowList();
            }


        }
        private void CellPhoneWatchlistFormShown(object sender, EventArgs e)
        {
            if (Store == null) throw new ArgumentNullException(nameof(Store));

            tbSearchAndAdd.Text = TargetValue;
            _searchTerm = TargetValue;

            ShowList();
            UpdateTotalCount();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Delete)
            {
                if (!tbSearchAndAdd.Focused && btnDelete.Enabled)
                    btnDelete.PerformClick();
            }
            else if (keyData == Keys.Enter)
            {
                if (tbSearchAndAdd.Focused && btnAdd.Enabled)
                    tbAcct.Focus();
                else if (tbAcct.Focused && btnAdd.Enabled)
                    btnAdd.PerformClick();
            }
            else if (keyData == Keys.Escape)
            {
                Close();
            }
            else if (keyData == (Keys.F | Keys.Control))
            {
                tbSearchAndAdd.Focus();
                tbSearchAndAdd.SelectAll();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void BtnDeleteClick(object sender, EventArgs e)
        {
            var selected = listViewCellPhone.SelectedItems[0];
            var value = (string)selected.Tag;
            if (MessageBox.Show($"Are you sure you want to delete subject '{value}'?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Store.Delete(value);
                listViewCellPhone.Items.RemoveAt(selected.Index);
                UpdateTotalCount();
            }
        }

        private void BtnClearClick(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete all subjects with saved names?", "Confirm clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Store.Clear();
                listViewCellPhone.Items.Clear();
                UpdateTotalCount();
            }
        }

        private void ListViewCellPhoneSelectedIndexChanged(object sender, EventArgs e)
        {
            btnDelete.Enabled = listViewCellPhone.SelectedItems.Count > 0;
            btnEdit.Enabled = listViewCellPhone.SelectedItems.Count > 0;

        }
        private void ListViewStore_DoubleClick(object sender, EventArgs e)
        {
            if (listViewCellPhone.SelectedItems.Count > 0)
            {
                var selectedText = listViewCellPhone.SelectedItems[0].Text.Split('-')[0];
                SessionManager.StoreName = selectedText;
                SessionManager.StoreAccNum = listViewCellPhone.SelectedItems[0].Text.Split('-')[1];
                this.Hide();
                MainForm main = new MainForm();
                main.Show();
            }
        }
        #endregion



        private void btnAdd_Click(object sender, EventArgs e)
        {
            tbAcct.Enabled = true;

            var value = CleanCellPhone(tbSearchAndAdd.Text);
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(tbAcct.Text))
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }
            if (Store.Contains(value) && IsStoreEdit == false)
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }
            var accountStoreExist = Store.FirstOrDefault(x => x.accNum == tbAcct.Text);
            var companyAccountExist = Company.FirstOrDefault(x => x.accNum == tbAcct.Text);
            
            if (accountStoreExist != null && IsStoreEdit == false)
            {
                tbAcct.Text = "";
                MessageBox.Show("Account number is already associated with store " + accountStoreExist.name);
                return;
            }
            if (companyAccountExist != null && IsStoreEdit == false)
            {
                tbAcct.Text = "";
                MessageBox.Show("Account number is already associated with company " + companyAccountExist.name);
                return;
            }
            if (IsStoreEdit == false)
            {
                Store.Add(value, tbAcct.Text, tbCell.Text, "", tbAddress.Text, null);
                var item = listViewCellPhone.Items.Add(value);
                item.Selected = true;
                item.Tag = tbAcct.Text;
                item.EnsureVisible();

            }
            else
            {
                Store.UpdateStore(value, tbAcct.Text, tbCell.Text, accountStoreExist.CameraUrl, tbAddress.Text,SessionManager.CompanyAccNum);

            }
            tbSearchAndAdd.Text = string.Empty;
            tbAcct.Text = string.Empty;
            tbCell.Text = string.Empty;
            tbAddress.Text = string.Empty;

            var store = new StoreCollection();
            Store = store;
            ShowList();

            UpdateTotalCount();
            tbSearchAndAdd.Focus();
            IsStoreEdit = false;
            
            btnAdd.Text = "Add";
        }
        private void lvResult_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            // Draw the column header normally
            e.DrawDefault = true;
            e.DrawBackground();
            e.DrawText();
        }
        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            // Get the graphics object
            Graphics g = e.Graphics;

            // Get the font and text of the current item
            Font font = e.Item.Font;
            string text = e.Item.Text;

            // Set the text color
            Color textColor = e.Item.Selected ? Color.SteelBlue : Color.Black;

            // Set up the text formatting
            TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter;

            // Calculate the rectangle for the item
            Rectangle rect = e.Bounds;

            rect.Width += 10 * text.Length;
            // Draw the text with an underline
            TextRenderer.DrawText(g, text, font, rect, textColor, flags | TextFormatFlags.EndEllipsis | TextFormatFlags.ModifyString);

            // Draw the underline
            int underlineHeight = 1; // Adjust this value to set the height of the underline
            int underlineOffset = 2; // Adjust this value to set the offset of the underline from the text

            Point startPoint = new Point(rect.Left, rect.Bottom - underlineOffset);
            Point endPoint = new Point(rect.Right, rect.Bottom - underlineOffset);

            g.DrawLine(new Pen(textColor, underlineHeight), startPoint, endPoint);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm login = new LoginForm();
            login.Show();
        }

        private void listViewCellPhone_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listViewCellPhone.SelectedItems.Count > 0)
            {
                var selected = listViewCellPhone.SelectedItems[0];
                var value = (string)selected.Tag;

                var store = Store.FirstOrDefault(x => x.accNum == value);
                if (store != null)
                {
                    tbSearchAndAdd.Text = store.name;
                    tbAcct.Text = store.accNum;
                    tbCell.Text = store.cell;
                    tbAddress.Text = store.Address;
                }
                IsStoreEdit = true;
                btnAdd.Enabled = true;
                tbAcct.Enabled = false;
                ShowList();
                btnAdd.Text = "Update";
                btnAdd.Refresh();


            }
        }
    }


}

