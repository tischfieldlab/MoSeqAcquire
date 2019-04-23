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
            this.RegisterCommand<ShowPerformanceMonitorCommand>();

            this.RegisterCommand<AddRecorderCommand>();
            this.RegisterCommand<EditRecorderCommand>();
            this.RegisterCommand<RemoveRecorderCommand>();

            this.RegisterCommand<AddTriggerCommand>();
            this.RegisterCommand<EditTriggerConfigCommand>();
            this.RegisterCommand<RemoveTriggerCommand>();

            this.RegisterCommand<StartRecordingCommand>();
            this.RegisterCommand<StopRecordingCommand>();

            this.RegisterCommand<LoadProtocolCommand>();
            this.RegisterCommand<SaveProtocolCommand>();
            this.RegisterCommand<UnloadProtocolCommand>();

            this.RegisterCommand<AddMediaSourceCommand>();
            this.RegisterCommand<RemoveMediaSourceCommand>();

            this.RegisterCommand<ToggleProtocolLockCommand>();
            this.RegisterCommand<ToggleOpenMediaSourceConfigCommand>();
            this.RegisterCommand<ToggleChannelEnabledCommand>();

            this.RegisterCommand<AddMetadataItemCommand>();
            this.RegisterCommand<EditMetadataItemCommand>();
            this.RegisterCommand<RemoveMetadataItemCommand>();
            this.RegisterCommand<ShowMetadataDefinitionWindowCommand>();
            this.RegisterCommand<CloseMetadataItemDefinitionEditorCommand>();
        }

        public OpenThemeDialogCommand OpenThemeDialog { get => this.GetCommand<OpenThemeDialogCommand>(); }
        public ShowPerformanceMonitorCommand ShowPerformanceMonitor { get => this.GetCommand<ShowPerformanceMonitorCommand>(); }

        public AddRecorderCommand AddRecorder { get => this.GetCommand<AddRecorderCommand>(); }
        public EditRecorderCommand EditRecorder { get => this.GetCommand<EditRecorderCommand>(); }
        public RemoveRecorderCommand RemoveRecorder { get => this.GetCommand<RemoveRecorderCommand>(); }

        public AddTriggerCommand AddTrigger { get => this.GetCommand<AddTriggerCommand>(); }
        public EditTriggerConfigCommand EditTriggerConfig { get => this.GetCommand<EditTriggerConfigCommand>(); }
        public RemoveTriggerCommand RemoveTrigger { get => this.GetCommand<RemoveTriggerCommand>(); }

        public StartRecordingCommand StartRecording { get => this.GetCommand<StartRecordingCommand>(); }
        public StopRecordingCommand StopRecording { get => this.GetCommand<StopRecordingCommand>(); }
        public LoadProtocolCommand LoadProtocol { get => this.GetCommand<LoadProtocolCommand>(); }
        public SaveProtocolCommand SaveProtocol { get => this.GetCommand<SaveProtocolCommand>(); }
        public UnloadProtocolCommand UnloadProtocol { get => this.GetCommand<UnloadProtocolCommand>(); }

        public AddMediaSourceCommand AddMediaSource { get => this.GetCommand<AddMediaSourceCommand>(); }
        public RemoveMediaSourceCommand RemoveMediaSource { get => this.GetCommand<RemoveMediaSourceCommand>(); }

        public ToggleProtocolLockCommand ToggleProtocolLock { get => this.GetCommand<ToggleProtocolLockCommand>(); }
        public ToggleOpenMediaSourceConfigCommand ToggleOpenMediaSourceConfig { get => this.GetCommand<ToggleOpenMediaSourceConfigCommand>(); }
        public ToggleChannelEnabledCommand ToggleChannelEnabled { get => this.GetCommand<ToggleChannelEnabledCommand>(); }

        public AddMetadataItemCommand AddMetadataItem { get => this.GetCommand<AddMetadataItemCommand>(); }
        public EditMetadataItemCommand EditMetadataItem { get => this.GetCommand<EditMetadataItemCommand>(); }
        public RemoveMetadataItemCommand RemoveMetadataItem { get => this.GetCommand<RemoveMetadataItemCommand>(); }
        public ShowMetadataDefinitionWindowCommand ShowMetadataDefinitionWindow { get => this.GetCommand<ShowMetadataDefinitionWindowCommand>(); }
        public CloseMetadataItemDefinitionEditorCommand CloseMetadataItemDefinitionEditor { get => this.GetCommand<CloseMetadataItemDefinitionEditorCommand>(); }

    }
}
