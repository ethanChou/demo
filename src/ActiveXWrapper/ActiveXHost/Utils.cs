using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace ActiveXHost
{
    [System.Runtime.InteropServices.ComVisible(false)]
    public static class Utils
    {
        public static string ToJson(this object ojb)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();   //实例化一个能够序列化数据的类
            
            return json.Serialize(ojb);
        }

        public static T FromJson<T>(this string ojb)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();   //实例化一个能够序列化数据的类
            return json.Deserialize<T>(ojb);
        }

        public static void Log(object obj)
        {
            string v = string.Format("{0}\\fblogs\\Ax-{1}.log", Path.GetTempPath(), DateTime.Now.ToString("yyyyMMdd"));
            using (TextWriter tw = new StreamWriter(v, true))
            {
                tw.WriteLine("[{0}] ActiveXHost: {1}", DateTime.Now, obj.ToString());
            }
        }
    }

}
