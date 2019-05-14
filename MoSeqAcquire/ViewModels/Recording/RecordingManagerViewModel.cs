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
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.ViewModels.MediaSources;

namespace MoSeqAcquire.ViewModels.Recording
{
    public class AvailableRecordingChannelsProvider : ObservableCollection<SelectableChannelViewModel>
    {
        public AvailableRecordingChannelsProvider(MediaSourceCollectionViewModel MediaSources)
        {
            MediaSources.Items.CollectionChanged += Items_CollectionChanged;
            MediaSources.Items
                        .SelectMany(s => s.Channels.Select(c => new SelectableChannelViewModel(c)))
                        .ForEach(scvm => this.Add(scvm));
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            e.NewItems?.Cast<MediaSourceViewModel>().ForEach(msvm => msvm.Channels.CollectionChanged += Channels_CollectionChanged);
            e.OldItems?.Cast<MediaSourceViewModel>().ForEach(msvm => msvm.Channels.CollectionChanged -= Channels_CollectionChanged);
        }

        private void Channels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            e.NewItems?.Cast<ChannelViewModel>().Select(cvm => new SelectableChannelViewModel(cvm)).ForEach(scvm => this.Add(scvm));

            if (e.OldItems != null)
            {
                var toRemove = this.Where(scvm => e.OldItems.Cast<ChannelViewModel>().Contains(scvm.Channel)).ToList();
                toRemove.ForEach(scvm => this.Remove(scvm));
            }
        }
    }

    public class RecordingManagerViewModel : BaseViewModel, INotifyDataErrorInfo, IDataErrorInfo
    {
        protected MoSeqAcquireViewModel rootViewModel;
        protected ObservableCollection<RecorderViewModel> recorders;
        protected RecorderViewModel selectedRecorder;
        protected RecordingManager recordingManager;

        

        public RecordingManagerViewModel(MoSeqAcquireViewModel RootViewModel) : base()
        {
            this.rootViewModel = RootViewModel;
            this.recorders = new ObservableCollection<RecorderViewModel>();

            this.recordingManager = new RecordingManager(this.rootViewModel.TriggerBus, RootViewModel.RecordingMetadata);
            this.recordingManager.BeforeStartRecording += (s, e) => this.Root.ForceProtocolLocked();
            this.recordingManager.RecordingFinished += (s, e) => this.Root.UndoForceProtocolLocked();
            this.recordingManager.RecordingFinished += (s, e) => this.RecordingMetadata.Items.ResetValuesToDefaults();
            this.recordingManager.RecordingAborted += (s, e) => this.Root.UndoForceProtocolLocked();
            this.recordingManager.PropertyChanged += (s, e) =>
            {
                this.NotifyPropertyChanged(null);
            };

            this.GeneralSettings.ErrorsChanged += SubItemErrorsChanged;
            this.RecordingMetadata.Items.ErrorsChanged += SubItemErrorsChanged;
        }

        private void SubItemErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            this.ErrorsChanged?.Invoke(this, e);
            this.NotifyPropertyChanged(nameof(this.Error));
            this.NotifyPropertyChanged(nameof(this.HasErrors));
        }

        public MoSeqAcquireViewModel Root { get => this.rootViewModel; }
        public GeneralRecordingSettings GeneralSettings
        {
            get => this.recordingManager.GeneralSettings;
        }
        public MetadataViewModel RecordingMetadata
        {
            get => this.recordingManager.RecordingMetadata;
        }

        #region Recorder CRUD Operations
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
            this.Root.Triggers.ResetStatuses();
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
        #endregion
    }
}
