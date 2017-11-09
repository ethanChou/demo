using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Interop;

namespace VisitorManager.ViewModel
{
    public class WindowMoveBehavior : Behavior<DependencyObject>
    {
        Window win;
        protected override void OnAttached()
        {
            base.OnAttached();
            FrameworkElement element = AssociatedObject as FrameworkElement;
            win = Window.GetWindow(element);
            if (element != null)
            {
                element.MouseDown += new MouseButtonEventHandler(element_MouseDown);
            }
        }

        /// <summary>
        /// Posted when the user presses the left mouse button while the cursor is within the nonclient area of a window  
        /// </summary>
        const int WM_NCLBUTTONDOWN = 0x00A1;

        void element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && win != null)
            {
                //最大化无法拖拽
                WindowInteropHelper wih = new WindowInteropHelper(win);
                SendMessage(wih.Handle, WM_NCLBUTTONDOWN, (int)2, 0);
            }
        }

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}
