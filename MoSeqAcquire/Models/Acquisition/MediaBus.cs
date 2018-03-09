using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MoSeqAcquire.Models.Acquisition
{
    public class MediaBus
    {
        #region Singleton
        private static MediaBus __instance;
        private MediaBus() { this.__sources = new List<IBusChannel>(); }
        public static MediaBus Instance
        {
            get
            {
                if(__instance == null)
                {
                    __instance = new MediaBus();
                }
                return __instance;
            }
        }
        #endregion

        public List<IBusChannel> __sources;
        public void Publish(MediaSource Source) {
            foreach (var c in Source.Channels)
            {
                this.__sources.Add(this.MakeBusChannel(Source, c));
            }
        }
        protected IBusChannel MakeBusChannel(MediaSource Source, IChannel Channel)
        {
            var d1 = typeof(BusChannel<>);
            Type[] typeArgs = new Type[] { Channel.BufferType };
            var makeme = d1.MakeGenericType(typeArgs);
            object o = Activator.CreateInstance(makeme, new object[]{ Source, Channel });
            return (IBusChannel)o;
        }
        public void Subscribe<T>(Predicate<BusChannel<T>> Selector, ITargetBlock<ChannelFrame<T>> Action) {
            foreach(var c in this.__sources)
            {
                if (Selector.Invoke(c as BusChannel<T>))
                {
                    (c as BusChannel<T>).Feed.LinkTo(Action);
                }
            }
        }
        
    }
    
    public interface IBusChannel { }
    public class BusChannel<T> : IBusChannel
    {
        public BusChannel(MediaSource Source, Channel<T> Channel)
        {
            this.Source = Source;
            this.Channel = Channel;
            var opts = new DataflowBlockOptions() { };
            this.Feed = new BroadcastBlock<ChannelFrame<T>>(item => item, opts);
            Channel.Buffer.LinkTo(this.Feed, new DataflowLinkOptions() { PropagateCompletion = true });
        }
        public MediaSource Source { get; }
        public Channel<T> Channel { get; }
        public BroadcastBlock<ChannelFrame<T>> Feed { get; }
    }
}
