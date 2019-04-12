using MaterialDesignThemes.Wpf;
using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.ViewModels.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class ToggleOpenMediaSourceConfigCommand : BaseCommand
    {
        public ToggleOpenMediaSourceConfigCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
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
            if (parameter != null && parameter is MediaSourceViewModel)
            {
                var msvm = parameter as MediaSourceViewModel;
                msvm.IsConfigOpen = !msvm.IsConfigOpen;
            }
        }
    }
}
