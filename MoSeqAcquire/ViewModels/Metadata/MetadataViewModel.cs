using MoSeqAcquire.ViewModels;
using MoSeqAcquire.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MoSeqAcquire.Views.Controls;

namespace MoSeqAcquire.ViewModels.Metadata
{
    public enum MetadataViewState
    {
        Grid = 0,
        Property = 1
    }


    public class MetadataViewModel : BaseViewModel
    {
        protected bool isTemplateEditable;
        protected MetadataViewState currentState;
        protected MetadataCollection currentCollection;
        protected MetadataItem currentItem;

        protected MetadataEditorWindow metadataEditorWindow;

        public MetadataViewModel()
        {
            this.AvailableTypes = new ObservableCollection<AvailableTypeViewModel>()
            {
                new AvailableTypeViewModel(typeof(bool)),
                new AvailableTypeViewModel(typeof(int)),
                new AvailableTypeViewModel(typeof(double)),
                new AvailableTypeViewModel(typeof(string))
            };
            this.currentCollection = new MetadataCollection();
            this.CurrentState = MetadataViewState.Grid;
            this.IsTemplateEditable = true;
        }
        public bool IsTemplateEditable
        {
            get => this.isTemplateEditable;
            set => this.SetField(ref this.isTemplateEditable, value);
        }
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

        public ObservableCollection<AvailableTypeViewModel> AvailableTypes { get; protected set; }

        public MetadataCollection Items
        {
            get => this.currentCollection;
            set => this.SetField(ref this.currentCollection, value);
        }
        public MetadataItem CurrentItem
        {
            get => this.currentItem;
            set => this.SetField(ref this.currentItem, value);
        }

    }
}
