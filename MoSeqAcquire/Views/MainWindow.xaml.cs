﻿using MoSeqAcquire.ViewModels;
using System;
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
using Microsoft.Extensions.DependencyInjection;

namespace MoSeqAcquire.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MoSeqAcquireViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.WindowState = WindowState.Maximized;
            this.ShowInTaskbar = true;
            this.ShowActivated = true;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            var protocolService = App.Current.Services.GetService<ProtocolManagerViewModel>();
            protocolService.UnloadProtocol();
            Application.Current.Shutdown();
        }
    }
}
