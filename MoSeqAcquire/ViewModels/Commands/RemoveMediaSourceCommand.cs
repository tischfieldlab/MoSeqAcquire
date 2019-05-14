using MoSeqAcquire.Models.Management;
using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;
using MoSeqAcquire.ViewModels.MediaSources;
using MoSeqAcquire.Views.Controls;

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

        public override async void Execute(object parameter)
        {
            if (parameter != null && parameter is MediaSourceViewModel)
            {
                var view = new ConfirmDialog
                {
                    DataContext = new ConfirmDialogViewModel()
                    {
                        Title = "Confirm Remove Source",
                        Message = "Are you sure you want to remove this media source?"
                    }
                };
                var result = await DialogHost.Show(view, "MainWindowDialogHost");

                if ((bool)result)
                {
                    var msvm = parameter as MediaSourceViewModel;
                    msvm.Shutdown();
                    this.ViewModel.MediaSources.Items.Remove(msvm);
                }
            }
        }
    }
}
