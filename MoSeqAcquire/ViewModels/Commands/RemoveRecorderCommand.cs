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
            if(this.ViewModel.Recorder.SelectedRecorder == null)
            {
                return false;
            }
            return true;
        }

        public override void Execute(object parameter)
        {            
            this.ViewModel.Recorder.RemoveSelectedRecorder();
        }
    }
}
