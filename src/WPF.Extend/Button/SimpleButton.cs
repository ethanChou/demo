using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF.Extend
{
    public class SimpleButton : System.Windows.Controls.Button
    {

        public ImageSource Normal
        {
            get { return (ImageSource)GetValue(NormalProperty); }
            set { SetValue(NormalProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty NormalProperty =
            DependencyProperty.Register("Normal", typeof(ImageSource), typeof(SimpleButton), new PropertyMetadata(null));



        public ImageSource Hover
        {
            get { return (ImageSource)GetValue(HoverProperty); }
            set { SetValue(HoverProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HoverProperty =
            DependencyProperty.Register("Hover", typeof(ImageSource), typeof(SimpleButton), new PropertyMetadata(null));



        public ImageSource Pressed
        {
            get { return (ImageSource)GetValue(PressedProperty); }
            set { SetValue(PressedProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PressedProperty =
            DependencyProperty.Register("Pressed", typeof(ImageSource), typeof(SimpleButton), new PropertyMetadata(null));



        public ImageSource Disabled
        {
            get { return (ImageSource)GetValue(DisabledProperty); }
            set { SetValue(DisabledProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DisabledProperty =
            DependencyProperty.Register("Disabled", typeof(ImageSource), typeof(SimpleButton), new PropertyMetadata(null));



    }
}
