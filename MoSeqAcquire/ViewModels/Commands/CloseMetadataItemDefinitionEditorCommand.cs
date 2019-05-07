using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.ViewModels.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.ViewModels.Metadata;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class CloseMetadataItemDefinitionEditorCommand : BaseCommand
    {
        public CloseMetadataItemDefinitionEditorCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            this.ViewModel.Recorder.RecordingMetadata.CurrentState = MetadataViewState.Grid;
        }
    }
}
