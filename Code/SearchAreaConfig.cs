using Neurotec.Surveillance;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace Neurotec.Samples.Code
{
	public class SearchAreaConfig : System.Xml.Serialization.IXmlSerializable
	{
		#region Public properties

		public List<NSearchArea> Areas { get; set; } = new List<NSearchArea>();
		public string SourceId { get; set; }
		public bool IsGrid { get; set; }
		public int Rows { get; set; }
		public int Columns { get; set; }
		public bool CheckSearchAreaByObjectCenter { get; set; }

		#endregion

		#region IXmlSerializable

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "SearchAreaConfig")
					{
						if (reader.MoveToAttribute("IsGrid"))
							IsGrid = bool.Parse(reader.Value);
						if (reader.MoveToAttribute("Rows"))
							Rows = int.Parse(reader.Value);
						if (reader.MoveToAttribute("Columns"))
							Columns = int.Parse(reader.Value);
						if (reader.MoveToAttribute("SourceId"))
							SourceId = reader.Value;
						if (reader.MoveToAttribute("CheckSearchAreaByObjectCenter"))
							CheckSearchAreaByObjectCenter = bool.Parse(reader.Value);
					}
					else if (reader.Name == "Rect")
					{
						var rect = new NRectangleSearchArea();
						reader.MoveToAttribute("Type");
						rect.Type = (NSearchAreaType)int.Parse(reader.Value);
						reader.Read();
						var value = reader.ReadContentAsString();

						var coords = value.Split(new char[] { ';' });
						rect.Rectangle = new RectangleF
						{
							X = float.Parse(coords[0]),
							Y = float.Parse(coords[1]),
							Width = float.Parse(coords[2]),
							Height = float.Parse(coords[3]),
						};
						Areas.Add(rect);
					}
					else if (reader.Name == "Polygon")
					{
						var polygon = new NPolygonSearchArea();
						reader.MoveToAttribute("Type");
						polygon.Type = (NSearchAreaType)int.Parse(reader.Value);
						reader.Read();
						var value = reader.ReadContentAsString();

						var points = value.Split(new char[] { '|' });
						foreach (var p in points)
						{
							var coords = p.Split(new char[] { ';' });
							polygon.Points.Add(new PointF(float.Parse(coords[0]), float.Parse(coords[1])));
						}
						Areas.Add(polygon);
					}
				}
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("SearchAreaConfig");
			writer.WriteAttributeString("SourceId", SourceId);
			writer.WriteAttributeString("IsGrid", IsGrid.ToString());
			writer.WriteAttributeString("Rows", Rows.ToString());
			writer.WriteAttributeString("Columns", Columns.ToString());
			writer.WriteAttributeString("CheckSearchAreaByObjectCenter", CheckSearchAreaByObjectCenter.ToString());
			{
				writer.WriteStartElement("Areas");
				foreach (var area in Areas)
				{
					if (area is NRectangleSearchArea rectArea)
					{
						var r = rectArea.Rectangle;
						writer.WriteStartElement("Rect");
						writer.WriteAttributeString("Type", ((int)rectArea.Type).ToString());
						writer.WriteString($"{r.X};{r.Y};{r.Width};{r.Height}");
						writer.WriteEndElement();
					}
					else if (area is NPolygonSearchArea polygonArea)
					{
						writer.WriteStartElement("Polygon");
						writer.WriteAttributeString("Type", ((int)polygonArea.Type).ToString());
						writer.WriteString(polygonArea
							.Points
							.Select(p => $"{p.X};{p.Y}")
							.Aggregate((a, b) => a + "|" + b));
						writer.WriteEndElement();
					}
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		#endregion
	}
}
