using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


namespace CSInvokeCPP
{
    class Program
    {
        static void Main(string[] args)
        {
            CSInvokeCPP.CPPAdapter.ClassGroup cgp;

            cgp = new CPPAdapter.ClassGroup();

            int m = CPPAdapter.InitClassGroup(ref cgp);

            Console.Write("*");


            int len = Marshal.SizeOf(typeof(CSInvokeCPP.CPPAdapter.Student));
            // 调用复杂结构体测试  
            int size = Marshal.SizeOf(typeof(CSInvokeCPP.CPPAdapter.ClassGroup)) * 50;
            IntPtr pBuff = Marshal.AllocHGlobal(size); // 直接分配50个元素的空间,比Marshal.copy方便多了  
            CPPAdapter.GetClassGroup(pBuff, 50);

            CSInvokeCPP.CPPAdapter.ClassGroup[] pClass = new CSInvokeCPP.CPPAdapter.ClassGroup[50];
            for (int i = 0; i < 50; i++)
            {
                IntPtr ptr1 = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(CSInvokeCPP.CPPAdapter.ClassGroup)) * i);
                pClass[i] = (CSInvokeCPP.CPPAdapter.ClassGroup)Marshal.PtrToStructure(ptr1, typeof(CSInvokeCPP.CPPAdapter.ClassGroup));

            }
            Marshal.FreeHGlobal(pBuff); // 释放内存  

            Console.Write(".");



            Console.ReadLine();

            int result = CPPAdapter.Add(10, 20);
            int[] arr = new int[] { 1, 2, 3, 4, 5 };

            Console.WriteLine("10 + 20 = {0}", result);

            result = CPPAdapter.Sub(30, 12);
            Console.WriteLine("30 - 12 = {0}", result);

            result = CPPAdapter.Multiply(5, 4);
            Console.WriteLine("5 * 4 = {0}", result);

            result = CPPAdapter.Divide(30, 5);
            Console.WriteLine("30 / 5 = {0}", result);

            result = CPPAdapter.Sum(arr, 5);
            Console.WriteLine("sum is {0}", result);

            IntPtr ptr = CPPAdapter.Create("Chevkio", 27);
            CPPAdapter.User user = (CPPAdapter.User)Marshal.PtrToStructure(ptr, typeof(CPPAdapter.User));
            Console.WriteLine("Name: {0}, Age: {1}", user.Name, user.Age);

            Console.ReadLine();

            ///--------1
            //IntPtr pv = Marshal.AllocHGlobal(148); //结构体在使用时一定要分配空间(4*sizeof(int)+128)  
            //Marshal.WriteInt32(pv, 148); //向内存块里写入数值  
            //if (CPPAdapter.GetVersionPtr(pv)) //直接以非托管内存块地址为参数  
            //{
            //    Console.WriteLine("--osVersion:{0}", Marshal.ReadInt32(pv, 0));
            //    Console.WriteLine("--Major:{0}", Marshal.ReadInt32(pv, 4)); //移动4个字节  
            //    Console.WriteLine("--BuildNum: " + Marshal.ReadInt32(pv, 12));
            //    Console.WriteLine("--szVersion: " + Marshal.PtrToStringAnsi((IntPtr)(pv.ToInt32() + 20)));
            //}
            //Marshal.FreeHGlobal(pv); //处理完记得释放内存 

            ///-----------2
            CSInvokeCPP.CPPAdapter.OSINFO info = new CPPAdapter.OSINFO();
            bool f = CPPAdapter.GetVersionPtr(ref info);

            Console.WriteLine("--osVersion:{0}", info.osVersion);
            Console.WriteLine("--Major:{0}", info.majorVersion); //移动4个字节  
            Console.WriteLine("--BuildNum: " + info.buildNum);
            Console.WriteLine("--minorVersion: " + info.minorVersion);
            Console.WriteLine("--platFormId: " + info.platFormId);
            Console.WriteLine("--szVersion: " + info.szVersion);

            Console.WriteLine("----------------------------------");
            Console.ReadLine();

            f = CPPAdapter.GetVersionRef(ref info);


            Console.WriteLine("--osVersion:{0}", info.osVersion);
            Console.WriteLine("--Major:{0}", info.majorVersion); //移动4个字节  
            Console.WriteLine("--BuildNum: " + info.buildNum);
            Console.WriteLine("--minorVersion: " + info.minorVersion);
            Console.WriteLine("--platFormId: " + info.platFormId);
            Console.WriteLine("--szVersion: " + info.szVersion);

            Console.ReadLine();

            Console.WriteLine("----------------------------------");

        }
    }

    public class CPPAdapter
    {
        [DllImport("CSInvokeCDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Add(int x, int y);

        [DllImport("CSInvokeCDll.dll")]
        public static extern int Sub(int x, int y);

        [DllImport("CSInvokeCDll.dll")]
        public static extern int Multiply(int x, int y);

        [DllImport("CSInvokeCDll.dll")]
        public static extern int Divide(int x, int y);

        [DllImport("CSInvokeCDll.dll")]
        public static extern int Sum(int[] Parr, int length);

        [DllImport("CSInvokeCDll.dll")]
        public static extern IntPtr Create(string name, int age);

        [DllImport("CSInvokeCDll.dll")]
        public static extern bool GetVersionPtr(ref OSINFO info);
        //public static extern bool GetVersionPtr(IntPtr info);

        [DllImport("CSInvokeCDll.dll")]
        public static extern bool GetVersionRef(ref OSINFO info);

        [StructLayout(LayoutKind.Sequential)]
        public struct User
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string Name;

            public int Age;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OSINFO
        {
            public int osVersion;
            public int majorVersion;
            public int minorVersion;
            public int buildNum;
            public int platFormId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szVersion;
        }

        // 接口定义   
        [DllImport("CSInvokeCDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetClassGroup(IntPtr pv, int len);
        [DllImport("CSInvokeCDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitClassGroup(ref ClassGroup pv);

        // 结构体定义  
        // Student  
        [StructLayout(LayoutKind.Sequential)]
        public struct Student
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string name;
            public int age;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
            public double[] scores;
        }

        // Class  
        [StructLayout(LayoutKind.Sequential)]
        public struct ClassGroup
        {
            public int number;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)] // 指定数组尺寸   
            public Student[] students; // 结构体数组定义  
        }

    }

}
