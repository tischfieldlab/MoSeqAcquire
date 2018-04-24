using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Playback
{
    class MediaTimelineController
    {
        public double ClockRate { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan Position { get; set; }


        public void Pause() { }
        public void Start() { }
        public void Resume() { }
    }
}
