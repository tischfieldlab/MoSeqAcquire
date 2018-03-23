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
            this.ViewModel.Recorder.AddRecorder();
            this.ViewModel.Commands.EditRecorder.Execute(parameter);
        }
    }
}
