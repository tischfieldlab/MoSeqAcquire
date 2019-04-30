using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.ViewModels.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.ViewModels.MediaSources;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class ToggleChannelEnabledCommand : BaseCommand
    {
        public ToggleChannelEnabledCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
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
            if (parameter != null && parameter is ChannelViewModel)
            {
                var msvm = parameter as ChannelViewModel;
                msvm.Enabled = !msvm.Enabled;
            }
        }
    }
}
