using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace VisitorManager
{
    public static class GotoCommands
    {
        public static event Action<object> Navigated;
        private static ICommand navigateLink = new DelegateCommand(NavigateCommand);

        /// <summary>
        /// Gets the navigate link routed command.
        /// </summary>
        public static ICommand NavigateLink
        {
            get { return navigateLink; }
            set { navigateLink = value; ; }
        }

        private static void NavigateCommand(object arg)
        {
            if (Navigated != null)
            {
                Navigated(arg);
            }
        }
    }

    public static class VisitorDeleteCommands
    {
        public static event Action<object> Deleted;
        private static ICommand _deleteCmd = new DelegateCommand(DeletedCommand);

        /// <summary>
        /// Gets the navigate link routed command.
        /// </summary>
        public static ICommand DeleteCmd
        {
            get { return _deleteCmd; }
            set { _deleteCmd = value; ; }
        }

        private static void DeletedCommand(object arg)
        {
            if (Deleted != null)
            {
                Deleted(arg);
            }
        }
    }

    /// <summary>
    /// 删除命令
    /// </summary>
    public static class DeleteCommands
    {
        private static ICommand deleteCmd;

        /// <summary>
        /// Gets the navigate link routed command.
        /// </summary>
        public static ICommand DeleteCmd
        {
            get { return deleteCmd; }
            set { deleteCmd = value; ; }
        }


    }

    public static class ComboxCheckedCommands
    {
        private static ICommand _checkedCmd;
        private static ICommand _uncheckedCmd;

        /// <summary>
        /// Gets the navigate link routed command.
        /// </summary>
        public static ICommand CheckedCmd
        {
            get { return _checkedCmd; }
            set { _checkedCmd = value; }
        }

        public static ICommand UnCheckedCmd
        {
            get { return _uncheckedCmd; }
            set { _uncheckedCmd = value; }
        }
    }


}
