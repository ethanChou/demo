using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace WPF.Extend
{
    /// <summary>
    /// 逻辑层
    /// </summary>
    internal class MsgBoxCoreViewModel : ViewModelBase
    {
        internal class MessageButton
        {
            public MessageButton()
            {

            }

            private MessageBoxResult _buttonType;

            private bool _isDefault = false;

            private ICommand _actionCmd;

            private string _buttonText;

            public ICommand ButtonCmd
            {
                get { return _actionCmd; }
                set { _actionCmd = value; }
            }

            public string ButtonText
            {
                get { return _buttonText; }
                set { _buttonText = value; }
            }

            public MessageBoxResult ButtonType
            {
                get { return _buttonType; }
                set { _buttonType = value; }
            }

            public bool IsDefault
            {
                get { return _isDefault; }
                set { _isDefault = value; }
            }
        }

        public MsgBoxCoreViewModel(IMsgWindow window)
        {
            _window = window;
            _buttonList = new List<MessageButton>();
        }

        public void Init(bool topMost, bool isShowCheckbox, string messageBoxText, string caption,
            MessageBoxButton button,
            MessageBoxImage icon,
            MessageBoxResult defaultResult)
        {
            Topmost = topMost;
            MessageBoxText = messageBoxText;
            MessageBoxTitle = caption;

            CheckBoxVis = isShowCheckbox ? Visibility.Visible : Visibility.Collapsed;

            InitButton(button, defaultResult);

            InitImage(icon);
        }

        private void InitButton(MessageBoxButton buttonType, MessageBoxResult defaultButton)
        {
            ButtonList.Clear();

            switch (buttonType)
            {
                case MessageBoxButton.OKCancel:
                    ButtonList.Add(new MessageButton()
                    {
                        ButtonType = MessageBoxResult.OK,
                        ButtonText = Strings.Ok,
                        ButtonCmd = new DelegateCommand(ButtonCommand),
                        IsDefault = defaultButton == MessageBoxResult.OK,
                    });
                    ButtonList.Add(new MessageButton()
                    {
                        ButtonType = MessageBoxResult.Cancel,
                        ButtonText = Strings.Cancel,
                        ButtonCmd = new DelegateCommand(ButtonCommand),
                        IsDefault = defaultButton == MessageBoxResult.Cancel,
                    });
                    break;
                case MessageBoxButton.YesNoCancel:
                    ButtonList.Add(new MessageButton()
                   {
                       ButtonType = MessageBoxResult.Yes,
                       ButtonText = Strings.Yes,
                       ButtonCmd = new DelegateCommand(ButtonCommand),
                       IsDefault = defaultButton == MessageBoxResult.Yes,

                   });
                    ButtonList.Add(new MessageButton()
                    {
                        ButtonType = MessageBoxResult.No,
                        ButtonText = Strings.No,
                        ButtonCmd = new DelegateCommand(ButtonCommand),
                        IsDefault = defaultButton == MessageBoxResult.No,

                    });
                    ButtonList.Add(new MessageButton()
                    {
                        ButtonType = MessageBoxResult.Cancel,
                        ButtonText = Strings.Cancel,
                        ButtonCmd = new DelegateCommand(ButtonCommand),
                        IsDefault = defaultButton == MessageBoxResult.Cancel,

                    });
                    break;
                case MessageBoxButton.YesNo:
                    ButtonList.Add(new MessageButton()
                   {
                       ButtonType = MessageBoxResult.Yes,
                       ButtonText = Strings.Yes,
                       ButtonCmd = new DelegateCommand(ButtonCommand),
                       IsDefault = defaultButton == MessageBoxResult.Yes,

                   });
                    ButtonList.Add(new MessageButton()
                    {
                        ButtonType = MessageBoxResult.No,
                        ButtonText = Strings.No,
                        ButtonCmd = new DelegateCommand(ButtonCommand),
                        IsDefault = defaultButton == MessageBoxResult.No,

                    });
                    break;
                case MessageBoxButton.OK:
                    ButtonList.Add(new MessageButton()
                    {
                        ButtonType = MessageBoxResult.OK,
                        ButtonText = Strings.Ok,
                        ButtonCmd = new DelegateCommand(ButtonCommand),
                        IsDefault = defaultButton == MessageBoxResult.OK,
                    });
                    break;
                default:
                    return;
            }
        }

        private void InitImage(MessageBoxImage imageType)
        {
            string tmp = "";
            string root = AppDomain.CurrentDomain.BaseDirectory;
            switch (imageType)
            {
                case MessageBoxImage.None:
                    tmp = string.Format("{0}\\{1}",root ,  Strings.ImageSource[0]);
                    break;
                case MessageBoxImage.Error:
                    tmp = string.Format("{0}\\{1}",root ,  Strings.ImageSource[1]);
                    break;
                case MessageBoxImage.Question:
                    tmp = string.Format("{0}\\{1}", root, Strings.ImageSource[3]);
                    break;
                case MessageBoxImage.Warning:
                    tmp = string.Format("{0}\\{1}", root, Strings.ImageSource[4]);
                    break;
                case MessageBoxImage.Information:
                    tmp = string.Format("{0}\\{1}", root, Strings.ImageSource[5]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("imageType", imageType, null);
            }

            ImagePath = tmp;
        }

        private IMsgWindow _window;
        private List<MessageButton> _buttonList;
    
        private Visibility _showCheckBox = Visibility.Collapsed;
        private bool _isChecked = false;
        private string _messageBoxTitle;
        private string _imagePath;
        private string _messageBoxText;
        private ICommand _closeCmd;
        private bool _topmost = false;
        private string _checkBoxText = Strings.CheckTip;
        public List<MessageButton> ButtonList
        {
            get { return _buttonList; }
            set
            {
                _buttonList = value;
                NotifyChange("ButtonList");
            }
        }

        public Visibility CheckBoxVis
        {
            get { return _showCheckBox; }
            set
            {
                _showCheckBox = value;
                NotifyChange("CheckBoxVis");
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                NotifyChange("IsChecked");

            }
        }

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                NotifyChange("ImagePath");
            }
        }

        public string MessageBoxText
        {
            get { return _messageBoxText; }
            set
            {
                _messageBoxText = value;
                NotifyChange("MessageBoxText");
            }
        }

        public string MessageBoxTitle
        {
            get { return _messageBoxTitle; }
            set
            {
                _messageBoxTitle = value;
                NotifyChange("MessageBoxTitle");
            }
        }

        public ICommand CloseCmd
        {
            get { return _closeCmd ?? (_closeCmd = new DelegateCommand(CloseCommand)); }
        }

        public bool Topmost
        {
            get { return _topmost; }
            set { _topmost = value; }
        }

        public string CheckBoxText
        {
            get { return _checkBoxText; }
            set { _checkBoxText = value; }
        }

  
        private void CloseCommand(object arg)
        {
            _window.Close();
        }

        private void ButtonCommand(object arg)
        {
            _window.Result = (MessageBoxResult)arg;
            _window.Close();
        }
    }

    /// <summary>
    /// 可以传递委托的命令
    /// </summary>
    internal class DelegateCommand : ICommand
    {
        #region Member

        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        #endregion

        #region Constructor

        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion
    }
}
