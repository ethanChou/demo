using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisitorManager.ViewModel
{
    public static class JsonUtil
    {
        /// <summary>
        /// 提供对象序列化为Json格式数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(this object obj)
        {
            var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            return jsonSerializer.Serialize(obj);
        }

        /// <summary>
        /// 将 JSON 格式字符串转换为指定类型的对象。
        /// </summary>
        /// <param name="jsString"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string jsString)
        {
            var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return (T)jsonSerializer.Deserialize(jsString, typeof(T));
        }
    }

}
