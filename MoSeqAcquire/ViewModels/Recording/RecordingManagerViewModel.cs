using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.Models.Management;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MvvmValidation;

namespace MoSeqAcquire.ViewModels.Recording
{
    
    public class RecordingManagerViewModel : ValidatingBaseViewModel
    {
        protected MoSeqAcquireViewModel rootViewModel;
        protected ObservableCollection<RecorderViewModel> recorders;
        protected RecorderViewModel selectedRecorder;
        protected RecordingManager _recordingManager;
        protected bool isRecording;


        public RecordingManagerViewModel(MoSeqAcquireViewModel RootViewModel)
        {
            this.rootViewModel = RootViewModel;
            this.recorders = new ObservableCollection<RecorderViewModel>();

            this._recordingManager = new RecordingManager(this.rootViewModel.TriggerBus);
            this._recordingManager.PropertyChanged += (s, e) => this.NotifyPropertyChanged(null);
            this.RegisterRules();
        }
        protected void RegisterRules()
        {
            Validator.AddRequiredRule(() => this.GeneralSettings.Directory, "Directory cannot be empty!");

        }
        public MoSeqAcquireViewModel Root { get => this.rootViewModel; }


        #region support typed recorder creation
        public void AddRecorder(Type RecorderType)
        {
            var recorder = new RecorderViewModel(this.rootViewModel, RecorderType);
            this.AddRecorder(recorder);
        }
        public void AddRecorder(RecorderViewModel Recorder)
        {
            this.Recorders.Add(Recorder);
            this.SelectedRecorder = Recorder;
            this._recordingManager.AddRecorder(Recorder.Writer);
        }
        public void RemoveRecorder(RecorderViewModel Recorder)
        {
            this._recordingManager.RemoveRecorder(Recorder.Writer);
            if(this.SelectedRecorder == Recorder)
            {
                this.SelectedRecorder = null;
            }
            this.Recorders.Remove(Recorder);
        }
        public void RemoveSelectedRecorder()
        {
            this.RemoveRecorder(this.SelectedRecorder);
        }
        public string GetNextDefaultRecorderName()
        {
            string namebase = "Recorder";
            string realname = "";
            int count = 1;
            while (count < int.MaxValue)
            {
                realname = namebase + " " + count.ToString();
                if (this.recorders.Count(rvm => rvm.Name.Equals(realname)) == 0)
                {
                    break;
                }
                count++;
            }
            return realname;
        }
        #endregion

        public ObservableCollection<RecorderViewModel> Recorders { get => this.recorders; }
        public RecorderViewModel SelectedRecorder
        {
            get => this.selectedRecorder;
            set => this.SetField(ref this.selectedRecorder, value);
        }



        public GeneralRecordingSettings GeneralSettings
        {
            get => this._recordingManager.GeneralSettings;
        }
        public bool IsRecording
        {
            get
            {
                if (this._recordingManager == null) { return false; }
                return this._recordingManager.IsRecording;
            }
        }
        public TimeSpan? Duration { get => this._recordingManager?.Duration; }
        public double? Progress { get => this._recordingManager?.Progress; }
        public TimeSpan? TimeRemaining { get => this._recordingManager?.TimeRemaining; }

        
        public void StartRecording()
        {
            this._recordingManager.Initialize(this.GeneralSettings);
            this._recordingManager.Start();
        }
        public void StopRecording()
        {
            this._recordingManager.Stop();
        }
    }
}
