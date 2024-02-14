using Neurotec.Samples.Code;
using Neurotec.Surveillance;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows.Forms;

namespace Neurotec.Samples.Forms
{
	public partial class SelectRegionOfInterestForm : Form
	{
		#region Private types

		private enum ToolType
		{
			Grid = 0,
			IncludePolygon,
			ExcludePolygon,
			IncludeRect,
			ExcludeRect,
		}

		private class SearchArea
		{
			#region Private properties

			public RectangleF Rect { get; set; }
			public bool Include { get; set; } = true;
			public List<PointF> Polygon { get; set; } = new List<PointF>();

			#endregion

			#region Public methods

			public GraphicsPath CreatePath(int w, int h)
			{
				if (Polygon?.Any() == true)
				{
					var gp = new GraphicsPath();
					gp.AddPolygon(Polygon.Select(p => ToAbsolute(p, w, h)).ToArray());
					gp.CloseFigure();
					return gp;
				}
				else
				{
					var gp = new GraphicsPath();
					gp.AddRectangle(ToAbsolute(Rect, w, h));
					return gp;
				}
			}

			public SearchArea Clone()
			{
				return new SearchArea
				{
					Rect = Rect,
					Include = Include,
					Polygon = Polygon.ToList()
				};
			}

			#endregion
		}

		private class UndoStack
		{
			#region Private fields

			private Stack<Tool> _stack = new Stack<Tool>();

			#endregion

			#region Public events

			public event EventHandler StackIsEmptyChanged;

			#endregion

			#region Public properties

			public bool IsEmpty
			{
				get => _stack.Count == 0;
			}

			#endregion

			#region Public methods

			public void Push(Tool value)
			{
				_stack.Push(value);
				if (_stack.Count == 1)
					StackIsEmptyChanged?.Invoke(this, EventArgs.Empty);
			}

			public Tool Pop()
			{
				var count = _stack.Count;
				if (count > 0)
				{
					var result = _stack.Pop();
					if (count == 1)
						StackIsEmptyChanged?.Invoke(this, EventArgs.Empty);
					return result;
				}
				return null;
			}

			public void Clear()
			{
				var count = _stack.Count;
				_stack.Clear();
				if (count > 0)
					StackIsEmptyChanged?.Invoke(this, EventArgs.Empty);
			}

			#endregion
		}

		private class GridTool : Tool
		{
			#region Public constructor

			public GridTool(UndoStack undoStack, int rows, int columns)
				: base(undoStack)
			{
				Grid = CreateGrid(rows, columns).ToList();
			}

			#endregion

			#region Private constructor

			private GridTool(UndoStack undoStack)
				: base(undoStack)
			{
			}

			#endregion

			#region Public properties

			public SearchArea CurrentRegion { get; set; }
			public override bool IsValid => Grid.Any(x => !x.Include);

			#endregion

			#region Private static methods

			private static IEnumerable<SearchArea> CreateGrid(int rows, int columns)
			{
				var w = 1.0f / columns;
				var h = 1.0f / rows;

				for (var row = 0; row < rows; row++)
				{
					for (var column = 0; column < columns; column++)
					{
						var rect = new RectangleF
						{
							X = column * w,
							Y = row * h,
							Width = w,
							Height = h
						};
						yield return new SearchArea { Rect = rect, Include = true };
					}
				}
			}

			#endregion

			#region Private methods

			private void SplitCurrentReqion(int w, int h)
			{
				if (CurrentRegion != null)
				{
					Undo.Push(Clone());
					Grid.Remove(CurrentRegion);
					Grid.AddRange(SplitCurrentRegionInternal(w, h));
				}
			}

			private IEnumerable<SearchArea> SplitCurrentRegionInternal(int w, int h)
			{
				var offsetX = CurrentPosition.X / w - CurrentRegion.Rect.X;
				var offsetY = CurrentPosition.Y / h - CurrentRegion.Rect.Y;

				var r = CurrentRegion.Rect;
				var topLeft = new SearchArea
				{
					Include = CurrentRegion.Include,
					Rect = new RectangleF(r.X, r.Y, offsetX, offsetY)
				};
				yield return topLeft;

				var topRight = new SearchArea
				{
					Include = CurrentRegion.Include,
					Rect = new RectangleF(r.X + offsetX, r.Y, r.Width - offsetX, offsetY)
				};
				yield return topRight;

				var bottomLeft = new SearchArea
				{
					Include = CurrentRegion.Include,
					Rect = new RectangleF(r.X, r.Y + offsetY, offsetX, r.Height - offsetY)
				};
				yield return bottomLeft;

				var bottomRight = new SearchArea
				{
					Include = CurrentRegion.Include,
					Rect = new RectangleF(r.X + offsetX, r.Y + offsetY, r.Width - offsetX, r.Height - offsetY)
				};
				yield return bottomRight;
			}

