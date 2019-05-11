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
        private TriggerExecutionMode triggersExecutionMode;

        public TriggerSettingsViewModel()
        {
            this.Header = "Triggers";
        }

        public TriggerExecutionMode TriggersExecutionMode
        {
            get => this.triggersExecutionMode;
            set
            {
                this.SetField(ref this.triggersExecutionMode, value);
                Properties.Settings.Default.TriggersExecutionMode = value;
            }
        }
    }
}
