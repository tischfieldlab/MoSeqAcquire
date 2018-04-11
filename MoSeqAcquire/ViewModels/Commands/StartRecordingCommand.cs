using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class StartRecordingCommand : BaseCommand
    {
        public StartRecordingCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return this.ViewModel.Recorder.IsRecording == false
                && this.ViewModel.Recorder.GeneralSettings.IsValid;
        }

        public override void Execute(object parameter)
        {
            this.ViewModel.Recorder.StartRecording();
        }
    }
}
