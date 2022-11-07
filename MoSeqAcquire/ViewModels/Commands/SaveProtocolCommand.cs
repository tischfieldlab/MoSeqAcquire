using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Recording;
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
            string path;
            if(parameter == null)
            {
                path = this.GetPath();
            }
            else
            {
                path = parameter as string;
            }
            if (path != null && !string.IsNullOrWhiteSpace(path))
            {
                //expects parameter to be string path to protocol
                MediaSettingsWriter.WriteProtocol(path, this.ViewModel.Protocol.GenerateProtocol());

                //push the path into the recent protocols collection
                this.ViewModel.Protocol.PushRecentProtocol(path);
            }
        }

        protected string GetPath()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                AddExtension = true,
                OverwritePrompt = true,
                Title = "Save Protocol As....",
                ValidateNames = true,
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
            var prep = string.Join(";", ext.Select(s => "*." + s));
            return desc + " (" + prep + ")|" + prep;
        }
    }
}
