using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class CommandLibrary : BaseCommandLibrary<MoSeqAcquireViewModel>
    {
        public CommandLibrary(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        protected override void InitializeCommands()
        {
            this.RegisterCommand<AddRecorderCommand>();
            this.RegisterCommand<EditRecorderCommand>();

            this.RegisterCommand<StartRecordingCommand>();

            this.RegisterCommand<LoadProtocolCommand>();
            this.RegisterCommand<SaveProtocolCommand>();
        }
        

        public AddRecorderCommand AddRecorder { get => this.GetCommand<AddRecorderCommand>(); }
        public EditRecorderCommand EditRecorder { get => this.GetCommand<EditRecorderCommand>(); }
        public StartRecordingCommand StartRecording { get => this.GetCommand<StartRecordingCommand>(); }
        public LoadProtocolCommand LoadProtocol { get => this.GetCommand<LoadProtocolCommand>(); }
        public SaveProtocolCommand SaveProtocol { get => this.GetCommand<SaveProtocolCommand>(); }



    }
}
