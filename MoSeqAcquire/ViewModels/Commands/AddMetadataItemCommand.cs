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
    public class AddMetadataItemCommand : BaseCommand
    {
        public AddMetadataItemCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
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
            var itm = new MetadataItem("New Item", typeof(string)) { DefaultValue = "Some Value" };
            this.ViewModel.Recorder.RecordingMetadata.Items.Add(itm);
            this.ViewModel.Commands.EditMetadataItem.Execute(itm);
        }
    }
}
