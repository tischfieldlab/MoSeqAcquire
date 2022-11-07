using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.DAQmx;
using System.Diagnostics;

namespace NationalInstruments
{
    [SettingsImplementation(typeof(SendTTLSignalConfig))]
    public class SendTTLSignal : TriggerAction
    {
        public SendTTLSignal()
        {
            this.Config = new SendTTLSignalConfig();
        }
        protected override Action<TriggerEvent> Action
        {
            get
            {
                return delegate (TriggerEvent trigger)
                {
                    var settings = this.Config as SendTTLSignalConfig;
                    //var settings = this.Config as WriteToConsoleConfig;
                    //Console.WriteLine($"Trigger {trigger.Name} {trigger.GetType().Name}");
                    //DaqSystem.Local.LoadDevice(settings.DeviceName).

                    using (DAQmx.Task digitalWriteTask = new DAQmx.Task())
                    {
                        digitalWriteTask.DOChannels.CreateChannel(settings.DeviceName, "", ChannelLineGrouping.OneChannelForEachLine);
                        DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                        writer.WriteSingleSampleSingleLine(true, true);
                        var timer = Stopwatch.StartNew();
                        while (timer.IsRunning)
                        {
                            if(timer.ElapsedMilliseconds >= 10)
                            {
                                timer.Stop();
                            }
                        }
                        writer.WriteSingleSampleSingleLine(true, false);
                    }
                };
            }
        }
    }


    public class SendTTLSignalConfig : TriggerActionConfig
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
            return DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOLine, PhysicalChannelAccess.External);
            //return DaqSystem.Local.Devices;
        }
    }
}
