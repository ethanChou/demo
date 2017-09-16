using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace VisitorManager
{
    public class IdCardThread
    {
        public delegate void GetPersonCallBack(PersonData data, bool isblack);
        private static Thread _thread;
        static int _iComm = -1;
        static string _lastIdCardNO = "";
        public static void Start(int port, IDispatcher dis, GetPersonCallBack callback)
        {
            if (_thread == null)
            {
                int index = 1;
                _thread = new Thread((x) =>
                {
                    while (true)
                    {
                        IdCardSDK.SNBC_InitComm(port);
                        _iComm = port;
                        if (IdCardSDK.SNBC_Authenticate() == 0 && IdCardSDK.SNBC_ReadContent() == 0)
                        {
                            StringBuilder strID = new StringBuilder(512);
                            int msg1 = 36;
                            IdCardSDK.SNBC_GetID(strID, msg1);
                            if (strID.ToString().Equals(_lastIdCardNO))
                            {
                                //try
                                //{
                                //    Directory.Delete(dirPath, true);
                                //}
                                //catch { }
                                //continue;
                            }
                            PersonData data = new PersonData();
                            //身份证
                            data.IdCardNO = _lastIdCardNO = strID.ToString();

                            //住址信息
                            StringBuilder strAddress = new StringBuilder(512);
                            int msg2 = 70;
                            IdCardSDK.SNBC_GetAddress(strAddress, msg2);

                            //姓名
                            StringBuilder strName = new StringBuilder(512);
                            int msg3 = 30;
                            IdCardSDK.SNBC_GetName(strName, msg3);

                            //性别
                            StringBuilder strSex = new StringBuilder(512);
                            int msg4 = 2;
                            IdCardSDK.SNBC_GetSex(strSex, msg4);
                            //出生日期
                            StringBuilder strBirth = new StringBuilder(512);
                            int msg5 = 16;
                            IdCardSDK.SNBC_GetBirth(strBirth, msg5);

                            // 民族
                            StringBuilder strNation = new StringBuilder(512);
                            int msg6 = 16;
                            IdCardSDK.SNBC_GetNation(strNation, msg6);

                            //签发机关
                            StringBuilder strDepartment = new StringBuilder(512);
                            int msg7 = 70;
                            IdCardSDK.SNBC_GetDepartment(strDepartment, msg7);

                            //其实时间
                            StringBuilder strISSUE = new StringBuilder(512);
                            int msg8 = 16;
                            IdCardSDK.SNBC_GetISSUE(strISSUE, msg8);

                            //结束时间
                            StringBuilder strEXPIRES = new StringBuilder(512);
                            int msg9 = 16;
                            IdCardSDK.SNBC_GetEXPIRES(strEXPIRES, msg9);

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
                            //try
                            //{
                            //    int d13 = IdCardSDK.SNBC_SavePhoto(bmpFileName);
                            //    if (!string.IsNullOrEmpty(Properties.Settings.Default.SavePath))
                            //    {
                            //        string copydir = string.Format("{0}/{1}{2}{3}", Properties.Settings.Default.SavePath,
                            //            DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                            //        if (!Directory.Exists(copydir)) { Directory.CreateDirectory(copydir); }
                            //        File.Copy(bmpFileName, string.Format("{0}/{1}", copydir, Path.GetFileName(bmpFileName)));
                            //    }
                            //}
                            //catch (Exception)
                            //{
                            //}
                            //data.BmpPath = bmpFileName;
                            //try
                            //{
                            //    dis.Dispatcher.Invoke(new Action(() =>
                            //    {
                            //        callback.Invoke(data, AccessControlOprator.SelectModel<IdCardOprator.Model.BlackList>("CardNO='" + data.IdCardNO + "'").Count > 0 ? true : false);
                            //    }));
                            //}
                            //catch (Exception)
                            //{

                            //}
                        }
                        //Thread.Sleep(500);
                        //PersonData data = new PersonData();
                        //data.Address ="dasd";
                        //data.Name = "dasd";
                        //data.BirthData = "dasd";
                        //data.EndTime = "dasd";
                        //data.StartTime = "dasd";
                        //data.Sex = "dasd";
                        //data.Nation = "dasd";
                        //data.BirthData = "dasd";
                        //data.SignedOrganization = "dasd";

                        //if (index % 2 == 0)
                        //{
                        //    data.IdCardNO = "dasd";
                        //    data.BmpPath = @"C:\Users\ljy\Documents\Tencent Files\497258352\FileRecv\ConsoleApplication2\VIA.Resource\Asset\VIA.png";
                        //}
                        //else
                        //{
                        //    data.IdCardNO = "36232919870913257X";
                        //    data.BmpPath = @"C:\Users\ljy\Documents\Tencent Files\497258352\FileRecv\ConsoleApplication2\VIA.Resource\1.jpg";
                        //}
                        //index++;
                        //try
                        //{
                        //    dis.Dispatcher.Invoke(new Action(() =>
                        //    {
                        //        callback.Invoke(data,AccessControlOprator.SelectModel<IdCardOprator.Model.BlackList>("CardNO='" + data.IdCardNO + "'").Count>0?true:false);
                        //    }));
                        //}
                        //catch (Exception)
                        //{

                        //}

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
                    IdCardSDK.SNBC_CloseComm(_iComm);
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
    public class PersonData
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
