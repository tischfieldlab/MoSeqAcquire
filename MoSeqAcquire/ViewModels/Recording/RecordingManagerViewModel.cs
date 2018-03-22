using MoSeqAcquire.Models.IO;
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
        protected ObservableCollection<RecorderViewModel> recorders;
        protected RecorderViewModel selectedRecorder;
        protected BaseRecordingSettingsViewModel settings;
        protected bool isRecording;

        protected static List<string> _inheritableSettings = new List<string>() { "Directory", "Basename" };


        public RecordingManagerViewModel(MoSeqAcquireViewModel RootViewModel)
        {
            this.rootViewModel = RootViewModel;
            this.recorders = new ObservableCollection<RecorderViewModel>();
            this.settings = new BaseRecordingSettingsViewModel();
            this.settings.PropertyChanged += (s, e) =>
            {
                if (_inheritableSettings.Contains(e.PropertyName))
                {

                }
            };
        }
        public MoSeqAcquireViewModel Root { get => this.rootViewModel; }
        
        public ObservableCollection<RecorderViewModel> Recorders { get => this.recorders; }
        
        public RecorderViewModel SelectedRecorder
        {
            get => this.selectedRecorder;
            set => this.SetField(ref this.selectedRecorder, value);
        }
        public bool IsRecording
        {
            get => this.isRecording;
            set => this.SetField(ref this.isRecording, value);
        }
        public BaseRecordingSettingsViewModel Settings { get => this.settings; }

        protected List<IMediaWriter> _realWriters;
        public void StartRecording()
        {
            this.IsRecording = true;
            this._realWriters = new List<IMediaWriter>();
            var settings = this.Settings;
            foreach (var r in this.Recorders)
            {
                this._realWriters.Add(r.MakeMediaWriter());
            }
            foreach (var r in this._realWriters)
            {
                r.Start();
            }
        }
        public void StopRecording()
        {
            foreach (var r in this._realWriters)
            {
                r.Stop();
            }
            this.IsRecording = false;
        }
    }
}
