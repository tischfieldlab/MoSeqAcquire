using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Models.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MoSeqAcquire.Models.Recording
{
    

    public abstract class MediaWriter : IMediaWriter
    {
        private Dictionary<string, MediaWriterPin> _pins;
        private DateTime _epoch;

        public MediaWriter()
        {
            this._pins = new Dictionary<string, MediaWriterPin>();
            this.Specification = new RecorderSpecification(this.GetType());
            this.Settings = this.Specification.SettingsFactory();
            this.Performance = new MediaWriterStats(this.Name);
        }
        public event DestinationBaseResponse RequestDestinationBase;
        protected string RequestBaseDestination()
        {
            return this.RequestDestinationBase?.Invoke();
        }
        public string Name { get; set; }
        public RecorderSettings Settings { get; }
        public IReadOnlyDictionary<string, MediaWriterPin> Pins
        {
            get => this._pins;
        }
        protected void RegisterPin(MediaWriterPin Pin)
        {
            this._pins.Add(Pin.Name, Pin);
        }
        public DateTime Epoch { get => this._epoch; }
        public bool IsRecording { get; protected set; }
        public MediaWriterStats Performance { get; protected set; }
        public RecorderSpecification Specification { get; protected set; }

        public virtual string FilePath
        {
            get
            {
                var basepath = this.RequestBaseDestination();
                return Path.Combine(basepath == null ? "" : basepath, this.Name + "." + this.Ext);
            }
        }
        protected abstract string Ext { get; }


        
        public virtual void Start()
        {
            this._epoch = PreciseDatetime.Now;
            this._pins.Values.ForEach(mwp => mwp.Connect());
            this.IsRecording = true;
            this.Performance.Start();
        }

        public virtual void Stop()
        {
            this._pins.Values.ForEach(mwp => mwp.Disconnect().Wait());
            this.Performance.Stop();
            this.IsRecording = false;
        }

        public virtual IDictionary<string, IEnumerable<Channel>> GetChannelFileMap()
        {
            return new Dictionary<string, IEnumerable<Channel>>()
            {
                { this.FilePath, this.Pins.Values.Where(mwp => mwp.Channel != null).Select(mwp => mwp.Channel) }
            };
        }

        public ProtocolRecorder GetProtocolRecorder()
        {
            return new ProtocolRecorder()
            {
                Name = this.Name,
                Provider = this.Specification.TypeName,
                Config = this.Settings.GetSnapshot(),
                Pins = this.Pins.Values.Select(mwp => new ProtocolRecorderPin() { Name = mwp.Name, Channel = mwp.Channel != null ? mwp.Channel.FullName : null }).ToList()
            };
        }
        public RecordingDevice GetDeviceInfo()
        {
            return new RecordingDevice()
            {
                Name = this.Name,
                Provider = this.Specification.TypeName,
                Config = this.Settings.GetSnapshot(),
                Records = this.GetChannelFileMap().Select(kvp => new RecordingRecord() { Filename = kvp.Key, Channels = kvp.Value.Select(c => c.FullName).ToList() }).ToList()
            };
        }
        public static MediaWriter FromProtocolRecorder(ProtocolRecorder protocolRecorder)
        {
            var writer = (MediaWriter)Activator.CreateInstance(protocolRecorder.GetProviderType());
            writer.Name = protocolRecorder.Name;
            writer.Settings.ApplySnapshot(protocolRecorder.Config);
            foreach(var rp in protocolRecorder.Pins)
            {
                if (writer.Pins.ContainsKey(rp.Name))
                {
                    writer.Pins[rp.Name].Channel = MediaBus.Instance.Channels.Select(bc => bc.Channel).FirstOrDefault(c => c.FullName == rp.Channel);
                }
            }
            return writer;
        }
    }
}
