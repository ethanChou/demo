using System.Runtime.InteropServices;

namespace ActiveXHost
{
    [ComVisible(true)]
    //[Guid("B28F2D87-E30C-431E-8233-5EDD1BEE1E4A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IHostContainerEvent
    {
        /// <summary>
        /// 回调
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        [DispId(0x001)]
        void CustumEventFired(string id, string eventName, string data);
    }

    /// <summary>
    /// Com组件对外回调的事件
    /// </summary>
    /// <param name="id"></param>
    /// <param name="eventName"></param>
    /// <param name="data"></param>
    [ComVisible(false)]
    public delegate void HostContainerEventHandler(string id, string eventName, string data);
}
