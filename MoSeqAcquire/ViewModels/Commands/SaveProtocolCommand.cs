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
                MediaSettingsWriter.WriteProtocol(path, this.ViewModel.GenerateProtocol());
            }
        }

        protected string GetPath()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.AddExtension = true;
            dlg.OverwritePrompt = true;
            dlg.Title = "Save Protocol As....";
            dlg.ValidateNames = true;
            /*if (editor.CurrentFilePath == null || editor.CurrentFilePath.Equals(""))
            {
                dlg.FileName = IWFFile.NewWorkflowDefaultName;
            }
            else
            {
                dlg.InitialDirectory = Path.GetDirectoryName(editor.CurrentFilePath);
                dlg.FileName = editor.CurrentFilePath;
            }*/
            dlg.DefaultExt = Protocol.Extension;
            dlg.Filter = this.GetFilterString(Protocol.TypeDesc, Protocol.Extension);
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
