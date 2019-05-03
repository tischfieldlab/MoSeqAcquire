using MaterialDesignThemes.Wpf;
using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.ViewModels.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MoSeqAcquire.ViewModels.MediaSources;
using MoSeqAcquire.Views;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class OpenMediaSourceConfigCommand : BaseCommand
    {
        public OpenMediaSourceConfigCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
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
                var mscd = new MediaSourceConfigView()
                {
                    DataContext = parameter as MediaSourceViewModel
                };
                mscd.ShowDialog();
            }
        }
    }
}
