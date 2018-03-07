using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition
{
    public class MediaBus
    {
        #region Singleton
        private static MediaBus __instance;
        private MediaBus() { }
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

        protected List<MediaSource> __sources;
        public void Publish(MediaSource Source) {
            this.__sources.Add(Source);
        }
        public void Subscribe(Predicate<MediaSource> Selector, Task Action) {
            this.__sources
                .SelectMany()
                .FindAll(Selector)
                .se((ms) => { ms.Channels });
        }
    }
}
