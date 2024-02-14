using Neurotec.Media;
using Neurotec.Samples.Code;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Neurotec.Samples.Forms
{
	public partial class SourceSettingsForm : Form
	{
		#region Private fields

		private SearchAreaConfig _searchAreaConfig;

		#endregion

		#region Public constructor

		public SourceSettingsForm()
		{
			InitializeComponent();
		}

		#endregion

		#region Public properties

		public SourceController State { get; set; }
		public Dictionary<string, Preset> Presets { get; set; }

		#endregion

		#region Public methods

		public void SaveSettings()
		{
			State.ShowSearchArea = chbShowSearchArea.Checked;
			State.OnChangeSearchArea(_searchAreaConfig);

			var selectedFormat = cbFormats.SelectedItem as NMediaFormat;
			if (selectedFormat != null)
			{
				State.OnChangeFormat(selectedFormat);
				State.SelectedFormat = selectedFormat;
			}

			if (cbPreset.SelectedIndex == 0)
			{
				State.OnChangePreset(null);
			}
			else
			{
				Preset selectedPreset = cbPreset.SelectedItem as Preset;
				State.OnChangePreset(selectedPreset.PresetGUID);
			}
		}

		#endregion

		#region Private methods

		private void ButtonSelectClick(object sender, EventArgs e)
		{
			using (var form = new SelectRegionOfInterestForm()
			{
				Source = State.Source,
				AreaConfig = _searchAreaConfig
			})
			{
				if (form.ShowDialog() == DialogResult.OK)
				{
					_searchAreaConfig = form.AreaConfig;
					UpdateControls();
				}
			}
		}

		private void UpdateControls()
		{
			bool selected = _searchAreaConfig != null;
			lblSearchArea.Text = selected ? "Search area selected" : "Not selected";
		}

		private void SourceSettingsFormShown(object sender, EventArgs e)
		{
			buttonSelect.Enabled = State.CanChangeProperties;
			cbFormats.Enabled = State.CanChangeProperties;
			cbPreset.Enabled = State.CanChangeProperties;

			if (SurveillanceConfig.SearchArea.TryGetValue(State.SourceId, out var config))
			{
				_searchAreaConfig = config;
			}

			chbShowSearchArea.Checked = State.ShowSearchArea;
			cbFormats.Items.Clear();
			if (State.Formats != null)
			{
				cbFormats.Items.AddRange(State.Formats);
				cbFormats.SelectedItem = State.SelectedFormat;
			}

			cbPreset.Items.Add("Default");
			cbPreset.SelectedIndex = 0;
			if (Presets != null)
			{
				foreach (var item in Presets)
				{
					cbPreset.Items.Add(item.Value);
					if (item.Key == State.SelectedPreset)
					{
						cbPreset.SelectedItem = item.Value;
					}
				}
			}

			UpdateControls();
		}

		#endregion
	}
}
