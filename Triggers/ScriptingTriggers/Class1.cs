using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.Triggers;

namespace ScriptingTriggers
{
    [DisplayName("Run Script Block")]
    [SettingsImplementation(typeof(RunScriptBlockSettings))]
    [DesignerImplementation(typeof(ScriptingTriggersDesigner))]
    public class RunScriptBlock : TriggerAction
    {
        protected override Action<Trigger> Action => throw new NotImplementedException();
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
