using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace VisitorManager
{
    public static class GotoCommands
    {
        private static ICommand navigateLink;

        /// <summary>
        /// Gets the navigate link routed command.
        /// </summary>
        public static ICommand NavigateLink
        {
            get { return navigateLink; }
            set { navigateLink = value; ; }
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
