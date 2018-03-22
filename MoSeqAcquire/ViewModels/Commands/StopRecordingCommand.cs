using MoSeqAcquire.Models.IO;
using MoSeqAcquire.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class StopRecordingCommand : BaseCommand
    {
        public StopRecordingCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return this.ViewModel.Recorder.IsRecording == true;
        }

        public override void Execute(object parameter)
        {
            this.ViewModel.Recorder.StopRecording();
        }
    }
}
