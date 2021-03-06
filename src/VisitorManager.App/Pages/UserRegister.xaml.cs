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

namespace VisitorManager.App.Pages
{
    /// <summary>
    /// UserRegister.xaml 的交互逻辑
    /// </summary>
    public partial class UserRegister : UserControl
    {
        private UserRegisterViewModel _viewModel;
        public UserRegister()
        {
            InitializeComponent();
            this._viewModel = new UserRegisterViewModel(this.displayImage);
            this.DataContext = _viewModel;
        }

        private void UserRegister_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
