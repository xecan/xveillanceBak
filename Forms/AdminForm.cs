using Neurotec.Samples.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neurotec.Samples.Forms
{
    public partial class AdminForm : Form
    {
        #region Constants

        private const int PageSize = 50;

        #endregion

        #region Private fields

        private string _searchTerm = string.Empty;
        private string _searchAcctNum = string.Empty;
        private bool _isAllVisitors = false;
        private bool IsStoreEdit = false;
        private bool IsCompanyEdit = false;

        #endregion
        public AdminForm()
        {
            InitializeComponent();
            this.Height = 700;
            this.tabControl1.Height = 645;

            var company = new CompanyCollection();
            var store = new StoreCollection();
            var user = new UserCollection();
            var visitors = new VisitorWatchListCollection();
            var records = new AllVisitorsCollection();


            Company = company;
            Store = store;
            User = user;
            Faces = visitors;
            Record = records;

            GetCompanyList();
            GetStore();
            ShowCompanyList();
            ShowStoreList();
            ShowUserList();
            UpdateTotalCount();
        }


        #region Company

        #region Public properties

        public CompanyCollection Company { get; set; }
        public StoreCollection Store { get; set; }
        public UserCollection User { get; set; }
        public VisitorWatchListCollection Faces { get; set; }
        public AllVisitorsCollection Record { get; set; }

        public string TargetValue { get; set; }

        #endregion

        #region Private methods

        private void UpdateTotalCount()
        {
            lblTotal.Text = $"Subject Count: {Company.GetCount()}";
            lblStoreCount.Text = $"Subject Count: {Store.GetCount()}";
            lblUserCount.Text = $"Subject Count: {User.GetCount()}";
        }

        private IEnumerable<CompanyRecord> GetCompanyList()
        {

            if (!string.IsNullOrWhiteSpace(_searchTerm) && IsCompanyEdit==false)
            {
                var term = _searchTerm.ToUpperInvariant();
                var acctNum = _searchAcctNum.ToUpperInvariant();

                return Company.Where(x => x.name.Contains(term)).OrderBy(x => x.name);
            }
            //if (!string.IsNullOrWhiteSpace(_searchAcctNum))
            //{
            //	var term = _searchTerm.ToUpperInvariant();
            //	var acctNum = _searchAcctNum.ToUpperInvariant();

            //	return Company.Where(x => x.name.Contains(term) && x.accNum.Contains(acctNum)).OrderBy(x => x.name);
            //}
            return Company.OrderBy(x => x.name);
        }

        private void ShowCompanyList()
        {
            //get company list
            listViewCompany.BeginUpdate();
            listViewCompany.Items.Clear();

            listViewCompany.View = View.Details;

            var selected = listViewCompany.SelectedItems.OfType<ListViewItem>().FirstOrDefault()?.SubItems[0].Text;
            ListViewItem selectedItem = null;
            //listViewCompany.CheckBoxes = true;

            string[] arr = new string[10];
            foreach (var company in GetCompanyList())
            {
                ListViewItem item1;

                var plate = string.IsNullOrEmpty(company.name) ? company.name : $"{company.name} - {company.accNum}";

                arr[1] = company.name;
                arr[2] = company.accNum;
                arr[3] = company.EnrollTime.ToString("MMMM dd yyyy");
                if (company.Status == true)
                {
                    arr[0] = "Active";
                    //item1.Checked = true;
                }
                else
                {
                    arr[0] = "InActive";

                }
                item1 = new ListViewItem(arr);
                var item = listViewCompany.Items.Add(item1);
                item.Tag = company.accNum;
                if (selected == plate)
                {
                    item.Selected = true;
                    selectedItem = item;
                }
            }

            listViewCompany.Columns[0].DisplayIndex = listViewCompany.Columns.Count - 1;
            listViewCompany.Invalidate();
            listViewCompany.EndUpdate();
            selectedItem?.EnsureVisible();
            //listViewCompany.ItemChecked += new ItemCheckedEventHandler(CheckedState);

        }

        private void CheckedState(object sender, System.Windows.Forms.ItemCheckedEventArgs e)
        {
            //Display message
            for (int i = 0; i < listViewCompany.Items.Count; i++)
            {
                if (listViewCompany.Items[i] != null)
                {
                    if (e.Item.Tag != null)
                    {

                        if (listViewCompany.Items[i].Checked)
                        {
                            Company.Update(e.Item.Tag.ToString(), true);
                            //MessageBox.Show("Listview items " + listViewCompany.Items[i] + " is checked");
                        }
                        else
                        {
                            Company.Update(e.Item.Tag.ToString(), false);

                            //MessageBox.Show("Listview items " + listViewCompany.Items[i] + " is unchecked");
                        }
                    }
                }
            }

        }

        private void CheckedStoreState(object sender, System.Windows.Forms.ItemCheckedEventArgs e)
        {
            //Display message
            for (int i = 0; i < listViewStore.Items.Count; i++)
            {
                if (listViewStore.Items[i] != null)
                {
                    if (e.Item.Tag != null)
                    {

                        if (listViewStore.Items[i].Checked)
                        {
                            Store.Update(e.Item.Tag.ToString(), true);
                            //MessageBox.Show("Listview items " + listViewCompany.Items[i] + " is checked");
                        }
                        else
                        {
                            Store.Update(e.Item.Tag.ToString(), false);

                            //MessageBox.Show("Listview items " + listViewCompany.Items[i] + " is unchecked");
                        }
                    }
                }
            }

        }

        private void ShowUserList()
        {
            //get user list
            listViewUser.BeginUpdate();

            var selectedUser = listViewUser.SelectedItems.OfType<ListViewItem>().FirstOrDefault()?.SubItems[0].Text;
            ListViewItem selectedUserItem = null;
            listViewUser.Clear();
            foreach (var user in GetUsers())
            {
                //var selectUser = string.IsNullOrEmpty(user.name) ? user.name : $"{user.name} - {user.accNum}";
                var selectUser = user.username;

                var item = listViewUser.Items.Add(selectUser);
                item.Tag = user.username;
                if (selectedUser == selectUser)
                {
                    item.Selected = true;
                    selectedUserItem = item;
                }
            }

            listViewUser.EndUpdate();
            selectedUserItem?.EnsureVisible();
        }
        private void ShowStoreList()
        { //get store list

            listViewStore.BeginUpdate();
            listViewStore.Items.Clear();
            listViewStore.View = View.Details;


            var selectedStore = listViewStore.SelectedItems.OfType<ListViewItem>().FirstOrDefault()?.SubItems[0].Text;
            ListViewItem selectedStoreItem = null;
            //listViewStore.CheckBoxes = true;

            string[] arr = new string[10];
            foreach (var store in GetStore())
            {
                ListViewItem item1;

                var plate = string.IsNullOrEmpty(store.name) ? store.name : $"{store.name} - {store.accNum}";

                arr[1] = store.name;
                arr[2] = store.accNum;
                arr[3] = store.EnrollDate.ToString("MMMM dd yyyy");
                if (store.Status == true)
                {
                    arr[0] = "Active";
                    //item1.Checked = true;
                }
                else
                {
                    arr[0] = "InActive";
                }
                item1 = new ListViewItem(arr);

                var item = listViewStore.Items.Add(item1);
                item.Tag = store.accNum;
                if (selectedStore == plate)
                {
                    item.Selected = true;
                    selectedStoreItem = item;
                }
            }
            listViewStore.Columns[0].DisplayIndex = listViewStore.Columns.Count - 1;
            listViewStore.Invalidate();
            listViewStore.EndUpdate();
            selectedStoreItem?.EnsureVisible();
            //listViewStore.ItemChecked += new ItemCheckedEventHandler(CheckedStoreState);


        }
        private string CleanCompany(string value)
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
            _searchTerm = CleanCompany(tbSearchAndAdd.Text);
            //btnAdd.Enabled = !string.IsNullOrWhiteSpace(_searchTerm);
            ShowCompanyList();
        }

        private void TbAcctTextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbSearchAndAdd.Text) && !string.IsNullOrWhiteSpace(tbAcct.Text))
            {
                _searchAcctNum = CleanAcctNum(tbAcct.Text);
                btnAdd.Enabled = !string.IsNullOrWhiteSpace(_searchAcctNum);
            }
            ShowCompanyList();


        }
        private void CellPhoneWatchlistFormShown(object sender, EventArgs e)
        {
            if (Company == null) throw new ArgumentNullException(nameof(Company));

            tbSearchAndAdd.Text = TargetValue;
            _searchTerm = TargetValue;

            ShowCompanyList();
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
            var selected = listViewCompany.SelectedItems[0];
            var value = (string)selected.Tag;
            if (MessageBox.Show($"Are you sure you want to delete subject '{value}'?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Company.Delete(value);
                listViewCompany.Items.RemoveAt(selected.Index);
                var company = new CompanyCollection();
                Company = company;
                UpdateTotalCount();

            }
        }

        private void BtnClearClick(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete all subjects with saved names?", "Confirm clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Company.Clear();
                listViewCompany.Items.Clear();
                UpdateTotalCount();
            }
        }

        private void ListViewCellPhoneSelectedIndexChanged(object sender, EventArgs e)
        {
            btnDelete.Enabled = listViewCompany.SelectedItems.Count > 0;
            btnCompanyEdit.Enabled = listViewCompany.SelectedItems.Count > 0;

        }
        private void ListViewStore_DoubleClick(object sender, EventArgs e)
        {
            if (listViewCompany.SelectedItems.Count > 0)
            {
                var selectedText = listViewCompany.SelectedItems[0].Text.Split('-')[0];
                SessionManager.StoreName = selectedText;
                this.Hide();
                MainForm main = new MainForm();
                main.Show();
            }
        }
        #endregion



        private void btnAdd_Click(object sender, EventArgs e)
        {
            var value = CleanCompany(tbSearchAndAdd.Text);
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(tbAcct.Text))
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }
            if (Company.Contains(value) && IsCompanyEdit==false)
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }
            var accountStoreExist = Store.FirstOrDefault(x => x.accNum == tbAcct.Text);
            var companyAccountExist = Company.FirstOrDefault(x => x.accNum == tbAcct.Text);
            if (accountStoreExist != null && IsCompanyEdit == false)
            {
                tbAcct.Text = "";
                MessageBox.Show("Account number is already associated with store " + accountStoreExist.name);
                return;
            }
            if (companyAccountExist != null && IsCompanyEdit == false)
            {
                tbAcct.Text = "";
                MessageBox.Show("Account number is already associated with company " + companyAccountExist.name);
                return;
            }
            if (IsCompanyEdit == false)
            {
                Company.Add(value, tbAcct.Text, tbCell.Text, tbCompanyAddress.Text);
                var item = listViewCompany.Items.Add(value);
                item.Selected = true;
                item.Tag = tbAcct.Text;
                item.EnsureVisible();

            }
            else
            {
                Company.UpdateCompany(value, tbAcct.Text, tbCell.Text, tbCompanyAddress.Text);
            }
            //item.Checked = true;
            tbSearchAndAdd.Text = string.Empty;
            tbAcct.Text = string.Empty;
            tbCompanyAddress.Text = string.Empty;
            tbCell.Text = string.Empty;

            var company = new CompanyCollection();
            Company = company;
            UpdateTotalCount();
            tbSearchAndAdd.Focus();
            IsCompanyEdit = false;
            tbAcct.Enabled = true;
            btnAdd.Text = "Add";
            ShowCompanyList();
        }
        #endregion

        #region Store
        private IEnumerable<StoreRecord> GetStore()
        {

            if (!string.IsNullOrWhiteSpace(_searchTerm) && IsStoreEdit == false)
            {
                var term = _searchTerm.ToUpperInvariant();
                var acctNum = _searchAcctNum.ToUpperInvariant();

                return Store.Where(x => x.name.ToUpper().Contains(term)).OrderBy(x => x.name);
            }
            //if (!string.IsNullOrWhiteSpace(_searchAcctNum))
            //{
            //	var term = _searchTerm.ToUpperInvariant();
            //	var acctNum = _searchAcctNum.ToUpperInvariant();

            //	return Store.Where(x => x.name.Contains(term) && x.accNum.Contains(acctNum)).OrderBy(x => x.name);
            //}
            return Store.OrderBy(x => x.name);
        }
        #endregion
        #region User
        private IEnumerable<UserRecord> GetUsers()
        {

            if (!string.IsNullOrWhiteSpace(_searchTerm))
            {
                var term = _searchTerm.ToUpperInvariant();
                var acctNum = _searchAcctNum.ToUpperInvariant();

                return User.Where(x => x.username.Contains(term)).OrderBy(x => x.username);
            }
            //if (!string.IsNullOrWhiteSpace(_searchAcctNum))
            //{
            //	var term = _searchTerm.ToUpperInvariant();
            //	var acctNum = _searchAcctNum.ToUpperInvariant();

            //	return Store.Where(x => x.name.Contains(term) && x.accNum.Contains(acctNum)).OrderBy(x => x.name);
            //}
            return User.OrderBy(x => x.username);
        }
        #endregion

        private void btnStoreAdd_Click(object sender, EventArgs e)
        {
            tbStoreAcctNum.Enabled = true;
            var value = tbStoreName.Text;
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(tbStoreAcctNum.Text))
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }
            if (Store.Contains(value) && IsStoreEdit==false)
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }
            var accountStoreExist = Store.FirstOrDefault(x => x.accNum == tbStoreAcctNum.Text);
            var companyAccountExist = Company.FirstOrDefault(x => x.accNum == tbStoreAcctNum.Text);
            if (tbCompanyAcc.Text != null && tbCompanyAcc.Text != "")
            {
                var companyAccount = Company.FirstOrDefault(x => x.accNum == tbCompanyAcc.Text);
                if (companyAccount == null)
                {
                    tbCompanyAcc.Text = "";
                    MessageBox.Show("Company Account number is not associated with company " + tbCompanyAcc.Text);
                    return;
                }
            }
            if (accountStoreExist != null && IsStoreEdit == false)
            {
                tbStoreAcctNum.Text = "";
                MessageBox.Show("Account number is already associated with store " + accountStoreExist.name);
                return;
            }
            if (companyAccountExist != null && IsStoreEdit == false)
            {
                tbStoreAcctNum.Text = "";
                MessageBox.Show("Account number is already associated with company " + companyAccountExist.name);
                return;
            }
            if (IsStoreEdit == false)
            {
                Store.Add(value, tbStoreAcctNum.Text, tbStoreCell.Text, tbCameraUrl.Text, tbStoreAddress.Text, tbCompanyAcc.Text);

                var item = listViewStore.Items.Add(value);
                item.Selected = true;
                item.Tag = tbAcct.Text;
                item.EnsureVisible();
            }
            else
            {
                Store.UpdateStore(value, tbStoreAcctNum.Text, tbStoreCell.Text, tbCameraUrl.Text, tbStoreAddress.Text, tbCompanyAcc.Text);
            }
            tbStoreAcctNum.Text = string.Empty;
            tbStoreCell.Text = string.Empty;
            tbStoreName.Text = string.Empty;
            tbCameraUrl.Text = string.Empty;
            tbStoreAddress.Text = string.Empty;
            tbCompanyAcc.Text = string.Empty;


            var store = new StoreCollection();
            Store = store;
            tbStoreName.Focus();
            UpdateTotalCount();
            listViewStore.Refresh();
            IsStoreEdit = false;
            ShowStoreList();
            btnStoreAdd.Text = "Add";
        }

        private void btnStoreDelete_Click(object sender, EventArgs e)
        {
            var selected = listViewStore.SelectedItems[0];
            var value = (string)selected.Tag;
            if (MessageBox.Show($"Are you sure you want to delete subject '{value}'?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Store.Delete(value);
                listViewStore.Items.RemoveAt(selected.Index);
                var store = new StoreCollection();
                Store = store;
                GetStore();
                UpdateTotalCount();
            }
        }

        private void btnStoreClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete all subjects with saved names?", "Confirm clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Store.Clear();
                listViewStore.Items.Clear();
                UpdateTotalCount();
            }
        }
        private void ListViewStoreSelectedIndexChanged(object sender, EventArgs e)
        {
            btnStoreDelete.Enabled = listViewStore.SelectedItems.Count > 0;
            btnEdit.Enabled = listViewStore.SelectedItems.Count > 0;
        }
        private void TbStoreNameTextChanged(object sender, EventArgs e)
        {
            _searchTerm = (tbStoreName.Text);
            //btnAdd.Enabled = !string.IsNullOrWhiteSpace(_searchTerm);
            ShowStoreList();
        }

        private void TbStoreAcctTextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbStoreName.Text) && !string.IsNullOrWhiteSpace(tbStoreAcctNum.Text))
            {
                _searchAcctNum = (tbStoreAcctNum.Text);
                btnStoreAdd.Enabled = !string.IsNullOrWhiteSpace(_searchAcctNum);
                ShowStoreList();
            }
        }

        private void TbUserNameTextChanged(object sender, EventArgs e)
        {
            _searchTerm = (tbUsername.Text);
            //btnAdd.Enabled = !string.IsNullOrWhiteSpace(_searchTerm);
            ShowUserList();
        }
        private void TbUserAcctTextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbUsername.Text) && !string.IsNullOrWhiteSpace(tbPassword.Text) && !string.IsNullOrWhiteSpace(tbAccNum.Text))
            {
                _searchAcctNum = (tbAccNum.Text);
                btnUserAdd.Enabled = !string.IsNullOrWhiteSpace(_searchAcctNum);
                ShowUserList();
            }
        }
        private void ListViewUserSelectedIndexChanged(object sender, EventArgs e)
        {
            btnUserDelete.Enabled = listViewUser.SelectedItems.Count > 0;
        }
        private void btnUserAdd_Click(object sender, EventArgs e)
        {
            var role = "";
            var value = tbUsername.Text;
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(tbAccNum.Text))
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }
            var user = User.FirstOrDefault(x => x.username.Contains(value));
            if (user != null)
            {
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show("User " + user.username + " already exist");
                return;
            }
            var store = Store.FirstOrDefault(x => x.accNum == tbAccNum.Text);
            var company = Company.FirstOrDefault(x => x.accNum == tbAccNum.Text);
            if (store != null)
            {
                role = "storeUser";

            }
            else if (company != null)
            {
                role = "companyAdminUser";
            }
            else
            {
                MessageBox.Show("Account number is not associated with store or company");
                return;
            }
            User.Add(value, tbPassword.Text, tbCell.Text, tbAccNum.Text, role, tbUserAddress.Text);
            var item = listViewStore.Items.Add(value);
            item.Selected = true;
            item.Tag = value;
            tbUsername.Text = string.Empty;
            tbPassword.Text = string.Empty;
            tbAccNum.Text = string.Empty;
            tbUserCell.Text = string.Empty;
            tbUserAddress.Text = string.Empty;

            tbUsername.Focus();
            GetUsers();
            GetStore();
            GetCompanyList();
            UpdateTotalCount();
            item.EnsureVisible();

        }

        private void btnUserDelete_Click(object sender, EventArgs e)
        {
            var selected = listViewUser.SelectedItems[0];
            var value = (string)selected.Tag;
            if (MessageBox.Show($"Are you sure you want to delete subject '{value}'?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                User.Delete(value);
                listViewUser.Items.RemoveAt(selected.Index);
                var user = new UserCollection();
                User = user;
                GetUsers();
                UpdateTotalCount();
            }

        }

        private void btnUserClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete all subjects with saved names?", "Confirm clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                User.Clear();
                listViewUser.Items.Clear();
                UpdateTotalCount();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm login = new LoginForm();
            login.Show();
        }

        private void btnStoreClose_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm login = new LoginForm();
            login.Show();
        }

        private void btnUserClose_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm login = new LoginForm();
            login.Show();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Text == "Store")//your specific tabname
            {
                var store = new StoreCollection();
                Store = store;
                this.Height = 749;
                this.tabControl1.Height = 695;
                ShowStoreList();
            }
            else if (tabControl1.SelectedTab.Text == "Company")
            {
                this.Height = 700;
                this.tabControl1.Height = 645;
                ShowCompanyList();
            }
            else if (tabControl1.SelectedTab.Text == "User")
            {
                this.Height = 725;
                this.tabControl1.Height = 670;
                ShowCompanyList();
            }
            else if (tabControl1.SelectedTab.Text == "Visitors")
            {
                this.Height = 725;
                this.tabControl1.Height = 670;
            }
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }
        private void AdminFormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;
        }
        private void btnPOI_Click(object sender, EventArgs e)
        {
            _isAllVisitors = false;
            lvVisitors.BeginUpdate();
            var Existface = Faces.Where(x => x.EnrollTime >= dpFrom.Value && x.EnrollTime <= dpTo.Value && x.FaceId == tbName.Text);
            lvVisitors.Clear();
            if (Existface.Count() > 0)
            {
                var item = lvVisitors.Items.Add(Existface.First().FaceId + " - " + Existface.First().EnrollTime);
                item.Tag = Existface.First().Id;
            }
            else
            {
                foreach (var FaceRecord in Faces)
                {
                    var plate = string.IsNullOrEmpty(FaceRecord.FaceId) ? "" : FaceRecord.FaceId + " - " + FaceRecord.EnrollTime;
                    var item = lvVisitors.Items.Add(plate);
                    item.Tag = FaceRecord.Id;
                }
            }

            lvVisitors.EndUpdate();

        }

        private void btnAllVisitors_Click(object sender, EventArgs e)
        {
            _isAllVisitors = true;
            lvVisitors.BeginUpdate();
            var Existface = Record.Where(x => x.EnrollTime >= dpFrom.Value && x.EnrollTime <= dpTo.Value && x.FaceId == tbName.Text);
            lvVisitors.Clear();
            if (Existface.Count() > 0)
            {
                var item = lvVisitors.Items.Add(Existface.First().FaceId + " - " + Existface.First().EnrollTime);
                item.Tag = Existface.First().Id;
            }
            else
            {
                foreach (var FaceRecord in Record)
                {
                    var plate = string.IsNullOrEmpty(FaceRecord.FaceId) ? "" : FaceRecord.FaceId + " - " + FaceRecord.EnrollTime;
                    var item = lvVisitors.Items.Add(plate);
                    item.Tag = FaceRecord.Id;
                }
            }

            lvVisitors.EndUpdate();

        }

        private void lvVisitors_DoubleClick(object sender, EventArgs e)
        {
            if (lvVisitors.SelectedItems.Count > 0)
            {
                if (_isAllVisitors == true)
                {
                    var selectedText = lvVisitors.SelectedItems[0].Tag;
                    SessionManager.FaceId = selectedText.ToString();
                    SessionManager.IsAllVisitors = _isAllVisitors;
                }
                else
                {
                    var selectedText = lvVisitors.SelectedItems[0].Text.Split('-')[0];
                    SessionManager.FaceId = selectedText;
                    SessionManager.IsAllVisitors = _isAllVisitors;
                }

                VisitorDetailForm visitor = new VisitorDetailForm();
                visitor.Show();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listViewStore.SelectedItems.Count > 0)
            {
                var selected = listViewStore.SelectedItems[0];
                var value = (string)selected.Tag;

                var store = Store.FirstOrDefault(x => x.accNum == value);
                if (store != null)
                {
                    tbStoreName.Text = store.name;
                    tbStoreAcctNum.Text = store.accNum;
                    tbCameraUrl.Text = store.CameraUrl;
                    tbStoreCell.Text = store.cell;
                    tbCompanyAcc.Text = store.CompanyAccNum;
                    tbStoreAddress.Text = store.Address;
                }
                IsStoreEdit = true;
                btnStoreAdd.Enabled = true;
                tbStoreAcctNum.Enabled = false;
                ShowStoreList();
                btnStoreAdd.Text = "Update";
                btnStoreAdd.Refresh();


            }
        }

        private void btnCompanyEdit_Click(object sender, EventArgs e)
        {
            if (listViewCompany.SelectedItems.Count > 0)
            {
                var selected = listViewCompany.SelectedItems[0];
                var value = (string)selected.Tag;

                var company = Company.FirstOrDefault(x => x.accNum == value);
                if (company != null)
                {
                    tbSearchAndAdd.Text = company.name;
                    tbAcct.Text = company.accNum;
                    tbCell.Text = company.cell;
                    tbCompanyAddress.Text = company.Address;
                }
                IsCompanyEdit = true;
                btnAdd.Enabled = true;
                tbAcct.Enabled = false;
                ShowCompanyList();
                btnAdd.Text = "Update";
                btnAdd.Refresh();


            }
        }
    }
}
