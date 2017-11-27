using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace VisitorManager.ViewModel
{
    /// <summary>
    /// 生份证信息提取器
    /// </summary>
    public static class IDCardInfoExtractor
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public delegate void IDCardInfoCallBack(IDCardData data);
        private static Thread _thread;
        static int _iComm = -1;
        static string _lastIdCardNO = "";
        public static string SavePath { get; set; }
        public static void Start(int port, IDCardInfoCallBack callback = null)
        {
            if (_thread == null)
            {
                int index = 1;
                _thread = new Thread((x) =>
                {
                    while (true)
                    {
                        IDCardSDK.SNBC_InitComm(port);
                        _iComm = port;
                        if (IDCardSDK.SNBC_Authenticate() == 0 && IDCardSDK.SNBC_ReadContent() == 0)
                        {
                            StringBuilder strID = new StringBuilder(512);
                            int msg1 = 36;
                            IDCardSDK.SNBC_GetID(strID, msg1);

                            IDCardData data = new IDCardData();
                            //身份证
                            data.IdCardNO = _lastIdCardNO = strID.ToString();

                            //住址信息
                            StringBuilder strAddress = new StringBuilder(512);
                            int msg2 = 70;
                            IDCardSDK.SNBC_GetAddress(strAddress, msg2);

                            //姓名
                            StringBuilder strName = new StringBuilder(512);
                            int msg3 = 30;
                            IDCardSDK.SNBC_GetName(strName, msg3);

                            //性别
                            StringBuilder strSex = new StringBuilder(512);
                            int msg4 = 2;
                            IDCardSDK.SNBC_GetSex(strSex, msg4);
                            //出生日期
                            StringBuilder strBirth = new StringBuilder(512);
                            int msg5 = 16;
                            IDCardSDK.SNBC_GetBirth(strBirth, msg5);

                            // 民族
                            StringBuilder strNation = new StringBuilder(512);
                            int msg6 = 16;
                            IDCardSDK.SNBC_GetNation(strNation, msg6);

                            //签发机关
                            StringBuilder strDepartment = new StringBuilder(512);
                            int msg7 = 70;
                            IDCardSDK.SNBC_GetDepartment(strDepartment, msg7);

                            //其实时间
                            StringBuilder strISSUE = new StringBuilder(512);
                            int msg8 = 16;
                            IDCardSDK.SNBC_GetISSUE(strISSUE, msg8);

                            //结束时间
                            StringBuilder strEXPIRES = new StringBuilder(512);
                            int msg9 = 16;
                            IDCardSDK.SNBC_GetEXPIRES(strEXPIRES, msg9);

                            data.Address = strAddress.ToString();
                            data.Name = strName.ToString();
                            data.BirthData = strBirth.ToString();
                            data.EndTime = strEXPIRES.ToString();
                            data.StartTime = strISSUE.ToString();
                            data.Sex = strSex.ToString().Substring(0, 1);
                            data.IdCardNO = strID.ToString();
                            data.Nation = strNation.ToString();
                            data.BirthData = strBirth.ToString();
                            data.SignedOrganization = strDepartment.ToString();
                            string dirPath = string.Format("{0}/Image/{1}{2}{3}", AppDomain.CurrentDomain.BaseDirectory,
                                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                            if (!Directory.Exists(dirPath))
                            {
                                Directory.CreateDirectory(dirPath);
                            }

                            String bmpFileName = string.Format("{0}/{1}-{2}.jpg", dirPath, data.IdCardNO, data.Name);
                            try
                            {
                                int d13 = IDCardSDK.SNBC_SavePhoto(bmpFileName);
                                using (Image image = PicUtil.GetImage(bmpFileName))
                                {
                                    byte[] buf = PicUtil.ImageToBytes(image);
                                    data.BmpPath = ThriftManager.UploadImg2Bimg(buf);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Warn(ex.ToString() + "-----SavePhoto");
                            }
                            try
                            {
                                if (callback != null) callback.Invoke(data);
                            }
                            catch (Exception e)
                            {
                                logger.Warn(e.ToString() + "---Callback");
                            }
                        }
                        else
                        {
                            //IDCardData data = new IDCardData();
                            //data.IdCardNO = "19874561233";
                            //data.Name = "tests";
                            logger.Error("Authenticate Or ReadContent fail, Port is " + port);
                        }
                        Thread.Sleep(500);
                    }
                });
                _thread.IsBackground = true;
                _thread.Start();
            }
            else
            {
                _thread.Resume();
            }
        }


        public static void Stop(bool isStop)
        {
            if (_thread != null)
            {
                if (isStop)
                {
                    _thread.Abort();
                    IDCardSDK.SNBC_CloseComm(_iComm);
                }
                else
                {
                    _thread.Suspend();
                }
            }
        }

    }

    /// <summary>
    /// 身份证提取的人员信息
    /// </summary>
    public class IDCardData
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string IdCardNO { get; set; }
        /// <summary>
        /// 证件照
        /// </summary>
        public string BmpPath { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string BirthData { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; }
        /// <summary>
        /// 家庭住址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 签发机关
        /// </summary>
        public string SignedOrganization { get; set; }
        /// <summary>
        /// 发证时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        public string EndTime { get; set; }
    }

}
