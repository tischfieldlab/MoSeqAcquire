using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Recording
{
    public class RecorderSettingsViewModel : BaseRecordingSettingsViewModel
    {
        public RecorderSettingsViewModel() : base() { }
        public RecorderSettingsViewModel(BaseRecordingSettingsViewModel settings) : base(settings)
        {

        }

        public override void ApplySnapshot(ConfigSnapshot snapshot)
        {
            base.ApplySnapshot(snapshot);
        }

        public override ConfigSnapshot GetSnapshot()
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
