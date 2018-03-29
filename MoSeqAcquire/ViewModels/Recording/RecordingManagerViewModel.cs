using MoSeqAcquire.Models.IO;
using MoSeqAcquire.Models.Management;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Recording
{
    
    public class RecordingManagerViewModel : BaseViewModel
    {
        protected MoSeqAcquireViewModel rootViewModel;
        protected Type selectedRecorderType;
        protected ObservableCollection<RecorderViewModel> recorders;
        protected RecorderViewModel selectedRecorder;
        protected GeneralRecordingSettings settings;
        protected bool isRecording;


        public RecordingManagerViewModel(MoSeqAcquireViewModel RootViewModel)
        {
            this.rootViewModel = RootViewModel;
            this.recorders = new ObservableCollection<RecorderViewModel>();
            this.settings = new GeneralRecordingSettings();

            this.AvailableRecorderTypes = new ReadOnlyObservableCollection<Type>(new ObservableCollection<Type>(ProtocolHelpers.FindRecorderTypes()));
        }
        public MoSeqAcquireViewModel Root { get => this.rootViewModel; }


        #region support typed recorder creation
        public ReadOnlyObservableCollection<Type> AvailableRecorderTypes { get; protected set; }
        public Type SelectedRecorderType
        {
            get => this.selectedRecorderType;
            set => this.SetField(ref this.selectedRecorderType, value);
        }
        protected IEnumerable<Type> FindRecorderTypes()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(IMediaWriter).IsAssignableFrom(t));
                //.Select(t => t.FullName);
        }
        public void AddRecorder()
        {
            var recorder = new RecorderViewModel(this.rootViewModel, this.SelectedRecorderType);
            recorder.Name = "New Recorder";
            this.Recorders.Add(recorder);
            this.SelectedRecorder = recorder;
        }
        #endregion



        public ObservableCollection<RecorderViewModel> Recorders { get => this.recorders; }
        
        public RecorderViewModel SelectedRecorder
        {
            get => this.selectedRecorder;
            set => this.SetField(ref this.selectedRecorder, value);
        }
        public bool IsRecording
        {
            get
            {
                if (this._recordingManager == null) { return false; }
                return this._recordingManager.IsRecording;
            }
        }
        public GeneralRecordingSettings GeneralSettings
        {
            get => this.settings;
            set => this.SetField(ref this.settings, value);
        }

        public TimeSpan? Duration { get => this._recordingManager?.Duration; }
        public double? Progress { get => this._recordingManager?.Progress; }
        public TimeSpan? TimeRemaining { get => this._recordingManager?.TimeRemaining; }

        protected RecordingManager _recordingManager;
        public void StartRecording()
        {
            this._recordingManager = new RecordingManager();
            this._recordingManager.PropertyChanged += (s, e) => this.NotifyPropertyChanged(null);
            this._recordingManager.RecordingFinished += (s, e) => { this._recordingManager = null; };
            foreach (var r in this.Recorders)
            {
                this._recordingManager.AddRecorder(r.MakeMediaWriter());
            }
            this._recordingManager.Initialize(this.GeneralSettings);
            this._recordingManager.Start();
        }
        public void StopRecording()
        {
            this._recordingManager.Stop();
            this._recordingManager = null;
        }
    }
}
