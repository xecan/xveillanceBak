using Neurotec.Devices;
using Neurotec.Gui;
using Neurotec.Images;
using Neurotec.Licensing;
using Neurotec.Media;
using Neurotec.Samples.Code;
using Neurotec.Surveillance;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Neurotec.Samples.Forms
{
    public partial class MainForm : Form
    {
        #region Private fields

        private volatile bool _isRunning;
        private int _maxCameraCount = 10;
        private bool _withALPR = true;
        private bool _withVehicleHuman = true;
        private bool _withFaces = true;
        private bool _saveEvents = true;
        private bool _startOnStartup = true;
        private string _capturedUserName = "";
        private NSurveillanceModalityType _modelOptimized = NSurveillanceModalityType.None;

        private NSurveillance _surveillance;
        private FaceRecordCollection _faceRecordsCollection;
        private LicensePlateCollection _licensePlateCollection;
        private CellPhoneCollection _cellPhoneCollection;
        private RecordCollection _recordCollection;
        public StoreCollection Store { get; set; }

        private List<SourceController> _states = new List<SourceController>();

        #endregion

        #region Public constructors

        public MainForm() : this(null)
        {

        }

        public MainForm(string[] args)
        {
            InitializeComponent();
            MenuStrip toolStrip1 = new MenuStrip();
            toolStrip1.RenderMode = ToolStripRenderMode.ManagerRenderMode;


            var store = new StoreCollection();
            Store = store;
            DoubleBuffered = true;

            _startOnStartup = args?.Any(x => x.Trim().Equals("--start")) == true;


        }

        #endregion

        #region Private surveillance events

        private void SurveillanceIsRunningChanged(object sender, EventArgs e)
        {
            BeginInvoke(new Action(OnIsRunningChanged));
        }

        private void ForwardSurveillanceEvent(NSurveillanceEventArgs e, Action<EventData[]> target)
        {
            try
            {
                NSurveillanceEventDetails[] details = e.EventDetailsArray;
                EventData[] data = new EventData[details.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = new EventData(details[i], e.EventType);
                    details[i].Dispose();
                }
                BeginInvoke(target, (object)data);
            }
            catch (Exception ex)
            {
                BeginInvoke(new Action<Exception>(error => Utils.ShowException(error)), ex);
            }
        }

        private void SurveillanceImageCaptured(object sender, NSurveillanceEventArgs e)
        {
            try
            {
                NSurveillanceEventDetails[] details = e.EventDetailsArray;
                CaptureEventData[] data = new CaptureEventData[details.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = new CaptureEventData(details[i]);
                    details[i].Dispose();
                }

                BeginInvoke((Action<CaptureEventData[]>)OnImageCaptured, (object)data);
            }
            catch (Exception ex)
            {
                RestartApplication();
                BeginInvoke(new Action<Exception>(error => Utils.ShowException(error)), ex);
            }
        }
        public void RestartApplication()
        {
            Process[] runningProcesses = Process.GetProcesses();
            var process = runningProcesses.Where(x => x.ProcessName == "XveillanceBackground").ToList();
            //if background camera active then close first
            if (process.Count > 0)
            {
                string exePath = ConfigurationManager.AppSettings["XveillanceBackground"];
                Process.Start(exePath);
                process[0].Kill(); 
            }
        }
        private void SurveillanceSubjectAppeared(object sender, NSurveillanceEventArgs e)
        {
            ForwardSurveillanceEvent(e, OnSubjectAppeared);
        }

        private void SurveillanceSubjectTrack(object sender, NSurveillanceEventArgs e)
        {
            ForwardSurveillanceEvent(e, OnSubjectTrack);
        }

        private void SurveillanceSubjectDisappeared(object sender, NSurveillanceEventArgs e)
        {
            ForwardSurveillanceEvent(e, OnSubjectDisappeared);
        }

        private void SurveillanceSubjectMerged(object sender, NSurveillanceEventArgs e)
        {
            ForwardSurveillanceEvent(e, OnSubjectMerged);
        }

        private void SurveillanceSubjectSplit(object sender, NSurveillanceEventArgs e)
        {
            ForwardSurveillanceEvent(e, OnSubjectSplit);
        }

        private void OnIsRunningChanged()
        {
            _isRunning = _surveillance.IsRunning;
            Text = string.Format("SentiVeillance SDK Sample - {0}", (_isRunning ? "Running" : "Stopped"));
        }

        private void OnImageCaptured(CaptureEventData[] dataArray)
        {
            foreach (var data in dataArray)
            {
                if (data.Error != null)
                {
                    RestartApplication();
                    MessageBox.Show(string.Format("Error occurred while capturing from source #{0}: {1}", data.Source, data.Error), "Capture error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    var view = viewsPanel.GetView(data.Source);
                    if (view != null)
                    {
                        view.SetCaptureEventData(data);
                    }
                }

                data.Dispose();
            }
            Invalidate();
        }

        private string GetSubjectStatus(SubjectInfo si)
        {
            return si.Face.SubjectId == null ? "Unknown" : $"{si.Face.SubjectId} ({si.Face.Score})";
        }

        private void OnSubjectAppeared(EventData[] dataArray)
        {
            var added = new List<SubjectInfo>(dataArray.Length);

            foreach (var data in dataArray)
            {
                try
                {
                    SubjectInfo si = data.CreateSubjectInfo();
                    CheckUpdateGalleryReferences(si, null, null);
                    added.Add(si);
                }
                catch (Exception ex)
                {
                    RestartApplication();
                    MessageBox.Show(string.Format("Error while processing event. Details: {0}", ex.Message));
                }
                finally
                {
                    data.Dispose();
                }
            }

            subjectsView.AddSubjects(added);
            added.Clear();
        }

        private void OnSubjectTrack(EventData[] dataArray)
        {
            var updated = new List<SubjectInfo>(dataArray.Length);
            foreach (var data in dataArray)
            {
                try
                {
                    if (data.Error != null)
                    {
                        MessageBox.Show(string.Format("Error occurred while tracking subject from source #{0}: {1}", data.Source, data.Error), "Tracking error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        SubjectInfo si = subjectsView.GetSubject(data.TraceIndex);
                        if (si != null)
                        {
                            var lastFaceId = si.Face.SubjectId;
                            var lastPlateValue = si.LicensePlate.Best?.Value;
                            data.UpdatedSubjectInfo(si);
                            CheckUpdateGalleryReferences(si, lastFaceId, lastPlateValue);
                            updated.Add(si);

                            if (si.TraceIndex == detailsView.Info?.TraceIndex)
                            {
                                detailsView.Info = si;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Error while processing event. Details: {0}", ex.Message));
                }
                finally
                {
                    data.Dispose();
                }
            }

            subjectsView.UpdateSubjects(updated);
            updated.Clear();
        }

        private void OnSubjectSplit(EventData[] dataArray)
        {
            var added = new List<SubjectInfo>();
            var updated = new List<SubjectInfo>();

            foreach (var data in dataArray)
            {
                try
                {
                    if (data.Error != null)
                    {
                        MessageBox.Show(string.Format("Error occurred while tracking subject from source #{0}: {1}", data.Source, data.Error), "Tracking error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        SubjectInfo si = data.CreateSubjectInfo();
                        CheckUpdateGalleryReferences(si, null, null);
                        added.Add(si);

                        var splitFrom = data.SplitFrom;
                        si = subjectsView.GetSubject(splitFrom.TraceIndex);
                        if (si != null)
                        {
                            splitFrom.UpdatedSubjectInfo(si);
                            CheckUpdateGalleryReferences(si, null, null);
                            updated.Add(si);
                            if (si.TraceIndex == detailsView.Info?.TraceIndex)
                            {
                                detailsView.Info = si;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Error while processing event. Details: {0}", ex.Message));
                }
                finally
                {
                    data.Dispose();
                }
            }

            subjectsView.AddSubjects(added, false);
            subjectsView.UpdateSubjects(updated);
            added.Clear();
            updated.Clear();
        }

        private void OnSubjectMerged(EventData[] dataArray)
        {
            var deleted = new List<SubjectInfo>();
            var updated = new List<SubjectInfo>();
            foreach (var data in dataArray)
            {
                try
                {
                    if (data.Error != null)
                    {
                        MessageBox.Show(string.Format("Error occurred while tracking subject from source #{0}: {1}", data.Source, data.Error), "Tracking error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        SubjectInfo si = subjectsView.GetSubject(data.TraceIndex);
                        if (si != null)
                        {
                            var lastFaceId = si.Face.SubjectId;
                            var lastPlateValue = si.LicensePlate.Best?.Value;
                            data.UpdatedSubjectInfo(si);
                            CheckUpdateGalleryReferences(si, lastFaceId, lastPlateValue);
                            updated.Add(si);
                            if (si.TraceIndex == detailsView.Info?.TraceIndex)
                            {
                                detailsView.Info = si;
                            }

                            si = subjectsView.GetSubject(data.MergedSubject.TraceIndex);
                            if (si != null)
                            {
                                deleted.Add(si);
                                if (si.TraceIndex == detailsView.Info?.TraceIndex)
                                    detailsView.Info = null;
                            }
                        }
                    }
                }
                finally
                {
                    data.Dispose();
                }
            }

            subjectsView.UpdateSubjects(updated);
            subjectsView.DeleteSubjects(deleted);
        }

        private async void OnSubjectDisappeared(EventData[] dataArray)
        {
            var disappeared = new List<SubjectInfo>();

            foreach (var data in dataArray)
            {
                try
                {
                    SubjectInfo si = subjectsView.GetSubject(data.TraceIndex);
                    if (si != null)
                    {
                        var lastFaceId = si.Face.SubjectId;
                        var lastPlateValue = si.LicensePlate.Best?.Value;
                        data.UpdatedSubjectInfo(si);
                        CheckUpdateGalleryReferences(si, lastFaceId, lastPlateValue);
                        disappeared.Add(si);
                        if (si.TraceIndex == detailsView.Info?.TraceIndex)
                        {
                            detailsView.Info = si;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Error while processing event. Details: {0}", ex.Message));
                }
                finally
                {
                    data.Dispose();
                }
            }

            if (_saveEvents)
            {
                var tasks = new List<Task>(disappeared.Count);
                foreach (var data in disappeared)
                {
                    NImage fullImage = null;
                    // Uncomment to save full image to db
                    // fullImage = (NImage)data.BestImage.Clone();
                    var objectThumb = !data.Object.IsEmpty ? (Bitmap)data.Object?.Thumbnail?.Clone() : null;
                    var licenseThumb = (Bitmap)data.LicensePlate.Thumbnail?.Clone();
                    var faceThumb = (Bitmap)data.Face.Thumbnail?.Clone();
                    var record = Record.CreateRecord(data, fullImage, objectThumb, licenseThumb, faceThumb);
                    tasks.Add(Task.Run(() => _recordCollection.AddAsync(record)));
                }
                subjectsView.UpdateSubjects(disappeared);
                disappeared.Clear();
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                subjectsView.UpdateSubjects(disappeared);
                disappeared.Clear();
            }
        }
        private void SendSMSToAllCellPhones(SubjectInfo si)
        {
            var cellPhones = _cellPhoneCollection.ToList();
            if (cellPhones.Count > 0)
            {
                var accountSid = ConfigurationManager.AppSettings["AccountSID"]; // fetching from the app config
                var authToken = ConfigurationManager.AppSettings["AuthToken"]; // fetching from the app config
                TwilioClient.Init(accountSid, authToken);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                foreach (var cell in cellPhones)
                {
                    try
                    {
                        var message = MessageResource.Create(
          body: "Xvillance Alert : " + si.Face.SubjectId + " you identified on " + DateTime.Now,
                    from: new Twilio.Types.PhoneNumber(ConfigurationManager.AppSettings["From"]),
          to: new Twilio.Types.PhoneNumber(cell.Value)
      );
                    }
                    catch (Exception ex)
                    {

                    }
                }
                _capturedUserName = si.Face.SubjectId; // stored subject name
            }
        }
        private void CheckUpdateGalleryReferences(SubjectInfo si, string lastFaceId, string lastPlateValue)
        {
            if (lastFaceId != si.Face.SubjectId)
            {
                si.Face.GalleryThumbnail?.Dispose();
                si.Face.GalleryThumbnail = null;
                if (si.Face.SubjectId != null)
                {
                    if (_faceRecordsCollection.TryGetValue(si.Face.SubjectId, out var rec))
                    {
                        using (var image = _faceRecordsCollection.GetThumbnailById(rec.Id))
                        {
                            si.Face.GalleryThumbnail = image.ToBitmap();
                        }
                    }
                    else
                    {
                        si.Face.GalleryThumbnail = (Bitmap)Properties.Resources.Unknown.Clone();
                    }
                }
            }
            si.Face.StatusText = GetSubjectStatus(si);

            //when face detected 
            if (si.Face.StatusText != "Unknown")
            {
                // Verifying whether the SMS has been sent to the user or not.
                if (si.Face.SubjectId != _capturedUserName)
                {
                    // Send sms to all cell phones
                    SendSMSToAllCellPhones(si);
                }
            }

            var lp = si.LicensePlate;
            if (!lp.IsEmpty)
            {
                if (lastPlateValue != lp.Best?.Value)
                {
                    lp.Watchlist = _licensePlateCollection[lp.Best.Value] ??
                    _licensePlateCollection[lp.Best.FormattedValue];
                }
            }
            else
            {
                lp.Watchlist = null;
            }
        }

        private async Task<bool> RemoveVideo(SourceController state, bool confirm)
        {
            if (state != null && state.Source.Video != null)
            {
                if (confirm &&
                    DialogResult.Yes != MessageBox.Show(this, "You are about to remove video source, continue?", "Remove source?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                {
                    return false;
                }

                await OnRemoveSource(state);
                EnableSourceControls();
                return true;
            }
            return false;
        }

        #endregion

        #region Private form events

        private void MainFormLoad(object sender, EventArgs e)
        {
            openFileDialog.Filter = NImages.GetOpenFileFilterString(true, true);
            aboutToolStripMenuItem.Text = '&' + AboutBox.Name;

            if (!DesignMode)
            {
                _withALPR = NLicense.IsComponentActivated("Sentiveillance.ALPR");
                _withVehicleHuman = NLicense.IsComponentActivated("Sentiveillance.VH");
                _withFaces = NLicense.IsComponentActivated("SentiVeillance");
            }

            if (!_withVehicleHuman)
            {
                tlpSubjects.RowStyles[1].SizeType = SizeType.Absolute;
                tlpSubjects.RowStyles[1].Height = 0;
            }

            ShowSourcesPanel(true);

            LayoutDefinition.Setup(_maxCameraCount);
            SwitchOrientation(SurveillanceConfig.Orientation);
            ShowDetailsView(SurveillanceConfig.ShowDetails);

            var name = SurveillanceConfig.Layout;
            var layouts = LayoutDefinition.GetLayouts().ToArray();
            var targetSize = _maxCameraCount > 4 ? 4 : _maxCameraCount;
            var selected = layouts.FirstOrDefault(x => x.Count == targetSize) ?? layouts.First();
            foreach (var layout in layouts)
            {
                var item = new ToolStripMenuItem(layout.Name, layout.Icon) { Tag = layout };
                toolLayout.DropDownItems.Add(item);

                if (layout.Name == name)
                {
                    selected = layout;
                }
            }

            SelectLayout(selected);
        }

        private static void ResetSettings(NSurveillance surveillance)
        {
            var specified = NPropertyBag.Parse(SurveillanceConfig.SurveillanceProperties);
            var current = new NPropertyBag();
            surveillance.CaptureProperties(current);
            specified.ApplyTo(surveillance);
            surveillance.CaptureProperties(current);
            foreach (var prop in current.Where(x => !specified.ContainsKey(x.Key)).ToArray())
            {
                if (prop.Key != "LastTrackId")
                    surveillance.ResetProperty(prop.Key);
            }
        }

        private NSurveillanceModalityType GetModalityType()
        {
            NSurveillanceModalityType value = NSurveillanceModalityType.None;
            if (_withALPR) value |= NSurveillanceModalityType.LicensePlateRecognition;
            if (_withVehicleHuman) value |= NSurveillanceModalityType.VehiclesAndHumans;
            if (_withFaces) value |= NSurveillanceModalityType.Faces;
            return value;
        }

        private async void MainFormShown(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // get connectionstring from appconfig
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
                var values = (Tuple<NSurveillance, FaceRecordCollection, RecordCollection, LicensePlateCollection, CellPhoneCollection>)LongActionDialog.ShowDialog(this, "Creating surveillance object", new Func<object>(() =>
                 {
                     // License Plate Watch List
                     var licensePlates = new LicensePlateCollection(connectionString);

                     // Cell Phone Watch List

                     var cellPhone = new CellPhoneCollection(connectionString);

                     // Records  Watch List
                     var records = new RecordCollection(connectionString);
                     records.ClearWeekOld();

                     var surveillance = new NSurveillance();
                     ResetSettings(surveillance);

                     // Faces  Watch List
                     var faces = new FaceRecordCollection(connectionString);
                     foreach (FaceRecord faceRecord in faces)
                     {
                         surveillance.AddTemplate(faceRecord.FaceId, faceRecord.Features);
                     }

                     return new Tuple<NSurveillance, FaceRecordCollection, RecordCollection, LicensePlateCollection, CellPhoneCollection>(surveillance, faces, records, licensePlates, cellPhone);
                 }));

                _surveillance = values.Item1;
                _faceRecordsCollection = values.Item2;
                _recordCollection = values.Item3;
                _licensePlateCollection = values.Item4;
                _cellPhoneCollection = values.Item5;

                subjectsView.MaxNodes = SurveillanceConfig.MiscMaxTreeNodeCount;
                subjectsView.DetailsFilter = SurveillanceConfig.DetailsFilter;
                _saveEvents = SurveillanceConfig.SaveEvents;

                sourcesPanel.SuspendLayout();
                try
                {
                    //foreach (var device in _surveillance.DeviceManager.Devices)
                    //{
                    //    OnNewSource(new NSurveillanceSource(GetModalityType(), (NCamera)device));
                    //}
                    //foreach (var state in _states)
                    //{
                    //    await OnSourceStartInternal(state, true, false);
                    //}
                    string cameraUrl = Store.FirstOrDefault(x => x.accNum == SessionManager.StoreAccNum)?.CameraUrl;
                    foreach (var item in Store.Where(x => x.IsXveillance == false).ToList())
                    {
                        if (!string.IsNullOrEmpty(item.CameraUrl))
                        {
                            if (item.CameraUrl.Contains(","))
                            {
                                string[] cameraUrlIds = item.CameraUrl.Split(',');

                                if (cameraUrlIds.Length > 1)
                                {
                                    foreach (var Url in cameraUrlIds)
                                    {
                                        using (var dialog = new ConnectToDeviceForm())
                                        {
                                            dialog.SetDynamicallyCameraUrl(Url);

                                            try
                                            {
                                                var camera = (NCamera)_surveillance.DeviceManager.ConnectToDevice(dialog.SelectedPlugin, dialog.Parameters);
                                                var source = new NSurveillanceSource(GetModalityType(), camera);
                                                var state = OnNewSource(source);
                                                state.SelectSource();
                                                OnSourceStartInternal(state, true, true);

                                                sourcesPanel.ScrollIntoView(state);
                                                EnableSourceControls();
                                            }
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (var dialog = new ConnectToDeviceForm())
                                {
                                    dialog.SetDynamicallyCameraUrl(item.CameraUrl);

                                    try
                                    {
                                        var camera = (NCamera)_surveillance.DeviceManager.ConnectToDevice(dialog.SelectedPlugin, dialog.Parameters);
                                        var source = new NSurveillanceSource(GetModalityType(), camera);
                                        var state = OnNewSource(source);
                                        state.SelectSource();

                                        sourcesPanel.ScrollIntoView(state);
                                        OnSourceStartInternal(state, true, true);
                                        EnableSourceControls();

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                }
                finally
                {
                    sourcesPanel.ResumeLayout();
                }

                EnableSourceControls();
                SubscribeToEvents();

                if (SurveillanceConfig.FirstStart)
                {
                    string message = "From Surveillance 8.1 when using GPU, AI models needs to be optimized, before starting to use functionality. It can be done through Tools menu. Would you like to optimize models now?";
                    if (MessageBox.Show(this, message, "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        using (var dialog = new OptimizationDialog()
                        {
                            Surveillance = _surveillance,
                            Modalities = NSurveillanceModalityType.Faces | NSurveillanceModalityType.VehiclesAndHumans | NSurveillanceModalityType.LicensePlateRecognition
                        })
                        {
                            if (dialog.ShowDialog(this) == DialogResult.OK)
                            {
                                _modelOptimized |= dialog.Modalities;
                            }
                        }
                    }
                    SurveillanceConfig.FirstStart = false;
                    SurveillanceConfig.Save();
                }

                if (_startOnStartup)
                {
                    foreach (var state in _states)
                    {
                        await OnSourceStartInternal(state, true, false);
                    }
                }


            }


            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create surveillance object {ex.Message}", @"Failed to initialize");

                Close();
            }
        }

        private void MainFormClosing(object sender, FormClosingEventArgs e)
        {

            if (_surveillance != null)
            {
                _states.ForEach(x =>
                {
                    x.Replay = false;
                    x.CancellationSource?.Cancel();
                });

                _surveillance.Stop();
                UnsubscribeFromEvents();
                _surveillance.Dispose();
                _surveillance = null;
            }

            if (_recordCollection != null)
            {
                _recordCollection.Close();
            }
        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            AboutBox.Show();
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Hide();
            foreach (var state in _states)
            {
                _surveillance.StopSourceAsync(state.Source);
                System.Threading.Thread.Sleep(1000);
                OnSourceStartInternal(state, true, false);
            }

        }

        private void ExportEventsToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var form = new ExportEventsForm() { RecordCollection = _recordCollection })
            {
                form.ShowDialog(this);
            }
        }

        private async void SettingsToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var dialog = new SettingsForm() { Surveillance = _surveillance })
            {
                dialog.LoadSettings();
                if (dialog.ShowDialog() == DialogResult.OK)
                {

                    try
                    {
                        _states.ForEach(x => x.Stopped -= OnSourceStateStopped);

                        var runningSources = _surveillance.Sources.Where(x => x.State == NMediaState.Running).ToArray();
                        if (runningSources.Any())
                        {
                            await _surveillance.StopAsync();
                        }

                        var retrying = _states.Where(x => x.RetryTask != null).ToList();
                        if (retrying.Any())
                        {
                            await Task.WhenAll(retrying.Select(x => CancelRetryTask(x)).ToArray());
                        }

                        dialog.SaveSettings(); _surveillance.UseGPU = false;
                        _modelOptimized = NSurveillanceModalityType.None;
                        subjectsView.MaxNodes = SurveillanceConfig.MiscMaxTreeNodeCount;
                        _saveEvents = SurveillanceConfig.SaveEvents;
                        foreach (var state in _states.Where(x => x.RetryTask != null))
                        {
                            state.ReplayTimeout = SurveillanceConfig.RetryFrequency;
                        }

                        foreach (SourceControl item in sourcesPanel.Controls)
                        {
                            item.UpdatePresetLabel();
                        }

                        if (runningSources.Any())
                        {
                            if (_surveillance.UseGPU || CheckCpuWarning(NSurveillanceModalityType.LicensePlateRecognition | NSurveillanceModalityType.VehiclesAndHumans, runningSources.Length - 1))
                            {
                                if (_surveillance.UseGPU)
                                {
                                    var modalities = runningSources.Select(x => x.ModalityType).Aggregate((a, b) => a | b);
                                    OptimizeModels(modalities);
                                }

                                var startTasks = runningSources.Select(x => OnSourceStartInternal(TryFindSourceController(x), showView: false)).ToArray();
                                await Task.WhenAll(startTasks);
                            }
                        }

                        retrying.ForEach(x => StartRetryTask(x));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Changing of settings failed: {0}", ex.Message), "Change Settings");
                    }
                    finally
                    {
                        _states.ForEach(x => x.Stopped += OnSourceStateStopped);
                    }
                }
            }
        }

        private void BtnClearInactiveClick(object sender, EventArgs e)
        {
            detailsView.Info = null;
            subjectsView.Clear();
        }

        private void SubjectsViewDoubleClick(object sender, SubjectInfo e)
        {
            using (var form = new SubjectForm() { Info = e, FaceRecords = _faceRecordsCollection, SubjectsView = subjectsView })
            {
                form.ShowDialog(this);
            }
        }

        private void ToolLayoutDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var layout = e.ClickedItem.Tag as LayoutDefinition.Layout;
            if (layout != null)
            {
                SurveillanceConfig.Layout = layout.Name;
                SurveillanceConfig.Save();
                SelectLayout(layout);
            }
        }

        private void TsbHideSourcesClick(object sender, EventArgs e)
        {
            ShowSourcesPanel(false);
        }

        private void TsbShowSourcesClick(object sender, EventArgs e)
        {
            ShowSourcesPanel(true);
        }

        private void TsbConnectClick(object sender, EventArgs e)
        {
            using (var dialog = new ConnectToDeviceForm())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var camera = (NCamera)_surveillance.DeviceManager.ConnectToDevice(dialog.SelectedPlugin, dialog.Parameters);
                        var source = new NSurveillanceSource(GetModalityType(), camera);
                        var state = OnNewSource(source);
                        state.SelectSource();
                        sourcesPanel.ScrollIntoView(state);
                        EnableSourceControls();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void TsbDisconnectClick(object sender, EventArgs e)
        {
            var state = _states.FirstOrDefault(x => x.IsSelected);
            if (state != null)
            {
                _surveillance.DeviceManager.DisconnectFromDevice(state.Source.Camera);
            }
        }

        private void TsbAddClick(object sender, EventArgs e)
        {
            using (var dialog = new VideoForm() { AllowedModalityTypes = GetModalityType() })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    sourcesPanel.SuspendLayout();

                    bool first = true;
                    foreach (var src in dialog.Sources)
                    {
                        var state = OnNewSource(src);
                        if (first)
                        {
                            state.SelectSource();
                            sourcesPanel.ScrollIntoView(state);
                        }
                        first = false;
                    }

                    sourcesPanel.ResumeLayout();

                    EnableSourceControls();
                }
            }
        }

        private async void TsbRemoveClick(object sender, EventArgs e)
        {
            var state = _states.FirstOrDefault(x => x.IsSelected);
            await RemoveVideo(state, false);
        }

        private void ChbAutoScrollCheckedChanged(object sender, EventArgs e)
        {
            subjectsView.AutoScrollToEnd = chbAutoScroll.Checked;
        }

        private void BtnFilterClick(object sender, EventArgs e)
        {
            using (var dialog = new SelectFilterForm { Filter = (Filter)subjectsView.Filter.Clone() })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    lblFilter.Text = $"Filter: {dialog.Filter}";
                    subjectsView.Filter = dialog.Filter;
                }
            }
        }

        private void BtnFilterDetailsClick(object sender, EventArgs e)
        {
            using (var dialog = new DetailsFilterForm { Filter = SurveillanceConfig.DetailsFilter })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SurveillanceConfig.DetailsFilter = dialog.Filter;
                    subjectsView.DetailsFilter = dialog.Filter;
                    subjectsView.Refresh();
                }
            }
        }

        private void BtnRotateClick(object sender, EventArgs e)
        {
            SwitchOrientation(subjectsView.Orientation == Orientation.Vertical ? Orientation.Horizontal : Orientation.Vertical);
        }

        private void EnrollFromDirectoryToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                using (var enrollDlg = new EnrollForm())
                {
                    enrollDlg.DirectoryPath = folderBrowserDialog.SelectedPath;
                    enrollDlg.Surveillance = _surveillance;
                    enrollDlg.FaceRecordsCollection = _faceRecordsCollection;
                    enrollDlg.ShowDialog();
                }
            }
        }

        private void EnrollFromImagesToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var enrollDlg = new EnrollForm())
                {
                    enrollDlg.Files = openFileDialog.FileNames;
                    enrollDlg.Surveillance = _surveillance;
                    enrollDlg.FaceRecordsCollection = _faceRecordsCollection;
                    enrollDlg.ShowDialog();
                };
            }
        }

        private void TsbShowDetailsClick(object sender, EventArgs e)
        {
            ShowDetailsView(true);
        }

        private void TsbHideDetailsClick(object sender, EventArgs e)
        {
            ShowDetailsView(false);
        }

        private void SubjectsViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                var selected = detailsView.Info = subjectsView.Selected;
                btnEnroll.Enabled = selected?.Face.Template != null || !selected?.LicensePlate.IsEmpty == true;
            }
        }

        private void BtnEnrollClick(object sender, EventArgs e)
        {
            SubjectInfo si = subjectsView.Selected;
            if (si.Face.Template != null)
            {
                using (var addForm = new AddToWatchListForm())
                {
                    if (addForm.ShowDialog(si) == DialogResult.OK)
                    {
                        if (addForm.SubjectId != string.Empty)
                        {
                            try
                            {
                                using (var thumb = SubjectInfo.CutFaceThumbnail(si.BestImage, si.Face.BestAttributes, 1, 1))
                                using (var buffer = si.Face.Template.Save())
                                {
                                    _surveillance.AddTemplate(addForm.SubjectId, buffer);
                                    var faceRecord = new FaceRecord(-1, buffer.ToArray(), addForm.SubjectId, DateTime.Now, si.Face.BestAttributes);
                                    _faceRecordsCollection.Add(faceRecord, si.BestImage, thumb);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(string.Format("Template was not added to the watch list. {0}", ex.Message), @"Add To Watch List");
                            }
                        }
                        else
                        {
                            MessageBox.Show(@"Template was not added to the watch list. Specify non-empty subject id.", @"Add To Watch List");
                        }
                    }
                }
            }
            else
            {
                using (var dialog = new LicensePlateWatchlistForm())
                {
                    dialog.LicensePlates = _licensePlateCollection;
                    dialog.TargetValue = si.LicensePlate.Best.Value;
                    dialog.ShowDialog(this);
                }
            }
        }

        private async void OptimizeModelsToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                _states.ForEach(x => x.Stopped -= OnSourceStateStopped);

                var runningSources = _surveillance.Sources.Where(x => x.State == NMediaState.Running).ToArray();
                if (runningSources.Any())
                {
                    await _surveillance.StopAsync();
                }

                var retrying = _states.Where(x => x.RetryTask != null).ToList();
                if (retrying.Any())
                {
                    await Task.WhenAll(retrying.Select(x => CancelRetryTask(x)).ToArray());
                }

                using (var dialog = new OptimizationDialog()
                {
                    Surveillance = _surveillance,
                    Modalities = NSurveillanceModalityType.Faces | NSurveillanceModalityType.VehiclesAndHumans | NSurveillanceModalityType.LicensePlateRecognition
                })
                {
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        _modelOptimized |= dialog.Modalities;
                    }
                }

                if (runningSources.Any())
                {
                    var startTasks = runningSources.Select(x => OnSourceStartInternal(TryFindSourceController(x), showView: false)).ToArray();
                    await Task.WhenAll(startTasks);
                }

                retrying.ForEach(x => StartRetryTask(x));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Changing of settings failed: {0}", ex.Message), "Change Settings");
            }
            finally
            {
                _states.ForEach(x => x.Stopped += OnSourceStateStopped);
            }
        }

        private void FacesToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var dialog = new FacesWatchlistForms())
            {
                dialog.Faces = _faceRecordsCollection;
                dialog.Surveillance = _surveillance;
                dialog.ShowDialog(this);
            }
        }

        private void LicensePlatesToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var dialog = new LicensePlateWatchlistForm())
            {
                dialog.LicensePlates = _licensePlateCollection;
                dialog.ShowDialog(this);
            }
        }

        #endregion

        #region Private methods

        private SourceController TryFindSourceController(NSurveillanceSource source)
        {
            return _states.FirstOrDefault(x => object.Equals(x.Source, source));
        }

        private SourceController OnNewSource(NSurveillanceSource source)
        {
            _surveillance.Sources.Add(source);
            var state = new SourceController(source, GetModalityType())
            {
                CanChangeProperties = true,
                CanShow = true,
                CanStart = true
            };
            state.ChangeFormat += OnSourceStateChangeFormat;
            state.ChangeSearchArea += OnSourceStateChangeSearchArea;
            state.ChangePreset += OnSourceStateChangePreset;
            state.Start += OnSourceStateStart;
            state.Stop += OnSourceStateStop;
            state.Select += OnSourceStateSelect;
            state.Stopped += OnSourceStateStopped;

            var sourcePresets = SurveillanceConfig.SourcePresets;
            if (sourcePresets.TryGetValue(state.SourceId, out var selectedPreset))
            {
                state.SelectedPreset = selectedPreset;
            }

            var searchAreas = SurveillanceConfig.SearchArea;
            if (searchAreas.TryGetValue(state.SourceId, out var area))
            {
                ApplySearchArea(state, area);
            }

            _states.Add(state);
            sourcesPanel.AddSource(state);
            viewsPanel.AddState(state);

            return state;
        }

        private void ApplySearchArea(SourceController state, SearchAreaConfig searchArea)
        {
            state.Source.SearchArea.Clear();
            if (searchArea != null)
            {
                state.Source.CheckSearchAreaByObjectCenter = searchArea.CheckSearchAreaByObjectCenter;
                foreach (var item in searchArea.Areas)
                {
                    state.Source.SearchArea.Add(item);
                }
            }
        }

        private async Task OnRemoveSource(SourceController state)
        {
            if (state != null)
            {
                await _surveillance.StopSourceAsync(state.Source);
                _surveillance.Sources.Remove(state.Source);

                sourcesPanel.RemoveSource(state);
                viewsPanel.RemoveState(state);

                state.UnselectSource();
                _states.Remove(state);
                EnableSourceControls();
            }
        }

        private void OnSourceStateSelect(object sender, EventArgs e)
        {
            var target = sender as SourceController;
            foreach (var state in _states.Where(x => x != target))
            {
                state.UnselectSource();
            }

            sourcesPanel.ScrollIntoView(target);
            EnableSourceControls();
        }

        private async void OnSourceStateStop(object sender, EventArgs e)
        {
            var state = sender as SourceController;
            state.CanStop = false;
            try
            {
                state.Stopped -= OnSourceStateStopped;
                await _surveillance.StopSourceAsync(state.Source);
            }
            catch (Exception ex)
            {
                Utils.ShowException(ex);
            }
            finally
            {
                state.Stopped += OnSourceStateStopped;
            }
            state.CanStart = true;
            state.CanChangeProperties = true;
        }

        private void OptimizeModels(NSurveillanceModalityType modalities)
        {
            LongActionDialog.ShowDialog(this, "Optimizing models, please wait ...", statusCallback =>
            {
                var _ = _surveillance.OptimizeModelsAsync(modalities, (__, arg) =>
                {
                    var text = $"Optimizing models ({arg.Value}/{arg.Maximum}), please wait ...";
                    statusCallback(text, arg.Value, arg.Maximum);
                }).Result;
            });
            _modelOptimized |= modalities;
        }

        private async Task<bool> OnSourceStartInternal(SourceController state, bool showError = true, bool showView = true)
        {
            state.CanStart = false;
            state.CanChangeProperties = false;
            if (showView) state.OnShow();
            try
            {
                _surveillance.UseGPU = false;
                if (CheckShouldOptimizeModel(state.ModalityType))
                {
                    OptimizeModels(state.ModalityType);
                }

                state.Source.ModalityType = state.ModalityType;
                ApplySourcePreset(state);
                try
                {
                    await _surveillance.StartSourceAsync(state.Source);
                }
                catch (Exception ex) { }
                state.CanStop = true;
                state.ReplayTimeout = -1;
                return true;
            }
            catch (Exception ex)
            {
                state.CanStart = true;
                state.CanChangeProperties = true;
                if (state.ReplayTimeout != -1)
                {
                    state.ReplayTimeout = SurveillanceConfig.RetryFrequency;
                }

                if (showError)
                {
                    Utils.ShowException(ex);
                }
                return false;
            }
            finally
            {
                Invalidate();
            }
        }

        private async Task RetryAfterTimeout(SourceController state)
        {
            bool successful = false;
            var token = state.CancellationSource.Token;

            while (!successful && !token.IsCancellationRequested)
            {
                state.ReplayTimeout = SurveillanceConfig.RetryFrequency;

                while (state.ReplayTimeout > 0)
                {
                    await Task.Delay(1000);
                    if (token.IsCancellationRequested)
                        break;
                    state.ReplayTimeout--;
                }

                if (!token.IsCancellationRequested)
                {
                    state.Stopped -= OnSourceStateStopped;
                    successful = await OnSourceStartInternal(state, false, false);
                    state.Stopped += OnSourceStateStopped;
                    if (successful)
                    {
                        state.RetryTask = null;
                        state.CancellationSource = null;
                    }
                }
            }
        }

        private async Task CancelRetryTask(SourceController state)
        {
            if (state.RetryTask != null)
            {
                state.CancellationSource.Cancel();
                await state.RetryTask;
                state.CancellationSource = null;
                state.RetryTask = null;
                state.ReplayTimeout = -1;
            }
        }

        private void StartRetryTask(SourceController state)
        {
            if (state.Replay)
            {
                state.CancellationSource = new CancellationTokenSource();
                state.RetryTask = RetryAfterTimeout(state);
            }
        }

        private void OnSourceStateStopped(object sender, EventArgs e)
        {
            var state = (SourceController)sender;
            if (state.Replay)
            {
                if (state.Source.Video != null)
                {
                    BeginInvoke(new Action<object, EventArgs>(OnSourceStateStart), state, new SourceRetryEventArgs());
                }
                else if (state.Source.Camera.IsAvailable)
                {
                    BeginInvoke(new Action<SourceController>(StartRetryTask), state);
                }
            }
        }

        private bool IsDetectorScalingExceeded(NSurveillanceModalityType modality, out string description)
        {
            switch (modality)
            {
                case NSurveillanceModalityType.VehiclesAndHumans:
                    description = "Vehicle-Human detector scale";
                    return (int)_surveillance.DetectorScaling > 1;
                case NSurveillanceModalityType.LicensePlateRecognition:
                    description = "License plate detector scale";
                    return (int)_surveillance.LicensePlateDetectorScaling > 1;
                case NSurveillanceModalityType.VehiclesAndHumans | NSurveillanceModalityType.LicensePlateRecognition:
                    description = "Vehicle-Human or License plate detector scale";
                    return (int)_surveillance.DetectorScaling > 1 || (int)_surveillance.LicensePlateDetectorScaling > 1;
                default:
                    description = "";
                    return false;
            }
        }

        private bool ShowContinueBox(string message)
        {
            return MessageBox.Show(message, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        private bool CheckCpuWarning(NSurveillanceModalityType modality, int runningSources)
        {
            if (_startOnStartup) return true;

            bool proceed = true;
            if (runningSources > 0)
            {
                proceed = ShowContinueBox("CPU only mode is designed for testing, it is recommended to use GPU mode. It is not recommended to run multiple cameras. Continue?");
            }
            else if (IsDetectorScalingExceeded(modality, out string message))
            {
                proceed = ShowContinueBox($"CPU only mode is designed for testing, it is recommended to use GPU mode. Consider lowering {message} for better performance. Continue?");
            }
            return proceed;
        }

        private async void OnSourceStateStart(object sender, EventArgs e)
        {
            var state = sender as SourceController;
            if (!_surveillance.UseGPU && !CheckCpuWarning(state.ModalityType, _states.Count(x => x.Source.State == NMediaState.Running)))
            {
                return;
            }

            bool showView = true;

            if (e is SourceRetryEventArgs)
            {
                showView = ((SourceRetryEventArgs)e).ShowView;
            }

            await CancelRetryTask(state);
            state.Stopped -= OnSourceStateStopped;
            bool success = await OnSourceStartInternal(state, showView: showView);
            state.Stopped += OnSourceStateStopped;
            if (!success && state.Replay)
            {
                StartRetryTask(state);
            }
        }

        private void OnSourceStateChangeFormat(object sender, NMediaFormat e)
        {
            var state = sender as SourceController;
            try
            {
                state.Source.SetCurrentFormat(e);
            }
            catch (Exception ex)
            {
                Utils.ShowException(ex);
            }
        }

        private void OnSourceStateChangeSearchArea(object sender, SearchAreaConfig value)
        {
            var state = sender as SourceController;
            try
            {
                ApplySearchArea(state, value);
                var persistentConfig = SurveillanceConfig.SearchArea;
                if (value != null)
                {
                    persistentConfig[value.SourceId] = value;
                }
                else
                {
                    persistentConfig.Remove(state.SourceId);
                }
                SurveillanceConfig.SearchArea = persistentConfig;
            }
            catch (Exception ex)
            {
                Utils.ShowException(ex);
            }
        }

        private void OnSourceStateChangePreset(object sender, string value)
        {
            var state = sender as SourceController;
            try
            {
                if (state.SelectedPreset != value)
                {
                    state.SelectedPreset = value;
                    _modelOptimized = NSurveillanceModalityType.None;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowException(ex);
            }
        }

        private void ApplySourcePreset(SourceController state)
        {
            try
            {
                state.Source.Reset();
                Dictionary<string, Preset> presets = SurveillanceConfig.Presets;
                if (state.SelectedPreset != null && presets.TryGetValue(state.SelectedPreset, out Preset preset))
                {
                    using (var pb = preset.GetPropertyBag())
                    {
                        pb.ApplyTo(state.Source);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowException(ex);
            }
        }

        private void OnDevicesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    BeginInvoke(new Action(() =>
                    {
                        if (!_surveillance.Sources.Any(x => object.Equals(x.Camera, item)))
                        {
                            OnNewSource(new NSurveillanceSource(GetModalityType(), (NCamera)item));
                        }
                    }));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    BeginInvoke(new Action(async () =>
                    {
                        var state = _states.FirstOrDefault(x => object.Equals(x.Source.Camera, item));
                        await OnRemoveSource(state);
                    }));
                }
            }
            else
                throw new NotImplementedException();
        }

        private void SubscribeToEvents()
        {
            if (_surveillance != null)
            {
                _surveillance.IsRunningChanged += SurveillanceIsRunningChanged;
                _surveillance.ImageCaptured += SurveillanceImageCaptured;
                _surveillance.SubjectAppeared += SurveillanceSubjectAppeared;
                _surveillance.SubjectTrack += SurveillanceSubjectTrack;
                _surveillance.SubjectDisappeared += SurveillanceSubjectDisappeared;
                _surveillance.SubjectMerged += SurveillanceSubjectMerged;
                _surveillance.SubjectSplit += SurveillanceSubjectSplit;

                _surveillance.DeviceManager.Devices.CollectionChanged += OnDevicesCollectionChanged;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (_surveillance != null)
            {
                _surveillance.IsRunningChanged -= SurveillanceIsRunningChanged;
                _surveillance.ImageCaptured -= SurveillanceImageCaptured;
                _surveillance.SubjectAppeared -= SurveillanceSubjectAppeared;
                _surveillance.SubjectTrack -= SurveillanceSubjectTrack;
                _surveillance.SubjectDisappeared -= SurveillanceSubjectDisappeared;
                _surveillance.SubjectMerged -= SurveillanceSubjectMerged;
                _surveillance.SubjectSplit -= SurveillanceSubjectSplit;

                _surveillance.DeviceManager.Devices.CollectionChanged -= OnDevicesCollectionChanged;
            }
        }

        private void SelectLayout(LayoutDefinition.Layout layout)
        {
            toolLayout.Image = layout.Icon;
            toolLayout.Text = layout.Name;
            viewsPanel.ViewsLayout = layout;
            Invalidate();
        }

        private void ShowSourcesPanel(bool value)
        {
            mainSplitContainer.Panel1Collapsed = !value;
            tsbShowSources.Visible = !value;
            tsbHideSources.Visible = value;
            tsbConnect.Visible = value;
            tsbDisconnect.Visible = value;
            tsbAdd.Visible = value;
            tsbRemove.Visible = value;
            toolStripSeparatorConnect.Visible = value;
        }

        private void EnableSourceControls()
        {
            var selected = _states.FirstOrDefault(x => x.IsSelected);
            if (selected != null)
            {
                tsbRemove.Enabled = selected.Source.Video != null;
                tsbDisconnect.Enabled = selected.Source.Camera?.IsDisconnectable == true;
            }
            else
            {
                tsbDisconnect.Enabled = false;
                tsbRemove.Enabled = false;
            }
        }

        private void SwitchOrientation(Orientation value)
        {
            splitContainerHorizontal.Panel2Collapsed = true;
            if (value != subjectsView.Orientation)
            {
                if (value == Orientation.Horizontal)
                {
                    splitContainerVertical.Panel2Collapsed = true;
                    splitContainerVertical.Panel2.Controls.Remove(tlpSubjects);

                    splitContainerHorizontal.Panel2Collapsed = false;
                    splitContainerHorizontal.Panel2.Controls.Add(tlpSubjects);

                    tlpSubjects.SetCellPosition(tlpFilterButtons, new TableLayoutPanelCellPosition(3, 0));
                    tlpSubjects.SetCellPosition(lblFilter, new TableLayoutPanelCellPosition(4, 0));
                    tlpSubjects.SetColumnSpan(lblFilter, 1);
                }
                else
                {
                    splitContainerHorizontal.Panel2Collapsed = true;
                    splitContainerVertical.Panel2.Controls.Add(tlpSubjects);
                    splitContainerVertical.Panel2Collapsed = false;

                    tlpSubjects.SetCellPosition(tlpFilterButtons, new TableLayoutPanelCellPosition(0, 1));
                    tlpSubjects.SetCellPosition(lblFilter, new TableLayoutPanelCellPosition(1, 1));
                    tlpSubjects.SetColumnSpan(lblFilter, 5);
                }
                subjectsView.Orientation = value;
                SurveillanceConfig.Orientation = value;
                SurveillanceConfig.Save();
            }
        }

        private void ShowDetailsView(bool value)
        {
            if (value)
            {
                btnUpdateCamera.Visible = true;
                tsbHideDetails.Visible = true;
                tlpCenter.ColumnStyles[1] = new ColumnStyle(SizeType.Absolute, 230);
                SurveillanceConfig.ShowDetails = true;
            }
            else
            {
                btnUpdateCamera.Visible = true;
                tsbHideDetails.Visible = false;
                tlpCenter.ColumnStyles[1] = new ColumnStyle(SizeType.Absolute, 0);
                SurveillanceConfig.ShowDetails = false;
            }
            SurveillanceConfig.Save();
        }

        private bool CheckShouldOptimizeModel(NSurveillanceModalityType modalities)
        {
            if (_surveillance.UseGPU)
            {
                return (_modelOptimized & modalities) != modalities;
            }
            return false;
        }

        private void BtnMinimizeClick(object sender, EventArgs e)
        {
            expandPanel.ClientSizeChanged -= viewsPanel.ExpandPanelClientSizeChanged;
            splitContainerViewAndDetails.Panel1Collapsed = true;
            splitContainerViewAndDetails.Panel2Collapsed = false;
            var cont = expandPanel.Controls.OfType<SurveillanceView>();
            var first = cont.FirstOrDefault();
            if (first != null)
            {
                viewsPanel.Controls.Add(first);
            }
            toolLayout.Enabled = true;
            splitContainerViewAndDetails.Panel2.Invalidate();
        }

        #endregion

        #region Protected methods

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Delete)
            {
                var selected = _states.FirstOrDefault(x => x.IsSelected);
                if (selected != null)
                {
                    var index = _states.IndexOf(selected);
                    if (selected.Source.Video != null)
                    {
                        RemoveVideo(selected, true)
                            .ContinueWith(async t =>
                            {
                                if (await t)
                                {
                                    BeginInvoke(new Action(() =>
                                    {
                                        if (index >= _states.Count)
                                            index--;
                                        if (index >= 0)
                                            _states[index].SelectSource();
                                    }));
                                }
                            });
                        return true;
                    }
                    else if (selected.Source.Camera.IsDisconnectable)
                    {
                        if (DialogResult.Yes == MessageBox.Show(this, "You are about to disconnect camera, continue?", "Disconnect camera?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                        {
                            _surveillance.DeviceManager.DisconnectFromDevice(selected.Source.Camera);

                            if (index >= _states.Count - 2)
                                index--;
                            if (index > 0)
                                _states[index].SelectSource();
                        }
                    }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (WindowState == FormWindowState.Maximized)
            {
                Update();
            }
        }

        #endregion

        private void cellPhoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new CellPhoneWatchlistForm())
            {
                dialog.CellPhone = _cellPhoneCollection;
                dialog.ShowDialog(this);
            }
        }

        private void MenuItem_MouseHover(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;

            ToolStripManager.Renderer = new ToolStripProfessionalRenderer(new MyColorTable(Color.Yellow));

            item.ForeColor = Color.Black;
            (item.DropDown as ToolStripDropDownMenu).ShowImageMargin = false;
            item.DropDown.BackColor = Color.SteelBlue;
            item.DropDown.ForeColor = Color.White;

        }
        private void MenuItem_MouseLeave(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            item.BackColor = Color.SteelBlue;
            item.ForeColor = Color.White;

        }
        private void button_MouseHover(object sender, EventArgs e)
        {
            var item = (ToolStripButton)sender;
            item.ForeColor = Color.Black;
            ToolStripManager.Renderer = new ToolStripProfessionalRenderer(new MyColorTable(Color.Yellow));

        }
        private void MyButton_EnabledChanged(object sender, EventArgs e)
        {
            // Adjust the button's appearance when enabled state changes
            ToolStripButton button = (ToolStripButton)sender;
            if (!button.Enabled)
            {
                // Set the button's appearance when disabled (e.g., white background)
                button.BackColor = Color.SteelBlue;

            }
        }
        private void button_MouseLeave(object sender, EventArgs e)
        {
            var item = (ToolStripButton)sender;

            item.ForeColor = Color.White;
        }

        private void toolstripDropdown_MouseHover(object sender, EventArgs e)
        {
            var item = (ToolStripDropDownButton)sender;
            item.ForeColor = Color.Black;
            ToolStripManager.Renderer = new ToolStripProfessionalRenderer(new MyColorTable(Color.Yellow));
            item.DropDown.BackColor = Color.SteelBlue;
            item.DropDown.ForeColor = Color.White;
        }
        private void toolstripDropdown_MouseLeave(object sender, EventArgs e)
        {
            var item = (ToolStripDropDownButton)sender;

            item.ForeColor = Color.White;
        }

        public class MyColorTable : ProfessionalColorTable
        {
            private Color menuItemSelectedColor;
            public MyColorTable(Color color) : base()
            {
                menuItemSelectedColor = color;
            }

            public override Color MenuItemSelected
            {
                get { return menuItemSelectedColor; }
            }

            public override Color MenuItemSelectedGradientBegin
            {
                get { return Color.Yellow; }
            }

            public override Color MenuItemSelectedGradientEnd
            {
                get { return Color.Yellow; }
            }

            public override Color ButtonSelectedGradientBegin
            {
                get { return Color.Yellow; }
            }
            public override Color ButtonSelectedGradientEnd
            {
                get { return Color.Yellow; }
            }
            public override Color ButtonPressedGradientBegin
            {
                get { return Color.Yellow; }
            }
            public override Color ButtonPressedGradientEnd
            {
                get { return Color.Yellow; }
            }



            public override Color ToolStripDropDownBackground
            {
                get
                {
                    return Color.SteelBlue;
                }
            }

            public override Color ImageMarginGradientBegin
            {
                get
                {
                    return Color.SteelBlue;
                }
            }

            public override Color ImageMarginGradientMiddle
            {
                get
                {
                    return Color.SteelBlue;
                }
            }

            public override Color ImageMarginGradientEnd
            {
                get
                {
                    return Color.SteelBlue;
                }
            }

            public override Color MenuBorder
            {
                get
                {
                    return Color.Black;
                }
            }

            public override Color MenuItemBorder
            {
                get
                {
                    return Color.Black;
                }
            }

            public override Color MenuStripGradientBegin
            {
                get
                {
                    return Color.SteelBlue;
                }
            }

            public override Color MenuStripGradientEnd
            {
                get
                {
                    return Color.SteelBlue;
                }
            }




            public override Color MenuItemPressedGradientBegin
            {
                get
                {
                    return Color.SteelBlue;
                }
            }

            public override Color MenuItemPressedGradientEnd
            {
                get
                {
                    return Color.SteelBlue;
                }
            }

        }

        private void btnUpdateCamera_Click(object sender, EventArgs e)
        {
            using (var dialog = new UpdateCameraForm() { })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (!string.IsNullOrEmpty(dialog.cameraUrl))
                    {
                        if (_surveillance != null)
                        {
                            _states.ForEach(x =>
                            {
                                x.Replay = false;
                                x.CancellationSource?.Cancel();
                            });

                            _surveillance.Stop();
                        }
                        var stateList = _states.ToList();
                        if (stateList.Count > 0)
                        {
                            foreach (var item in stateList)
                            {
                                var index = _states.IndexOf(item);
                                _surveillance.DeviceManager.DisconnectFromDevice(item.Source.Camera);
                                if (index >= _states.Count - 2)
                                    index--;
                                if (index > 0)
                                    _states[index].SelectSource();

                                //_states.Clear();
                                if (item != null)
                                {
                                    _surveillance.StopSourceAsync(item.Source);
                                    _surveillance.Sources.Remove(item.Source);

                                    sourcesPanel.RemoveSource(item);
                                    viewsPanel.RemoveState(item);

                                    item.UnselectSource();
                                    _states.Remove(item);
                                    EnableSourceControls();
                                }
                            }
                        }
                        if (dialog.cameraUrl.Contains(","))
                        {
                            string[] cameraUrlIds = dialog.cameraUrl.Split(',');

                            if (cameraUrlIds.Length > 1)
                            {
                                foreach (var Url in cameraUrlIds)
                                {
                                    using (var dialogDevice = new ConnectToDeviceForm())
                                    {
                                        dialogDevice.SetDynamicallyCameraUrl(Url);

                                        try
                                        {
                                            var camera = (NCamera)_surveillance.DeviceManager.ConnectToDevice(dialogDevice.SelectedPlugin, dialogDevice.Parameters);
                                            var source = new NSurveillanceSource(GetModalityType(), camera);
                                            var state = OnNewSource(source);
                                            state.SelectSource();
                                            //OnSourceStartInternal(state, true, true);
                                            //sourcesPanel.ScrollIntoView(state);
                                            //EnableSourceControls();
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var dialogDevice = new ConnectToDeviceForm())
                            {
                                dialogDevice.SetDynamicallyCameraUrl(dialog.cameraUrl);

                                try
                                {
                                    var camera = (NCamera)_surveillance.DeviceManager.ConnectToDevice(dialogDevice.SelectedPlugin, dialogDevice.Parameters);
                                    var source = new NSurveillanceSource(GetModalityType(), camera);
                                    var state = OnNewSource(source);
                                    state.SelectSource();
                                    //sourcesPanel.ScrollIntoView(state);
                                    //OnSourceStartInternal(state, true, true);
                                    //EnableSourceControls();

                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    foreach (var state in _states)
                    {
                        state.SelectSource();
                        OnSourceStartInternal(state, true, true);
                    }
                }
                else
                {

                }
            }



        }
    }

    public class SourceRetryEventArgs : EventArgs
    {
        public bool ShowView { get; set; } = false;

        public SourceRetryEventArgs(bool showView = false)
        {
            this.ShowView = showView;
        }


    }
}
