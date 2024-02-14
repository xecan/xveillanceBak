using Neurotec.Media;
using Neurotec.Surveillance;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neurotec.Samples.Forms
{
	public partial class VideoForm : Form
	{
		private enum OpenStatus
		{
			Pending,
			InProgress,
			Opened,
			Error,
			Canceled
		}

		private class VideoStatus
		{
			public string DisplayName { get; set; }
			public string FileName { get; set; }
			public OpenStatus Status { get; set; }
			public string Resolution { get; set; } = "-";
			public string Length { get; set; } = "-";
			public string Error { get; set; }

			public int Index { get; set; }
			public NMediaReader Reader { get; set; }
		}

		#region Private fields

		private CancellationTokenSource _tokenSource = new CancellationTokenSource();
		private List<VideoStatus> _videos = new List<VideoStatus>();

		#endregion

		#region Public constructor

		public VideoForm()
		{
			InitializeComponent();
		}

		#endregion

		#region Private methods

		private static bool OpenReader(VideoStatus e, bool preferVlc)
		{
			try
			{
				if (File.Exists(e.FileName))
				{
					e.Reader = new NMediaReader(e.FileName, NMediaType.Video, false, preferVlc ? NMediaSource.FlagPreferVlc : 0u);
					e.Length = e.Reader.Length > TimeSpan.FromSeconds(0) ? e.Reader.Length.ToString() : "-";

					try
					{
						var format = e.Reader.Source.GetCurrentFormat(NMediaType.Video) as NVideoFormat;
						if (format != null)
						{
							e.Resolution = string.Format("{0} x {1}", format.Width, format.Height);
						}
					}
					catch
					{
					}

					e.Status = OpenStatus.Opened;
					return true;
				}
				else
				{
					e.Status = OpenStatus.Error;
					e.Error = "File not found";
				}
			}
			catch (Exception ex)
			{
				e.Error = ex.Message;
				e.Status = OpenStatus.Error;
			}
			return false;
		}

		private void OnVideoStatusChanged(VideoStatus e)
		{
			var item = lvVideos.Items[e.Index];
			var lviStatus = item.SubItems[1];
			lviStatus.Text = e.Status.ToString();
			if (e.Status >= OpenStatus.Error)
			{
				item.ForeColor = Color.Red;
				item.ToolTipText = $"Couldn't open file: {e.Error}";
			}
			else if (e.Status == OpenStatus.Opened)
			{
				item.ForeColor = Color.Green;
			}

			item.SubItems[2].Text = e.Resolution;
			item.SubItems[3].Text = e.Length;
		}

		private void Enable(bool enable)
		{
			btnBrowse.Enabled = enable;
			btnOk.Enabled = enable;
			btnReset.Enabled = enable;
		}

		private void CleanupVideos()
		{
			if (_videos.Any())
			{
				foreach (var v in _videos)
				{
					v.Reader?.Dispose();
				}
				_videos.Clear();
			}
		}

		private async Task OpenVideoFiles(params string[] files)
		{
			_tokenSource = new CancellationTokenSource();

			Enable(false);

			chbPreferVlc.Enabled = false;

			tbFileName.ReadOnly = true;
			tbFileName.Text = files.Select(x => Path.GetFileName(x)).Aggregate((a, b) => a + "; " + b);

			CleanupVideos();
			lvVideos.SuspendLayout();
			lvVideos.Items.Clear();

			int rowIndex = 0;
			foreach (var fileName in files)
			{
				var entry = new VideoStatus { FileName = fileName, DisplayName = Path.GetFileName(fileName), Index = rowIndex };

				var lvi = new ListViewItem(new[] { entry.DisplayName, entry.Status.ToString(), "-", "-" })
				{
					UseItemStyleForSubItems = true
				};

				lvVideos.Items.Add(lvi);
				rowIndex++;

				_videos.Add(entry);
			}

			lvVideos.ResumeLayout();

			bool preferVlc = chbPreferVlc.Checked;
			await Task.Factory.StartNew(() =>
			{
				foreach (var item in _videos)
				{
					if (!_tokenSource.IsCancellationRequested)
					{
						item.Status = OpenStatus.InProgress;
						BeginInvoke(new Action<VideoStatus>(OnVideoStatusChanged), item);
						OpenReader(item, preferVlc);
					}
					else
					{
						item.Status = OpenStatus.Canceled;
					}
					BeginInvoke(new Action<VideoStatus>(OnVideoStatusChanged), item);
				}
			}, TaskCreationOptions.LongRunning);

			Enable(true);
		}

		private async void BtnBrowseClick(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				await OpenVideoFiles(openFileDialog.FileNames);
			}
		}

		private void BtnOkClick(object sender, EventArgs e)
		{
			if (_videos.Any() == false)
			{
				MessageBox.Show("Please select a video file(s)");
			}
			else if (_videos.Any(x => x.Status == OpenStatus.Opened) == false)
			{
				MessageBox.Show("Couldn't open any videos, please select a video file(s)");
			}
			else
			{
				Sources = _videos
					.Where(x => x.Status == OpenStatus.Opened)
					.Select(x => new NSurveillanceSource(SelectedModality, x.Reader))
					.ToArray();
				DialogResult = DialogResult.OK;
			}
		}

		private void BtnCancelClick(object sender, EventArgs e)
		{
			_tokenSource.Cancel();
			if (btnBrowse.Enabled)
			{
				CleanupVideos();
				DialogResult = DialogResult.Cancel;
			}
		}

		private void BtnResetClick(object sender, EventArgs e)
		{
			CleanupVideos();
			lvVideos.Items.Clear();
			chbPreferVlc.Enabled = true;
		}

		private async void TbFileNameTextChanged(object sender, EventArgs e)
		{
			if (tbFileName.ReadOnly == false)
			{
				var fileName = tbFileName.Text;
				if (File.Exists(fileName))
				{
					await OpenVideoFiles(fileName);
				}
			}
		}

		private void VideoFormFormClosing(object sender, FormClosingEventArgs e)
		{
			if (!btnOk.Enabled)
			{
				e.Cancel = true;
				_tokenSource.Cancel();
			}
		}

		private void VideoFormShown(object sender, EventArgs e)
		{
			SelectedModality = AllowedModalityTypes;
			if (IsMoreThanOneFlagSet(AllowedModalityTypes))
			{
				tsbFaces.Visible = (AllowedModalityTypes & NSurveillanceModalityType.Faces) != 0;
				tsbObjects.Visible = (AllowedModalityTypes & NSurveillanceModalityType.VehiclesAndHumans) != 0;
				tsbLpr.Visible = (AllowedModalityTypes & NSurveillanceModalityType.LicensePlateRecognition) != 0;
			}
			else
			{
				toolStripModalities.Visible = false;
				lblModalities.Visible = false;
			}
		}

		private static bool IsFlagSet(NSurveillanceModalityType flags, NSurveillanceModalityType flag)
		{
			return (flags & flag) == flag;
		}

		private static bool IsMoreThanOneFlagSet(NSurveillanceModalityType value)
		{
			return (value & (value - 1)) != 0;
		}

		private void OnModalityCheckChanged(bool check, NSurveillanceModalityType modality)
		{
			var value = SelectedModality;
			if (check)
				value |= modality;
			else
				value &= ~modality;

			if (value == NSurveillanceModalityType.None)
			{
				bool SetIfSupported(NSurveillanceModalityType t, ToolStripButton tsb)
				{
					if (modality != t && IsFlagSet(AllowedModalityTypes, t))
					{
						tsb.Checked = true;
						value |= t;
						return true;
					}
					return false;
				}

				if (!SetIfSupported(NSurveillanceModalityType.Faces, tsbFaces))
				{
					if (!SetIfSupported(NSurveillanceModalityType.VehiclesAndHumans, tsbObjects))
						SetIfSupported(NSurveillanceModalityType.LicensePlateRecognition, tsbLpr);
				}
			}

			SelectedModality = value;
		}

		private void TsbFacesClick(object sender, EventArgs e)
		{
			OnModalityCheckChanged(tsbFaces.Checked, NSurveillanceModalityType.Faces);
		}

		private void TsbObjectsClick(object sender, EventArgs e)
		{
			OnModalityCheckChanged(tsbObjects.Checked, NSurveillanceModalityType.VehiclesAndHumans);
		}

		private void TsbLprClick(object sender, EventArgs e)
		{
			OnModalityCheckChanged(tsbLpr.Checked, NSurveillanceModalityType.LicensePlateRecognition);
		}

		#endregion

		#region Public properties

		public NSurveillanceSource[] Sources
		{
			get;
			private set;
		}

		public NSurveillanceSource ActiveSource
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public NSurveillanceModalityType AllowedModalityTypes { get; set; }

		public NSurveillanceModalityType SelectedModality { get; set; }

		#endregion

	}
}
