using MoSeqAcquire.ViewModels.Recording;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class RemoveRecorderCommand : BaseCommand
    {
        public RemoveRecorderCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            var rvm = this.GetRecorderViewModel(parameter);
            return !(rvm == null);
        }

        public override void Execute(object parameter)
        {
            var rvm = this.GetRecorderViewModel(parameter);
            this.ViewModel.Recorder.RemoveRecorder(rvm);
        }
        protected RecorderViewModel GetRecorderViewModel(object parameter)
        {
            RecorderViewModel recorderViewModel = null;
            if (parameter != null && parameter is RecorderViewModel)
            {
                recorderViewModel = parameter as RecorderViewModel;
            }
            else if (this.ViewModel.Recorder.SelectedRecorder != null)
            {
                recorderViewModel = this.ViewModel.Recorder.SelectedRecorder;
            }
            return recorderViewModel;
        }
    }
}
