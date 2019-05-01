using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.ViewModels.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.ViewModels.Metadata;
using MaterialDesignThemes.Wpf;
using MoSeqAcquire.Views.Controls;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class ResetMetadataItemValuesCommand : BaseCommand
    {
        public ResetMetadataItemValuesCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
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
            var view = new ConfirmDialog
            {
                DataContext = new ConfirmDialogViewModel()
                {
                    Title = "Confirm Reset of Values",
                    Message = "Are you sure you want to reset metadata values to defaults?"
                }
            };
            var result = await DialogHost.Show(view, "MainWindowDialogHost");

            if ((bool) result)
            {
                foreach (var item in this.ViewModel.Recorder.RecordingMetadata.Items)
                {
                    item.ResetValue();
                }
            }
        }
    }
}
