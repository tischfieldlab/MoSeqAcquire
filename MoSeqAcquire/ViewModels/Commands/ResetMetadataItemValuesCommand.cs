using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.ViewModels.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.ViewModels.Metadata;
using MaterialDesignThemes.Wpf;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class ResetMetadataItemValuesCommand : BaseCommand
    {
        public ResetMetadataItemValuesCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
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
            //var result = MaterialDesignThemes.Wpf.DialogHost.Show(parameter);//
            foreach (var item in this.ViewModel.Recorder.RecordingMetadata.Items)
            {
                item.ResetValue();
            }
            this.ViewModel.IsDialogOpen = false;
        }
    }
}