			#endregion

			#region Public methods

			public override bool OnMouseMove(PointF imagePosition, int w, int h, Keys modifierKeys, MouseButtons mouseButtons)
			{
				base.OnMouseMove(imagePosition, w, h, modifierKeys, mouseButtons);

				var relativePos = ToRelative(imagePosition, w, h);
				var region = Grid.FirstOrDefault(x => x.Rect.Contains(relativePos));
				if (region != null)
				{
					CurrentRegion = region;
					return true;
				}
				else if (CurrentRegion != null)
				{
					CurrentRegion = null;
					return true;
				}

				return (Keys.Control & modifierKeys) == Keys.Control;
			}

			public override bool OnMouseLeftDown(PointF imagePosition, int w, int h, Keys modifierKeys, int clickCount)
			{
				base.OnMouseLeftDown(imagePosition, w, h, modifierKeys, clickCount);

				if (CurrentRegion != null)
				{
					if ((modifierKeys & Keys.Control) == Keys.Control)
					{
						SplitCurrentReqion(w, h);
					}
					else
					{
						Undo.Push(Clone());
						CurrentRegion.Include = !CurrentRegion.Include;
					}
					return true;
				}
				return false;
			}

			public override Tool Clone()
			{
				return new GridTool(Undo)
				{
					Grid = Grid.Select(x => x.Clone()).ToList(),
				};
			}

			#endregion
		}

		private class PolygonSelectionTool : Tool
		{
			#region Private types

			private class Line
			{
				#region Public constructor

				public Line(PointF p1, PointF p2)
				{
					if (p1.X < p2.X)
					{
						X1 = Convert.ToDecimal(p1.X);
						Y1 = Convert.ToDecimal(p1.Y);
						X2 = Convert.ToDecimal(p2.X);
						Y2 = Convert.ToDecimal(p2.Y);
					}
					else
					{
						X1 = Convert.ToDecimal(p2.X);
						Y1 = Convert.ToDecimal(p2.Y);
						X2 = Convert.ToDecimal(p1.X);
						Y2 = Convert.ToDecimal(p1.Y);
					}

					if (X2 != X1)
					{
						M = (Y2 - Y1) / (X2 - X1);
						B = Y1 - (M * X1);
					}
					else
					{
						M = 0;
						B = X1;
					}
				}

				#endregion

				#region Public properties

				public decimal X1 { get; private set; }
				public decimal Y1 { get; private set; }
				public decimal X2 { get; private set; }
				public decimal Y2 { get; private set; }
				public decimal M { get; private set; }
				public decimal B { get; private set; }

				#endregion

				#region Private methods

				private bool TestInterval(decimal x, decimal y)
				{
					var minx = Math.Min(X1, X2);
					var maxx = Math.Max(X1, X2);
					var miny = Math.Min(Y1, Y2);
					var maxy = Math.Max(Y1, Y2);

					return minx <= x && x <= maxx
						&& miny <= y && y <= maxy;
				}

				#endregion

				#region Public methods

				public bool Intersects(Line line, out PointF point)
				{
					point = PointF.Empty;
					if (M == line.M)
					{
						if (B == line.B)
						{
							// Same line, but maybe different ranges
							if (X1 != 0)
							{
								return (X1 < line.X1 && line.X1 < X2) || (X1 < line.X2 && line.X2 < X2);
							}
							else
							{
								return (Y1 < line.Y1 && line.Y1 < Y2) || (Y1 < line.Y2 && line.Y2 < Y2);
							}
						}
						return false;
					}
					else
					{
						var x = (line.B - B) / (M - line.M);
						var y = M * x + B;

						float Distance( decimal lx, decimal ly)
						{
							var a = lx - x;
							var b = ly - y;
							var cSquare = Convert.ToSingle(a * a + b * b);
							return (float)Math.Sqrt(cSquare);
						}

						point = new PointF(Convert.ToSingle(x), Convert.ToSingle(y));

						var diff1 = Math.Min(Distance( X1, Y1), Distance(X2, Y2));
						var diff2 = Math.Min(Distance(line.X1, line.Y1), Distance(line.X2, line.Y2));
						return TestInterval(x, y) && line.TestInterval(x, y) && diff1 > float.Epsilon && diff2 > float.Epsilon;
					}
				}

				#endregion
			}

			#endregion

			#region Public constructor

