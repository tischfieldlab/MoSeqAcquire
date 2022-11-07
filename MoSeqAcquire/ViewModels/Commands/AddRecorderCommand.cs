using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class AddRecorderCommand : BaseCommand
    {
        public AddRecorderCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
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
            var dialog = new RecorderEditor
            {
                DataContext = new RecorderEditorViewModel(this.ViewModel, null)
            };
            dialog.ShowDialog();
        }
    }
}
