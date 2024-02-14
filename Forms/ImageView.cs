using Neurotec.Biometrics;
using Neurotec.Samples.Code;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Neurotec.Samples.Forms
{
	public class ImageView : Neurotec.Gui.NView
	{
		#region Private fields

		private Bitmap _bmp;
		private SubjectInfo _info;

		#endregion

		#region Public constructor

		public ImageView()
		{
			CanPan = true;
		}

		#endregion

		#region Public properties

		public Bitmap Image
		{
			get => _bmp;
			set
			{
				_bmp?.Dispose();
				_bmp = value;
				if (value != null)
				{
					DataChanged(_bmp.Width, _bmp.Height);
				}
				else
					DataChanged(1, 1);
			}
		}

		public SubjectInfo Info
		{
			get => _info;
			set
			{
				_info = value;
				Invalidate();
			}
		}

		#endregion

		#region Protected methods

		protected void DrawSubject(Graphics g, SubjectInfo info)
		{
			var brush = Brushes.Green;
			using (var pen = new Pen(brush))
			{
				var faceRect = info.Face.Rect;
				if (!faceRect.IsEmpty)
				{
					g.DrawRectangle(pen, faceRect.X, faceRect.Y, faceRect.Width, faceRect.Height);
					if (info.Face != null)
					{
						var attributes = info.Face.BestAttributes;
						var conf = attributes.GetAttributeValue(attributes.Gender == Biometrics.NGender.Male ? NBiometricAttributeId.GenderMale : NBiometricAttributeId.GenderFemale);
						var text = $"{attributes.Gender} ({conf:0.00})";
						var size = g.MeasureString(text, Font);
						g.DrawString(text, Font, brush, faceRect.Left + (faceRect.Width - size.Width) / 2, faceRect.Bottom + 5);
					}
				}

				var obj = info.Object;
				if (!obj.IsEmpty)
				{
					g.DrawRectangle(pen, obj.Rect);

					var vehicleDetails = obj.VehicleDetails;
					if (vehicleDetails != null)
					{
						var vehicleModel = vehicleDetails.Models.First();
						var makeModel = vehicleModel.MakeModels.First();
						var make = makeModel.Key;
						var model = makeModel.Value;
						var text = $"{make} ({vehicleModel.Confidence * 100:0.00}) - {model}";
						var rect = obj.Rect;
						var size = g.MeasureString(text, Font);
						g.DrawString(text, Font, brush, rect.Left + (rect.Width - size.Width) / 2, rect.Top - size.Height - 5);

						if (vehicleDetails.Tags.Any())
						{
							var tag = vehicleDetails.Tags.First();
							text = $"{tag.Name} ({tag.Confidence * 100:0.00})";
							size = g.MeasureString(text, Font);
							g.DrawString(text, Font, brush, rect.Left + (rect.Width - size.Width) / 2, rect.Top + 5);
						}

						text = $"{vehicleDetails.Orientation} - {vehicleDetails.OrientationAngle:0.00}? ({vehicleDetails.OrientationConfidence * 100:0.00})";
						size = g.MeasureString(text, Font);
						g.DrawString(text, Font, brush, rect.Left + (rect.Width - size.Width) / 2, rect.Bottom + 5);
					}

					var clothingDetails = obj.ClothingDetails;
					if (clothingDetails != null)
					{
						var text = $"{clothingDetails.Gender} ({clothingDetails.GenderConfidence:0.00})";
						var rect = obj.Rect;
						var size = g.MeasureString(text, Font);
						g.DrawString(text, Font, brush, rect.Left + (rect.Width - size.Width) / 2, rect.Bottom - size.Height - 5);
						var prev = size;

						var clothes = clothingDetails.Values.Select(x => $"{x.Name} ({x.Confidence * 100:0.00})").ToArray();
						if (clothes.Any())
						{
							text = clothes.Aggregate((a, b) => a + ", " + b);
							size = g.MeasureString(text, Font);
							g.DrawString(text, Font, brush, rect.Left + (rect.Width - size.Width) / 2, rect.Bottom - size.Height - 5 - prev.Height - 5);
						}
					}
				}

				if (!info.LicensePlate.IsEmpty)
				{
					var lp = info.LicensePlate.Best;
					var rect = new Rectangle(lp.Rectangle.X, lp.Rectangle.Y, lp.Rectangle.Width, lp.Rectangle.Height);
					var text = lp.Value;
					var w = g.MeasureString(text, Font).Width;

					g.TranslateTransform(lp.Rectangle.X + lp.Rectangle.Width / 2, lp.Rectangle.Y);
					g.RotateTransform(lp.Rotation);
					g.TranslateTransform(-lp.Rectangle.X - lp.Rectangle.Width / 2, -lp.Rectangle.Y);

					g.DrawString(text, Font, brush, rect.Left + (rect.Width - w) / 2, rect.Bottom + 5);
					g.DrawRectangle(pen, rect);

					g.TranslateTransform(lp.Rectangle.X + lp.Rectangle.Width / 2, lp.Rectangle.Y);
					g.RotateTransform(-lp.Rotation);
					g.TranslateTransform(-lp.Rectangle.X - lp.Rectangle.Width / 2, -lp.Rectangle.Y);
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			var g = e.Graphics;
			if (_bmp != null)
			{
				PrepareGraphics(g, new Matrix());

				g.DrawImage(_bmp, 0, 0, _bmp.Width, _bmp.Height);
				DrawSubject(g, Info);
			}
		}

		#endregion
	}
}
