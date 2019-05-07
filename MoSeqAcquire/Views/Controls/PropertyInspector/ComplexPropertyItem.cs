using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;

namespace MoSeqAcquire.Views.Controls.PropertyInspector
{
    class ComplexPropertyItem : PropertyItem
    {
        protected ComplexProperty complexProperty;

        public ComplexPropertyItem(object SourceObject, string PropertyName, ComplexProperty BackingProperty) : base(SourceObject, PropertyName)
        {
            this.complexProperty = BackingProperty;
        }

        public override Type ValueType
        {
            get => this.Value.GetType();
        }

        public override object Value
        {
            get => this.complexProperty.Value;
            set
            {
                this.complexProperty.Value = value;
                this.NotifyPropertyChanged();
            }
        }

        public override object DefaultValue
        {
            get => this.complexProperty.Default;
        }
        public override bool IsEnabled
        {
            get => this.complexProperty.IsSupported;
        }

        public override bool SupportsChoices
        {
            get => this.complexProperty.HasChoices;
        }

        public override IEnumerable<object> Choices
        {
            get => this.complexProperty.Choices;
        }

        public override string ChoicesDisplayPath
        {
            get => this.complexProperty.DisplayPath;
        }

        public override string ChoicesValuePath
        {
            get => this.complexProperty.ValuePath;
        }

        public override bool SupportsRange
        {
            get => this.complexProperty.HasRange;
        }

        public override object MinValue
        {
            get => this.complexProperty.Min;
        }

        public override object MaxValue
        {
            get => this.complexProperty.Max;
        }

        public override object StepValue
        {
            get => this.complexProperty.Step;
        }

        public override bool SupportsAutomatic
        {
            get => this.complexProperty.AllowsAuto;
        }

        public override bool IsAutomatic
        {
            get => this.complexProperty.IsAutomatic;
            set
            {
                this.complexProperty.IsAutomatic = value;
                this.NotifyPropertyChanged("IsAutomatic");
            }
        }
    }
}
