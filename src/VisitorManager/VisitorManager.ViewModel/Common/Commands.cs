using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace VisitorManager.ViewModel
{
    public static class UserVisitingCommands
    {
        private static ICommand _deleteCmd = new DelegateCommand(DeleteCommand);
        public static Action<object> Delete;

        public static ICommand DeleteCmd
        {
            get { return _deleteCmd; }
            set { _deleteCmd = value; }
        }

        private static void DeleteCommand(object arg)
        {
            if (Delete != null) Delete(arg);
        }
    }

    public static class UserSearchCommands
    {
        private static ICommand _viewCmd;

        /// <summary>
        /// Gets the navigate link routed command.
        /// </summary>
        public static ICommand ViewCmd
        {
            get { return _viewCmd; }
            set { _viewCmd = value; ; }
        }
    }


    public static class UserLeaveCommands
    {
        /// <summary>
        /// Gets the navigate link routed command.
        /// </summary>
        public static ICommand LeaveCmd
        {
            get { return _leaveCmd; }
            set { _leaveCmd = value; ; }
        }
        private static ICommand _leaveCmd = new DelegateCommand(LeaveCommand);

        public static Action<object> Leave;

        private static void LeaveCommand(object arg)
        {
            if (Leave != null) Leave(arg);
        }

        private static ICommand _checkedCmd;
        private static ICommand _uncheckedCmd;
        public static Action<object> Selected;
        private static ICommand _selectedCmd = new DelegateCommand(SelectedCommand);

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

        public static ICommand SelectedCmd
        {
            get { return _selectedCmd; }
            set { _selectedCmd = value; }
        }

        private static void SelectedCommand(object arg)
        {
            if (Selected != null) Selected(arg);
        }
    }

    /// <summary>
    /// 对应UserRegisterViewModel
    /// </summary>
    public static class UserRegisterCommands
    {
        private static ICommand _addPeerUserCmd = new DelegateCommand(AddPeerCommand);
        private static ICommand _deleteWaitUserCmd = new DelegateCommand(WaitUserDelCommand);
        private static ICommand _deleteTempUserCmd = new DelegateCommand(TempUserDelCommand);

        /// <summary>
        /// 同行人添加
        /// </summary>
        public static event Action<object> PeerUserAdded;
        /// <summary>
        /// 删除等待Visitor
        /// </summary>
        public static event Action<object> WaitUsersDeleted;
        /// <summary>
        /// 删除暂存Visitor
        /// </summary>
        public static event Action<object> TempUsersDeleted;

        /// <summary>
        /// 
        /// </summary>
        public static ICommand AddPeerUserCmd
        {
            get { return _addPeerUserCmd; }
            set { _addPeerUserCmd = value; }
        }

        private static void AddPeerCommand(object arg)
        {
            if (PeerUserAdded != null) PeerUserAdded(arg);
        }

        private static void WaitUserDelCommand(object arg)
        {
            if (WaitUsersDeleted != null) WaitUsersDeleted(arg);
        }

        private static void TempUserDelCommand(object arg)
        {
            if (TempUsersDeleted != null) TempUsersDeleted(arg);
        }

      
       
        public static ICommand DeleteWaitUserCmd
        {
            get { return _deleteWaitUserCmd; }
            set { _deleteWaitUserCmd = value; }
        }

        public static ICommand DeleteTempUserCmd
        {
            get { return _deleteTempUserCmd; }
            set { _deleteTempUserCmd = value; }
        }


    }
}