			public PolygonSelectionTool(UndoStack undoStack, ToolType mode)
				: base(undoStack)
			{
				Mode = mode;
			}

			#endregion

			#region Public properties

			public bool IsEmpty { get => Grid.Count == 0; }
			public PointF StartPosition { get; set; }
			public ToolType Mode { get; set; } = ToolType.IncludeRect;
			public bool Modifying { get; set; }
			public bool IsInclude { get => Mode == ToolType.IncludeRect || Mode == ToolType.IncludePolygon; }
			public bool IsPolygon { get => Mode == ToolType.ExcludePolygon || Mode == ToolType.IncludePolygon; }
			public Bitmap RegionMap { get; set; }
			public List<PointF> Points { get; set; } = new List<PointF>();
			public override bool IsValid => Grid.Any();

			#endregion

			#region Private methods

			private void AddGridEntry(SearchArea entry)
			{
				Undo.Push(Clone());
				if (Grid.Count == 0)
				{
					Grid.Add(new SearchArea
					{
						Include = !IsInclude,
						Rect = new RectangleF { X = 0, Y = 0, Width = 1, Height = 1 }
					});
				}
				Grid.Add(entry);
				RegionMap?.Dispose();
				RegionMap = null;
			}

			private PointF ClampOnImageBorderIntersect(PointF p, PointF previous, int w, int h)
			{
				var topLeft = PointF.Empty;
				var topRigth = new PointF(w, 0);
				var bottomLeft = new PointF(0, h);
				var bottomRight = new PointF(w, h);

				var line = new Line(p, previous);
				var topLine = new Line(topLeft, topRigth);
				var bottomLine = new Line(bottomLeft, bottomRight);
				var leftLine = new Line(topLeft, bottomLeft);
				var rightLine = new Line(topRigth, bottomRight);
				if (line.Intersects(topLine, out var intersect) || line.Intersects(bottomLine, out intersect) ||
					line.Intersects(leftLine, out intersect) || line.Intersects(rightLine, out intersect))
				{
					return intersect;
				}

				return p;
			}

			#endregion

			#region Public methods

			public Bitmap CreateRegionBitmap(int w, int h)
			{
				Bitmap bmp = null;

				if (IsEmpty)
					return null;

				try
				{
					bmp = new Bitmap(w, h);
					using (var g = Graphics.FromImage(bmp))
					using (var greenBrush = new SolidBrush(Color.FromArgb(50, 0, 255, 0)))
					using (var redBrush = new SolidBrush(Color.FromArgb(50, 255, 0, 0)))
					{
						Region topLevelUnion = null;
						foreach (var cell in Grid.AsEnumerable().Reverse())
						{
							using (var path = cell.CreatePath(w, h))
							{
								var activeRegion = new Region(path);
								if (topLevelUnion != null)
								{
									if (!activeRegion.IsEmpty(g) && !activeRegion.IsInfinite(g))
										activeRegion.Exclude(topLevelUnion);
									if (cell.Polygon?.Any() == false)
										topLevelUnion.Union(ToAbsolute(cell.Rect, w, h));
									else
										topLevelUnion.Union(activeRegion.Clone() as Region);
								}
								else
								{
									topLevelUnion = activeRegion;
								}

								g.FillRegion(cell.Include ? greenBrush : redBrush, activeRegion);
								if (topLevelUnion != activeRegion)
									activeRegion.Dispose();
							}
						}
						topLevelUnion?.Dispose();
					}
				}
				catch
				{
					bmp.Dispose();
					throw;
				}

				return bmp;
			}

			public bool IsPolygonPointValid(PointF imagePosition)
			{
				return IsPolygonPointValid(Points, imagePosition, out var _);
			}

			public static bool IsPolygonPointValid(List<PointF> points, PointF imagePosition, out PointF intersectPoint)
			{
				intersectPoint = PointF.Empty;
				if (points.Count > 2)
				{
					var lastPoint = points.Last();
					var newLine = new Line(lastPoint, imagePosition);
					for (int i = 0; i < points.Count - 1; i++)
					{
						var p1 = points[i];
						var p2 = points[i + 1];
						var line = new Line(p1, p2);

						if (imagePosition != p1 && imagePosition != p2 && line.Intersects(newLine, out intersectPoint))
							return false;
					}
				}
				return true;
			}

