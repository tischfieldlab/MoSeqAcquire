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
    public class LoadProtocolCommand : BaseCommand
    {
        public LoadProtocolCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            //expects parameter to be string path to protocol, or protocol instance
            Protocol pcol = null;
            if(parameter is string)
            {
                try
                {
                    pcol = MediaSettingsWriter.ReadProtocol(parameter as string);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else if(parameter is Protocol)
            {
                pcol = parameter as Protocol;
            }

            if (pcol == null)
            {
                pcol = ProtocolExtensions.GetDefaultProtocol();
            }
            this.ViewModel.ApplyProtocol(pcol);
        }

        
    }
}
