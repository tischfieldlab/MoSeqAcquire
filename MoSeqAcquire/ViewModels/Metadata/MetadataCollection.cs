using MoSeqAcquire.Models.Configuration;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace MoSeqAcquire.ViewModels.Metadata
{
    public class MetadataCollection : ObservableCollection<MetadataItem>
    {

        public MetadataCollection() : base()
        {
            this.Initialize();  
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

        public override bool Equals(object obj)
        {
            return this.SequenceEqual(obj as MetadataCollection);
        }
    }
}