			public bool CompleteModification(int w, int h, bool addPoint)
			{
				if (Modifying)
				{
					if (IsPolygon)
					{
						var p = ClampOnImageBorderIntersect(CurrentPosition, Points.LastOrDefault(), w, h);
						p = ClampAbsolute(CurrentPosition, w, h);

						if (p == Points.LastOrDefault())
						{
							addPoint = false;
						}

						if (addPoint)
						{
							if (Points.Count == 1)
							{
								SystemSounds.Beep.Play();
								return false;
							}

							if (!IsPolygonPointValid(p))
							{
								SystemSounds.Beep.Play();
								return false;
							}

							Undo.Push(Clone());
							Points.Add(p);
						}

						if (Points.Count >= 3)
						{
							if (!IsPolygonPointValid(Points.First()))
							{
								if (addPoint)
								{
									Points.RemoveAt(Points.Count - 1);
									Undo.Pop();
								}
								SystemSounds.Beep.Play();
								return false;
							}

							AddGridEntry(new SearchArea
							{
								Include = IsInclude,
								Polygon = Points
									.Select(points => ToRelative(points, w, h))
									.ToList()
							});
							Modifying = false;
							Points.Clear();
							return true;
						}
						else
						{
							SystemSounds.Beep.Play();
							return false;
						}
					}
					else
					{
						bool include = Mode == ToolType.IncludeRect;
						var rect = ToRelative(GetDragedRectangle(), w, h);
						var clamped = Clamp(rect);
						if (clamped.Width > 0 && clamped.Height > 0)
						{
							var newRegion = new SearchArea
							{
								Include = include,
								Rect = clamped
							};

							StartPosition = PointF.Empty;
							Modifying = false;
							AddGridEntry(newRegion);
						}
						return true;
					}
				}
				return false;
			}

			public RectangleF GetDragedRectangle()
			{
				var x = Math.Min(StartPosition.X, CurrentPosition.X);
				var y = Math.Min(StartPosition.Y, CurrentPosition.Y);
				var w = Math.Abs(StartPosition.X - CurrentPosition.X);
				var h = Math.Abs(StartPosition.Y - CurrentPosition.Y);
				return new RectangleF(x, y, w, h);
			}

			public override bool OnMouseLeftDown(PointF imagePosition, int w, int h, Keys modifierKeys, int clickCount)
			{
				base.OnMouseLeftDown(imagePosition, w, h, modifierKeys, clickCount);

				if (IsPolygon)
				{
					imagePosition = ClampOnImageBorderIntersect(imagePosition, Points.LastOrDefault(), w, h);
					imagePosition = ClampAbsolute(imagePosition, w, h);
					if (clickCount > 1) return CompleteModification(w, h, true);
					if (IsPolygonPointValid(imagePosition))
					{
						Undo.Push(Clone());
						Modifying = true;
						Points.Add(imagePosition);
						return true;
					}
					else
					{
						SystemSounds.Beep.Play();
						return false;
					}
				}
				else
				{
					Modifying = true;
					StartPosition = imagePosition;
					return true;
				}

			}

			public override bool OnMouseLeftUp(PointF pointPosition, int w, int h)
			{
				base.OnMouseLeftUp(pointPosition, w, h);

				try
				{
					if (Modifying && !IsPolygon)
					{
						CompleteModification(w, h, true);
					}

					return false;
				}
				finally
				{
					StartPosition = PointF.Empty;
					if (!IsPolygon)
					{
						Modifying = false;
					}
				}
			}

			public override bool OnMouseMove(PointF imagePosition, int w, int h, Keys modifierKeys, MouseButtons mouseButtons)
			{
				base.OnMouseMove(imagePosition, w, h, modifierKeys, mouseButtons);

				CurrentPosition = imagePosition;
				return (mouseButtons & MouseButtons.Left) == MouseButtons.Left || IsPolygon;
			}

			public override Tool Clone()
			{
				return new PolygonSelectionTool(Undo, Mode)
				{
					Grid = Grid.Select(x => x.Clone()).ToList(),
					Modifying = Modifying,
					Points = Points.ToList(),
					CurrentPosition = CurrentPosition,
					RegionMap = null,
				};
			}

			#endregion
		}

		private abstract class Tool
		{
			#region Public constructor

			public Tool(UndoStack undoStack)
			{
				Undo = undoStack ?? throw new ArgumentNullException(nameof(undoStack));
			}

			#endregion

			#region Public properties

			public PointF CurrentPosition { get; set; }
			public UndoStack Undo { get; private set; }
			public List<SearchArea> Grid { get; set; } = new List<SearchArea>();
			public abstract bool IsValid { get; }

			#endregion

			#region Public methods

			public abstract Tool Clone();

			public virtual bool OnMouseMove(PointF imagePosition, int w, int h, Keys modifierKeys, MouseButtons mouseButtons)
			{
				CurrentPosition = imagePosition;
				return (mouseButtons & MouseButtons.Left) == MouseButtons.Left;
			}

