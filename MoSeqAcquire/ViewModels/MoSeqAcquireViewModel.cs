using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models;
using MoSeqAcquire.Models.Acquisition.Kinect;


namespace MoSeqAcquire.ViewModels
{
    class MoSeqAcquireViewModel
    {
        protected KinectManager manager;
        public MoSeqAcquireViewModel()
        {
            this.manager = new KinectManager();
        }

        public void Connect()
        {

        }
    }
}
