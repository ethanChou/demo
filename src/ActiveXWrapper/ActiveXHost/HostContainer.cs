using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ActiveXHost
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    [ComSourceInterfaces(typeof(IHostContainerEvent))]
    public partial class HostContainer : UserControl, IHostContainer
    {
        private string _host;
        private string _id;
        private string _userData;
        private string _curURL;
        private Assembly _curAssembly;

        private Object _curObj;
        private Type _curType;


        public HostContainer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Host
        /// </summary>
        public string Host
        {
            get
            {
                return _host;
            }
            set
            {
                _host = value;
            }
        }

        /// <summary>
        /// Id
        /// </summary>
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        // Tag
        /// </summary>
        public string UserData
        {
            get
            {
                return _userData;
            }
            set
            {
                _userData = value;
            }
        }

        public string LoadPlugin(string pluginName)
        {
            var m = HostMessage.Empty();
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var basePath = Path.GetDirectoryName(assembly.Location);
                _curAssembly = Assembly.LoadFrom(string.Format("{0}\\{1}.dll", basePath, pluginName));
                lblInfo.Text = "请初始化控件";
            }
            catch (Exception ex)
            {
                m.code = -1;
                m.msg = ex.Message;
                Utils.Log(ex);
            }
            return m.ToJson();
        }

        public string Initialize(string className)
        {
            var m = HostMessage.Empty();
            try
            {
                if (_curAssembly == null)
                {
                    m.code = -1;
                    m.msg = "未初始化插件";
                    return m.ToJson();
                }

                Type[] clsTypes = _curAssembly.GetTypes();
                _curType = clsTypes.FirstOrDefault(v => v.Name == className);
                if (_curType == null)
                {
                    m.code = -1;
                    m.msg = "未找到" + className;
                    return m.ToJson();
                }

                if (IsSubClassOf(_curType, typeof(Control)))
                {
                    //如果是Control，则添加到界面里面
                    _curObj = Activator.CreateInstance(_curType);
                    var curCtrl = (Control)_curObj;
                    curCtrl.Dock = DockStyle.Fill;
                    this.Controls.Add(curCtrl);
                }
                else
                {
                    _curObj = Activator.CreateInstance(_curType);
                }

                //如果组件里有CustumEventFired事件，需要注册进来，回调给浏览器
                EventInfo evt = _curType.GetEvent("CustumEventFired", BindingFlags.Instance | BindingFlags.Public);
                if (evt != null)
                {
                    //注册插件里面的事件到HostContainer
                    MethodInfo method = this.GetType().GetMethod("OnCustumEventFired", BindingFlags.Public | BindingFlags.Instance);
                    var dele = EventHandlerFacotroy.CreateDelegate(evt.EventHandlerType, method, this);
                    evt.AddEventHandler(_curObj, dele);
                }

                //隐藏提示信息
                lblInfo.Text = "";
                lblInfo.Dock = DockStyle.None;
                lblInfo.Width = 0;
                lblInfo.Height = 0;
            }
            catch (Exception ex)
            {
                m.code = -1;
                m.msg = ex.ToString();
                Utils.Log(ex);
            }
            return m.ToJson();
        }

        public string DeInitialize()
        {
            var m = HostMessage.Empty();
            try
            {
                _curAssembly = null;
                MethodInfo method = _curType.GetMethod("Dispose", BindingFlags.Public | BindingFlags.Instance);
                if (method != null) method.Invoke(_curObj, null);
                GC.Collect();
            }
            catch (Exception ex)
            {
                m.code = -1;
                m.msg = ex.ToString();
                Utils.Log(ex);
                GC.Collect();
            }
            return m.ToJson();
        }

        public string Excute(string config)
        {
            var m = HostMessage.Empty();
            try
            {
                ConfigParameter p = config.FromJson<ConfigParameter>();
                if (_curType == null)
                {
                    m.code = -1;
                    m.msg = "未初始化对象";
                    return m.ToJson();
                }
                MethodInfo method = _curType.GetMethod(p.MethodName);
                if (method == null)
                {
                    m.code = -1;
                    m.msg = "未找到此方法";
                    return m.ToJson();
                }
                var paras = method.GetParameters();
                if (paras.Length == p.Args.Length)
                {
                    try
                    {
                        var res = method.Invoke(_curObj, p.Args);
                        if (res != null)
                            m.msg = res.ToString();
                    }
                    catch (Exception ex)
                    {
                        m.code = -1;
                        m.msg = ex.ToString();
                        Utils.Log(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                m.code = -1;
                m.msg = ex.ToString();
                Utils.Log(ex);
            }
            return m.ToJson();
        }

        public string Get(string propName)
        {
            var m = HostMessage.Empty();
            try
            {
                PropertyInfo prop = _curType.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null)
                {
                    m.code = -1;
                    m.msg = "未找到此属性";
                    return m.ToJson();
                }
                object obj = prop.GetValue(_curObj, null);
                m.data = obj;
                //m.msg = "TestRun(1,\'zxm\')";

            }
            catch (Exception ex)
            {
                m.code = -1;
                m.msg = ex.ToString();
                Utils.Log(ex);
            }
            return m.ToJson();
        }

        public string Set(string propName, object val)
        {
            Utils.Log(string.Format("propName:{0},val:{1}", propName, val));
            var m = HostMessage.Empty();
            try
            {
                PropertyInfo prop = _curType.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null)
                {
                    m.code = -1;
                    m.msg = "未找到此属性";
                    return m.ToJson();
                }
                prop.SetValue(_curObj, val, null);
                m.msg = "成功";
            }
            catch (Exception ex)
            {
                m.code = -1;
                m.msg = ex.ToString();
                Utils.Log(ex);
            }
            return m.ToJson();
        }

        public string CurrentURL(string url)
        {
            var m = HostMessage.Empty();
            _curURL = url;
            try
            {
                var res = Regex.Match(url, @"\d{2,3}([.]\d{1,3}){3}:\d{2,5}");
                Host = res.Value;
                m.msg = "成功";
            }
            catch (Exception ex)
            {
                m.code = -1;
                m.msg = ex.ToString();
                Utils.Log(ex);
            }
            return m.ToJson();
        }

        private bool IsSubClassOf(Type type, Type baseType)
        {
            var b = type.BaseType;
            while (b != null)
            {
                if (b.Equals(baseType))
                {
                    return true;
                }
                b = b.BaseType;
            }
            return false;
        }

        public event HostContainerEventHandler CustumEventFired;

        public void OnCustumEventFired(string id, string eventName, string data)
        {
            if (CustumEventFired != null) CustumEventFired(id, eventName, data);
        }
    }
}
