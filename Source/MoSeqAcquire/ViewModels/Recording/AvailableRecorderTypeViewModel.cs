using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Recording;

namespace MoSeqAcquire.ViewModels.Recording
{
    public class AvailableRecorderTypeViewModel
    {
        protected Type recorderType;
        protected RecorderSpecification spec;

        public AvailableRecorderTypeViewModel(Type RecorderType)
        {
            this.recorderType = RecorderType;
            this.spec = new RecorderSpecification(RecorderType);
        }
        public Type RecorderType
        {
            get => this.recorderType;
        }
        public string Name
        {
            get => this.spec.DisplayName;
        }
    }
}
