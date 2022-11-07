using MoSeqAcquire.Models.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Recording;

namespace MoSeqAcquire.ViewModels.AppSettings
{
    public class RecordingSettingsViewModel : BaseComponentSettingsViewModel
    {
        public RecordingSettingsViewModel()
        {
            this.Header = "Recordings";
        }
        public RecordingSummaryOutputFormat RecordingSummaryOutputFormat
        {
            get => (RecordingSummaryOutputFormat)Enum.Parse(typeof(RecordingSummaryOutputFormat), Properties.Settings.Default.RecordingSummaryOutputFormat, true);
            set
            {
                Properties.Settings.Default.RecordingSummaryOutputFormat = value.ToString();
                this.NotifyPropertyChanged();
            }
        }
    }
}
