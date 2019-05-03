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
using MoSeqAcquire.Properties;

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
            if(parameter == null || parameter is string)
            {
                string path = parameter as string;
                if(string.IsNullOrWhiteSpace(path))
                {
                    path = this.RequestPath();
                }
                try
                {
                    pcol = MediaSettingsWriter.ReadProtocol(path as string);

                    //push the path into the recent protocols collection
                    this.ViewModel.PushRecentProtocol(path);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error loading protocol \"" + path + "\":\n\n" + e.Message);
                }
                
            }
            else if(parameter is Protocol)
            {
                pcol = parameter as Protocol;
            }

            if (pcol == null)
            {
                pcol = ProtocolExtensions.GetDefaultProtocol();
                //return;
            }
            this.ViewModel.ApplyProtocol(pcol);
        }

        protected string RequestPath()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = false,
                DefaultExt = Protocol.Extension,
                Filter = this.GetFilterString(Protocol.TypeDesc, Protocol.Extension)
            };
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                return dlg.FileName;
            }
            return null;
        }

        protected string GetFilterString(string desc, params string[] ext)
        {
            var prep =string.Join(";", ext.Select(s => "*." + s));
            return desc+" (" + prep + ")|" + prep;
        }


    }
}
