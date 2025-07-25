﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MoSeqAcquire.Models.Acquisition
{
    public class SourceAvailabilityEventArgs : EventArgs
    {
        public SourceAvailabilityEventArgs(MediaSource Source)
        {
            this.Source = Source;
        }
        public MediaSource Source { get; set; }
    }

    public class MediaBus
    {
        private readonly List<BusChannel> __channels;
        private readonly List<MediaSource> __sources;

        public event EventHandler<SourceAvailabilityEventArgs> SourcePublished;
        public event EventHandler<SourceAvailabilityEventArgs> SourceUnPublished;

        #region Singleton
        private static MediaBus __instance;
        private MediaBus() {
            this.__channels = new List<BusChannel>();
            this.__sources = new List<MediaSource>();
        }
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
        public void Clear()
        {
            this.__sources.Clear();
            this.__channels.Clear();
        }
        public IEnumerable<MediaSource> Sources { get => this.__sources; }
        public IEnumerable<BusChannel> Channels { get => this.__channels; }
        public void Publish(MediaSource Source) {
            if (!this.__sources.Contains(Source))
            {
                this.__sources.Add(Source);
            }
            foreach (var c in Source.Channels)
            {
                if (!this.__channels.Exists(bc => bc.Channel.Equals(c))){
                    this.__channels.Add(new BusChannel(Source, c));
                }
            }
            this.SourcePublished?.Invoke(this, new SourceAvailabilityEventArgs(Source));
        }
        public void UnPublish(MediaSource Source)
        {
            if (this.__sources.Contains(Source)){
                var to_remove = this.__channels.Where(bc => bc.Source.Equals(Source)).ToList();
                foreach (var tr in to_remove) {
                    tr.Complete();
                    this.__channels.Remove(tr);
                }
                this.__sources.Remove(Source);
            }
            this.SourceUnPublished?.Invoke(this, new SourceAvailabilityEventArgs(Source));
        }
        public void Subscribe<TChannelType>(ITargetBlock<ChannelFrame> Action)
        {
            this.Subscribe(c => c.Channel.GetType() == typeof(TChannelType), Action);
        }
        public void Subscribe(Predicate<BusChannel> Selector, ITargetBlock<ChannelFrame> Action) {
            foreach(var c in this.__channels)
            {
                if (c is BusChannel c_typed && Selector.Invoke(c_typed))
                {
                    (c_typed).Feed.LinkTo(Action);
                }
            }
        }   
    }
}