			public virtual bool OnMouseLeftDown(PointF imagePosition, int w, int h, Keys modifierKeys, int clickCount)
			{
				return false;
			}

			public virtual bool OnMouseLeftUp(PointF pointPosition, int w, int h)
			{
				return false;
			}

			#endregion
		}

		#endregion

		#region Private fields

		private Thread _thread = null;
		private bool _continue = true;
		private Tool _tool;
		private UndoStack _undoStack = new UndoStack();
		private Bitmap _image = null;

		#endregion

		#region Public constructor

		public SelectRegionOfInterestForm()
		{
			InitializeComponent();
		}

		#endregion

		#region Public properties

		public NSurveillanceSource Source { get; set; }
		public SearchAreaConfig AreaConfig { get; set; }

		#endregion

		#region Private static methods

		private static PointF ToRelative(PointF point, int w, int h)
		{
			return new PointF(point.X / w, point.Y / h);
		}

		private static RectangleF ToRelative(RectangleF rect, int w, int h)
		{
			return new RectangleF(rect.X / w, rect.Y / h, rect.Width / w, rect.Height / h);
		}

		private static RectangleF ToAbsolute(RectangleF rect, int w, int h)
		{
			return new RectangleF(rect.X * w, rect.Y * h, rect.Width * w, rect.Height * h);
		}

		private static PointF ToAbsolute(PointF p, int w, int h)
		{
			return new PointF(p.X * w, p.Y * h);
		}

		private static PointF ClampAbsolute(PointF p, int w, int h)
		{
			return new PointF
			{
				X = Math.Min(w, Math.Max(0, p.X)),
				Y = Math.Min(h, Math.Max(0, p.Y)),
			};
		}

		private static RectangleF Clamp(RectangleF rect)
		{
			var result = rect;
			if (rect.X < 0)
			{
				result.Width += rect.X;
				result.X = 0;
			}
			if (rect.Y < 0)
			{
				result.Height += rect.Y;
				result.Y = 0;
			}
			if (rect.Right > 1)
			{
				result.Width -= rect.Right - 1;
			}
			if (rect.Bottom > 1)
			{
				result.Height -= rect.Bottom - 1;
			}
			return result;
		}

		private static PointF ToImagePosition(Point mouseLocation, float dx, float dy, float scale)
		{
			return new PointF((mouseLocation.X - dx) / scale, (mouseLocation.Y - dy) / scale);
		}

		#endregion

		#region Private methods

		private void SetHint(string text)
		{
			lblHint.Text = text;
		}

		private void OnImageCaptured(Bitmap image)
		{
			_image?.Dispose();
			_image = image;
			panelImage.Invalidate();
		}

		private void CaptureThread()
		{
			var camera = Source.Camera;
			var reader = Source.Video;
			bool stop = false;

			try
			{
				if (camera != null)
				{
					camera.StartCapturing();
					try
					{
						using (var format = Source.GetCurrentFormat())
						{
							if (format != null)
								camera.SetCurrentFormat(format);
						}
					}
					catch
					{
					}
				}
				else
					reader.Start();
				stop = true;

				while (_continue)
				{
					using (var image = camera != null ? camera.GetFrame() : reader.ReadVideoSample())
					{
						if (image != null)
						{
							BeginInvoke(new Action<Bitmap>(OnImageCaptured), image.ToBitmap());
						}
						else
							break;
					}
				}
			}
			catch (Exception ex)
			{
				BeginInvoke(new Action<string>(SetHint), $"Capture failed: {ex.Message}");
			}
			finally
			{
				if (stop)
				{
					if (camera != null)
						camera.StopCapturing();
					else
						reader.Stop();
				}
			}
		}

		private bool GetDimensions(out int w, out int h)
		{
			return GetDimensions(out w, out h, out var _, out var __, out var ___);
		}

		private bool GetDimensions(out int w, out int h, out float scale, out float dx, out float dy)
		{
			w = h = 0;
			dx = dy = scale = float.NaN;

			const int Padding = 20;
			var image = _image;
			if (image != null)
			{
				w = image.Width;
				h = image.Height;
				var scalex = (float)(panelImage.Width - Padding * 2) / w;
				var scaley = (float)(panelImage.Height - Padding * 2) / h;
				scale = Math.Min(scalex, scaley);
				dx = (panelImage.Width - w * scale) / 2.0f;
				dy = (panelImage.Height - h * scale) / 2.0f;
				return true;
			}

			return false;
		}

