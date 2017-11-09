using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPF.Extend
{
    internal static class Strings
    {
        /// <summary>
        /// none error ok question warning infor
        /// </summary>
        public static List<string> ImageSource = new List<string> {
        "",
        "\\Image\\Msg\\messagebox_error.png",
        "\\Image\\Msg\\messagebox_ok.png",
        "\\Image\\Msg\\messagebox_question.png",
        "\\Image\\Msg\\messagebox_warning.png",
        "\\Image\\Msg\\messagebox_infor.png",
        };

        /// <summary>
        /// 消息提示
        /// </summary>
        public static string Caption = "消息提示";
        /// <summary>
        /// "{0}秒后关闭"
        /// </summary>
        public static string CloseTipFormat = "{0}秒后关闭";
        /// <summary>
        /// 确定
        /// </summary>
        public static string Ok = "确定";
        /// <summary>
        /// 取消
        /// </summary>
        public static string Cancel = "取消";
        /// <summary>
        /// 中止
        /// </summary>
        public static string Abort = "中止";
        /// <summary>
        /// 忽略
        /// </summary>
        public static string Ignore = "忽略";
        /// <summary>
        /// 重试
        /// </summary>
        public static string Retry = "重试";
        /// <summary>
        /// 是
        /// </summary>
        public static string Yes = "是";
        /// <summary>
        /// 否
        /// </summary>
        public static string No = "否";
        /// <summary>
        /// 下次不再提示
        /// </summary>
        public static string CheckTip = "不再提示";

    }
}
