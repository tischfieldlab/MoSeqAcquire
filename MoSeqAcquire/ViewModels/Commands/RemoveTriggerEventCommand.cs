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
    public class RemoveTriggerEventCommand : BaseCommand
    {
        public RemoveTriggerEventCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            if (this.ViewModel.Protocol.IsProtocolLocked)
                return false;

            if (parameter != null && parameter is TriggerEventViewModel)
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
            TriggerEventViewModel viewModel = null;
            if (parameter != null && parameter is TriggerEventViewModel)
            {
                viewModel = parameter as TriggerEventViewModel;
            }
            //else if (this.ViewModel.Triggers.SelectedTrigger != null)
            //{
            //    viewModel = this.ViewModel.Triggers.SelectedTrigger;
            //} //TODO

            if (viewModel != null)
            {
                var confirmVM = new ConfirmDialogViewModel()
                {
                    Title = "Confirm Remove Trigger Event",
                    Message = "Are you sure you want to remove this trigger event? This will also remove all associated Actions!"
                };
                var result = await DialogHost.Show(confirmVM, "MainWindowDialogHost");

                if ((bool) result)
                {
                    var binding = this.ViewModel.Triggers.FindBindingForEvent(viewModel);
                    this.ViewModel.Triggers.RemoveTrigger(binding); //TODO
                }
            }
        }
    }
}
