using MoSeqAcquire.ViewModels.Recording;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class AddRecorderCommand : BaseCommand
    {
        public AddRecorderCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var recorder = new RecorderViewModel(this.ViewModel, this.ViewModel.Recorder.Settings);
            recorder.Name = "New Recorder";
            this.ViewModel.Recorder.Recorders.Add(recorder);
            this.ViewModel.Recorder.SelectedRecorder = recorder;
            this.ViewModel.Commands.EditRecorder.Execute(parameter);
        }
    }
}
