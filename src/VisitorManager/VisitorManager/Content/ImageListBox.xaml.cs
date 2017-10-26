using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// ImageListBox.xaml 的交互逻辑
    /// </summary>
    public partial class ImageListBox : UserControl
    {
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ImageListBox),
                new PropertyMetadata(new PropertyChangedCallback(ItemsSourceChanged)));

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageListBox box = d as ImageListBox;
            if (box != null)
            {
                box.Update(e.NewValue);
            }
        }

        public string DateTemplateName
        {
            get { return (string)GetValue(DateTemplateNameProperty); }
            set { SetValue(DateTemplateNameProperty, value); }
        }

        public static readonly DependencyProperty DateTemplateNameProperty =
            DependencyProperty.Register("DateTemplateName", typeof(string), typeof(ImageListBox),
                new PropertyMetadata(new PropertyChangedCallback(DateTemplateNameChanged)));


        private static void DateTemplateNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageListBox box = d as ImageListBox;
            if (box != null)
            {
                box.UpdateDateTemplate(e.NewValue);
            }
        }

        private void Update(object obj)
        {
            lsPricture.ItemsSource = (IEnumerable)obj;
        }

        private void UpdateDateTemplate(object obj)
        {
            DataTemplate dt = (DataTemplate)this.TryFindResource(obj.ToString());
            if (dt != null)
            {
                lsPricture.ItemTemplate = dt;
            }
        }

        public ImageListBox()
        {
            InitializeComponent();
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);

           
        }
    }
}
