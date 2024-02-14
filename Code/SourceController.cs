using Neurotec.Media;
using Neurotec.Surveillance;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Neurotec.Samples.Code
{
	public class SourceController: INotifyPropertyChanged, IDisposable
	{
		#region Private fields

		private bool _canShow;
		private bool _canHide;
		private bool _canStart;
		private bool _canStop;
		private bool _canChangeProperties;
		private bool _isSelected;
		private bool _replay;
		private int _replayTimeout = -1;
		private CancellationTokenSource _cancellationSource;
		private Task _retryTask;
		private NSurveillanceModalityType _modalityType = NSurveillanceModalityType.VehiclesAndHumans | NSurveillanceModalityType.LicensePlateRecognition;
		private bool _showSearchArea = true;
		private string _preset;

		#endregion

		#region Public constructor

		public SourceController(NSurveillanceSource source, NSurveillanceModalityType supportedModalities)
		{
			this.Source = source;
			source.PropertyChanged += OnSourcePropertyChanged;
			this.SupportedModalities = supportedModalities;
			ModalityType = source.ModalityType;

			var camera = source.Camera;
			if (camera?.Plugin.ToString() == "Onvif")
			{
				CanChangePassword = true;
			}
		}

		#endregion

		#region Public events

		public event EventHandler Start;
		public event EventHandler Stop;
		public event EventHandler Show;
		public event EventHandler Hide;
		public event EventHandler<NMediaFormat> ChangeFormat;
		public event EventHandler<SearchAreaConfig> ChangeSearchArea;
		public event EventHandler<string> ChangePreset;
		public event EventHandler Select;
		public event EventHandler Stopped;

		#endregion

		#region Public properties

		public NSurveillanceSource Source { get; private set; }

		public bool CanChangePassword { get; private set; }

		public NSurveillanceModalityType SupportedModalities { get; private set; } = NSurveillanceModalityType.Faces;

		public NMediaFormat[] Formats { get; set; }

		public NMediaFormat SelectedFormat { get; set; }

		public bool CanShow
		{
			get { return _canShow; }
			set { SetProperty(ref _canShow, value); }
		}

		public bool CanHide
		{
			get { return _canHide; }
			set { SetProperty(ref _canHide, value); }
		}

		public bool CanStart
		{
			get { return _canStart; }
			set { SetProperty(ref _canStart, value); }
		}

		public bool CanStop
		{
			get { return _canStop; }
			set { SetProperty(ref _canStop, value); }
		}

		public bool CanChangeProperties
		{
			get { return _canChangeProperties; }
			set { SetProperty(ref _canChangeProperties, value); }
		}

		public NSurveillanceModalityType ModalityType
		{
			get { return _modalityType; }
			set
			{
				if ((value & ~SupportedModalities) != 0)
					throw new ArgumentOutOfRangeException("Unsupported value");
				SetProperty(ref _modalityType, value);
			}
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			private set { SetProperty(ref _isSelected, value); }
		}

		public bool Replay
		{
			get { return _replay; }
			set
			{
				if (SetProperty(ref _replay, value))
				{
					if (!value)
					{
						ReplayTimeout = -1;
					}
				}
			}
		}

		public int ReplayTimeout
		{
			get { return _replayTimeout; }
			set { SetProperty(ref _replayTimeout, value); }
		}

		public CancellationTokenSource CancellationSource
		{
			get { return _cancellationSource; }
			set { SetProperty(ref _cancellationSource, value); }
		}

		public Task RetryTask
		{
			get { return _retryTask; }
			set { SetProperty(ref _retryTask, value); }
		}

		public bool ShowSearchArea
		{
			get => _showSearchArea;
			set => SetProperty(ref _showSearchArea, value);
		}

		public string SelectedPreset
		{
			get => _preset;
			set => SetProperty(ref _preset, value);
		}

		public string SourceId { get => Source.Camera?.Id ?? Source.Video.Source.Id; }

		#endregion

		#region Public methods

		public void OnStart()
		{
			if (CanStart)
				Start?.Invoke(this, EventArgs.Empty);
		}

		public void OnStop()
		{
			if (CanStop)
				Stop?.Invoke(this, EventArgs.Empty);
		}

		public void OnShow()
		{
			if (CanShow)
				Show?.Invoke(this, EventArgs.Empty);
		}

		public void OnHide()
		{
			if (CanHide)
				Hide?.Invoke(this, EventArgs.Empty);
		}

		public void OnChangeFormat(NMediaFormat format)
		{
			if (CanChangeProperties)
				ChangeFormat?.Invoke(this, format);
		}

		public void OnChangeSearchArea(SearchAreaConfig value)
		{
			if (CanChangeProperties)
				ChangeSearchArea?.Invoke(this, value);
		}

		public void OnChangePreset(string presetGUID)
		{
			if (CanChangeProperties)
				ChangePreset?.Invoke(this, presetGUID);
		}

		public void SelectSource()
		{
			if (!IsSelected)
			{
				IsSelected = true;
				Select?.Invoke(this, EventArgs.Empty);
			}
		}

		public void UnselectSource()
		{
			IsSelected = false;
		}

		#endregion

		#region Private methods

		private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "State" && Source.State == NMediaState.Stopped)
			{
				CanStart = true;
				CanStop = false;
				CanChangeProperties = true;

				Stopped?.Invoke(this, EventArgs.Empty);
			}
		}

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		private bool SetProperty<T>(ref T value, T newValue, [CallerMemberName] string propertyName = null)
		{
			if (!object.Equals(value, newValue))
			{
				value = newValue;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
				return true;
			}

			return false;
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (Source != null)
				Source.PropertyChanged -= OnSourcePropertyChanged;
		}

		#endregion
	}
}
