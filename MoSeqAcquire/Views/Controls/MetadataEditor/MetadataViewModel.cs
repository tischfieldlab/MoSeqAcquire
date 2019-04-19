using MoSeqAcquire.ViewModels;
using MoSeqAcquire.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoSeqAcquire.Views.Controls.MetadataEditor
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

        public ICommand NewItemCommand => new ActionCommand(
            (p) => this.Items.Add(new MetadataItem("New Item", typeof(string)) { Value = "Some Value" }),
            (p) => this.IsTemplateEditable
        );
        public ICommand EditItemDefinition => new ActionCommand(
            (p) => this.CurrentState = MetadataViewState.Property,
            (p) => this.IsTemplateEditable
        );
        public ICommand RemoveItem => new ActionCommand(
            (p) => this.Items.Remove(p as MetadataItem),
            (p) => this.IsTemplateEditable
        );
        public ICommand HomeViewCommand => new ActionCommand((o) =>
        {
            this.CurrentState = MetadataViewState.Grid;
        });
        
    }

    public class AvailableTypeViewModel : BaseViewModel
    {
        protected Type type;
        protected bool isEnabled;

        public AvailableTypeViewModel(Type type, bool isEnabled = true)
        {
            this.Type = type;
            this.IsEnabled = isEnabled;
        }

        public Type Type
        {
            get => this.type;
            set => this.SetField(ref type, value);
        }
        public bool IsEnabled
        {
            get => this.isEnabled;
            set => this.SetField(ref isEnabled, value);
        }
    }
}
