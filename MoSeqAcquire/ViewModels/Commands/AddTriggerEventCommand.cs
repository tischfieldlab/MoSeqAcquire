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
    public class AddTriggerEventCommand : BaseCommand
    {
        public AddTriggerEventCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            if (this.ViewModel.Protocol.IsProtocolLocked)
                return false;
            return true;
        }

        public override void Execute(object parameter)
        {
            //this.ViewModel.Triggers.AddTrigger();
            var dialog = new TriggerEventEditorWindow
            {
                DataContext = new TriggerEventEditorViewModel(this.ViewModel, null)
            };
            dialog.ShowDialog();
        }
    }
}
