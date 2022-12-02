using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.ViewModels.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using MoSeqAcquire.Views.Controls;
using MoSeqAcquire.Models.Triggers;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class RemoveTriggerActionCommand : BaseCommand
    {
        public RemoveTriggerActionCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            if (this.ViewModel.Protocol.IsProtocolLocked)
                return false;

            if (parameter != null && parameter is TriggerActionViewModel)
            {
                return true;
            }

            //if (this.ViewModel.Triggers.SelectedTrigger != null)
            //{
            //    return true;
            //} // TODO
            return false;
        }

        public override async void Execute(object parameter)
        {
            TriggerActionViewModel viewModel = null;
            if (parameter != null && parameter is TriggerActionViewModel)
            {
                viewModel = parameter as TriggerActionViewModel;
            }
            //else if (this.ViewModel.Triggers.SelectedTrigger != null)
            //{
            //    viewModel = this.ViewModel.Triggers.SelectedTrigger;
            //} // TODO

            if (viewModel != null)
            {
                var confirmVM = new ConfirmDialogViewModel()
                {
                    Title = "Confirm Remove Trigger Action",
                    Message = "Are you sure you want to remove this trigger action?"
                };
                var result = await DialogHost.Show(confirmVM, "MainWindowDialogHost");

                if ((bool) result)
                {
                    this.ViewModel.Triggers.RemoveTriggerAction(viewModel); // TODO
                }
            }
        }
    }
}
