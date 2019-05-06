using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MoSeqAcquire.Models.Metadata;
using MoSeqAcquire.ViewModels.Metadata;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class EditMetadataItemCommand : BaseCommand
    {
        public EditMetadataItemCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            if (this.ViewModel.IsProtocolLocked)
                return false;
            return true;
        }

        public override void Execute(object parameter)
        {
            this.ViewModel.Recorder.RecordingMetadata.CurrentItem = parameter as MetadataItemDefinition;
            this.ViewModel.Recorder.RecordingMetadata.CurrentState = MetadataViewState.Property;
        }
    }
}
