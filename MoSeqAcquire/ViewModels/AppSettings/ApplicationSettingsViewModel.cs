using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.AppSettings
{
    public class ApplicationSettingsViewModel
    {
        public ApplicationSettingsViewModel()
        {
            this.SettingsParts = new ObservableCollection<BaseComponentSettingsViewModel>()
            {
                //new ThemeViewModel(),
                new SystemSettingsViewModel(),
                new RecordingSettingsViewModel(),
                new TriggerSettingsViewModel()
            };
            this.SettingsParts.ForEach(svm => svm.PropertyChanged += Svm_PropertyChanged);
       
        }

        private void Svm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        public ObservableCollection<BaseComponentSettingsViewModel> SettingsParts { get; protected set; }
    }
}
