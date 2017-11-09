using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WPF.Extend
{
    internal class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 封装PropertyChanged触发方法，触发所有属性
        /// </summary>
        public virtual void NotifyChange()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(null));
        }

        /// <summary>
        /// 封装PropertyChanged触发方法，触发指定属性
        /// </summary>
        /// <param name="propertyName">通知的属性名</param>
        public virtual void NotifyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 封装PropertyChanged触发方法，触发指定属性并传递指定对象
        /// </summary>
        /// <param name="sender">对象</param>
        /// <param name="propertyName">通知的属性名</param>
        public virtual void NotifyChange(object sender, string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
        }
    }
}
