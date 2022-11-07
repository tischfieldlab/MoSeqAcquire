using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.ViewModels.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using MoSeqAcquire.Views.Controls;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class RemoveTriggerCommand : BaseCommand
    {
        public RemoveTriggerCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            if (this.ViewModel.Protocol.IsProtocolLocked)
                return false;

            if (parameter != null && parameter is TriggerViewModel)
            {
                return true;
            }

            if (this.ViewModel.Triggers.SelectedTrigger != null)
            {
                return true;
            }
            return false;
        }

        public override async void Execute(object parameter)
        {
            TriggerViewModel viewModel = null;
            if (parameter != null && parameter is TriggerViewModel)
            {
                viewModel = parameter as TriggerViewModel;
            }
            else if (this.ViewModel.Triggers.SelectedTrigger != null)
            {
                viewModel = this.ViewModel.Triggers.SelectedTrigger;
            }

            if (viewModel != null)
            {
                var confirmVM = new ConfirmDialogViewModel()
                {
                    Title = "Confirm Remove Trigger",
                    Message = "Are you sure you want to remove this trigger?"
                };
                var result = await DialogHost.Show(confirmVM, "MainWindowDialogHost");

                if ((bool) result)
                {
                    this.ViewModel.Triggers.RemoveTrigger(viewModel);
                }
            }
        }
    }
}
