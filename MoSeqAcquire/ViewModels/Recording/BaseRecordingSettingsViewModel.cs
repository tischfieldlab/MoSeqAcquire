using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Recording
{
    public class BaseRecordingSettingsViewModel : BaseViewModel, IConfigSnapshotProvider
    {
        protected string directory;
        protected string basename;
        protected RecordingMode recordingMode;
        protected int recordingFrameCount;
        protected int recordingSeconds;

        public BaseRecordingSettingsViewModel() { }
        public BaseRecordingSettingsViewModel(BaseRecordingSettingsViewModel settings)
        {
            this.directory = settings.directory;
            this.basename = settings.basename;
            this.recordingMode = settings.recordingMode;
            this.recordingFrameCount = settings.recordingFrameCount;
            this.recordingSeconds = settings.recordingSeconds;
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

        public virtual void ApplySnapshot(ConfigSnapshot snapshot)
        {
            var other = snapshot as RecorderSettings;
            this.Directory = other.Directory;
            this.Basename = other.Basename;
            this.RecordingMode = other.RecordingMode;
            this.RecordingFrameCount = other.RecordingFrameCount;
            this.RecordingSeconds = this.RecordingSeconds;
        }

        public ConfigSnapshot GetSnapshot()
        {
            return new RecorderSettings()
            {
                Directory = this.directory,
                Basename = this.basename,
                RecordingMode = this.RecordingMode,
                RecordingFrameCount = this.recordingFrameCount,
                RecordingSeconds = this.recordingSeconds
            };
        }
    }
}
