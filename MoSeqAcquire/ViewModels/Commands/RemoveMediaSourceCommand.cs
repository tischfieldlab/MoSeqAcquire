using MoSeqAcquire.Models.Management;
using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class RemoveMediaSourceCommand : BaseCommand
    {
        public RemoveMediaSourceCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            if (this.ViewModel.IsProtocolLocked)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            if(parameter != null && parameter is MediaSourceViewModel)
            {
                var msvm = parameter as MediaSourceViewModel;
                msvm.MediaSource.Stop();
                this.ViewModel.MediaSources.Remove(msvm);
            }
        }
    }
}
