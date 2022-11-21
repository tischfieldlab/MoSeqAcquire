using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.ViewModels.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Views;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class AddTriggerActionCommand : BaseCommand
    {
        public AddTriggerActionCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            if (this.ViewModel.Protocol.IsProtocolLocked)
                return false;

            if (this.GetTriggerBindingViewModel(parameter) == null)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var tbvm = this.GetTriggerBindingViewModel(parameter);
            var dialog = new TriggerActionEditorWindow
            {
                DataContext = new TriggerActionEditorViewModel(this.ViewModel, tbvm, null)
            };
            dialog.ShowDialog();
        }

        protected TriggerBindingViewModel GetTriggerBindingViewModel(object parameter)
        {
            TriggerBindingViewModel viewModel = null;
            if (parameter is TriggerBindingViewModel)
            {
                viewModel = parameter as TriggerBindingViewModel;
            }
            //else if (this.ViewModel.Triggers.SelectedTrigger != null)
            //{
            //    viewModel = this.ViewModel.Triggers.SelectedTrigger;
            //} // TODO
            return viewModel;
        }
    }
}
