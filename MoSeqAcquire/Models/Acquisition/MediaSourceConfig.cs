using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MoSeqAcquire.Models.Acquisition
{
    
    public abstract class ConfigSnapshot// : ISerializable
    {
        protected ConfigSnapshot() { }

        public static ConfigSnapshot GetDefault() { throw new NotImplementedException(); }
    }
    public interface IConfigSnapshotProvider
    {
        ConfigSnapshot GetSnapshot();
        void ApplySnapshot(ConfigSnapshot snapshot);
    }

    public abstract class MediaSourceConfig : INotifyPropertyChanged, IConfigSnapshotProvider
    {
        public abstract void ReadState();

        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
        protected bool SetField<T>(ref T field, T value, Action PreTask, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            Task.Run(PreTask);
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract ConfigSnapshot GetSnapshot();
        public abstract void ApplySnapshot(ConfigSnapshot snapshot);
    }
}
