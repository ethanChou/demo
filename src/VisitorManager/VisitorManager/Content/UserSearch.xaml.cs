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

namespace VisitorManager.Content
{
    /// <summary>
    /// UserSearch.xaml 的交互逻辑
    /// </summary>
    public partial class UserSearch : UserControl
    {
        public UserSearch()
        {
            InitializeComponent();
            this.IsVisibleChanged += UserSearch_IsVisibleChanged;
        }

        void UserSearch_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Console.WriteLine(e.NewValue);
        }

      
    }
}
