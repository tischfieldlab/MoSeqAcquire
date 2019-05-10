using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Triggers;

namespace ScriptingTriggers
{
    [DisplayName("Run Script Block")]
    [SettingsImplementation(typeof(RunScriptBlockSettings))]
    [DesignerImplementation(typeof(ScriptingTriggersDesigner))]
    public class RunScriptBlock : TriggerAction
    {
        protected override Action<Trigger> Action => delegate (Trigger trigger)
        {
            var settings = this.Settings as RunScriptBlockSettings;

            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();

            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            //info.RedirectStandardOutput = true;

            info.UseShellExecute = false;
            info.CreateNoWindow = false;
            if (settings.WorkingDirectory != null && !String.Empty.Equals(settings.WorkingDirectory))
            {
                info.WorkingDirectory = settings.WorkingDirectory;
            }

            p.StartInfo = info;
            //p.OutputDataReceived += new DataReceivedEventHandler(this.OutputRecievedHandler);

            p.Start();
            //p.BeginOutputReadLine();

            String[] command;
            command = settings.ScriptBlock.Split('\n');  

            StreamWriter sw = p.StandardInput;
            if (sw.BaseStream.CanWrite)
            {
                foreach (var c in command)
                {
                    sw.WriteLine(c);
                }
            }

            sw.Close();
            p.WaitForExit();
            p.Close();
        };
    }

    public class RunScriptBlockSettings : TriggerConfig
    {
        protected string workingDirectory;
        protected string scriptBlock;
        protected TextDocument codeDocument;

        public RunScriptBlockSettings() : base()
        {
            this.codeDocument = new TextDocument();
            this.codeDocument.TextChanged += (s, e) =>
            {
                this.scriptBlock = this.codeDocument.Text;
                this.NotifyPropertyChanged(nameof(this.ScriptBlock));
            };
        }

        public string WorkingDirectory
        {
            get => this.workingDirectory;
            set => this.SetField(ref this.workingDirectory, value);
        }
        public string ScriptBlock
        {
            get => this.scriptBlock;
            set
            {
                this.scriptBlock = value;
                this.CodeDocument.Text = value;
                this.NotifyPropertyChanged(nameof(this.CodeDocument));
                this.NotifyPropertyChanged(nameof(this.ScriptBlock));
            }
        }
        [Hidden]
        public TextDocument CodeDocument
        {
            get { return this.codeDocument; }
            set
            {
                this.codeDocument = value;
                this.NotifyPropertyChanged(nameof(this.CodeDocument));
                this.NotifyPropertyChanged(nameof(this.ScriptBlock));
            }
        }
    }
}
