using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Management;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MoSeqAcquire.Models.Recording
{
    public enum ChannelCapacity
    {
        Multiple = -1,
        None = 0,
        Single = 1,
    }
    public class MediaWriterPin
    {
        protected string name;
        protected Channel channel;
        protected BufferBlock<ChannelFrame> backBuffer;
        protected Func<ActionBlock<ChannelFrame>> sinkFactory;
        protected ActionBlock<ChannelFrame> sink;

        public MediaWriterPin(MediaType mediaType, ChannelCapacity Capacity, Func<ActionBlock<ChannelFrame>> WorkerFactory)
        {
            this.MediaType = mediaType;
            this.Capacity = Capacity;
            this.sinkFactory = WorkerFactory;
            this.name = this.MediaType.ToString() + " Pin";
        }
        public MediaWriterPin(string name, MediaType mediaType, ChannelCapacity Capacity, Func<ActionBlock<ChannelFrame>> WorkerFactory) : this(mediaType, Capacity, WorkerFactory)
        {
            this.name = name;
        }
        public string Name
        {
            get => this.name;
        }
        public MediaType MediaType { get; protected set; }
        public ChannelCapacity Capacity { get; protected set; }
        public Channel Channel
        {
            get => this.channel;
            set => this.channel = value;
        }
        public void Connect()
        {
            this.backBuffer = new BufferBlock<ChannelFrame>(new DataflowBlockOptions() { EnsureOrdered = true, });
            this.sink = this.sinkFactory.Invoke();
            MediaBus.Instance.Subscribe(bc => bc.Channel == Channel, this.backBuffer);
            this.backBuffer.LinkTo(this.sink, new DataflowLinkOptions() { PropagateCompletion = true });
        }
        public Task Disconnect()
        {
            this.backBuffer.Complete();
            return this.sink.Completion;
        }
    }


    public abstract class MediaWriter : IMediaWriter
    {
        private Dictionary<string, MediaWriterPin> _pins;

        public MediaWriter()
        {
            this._pins = new Dictionary<string, MediaWriterPin>();
            this.Specification = new RecorderSpecification(this.GetType());
            this.Settings = this.Specification.SettingsFactory();
            this.Stats = new MediaWriterStats();
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

        public bool IsRecording { get; protected set; }
        public MediaWriterStats Stats { get; protected set; }
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
            this._pins.Values.ForEach(mwp => mwp.Connect());
            this.IsRecording = true;
            this.Stats.Start();
        }

        public virtual void Stop()
        {
            this._pins.Values.ForEach(mwp => mwp.Disconnect().Wait());
            this.Stats.Stop();
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
