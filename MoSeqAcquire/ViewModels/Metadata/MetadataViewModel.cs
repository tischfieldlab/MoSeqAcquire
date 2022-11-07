using MoSeqAcquire.ViewModels;
using MoSeqAcquire.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MoSeqAcquire.Models.Metadata;
using System.Collections;
using System.Collections.Specialized;
using MoSeqAcquire.Models.Metadata.DataTypes;

namespace MoSeqAcquire.ViewModels.Metadata
{
    public enum MetadataViewState
    {
        Grid = 0,
        Property = 1
    }


    public class MetadataViewModel : BaseViewModel//, INotifyDataErrorInfo
    {
        protected bool isTemplateEditable;
        protected MetadataViewState currentState;
        protected MetadataDefinitionCollection currentCollection;
        protected MetadataItemDefinition currentItem;

        public MetadataViewModel(MoSeqAcquireViewModel RootViewModel)
        {
            this.Root = RootViewModel;
            this.AvailableTypes = new ObservableCollection<AvailableTypeViewModel>(BaseDataType.AvailableTypes().Select(t => new AvailableTypeViewModel(t)));
            this.currentCollection = new MetadataDefinitionCollection();
            this.CurrentState = MetadataViewState.Grid;
        }
        public MoSeqAcquireViewModel Root { get; protected set; }
        public ObservableCollection<AvailableTypeViewModel> AvailableTypes { get; protected set; }

        public MetadataViewState CurrentState
        {
            get => this.currentState;
            set
            {
                this.SetField(ref this.currentState, value);
                this.NotifyPropertyChanged(nameof(this.CurrentIndex));
            }
        }
        public int CurrentIndex
        {
            get => (int)this.currentState;
        }

        

        public MetadataDefinitionCollection Items
        {
            get => this.currentCollection;
            protected set => this.SetField(ref this.currentCollection, value);
        }
        public MetadataItemDefinition CurrentItem
        {
            get => this.currentItem;
            set => this.SetField(ref this.currentItem, value);
        }




        /*public bool HasErrors => this.GetErrors(null).Cast<string>().Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return this.currentCollection.SelectMany(item => item.GetErrors(propertyName));
        }*/
    }
}
