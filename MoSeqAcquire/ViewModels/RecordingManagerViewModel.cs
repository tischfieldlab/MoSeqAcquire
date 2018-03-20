using MoSeqAcquire.Models.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels
{
    
    public class RecordingManagerViewModel : BaseViewModel
    {
        protected MoSeqAcquireViewModel rootViewModel;
        protected ObservableCollection<RecorderViewModel> recorders;
        protected RecorderViewModel selectedRecorder;
        protected bool isRecording;
        protected string directory;
        protected string basename;
        protected RecordingMode recordingMode;
        protected int recordingFrameCount;
        protected int recordingSeconds;

        public RecordingManagerViewModel(MoSeqAcquireViewModel RootViewModel)
        {
            this.rootViewModel = RootViewModel;
            this.recorders = new ObservableCollection<RecorderViewModel>();
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
        public string Directory
        {
            get => this.directory;
            set => this.SetField(ref this.directory, value);
        }
        public string Basename
        {
            get => this.basename;
            set => this.SetField(ref this.basename, value);
        }
        public RecordingMode RecordingMode
        {
            get => this.recordingMode;
            set => this.SetField(ref this.recordingMode, value);
        }
        public int RecordingFrameCount
        {
            get => this.recordingFrameCount;
            set => this.SetField(ref this.recordingFrameCount, value);
        }
        public int RecordingSeconds
        {
            get => this.recordingSeconds;
            set => this.SetField(ref this.recordingFrameCount, value);
        }
    }
}
