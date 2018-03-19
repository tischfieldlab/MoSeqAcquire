using MoSeqAcquire.Models.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels
{
    public class RecorderViewModel : BaseViewModel
    {
        
        public RecorderViewModel()
        {
            this.AvailableRecorderTypes = new ReadOnlyObservableCollection<string>(new ObservableCollection<string>(this.FindRecorderTypes()));
        }
        protected string recorderType;
        public string RecorderType
        {
            get => this.recorderType;
            set => this.SetField(ref this.recorderType, value);
        }
        public ReadOnlyObservableCollection<String> AvailableRecorderTypes { get; protected set; }


        protected IEnumerable<string> FindRecorderTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsAssignableFrom(typeof(MediaWriter))).Select(t => t.Name);
        }
    }
}
