using MoSeqAcquire.Models.Triggers;
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
    public class EditTriggerActionConfigCommand : BaseCommand
    {
        public EditTriggerActionConfigCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            if (this.ViewModel.Protocol.IsProtocolLocked)
                return false;

            var vm = this.GetTriggerActionViewModel(parameter);
            return vm != null;
        }

        public override void Execute(object parameter)
        {
            var vm = this.GetTriggerActionViewModel(parameter);
            if (vm != null)
            {
                var tbvm = this.ViewModel.Triggers.FindBindingForAction(vm);
                var dialog = new TriggerActionEditorWindow
                {
                    DataContext = new TriggerActionEditorViewModel(this.ViewModel, tbvm, vm)
                };
                dialog.ShowDialog();
            }
        }

        protected TriggerActionViewModel GetTriggerActionViewModel(object parameter)
        {
            TriggerActionViewModel viewModel = null;
            if (parameter is TriggerActionViewModel)
            {
                viewModel = parameter as TriggerActionViewModel;
            }
            //else if (this.ViewModel.Triggers.SelectedTrigger != null)
            //{
            //    viewModel = this.ViewModel.Triggers.SelectedTrigger;
            //} // TODO
            return viewModel;
        }
    }
}
