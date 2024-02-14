using Neurotec.Samples.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Neurotec.Samples.Forms
{
    public partial class CellPhoneWatchlistForm : Form
    {
		#region Constants

		private const int PageSize = 50;

		#endregion

		#region Private fields

		private string _searchTerm = string.Empty;

		#endregion

		#region Public constructor

		public CellPhoneWatchlistForm()
		{
			InitializeComponent();
		}

		#endregion

		#region Public properties

		public CellPhoneCollection CellPhone { get; set; }
		public string TargetValue { get; set; }

		#endregion

		#region Private methods

		private void UpdateTotalCount()
		{
			lblTotal.Text = $"Subject Count: {CellPhone.GetCount()}";
		}

		private IEnumerable<CellPhoneRecord> GetCellPhone()
		{
			if (!string.IsNullOrWhiteSpace(_searchTerm))
			{
				var term = _searchTerm.ToUpperInvariant();
				return CellPhone.Where(x => x.Value.Contains(term)).OrderBy(x => x.Value);
			}
			return CellPhone.OrderBy(x => x.Value);
		}
		
		private void ShowList()
		{
			listViewCellPhone.BeginUpdate();

			var selected = listViewCellPhone.SelectedItems.OfType<ListViewItem>().FirstOrDefault()?.SubItems[0].Text;
			ListViewItem selectedItem = null;
			listViewCellPhone.Clear();
			foreach (var plateRecord in GetCellPhone())
			{
				var plate = string.IsNullOrEmpty(plateRecord.Owner) ? plateRecord.Value : $"{plateRecord.Value} - {plateRecord.Owner}";
				var item = listViewCellPhone.Items.Add(plate);
				item.Tag = plateRecord.Value;
				if (selected == plate)
				{
					item.Selected = true;
					selectedItem = item;
				}
			}

			listViewCellPhone.EndUpdate();
			selectedItem?.EnsureVisible();
		}

		private string CleanCellPhone(string value)
		{
			return tbSearchAndAdd.Text?.Trim().ToUpperInvariant() ?? string.Empty;
		}

		#endregion

		#region Private form events

		private void BtnAddClick(object sender, EventArgs e)
		{
			var value = CleanCellPhone(tbSearchAndAdd.Text);
			if (string.IsNullOrWhiteSpace(value))
			{
				System.Media.SystemSounds.Beep.Play();
			}
			if (CellPhone.Contains(value))
			{
				System.Media.SystemSounds.Beep.Play();
				return;
			}

			CellPhone.Add(value, tbOwner.Text);
			var item = listViewCellPhone.Items.Add(value);
			item.Selected = true;
			item.Tag = value;
			tbSearchAndAdd.Text = string.Empty;
			tbOwner.Text = string.Empty;
			item.EnsureVisible();

			UpdateTotalCount();
			tbSearchAndAdd.Focus();
		}

		private void TbSearchTextChanged(object sender, EventArgs e)
		{
			_searchTerm = CleanCellPhone(tbSearchAndAdd.Text);
			btnAdd.Enabled = !string.IsNullOrWhiteSpace(_searchTerm);
			ShowList();
		}

		private void CellPhoneWatchlistFormShown(object sender, EventArgs e)
		{
			if (CellPhone == null) throw new ArgumentNullException(nameof(CellPhone));

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
					tbOwner.Focus();
				else if (tbOwner.Focused && btnAdd.Enabled)
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
				CellPhone.Delete(value);
				listViewCellPhone.Items.RemoveAt(selected.Index);
				UpdateTotalCount();
			}
		}

		private void BtnClearClick(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to delete all subjects with saved names?", "Confirm clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			{
				CellPhone.Clear();
				listViewCellPhone.Items.Clear();
				UpdateTotalCount();
			}
		}

		private void ListViewCellPhoneSelectedIndexChanged(object sender, EventArgs e)
		{
			btnDelete.Enabled = listViewCellPhone.SelectedItems.Count > 0;
		}

        #endregion

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lblTotal_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void tbOwner_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {

        }
    }
}
