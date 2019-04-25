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

        public MetadataViewModel()
        {
            this.AvailableTypes = new ObservableCollection<AvailableTypeViewModel>()
            {
                new AvailableTypeViewModel(typeof(bool)),
                new AvailableTypeViewModel(typeof(int)),
                new AvailableTypeViewModel(typeof(double)),
                new AvailableTypeViewModel(typeof(string))
            };
            this.currentCollection = new MetadataDefinitionCollection();
            //this.currentCollection.CollectionChanged += CurrentCollection_CollectionChanged;
            this.CurrentState = MetadataViewState.Grid;
            //this.IsTemplateEditable = true;
        }

        /*private void CurrentCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            e.NewItems.OfType<INotifyPropertyChanged>().ForEach(item => item.PropertyChanged += Item_PropertyChanged);
            e.OldItems.OfType<INotifyPropertyChanged>().ForEach(item => item.PropertyChanged -= Item_PropertyChanged);
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(e.PropertyName));
        }*/

        public ObservableCollection<AvailableTypeViewModel> AvailableTypes { get; protected set; }
        /*public bool IsTemplateEditable
        {
            get => this.isTemplateEditable;
            set => this.SetField(ref this.isTemplateEditable, value);
        }*/
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
