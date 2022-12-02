using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.DAQmx;
using System.Diagnostics;
using System.ComponentModel;
using MoSeqAcquire;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace NationalInstruments
{
    [DisplayName("Listen TTL Signal")]
    [SettingsImplementation(typeof(ListenTTLSignalConfig))]
    public class ListenTTLSignal : TriggerEvent
    {
        protected System.Threading.Tasks.Task backgroundListenTask;
        protected CancellationTokenSource backgroundListenTaskCancellationTokenSource;

        protected void ListenForTTL(CancellationToken cancellationToken)
        {
            var settings = this.Settings as ListenTTLSignalConfig;

            using (DAQmx.Task readTask = new DAQmx.Task())
            {
                readTask.DIChannels.CreateChannel(settings.DeviceName, "", ChannelLineGrouping.OneChannelForEachLine);

                // Create the reader
                var digitalReader = new DigitalSingleChannelReader(readTask.Stream);

                // Start the task
                readTask.Start();

                bool lastState = digitalReader.ReadSingleSampleSingleLine();
                bool initState = lastState;
                while (!cancellationToken.IsCancellationRequested)
                {
                    bool currState = digitalReader.ReadSingleSampleSingleLine();
                    if (currState != initState && currState != lastState)
                    {
                        this.Fire();
                    }
                    lastState = currState;
                }
                readTask.Stop();
            }
        }

        public override void Start()
        {
            this.backgroundListenTaskCancellationTokenSource = new CancellationTokenSource();
            CancellationToken ct = backgroundListenTaskCancellationTokenSource.Token;
            this.backgroundListenTask = System.Threading.Tasks.Task.Run(() => this.ListenForTTL(ct), ct)
                .ContinueWith((antecedant) => this.Cleanup())
                .ContinueWith((antecedant) => this.OnExecutionFinished(""), TaskContinuationOptions.OnlyOnRanToCompletion)
                .ContinueWith((antecedant) => this.OnExecutionFaulted(antecedant.Exception, antecedant.Exception.Message), TaskContinuationOptions.OnlyOnFaulted);
            this.OnExecutionStarted();
        }

        public override void Stop()
        {
            if (this.backgroundListenTaskCancellationTokenSource != null)
            {
                this.backgroundListenTaskCancellationTokenSource.Cancel();
            }
        }

        protected void Cleanup()
        {
            this.backgroundListenTaskCancellationTokenSource.Cancel();
            this.backgroundListenTaskCancellationTokenSource.Dispose();
            this.backgroundListenTaskCancellationTokenSource = null;
            this.backgroundListenTask.Wait();
            this.backgroundListenTask.Dispose();
            this.backgroundListenTask = null;
        }
    }


    public class ListenTTLSignalConfig : TriggerEventConfig
    {

        protected string deviceName;

        [ChoicesMethod("DiscoverDevices")]
        public string DeviceName
        {
            get => this.deviceName;
            set => this.SetField(ref this.deviceName, value);
        }


        public static string[] DiscoverDevices()
        {
            return DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DILine, PhysicalChannelAccess.External);
        }
    }
}
