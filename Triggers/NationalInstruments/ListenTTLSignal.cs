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
            var triggerBus = App.Current.Services.GetService<TriggerBus>();
            //var settings = this.Config as WriteToConsoleConfig;
            //Console.WriteLine($"Trigger {trigger.Name} {trigger.GetType().Name}");
            //DaqSystem.Local.LoadDevice(settings.DeviceName).
            try
            {
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
            catch (Exception e)
            {
                this.Cleanup();
                this.OnExecutionFaulted(e, e.Message);
            }
        }

        public override void Start()
        {
            this.backgroundListenTaskCancellationTokenSource = new CancellationTokenSource();
            CancellationToken ct = backgroundListenTaskCancellationTokenSource.Token;
            this.backgroundListenTask = System.Threading.Tasks.Task.Run(() => this.ListenForTTL(ct), ct);
            this.backgroundListenTask
                .ContinueWith((antecedant) => this.Cleanup())
                .ContinueWith((antecedant) => this.OnExecutionFinished(""), TaskContinuationOptions.OnlyOnRanToCompletion);
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
            //return DaqSystem.Local.Devices;
        }
    }
}
