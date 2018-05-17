using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


namespace ActiveXHost
{
     [ComVisible(true)]
    public interface IHostContainer
    {
        /// <summary>
        /// 127.0.0.1:8080
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Tag
        /// </summary>
        string UserData { get; set; }

         /// <summary>
         /// 加载插件
         /// </summary>
         /// <param name="pluginName"></param>
         /// <returns></returns>
         string LoadPlugin(string pluginName);

         /// <summary>
         /// 初始化控件
         /// </summary>
         /// <param name="className"></param>
         string Initialize(string className);


        /// <summary>
        /// 释放资源
        /// </summary>
        string DeInitialize();

        /// <summary>
        /// 当前页面的location
        /// </summary>
        /// <param name="url"></param>
        string CurrentURL(string url);

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <returns>json格式的字符串</returns>
        string Excute(string config);

         /// <summary>
         /// 获取指定属性
         /// </summary>
         /// <param name="propName"></param>
         /// <returns></returns>
         string Get(string propName);

         /// <summary>
         /// 设置指定属性
         /// </summary>
         /// <param name="propName"></param>
         /// <param name="val"></param>
         /// <returns></returns>
         string Set(string propName,object val);
    }
}
