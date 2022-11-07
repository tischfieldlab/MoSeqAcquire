using MoSeqAcquire.ViewModels.Performance;
using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Views.Metadata;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class ShowMetadataDefinitionWindowCommand : BaseCommand
    {
        private MetadataEditorWindow metadataEditorWindow;

        public ShowMetadataDefinitionWindowCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
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
            if (this.metadataEditorWindow == null)
            {
                this.metadataEditorWindow = new MetadataEditorWindow
                {
                    DataContext = this.ViewModel
                };
                this.metadataEditorWindow.Closing += (sender, args) =>
                {
                    (sender as MetadataEditorWindow).Hide();
                    args.Cancel = true;
                };
            }
            this.metadataEditorWindow.Show();
        }
    }
}
