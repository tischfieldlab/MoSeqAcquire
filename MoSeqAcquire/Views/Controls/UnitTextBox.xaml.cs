﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace MoSeqAcquire.Views.Controls
{
    /// <summary>
    /// Interaction logic for FileChooser.xaml
    /// </summary>
    public partial class UnitTextBox : UserControl, INotifyPropertyChanged
    {

        public enum UnitAlignment
        {
            Left, Right
        }


        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(String), typeof(UnitTextBox), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty UnitsProperty = DependencyProperty.Register("Units", typeof(String), typeof(UnitTextBox), new PropertyMetadata(String.Empty));
        public static readonly DependencyProperty UnitsAlignmentProperty = DependencyProperty.Register("UnitsAlignment", typeof(UnitAlignment), typeof(UnitTextBox), new PropertyMetadata(UnitAlignment.Right, new PropertyChangedCallback(UnitAlignmentPropertyChanged)));


        public UnitTextBox()
        {
            InitializeComponent();
            this.UnitsText.SizeChanged += (s, e) => { this.NotifyDimentionalChange(); };
            this.ValueText.SizeChanged += (s, e) => { this.NotifyDimentionalChange(); };
        }
        public String Units
        {
            get { return (String)GetValue(UnitsProperty); }
            set { SetValue(UnitsProperty, value); }
        }
        public String Value
        {
            get { return (String)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public UnitAlignment UnitsAlignment
        {
            get { return (UnitAlignment)GetValue(UnitsAlignmentProperty); }
            set { SetValue(UnitsAlignmentProperty, value); }
        }


        public Thickness UnitsMargin
        {
            get
            {
                Thickness thickness;
                if (this.UnitsText != null)
                {
                    switch (this.UnitsAlignment)
                    {
                        case UnitAlignment.Left:
                            thickness = new Thickness(0, 0, 0, 0); break;

                        case UnitAlignment.Right:
                        default:
                            thickness = new Thickness(-1 * this.UnitsText.ActualWidth, 0, 0, 0); break;
                    }
                }
                else
                {
                    thickness = new Thickness();
                }
                Console.WriteLine("Units Margin: " + thickness.ToString());
                return thickness;
            }
        }
        public Thickness ValuePadding
        {
            get
            {
                Thickness thickness;
                if (this.UnitsText != null)
                {
                    switch (this.UnitsAlignment)
                    {
                        case UnitAlignment.Left:
                            thickness = new Thickness(this.UnitsText.ActualWidth, 0, 0, 0); break;

                        case UnitAlignment.Right:
                        default:
                            thickness = new Thickness(0, 0, this.UnitsText.ActualWidth, 0); break;
                    }
                }
                else
                {
                    thickness = new Thickness();
                }
                Console.WriteLine("Value Padding: " + thickness.ToString());
                return thickness;
            }
        }

        protected void NotifyDimentionalChange()
        {
            this.OnPropertyChanged("UnitsMargin");
            this.OnPropertyChanged("ValuePadding");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        private static void UnitAlignmentPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var self = (UnitTextBox)sender;
            self.NotifyDimentionalChange();
        }

    }
}