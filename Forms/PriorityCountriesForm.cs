using Neurotec.Surveillance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Neurotec.Samples.Forms
{
	public partial class PriorityCountriesForm : Form
	{

		#region Public constructor

		public PriorityCountriesForm()
		{
			InitializeComponent();
		}

		#endregion Public constructor

		#region Public properties

		public string PriorityCountries { get; set; }

		#endregion Public properties

		#region Private properties

		private Dictionary<string, string> countryCodeMap = new Dictionary<string, string>();

		#endregion Private properties

		#region Private methods

		private void LoadCountryList()
		{
			foreach (var entry in countryCodeMap)
			{
				listBoxSupported.Items.Add(AddCountryName(entry.Key));
			}

			var countryCodeList = PriorityCountries.Split(' ');
			foreach (var countryCode in countryCodeList)
			{
				if (countryCode.Length > 1)
				{
					listBoxSelected.Items.Add(AddCountryName(countryCode));
					listBoxSupported.Items.Remove(AddCountryName(countryCode));
				}
			}
		}

		private string AddCountryName(string countryCode)
		{
			string trimmed = countryCode.Trim();
			if (countryCodeMap.ContainsKey(trimmed))
			{
				return trimmed + " - " + countryCodeMap[trimmed];
			}
			return trimmed;
		}

		private string RemoveCountryName(string input)
		{
			int index = input.IndexOf("-");
			if (index > 0)
			{
				input = input.Substring(0, index);
			}

			return input.Trim();
		}

		private void MoveToList(ListBox from, ListBox to)
		{
			var lastSelected = from.SelectedIndex;
			while (from.SelectedItems.Count > 0)
			{
				to.Items.Add(from.SelectedItems[0]);
				lastSelected = from.SelectedIndex;
				from.Items.Remove(from.SelectedItems[0]);
			}

			if (lastSelected != -1 && from.Items.Count > 0)
			{
				if (lastSelected >= from.Items.Count)
				{
					from.SetSelected(lastSelected - 1, true);
				}
				else
				{
					from.SetSelected(lastSelected, true);
				}
			}
		}

		#endregion Private methods

		#region Private events

		private void PriorityCountriesFormShown(object sender, EventArgs e)
		{
			countryCodeMap = NSurveillance.GetSupportedPriorityCountryCodes()
				.ToDictionary(x => x.Key, x => x.Value);

			LoadCountryList();
		}

		private void ButtonAddClick(object sender, EventArgs e)
		{
			MoveToList(listBoxSupported, listBoxSelected);
		}

		private void ButtonRemoveClick(object sender, EventArgs e)
		{
			MoveToList(listBoxSelected, listBoxSupported);
		}

		private void ButtonUpClick(object sender, EventArgs e)
		{
			int selectedIndex = listBoxSelected.SelectedIndex;
			if (selectedIndex > 0)
			{
				listBoxSelected.Items.Insert(selectedIndex - 1, listBoxSelected.Items[selectedIndex]);
				listBoxSelected.Items.RemoveAt(selectedIndex + 1);
				listBoxSelected.SelectedIndex = selectedIndex - 1;
			}
		}

		private void ButtonDownClick(object sender, EventArgs e)
		{
			int selectedIndex = listBoxSelected.SelectedIndex;
			if (selectedIndex < listBoxSelected.Items.Count - 1 & selectedIndex != -1)
			{
				listBoxSelected.Items.Insert(selectedIndex + 2, listBoxSelected.Items[selectedIndex]);
				listBoxSelected.Items.RemoveAt(selectedIndex);
				listBoxSelected.SelectedIndex = selectedIndex + 1;
			}
		}

		private void ListBoxSupportedSelectedValueChanged(object sender, EventArgs e)
		{
			buttonAdd.Enabled = listBoxSupported.SelectedIndex != -1;
		}

		private void ListBoxSelectedSelectedValueChanged(object sender, EventArgs e)
		{
			buttonRemove.Enabled = listBoxSelected.SelectedIndex != -1;
			buttonUp.Enabled = listBoxSelected.SelectedItems.Count == 1;
			if (listBoxSelected.SelectedIndex <= 0)
			{
				buttonUp.Enabled = false;
			}
			buttonDown.Enabled = listBoxSelected.SelectedItems.Count == 1;
			if (listBoxSelected.SelectedIndex == listBoxSelected.Items.Count - 1)
			{
				buttonDown.Enabled = false;
			}
		}

		private void ButtonOKClick(object sender, EventArgs e)
		{
			PriorityCountries = "";
			foreach (var item in listBoxSelected.Items)
			{
				PriorityCountries += RemoveCountryName(item.ToString()) + " ";
			}

			if (listBoxSelected.Items.Count > 0)
			{
				PriorityCountries = PriorityCountries.Remove(PriorityCountries.Length - 1);
			}
		}

		#endregion
	}
}
