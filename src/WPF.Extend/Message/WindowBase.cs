using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF.Extend
{
    public class WindowBase : Window
    {
        static WindowBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowBase), new FrameworkPropertyMetadata(typeof(WindowBase)));
        }

        public WindowBase()
        {
            this.MouseLeftButtonDown += XeWindows_MouseLeftButtonDown;
        }

        private void XeWindows_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            WindowBehaviorHelperEX wh = new WindowBehaviorHelperEX(this);
            wh.RepairBehavior();
        }
        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置窗体圆角值")]
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(WindowBase), new PropertyMetadata(new CornerRadius(0)));


        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置窗体透明度")]
        public double BackOpacity
        {
            get { return (double)GetValue(BackOpacityProperty); }
            set { SetValue(BackOpacityProperty, value); }
        }

        public static readonly DependencyProperty BackOpacityProperty = DependencyProperty.Register("BackOpacity", typeof(double), typeof(WindowBase), new UIPropertyMetadata(1d));


        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置窗体阴影大小")]
        public double ShadowSize
        {
            get { return (double)GetValue(ShadowSizeProperty); }
            set { SetValue(ShadowSizeProperty, value); }
        }
        public static readonly DependencyProperty ShadowSizeProperty = DependencyProperty.Register("ShadowSize", typeof(double), typeof(WindowBase), new UIPropertyMetadata(0d));


        public static readonly DependencyProperty NineGridProperty = DependencyProperty.Register("NineGrid", typeof(Thickness), typeof(WindowBase), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.None));

        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置阴影九宫格图片如何拉伸")]
        public Thickness NineGrid
        {
            get { return (Thickness)GetValue(NineGridProperty); }
            set { SetValue(NineGridProperty, value); }
        }

        public static readonly DependencyProperty NineImageProperty = DependencyProperty.Register("NineImage", typeof(ImageSource), typeof(WindowBase));
        [CategoryAttribute("自定义属性"), DescriptionAttribute("获取或设置阴影九宫格图片")]
        public ImageSource NineImage
        {
            get { return (ImageSource)GetValue(NineImageProperty); }
            set { SetValue(NineImageProperty, value); }
        }
    }
}
