using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MoSeqAcquire.Models.Metadata;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class RemoveMetadataItemCommand : BaseCommand
    {
        public RemoveMetadataItemCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
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
            this.ViewModel.Recorder.RecordingMetadata.Items.Remove(parameter as MetadataItemDefinition);
        }
    }
}
