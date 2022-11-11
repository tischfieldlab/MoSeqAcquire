using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.Models.Management;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MvvmValidation;
using System.Windows.Input;
using MoSeqAcquire.Models.Metadata;
using MoSeqAcquire.Models.Triggers;
using MoSeqAcquire.ViewModels.Metadata;
using System.Collections;
using System.Windows;
using System.Windows.Data;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.ViewModels.MediaSources;
using System.Threading;
using MaterialDesignThemes.Wpf;
using MoSeqAcquire.Views.RecorderViews;
using MoSeqAcquire.ViewModels.Triggers;
using Microsoft.Extensions.DependencyInjection;


namespace MoSeqAcquire.ViewModels.Recording
{
    public class RecordingManagerViewModel : BaseViewModel, INotifyDataErrorInfo, IDataErrorInfo
    {
        protected MoSeqAcquireViewModel rootViewModel;
        protected ObservableCollection<RecorderViewModel> recorders;
        protected RecorderViewModel selectedRecorder;
        protected RecordingManager recordingManager;

        protected ProtocolManagerViewModel protocolManager;

        

        public RecordingManagerViewModel(ProtocolManagerViewModel protocolManager) : base()
        {
            this.protocolManager = protocolManager;
            //this.rootViewModel = RootViewModel;
            this.recorders = new ObservableCollection<RecorderViewModel>();

            this.recordingManager = new RecordingManager();
            this.recordingManager.BeforeStartRecording += (s, e) => this.protocolManager.ForceProtocolLocked();
            this.recordingManager.RecordingFinished += (s, e) => this.protocolManager.UndoForceProtoclLocked();
            this.recordingManager.RecordingFinished += (s, e) => this.RecordingMetadata.Items.ResetValuesToDefaults();
            this.recordingManager.RecordingAborted += (s, e) => this.protocolManager.UndoForceProtoclLocked();

            this.recordingManager.PropertyChanged += (s, e) =>
            {
                this.NotifyPropertyChanged(null);
            };

            this.GeneralSettings.ErrorsChanged += SubItemErrorsChanged;
            this.RecordingMetadata.Items.ErrorsChanged += SubItemErrorsChanged;
        }
        public RecordingManager RecordingManager { get => this.recordingManager; }

        private void SubItemErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            this.ErrorsChanged?.Invoke(this, e);
            this.NotifyPropertyChanged(nameof(this.Error));
            this.NotifyPropertyChanged(nameof(this.HasErrors));
        }

        //public MoSeqAcquireViewModel Root { get => this.rootViewModel; }
        public GeneralRecordingSettings GeneralSettings
        {
            get => this.recordingManager.GeneralSettings;
        }
        public MetadataViewModel RecordingMetadata
        {
            get => this.recordingManager.RecordingMetadata;
        }

        #region Recorder CRUD Operations
        public void AddRecorder(Type RecorderType)
        {
            var recorder = new RecorderViewModel(RecorderType);
            this.AddRecorder(recorder);
        }

        public void AddRecorder(RecorderViewModel Recorder)
        {
            Recorder.ErrorsChanged += this.SubItemErrorsChanged;
            this.Recorders.Add(Recorder);
            this.SelectedRecorder = Recorder;
            this.recordingManager.AddRecorder(Recorder.Writer);
        }
        public void RemoveRecorder(RecorderViewModel Recorder)
        {
            Recorder.ErrorsChanged -= this.SubItemErrorsChanged;
            this.recordingManager.RemoveRecorder(Recorder.Writer);
            if(this.SelectedRecorder == Recorder)
            {
                this.SelectedRecorder = null;
            }
            this.Recorders.Remove(Recorder);
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
        

        public ObservableCollection<RecorderViewModel> Recorders { get => this.recorders; }
        public RecorderViewModel SelectedRecorder
        {
            get => this.selectedRecorder;
            set => this.SetField(ref this.selectedRecorder, value);
        }

        public void ClearRecorders()
        {
            this.recordingManager.ClearRecorders();
            this.recorders.Clear();
        }
#endregion

        



        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public string Error
        {
            get => string.Join(" ", this.GetErrors(null).Cast<string>());
        }

        public bool HasErrors
        {
            get => !string.IsNullOrEmpty(this.Error);
        }

        public string this[string columnName]
        {
            get { return this.GetErrors(columnName).Cast<string>().FirstOrDefault(); }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            var errors = new List<string>();
            errors.AddRange(this.GeneralSettings.GetErrors(propertyName).Cast<string>().Select(e => "Recording Settings: "+e));
            errors.AddRange(this.Recorders.SelectMany(r => r.GetErrors(propertyName).Cast<string>().Select(e => $"{r.Name}: {e}")));
            errors.AddRange(this.RecordingMetadata.Items.GetErrors(propertyName).Cast<string>().Select(e => "Recording Metadata: "+e));
            return errors;
        }



        #region Recording Life Cycle
        public RecordingManagerState State { get => this.recordingManager.State; }
        public string CurrentTask { get => this.recordingManager.CurrentTask; }
        public TimeSpan? Duration { get => this.recordingManager?.Duration; }
        public double? Progress { get => this.recordingManager?.Progress; }
        public TimeSpan? TimeRemaining { get => this.recordingManager?.TimeRemaining; }

        public void StartRecording()
        {
            // App.Current.Services.GetService<TriggerManagerViewModel>().ResetStatuses(); // TODO
            Task.Run(() =>
            {
                this.recordingManager.Start();
            }).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Serilog.Log.Logger.Error(t.Exception, "Error encountered during recording start. Aborting....");
                    this.recordingManager.Abort();
                    this.rootViewModel.Commands.ShowRecordingErrorDialog.Execute(new RecordingErrorData(t.Exception));
                }
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.FromCurrentSynchronizationContext());
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
            /* TODO: try to find a better way here.
             * so running this via task.run seems to introduce a race condition where the recorder
             * can escape aborting. running sync seems to fix, but kills interactivity during this step.
             * seems only to affect SLOW / low resource systems
             * 
             */
            //Task.Run(() =>
            //{
                this.recordingManager.Abort();
            //});
        }
        #endregion
    }

    public class RecordingErrorData
    {
         
        public RecordingErrorData(Exception e)
        {
            this.Exception = e;
        }
        public Exception Exception { get; set; }
    }
}