		private ToolType GetToolType()
		{
			if (rbGridTool.Checked)
				return ToolType.Grid;
			else if (tsbIncludeRect.Checked)
				return ToolType.IncludeRect;
			else if (tsbExcludeRect.Checked)
				return ToolType.ExcludeRect;
			else if (tsbExcludePolygon.Checked)
				return ToolType.ExcludePolygon;
			else if (tsbIncludePolygon.Checked)
				return ToolType.IncludePolygon;

			return ToolType.Grid;
		}

		private void UndoChange()
		{
			var pop = _undoStack.Pop();
			if (pop != null)
			{
				pop.CurrentPosition = _tool.CurrentPosition;
				_tool = pop;
				panelImage.Invalidate();
				Invalidate();
			}
		}

		private void SetToolHint(ToolType value)
		{
			if (value == ToolType.Grid)
			{
				SetHint("Click on rectangles to change if you want to search in specified area" +
					"\nClick while holding CTRL key to split rectangle into smaller pieces.");
			}
			else if (value == ToolType.IncludeRect)
			{
				SetHint("Click and drag to select region where search should be performed.");
			}
			else if (value == ToolType.ExcludeRect)
			{
				SetHint("Click and drag to select region where search should NOT be performed.");
			}
			else if (value == ToolType.IncludePolygon)
			{
				SetHint("Click to select points of polygon region where search should be performed." +
					"\nPress 'Esc', 'Enter' or double click to complete polygon selection.");
			}
			else if (value == ToolType.ExcludePolygon)
			{
				SetHint("Click to select points of polygon region where search should NOT be performed." +
					"\nPress 'Esc', 'Enter' or double click to complete polygon selection.");
			}
		}

		private void SelectTool(ToolType value, Tool tool = null)
		{
			if (tool == null)
			{
				_tool = value == ToolType.Grid ?
					(Tool)new GridTool(_undoStack, Convert.ToInt32(nudRows.Value), Convert.ToInt32(nudColumns.Value)) :
					new PolygonSelectionTool(_undoStack, value);
			}
			else
				_tool = tool;

			_undoStack.Clear();
			panelImage.Cursor = value != ToolType.Grid ? Cursors.Cross : Cursors.Default;

			nudRows.Enabled = value == ToolType.Grid;
			nudColumns.Enabled = value == ToolType.Grid;
			tsbIncludeRect.Enabled = value != ToolType.Grid;
			tsbExcludeRect.Enabled = value != ToolType.Grid;
			tsbIncludePolygon.Enabled = value != ToolType.Grid;
			tsbExcludePolygon.Enabled = value != ToolType.Grid;

			SetToolHint(value);

			panelImage.Invalidate();
			Invalidate();
		}

		private void PaintGrid(Graphics g, GridTool tool, int w, int h)
		{
			using (var greenBrush = new SolidBrush(Color.FromArgb(50, 0, 255, 0)))
			using (var redBrush = new SolidBrush(Color.FromArgb(50, 255, 0, 0)))
			using (var greenActiveBrush = new SolidBrush(Color.FromArgb(90, 0, 255, 0)))
			using (var redActiveBrush = new SolidBrush(Color.FromArgb(90, 255, 0, 0)))
			{
				var pen = Pens.Black;
				foreach (var cell in tool.Grid)
				{
					var rect = ToAbsolute(cell.Rect, w, h);
					if ((ModifierKeys & Keys.Control) == Keys.Control)
					{
						var offsetX = tool.CurrentPosition.X;
						var offsetY = tool.CurrentPosition.Y;
						g.DrawLine(pen, offsetX, rect.Top, offsetX, rect.Bottom);
						g.DrawLine(pen, rect.Left, offsetY, rect.Right, offsetY);
					}

					g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
					if (cell != tool.CurrentRegion)
					{
						g.FillRectangle(cell.Include ? greenBrush : redBrush, rect);
					}
					else
					{
						g.FillRectangle(cell.Include ? greenActiveBrush : redActiveBrush, rect);
					}
				}
			}
		}

