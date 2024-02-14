using Neurotec.Samples.Properties;
using System.Collections.Generic;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Neurotec.Samples.Code
{
	public class SurveillanceConfig
	{
		#region Public constants

		public const string DBFileFaceWatchlist = "SurveillanceSampleWatchlistV9.db";
		public const string DBFileLicensePlateWatchlist = "LicensePlateWatchlistV9.db";
		public const string DBFileEvents = "SurveillanceSampleEventsV9.db";
		public const string DBFileCellPhoneWatchList = "CellPhoneWatchlistV9.db";

		#endregion

		#region Public static properties

		public static bool SaveEvents
		{
			get
			{
				try { return Settings.Default.SaveEvents; }
				catch { return true; }
			}
			set
			{
				Settings.Default.SaveEvents = value;
			}
		}

		public static int MiscMaxTreeNodeCount
		{
			get
			{
				try { return Settings.Default.MiscMaxTreeNodeCount; }
				catch { return 100; }
			}
			set
			{
				Settings.Default.MiscMaxTreeNodeCount = value;
			}
		}

		public static int RetryFrequency
		{
			get
			{
				try { return Settings.Default.RetryFrequency; }
				catch { return 60; }
			}
			set
			{
				Settings.Default.RetryFrequency = value;
			}
		}

		public static string SurveillanceProperties
		{
			get
			{
				try { return Settings.Default.SurveillanceProperties; }
				catch { return string.Empty; }
			}
			set
			{
				Settings.Default.SurveillanceProperties = value;
			}
		}

		public static Orientation Orientation
		{
			get
			{
				try { return (Orientation)Settings.Default.Orientation; }
				catch { return Orientation.Vertical; }
			}
			set
			{
				Settings.Default.Orientation = (int)value;
			}
		}

		public static string Layout
		{
			get
			{
				try { return Settings.Default.Layout; }
				catch { return null; }
			}
			set
			{
				Settings.Default.Layout = value;
			}
		}

		public static bool ShowDetails
		{
			get
			{
				try { return Settings.Default.ShowDetails; }
				catch { return true; }
			}
			set
			{
				Settings.Default.ShowDetails = value;
			}
		}

		public static bool FirstStart
		{
			get
			{
				try { return Settings.Default.FirstStart; }
				catch { return true; }
			}
			set
			{
				Settings.Default.FirstStart = value;
			}
		}

		public static Dictionary<string, Preset> Presets
		{
			get
			{
				try
				{
					Dictionary<string, Preset> result = new Dictionary<string, Preset>();
					if (Settings.Default.PresetNameList != null)
					{
						for (int i = 0; i < (int)Settings.Default.PresetNameList.Count; i++)
						{
							string name = Settings.Default.PresetNameList[i];
							string guid = Settings.Default.PresetGUIDList[i];
							string propertiesString = Settings.Default.PresetPropertiesList[i];
							result[guid] = new Preset(name, guid, propertiesString);
						}
					}

					return result;
				}
				catch { return new Dictionary<string, Preset>(); }
			}
			set
			{
				StringCollection presetNameList = new StringCollection();
				StringCollection presetGUIDList = new StringCollection();
				StringCollection presetPropertiesList = new StringCollection();

				foreach (var item in value)
				{
					presetGUIDList.Add(item.Key);
					presetNameList.Add(item.Value.Name);
					presetPropertiesList.Add(item.Value.PropertiesString);
				}

				Settings.Default.PresetNameList = presetNameList;
				Settings.Default.PresetGUIDList = presetGUIDList;
				Settings.Default.PresetPropertiesList = presetPropertiesList;
			}
		}

		public static Dictionary<string, string> SourcePresets
		{
			get
			{
				try
				{
					Dictionary<string, string> result = new Dictionary<string, string>();
					if (Settings.Default.SourceIDList != null)
					{
						for (int i = 0; i < (int)Settings.Default.SourceIDList.Count; i++)
						{
							result.Add(Settings.Default.SourceIDList[i], Settings.Default.SourcePresetGUIDList[i]);
						}
					}

					return result;
				}
				catch { return new Dictionary<string, string>(); }
			}
			set
			{
				StringCollection sourceIDList = new StringCollection();
				StringCollection sourcePresetGUIDList = new StringCollection();

				foreach (var item in value)
				{
					sourceIDList.Add(item.Key);
					sourcePresetGUIDList.Add(item.Value);
				}

				Settings.Default.SourceIDList = sourceIDList;
				Settings.Default.SourcePresetGUIDList = sourcePresetGUIDList;

			}
		}

		public static Dictionary<string, SearchAreaConfig> SearchArea
		{
			get
			{
				try
				{
					var desiarializer = new XmlSerializer(typeof(SearchAreaConfig));
					var dict = new Dictionary<string, SearchAreaConfig>();
					if (Settings.Default.SearchArea != null)
					{
						foreach (var value in Settings.Default.SearchArea)
						{
							var reader = new StringReader(value);
							var area = (SearchAreaConfig)desiarializer.Deserialize(reader);
							dict.Add(area.SourceId, area);
							reader.Dispose();
						}
					}
					return dict;
				}
				catch { return new Dictionary<string, SearchAreaConfig>(); }
			}
			set
			{
				var result = new StringCollection();
				if (value != null)
				{
					var serializer = new XmlSerializer(typeof(SearchAreaConfig));
					foreach (var pair in value)
					{
						var writer = new StringWriter();
						serializer.Serialize(writer, pair.Value);
						result.Add(writer.ToString());
						writer.Dispose();
					}
				}
				Settings.Default.SearchArea = result;
				Settings.Default.Save();
			}
		}

		public static DetailsFilter DetailsFilter
		{
			get
			{
				try
				{
					
					if (!string.IsNullOrEmpty(Settings.Default.DetailsFilter))
					{
						var serializer = new XmlSerializer(typeof(DetailsFilter));
						using (var reader = new StringReader(Settings.Default.DetailsFilter))
						{
							return (DetailsFilter)serializer.Deserialize(reader);
						}
					}
					return new DetailsFilter();
				}
				catch { return new DetailsFilter(); }
			}
			set
			{
				if (value == null) throw new NullReferenceException(nameof(value));

				var serializer = new XmlSerializer(typeof(DetailsFilter));
				using (var writer = new StringWriter())
				{
					serializer.Serialize(writer, value);
					Settings.Default.DetailsFilter = writer.ToString();
					Settings.Default.Save();
				}
			}
		}

		#endregion

		#region Public static methods

		public static void Save()
		{
			Settings.Default.Save();
		}

		public static void Reload()
		{
			Settings.Default.Reload();
		}

		private static string GetDefaultValue(string property)
		{
			System.ComponentModel.AttributeCollection attributes = System.ComponentModel.TypeDescriptor.GetProperties(Settings.Default)[property].Attributes;
			System.Configuration.DefaultSettingValueAttribute attribute = (System.Configuration.DefaultSettingValueAttribute)attributes[typeof(System.Configuration.DefaultSettingValueAttribute)];
			return attribute.Value;
		}

		public static void ResetGeneralSettings()
		{
			Settings.Default.MiscMaxTreeNodeCount = System.Int32.Parse(GetDefaultValue("MiscMaxTreeNodeCount"));
			Settings.Default.RetryFrequency = System.Int32.Parse(GetDefaultValue("RetryFrequency"));
			Settings.Default.SaveEvents = System.Boolean.Parse(GetDefaultValue("SaveEvents"));
		}

		#endregion
	}
}
