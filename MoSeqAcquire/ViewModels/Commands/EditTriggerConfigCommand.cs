using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.ViewModels.Triggers;
using MoSeqAcquire.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class EditTriggerConfigCommand : BaseCommand
    {
        public EditTriggerConfigCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            if(parameter != null && parameter is TriggerViewModel)
            {
                return true;
            }

            if (this.ViewModel.Triggers.SelectedTrigger != null)
            {
                return true;
            }
            return false;
        }

        public override void Execute(object parameter)
        {
            TriggerViewModel viewModel = null;
            if (parameter != null && parameter is TriggerViewModel)
            {
                viewModel = parameter as TriggerViewModel;
            }
            else if(this.ViewModel.Triggers.SelectedTrigger != null)
            {
                viewModel = this.ViewModel.Triggers.SelectedTrigger;
            }

            var dialog = new TriggerConfigView
            {
                DataContext = viewModel
            };
            dialog.ShowDialog();
        }
    }
}
