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
using System.Windows.Input;
using MoSeqAcquire.Models.Triggers;

namespace MoSeqAcquire.ViewModels.Recording
{

    public class RecordingManagerViewModel : ValidatingBaseViewModel
    {
        protected MoSeqAcquireViewModel rootViewModel;
        protected ObservableCollection<RecorderViewModel> recorders;
        protected RecorderViewModel selectedRecorder;
        protected RecordingManager recordingManager;


        public RecordingManagerViewModel(MoSeqAcquireViewModel RootViewModel)
        {
            this.rootViewModel = RootViewModel;
            this.recorders = new ObservableCollection<RecorderViewModel>();

            this.recordingManager = new RecordingManager(this.rootViewModel.TriggerBus);
            this.recordingManager.PropertyChanged += (s, e) =>
            {
                this.NotifyPropertyChanged(null);
            };
            this.RegisterRules();

            this.rootViewModel.TriggerBus.Subscribe(typeof(BeforeRecordingStartedTrigger),
                                                    new ActionTriggerAction() {
                                                        Delegate = (t) => this.Root.ForceProtocolLocked()
                                                    });
            this.rootViewModel.TriggerBus.Subscribe(typeof(AfterRecordingFinishedTrigger),
                                                    new ActionTriggerAction()
                                                    {
                                                        Delegate = (t) => this.Root.UndoForceProtoclLocked()
                                                    });
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
            this.recordingManager.AddRecorder(Recorder.Writer);
        }
        public void RemoveRecorder(RecorderViewModel Recorder)
        {
            this.recordingManager.RemoveRecorder(Recorder.Writer);
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
                realname = namebase + "_" + count.ToString();
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
            get => this.recordingManager.GeneralSettings;
        }
        public RecordingManagerState State
        {
            get => this.recordingManager.State;
        }
        public string CurrentTask
        {
            get => this.recordingManager.CurrentTask;
        }
        public TimeSpan? Duration { get => this.recordingManager?.Duration; }
        public double? Progress { get => this.recordingManager?.Progress; }
        public TimeSpan? TimeRemaining { get => this.recordingManager?.TimeRemaining; }

        
        public void StartRecording()
        {
            Task.Run(() =>
            {
                this.recordingManager.Start();
            });
        }
        public void StopRecording()
        {
            Task.Run(() =>
            {
                this.recordingManager.Stop();
            });
        }
        public void AbortRecording()
        {
            Task.Run(() =>
            {
                this.recordingManager.Abort();
            });
        }
    }
}
