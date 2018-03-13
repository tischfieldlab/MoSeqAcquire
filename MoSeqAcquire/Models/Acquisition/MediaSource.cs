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
        public MediaSourceConfig Config { get; protected set; }
        
        public bool IsInitialized { get; protected set; }
        public abstract bool Initalize();
        public abstract void Start();
        public abstract void Stop();

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
