using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ActiveXHost
{
    /// <summary>
    ///方法的参数
    /// </summary>
     [ComVisible(false)]
    public class ConfigParameter
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 调用的方法名
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 参数列表,必须严格按照方法的参数次序添加
        /// </summary>
        public object[] Args { get; set; }

    }
}
