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
        protected MetadataItem currentItem;

        public MetadataViewModel()
        {
            this.AvailableTypes = new ObservableCollection<Type>()
            {
                typeof(bool), typeof(int), typeof(double), typeof(string)
            };
            this.Items = new MetadataCollection(null);
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
                this.NotifyPropertyChanged("CurrentIndex");
            }
        }
        public int CurrentIndex
        {
            get => (int)this.currentState;
        }

        public ObservableCollection<Type> AvailableTypes { get; protected set; }

        public MetadataCollection Items { get; protected set; }
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
}
