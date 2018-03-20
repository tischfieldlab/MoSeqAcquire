using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.IO;
using MoSeqAcquire.Models.Management;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class SaveProtocolCommand : BaseCommand
    {
        public SaveProtocolCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            //expects parameter to be string path to protocol
            MediaSettingsWriter.WriteProtocol(parameter as string, this.ViewModel.GenerateProtocol());
        }

        
    }
}
