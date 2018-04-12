using MoSeqAcquire.Models.Recording;
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
        protected RecordingManager _recordingManager;
        //protected GeneralRecordingSettings settings;
        protected bool isRecording;


        public RecordingManagerViewModel(MoSeqAcquireViewModel RootViewModel)
        {
            this.rootViewModel = RootViewModel;
            this.recorders = new ObservableCollection<RecorderViewModel>();
            //this.settings = new GeneralRecordingSettings();
            this.PopulateAvailableRecorderTypes();

            this._recordingManager = new RecordingManager();
            this._recordingManager.PropertyChanged += (s, e) => this.NotifyPropertyChanged(null);
            /*foreach (var r in this.Recorders)
            {
                this._recordingManager.AddRecorder(r.MakeMediaWriter());
            }*/
        }
        protected void PopulateAvailableRecorderTypes()
        {
            this.AvailableRecorderTypes = new ReadOnlyObservableCollection<AvailableRecorderTypeViewModel>(new ObservableCollection<AvailableRecorderTypeViewModel>(ProtocolHelpers.FindRecorderTypes().Select(t => new AvailableRecorderTypeViewModel(t))));
        }
        public MoSeqAcquireViewModel Root { get => this.rootViewModel; }


        #region support typed recorder creation
        public ReadOnlyObservableCollection<AvailableRecorderTypeViewModel> AvailableRecorderTypes { get; protected set; }
        public Type SelectedRecorderType
        {
            get => this.selectedRecorderType;
            set => this.SetField(ref this.selectedRecorderType, value);
        }
        public void AddRecorder()
        {
            var recorder = new RecorderViewModel(this.rootViewModel, this.SelectedRecorderType);
            recorder.Name = "New Recorder";
            this.Recorders.Add(recorder);
            this.SelectedRecorder = recorder;

            this._recordingManager.AddRecorder(recorder.MakeMediaWriter());
        }
        public void RemoveSelectedRecorder()
        {
            if(this.SelectedRecorder != null)
            {
                var currIdx = this.Recorders.IndexOf(this.SelectedRecorder);
                this.Recorders.Remove(this.SelectedRecorder);
                this.SelectedRecorder = this.Recorders.ElementAtOrDefault(currIdx - 1);
            }
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
            get => this._recordingManager.GeneralSettings;
            set => this._recordingManager.GeneralSettings = value;
        }

        public TimeSpan? Duration { get => this._recordingManager?.Duration; }
        public double? Progress { get => this._recordingManager?.Progress; }
        public TimeSpan? TimeRemaining { get => this._recordingManager?.TimeRemaining; }

        
        public void StartRecording()
        {
            //this._recordingManager = new RecordingManager();
            //this._recordingManager.PropertyChanged += (s, e) => this.NotifyPropertyChanged(null);
            //this._recordingManager.RecordingFinished += (s, e) => { this._recordingManager = null; };
            this._recordingManager.ClearRecorders();
            foreach (var r in this.Recorders)
            {
                this._recordingManager.AddRecorder(r.MakeMediaWriter());
            }
            this._recordingManager.Initialize(this.GeneralSettings);
            this._recordingManager.Start();
        }
        public void StopRecording()
        {
            this._recordingManager = new 
            this._recordingManager.Stop();
            this._recordingManager = null;
        }
    }


    public class AvailableRecorderTypeViewModel
    {
        protected Type recorderType;
        protected RecorderSpecification spec;

        public AvailableRecorderTypeViewModel(Type RecorderType)
        {
            this.recorderType = RecorderType;
            this.spec = new RecorderSpecification(RecorderType);
        }
        public Type RecorderType
        {
            get => this.recorderType;
        }
        public string Name
        {
            get => this.spec.Name;
        }
    }
}
