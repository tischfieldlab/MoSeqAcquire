using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition
{
    public abstract class MediaSource
    {
        public List<Channel> Channels;


        public MediaSource()
        {
            this.Channels = new List<Channel>();
        }
        public string Name { get; set; }
        public string DeviceId { get; set; }
        public string Status { get; protected set; }
        public MediaSourceConfig Config { get; protected set; }
        
        public bool IsInitialized { get; protected set; }
        public abstract List<Tuple<string, string>> ListAvailableDevices();
        public abstract bool Initalize(string DeviceId);
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
    }
}
