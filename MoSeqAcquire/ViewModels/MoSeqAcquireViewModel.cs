using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models;
using ReactiveUI;

namespace MoSeqAcquire.ViewModels
{
    class MoSeqAcquireViewModel : ReactiveObject
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
