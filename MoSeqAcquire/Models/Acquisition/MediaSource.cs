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
        
        public MediaSource()
        {
            this.Channels = new List<IChannel>();
        }
        public string Name { get; set; }
        public ConfigurationSection Config { get; protected set; }
        public List<IChannel> Channels;

        public abstract void Initalize();
        public abstract void Start();
        public abstract void Stop();

        protected void RegisterChannel(IChannel Channel)
        {
            
            this.Channels.Add(Channel);
        }
    }
}
