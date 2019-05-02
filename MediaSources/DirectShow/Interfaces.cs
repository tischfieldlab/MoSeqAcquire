using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.DirectSound;
using MoSeqAcquire.Models.Acquisition.DirectShow.Internal;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    public interface IProvider
    {
        string Name { get; }
    }
    public interface IVideoProvider : IProvider
    {
        ExVideoCaptureDevice VideoDevice { get; }
    }
    public interface IAudioProvider : IProvider
    {
        AudioCaptureDevice AudioDevice { get; }
    }
}
