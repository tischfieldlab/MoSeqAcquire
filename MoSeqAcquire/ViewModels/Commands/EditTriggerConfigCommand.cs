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
            if (this.ViewModel.IsProtocolLocked)
                return false;

            var vm = this.GetTriggerViewModel(parameter);
            return vm != null;
        }

        public override void Execute(object parameter)
        {
            var vm = this.GetTriggerViewModel(parameter);
            if (vm != null)
            {
                var dialog = new TriggerEditorWindow
                {
                    DataContext = new TriggerEditorViewModel(this.ViewModel, vm)
                };
                dialog.ShowDialog();
            }
        }

        protected TriggerViewModel GetTriggerViewModel(object parameter)
        {
            TriggerViewModel viewModel = null;
            if (parameter is TriggerViewModel)
            {
                viewModel = parameter as TriggerViewModel;
            }
            else if (this.ViewModel.Triggers.SelectedTrigger != null)
            {
                viewModel = this.ViewModel.Triggers.SelectedTrigger;
            }
            return viewModel;
        }
    }
}
