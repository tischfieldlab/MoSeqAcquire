using System;
using MoSeqAcquire.Models.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;

namespace MoSeqAcquire.Models.Metadata
{
    public class MetadataDefinitionCollection : ObservableCollection<MetadataItemDefinition>, IXmlSerializable, INotifyDataErrorInfo
    {
        

        public MetadataDefinitionCollection() : base()
        {
            this.CollectionChanged += MetadataDefinitionCollection_CollectionChanged;
        }

        private void MetadataDefinitionCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    (item as INotifyDataErrorInfo).ErrorsChanged += MetadataDefinitionCollection_ErrorsChanged; ;
                }
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    (item as INotifyDataErrorInfo).ErrorsChanged -= MetadataDefinitionCollection_ErrorsChanged;
                }
            }
        }

        

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private void MetadataDefinitionCollection_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(e.PropertyName));
        }
        protected void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(e.PropertyName));
        }
        public bool HasErrors => this.GetErrors(null).Cast<string>().Any();
        public IEnumerable GetErrors(string propertyName)
        {
            var errors = new List<string>();
            foreach (var item in this)
            {
                errors.AddRange(item.GetErrors(propertyName).Cast<string>());
            }

            return errors;
        }

        public override bool Equals(object obj)
        {
            return this.SequenceEqual(obj as MetadataDefinitionCollection);
        }

        public RecordingMetadataSnapshot GetSnapshot()
        {
            var snapshot = new RecordingMetadataSnapshot();
            this.Items.ForEach(item => snapshot.Add(item));
            return snapshot;
        }


        #region IXmlSerializable Members
        public XmlSchema GetSchema()
        {
            return null;
        }
        public void ReadXml(XmlReader reader)
        {
            bool isEmptyElement = reader.IsEmptyElement;
            string selfName = reader.Name;

            reader.ReadStartElement();

            if (!isEmptyElement)
            {
                while (true)
                {
                    if (reader.Name == "MetadataItem" && reader.NodeType == XmlNodeType.Element)
                    {
                        MetadataItemDefinition item = (MetadataItemDefinition)Activator.CreateInstance(typeof(MetadataItemDefinition), true);
                        item.ReadXml(reader);
                        this.Add(item);
                    }
                    else if (reader.Name.Equals(selfName) && reader.NodeType == XmlNodeType.EndElement)
                    {
                        reader.ReadEndElement();
                        return;
                    }
                }
            }
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (MetadataItemDefinition item in this)
            {
                writer.WriteStartElement("MetadataItem");
                item.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
