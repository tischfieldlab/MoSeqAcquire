using MoSeqAcquire.Models.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.AppSettings
{
    public class TriggerSettingsViewModel : BaseComponentSettingsViewModel
    {
        

        public TriggerSettingsViewModel()
        {
            this.Header = "Triggers";
        }

        public TriggerExecutionMode TriggersExecutionMode
        {
            get => (TriggerExecutionMode)Enum.Parse(typeof(TriggerExecutionMode), Properties.Settings.Default.TriggersExecutionMode);
            set
            {
                Properties.Settings.Default.TriggersExecutionMode = value.ToString();
                this.NotifyPropertyChanged();
            }
        }
    }
}
