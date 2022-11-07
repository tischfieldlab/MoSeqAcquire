using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Views.Controls;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class UnloadProtocolCommand : BaseCommand
    {
        public UnloadProtocolCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override async void Execute(object parameter)
        {
            var confirmVM = new ConfirmDialogViewModel()
            {
                Title = "Confirm Unload Protocol",
                Message = "Are you sure you want to unload the current protocol?"
            };
            var result = await DialogHost.Show(confirmVM, "MainWindowDialogHost");

            if ((bool)result)
            {
                this.ViewModel.Protocol.UnloadProtocol();
            }
        }
    }
}