		private void PaintPolygons(Graphics g, PolygonSelectionTool tool, int w, int h, float scale)
		{
			var targetWidth = (int)(w * scale);
			var targetHeight = (int)(h * scale);

			var map = tool.RegionMap;
			if (!tool.IsEmpty && (map?.Width != targetWidth || map?.Height != targetHeight))
			{
				tool.RegionMap?.Dispose();
				tool.RegionMap = null;
				map = tool.RegionMap = tool.CreateRegionBitmap(targetWidth, targetHeight);
			}

			if (map != null)
			{
				g.ScaleTransform(1 / scale, 1 / scale);
				g.DrawImage(map, 0, 0);
				g.ScaleTransform(scale, scale);
			}

			using (var greenActiveBrush = new SolidBrush(Color.FromArgb(90, 0, 255, 0)))
			using (var redActiveBrush = new SolidBrush(Color.FromArgb(90, 255, 0, 0)))
			using (var thickPen = new Pen(tool.IsInclude ? Brushes.Green : Brushes.Red, 2))
			{

				if (tool.Modifying)
				{
					if (tool.IsPolygon)
					{
						var points = tool.Points.ToList();
						if (points.Count > 1)
						{
							g.DrawLines(thickPen, points.ToArray());
						}

						if (points.Count > 0)
						{
							var color = tool.IsInclude ? Color.Green : Color.Red;
							using (var validPen = new Pen(color, 5))
							using (var invalidPen = new Pen(Color.DarkRed, 8))
							using (var invalidPenLight = new Pen(Color.DarkRed, 5))
							{
								validPen.DashPattern = new[] { 3.0f, 5.0f };
								invalidPenLight.DashPattern = new[] { 3.0f, 5.0f };

								bool valid = tool.IsPolygonPointValid(tool.CurrentPosition);
								g.DrawLine(valid ? validPen : invalidPen, tool.CurrentPosition, points.Last());
								points.Add(tool.CurrentPosition);

								valid = PolygonSelectionTool.IsPolygonPointValid(points, points.First(), out var _);
								g.DrawLine(valid ? validPen : invalidPenLight, tool.CurrentPosition, tool.Points.First());
							}

							g.FillPolygon(tool.IsInclude ? greenActiveBrush : redActiveBrush, points.ToArray());
						}
					}
					else
					{
						g.FillRectangle(tool.IsInclude ? greenActiveBrush : redActiveBrush, tool.GetDragedRectangle());
					}
				}
			}
		}

		private SearchAreaConfig SaveToolConfig()
		{
			if (!_tool.IsValid)
				return null;

			var results = new List<NSearchArea>();
			foreach (var item in _tool.Grid)
			{
				var areaType = item.Include ? NSearchAreaType.Include : NSearchAreaType.Exclude;
				if (item.Polygon.Any())
				{
					var polygon = new NPolygonSearchArea { Type = areaType };
					foreach (var p in item.Polygon)
					{
						polygon.Points.Add(p);
					}
					results.Add(polygon);
				}
				else
				{
					results.Add(new NRectangleSearchArea { Type = areaType, Rectangle = item.Rect });
				}
			}

			return new SearchAreaConfig
			{
				Areas = results,
				IsGrid = _tool is GridTool,
				Columns = Convert.ToInt32(nudColumns.Value),
				Rows = Convert.ToInt32(nudRows.Value),
				SourceId = Source.Camera?.Id ?? Source.Video.Source.Id,
				CheckSearchAreaByObjectCenter = chbAreaByCenter.Checked
			};
		}

		private Tool LoadToolConfig(SearchAreaConfig config)
		{
			if (config != null)
			{
				var tool = config.IsGrid ? (Tool)new GridTool(_undoStack, config.Rows, config.Columns) : new PolygonSelectionTool(_undoStack, ToolType.IncludeRect);
				tool.Grid.Clear();

				foreach (var item in config.Areas)
				{
					var region = new SearchArea { Include = item.Type == NSearchAreaType.Include };
					if (item is NRectangleSearchArea rectArea)
					{
						region.Rect = rectArea.Rectangle;
					}
					else if (item is NPolygonSearchArea polygonArea)
					{
						region.Polygon = polygonArea.Points.ToList();
					}
					tool.Grid.Add(region);
				}
				return tool;
			}
			return null;
		}

		#endregion

		#region Private form events

		private void SelectRegionOfInterestFormShown(object sender, EventArgs e)
		{
			if (Source == null)
				throw new ArgumentNullException(nameof(Source));

			SetHint("Starting capture, please wait ...");
			_undoStack.StackIsEmptyChanged += (_, __) => btnUndo.Enabled = !_undoStack.IsEmpty;

			if (AreaConfig != null)
			{
				var tool = LoadToolConfig(AreaConfig);
				if (tool is GridTool gridTool)
				{
					rbGridTool.Checked = true;
					nudColumns.Value = AreaConfig.Columns;
					nudRows.Value = AreaConfig.Rows;
					SelectTool(ToolType.Grid, tool);
				}
				else
				{
					rbToolRegions.Checked = true;
					SelectTool(ToolType.IncludeRect, tool);
				}
				chbAreaByCenter.Checked = AreaConfig.CheckSearchAreaByObjectCenter;
			}
			else
			{
				SelectTool(ToolType.Grid);
			}

			_thread = new Thread(CaptureThread);
			_thread.Start();
		}

