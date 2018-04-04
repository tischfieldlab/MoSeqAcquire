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
            this.RegisterCommand<OpenThemeDialogCommand>();

            this.RegisterCommand<AddRecorderCommand>();
            this.RegisterCommand<EditRecorderCommand>();
            this.RegisterCommand<RemoveRecorderCommand>();

            this.RegisterCommand<StartRecordingCommand>();
            this.RegisterCommand<StopRecordingCommand>();

            this.RegisterCommand<LoadProtocolCommand>();
            this.RegisterCommand<SaveProtocolCommand>();

            this.RegisterCommand<AddMediaSourceCommand>();
            this.RegisterCommand<RemoveMediaSourceCommand>();
        }

        public OpenThemeDialogCommand OpenThemeDialog { get => this.GetCommand<OpenThemeDialogCommand>(); }

        public AddRecorderCommand AddRecorder { get => this.GetCommand<AddRecorderCommand>(); }
        public EditRecorderCommand EditRecorder { get => this.GetCommand<EditRecorderCommand>(); }
        public RemoveRecorderCommand RemoveRecorder { get => this.GetCommand<RemoveRecorderCommand>(); }

        public StartRecordingCommand StartRecording { get => this.GetCommand<StartRecordingCommand>(); }
        public StopRecordingCommand StopRecording { get => this.GetCommand<StopRecordingCommand>(); }
        public LoadProtocolCommand LoadProtocol { get => this.GetCommand<LoadProtocolCommand>(); }
        public SaveProtocolCommand SaveProtocol { get => this.GetCommand<SaveProtocolCommand>(); }

        public AddMediaSourceCommand AddMediaSource { get => this.GetCommand<AddMediaSourceCommand>(); }
        public RemoveMediaSourceCommand RemoveMediaSource { get => this.GetCommand<RemoveMediaSourceCommand>(); }



    }
}
