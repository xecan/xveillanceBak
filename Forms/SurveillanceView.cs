using Neurotec.Images;
using Neurotec.IO;
using Neurotec.Media;
using Neurotec.Samples.Code;
using Neurotec.Surveillance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neurotec.Samples.Forms
{
	public partial class SurveillanceView : Control, ISurveillanceView
	{
		#region Private constants

		private const uint SrcCopy = 0x00CC0020;
		private const int StretchHalftone = 4;

		#endregion

		#region External code

		[DllImport("gdi32")]
		private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		[DllImport("gdi32")]
		private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32")]
		private static extern bool DeleteObject(IntPtr hObject);

		[DllImport("gdi32")]
		private static extern bool DeleteDC(IntPtr hdc);

		[DllImport("gdi32")]
		static extern bool StretchBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, uint rop);

		[DllImport("gdi32")]
		private static extern bool SetStretchBltMode(IntPtr hdc, int stretchMode);

		#endregion

		#region Private fields

		private NImage _image = null;
		private NVideoSample _sample = null;
		private IntPtr _bmp = IntPtr.Zero;
		private int _imageWidth, _imageHeight;
		private Task<Tuple<IntPtr, int, int>> _convertTask = null;

		private SourceController _state;
		private string _sourceName = string.Empty;
		private string _errorMessage = string.Empty;
		private NMediaState _mediaState = NMediaState.Running;
		private bool _showSearchArea = true;
		private Queue<DateTime> _framesInTime = new Queue<DateTime>();
		private Queue<DateTime> _framesInRealTime = new Queue<DateTime>();

		private Brush _runningBrush = Brushes.GreenYellow;
		private Brush _pendingBrush = Brushes.Black;
		private Brush _errorBrush = Brushes.Red;
		private Font _boldFont;
		private Color _regionColor = Color.FromArgb(50, 0, 255, 0);

		private Bitmap _searchAreaOverlay = null;
		private Bitmap _searchAreaOverlayFull = null;

		#endregion

		#region Public constructor

		public SurveillanceView()
		{
			InitializeComponent();

			DoubleBuffered = true;
		}

		#endregion

		#region Public properties

		public SourceController State
		{
			get { return _state; }
			set
			{
				if (_state != value)
				{
					if (_state != null)
					{
						_state.Source.PropertyChanged -= SourcePropertyChanged;
						_state.PropertyChanged -= SourceStatePropertyChanged;
					}

					_state = value;
					ResetSearchArea();
					if (_state != null)
					{
						_state.PropertyChanged += SourceStatePropertyChanged;
						_state.Source.PropertyChanged += SourcePropertyChanged;
						_sourceName = _state.Source.Camera?.DisplayName ?? _state.Source.Video.Source.ToString();

						_mediaState = _state.Source.State;
						_showSearchArea = _state.ShowSearchArea;
					}

					Reset();
				}
			}
		}

		public double FpsMeasureTime { get; set; } = 3;

		#endregion

		#region Protected methods

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (DesignMode) return;

			float scalex = 1, scaley = 1, scale = 1;
			float dx = 0, dy = 0;

			TryTakeImage(_bmp == IntPtr.Zero);

			var g = e.Graphics;

			bool withImage = _bmp != IntPtr.Zero;
			if (withImage)
			{
				scalex = (float)Width / _imageWidth;
				scaley = (float)Height / _imageHeight;
				scale = Math.Min(scalex, scaley);
				dx = (float)Math.Round((Width - _imageWidth * scale) / 2.0f);
				dy = (float)Math.Round((Height - _imageHeight * scale) / 2.0f);

				var dc = g.GetHdc();
				var srcDc = CreateCompatibleDC(dc);
				var obj = SelectObject(srcDc, _bmp);

				SetStretchBltMode(dc, StretchHalftone);

				StretchBlt(dc, (int)dx, (int)dy, (int)(Width - 2 * dx), (int)(Height - 2 * dy), srcDc, 0, 0, _imageWidth, _imageHeight, SrcCopy);

				DeleteObject(obj);
				DeleteDC(srcDc);

				g.ReleaseHdc();

				PaintSearchArea(g, dx, dy, scale);
			}

			PaintTextOverlay(g, dx, dy);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			Invalidate();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);

			State = null;
			_image?.Dispose();
			_sample?.Dispose();
			DeleteAndZeroBitmap(ref _bmp);
		}

		#endregion

		#region Private static methods

		private static Bitmap ScaleBitmap(Bitmap target, int width, int height)
		{
			var dstImage = new Bitmap(width, height);
			dstImage.SetResolution(target.HorizontalResolution, target.VerticalResolution);
			using (var graphics = Graphics.FromImage(dstImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(target, new Rectangle(0, 0, width, height), 0, 0, target.Width, target.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}
			return dstImage;
		}

		#endregion

		#region Private methods

		private void PaintSearchArea(Graphics g, float dx, float dy, float scale)
		{
			if (_showSearchArea && State.Source.SearchArea.Count > 0)
			{
				if (_searchAreaOverlayFull == null)
				{
					using (var mask = State.Source.GetSearchAreaMask(_imageWidth, _imageHeight))
					{
						var bytes = mask.ToArray();
						var rgba = new byte[4 * bytes.Length];
						for (int i = 0; i < bytes.Length; i++)
						{
							bool set = bytes[i] > 0;
							rgba[i * 4 + 0] = (byte)(set ? 0 : 255);
							rgba[i * 4 + 1] = (byte)(set ? 255 : 0);
							rgba[i * 4 + 2] = 0;
							rgba[i * 4 + 3] = 30;
						}

						using (var data = new NBuffer(rgba))
						using (var roiImage = NImage.FromData(NPixelFormat.RgbA8U, (uint)_imageWidth, (uint)_imageHeight, 0, (uint)(4 * _imageWidth), data))
						{
							_searchAreaOverlayFull = roiImage.ToBitmap();
						}
					}
				}

				var targetWidth = (int)(scale * _imageWidth);
				var targetHeight = (int)(scale * _imageHeight);
				if (_searchAreaOverlay == null || _searchAreaOverlay.Width != targetWidth || _searchAreaOverlay.Height != targetHeight)
				{
					_searchAreaOverlay?.Dispose();
					_searchAreaOverlay = null;

					if (targetWidth != _imageWidth || targetHeight != _imageHeight)
					{
						_searchAreaOverlay = ScaleBitmap(_searchAreaOverlayFull, targetWidth, targetHeight);
					}
					else
					{
						_searchAreaOverlay = (Bitmap)_searchAreaOverlayFull.Clone();
					}
				}

				if (_searchAreaOverlay != null)
				{
					g.DrawImage(_searchAreaOverlay, dx, dy);
				}
			}
		}

		private void PaintTextOverlay(Graphics g, float dx, float dy)
		{
			bool isRunning = _mediaState == NMediaState.Running;
			bool withImage = _bmp != IntPtr.Zero;
			var b = GetBrush(isRunning, withImage);
			var f = GetFont();
			var prefix = string.Empty;
			if (isRunning)
			{
				prefix = _framesInTime.Count > 0 ? string.Format("{0:f0} / {1:f0} fps", _framesInRealTime.Count / FpsMeasureTime, _framesInTime.Count / FpsMeasureTime) : "Waiting for frames";
			}
			else if (!string.IsNullOrEmpty(_errorMessage))
			{
				prefix = $"Stopped (Error: {_errorMessage})";
			}
			else
			{
				prefix = $"Stopped";
			}
			var text = $"{prefix} - {_sourceName}";
			if (withImage)
			{
				g.DrawString(text, f, b, new Point(5 + (int)dx, 5 + (int)dy));
			}
			else
			{
				var sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
				g.DrawString(text, f, b, new RectangleF(0, 0, Width, Height), sf);
			}
		}

		private void TryTakeImage(bool force)
		{
			if (force || _convertTask?.IsCompleted == true)
			{
				DeleteAndZeroBitmap(ref _bmp);
				if (_convertTask != null)
				{
					var result = _convertTask.Result;
					_bmp = result.Item1;
					_imageWidth = result.Item2;
					_imageHeight = result.Item3;
					_convertTask = null;
				}
			}

			if (_convertTask == null && (_image != null || _sample != null))
			{
				_convertTask = ConvertAndInvalidate();
			}
		}

		private Task<Tuple<IntPtr, int, int>> ConvertAndInvalidate()
		{
			Task<Tuple<IntPtr, int, int>> task = null;

			if (_image != null)
			{
				var image = _image;
				_image = null;

				task = Task.Run(() =>
				{
					using (image)
					{
						return Tuple.Create(image.ToHBitmap(), (int)image.Width, (int)image.Height);
					}
				});
			}
			else if (_sample != null)
			{
				var sample = _sample;
				_sample = null;

				task = Task.Run(() =>
				{
					using (var image = sample.ToImage())
					{
						sample.Dispose();
						return Tuple.Create(image.ToHBitmap(), (int)image.Width, (int)image.Height);
					}
				});
			}

			if(task != null)
			{
				task.ContinueWith(_ =>
				{
					BeginInvoke(new Action(() =>
					{
						if (IsHandleCreated)
							Invalidate();
					}));
				});

				return task;
			}

			return null;
		}

		private Font GetFont()
		{
			return _boldFont ?? (_boldFont = new Font(Font.FontFamily, 12, FontStyle.Bold));
		}

		private Brush GetBrush(bool isRunning, bool withImage)
		{
			if (isRunning)
				return withImage ? _runningBrush : _pendingBrush;
			else
				return _errorBrush;
		}

		private void DeleteAndZeroBitmap(ref IntPtr hBitmap)
		{
			if (hBitmap != IntPtr.Zero)
			{
				IntPtr handle = hBitmap;
				hBitmap = IntPtr.Zero;
				Task.Run(() => DeleteObject(handle));
			}
		}

		private void Reset()
		{
			_framesInRealTime.Clear();
			_framesInTime.Clear();
			_image?.Dispose();
			_image = null;
			_sample?.Dispose();
			_sample = null;
			ResetSearchArea();
			DeleteAndZeroBitmap(ref _bmp);
			if (_convertTask != null)
			{
				var result = _convertTask.Result.Item1;
				DeleteAndZeroBitmap(ref result);
				_convertTask = null;
			}
			Invalidate();
		}

		private void ResetSearchArea()
		{
			_searchAreaOverlay?.Dispose();
			_searchAreaOverlay = null;
			_searchAreaOverlayFull?.Dispose();
			_searchAreaOverlayFull = null;
		}

		private void SourceStatePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ShowSearchArea")
			{
				_showSearchArea = _state.ShowSearchArea;
				Invalidate();
			}
		}

		private void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (IsHandleCreated)
			{
				if (e.PropertyName == "State")
				{
					BeginInvoke(new Action<NMediaState>(state =>
					{
						_mediaState = state;
						if (state == NMediaState.Running)
						{
							ResetSearchArea();
						}

						Reset();
					}), ((NSurveillanceSource)sender).State);
				}
				else if (e.PropertyName == "Error")
				{
					BeginInvoke(new Action<Exception>(ex =>
					{
						_errorMessage = ex?.Message;
						Invalidate();
					}), ((NSurveillanceSource)sender).Error);
				}
			}
		}

		private void SurveillanceViewMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Clicks == 1)
			{
				if (e.Button == MouseButtons.Left && State != null)
				{
					DoDragDrop(State, DragDropEffects.Move | DragDropEffects.Link);
					State.SelectSource();
				}
			}
		}

		#endregion

		#region Public methods

		public void SetCaptureEventData(CaptureEventData data)
		{
			var now = DateTime.Now.ToUniversalTime();

			_framesInTime.Enqueue(data.TimeStamp);
			_framesInRealTime.Enqueue(now);

			for (; ; )
			{
				if ((data.TimeStamp - _framesInTime.Peek()).TotalSeconds > FpsMeasureTime)
					_framesInTime.Dequeue();
				else
					break;
			}

			for (; ; )
			{
				if ((now - _framesInRealTime.Peek()).TotalSeconds > FpsMeasureTime)
					_framesInRealTime.Dequeue();
				else
					break;
			}

			if (_sample != null)
			{
				_sample.Dispose();
				_sample = null;
			}

			if (_image != null)
			{
				_image.Dispose();
				_image = null;
			}

			if (data.Image != null)
			{
				_image = data.Image;
				data.Image = null;
			} 
			else if (data.Sample != null)
			{
				_sample = data.Sample;
				data.Sample = null;
			}

			if (_convertTask == null)
			{
				_convertTask = ConvertAndInvalidate();
			}
			
		}

		#endregion
	}
}
