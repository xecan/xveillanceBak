using Neurotec.Samples.Code;
using System;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace Neurotec.Samples.Forms
{
	public partial class SubjectForm : Form
	{
		#region Public constructors

		public SubjectForm()
		{
			KeyPreview = true;
			InitializeComponent();
		}

		#endregion

		#region Public properties

		public SubjectInfo Info { get; set; }
		public FaceRecordCollection FaceRecords { get; set; }
		public SubjectsView SubjectsView { get; set; }

		#endregion

		#region Private methods

		private void ShowGallery(int index)
		{
			var matches = Info.Face.BestMatches;
			if (index >= 0 && index < matches?.Length)
			{
				var m = matches[index];
				if (FaceRecords.TryGetValue(m.Id, out var record))
				{
					using (var image = FaceRecords.GetThumbnailById(record.Id))
					{
						var old = pbGalery.Image;
						pbGalery.Image = image.ToBitmap();
						old?.Dispose();
					}
				}
				else
				{
					var old = pbGalery.Image;
					pbGalery.Image = (Bitmap)Properties.Resources.Unknown.Clone();
					old?.Dispose();
				}
			}
		}

		private void ShowInfo()
		{
			Bitmap bmp = Info.BestImage.ToBitmap();
			detailsView1.Info = Info;

			bool hasObjects = !Info.Object.IsEmpty;

			Text = Info.AppearedTimeStamp == new DateTime() ? 
				$"Subject #{Info.TraceIndex}" : 
				Info.DissapearedTimeStamp == new DateTime() ? 
					$"{Info.AppearedTimeStamp.ToLocalTime()} Subject #{Info.TraceIndex}" : 
					$"{Info.AppearedTimeStamp.ToLocalTime()} - {Info.DissapearedTimeStamp.ToLocalTime()} Subject #{Info.TraceIndex}";
			if (hasObjects)
			{
				lblDetectionConf.Text = $"Detection Confidence: {Info.Object.DetectionConfidence * 100:0.00}";
			}
			else
			{
				lblDetectionConf.Visible = false;
			}

			var oldBitmap = pbLicensePlate.Image;
			pbLicensePlate.Image = (Bitmap)Info.LicensePlate.Thumbnail?.Clone();
			oldBitmap?.Dispose();
			if (pbLicensePlate.Image == null)
			{
				tlpLeft.RowStyles[1] = new RowStyle(SizeType.Absolute, 0);
			}
			else
			{
				var lp = Info.LicensePlate.Best;
				tlpLeft.RowStyles[1] = new RowStyle(SizeType.Percent, 100 / 3.0f);
				lblLicensePlate.Text = $"License plate: {lp.Value}";
				lblOrigin.Text = $"Origin: {lp.Origin}";
				lblLprDetectionConf.Text = $"Detection confidence: {lp.DetectionConfidence * 100:0.00}";
				lblOcrConf.Text = $"Ocr confidence: {lp.OcrConfidence * 100:0.00}";
			}

			if (hasObjects || !Info.Face.IsEmpty)
			{ 
				oldBitmap = thumbBox.Image;
				thumbBox.Image = (Bitmap)Info.Thumbnail.Clone();
				oldBitmap?.Dispose();
				tlpLeft.RowStyles[0] = new RowStyle(SizeType.Percent, 100 / 3.0f * 2);
			}
			else
			{
				tlpLeft.RowStyles[0] = new RowStyle(SizeType.Absolute, 0);
			}
			tlpLeft.Invalidate();

			objectView.Image = bmp;
			objectView.Info = Info;

			ShowGallery(0);
		}

		private void UpdateInfo()
		{
			if (Info == null)
				throw new InvalidOperationException();

			tlpCenter.SuspendLayout();
			tlpCenter.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 50);
			tlpCenter.RowStyles[2] = new RowStyle(SizeType.AutoSize, 0);

			cbBestMatches.Items.Clear();
			lblBestMatch.Text = string.Empty;
			if (Info.Face.BestMatches?.Any() == true)
			{
				var matches = Info.Face.BestMatches;
				bool multiple = matches.Length > 1;
				if (multiple)
				{
					lblBestMatch.Text = $"Matched with {matches.Length} subjects";
					foreach (var m in matches)
					{
						cbBestMatches.Items.Add($"{m.Id}, score {m.Score}");
					}
					cbBestMatches.SelectedIndex = 0;
				}
				else
				{
					lblBestMatch.Text = $"Matched with {Info.Face.SubjectId}, Score {Info.Face.Score}";
					tlpCenter.RowStyles[2] = new RowStyle(SizeType.Absolute, 0);
				}
			}
			else
			{
				tlpCenter.ColumnStyles[1] = new ColumnStyle(SizeType.Absolute, 0);
			}
			tlpCenter.ResumeLayout();

			ShowInfo();
		}

		private void SubjectFormLoad(object sender, EventArgs e)
		{
			UpdateInfo();
		}

		private void CbBestMatchesSelectedIndexChanged(object sender, EventArgs e)
		{
			ShowGallery(cbBestMatches.SelectedIndex);
		}

		private void ShowPrevious()
		{
			if (SubjectsView.TrySelectPrevious(out var prev))
			{
				Info = prev;
				UpdateInfo();
			}
			else
			{
				SystemSounds.Beep.Play();
			}
		}

		private void ShowNext()
		{
			if (SubjectsView.TrySelectNext(out var next))
			{
				Info = next;
				UpdateInfo();
			}
			else
			{
				SystemSounds.Beep.Play();
			}
		}

		private void BtnNextClick(object sender, EventArgs e)
		{
			ShowNext();
		}

		private void BtnPrevClick(object sender, EventArgs e)
		{
			ShowPrevious();
		}

		#endregion

		#region Protected methods

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape)
			{
				Close();
				return true;
			}
			else if (keyData == Keys.Left)
			{
				ShowPrevious();
				return true;
			}
			else if (keyData == Keys.Right)
			{
				ShowNext();
				return true;
			}
			else
			{
				return base.ProcessCmdKey(ref msg, keyData);
			}
		}

		#endregion
	}
}
