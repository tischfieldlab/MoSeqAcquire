using MoSeqAcquire.Models.Triggers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.AppSettings
{
    public class SystemSettingsViewModel : BaseComponentSettingsViewModel
    {
        public SystemSettingsViewModel()
        {
            this.Header = "System";
        }

        public StringCollection PluginPaths
        {
            get => Properties.Settings.Default.PluginPaths;
        }
    }
}
