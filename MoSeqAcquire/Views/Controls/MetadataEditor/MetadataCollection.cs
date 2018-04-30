using MoSeqAcquire.Models.Configuration;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace MoSeqAcquire.Views.Controls.MetadataEditor
{
    public class MetadataCollection : ObservableCollection<MetadataItem>
    {
        protected object sourceObject;

        public MetadataCollection(object data) : base()
        {
            this.sourceObject = data;
            this.Initialize();  
        }
        public object SourceObject
        {
            get => this.sourceObject;
        }
        public new void Add(MetadataItem Item)
        {
            base.Add(Item);
        }
        public new void Remove(MetadataItem Item)
        {
            base.Remove(Item);
        }

        protected void Initialize()
        {
        }
    }
}
