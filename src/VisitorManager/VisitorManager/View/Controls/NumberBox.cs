using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;


namespace VisitorManager
{
    public class NumberBox : TextBox
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            //屏蔽非法按键
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Decimal)
            {
                if (this.Text.Contains(".") || e.Key == Key.Decimal)
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            else if (((e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.OemPeriod) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
            {
                if (this.Text.Contains(".") || e.Key == Key.OemPeriod)
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
            //base.OnKeyDown(e);
        }

        protected virtual void OnTextChanged(TextChangedEventArgs e)
        {
            //屏蔽中文输入和非法字符粘贴输入

            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);

            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(this.Text, out num))
                {
                    this.Text = this.Text.Remove(offset, change[0].AddedLength);
                    this.Select(offset, 0);
                }
            }
            // base.OnTextChanged(e);
        }
    }
}
