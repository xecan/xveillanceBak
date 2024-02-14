using Neurotec.Samples.Code;
using Neurotec.Surveillance;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Neurotec.Samples.Forms
{
	public partial class SelectFilterForm : Form
	{
		#region Public constructor

		public SelectFilterForm()
		{
			InitializeComponent();
		}

		#endregion

		#region Public properties

		public Filter Filter { get; set; }

		#endregion

		#region Private methods

		private void PaintAttribute(Graphics g, Control target, GraphicsPath path, Brush brush, Pen pen = null)
		{
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			var w = target.Width - 8;
			var h = target.Height - 8;

			var bounds = path.GetBounds();
			var scaleY = h / bounds.Height;
			var scaleX = w / bounds.Width;
			var scale = Math.Min(scaleX, scaleY);
			var dx = (target.Width - bounds.Width * scale) / 2;
			var dy = (target.Height - bounds.Height * scale) / 2;
			var m = new Matrix();
			m.Translate(dx, dy);
			m.Scale(scale, scale);
			path.Transform(m);

			g.FillPath(brush, path);
			if (pen != null)
				g.DrawPath(pen, path);
		}

		private void OnPanelMouseEnter(object sender, EventArgs e)
		{
			var target = (Panel)sender;
			target.BackColor = SystemColors.ControlLight;
			target.Invalidate();
		}

		private void OnPanelMouseLeave(object sender, EventArgs e)
		{
			var target = (Panel)sender;
			target.BackColor = SystemColors.ControlLightLight;
			target.Invalidate();
		}

		private void ChbWithFaceCheckedChanged(object sender, EventArgs e)
		{
			Filter.MustHaveFace = chbWithFace.Checked;
			if (chbWithFace.Checked)
				chbWithLicensePlate.Checked = false;
		}

		private void ChbWithLicensePlateCheckedChanged(object sender, EventArgs e)
		{
			Filter.MustHaveLicensePlate = chbWithLicensePlate.Checked;
			if (chbWithLicensePlate.Checked)
				chbWithFace.Checked = false;
		}

		private void RbAllCheckedChanged(object sender, EventArgs e)
		{
			if (rbAll.Checked)
			{
				Filter.WatchlistFilter = null;
			}
		}

		private void RbInWatchlistCheckedChanged(object sender, EventArgs e)
		{
			if (rbInWatchlist.Checked)
			{
				Filter.WatchlistFilter = true;
			}
		}

		private void RbNotInWatchlistCheckedChanged(object sender, EventArgs e)
		{
			if (rbNotInWatchlist.Checked)
			{
				Filter.WatchlistFilter = false;
			}
		}

		#endregion

		#region Types filter

		private void CheckType(Panel target, NSurveillanceObjectType value)
		{
			if (IsFlagSet(Filter.Type, value))
			{
				Filter.Type &= ~value;
				target.BorderStyle = BorderStyle.None;
			}
			else
			{
				Filter.Type |= value;
				target.BorderStyle = BorderStyle.FixedSingle;
			}
			target.Invalidate();
		}

		private Brush GetBrush(NSurveillanceObjectType value)
		{
			return IsFlagSet(Filter.Type, value) ? Brushes.Black : Brushes.Gray;
		}

		private void PanelCarPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, PathUtils.CreateCarPath(), GetBrush(NSurveillanceObjectType.Car));
		}

		private void PanelTruckPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, PathUtils.CreateTruckPath(), GetBrush(NSurveillanceObjectType.Truck));
		}

		private void PanelBusPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, PathUtils.CreateBusPath(), GetBrush(NSurveillanceObjectType.Bus));
		}

		private void PanelBikePaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, PathUtils.CreateBikePath(), GetBrush(NSurveillanceObjectType.Bike));
		}

		private void PanelPersonPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, PathUtils.CreatePersonPath(), GetBrush(NSurveillanceObjectType.Person));
		}

		private void PanelCarMouseDown(object sender, MouseEventArgs e)
		{
			CheckType((Panel)sender, NSurveillanceObjectType.Car);
		}

		private void PanelTruckMouseDown(object sender, MouseEventArgs e)
		{
			CheckType((Panel)sender, NSurveillanceObjectType.Truck);
		}

		private void PanelBusMouseDown(object sender, MouseEventArgs e)
		{
			CheckType((Panel)sender, NSurveillanceObjectType.Bus);
		}

		private void PanelBikeMouseDown(object sender, MouseEventArgs e)
		{
			CheckType((Panel)sender, NSurveillanceObjectType.Bike);
		}

		private void PanelPersonMouseDown(object sender, MouseEventArgs e)
		{
			CheckType((Panel)sender, NSurveillanceObjectType.Person);
		}

		private void TrackBarTypeConfidenceScroll(object sender, EventArgs e)
		{
			Filter.TypeConfidence = trackBarTypeConfidence.Value / 100f;
			lblTypeConf.Text = $"Confidence: {Filter.TypeConfidence:0.00}";
		}

		private void BtnDefaultClick(object sender, EventArgs e)
		{
			Filter.Type = NSurveillanceObjectType.None;
			Filter.TypeConfidence = .5f;
			Filter.Color = NSurveillanceObjectColor.None;
			Filter.ColorConfidence = .5f;
			Filter.Direction = NSurveillanceObjectDirection.None;
			Filter.DirectionConfidence = .3f;
			Filter.WatchlistFilter = null;
			Filter.MustHaveFace = false;
			Filter.MustHaveLicensePlate = false;

			LoadFilter();
		}

		private BorderStyle GetBorderStyle(bool withBorder)
		{
			return withBorder ? BorderStyle.FixedSingle : BorderStyle.None;
		}

		private void LoadFilter()
		{
			if (Filter.WatchlistFilter == null)
				rbAll.Checked = true;
			else if (Filter.WatchlistFilter == true)
				rbInWatchlist.Checked = true;
			else
				rbNotInWatchlist.Checked = true;
			chbWithFace.Checked = Filter.MustHaveFace;
			chbWithLicensePlate.Checked = Filter.MustHaveLicensePlate;

			trackBarColorConf.Value = (int)(Filter.ColorConfidence * 100);
			lblColorConfidence.Text = $"Confidence: {Filter.ColorConfidence:0.00}";
			trackBarDirectionConf.Value = (int)(Filter.DirectionConfidence * 100);
			lblDirectionConf.Text = $"Confidence: {Filter.DirectionConfidence:0.00}";
			trackBarTypeConfidence.Value = (int)(Filter.TypeConfidence * 100);
			lblTypeConf.Text = $"Confidence: {Filter.TypeConfidence:0.00}";

			panelCar.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Type, NSurveillanceObjectType.Car));
			panelTruck.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Type, NSurveillanceObjectType.Truck));
			panelBus.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Type, NSurveillanceObjectType.Bus));
			panelBike.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Type, NSurveillanceObjectType.Bike));
			panelPerson.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Type, NSurveillanceObjectType.Person));

			panelRed.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Color, NSurveillanceObjectColor.Red));
			panelOrange.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Color, NSurveillanceObjectColor.Orange));
			panelYellow.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Color, NSurveillanceObjectColor.Yellow));
			panelGreen.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Color, NSurveillanceObjectColor.Green));
			panelBlue.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Color, NSurveillanceObjectColor.Blue));
			panelSilver.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Color, NSurveillanceObjectColor.Silver));
			panelWhite.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Color, NSurveillanceObjectColor.White));
			panelBlack.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Color, NSurveillanceObjectColor.Black));
			panelBrown.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Color, NSurveillanceObjectColor.Brown));
			panelGray.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Color, NSurveillanceObjectColor.Gray));

			panelNorth.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Direction, NSurveillanceObjectDirection.North));
			panelNorthEast.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Direction, NSurveillanceObjectDirection.NorthEast));
			panelEast.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Direction, NSurveillanceObjectDirection.East));
			panelSouthEast.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Direction, NSurveillanceObjectDirection.SouthEast));
			panelSouth.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Direction, NSurveillanceObjectDirection.South));
			panelSouthWest.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Direction, NSurveillanceObjectDirection.SouthWest));
			panelWest.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Direction, NSurveillanceObjectDirection.West));
			panelNorthWest.BorderStyle = GetBorderStyle(IsFlagSet(Filter.Direction, NSurveillanceObjectDirection.NorthWest));

			Invalidate();
		}

		private void SelectFilterFormShown(object sender, EventArgs e)
		{
			LoadFilter();
		}

		#endregion

		#region Directions filter

		private Brush GetBrush(NSurveillanceObjectDirection value)
		{
			return IsFlagSet(Filter.Direction, value) ? Brushes.Black : Brushes.Gray;
		}

		private void CheckDirection(Panel target, NSurveillanceObjectDirection value)
		{
			if (IsFlagSet(Filter.Direction, value))
			{
				Filter.Direction &= ~value;
				target.BorderStyle = BorderStyle.None;
			}
			else
			{
				Filter.Direction |= value;
				target.BorderStyle = BorderStyle.FixedSingle;
			}
			target.Invalidate();
		}

		private void PanelNorthWestPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, PathUtils.CreateNorthWestPath(), GetBrush(NSurveillanceObjectDirection.NorthWest));
		}

		private void PanelNorthPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, PathUtils.CreateNorthPath(), GetBrush(NSurveillanceObjectDirection.North));
		}

		private void PanelNorthEastPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, PathUtils.CreateNorthEastPath(), GetBrush(NSurveillanceObjectDirection.NorthEast));
		}

		private void PanelWestPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, PathUtils.CreateWestPath(), GetBrush(NSurveillanceObjectDirection.West));
		}

		private void PanelEastPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, PathUtils.CreateEastPath(), GetBrush(NSurveillanceObjectDirection.East));
		}

		private void PanelSouthWestPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, PathUtils.CreateSouthWestPath(), GetBrush(NSurveillanceObjectDirection.SouthWest));
		}

		private void PanelSouthPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, PathUtils.CreateSouthPath(), GetBrush(NSurveillanceObjectDirection.South));
		}

		private void PanelSouthEastPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, PathUtils.CreateSouthEastPath(), GetBrush(NSurveillanceObjectDirection.SouthEast));
		}

		private void PanelNorthWestMouseDown(object sender, MouseEventArgs e)
		{
			CheckDirection((Panel)sender, NSurveillanceObjectDirection.NorthWest);
		}

		private void PanelNorthMouseDown(object sender, MouseEventArgs e)
		{
			CheckDirection((Panel)sender, NSurveillanceObjectDirection.North);
		}

		private void PanelNorthEastMouseDown(object sender, MouseEventArgs e)
		{
			CheckDirection((Panel)sender, NSurveillanceObjectDirection.NorthEast);
		}

		private void PanelEastMouseDown(object sender, MouseEventArgs e)
		{
			CheckDirection((Panel)sender, NSurveillanceObjectDirection.East);
		}

		private void PanelSouthEastMouseDown(object sender, MouseEventArgs e)
		{
			CheckDirection((Panel)sender, NSurveillanceObjectDirection.SouthEast);
		}

		private void PanelSouthMouseDown(object sender, MouseEventArgs e)
		{
			CheckDirection((Panel)sender, NSurveillanceObjectDirection.South);
		}

		private void PanelSouthWestMouseDown(object sender, MouseEventArgs e)
		{
			CheckDirection((Panel)sender, NSurveillanceObjectDirection.SouthWest);
		}

		private void PanelWestMouseDown(object sender, MouseEventArgs e)
		{
			CheckDirection((Panel)sender, NSurveillanceObjectDirection.West);
		}

		private void TrackBarDirectionConfScroll(object sender, EventArgs e)
		{
			Filter.DirectionConfidence = trackBarDirectionConf.Value / 100f;
			lblDirectionConf.Text = $"Confidence: {Filter.DirectionConfidence:0.00}";
		}

		#endregion

		#region Color filter

		private Brush GetBrush(NSurveillanceObjectColor value)
		{
			switch (value)
			{
				case NSurveillanceObjectColor.Red: return Brushes.Red;
				case NSurveillanceObjectColor.Orange: return Brushes.Orange;
				case NSurveillanceObjectColor.Yellow: return Brushes.Yellow;
				case NSurveillanceObjectColor.Green: return Brushes.Green;
				case NSurveillanceObjectColor.Blue: return Brushes.Blue;
				case NSurveillanceObjectColor.Silver: return Brushes.Silver;
				case NSurveillanceObjectColor.White: return Brushes.White;
				case NSurveillanceObjectColor.Black: return Brushes.Black;
				case NSurveillanceObjectColor.Brown: return Brushes.Brown;
				case NSurveillanceObjectColor.Gray: return Brushes.Gray;
				default:
					throw new NotImplementedException();
			}
		}

		private void CheckColor(Panel target, NSurveillanceObjectColor value)
		{
			if (IsFlagSet(Filter.Color, value))
			{
				Filter.Color &= ~value;
				target.BorderStyle = BorderStyle.None;
			}
			else
			{
				Filter.Color |= value;
				target.BorderStyle = BorderStyle.FixedSingle;
			}
			target.Invalidate();
		}

		private GraphicsPath CreateCirclePath()
		{
			var gp = new GraphicsPath();
			gp.AddEllipse(0, 0, 100, 100);

			var bounds = gp.GetBounds();
			var m = new Matrix();
			m.Translate(-bounds.X, -bounds.Y);
			gp.Transform(m);

			return gp;
		}

		private void PanelRedPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, CreateCirclePath(), GetBrush(NSurveillanceObjectColor.Red), Pens.Black);
		}

		private void PanelOrangePaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, CreateCirclePath(), GetBrush(NSurveillanceObjectColor.Orange), Pens.Black);
		}

		private void PanelYellowPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, CreateCirclePath(), GetBrush(NSurveillanceObjectColor.Yellow), Pens.Black);
		}

		private void PanelGreenPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, CreateCirclePath(), GetBrush(NSurveillanceObjectColor.Green), Pens.Black);
		}

		private void PanelBluePaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, CreateCirclePath(), GetBrush(NSurveillanceObjectColor.Blue), Pens.Black);
		}

		private void PanelSilverPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, CreateCirclePath(), GetBrush(NSurveillanceObjectColor.Silver), Pens.Black);
		}

		private void PanelWhitePaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, CreateCirclePath(), GetBrush(NSurveillanceObjectColor.White), Pens.Black);
		}

		private void PanelBlackPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, CreateCirclePath(), GetBrush(NSurveillanceObjectColor.Black), Pens.Black);
		}

		private void PanelBrownPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, CreateCirclePath(), GetBrush(NSurveillanceObjectColor.Brown), Pens.Black);
		}

		private void PanelGrayPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, CreateCirclePath(), GetBrush(NSurveillanceObjectColor.Gray), Pens.Black);
		}

		private void PanelRedMouseDown(object sender, MouseEventArgs e)
		{
			CheckColor((Panel)sender, NSurveillanceObjectColor.Red);
		}

		private void PanelOrangeMouseDown(object sender, MouseEventArgs e)
		{
			CheckColor((Panel)sender, NSurveillanceObjectColor.Orange);
		}

		private void PanelYellowMouseDown(object sender, MouseEventArgs e)
		{
			CheckColor((Panel)sender, NSurveillanceObjectColor.Yellow);
		}

		private void PanelGreenMouseDown(object sender, MouseEventArgs e)
		{
			CheckColor((Panel)sender, NSurveillanceObjectColor.Green);
		}

		private void PanelBlueMouseDown(object sender, MouseEventArgs e)
		{
			CheckColor((Panel)sender, NSurveillanceObjectColor.Blue);
		}

		private void PanelSilverMouseDown(object sender, MouseEventArgs e)
		{
			CheckColor((Panel)sender, NSurveillanceObjectColor.Silver);
		}

		private void PanelWhiteMouseDown(object sender, MouseEventArgs e)
		{
			CheckColor((Panel)sender, NSurveillanceObjectColor.White);
		}

		private void PanelBlackMouseDown(object sender, MouseEventArgs e)
		{
			CheckColor((Panel)sender, NSurveillanceObjectColor.Black);
		}

		private void PanelBrownMouseDown(object sender, MouseEventArgs e)
		{
			CheckColor((Panel)sender, NSurveillanceObjectColor.Brown);
		}

		private void PanelGrayMouseDown(object sender, MouseEventArgs e)
		{
			CheckColor((Panel)sender, NSurveillanceObjectColor.Gray);
		}

		private void TrackBarColorConfScroll(object sender, EventArgs e)
		{
			Filter.ColorConfidence = trackBarColorConf.Value / 100f;
			lblColorConfidence.Text = $"Confidence: {Filter.ColorConfidence:0.00}";
		}

		#endregion

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if ((keyData & Keys.Escape) == Keys.Escape)
			{
				DialogResult = DialogResult.Cancel;
				return true;
			}
			return false;
		}

		#region Private static methods

		private static bool IsFlagSet(NSurveillanceObjectColor value, NSurveillanceObjectColor flag)
		{
			return (value & flag) == flag;
		}

		private static bool IsFlagSet(NSurveillanceObjectType value, NSurveillanceObjectType flag)
		{
			return (value & flag) == flag;
		}

		private static bool IsFlagSet(NSurveillanceObjectDirection value, NSurveillanceObjectDirection flag)
		{
			return (value & flag) == flag;
		}

		#endregion
	}
}
