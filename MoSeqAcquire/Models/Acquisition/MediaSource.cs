﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Core;
using MoSeqAcquire.Models.Performance;

namespace MoSeqAcquire.Models.Acquisition
{
    public abstract class MediaSource : Component, IAggregatePerformanceProvider
    {
        public List<Channel> Channels;


        protected MediaSource()
        {
            this.Specification = new MediaSourceSpecification(this.GetType());
            this.Channels = new List<Channel>();
            this.Settings = (MediaSourceConfig)this.Specification.SettingsFactory();
        }
        public string Name { get; set; }
        public string DeviceId { get; set; }
        public string Status { get; protected set; }

        public bool IsInitialized { get; protected set; }
        public abstract List<Tuple<string, string>> ListAvailableDevices();
        public abstract bool Initialize(string deviceId);
        public virtual void Start()
        {
            MediaBus.Instance.Publish(this);
        }
        public virtual void Stop()
        {
            MediaBus.Instance.UnPublish(this);
        }

        protected void RegisterChannel(Channel Channel)
        {
            if (!this.Channels.Contains(Channel))
            {
                this.Channels.Add(Channel);
            }
        }
        public T FindChannel<T>() where T : Channel
        {
            foreach(var c in this.Channels)
            {
                if(typeof(T).Equals(c.GetType()))
                {
                    return (T)c;
                }
            }
            return default(T);
        }

        public List<IPerformanceProvider> GetPerformances()
        {
            return this.Channels.Select(c => c.Performance).ToList<IPerformanceProvider>();
        }
    }
}
