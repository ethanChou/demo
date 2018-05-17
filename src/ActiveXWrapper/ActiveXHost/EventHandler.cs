using System.Runtime.InteropServices;

namespace ActiveXHost
{
    /// <summary>
    /// Com组件对外回调的事件
    /// </summary>
    /// <param name="id"></param>
    /// <param name="eventName"></param>
    /// <param name="data"></param>
    [ComVisible(false)]
    public delegate void HostControlEventHandler(string id, string eventName, string data);
}
