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
    public class EditTriggerEventConfigCommand : BaseCommand
    {
        public EditTriggerEventConfigCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            if (this.ViewModel.Protocol.IsProtocolLocked)
                return false;

            var vm = this.GetTriggerEventViewModel(parameter);
            return vm != null;
        }

        public override void Execute(object parameter)
        {
            var vm = this.GetTriggerEventViewModel(parameter);
            if (vm != null)
            {
                var dialog = new TriggerEventEditorWindow
                {
                    DataContext = new TriggerEventEditorViewModel(this.ViewModel, vm)
                };
                dialog.ShowDialog();
            }
        }

        protected TriggerEventViewModel GetTriggerEventViewModel(object parameter)
        {
            TriggerEventViewModel viewModel = null;
            if (parameter is TriggerEventViewModel)
            {
                viewModel = parameter as TriggerEventViewModel;
            }
            //else if (this.ViewModel.Triggers.SelectedTrigger != null)
            //{
            //    viewModel = this.ViewModel.Triggers.SelectedTrigger;
            //} // TODO
            return viewModel;
        }
    }
}
