using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Triggers;
using MoSeqAcquire.ViewModels.Recording;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MoSeqAcquire.Models.Recording;

namespace MoSeqAcquire.Models.Acquisition
{
    [DisplayName("Frame Captured")]
    [SettingsImplementation(typeof(FrameCapturedTriggerConfig))]
    public class FrameCapturedTrigger : TriggerEvent
    {
        protected Channel listeningChannel;
        protected bool onlyWhileRecording;
        protected RecordingManagerViewModel rmvm;

        protected void handler(object sender, EventArgs eventArgs)
        {
            if (this.onlyWhileRecording && this.rmvm.State == RecordingManagerState.Recording)
                this.Fire();
        }

        public override void Start()
        {
            var config = this.Settings as FrameCapturedTriggerConfig;
            this.rmvm = App.Current.Services.GetService<RecordingManagerViewModel>();
            this.onlyWhileRecording = config.OnlyWhileRecording;
            this.listeningChannel = MediaBus.Instance.Channels.FirstOrDefault((bc) => bc.Channel.FullName == config.Channel)?.Channel;
            if (this.listeningChannel != null)
            {
                this.listeningChannel.FrameCaptured += this.handler;
                this.OnExecutionStarted();
            }
            else
            {
                this.rmvm = null;
                this.OnExecutionFaulted(new ArgumentException($"Unable to find channel \"{config.Channel}\""));
            }
        }

        public override void Stop()
        {
            this.listeningChannel.FrameCaptured -= this.handler;
            this.listeningChannel = null;
            this.rmvm = null;
            this.OnExecutionFinished();
        }
    }

    public class FrameCapturedTriggerConfig : TriggerEventConfig
    {
        [DisplayName("Only Fire While Recording")]
        [DefaultValue(true)]
        public bool OnlyWhileRecording
        {
            get => this.onlyWhileRecording;
            set => this.SetField(ref this.onlyWhileRecording, value);
        }
        protected bool onlyWhileRecording;


        [DisplayName("Channel")]
        [ChoicesMethod("DiscoverChannels")]
        public string Channel
        {
            get => this.channel;
            set => this.SetField(ref this.channel, value);
        }
        protected string channel;



        public static string[] DiscoverChannels()
        {
            return MediaBus.Instance.Sources.SelectMany((ms) => ms.Channels.Select((c) => c.FullName)).ToArray();
        }
    }
}
