using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace VisitorManager
{
    class IdCardSDK
    {
        /// <summary>
        ///  1、打开通讯端口
        /// </summary>
        /// <returns>如果成功返回0x00,失败返回1</returns>
        [DllImport("BY618_108.dll")]
        public static extern int SNBC_InitComm(int iComm);

        /// <summary>
        ///  2、卡认证
        /// </summary>
        /// <returns>如果成功返回0x00,失败返回1</returns>
        [DllImport("BY618_108.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SNBC_Authenticate();


        /// <summary>
        ///  3、读卡操作
        /// </summary>
        /// <returns>如果成功返回0x00,失败返回1</returns>
        [DllImport("BY618_108.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SNBC_ReadContent();


        /// <summary>
        ///  4、获取姓名信息字符串。
        /// </summary>
        /// <returns>如果成功返回0x00,失败返回1</returns>
        [DllImport("BY618_108.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SNBC_GetName(StringBuilder strName, int iLen);


        /// <summary>
        ///  5、获取性别信息字符串。
        /// </summary>
        /// <returns>如果成功返回0x00,失败返回1</returns>
        [DllImport("BY618_108.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SNBC_GetSex(StringBuilder strSex, int iLen);

        /// <summary>
        ///  6、获取出生信息字符串。
        /// </summary>
        /// <returns>如果成功返回0x00,失败返回1</returns>
        [DllImport("BY618_108.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SNBC_GetBirth(StringBuilder strBirth, int iLen);


        /// <summary>
        ///  7、获取民族信息字符串。
        /// </summary>
        /// <returns>如果成功返回0x00,失败返回1</returns>
        [DllImport("BY618_108.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SNBC_GetNation(StringBuilder strNation, int iLen);


        /// <summary>
        ///  8、获取身份证号码信息字符串。
        /// </summary>
        /// <returns>如果成功返回0x00,失败返回1</returns>
        [DllImport("BY618_108.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SNBC_GetID(StringBuilder strID, int iLen);

        /// <summary>
        ///  9、获取家庭住址信息字符串。
        /// </summary>
        /// <returns>如果成功返回0x00,失败返回1</returns>
        [DllImport("BY618_108.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SNBC_GetAddress(StringBuilder strAddress, int iLen);


        /// <summary>
        ///  10、获取签发机关信息字符串。
        /// </summary>
        /// <returns>如果成功返回0x00,失败返回1</returns>
        [DllImport("BY618_108.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SNBC_GetDepartment(StringBuilder strDepartment, int iLen);



        /// <summary>
        ///  11、获取有效期开始信息字符串。
        /// </summary>
        /// <returns>如果成功返回0x00,失败返回1</returns>
        [DllImport("BY618_108.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SNBC_GetISSUE(StringBuilder strISSUE, int iLen);



        /// <summary>
        ///  12、获取有效期结束信息字符串。
        /// </summary>
        /// <returns>如果成功返回0x00,失败返回1</returns>
        [DllImport("BY618_108.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SNBC_GetEXPIRES(StringBuilder strEXPIRES, int iLen);


        /// <summary>
        ///  13、获取最新住址信息字符串。
        /// </summary>
        /// <returns>如果成功返回0x00,失败返回1</returns>
        [DllImport("BY618_108.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SNBC_GetNewAddress(StringBuilder strNewAddress, int iLen);


        /// <summary>
        ///  14、获取照片信息字符串。
        /// </summary>
        /// <returns>如果成功返回0x00,失败返回1</returns>
        [DllImport("BY618_108.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SNBC_SavePhoto(String bmpFileName);



        /// <summary>
        ///  15、关闭通讯端口。
        /// </summary>
        /// <returns>如果成功返回0x00,失败返回1</returns>
        [DllImport("BY618_108.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SNBC_CloseComm(int iComm);
    }
}