		private void BtnResetClick(object sender, EventArgs e)
		{
			_undoStack.Clear();
			SelectTool(GetToolType());
			panelImage.Invalidate();
			Invalidate();
		}

		private void SelectRegionOfInterestFormClosing(object sender, FormClosingEventArgs e)
		{
			_continue = false;
			if (_thread != null)
			{
				_thread.Join();
			}
			_image?.Dispose();
		}

		private void PanelImagePaint(object sender, PaintEventArgs e)
		{
			if (_image != null)
			{
				GetDimensions(out var w, out var h, out var scale, out var dx, out var dy);

				var g = e.Graphics;
				g.TranslateTransform(dx, dy);
				g.ScaleTransform(scale, scale);

				g.DrawImage(_image, 0, 0);

				if (_tool is GridTool gridTool)
				{
					PaintGrid(g, gridTool, w, h);
				}
				else if (_tool is PolygonSelectionTool polygonTool)
				{
					PaintPolygons(g, polygonTool, w, h, scale);
				}
			}
		}

		private void PanelImageMouseMove(object sender, MouseEventArgs e)
		{
			if (GetDimensions(out var w, out var h, out var scale, out var dx, out var dy))
			{
				var imagePosition = ToImagePosition(e.Location, dx, dy, scale);
				if (_tool.OnMouseMove(imagePosition, w, h, ModifierKeys, MouseButtons))
				{
					panelImage.Invalidate();
				}
			}
		}

		private void PanelImageMouseUp(object sender, MouseEventArgs e)
		{
			if (GetDimensions(out var w, out var h, out var scale, out var dx, out var dy))
			{
				var imagePosition = ToImagePosition(e.Location, dx, dy, scale);
				if (_tool.OnMouseLeftUp(imagePosition, w, h))
				{
					panelImage.Invalidate();
				}
			}
		}

		private void PanelImageMouseDown(object sender, MouseEventArgs e)
		{
			if (GetDimensions(out var w, out var h, out var scale, out var dx, out var dy))
			{
				var imagePosition = ToImagePosition(e.Location, dx, dy, scale);
				if (_tool.OnMouseLeftDown(imagePosition, w, h, ModifierKeys, e.Clicks))
				{
					Invalidate();
				}
				panelImage.Focus();
			}
		}

		private void RadioBoxSelectionToolCheckedChanged(object sender, EventArgs e)
		{
			SelectTool(GetToolType());
		}

		private void NudGridValueChanged(object sender, EventArgs e)
		{
			SelectTool(ToolType.Grid);
		}

		private void ToolStripButtonClick(object sender, EventArgs e)
		{
			var target = ((ToolStripButton)sender);
			if (target.Checked)
				return;

			tsbExcludeRect.Checked = false;
			tsbExcludePolygon.Checked = false;
			tsbIncludeRect.Checked = false;
			tsbIncludePolygon.Checked = false;

			target.Checked = true;
			if (_tool is PolygonSelectionTool tool)
			{
				tool.Mode = GetToolType();
				SetToolHint(tool.Mode);
			}
		}

		private void BtnUndoClick(object sender, EventArgs e)
		{
			UndoChange();
		}

		private void BtnOkClick(object sender, EventArgs e)
		{
			AreaConfig = SaveToolConfig();
			DialogResult = DialogResult.OK;
		}

		private void PanelImageSizeChanged(object sender, EventArgs e)
		{
			panelImage.Invalidate();
		}

		#endregion

		#region Protected methods

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if ((keyData & Keys.Z) == Keys.Z && (keyData & Keys.Control) == Keys.Control)
			{
				UndoChange();
			}
			else if (rbToolRegions.Checked)
			{
				bool escape = (keyData & Keys.Escape) == Keys.Escape;
				bool enter = (keyData & Keys.Enter) == Keys.Enter;
				if (enter || escape)
				{
					if (_tool is PolygonSelectionTool tool && GetDimensions(out var w, out var h))
					{
						tool.CompleteModification(w, h, enter);
					}
				}
				else
				{
					var key = (Keys)((int)keyData & 0x0FFFF);
					switch (key)
					{
						case Keys.D1:
						case Keys.NumPad1:
							tsbIncludeRect.PerformClick();
							break;
						case Keys.D2:
						case Keys.NumPad2:
							tsbExcludeRect.PerformClick();
							break;
						case Keys.D3:
						case Keys.NumPad3:
							tsbIncludePolygon.PerformClick();
							break;
						case Keys.D4:
						case Keys.NumPad4:
							tsbExcludePolygon.PerformClick();
							break;
						default:
							break;
					};
				}
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		#endregion
	}
}
