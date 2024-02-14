using Neurotec.Biometrics;
using Neurotec.Samples.Code;
using Neurotec.Surveillance;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Neurotec.Samples.Forms
{
	using System.Collections.Generic;
	using static PathUtils;

	public partial class DetailsView : UserControl
	{
		#region Public types

		[Flags]
		public enum DetailsType
		{
			None = 0,
			Faces = 1,
			Clothing = 2,
			Vehicle = 4,
			LicensePlate = 8,
			Colors = 16,
			Type = 32,
			Directions = 64,
			Matches = 128,
			AgeGroup = 256
		}

		#endregion

		#region Private fields

		private SubjectInfo _info;
		private bool _showTypes = true;
		private bool _showColors = true;
		private bool _showVehicleDetails = true;
		private bool _showClothingDetails = true;
		private bool _showLicensePlateDetails = true;
		private bool _showFaceDetails = true;
		private bool _showDirectionDetails = true;
		private bool _showMatchingDetails = true;
		private bool _showAgeGroups = true;
		private NSurveillanceObjectColorDetails _colorDetails;
		private NSurveillanceObjectTypeDetails _typeDetails;
		private NSurveillanceObjectDirectionDetails _directionDetails;

		private NGender _gender;
		private NGender _clothingGender;
		private bool _withGlasses, _withDarkGlasses, _withBeard, _withMustache, _smile, _blink, _mouthOpen, _withMask;
		private DetailsType _filter;

		#endregion

		#region Public constructor

		public DetailsView()
		{
			InitializeComponent();
			DoubleBuffered = true;
			OnInfoChanged();
		}

		#endregion

		#region Public properties

		public SubjectInfo Info
		{
			get { return _info; }
			set
			{
				_info = value;
				OnInfoChanged();
			}
		}

		public DetailsType Filter
		{
			get { return _filter; }
			set
			{
				if (_filter != value)
				{
					_filter = value;
					OnInfoChanged();
				}
			}
		}

		#endregion

		#region Private static methods

		private static Brush GetSetBrush(bool isSet)
		{
			return isSet ? Brushes.Black : Brushes.Gray;
		}

		private static string GetYesNoString(bool value)
		{
			return value ? "Yes" : "No";
		}

		private static void SetLabel(Label target, Panel icon, string name, float conf, float threshold)
		{
			bool shouldBeBold = conf >= threshold;
			target.Text = $"{name}, confidence {conf * 100:0.00}";
			target.ForeColor = shouldBeBold ? Color.Black : Color.Gray;
			if (shouldBeBold != target.Font.Bold)
			{
				target.Font = new Font(target.Font, shouldBeBold ? FontStyle.Bold : FontStyle.Regular);
				icon?.Invalidate();
			}
		}

		private static Brush GetTypeBrush(float confidence)
		{
			return confidence >= 0.5f ? Brushes.Black : Brushes.Gray;
		}

		#endregion

		#region Private methods

		private void OnInfoChanged()
		{
			if (Info != null)
			{
				UpdateVehicleDetails(Info.Object.VehicleDetails);
				UpdateClothingDetails(Info.Object.ClothingDetails);
				UpdateColorDetails(Info.Object.ColorDetails);
				UpdateAgeGroupDetails(Info.Object.AgeGroupDetails);
				UpdateTypeDetails(Info.Object.DetectionConfidence, Info.Object.TypeDetails);
				UpdateDirectionDetails(Info.Object.DirectionDetails);
				UpdateLicensePlateDetails(Info.LicensePlate);
				UpdateFaceDetails(Info.Face);
				UpdateMatchingDetails(Info.Face);
			}
			else
			{
				tlpVechicleDetailsContainer.Visible = false;
				tlpColorsContainer.Visible = false;
				tlpTypesContainer.Visible = false;
				tlpClothingContainer.Visible = false;
				tlpLicensePlateContainer.Visible = false;
				tlpFaceContainer.Visible = false;
				tlpDirectionsContainer.Visible = false;
				tlpMatchingDetailsContainer.Visible = false;
				tlpAgeGroupsContainer.Visible = false;
			}
			Invalidate();
			Update();
		}

		private void UpdateMatchingDetails(SubjectInfo.FaceDetails face)
		{
			var show = !face.IsEmpty && (Filter & DetailsType.Matches) != DetailsType.Matches;
			tlpMatchingDetailsContainer.Visible = show;
			if (show)
			{
				tlpMatches.SuspendLayout();

				var matches = face.BestMatches ?? new NsedMatchResult[0];
				var currentCount = tlpMatches.Controls.Count;
				var requiredCount = matches.Any() ? matches.Length : 1;
				if (requiredCount > currentCount)
				{
					for (int i = 0; i < requiredCount - currentCount; i++)
					{
						var index = tlpMatches.RowStyles.Add(new RowStyle(SizeType.AutoSize, 100));
						var lbl = new Label { Dock = DockStyle.Fill, AutoSize = true };
						tlpMatches.Controls.Add(lbl, 0, index);
					}
				}
				else if (requiredCount < currentCount)
				{
					for (int i = currentCount - 1; i >= currentCount - (currentCount - requiredCount); i--)
					{
						tlpMatches.Controls.RemoveAt(i);
						tlpMatches.RowStyles.RemoveAt(i);
					}
				}

				for (int i = 0; i < requiredCount; i++)
				{
					var lbl = (Label)tlpMatches.Controls[i];
					var m = matches.Any() ? matches[i] : null;
					lbl.Text = m != null ? $"{m.Id}, Score: {m.Score}" : "No Matches found";
				}
				tlpMatches.ResumeLayout();
			}
		}

		private void UpdateFaceDetails(SubjectInfo.FaceDetails face)
		{
			var show = !face.IsEmpty && (Filter & DetailsType.Faces) != DetailsType.Faces;
			tlpFaceContainer.Visible = show;
			if (show)
			{
				var attributes = face.BestAttributes;

				_gender = attributes.Gender;
				panelGender.Invalidate();

				var glasses = attributes.GetAttributeValue(NBiometricAttributeId.Glasses);
				var darkGlasses = attributes.GetAttributeValue(NBiometricAttributeId.DarkGlasses);
				var beard = attributes.GetAttributeValue(NBiometricAttributeId.Beard);
				var mustache = attributes.GetAttributeValue(NBiometricAttributeId.Mustache);
				var moutOpen = attributes.GetAttributeValue(NBiometricAttributeId.MouthOpen);
				var smile = attributes.GetAttributeValue(NBiometricAttributeId.Smile);
				var faceMask = attributes.GetAttributeValue(NBiometricAttributeId.FaceMask);
				var genderConf = attributes.GetAttributeValue(attributes.Gender == NGender.Male ? NBiometricAttributeId.GenderMale : NBiometricAttributeId.GenderFemale);

				var eyesOpen = attributes.GetAttributeValue(NBiometricAttributeId.EyesOpen);
				var blink = (byte)(eyesOpen <= 100 ? 100 - eyesOpen : eyesOpen);

				bool TestConfidence(byte value)
				{
					return 50 <= value && value <= 100;
				}

				_withGlasses = TestConfidence(glasses);
				_withDarkGlasses = TestConfidence(darkGlasses);
				_withBeard = TestConfidence(beard);
				_withMustache = TestConfidence(mustache);
				_smile = TestConfidence(smile);
				_blink = TestConfidence(blink);
				_mouthOpen = TestConfidence(moutOpen);
				_withMask = TestConfidence(faceMask);

				lblFaceQuality.Text = $"Face Quality: {attributes.Quality}";
				lblAge.Text = $"Age: {attributes.GetAttributeValue(NBiometricAttributeId.Age)}";
				lblGender.Text = $"Gender: {attributes.Gender}, confidence: {genderConf:0.00}";
				lblGlasses.Text = $"Glasses: {GetYesNoString(_withGlasses)}, confidence {glasses:0.00}";
				lblDarkGlasses.Text = $"Dark Glasses: {GetYesNoString(_withDarkGlasses)}, confidence {darkGlasses:0.00}";
				lblBeard.Text = $"Beard: {GetYesNoString(_withBeard)}, confidence {beard:0.00}";
				lblMustache.Text = $"Mustache: {GetYesNoString(_withMustache)}, confidence {mustache:0.00}";
				lblSmile.Text = $"Smile: {GetYesNoString(_smile)}, confidence: {smile:0.00}";
				lblBlink.Text = $"Blink: {GetYesNoString(_blink)}, confidence {blink:0.00}";
				lblMoutOpen.Text = $"Mouth Open: {GetYesNoString(_mouthOpen)}, confidence {moutOpen:0.00}";

				var maskConfidenceString = faceMask == NBiometricTypes.QualityUnknown ? "Not Detected" : $"{faceMask:0.00}";
				lblMask.Text = $"Mask: {GetYesNoString(_withMask)} confidence: {maskConfidenceString}";

				lblEthWhite.Text = $"White: {attributes.GetAttributeValue(NBiometricAttributeId.EthnicityWhite):0.00}";
				lblEthBlack.Text = $"Black: {attributes.GetAttributeValue(NBiometricAttributeId.EthnicityBlack):0.00}";
				lblEthHispanic.Text = $"Hispanic: {attributes.GetAttributeValue(NBiometricAttributeId.EthnicityHispanic):0.00}";
				lblEthAsian.Text = $"Asian: {attributes.GetAttributeValue(NBiometricAttributeId.EthnicityAsian):0.00}";
				lblEthIndian.Text = $"Indian: {attributes.GetAttributeValue(NBiometricAttributeId.EthnicityIndian):0.00}";
				lblEthArabian.Text = $"Arabian: {attributes.GetAttributeValue(NBiometricAttributeId.EthnicityArabian):0.00}";
				lblNeutral.Text = $"Neutral: {attributes.GetAttributeValue(NBiometricAttributeId.EmotionNeutral):0.00}";
				lblAnger.Text = $"Anger: {attributes.GetAttributeValue(NBiometricAttributeId.EmotionAnger):0.00}";
				lblContempt.Text = $"Contempt: {attributes.GetAttributeValue(NBiometricAttributeId.EmotionContempt):0.00}";
				lblDisgust.Text = $"Disgust: {attributes.GetAttributeValue(NBiometricAttributeId.EmotionDisgust):0.00}";
				lblFear.Text = $"Fear: {attributes.GetAttributeValue(NBiometricAttributeId.EmotionFear):0.00}";
				lblHapiness.Text = $"Happiness: {attributes.GetAttributeValue(NBiometricAttributeId.EmotionHappiness):0.00}";
				lblSadness.Text = $"Sadness: {attributes.GetAttributeValue(NBiometricAttributeId.EmotionSadness):0.00}";
				lblSurprise.Text = $"Surprise: {attributes.GetAttributeValue(NBiometricAttributeId.EmotionSurprise):0.00}";

				tlpFaceContainer.Invalidate();
			}
		}

		private void UpdateLicensePlateDetails(SubjectInfo.LicensePlateDetails licensePlateDetails)
		{
			var show = licensePlateDetails.IsEmpty == false && (Filter & DetailsType.LicensePlate) != DetailsType.LicensePlate;
			if (show)
			{
				var details = licensePlateDetails.Best;

				tlpLicensePlateContainer.Visible = true;
				lblLicensePlate.Text = $"License Plate: {details.Value}";
				lblLicensePlateFormatted.Text = $"License Plate Formatted: {details.FormattedValue}";
				lblOrigin.Text = $"Origin: {details.Origin}";
				lblLpDetectionConf.Text = $"Detection Confidence: {details.DetectionConfidence * 100:0.00}";
				lblOcrConf.Text = $"OCR Confidence: {details.OcrConfidence * 100:0.00}";
				lblLpCharacterHeight.Text = $"Character Height: {details.CharacterHeight:0.00} pixels";
				lblLpType.Text = $"License Plate Type: {details.Type}";
				lblLpType.Visible = details.Type != "";
				lblLpRegion.Text = $"License Plate Region: {details.Region}";
				lblLpRegion.Visible = !string.IsNullOrEmpty(details.Region);
				if (licensePlateDetails.Watchlist != null)
				{
					lblWatchlist.Visible = true;
					lblWatchlist.Text = string.IsNullOrEmpty(licensePlateDetails.Watchlist.Owner) ? "In Watchlist" :
						$"In Watchlist: {licensePlateDetails.Watchlist.Owner}";
				}
				else
				{
					lblWatchlist.Visible = false;
				}
			}
			else
			{
				tlpLicensePlateContainer.Visible = false;
			}
		}

		private void UpdateVehicleDetails(NVehicleDetails details)
		{
			bool show = details != null && (Filter & DetailsType.Vehicle) != DetailsType.Vehicle;
			tlpVechicleDetailsContainer.Visible = show;
			if (show)
			{
				IEnumerable<string> GetModelLabels()
				{
					foreach (var m in details.Models)
					{
						var makeModels = m.MakeModels;
						var first = true;
						foreach (var pair in makeModels)
						{
							if (first)
							{
								first = false;
								yield return $"{pair.Key} ({m.Confidence * 100:0.00}) - {pair.Value}";
							}
							else
								yield return $"   {pair.Key} - {pair.Value}";
						}
					}
				}

				IEnumerable<string> GetTagsLabels()
				{
					foreach (var tag in details.Tags)
					{
						yield return $"{tag.Name} ({tag.Confidence * 100:0.00})";
					}
				}

				string GetOrientationLabel() => $"{details.Orientation} - {details.OrientationAngle:0.00}? ({details.OrientationConfidence * 100:0.00})";

				tlpVehicleDetails.SuspendLayout();
				var modelCount = details.Models.Select(x => x.MakeModels.Count).Sum();
				var requiredCount = modelCount + details.Tags.Count + 1 + 3;
				if (tlpVehicleDetails.Controls.Count == requiredCount && // Number of labels have not changed
					tlpVehicleDetails.Controls.IndexOf(lblTags) == (modelCount + 1)) // Model count has not changed
				{
					var index = 1;
					Label lbl = null;
					foreach (var text in GetModelLabels())
					{
						lbl = (Label)tlpVehicleDetails.Controls[index++];
						lbl.Text = text;
					}

					index++;
					foreach (var text in GetTagsLabels())
					{
						lbl = (Label)tlpVehicleDetails.Controls[index++];
						lbl.Text = text;
					}

					index++;
					lbl = (Label)tlpVehicleDetails.Controls[index];
					lbl.Text = GetOrientationLabel();
				}
				else
				{
					int index = 1;
					var controls = tlpVehicleDetails.Controls
						.Cast<Label>()
						.Where(x => x != lblOrientation && x != lblTags && x != lblModels)
						.ToList();
					tlpVehicleDetails.Controls.Clear();
					tlpVehicleDetails.RowStyles.Clear();
					controls.ForEach(x => x.Dispose());

					void InsertLabel(string text)
					{
						var lbl = new Label { Text = text, Dock = DockStyle.Fill, AutoSize = true };
						tlpVehicleDetails.RowStyles.Insert(index, new RowStyle(SizeType.AutoSize, 0));
						tlpVehicleDetails.Controls.Add(lbl, 0, index++);
					}

					tlpVehicleDetails.RowStyles.Add(new RowStyle(SizeType.AutoSize, 0));
					tlpVehicleDetails.Controls.Add(lblModels, 0, 0);

					foreach (var text in GetModelLabels())
					{
						InsertLabel(text);
					}

					tlpVehicleDetails.RowStyles.Add(new RowStyle(SizeType.AutoSize, 0));
					tlpVehicleDetails.Controls.Add(lblTags, 0, index++);
					foreach (var tag in GetTagsLabels())
					{
						InsertLabel(tag);
					}

					tlpVehicleDetails.RowStyles.Add(new RowStyle(SizeType.AutoSize, 0));
					tlpVehicleDetails.Controls.Add(lblOrientation, 0, index++);
					InsertLabel(GetOrientationLabel());
				}

				tlpVehicleDetails.ResumeLayout();
				tlpVehicleDetails.PerformLayout();
			}
		}

		private void UpdateDirectionDetails(NSurveillanceObjectDirectionDetails details)
		{
			var show = !Info.Object.IsEmpty &&
				(Filter & DetailsType.Directions) != DetailsType.Directions;

			if (show)
			{
				_directionDetails = details;
				SetLabel(lblNorth, panelNorth, "North", _directionDetails.NorthConfidence, .3f);
				SetLabel(lblNorthEast, panelNorthEast, "North East", _directionDetails.NorthEastConfidence, .3f);
				SetLabel(lblEast, panelEast, "East", _directionDetails.EastConfidence, .3f);
				SetLabel(lblSouthEast, panelSouthEast, "South East", _directionDetails.SouthEastConfidence, .3f);
				SetLabel(lblSouth, panelSouth, "South", _directionDetails.SouthConfidence, .3f);
				SetLabel(lblSouthWest, panelSouthWest, "South West", _directionDetails.SouthWestConfidence, .3f);
				SetLabel(lblWest, panelWest, "West", _directionDetails.WestConfidence, .3f);
				SetLabel(lblNorthWest, panelNorthWest, "North West", _directionDetails.NorthWestConfidence, .3f);

				tlpDirectionsContainer.Visible = true;
				tlpDirectionsContainer.Invalidate();
			}
			else
			{
				tlpDirectionsContainer.Visible = false;
			}
		}

		private void UpdateTypeDetails(float conf, NSurveillanceObjectTypeDetails details)
		{
			var show = !Info.Object.IsEmpty &&
				(Filter & DetailsType.Type) != DetailsType.Type;
			if (show)
			{
				_typeDetails = details;
				SetLabel(lblCar, panelCar, "Car", _typeDetails.CarConfidence, .5f);
				SetLabel(lblTruck, panelTruck, "Truck", _typeDetails.TruckConfidence, .5f);
				SetLabel(lblBus, panelBus, "Bus", _typeDetails.BusConfidence, .5f);
				SetLabel(lblBike, panelBike, "Bike", _typeDetails.BikeConfidence, .5f);
				SetLabel(lblPerson, panelPerson, "Person", _typeDetails.PersonConfidence, .5f);
				lblDetectionConf.Text = $"Detection Confidence: {conf * 100:0.00}";

				tlpTypesContainer.Visible = true;
				tlpTypesContainer.Invalidate();
			}
			else
			{
				tlpTypesContainer.Visible = false;
			}
		}

		private void UpdateColorDetails(NSurveillanceObjectColorDetails details)
		{
			var show = !Info.Object.IsEmpty &&
				(Filter & DetailsType.Colors) != DetailsType.Colors;
			if (show)
			{
				_colorDetails = details;
				SetLabel(lblRed, panelRed, "Red", details.RedColorConfidence, .5f);
				SetLabel(lblOrange, panelOrange, "Orange", details.OrangeColorConfidence, .5f);
				SetLabel(lblYellow, panelYellow, "Yellow", details.YellowColorConfidence, .5f);
				SetLabel(lblGreen, panelGreen, "Green", details.GreenColorConfidence, .5f);
				SetLabel(lblBlue, panelBlue, "Blue", details.BlueColorConfidence, .5f);
				SetLabel(lblSilver, panelSilver, "Silver", details.SilverColorConfidence, .5f);
				SetLabel(lblWhite, panelWhite, "White", details.WhiteColorConfidence, .5f);
				SetLabel(lblBlack, panelBlack, "Black", details.BlackColorConfidence, .5f);
				SetLabel(lblBrown, panelBrown, "Brown", details.BrownColorConfidence, .5f);
				SetLabel(lblGray, panelGray, "Gray", details.GrayColorConfidence, .5f);
				tlpColorsContainer.Visible = true;
				tlpColorsContainer.Invalidate();
			}
			else
			{
				tlpColorsContainer.Visible = false;
			}
		}

		private void UpdateClothingDetails(NClothingDetails details)
		{
			var show = details != null && (Filter & DetailsType.Clothing) != DetailsType.Clothing;
			if (show)
			{
				tlpClothingContainer.Visible = details != null;
				if (details != null)
				{
					IEnumerable<string> GetLabels()
					{
						yield return $"Gender: {details.Gender} ({details.GenderConfidence:0.00})";
						yield return $"Headwear: {details.Headwear.Name} ({details.Headwear.Confidence * 100:0.00})";
						yield return $"Torso: {details.Torso.Name} ({details.Torso.Confidence * 100:0.00})";
						yield return $"Arms: {details.Arms.Name} ({details.Arms.Confidence * 100:0.00})";
						yield return $"Legs: {details.Legs.Name} ({details.Legs.Confidence * 100:0.00})";
						yield return $"Feet: {details.Feet.Name} ({details.Feet.Confidence * 100:0.00})";

						foreach (var c in details.Values)
						{
							yield return $"{c.Name} ({c.Confidence * 100:0.00})";
						}
					}

					tlpClothing.SuspendLayout();
					_clothingGender = details.Gender;
					panelClothingGender.Invalidate();
					int index = 0;
					if (tlpClothing.Controls.Count == 1 + details.Values.Count + 5)
					{
						foreach (var text in GetLabels())
						{
							var lbl = (Label)tlpClothing.Controls[index++];
							lbl.Text = text;
						}
					}
					else
					{
						var controls = tlpClothing.Controls
							.Cast<Label>()
							.ToList();
						tlpClothing.Controls.Clear();
						tlpClothing.RowStyles.Clear();
						controls.ForEach(x => x.Dispose());

						foreach (var text in GetLabels())
						{
							var lbl = new Label { Text = text, Dock = DockStyle.Fill, AutoSize = true };
							tlpClothing.RowStyles.Insert(index, new RowStyle(SizeType.AutoSize, 0));
							tlpClothing.Controls.Add(lbl, 0, index++);
						}
					}
					tlpClothing.ResumeLayout();
					tlpClothing.PerformLayout();
				}
			}
			else
			{
				tlpClothingContainer.Visible = false;
			}
		}

		private void UpdateAgeGroupDetails(NAgeGroupDetails details)
		{
			var show = details != null && (Filter & DetailsType.AgeGroup) != DetailsType.AgeGroup;
			if (show)
			{
				tlpAgeGroupsContainer.Visible = details != null;
				if (details != null)
				{
					lblAgeGroup.Text = $"Age group: {details.Group} ({details.GroupConfidence * 100:0.00})";
					lblAdult.Text = $"Adult: {details.AdultConfidence * 100:0.00}";
					lblChild.Text = $"Child: {details.ChildConfidence * 100:0.00}";
					lblSenior.Text = $"Senior: {details.SeniorConfidence * 100:0.00}";
					lblTeenager.Text = $"Teenager: {details.TeenagerConfidence * 100:0.00}";
				}
			}
			else
			{
				tlpAgeGroupsContainer.Visible = false;
			}
		}

		private void ToggleView(ref bool show, params Control[] views)
		{
			show = !show;
			if (views != null)
			{
				foreach (var view in views)
				{
					view.Visible = show;
				}
			}
			PerformLayout();
		}

		private void TsbShowAgeGroupsClick(object sender, EventArgs e)
		{
			ToggleView(ref _showAgeGroups, tlpAgeGroups);
		}

		private void TsbShowTypeDetailsClick(object sender, EventArgs e)
		{
			ToggleView(ref _showTypes, tlpTypes);
		}

		private void TsbColorDetailsClick(object sender, EventArgs e)
		{
			ToggleView(ref _showColors, tlpColors);
		}

		private void TsbShowVehicleDetailsClick(object sender, EventArgs e)
		{
			ToggleView(ref _showVehicleDetails, tlpVehicleDetails);
		}

		private void TsbClothingDetailsClick(object sender, EventArgs e)
		{
			ToggleView(ref _showClothingDetails, tlpClothing, panelClothingGender);
		}

		private void TsbLicensePlateClick(object sender, EventArgs e)
		{
			ToggleView(ref _showLicensePlateDetails, tlpLicensePlate);
		}

		private void TsbFaceDetailsClick(object sender, EventArgs e)
		{
			ToggleView(ref _showFaceDetails, tlpFaceDetails);
		}

		private void TsbShowDirectionsClick(object sender, EventArgs e)
		{
			ToggleView(ref _showDirectionDetails, tlpDirectionDetails);
		}

		private void TsbShowMatchingDetailsClick(object sender, EventArgs e)
		{
			ToggleView(ref _showMatchingDetails, tlpMatches);
		}

		private void PanelRedPaint(object sender, PaintEventArgs e)
		{
			var control = (Control)sender;
			using (var brush = GetColorBrush(control, _colorDetails, NSurveillanceObjectColor.Red))
			{
				PaintAttribute(e.Graphics, control, CreateCirclePath(), brush, Pens.Black);
			}
		}

		private void PanelOrangePaint(object sender, PaintEventArgs e)
		{
			var control = (Control)sender;
			using (var brush = GetColorBrush(control, _colorDetails, NSurveillanceObjectColor.Orange))
			{
				PaintAttribute(e.Graphics, control, CreateCirclePath(), brush, Pens.Black);
			}
		}

		private void PanelYellowPaint(object sender, PaintEventArgs e)
		{
			var control = (Control)sender;
			using (var brush = GetColorBrush(control, _colorDetails, NSurveillanceObjectColor.Yellow))
			{
				PaintAttribute(e.Graphics, control, CreateCirclePath(), brush, Pens.Black);
			}
		}

		private void PanelGreenPaint(object sender, PaintEventArgs e)
		{
			var control = (Control)sender;
			using (var brush = GetColorBrush(control, _colorDetails, NSurveillanceObjectColor.Green))
			{
				PaintAttribute(e.Graphics, control, CreateCirclePath(), brush, Pens.Black);
			}
		}

		private void PanelBluePaint(object sender, PaintEventArgs e)
		{
			var control = (Control)sender;
			using (var brush = GetColorBrush(control, _colorDetails, NSurveillanceObjectColor.Blue))
			{
				PaintAttribute(e.Graphics, control, CreateCirclePath(), brush, Pens.Black);
			}
		}

		private void PanelSilverPaint(object sender, PaintEventArgs e)
		{
			var control = (Control)sender;
			using (var brush = GetColorBrush(control, _colorDetails, NSurveillanceObjectColor.Silver))
			{
				PaintAttribute(e.Graphics, control, CreateCirclePath(), brush, Pens.Black);
			}
		}

		private void PanelWhitePaint(object sender, PaintEventArgs e)
		{
			var control = (Control)sender;
			using (var brush = GetColorBrush(control, _colorDetails, NSurveillanceObjectColor.White))
			{
				PaintAttribute(e.Graphics, control, CreateCirclePath(), brush, Pens.Black);
			}
		}

		private void PanelBlackPaint(object sender, PaintEventArgs e)
		{
			var control = (Control)sender;
			using (var brush = GetColorBrush(control, _colorDetails, NSurveillanceObjectColor.Black))
			{
				PaintAttribute(e.Graphics, control, CreateCirclePath(), brush, Pens.Black);
			}
		}

		private void PanelBrownPaint(object sender, PaintEventArgs e)
		{
			var control = (Control)sender;
			using (var brush = GetColorBrush(control, _colorDetails, NSurveillanceObjectColor.Brown))
			{
				PaintAttribute(e.Graphics, control, CreateCirclePath(), brush, Pens.Black);
			}
		}

		private void PanelGrayPaint(object sender, PaintEventArgs e)
		{
			var control = (Control)sender;
			using (var brush = GetColorBrush(control, _colorDetails, NSurveillanceObjectColor.Gray))
			{
				PaintAttribute(e.Graphics, control, CreateCirclePath(), brush, Pens.Black);
			}
		}

		private void PanelCarPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, CreateCarPath(), GetTypeBrush(_typeDetails.CarConfidence));
		}

		private void PanelTruckPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, CreateTruckPath(), GetTypeBrush(_typeDetails.TruckConfidence));
		}

		private void PanelBusPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, CreateBusPath(), GetTypeBrush(_typeDetails.BusConfidence));
		}

		private void PanelBikePaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, CreateBikePath(), GetTypeBrush(_typeDetails.BikeConfidence));
		}

		private void PanelPersonPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, (Control)sender, CreatePersonPath(), GetTypeBrush(_typeDetails.PersonConfidence));
		}

		private void PanelGenderPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelGender, _gender == NGender.Male ? CreateMalePath() : CreateFemalePath(), Brushes.Black);
		}
		private void PanelClothingGenderPaint(object sender, PaintEventArgs e)
		{
			if (_clothingGender != NGender.Unknown)
			{
				panelClothingGender.Visible = true;
				PaintAttribute(e.Graphics, panelClothingGender, _clothingGender == NGender.Male ? CreateMalePath() : CreateFemalePath(), Brushes.Black);
			}
			else
			{
				panelClothingGender.Visible = false;
			}
		}

		private void PanelGlassesPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelGlasses, CreateGlassesPath(), GetSetBrush(_withGlasses));
		}

		private void PanelDarkGlassesPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelGlasses, CreateDarkGlassesPath(), GetSetBrush(_withDarkGlasses));
		}

		private void PanelNorthPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelNorth, CreateNorthPath(), GetSetBrush(_directionDetails.NorthConfidence > .3f));
		}

		private void PanelNorthEastPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelNorthEast, CreateNorthEastPath(), GetSetBrush(_directionDetails.NorthEastConfidence > .3f));
		}

		private void PanelEastPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelEast, CreateEastPath(), GetSetBrush(_directionDetails.EastConfidence > .3f));
		}

		private void PanelSouthEastPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelSouthEast, CreateSouthEastPath(), GetSetBrush(_directionDetails.SouthEastConfidence > .3f));
		}

		private void PanelSouthPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelSouth, CreateSouthPath(), GetSetBrush(_directionDetails.SouthConfidence > .3f));
		}

		private void PanelSouthWestPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelSouthWest, CreateSouthWestPath(), GetSetBrush(_directionDetails.SouthWestConfidence > .3f));
		}

		private void PanelWestPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelWest, CreateWestPath(), GetSetBrush(_directionDetails.WestConfidence > .3f));
		}

		private void PanelNorthWestPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelNorthWest, CreateNorthWestPath(), GetSetBrush(_directionDetails.NorthWestConfidence > .3f));
		}

		private void PanelBeardPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelBeard, CreateBeardPath(), GetSetBrush(_withBeard));
		}

		private void PanelMustachePaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelMustache, CreateMustachePath(), GetSetBrush(_withMustache));
		}

		private void PanelSmilePaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelSmile, CreateSmilePath(), GetSetBrush(_smile));
		}

		private void PanelBlinkPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelBlink, CreateBlinkPath(), GetSetBrush(_blink));
		}

		private void PanelMouthOpenPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelMouthOpen, CreateMouthOpenPath(), GetSetBrush(_mouthOpen));
		}

		private void PanelMaskPaint(object sender, PaintEventArgs e)
		{
			PaintAttribute(e.Graphics, panelMask, CreateMaskPath(), GetSetBrush(_withMask));
		}

		private void DetailsViewPaint(object sender, PaintEventArgs e)
		{
			if (Info == null)
			{
				var g = e.Graphics;
				g.DrawLine(Pens.Black, 0, 0, 0, ClientRectangle.Height);

				var text = "Select subject to view details";
				using (var f = new Font(Font, FontStyle.Bold))
				{
					var sz = g.MeasureString(text, f);
					g.DrawString(text, f, Brushes.Black, (ClientRectangle.Width - sz.Width) / 2, (ClientRectangle.Height - sz.Height) / 2);
				}
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			Invalidate();
		}

		#endregion
	}
}
