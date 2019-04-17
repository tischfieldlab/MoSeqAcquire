using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.Models.Management;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class UnloadProtocolCommand : BaseCommand
    {
        public UnloadProtocolCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            this.ViewModel.UnloadProtocol();
        }
    }
}
