using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace VisitorManager.ViewModel
{
    public class FluidMoveBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(Duration), typeof(FluidMoveBehavior), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(0.50))));
        public static readonly DependencyProperty AppliesToProperty = DependencyProperty.Register("AppliesTo", typeof(FluidMoveScope), typeof(FluidMoveBehavior), new PropertyMetadata(FluidMoveScope.Self));
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(FluidMoveBehavior), new PropertyMetadata(true));
        private Dictionary<FrameworkElement, Rect> rectDictionary;
        private Dictionary<FrameworkElement, Storyboard> transitionStoryboardDictionary;
        public Duration Duration
        {
            get
            {
                return (Duration)base.GetValue(FluidMoveBehavior.DurationProperty);
            }
            set
            {
                base.SetValue(FluidMoveBehavior.DurationProperty, value);
            }
        }
        public FluidMoveScope AppliesTo
        {
            get
            {
                return (FluidMoveScope)base.GetValue(FluidMoveBehavior.AppliesToProperty);
            }
            set
            {
                base.SetValue(FluidMoveBehavior.AppliesToProperty, value);
            }
        }
        public bool IsActive
        {
            get
            {
                return (bool)base.GetValue(FluidMoveBehavior.IsActiveProperty);
            }
            set
            {
                base.SetValue(FluidMoveBehavior.IsActiveProperty, value);
            }
        }
        protected override void OnAttached()
        {
            base.OnAttached();
            this.rectDictionary = new Dictionary<FrameworkElement, Rect>();
            this.transitionStoryboardDictionary = new Dictionary<FrameworkElement, Storyboard>();
            base.AssociatedObject.LayoutUpdated += new EventHandler(this.AssociatedObject_LayoutUpdated);
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.rectDictionary = null;
            this.transitionStoryboardDictionary = null;
            base.AssociatedObject.LayoutUpdated -= new EventHandler(this.AssociatedObject_LayoutUpdated);
        }
        private void AssociatedObject_LayoutUpdated(object sender, EventArgs e)
        {
            if (!this.IsActive)
            {
                return;
            }
            if (this.AppliesTo == FluidMoveScope.Self)
            {
                this.UpdateLayoutTransition(base.AssociatedObject);
                return;
            }
            Panel panel = base.AssociatedObject as Panel;
            if (panel != null)
            {
                foreach (FrameworkElement child in panel.Children)
                {
                    this.UpdateLayoutTransition(child);
                }
            }
        }
        private void UpdateLayoutTransition(FrameworkElement child)
        {
            Rect layoutRect = FluidMoveBehavior.GetLayoutRect(child);
            Rect empty = Rect.Empty;
            if (!this.rectDictionary.TryGetValue(child, out empty))
            {
                this.rectDictionary.Add(child, empty);
            }
            if (!FluidMoveBehavior.IsEmptyRect(empty) && !FluidMoveBehavior.IsEmptyRect(layoutRect) && (!FluidMoveBehavior.IsClose(empty.Left, layoutRect.Left) || !FluidMoveBehavior.IsClose(empty.Top, layoutRect.Top)))
            {
                TranslateTransform translateTransform = new TranslateTransform();
                UIElement visual = (UIElement)VisualTreeHelper.GetParent(child);
                Rect rect;
                try
                {
                    Transform transform = (Transform)child.TransformToVisual(visual);
                    rect = new Rect(transform.Transform(default(Point)), child.RenderSize);
                }
                catch (ArgumentException)
                {
                    rect = new Rect(default(Point), child.RenderSize);
                }
                Storyboard storyboard = null;
                if (this.transitionStoryboardDictionary.TryGetValue(child, out storyboard))
                {
                    storyboard.Stop();
                    storyboard = null;
                    this.transitionStoryboardDictionary.Remove(child);
                    FluidMoveBehavior.RemoveTransform(child);
                }
                FluidMoveBehavior.AddTransform(child, translateTransform);
                double num = rect.Left - layoutRect.Left;
                double num2 = empty.Left + num - layoutRect.Left;
                double num3 = rect.Top - layoutRect.Top;
                double num4 = empty.Top + num3 - layoutRect.Top;

                if (empty.Left == 0 && empty.Top == 0)
                {
                    num2 = 0;
                    num4 = 0;
                }

                translateTransform.X = num2;
                translateTransform.Y = num4;
                Duration duration = this.Duration;
                storyboard = new Storyboard();
                storyboard.Duration = duration;
                string text = "(FrameworkElement.RenderTransform).";
                TransformGroup transformGroup = child.RenderTransform as TransformGroup;
                if (transformGroup != null)
                {
                    object obj = text;
                    text = string.Concat(new object[]
					{
						obj, 
						"(TransformGroup.Children)[", 
						transformGroup.Children.Count - 1, 
						"]."
					});
                }
                DoubleAnimation doubleAnimation = new DoubleAnimation
                {
                    Duration = duration,
                    From = new double?(num2),
                    To = new double?(0.0)
                };
                Storyboard.SetTarget(doubleAnimation, child);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(text + "(TranslateTransform.X)", new object[0]));
                storyboard.Children.Add(doubleAnimation);
                DoubleAnimation doubleAnimation2 = new DoubleAnimation
                {
                    Duration = duration,
                    From = new double?(num4),
                    To = new double?(0.0)
                };

                Storyboard.SetTarget(doubleAnimation2, child);
                Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath(text + "(TranslateTransform.Y)", new object[0]));
                storyboard.Children.Add(doubleAnimation2);
                this.transitionStoryboardDictionary.Add(child, storyboard);
                storyboard.Completed += delegate(object sender, EventArgs e)
                {
                    if (this.transitionStoryboardDictionary != null && this.transitionStoryboardDictionary.ContainsKey(child))
                    {
                        this.transitionStoryboardDictionary.Remove(child);
                        FluidMoveBehavior.RemoveTransform(child);
                    }
                }
                ;
                storyboard.Begin();
            }
            this.rectDictionary[child] = layoutRect;
        }
        private static void AddTransform(FrameworkElement child, Transform transform)
        {
            if (child.RenderTransform == null)
            {
                child.RenderTransform = transform;
                return;
            }
            TransformGroup transformGroup = child.RenderTransform as TransformGroup;
            if (transformGroup == null)
            {
                transformGroup = new TransformGroup();
                transformGroup.Children.Add(child.RenderTransform);
                child.RenderTransform = transformGroup;
            }
            transformGroup.Children.Add(transform);
        }
        private static void RemoveTransform(FrameworkElement child)
        {
            if (child.RenderTransform is TranslateTransform)
            {
                child.ClearValue(UIElement.RenderTransformProperty);
                return;
            }
            TransformGroup transformGroup = child.RenderTransform as TransformGroup;
            if (transformGroup.Children.Count == 2 && transformGroup.Children[1] is TranslateTransform)
            {
                child.RenderTransform = transformGroup.Children[0];
                return;
            }
            transformGroup.Children.RemoveAt(transformGroup.Children.Count - 1);
        }
        private static Rect GetLayoutRect(FrameworkElement element)
        {
            Rect layoutSlot = LayoutInformation.GetLayoutSlot(element);
            Thickness margin = element.Margin;


            double x = 0.0;
            double y = 0.0;
            switch (element.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    {
                        x = layoutSlot.Left + margin.Left;
                        break;
                    }
                case HorizontalAlignment.Center:
                    {
                        x = (layoutSlot.Left + margin.Left + layoutSlot.Right - margin.Right) / 2.0 - element.ActualWidth / 2.0;
                        break;
                    }
                case HorizontalAlignment.Right:
                    {
                        x = layoutSlot.Right - margin.Right - element.ActualWidth;
                        break;
                    }
                case HorizontalAlignment.Stretch:
                    {
                        x = Math.Max(layoutSlot.Left + margin.Left, (layoutSlot.Left + margin.Left + layoutSlot.Right - margin.Right) / 2.0 - element.ActualWidth / 2.0);
                        break;
                    }
            }
            switch (element.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    {
                        y = layoutSlot.Top + margin.Top;
                        break;
                    }
                case VerticalAlignment.Center:
                    {
                        y = (layoutSlot.Top + margin.Top + layoutSlot.Bottom - margin.Bottom) / 2.0 - element.ActualHeight / 2.0;
                        break;
                    }
                case VerticalAlignment.Bottom:
                    {
                        y = layoutSlot.Bottom - margin.Bottom - element.ActualHeight;
                        break;
                    }
                case VerticalAlignment.Stretch:
                    {
                        y = Math.Max(layoutSlot.Top + margin.Top, (layoutSlot.Top + margin.Top + layoutSlot.Bottom - margin.Bottom) / 2.0 - element.ActualHeight / 2.0);
                        break;
                    }
            }
            return new Rect(x, y, element.ActualWidth, element.ActualHeight);
        }
        private static bool IsClose(double a, double b)
        {
            return Math.Abs(a - b) < 1E-07;
        }
        private static bool IsEmptyRect(Rect rect)
        {
            return rect.IsEmpty || double.IsNaN(rect.Left) || double.IsNaN(rect.Top);
        }
    }

    public enum FluidMoveScope
    {
        Self,
        Children
    }
}
