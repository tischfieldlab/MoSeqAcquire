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
        protected string name;
        protected string recorderType;
        public RecorderViewModel()
        {
            this.AvailableRecorderTypes = new ReadOnlyObservableCollection<string>(new ObservableCollection<string>(this.FindRecorderTypes()));
        }
        
        public string Name
        {
            get => this.name;
            set => this.SetField(ref this.name, value);
        }
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
        public MediaWriter MakeMediaWriter()
        {
            var writer = (MediaWriter)Activator.CreateInstance(Type.GetType(this.recorderType));

            return writer;
        }
    }
}
