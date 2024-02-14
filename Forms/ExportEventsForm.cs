using Neurotec.Biometrics;
using Neurotec.Samples.Code;
using Neurotec.Surveillance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neurotec.Samples.Forms
{
	public enum SubjectType
	{
		[Description("Any")]
		Any,
		[Description("Vehicles And Humans")]
		VehiclesAndHumans,
		[Description("ALPR")]
		ALPR,
		[Description("Vehicles With ALPR")]
		VehiclesWithALPR,
		[Description("Faces")]
		Faces,
		[Description("Humans With Faces")]
		HumansWithFaces
	}
	public partial class ExportEventsForm : Form
	{
		#region Private readonly fields
		private readonly int _itemWidth = 800;
		private readonly int _itemHeight = 300;
		private readonly int _lineHeight = 20;
		private FaceRecordCollection _matchFaceRecords { get; set; }
		#endregion

		#region Private fields

		private int _lastId;
		private Font _font = new Font("Arial", 12, FontStyle.Bold);

		#endregion

		#region Public properties
		public RecordCollection RecordCollection { get; set; }
		#endregion

		#region Public constructor
		public ExportEventsForm()
		{
			InitializeComponent();
			fromDateTimePicker.Value = DateTime.Now.Subtract(TimeSpan.FromDays(7));
			orderCmbBox.DataSource = Enum.GetValues(typeof(Order)).Cast<Order>().ToArray();

			typeCmbBox.DisplayMember = "Description";
			typeCmbBox.ValueMember = "value";
			typeCmbBox.DataSource = Enum.GetValues(typeof(SubjectType)).Cast<SubjectType>().Select(value => new
			{
				(Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute).Description,
				value
			}).OrderBy(item => item.value).ToArray();

			classCmbBox.DataSource = new string[] { "Any", "Bike", "Bus", "Car", "Person", "Truck" };
			classCmbBox.SelectedItem = "Any";

			var dir = Utils.GetUserLocalDataDir("SurveillanceSample");
			var dbFileFaces = Path.Combine(dir, SurveillanceConfig.DBFileFaceWatchlist);
			_matchFaceRecords = new FaceRecordCollection(dbFileFaces);
		}
		#endregion

		#region Graphics

		private void SetupGraphics(Graphics g)
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
			g.InterpolationMode = InterpolationMode.High;
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.Clear(Color.White);
		}

		private void DrawColors(Graphics g, ObjectRecord record, ref float offsetX, ref float offsetY)
		{
			var colors = new Dictionary<Color, float>()
			{
				{ Color.Red, record.RedColorConfidence },
				{ Color.Yellow, record.YellowColorConfidence },
				{ Color.White, record.WhiteColorConfidence },
				{ Color.Silver, record.SilverColorConfidence },
				{ Color.Orange, record.OrangeColorConfidence },
				{ Color.Green, record.GreenColorConfidence },
				{ Color.Gray, record.GrayColorConfidence },
				{ Color.Brown, record.BrownColorConfidence },
				{ Color.Blue, record.BlueColorConfidence },
				{ Color.Black, record.BlackColorConfidence },
			};

			int count = 0;
			float relativeX = 0;
			foreach (var color in colors.Where(c => c.Value >= 0.001).OrderByDescending(c => c.Value))
			{
				if (count == 5)
				{
					count = 0;
					relativeX = 0;
					offsetY += _lineHeight;
				}
				DrawColor(g, color.Key, color.Value, offsetX + relativeX, offsetY);
				count++;
				relativeX += 75;
			}
			if (count > 0)
				offsetY += _lineHeight;
		}

		private void DrawClasses(Graphics g, ObjectRecord record, ref float offsetX, ref float offsetY)
		{
			var objectClasses = new Dictionary<GraphicsPath, float>()
			{
				{ PathUtils.CreateBikePath(), record.BikeConfidence},
				{ PathUtils.CreateBusPath(), record.BusConfidence},
				{ PathUtils.CreateCarPath(), record.CarConfidence},
				{ PathUtils.CreatePersonPath(), record.PersonConfidence},
				{ PathUtils.CreateTruckPath(), record.TruckConfidence},
			};

			float relativeX = 0;
			foreach (var objClass in objectClasses.Where(x => x.Value >= 0.001).OrderByDescending(x => x.Value))
			{
				DrawIcon(g, objClass.Value, objClass.Key, GetTypeBrush(objClass.Value), offsetX + relativeX, offsetY);
				relativeX += 75;
			}
			offsetY += _lineHeight;
		}

		private void DrawModels(Graphics g, ObjectRecord record, ref float offsetX, ref float offsetY)
		{
			if (record.VehicleMake?.Length > 0)
			{
				g.DrawString("Make: " + record.VehicleMake + " " + record.VehicleMakeConfidence.ToString("0.0%"), _font, GetDirectionBrush((float)record.VehicleMakeConfidence), offsetX, offsetY);
				offsetY += _lineHeight;
			}
			if (record.CarModel?.Length > 0)
			{
				g.DrawString("Model: " + record.CarModel, _font, GetDirectionBrush((float)record.VehicleMakeConfidence), offsetX, offsetY);
				offsetY += _lineHeight;
			}
		}

		private void DrawClothes(Graphics g, ObjectRecord record, ref float offsetX, ref float offsetY)
		{
			if (record.Clothes?.Count > 0)
			{
				foreach (var rec in record.Clothes)
				{
					g.DrawString(rec.Key + rec.Value.ToString(": 0.0%"), _font, GetDirectionBrush((float)rec.Value), offsetX, offsetY);
					offsetY += _lineHeight;
				}
			}
		}

		private void DrawAgeGroup(Graphics g, ObjectRecord record, ref float offsetX, ref float offsetY)
		{
			if (record.AgeGroup != NAgeGroup.Unknown)
			{
				g.DrawString($"{record.AgeGroup}: {record.AgeGroupConfidence:0.0%}", _font, Brushes.Black, offsetX, offsetY);
				offsetY += _lineHeight;
			}
		}

		private void DrawIcon(Graphics g, float confidence, GraphicsPath path, Brush brush, float x, float y)
		{
			DrawGraphicsPath(g, path, x, y, brush);
			g.DrawString(confidence.ToString("0.0%"), _font, brush, x + 18, y);
		}

		private void DrawIcon(Graphics g, bool with, float confidence, string attribute, GraphicsPath path, Brush brush, float x, float y, int fontSize)
		{
			confidence = confidence / 100;

			if (with)
			{
				DrawGraphicsPath(g, path, x, y, Brushes.Black);
				g.DrawString(attribute + "Yes, " + confidence.ToString(" 0.0%"), _font, Brushes.Black, x + 18, y);
			}
			else
			{
				DrawGraphicsPath(g, path, x, y, brush);
				g.DrawString(attribute + "No, " + confidence.ToString(" 0.0%"), _font, Brushes.Black, x + 18, y);
			}
		}

		private void DrawGraphicsPath(Graphics g, GraphicsPath path, float x, float y, Brush brush, Pen pen = null)
		{
			var bounds = path.GetBounds();
			var scaleY = 18 / bounds.Height;
			var scaleX = 18 / bounds.Width;
			var scale = Math.Min(scaleX, scaleY);
			var m = new Matrix();
			m.Translate(x, y);
			m.Scale(scale, scale);
			path.Transform(m);

			g.FillPath(brush, path);
			if (pen != null)
				g.DrawPath(pen, path);
		}

		private void DrawObjectInfo(Graphics g, Record record, float offsetX, float offsetY)
		{
			offsetY += (_itemHeight - 100) / 2;
			offsetY = record.Object.CarModel == null ? offsetY : offsetY - 20;
			offsetY = record.Object.Clothes == null ? offsetY : offsetY - 10 * record.Object.Clothes.Count;
			offsetY = record.Object.HasColor ? offsetY - 10 : offsetY;
			offsetY = record.Object.HasDirection ? offsetY - 10 : offsetY;
			g.DrawString(record.Timestamp.ToString(), _font, Brushes.Black, offsetX, offsetY);
			offsetY += _lineHeight;
			RectangleF rec = new RectangleF(new PointF(offsetX, offsetY), new Size(_itemWidth - _itemHeight, _lineHeight));
			g.DrawString(record.Source, _font, Brushes.Black, rec);
			offsetY += _lineHeight;
			DrawColors(g, record.Object, ref offsetX, ref offsetY);
			DrawClasses(g, record.Object, ref offsetX, ref offsetY);
			DrawAgeGroup(g, record.Object, ref offsetX, ref offsetY);
			DrawDirections(g, record.Object, ref offsetX, ref offsetY);
			DrawModels(g, record.Object, ref offsetX, ref offsetY);
			DrawClothes(g, record.Object, ref offsetX, ref offsetY);
		}

		private void DrawLicenseInfo(Graphics g, Record record, float offsetX, float offsetY, bool withObject = false)
		{
			string GetOrigin(string origin) => !string.IsNullOrEmpty(origin) ? origin : "Unknown";
			if (!withObject)
			{
				offsetY += (_itemHeight - 120) / 2;
				g.DrawString($"{record.Timestamp.ToString()}", _font, Brushes.Black, offsetX, offsetY);
				offsetY += _lineHeight;
				RectangleF rec = new RectangleF(new PointF(offsetX, offsetY), new Size(_itemWidth - _itemHeight, _lineHeight));
				g.DrawString($"{record.Source}", _font, Brushes.Black, rec);
				offsetY += _lineHeight;
			}
			else
			{
				offsetY += (_itemHeight - 80) / 2;
			}
			g.DrawString($"Detection confidence: {record.License.DetectionConfidence.ToString("0.0%")}", _font, Brushes.Black, offsetX, offsetY);
			offsetY += _lineHeight;
			g.DrawString($"Value: {record.License.Value}", _font, Brushes.Black, offsetX, offsetY);
			offsetY += _lineHeight;
			g.DrawString($"Origin: {GetOrigin(record.License.Origin)}", _font, Brushes.Black, offsetX, offsetY);
			offsetY += _lineHeight;
			g.DrawString($"OCR confidence: {record.License.OcrConfidence.ToString("0.0%")}", _font, Brushes.Black, offsetX, offsetY);
		}

		private void DrawFaceInfo(Graphics g, Record record, float offsetX, float offsetY, bool withObject = false)
		{
			if (!withObject)
			{
				offsetY += (_itemHeight - 250) / 2;
				g.DrawString($"{record.Timestamp.ToString()}", _font, Brushes.Black, offsetX, offsetY);
				offsetY += _lineHeight;
				RectangleF rect = new RectangleF(new PointF(offsetX, offsetY), new Size(_itemWidth - _itemHeight, _lineHeight));
				g.DrawString($"{record.Source}", _font, Brushes.Black, rect);
				offsetY += _lineHeight;
			}
			else
			{
				offsetY += (_itemHeight - 220) / 2;
			}

			DrawGenderAndAge(g, record, offsetX, ref offsetY);
			DrawAttributes(g, record.Face, offsetX, ref offsetY, 12);

			if (record.Face.Match != null)
			{
				var matchOffsetY = withObject ? offsetY + 16 : offsetY - 5;
				var text = $"{record.Face.Match.Id}, Score: {record.Face.Match.Score}";
				g.DrawString(text, _font, Brushes.Black, offsetX, matchOffsetY - 20);
			}
		}

		private void DrawGenderAndAge(Graphics g, Record record, float offsetX, ref float offsetY)
		{
			var gender = record.Face.Gender;
			var rect = new RectangleF(new PointF(offsetX, offsetY), new Size(_itemWidth - _itemHeight, _lineHeight));
			var genderConfidence = record.Face.GetAttributeValue(gender == NGender.Male ? NBiometricAttributeId.GenderMale : NBiometricAttributeId.GenderFemale);
			if (gender == NGender.Male)
			{
				DrawIcon(g, genderConfidence / 100f, PathUtils.CreateMalePath(), Brushes.Black, offsetX, offsetY);
				offsetX += 115;
			}
			else if (record.Face.Gender == Biometrics.NGender.Female)
			{
				DrawIcon(g, genderConfidence / 100f, PathUtils.CreateFemalePath(), Brushes.Black, offsetX, offsetY);
				offsetX += 135;
			}
			var age = record.Face.GetAttributeValue(NBiometricAttributeId.Age);
			if (age <= NBiometricTypes.QualityMax)
			{
				var ageString = $"Age: {age}";
				g.DrawString(ageString, _font, Brushes.Black, offsetX, offsetY);
			}
			offsetY += _lineHeight;
		}

		private void DrawAttributes(Graphics g, FullFaceRecord events, float offsetX, ref float offsetY, int fontSize)
		{
			float add;

			if (fontSize == 11)
				add = _lineHeight - 3;
			else
				add = _lineHeight;

			var offsetYIntenrnal = offsetY;
			void CheckAndDraw(NBiometricAttributeId id, string prefix, GraphicsPath path, bool invert = false)
			{
				var conf = events.GetAttributeValue(id);
				if (conf <= 100)
				{
					if (invert) conf = (byte)(100 - conf);
					DrawIcon(g, conf >= 50, conf, prefix, path, Brushes.Gray, offsetX, offsetYIntenrnal, fontSize);
					offsetYIntenrnal += add;
				}
			}

			CheckAndDraw(NBiometricAttributeId.Glasses, "Glasses: ", PathUtils.CreateGlassesPath());
			CheckAndDraw(NBiometricAttributeId.DarkGlasses, "Dark Glasses: ", PathUtils.CreateDarkGlassesPath());
			CheckAndDraw(NBiometricAttributeId.Mustache, "Mustache: ", PathUtils.CreateMustachePath());
			CheckAndDraw(NBiometricAttributeId.Beard, "Beard: ", PathUtils.CreateBeardPath());
			CheckAndDraw(NBiometricAttributeId.EyesOpen, "Blink: ", PathUtils.CreateBlinkPath(), invert: true);
			CheckAndDraw(NBiometricAttributeId.MouthOpen, "Mouth Open: ", PathUtils.CreateMouthOpenPath());
			CheckAndDraw(NBiometricAttributeId.FaceMask, "Mask: ", PathUtils.CreateMaskPath());
			CheckAndDraw(NBiometricAttributeId.Smile, "Smile: ", PathUtils.CreateSmilePath());
			DrawEmotionAndEthnicity(g, events, ref offsetX, ref offsetYIntenrnal);

			offsetY = offsetYIntenrnal;
			offsetY += add;
		}

		private void DrawEmotionAndEthnicity(Graphics g, FullFaceRecord record, ref float offsetX, ref float offsetY)
		{
			var emotions = new Dictionary<string, float>()
			{
				{ "Neutral", record.GetAttributeValue(NBiometricAttributeId.EmotionNeutral) },
				{ "Anger", record.GetAttributeValue(NBiometricAttributeId.EmotionAnger)},
				{ "Contempt", record.GetAttributeValue(NBiometricAttributeId.EmotionContempt)},
				{ "Disgust", record.GetAttributeValue(NBiometricAttributeId.EmotionDisgust)},
				{ "Fear", record.GetAttributeValue(NBiometricAttributeId.EmotionFear)},
				{ "Happiness", record.GetAttributeValue(NBiometricAttributeId.EmotionHappiness)},
				{ "Sadness", record.GetAttributeValue(NBiometricAttributeId.EmotionSadness)},
				{ "Surprise", record.GetAttributeValue(NBiometricAttributeId.EmotionSurprise)},
			};

			var ethnicities = new Dictionary<string, float>()
			{
				{ "Ethnicity: White", record.GetAttributeValue(NBiometricAttributeId.EthnicityWhite) },
				{ "Ethnicity: Black", record.GetAttributeValue(NBiometricAttributeId.EthnicityBlack) },
				{ "Ethnicity: Asian", record.GetAttributeValue(NBiometricAttributeId.EthnicityAsian) },
				{ "Ethnicity: Hispanic", record.GetAttributeValue(NBiometricAttributeId.EthnicityHispanic) },
				{ "Ethnicity: Indian", record.GetAttributeValue(NBiometricAttributeId.EthnicityIndian) },
				{ "Ethnicity: Arabian", record.GetAttributeValue(NBiometricAttributeId.EthnicityArabian) },
			};

			var emotion = emotions.Where(x => x.Value <= NBiometricTypes.QualityMax).OrderByDescending(x => x.Value).FirstOrDefault();
			if (emotion.Key != null)
			{
				var text = $"{emotion.Key}: {emotion.Value}%";
				g.DrawString(text, _font, GetDirectionBrush(emotion.Value), offsetX, offsetY);
				offsetY += _lineHeight;
			}

			var ethnicity = ethnicities.Where(x => x.Value <= NBiometricTypes.QualityMax).OrderByDescending(x => x.Value).FirstOrDefault();
			if (ethnicity.Key != null)
			{
				var text = $"{ethnicity.Key}, {ethnicity.Value}%";
				g.DrawString(text, _font, GetDirectionBrush(ethnicity.Value), offsetX, offsetY);
				offsetY += _lineHeight;
			}
		}

		private void DrawInfo(Graphics g, Record record, float offsetX, float offsetY)
		{
			if (record.HasObject)
			{
				DrawObjectInfo(g, record, offsetX, offsetY);
			}
			else if (record.HasLicense)
			{
				DrawLicenseInfo(g, record, offsetX, offsetY);
			}
			else
			{
				DrawFaceInfo(g, record, offsetX, offsetY);
			}
		}

		private void DrawDirections(Graphics g, ObjectRecord record, ref float offsetX, ref float offsetY)
		{
			var directions = new Dictionary<GraphicsPath, float>()
			{
				{ PathUtils.CreateEastPath(), record.EastConfidence},
				{ PathUtils.CreateWestPath(), record.WestConfidence},
				{ PathUtils.CreateNorthPath(), record.NorthConfidence},
				{ PathUtils.CreateSouthPath(), record.SouthConfidence},
				{ PathUtils.CreateNorthEastPath(), record.NorthEastConfidence},
				{ PathUtils.CreateSouthEastPath(), record.SouthEastConfidence},
				{ PathUtils.CreateNorthWestPath(), record.NorthWestConfidence},
				{ PathUtils.CreateSouthWestPath(), record.SouthWestConfidence},
			};

			int count = 0;
			float relativeX = 0;
			foreach (var direction in directions.Where(d => d.Value >= 0.001).OrderByDescending(d => d.Value))
			{
				if (count == 4)
				{
					count = 0;
					relativeX = 0;
					offsetY += _lineHeight;
				}
				DrawIcon(g, direction.Value, direction.Key, GetDirectionBrush(direction.Value), offsetX + relativeX, offsetY);
				relativeX += 75;
				count++;
			}
			offsetY += _lineHeight;
		}

		private void DrawColor(Graphics g, Color color, float confidence, float x, float y)
		{
			using (Brush b = GetColorBrush(color, confidence))
			{
				DrawGraphicsPath(g, CreateCirclePath(), x, y, b, new Pen(Brushes.Black));
			}
			g.DrawString(confidence.ToString("0.0%"), _font, GetTypeBrush(confidence), x + 18, y);
		}

		private void DrawThumbnail(Graphics g, Bitmap thumb, float offsetX, float offsetY)
		{
			var newSize = ResizeKeepAspect(thumb.Size, _itemHeight - 5, _itemHeight - 5, true);
			float x = newSize.Width < _itemHeight ? offsetX + ((_itemHeight - newSize.Width) / 2) : offsetX;
			float y = newSize.Height < _itemHeight ? offsetY + ((_itemHeight - newSize.Height) / 2) : offsetY;
			g.DrawImage(thumb, x, y, newSize.Width, newSize.Height);
		}

		#endregion

		#region Event handlers

		private async void OnExportClick(object sender, EventArgs e)
		{
			if (RecordCollection == null) return;

			var records = await GetRecordsAsync(100);
			if (records.Count > 0)
			{
				string file = ShowExportDialog(records);
				if (file != null)
				{
					exportNextBtn.Enabled = true;
					filePathLbl.Text = file;
				}
				records.ForEach(rec => rec.Dispose());
			}
			else
			{
				MessageBox.Show("No records found to export.");
			}
		}

		private async void OnExportNextClick(object sender, EventArgs e)
		{
			var records = await GetRecordsAsync(100, _lastId);
			if (records.Count > 0)
			{
				string file = ShowExportDialog(records);
				if (file != null)
				{
					filePathLbl.Text = file;
				}
				records.ForEach(rec => rec.Dispose());
			}
			else
			{
				MessageBox.Show("No records found to export.");
			}
		}

		private void OnFilterChanged(object sender, EventArgs e)
		{
			exportNextBtn.Enabled = false;
		}

		private void OnFilePathClick(object sender, EventArgs e)
		{
			if (File.Exists(filePathLbl.Text))
			{
				Process.Start(filePathLbl.Text);
			}
		}

		private void OnTypeChanged(object sender, EventArgs e)
		{
			if (typeCmbBox.SelectedValue != null)
			{
				if ((SubjectType)typeCmbBox.SelectedValue == SubjectType.VehiclesAndHumans)
				{
					classCmbBox.Visible = true;
					classLbl.Visible = true;
				}
				else
				{
					classCmbBox.Visible = false;
					classLbl.Visible = false;
				}
			}
			exportNextBtn.Enabled = false;
		}

		private async void ButtonExportCsvClick(object sender, EventArgs e)
		{
			using (var dialog = new SaveFileDialog { FileName = "events.csv", Title = "Export", Filter = "Csv files|*.csv" })
			{
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					try
					{
						var sb = new StringBuilder();
						sb.Append("dbid;timestamp;source;hasObject;hasLicensePlate;hasFace;");
						sb.Append("person;car;truck;bus;bike;");
						sb.Append("red;orange;green;blue;silver;white;black;brown;gray;");
						sb.Append("north;northEast;east;southEast;south;southWest;west;northWest;clothes;clothesConfidence;");
						sb.Append("x;y;width;height;");
						sb.Append("vehicleMake;vehicleMakeConfidence;carModel;carModelConfidence;ageGroup;ageGroupConfidence");
						sb.Append("licensePlate;origin;ocrConf;detectionConf;licenseX;licenseY;licenseWidth;licenseHeight;");
						sb.Append("faceX;faceY;faceWidth;faceHeight;");
						sb.Append("age;gender;genderConfidence;withGlasses;glassesConfidence;withDarkGlasses;darkGlassesConfidence;withBeard;beardConfidence;");
						sb.Append("withMustache;mustacheConfidence;smile;smileConfidence;Blink;blinkConfidence;MouthOpen;mouthOpenConfidence;");
						sb.Append("FaceMask;faceMaskConfidence;ethnicityWhiteConfidence;ethnicityAsianConfidence;ethnicityHispanicConfidence;ethnicityIndianConfidence;ethnicityBlackConfidence;ethnicityArabianConfidence");
						sb.Append("emotionNeutralConfidence;emotionAngerConfidence;emotionContemptConfidence;emotionDisgustConfidence;");
						sb.Append("emotionFearConfidence;emotionHappinessConfidence;emotionSadnessConfidence;emotionSurpriseConfidence;");
						sb.Append("matchId;matchScore;");
						sb.AppendLine();

						var records = await GetRecordsAsync(1000, null, false);
						while (records?.Any() == true)
						{
							int lastId = 0;
							foreach (var r in records)
							{
								sb.AppendLine(GetCsvValues(r).Select(x => x?.ToString() ?? string.Empty).Aggregate((a, b) => a + ";" + b));
								lastId = r.Id;
								r.Dispose();
							}
							records = await GetRecordsAsync(1000, lastId, false);
						}

						File.WriteAllText(dialog.FileName, sb.ToString());
						filePathLbl.Text = dialog.FileName;
					}
					catch (Exception ex)
					{
						Utils.ShowException(ex);
					}
				}
			}
		}

		#endregion

		#region Private methods

		private IEnumerable<object> GetCsvValues(Record r)
		{
			yield return r.Id;
			yield return r.Timestamp;
			yield return r.Source;
			yield return r.HasObject;
			yield return r.HasLicense;
			yield return r.HasFace;

			if (r.HasObject)
			{
				var obj = r.Object;
				yield return obj.PersonConfidence;
				yield return obj.CarConfidence;
				yield return obj.TruckConfidence;
				yield return obj.BusConfidence;
				yield return obj.BikeConfidence;
				yield return obj.RedColorConfidence;
				yield return obj.OrangeColorConfidence;
				yield return obj.GreenColorConfidence;
				yield return obj.BlueColorConfidence;
				yield return obj.SilverColorConfidence;
				yield return obj.WhiteColorConfidence;
				yield return obj.BlackColorConfidence;
				yield return obj.BrownColorConfidence;
				yield return obj.GrayColorConfidence;
				yield return obj.NorthConfidence;
				yield return obj.NorthEastConfidence;
				yield return obj.EastConfidence;
				yield return obj.SouthEastConfidence;
				yield return obj.SouthConfidence;
				yield return obj.SouthWestConfidence;
				yield return obj.WestConfidence;
				yield return obj.NorthWestConfidence;
				string clothesString = null;
				string confidencesString = null;
				foreach (var v in obj.Clothes)
				{
					clothesString += v.Key + "'";
					confidencesString += v.Value.ToString() + "'";
				}
				yield return clothesString;
				yield return confidencesString;

				yield return obj.BoundingRect.X;
				yield return obj.BoundingRect.Y;
				yield return obj.BoundingRect.Width;
				yield return obj.BoundingRect.Height;

				if (obj.VehicleMake?.Length > 0)
				{
					yield return obj.VehicleMake;
					yield return obj.VehicleMakeConfidence;
				}
				else
				{
					yield return null;
					yield return float.NaN;
				}
				if (obj.CarModel?.Length > 0)
				{
					yield return obj.CarModel;
				}
				else
				{
					yield return null;
					yield return float.NaN;
				}

				yield return obj.AgeGroup.ToString();
				yield return obj.AgeGroupConfidence;
			}
			else
			{
				for (int i = 0; i < 22; i++)
				{
					yield return float.NaN;
				}
				for (int i = 0; i < 2; i++)
				{
					yield return null;
				}
				for (int i = 0; i < 4; i++)
				{
					yield return -1;
				}
				for (int i = 0; i < 2; i++)
				{
					yield return null;
				}
				yield return null;
				yield return float.NaN;
			}

			if (r.HasLicense)
			{
				var lic = r.License;
				yield return lic.Value;
				yield return lic.Origin;
				yield return lic.OcrConfidence;
				yield return lic.DetectionConfidence;
				yield return lic.BoundingRect.X;
				yield return lic.BoundingRect.Y;
				yield return lic.BoundingRect.Width;
				yield return lic.BoundingRect.Height;
			}
			else
			{
				for (int i = 0; i < 2; i++)
				{
					yield return null;
				}
				for (int i = 0; i < 2; i++)
				{
					yield return float.NaN;
				}
				for (int i = 0; i < 4; i++)
				{
					yield return -1;
				}
			}

			if (r.HasFace)
			{
				var face = r.Face;
				yield return face.BoundingRect.X;
				yield return face.BoundingRect.Y;
				yield return face.BoundingRect.Width;
				yield return face.BoundingRect.Height;
				yield return face.GetAttributeValue(NBiometricAttributeId.Age);
				yield return face.Gender;

				if (face.Gender == NGender.Male)
					yield return face.GetAttributeValue(NBiometricAttributeId.GenderMale);
				else if (face.Gender == NGender.Female)
					yield return face.GetAttributeValue(NBiometricAttributeId.GenderFemale);
				else
					yield return NBiometricTypes.QualityUnknown;

				var ids = new[]
				{
					NBiometricAttributeId.Glasses,
					NBiometricAttributeId.DarkGlasses,
					NBiometricAttributeId.Beard,
					NBiometricAttributeId.Mustache,
					NBiometricAttributeId.Smile,
					NBiometricAttributeId.EyesOpen,
					NBiometricAttributeId.MouthOpen,
					NBiometricAttributeId.FaceMask,
					NBiometricAttributeId.EthnicityWhite,
					NBiometricAttributeId.EthnicityAsian,
					NBiometricAttributeId.EthnicityHispanic,
					NBiometricAttributeId.EthnicityIndian,
					NBiometricAttributeId.EthnicityBlack,
					NBiometricAttributeId.EthnicityArabian,
					NBiometricAttributeId.EmotionNeutral,
					NBiometricAttributeId.EmotionAnger,
					NBiometricAttributeId.EmotionContempt,
					NBiometricAttributeId.EmotionDisgust,
					NBiometricAttributeId.EmotionFear,
					NBiometricAttributeId.EmotionHappiness,
					NBiometricAttributeId.EmotionSadness,
					NBiometricAttributeId.EmotionSurprise
				};

				foreach (var id in ids)
				{
					var conf = face.GetAttributeValue(id);
					yield return conf > 50 && conf <= NBiometricTypes.QualityMax;
					yield return conf;
				}

				yield return face.Match?.Id ?? null;
				yield return face.Match?.Score ?? -1;
			}
			else
			{
				for (int i = 0; i < 4; i++)
				{
					yield return -1;
				}
				for (int i = 0; i < 35; i++)
				{
					yield return float.NaN;
				}
			}
		}

		private string ShowExportDialog(List<Record> records)
		{
			var saveFileDialog = new SaveFileDialog
			{
				FileName = $"Events_{records.First().Id}_{records.Last().Id}.png",
				Filter = "Png Image|*.png|Jpeg Image|*.jpg|Bitmap Image|*.bmp",
				Title = "Export"
			};

			bool save = saveFileDialog.ShowDialog() == DialogResult.OK;

			if (save)
			{
				if (string.IsNullOrEmpty(saveFileDialog.FileName))
				{
					MessageBox.Show("Invalid file path");
					return null;
				}
				Export(records, saveFileDialog.FileName);
				return saveFileDialog.FileName;
			}
			else
				return null;
		}

		private void Export(List<Record> records, string filename)
		{
			float maxHeight = 0;
			using (var bm = new Bitmap(10 * _itemWidth, GetImageHeight(records)))
			{
				using (var g = Graphics.FromImage(bm))
				{
					SetupGraphics(g);
					float offsetX = 0;
					float offsetY = 0;
					int index = 0;
					for (; index < records.Count; index++)
					{
						var record = records[index];
						if (offsetY + _itemHeight > (10 * _itemHeight))
						{
							offsetY = 0;
							offsetX += _itemWidth;
							g.DrawLine(new Pen(Color.DodgerBlue, 3), new Point((int)offsetX - 10, 0), new Point((int)offsetX - 10, _itemHeight * 10));
							if (offsetX >= (10 * _itemWidth))
								break;
						}
						if (record.HasObject)
						{
							DrawThumbnail(g, record.Object.Thumbnail, offsetX, offsetY);
							DrawInfo(g, record, offsetX + _itemHeight, offsetY);
						}
						else if (record.HasLicense)
						{
							DrawThumbnail(g, record.License.Thumbnail, offsetX, offsetY);
							DrawInfo(g, record, offsetX + _itemHeight, offsetY);
						}
						else
							DrawFace(g, record, offsetX, offsetY);
						offsetY += _itemHeight;

						if (record.HasObject && record.HasLicense)
						{
							DrawThumbnail(g, record.License.Thumbnail, offsetX, offsetY);
							DrawLicenseInfo(g, record, offsetX + _itemHeight, offsetY, true);
							offsetY += _itemHeight;
						}
						else if (record.HasObject && record.HasFace)
						{
							DrawFace(g, record, offsetX, offsetY, true);
							offsetY += _itemHeight;
						}

						g.DrawLine(new Pen(Color.DodgerBlue, 3), new Point((int)offsetX - 10, (int)offsetY), new Point(_itemWidth - 10 + (int)offsetX, (int)offsetY));
						if (offsetY + _itemHeight > (10 * _itemHeight) && record.HasObject && record.HasLicense)
						{
							g.DrawLine(new Pen(Color.DodgerBlue, 3), new Point((int)offsetX - 10, _itemHeight * 10), new Point((int)offsetX - 10, _itemHeight * 11));
							g.DrawLine(new Pen(Color.DodgerBlue, 3), new Point((int)offsetX - 10 + _itemWidth, _itemHeight * 10), new Point((int)offsetX - 10 + _itemWidth, _itemHeight * 11));
						}
						maxHeight = offsetY > maxHeight ? offsetY : maxHeight;
					}

					if (index > 0)
						_lastId = records[index - 1].Id;

					if (bm.Width > (offsetX + _itemWidth) || bm.Height > maxHeight)
					{
						using (var cropped = CropImage(bm, new Rectangle(0, 0, (int)offsetX + _itemWidth, (int)maxHeight)))
						{
							cropped.Save(filename);
						}
					}
					else
						bm.Save(filename);
				}
			}
		}

		private void DrawFace(Graphics g, Record record, float offsetX, float offsetY, bool withObject = false)
		{
			if (record.Face.Match == null)
			{
				DrawThumbnail(g, record.Face.Thumbnail, offsetX, offsetY);
				DrawFaceInfo(g, record, offsetX + _itemHeight, offsetY, withObject);
			}
			else
			{
				DrawMatchThumbnail(g, record.Face.Thumbnail, offsetX == 0 ? offsetX : offsetX - 10, offsetY);
				var match = _matchFaceRecords[record.Face.Match.Id];
				using (var image = _matchFaceRecords.GetThumbnailById(match.Id))
				{
					DrawMatchThumbnail(g, image.ToBitmap(), offsetX + _itemHeight - 25, offsetY);
				}
				DrawFaceInfo(g, record, offsetX + (_itemHeight * 2) - 45, offsetY, withObject);
			}
			offsetY += _itemHeight;
		}

		private void DrawMatchThumbnail(Graphics g, Bitmap thumb, float offsetX, float offsetY)
		{
			var newSize = ResizeKeepAspect(thumb.Size, _itemHeight - 5, _itemHeight - 5, true);
			float x = newSize.Width < _itemHeight ? offsetX + ((_itemHeight - newSize.Width) / 2) : offsetX;
			float y = newSize.Height < _itemHeight ? offsetY + ((_itemHeight - newSize.Height) / 2) : offsetY;
			g.DrawImage(thumb, x, y, newSize.Width - 20, newSize.Height - 20);

		}

		private Task<List<Record>> GetRecordsAsync(int limit, int? fromId = null, bool withThumbnails = true)
		{
			var type = (SubjectType)typeCmbBox.SelectedValue;
			switch (type)
			{
				case SubjectType.Any:
					return RecordCollection.GetRecordsAsync(limit, fromDateTimePicker.Value, (Order)orderCmbBox.SelectedItem, fromId, withThumbnails);
				case SubjectType.VehiclesAndHumans:
					return RecordCollection.GetObjectRecordsAsync(limit, fromDateTimePicker.Value, (Order)orderCmbBox.SelectedItem, fromId, classCmbBox.SelectedValue.ToString().ToLower(), withThumbnails);
				case SubjectType.ALPR:
					return RecordCollection.GetLicenseRecordsAsync(limit, fromDateTimePicker.Value, (Order)orderCmbBox.SelectedItem, fromId, withThumbnails);
				case SubjectType.VehiclesWithALPR:
					return RecordCollection.GetObjectsWithLicenseAsync(limit, fromDateTimePicker.Value, (Order)orderCmbBox.SelectedItem, fromId, withThumbnails);
				case SubjectType.Faces:
					return RecordCollection.GetFaceRecordsAsync(limit, fromDateTimePicker.Value, (Order)orderCmbBox.SelectedItem, fromId, withThumbnails);
				case SubjectType.HumansWithFaces:
					return RecordCollection.GetObjectsWithFacesAsync(limit, fromDateTimePicker.Value, (Order)orderCmbBox.SelectedItem, fromId, withThumbnails);
				default:
					return RecordCollection.GetRecordsAsync(limit, fromDateTimePicker.Value, (Order)orderCmbBox.SelectedItem, fromId, withThumbnails);
			}
		}

		private int GetImageHeight(List<Record> records)
		{
			if (!records.Any(x => x.HasObject && x.HasLicense))
				return 10 * _itemHeight;

			int pattern = 9;
			for (int i = 0; i < records.Count; i++)
			{
				var record = records[i];

				if (record.HasObject && record.HasLicense)
				{
					if (i == pattern)
						return 11 * _itemHeight;
					pattern -= 1;
				}

				if (i == pattern)
				{
					pattern += 10;
				}
			}
			return 10 * _itemHeight;
		}

		#endregion

		#region Helper functions

		private static Bitmap CropImage(Bitmap source, Rectangle section)
		{
			var bmp = new Bitmap(section.Width, section.Height);
			using (var g = Graphics.FromImage(bmp))
			{
				g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
			}
			return bmp;
		}

		private static Size ResizeKeepAspect(Size srcSize, int maxWidth, int maxHeight, bool enlarge = false)
		{
			maxWidth = enlarge ? maxWidth : Math.Min(maxWidth, srcSize.Width);
			maxHeight = enlarge ? maxHeight : Math.Min(maxHeight, srcSize.Height);

			decimal rnd = Math.Min(maxWidth / (decimal)srcSize.Width, maxHeight / (decimal)srcSize.Height);
			return new Size((int)Math.Round(srcSize.Width * rnd), (int)Math.Round(srcSize.Height * rnd));
		}

		private static GraphicsPath CreateCirclePath()
		{
			var gp = new GraphicsPath();
			gp.AddEllipse(0, 0, 100, 100);

			var bounds = gp.GetBounds();
			var m = new Matrix();
			m.Translate(-bounds.X, -bounds.Y);
			gp.Transform(m);

			return gp;
		}

		private static Brush GetTypeBrush(float confidence)
		{
			return confidence >= 0.5f ? Brushes.Black : Brushes.Gray;
		}

		private static Brush GetDirectionBrush(float confidence)
		{
			return confidence >= 0.3f ? Brushes.Black : Brushes.Gray;
		}

		private static Brush GetColorBrush(Color color, float confidence)
		{
			return confidence >= 0.5f ? new SolidBrush(color) : new SolidBrush(color);
		}

		#endregion
	}
}
