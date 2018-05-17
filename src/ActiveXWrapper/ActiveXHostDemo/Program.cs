using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace ActiveXHostDemo
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            DynamicMethod dm = new DynamicMethod("Test", null,
              new Type[] { typeof(string) }, typeof(string).Module);
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);//把参数推到堆栈上
            MethodInfo call = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
            il.Emit(OpCodes.Call, call);//执行Console.WriteLine方法
            il.Emit(OpCodes.Ret);//结束返回
            Action<string> test = (Action<string>)dm.CreateDelegate(typeof(Action<string>));
            test("henry");

            //下面Test1方法和Test完成的方法是一样的,但IL似乎有些不同.
            //主要体现变量设置,对于变量的位置也会影响指令
            dm = new DynamicMethod("Test1", null,
                new Type[] { typeof(string) }, typeof(string).Module);
            il = dm.GetILGenerator();
            il.DeclareLocal(typeof(string));
            il.Emit(OpCodes.Ldarg_0);//把参数推到堆栈上
            il.Emit(OpCodes.Stloc_0);//把值保存到索引为0的变量里
            il.Emit(OpCodes.Ldloc_0);//把索引为0的变量推到堆栈上
            call = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
            il.Emit(OpCodes.Call, call);//执行Console.WriteLine方法
            il.Emit(OpCodes.Ret);
            test = (Action<string>)dm.CreateDelegate(typeof(Action<string>));
            test("henry");

            //对于下面的方法大家自己推一下,其实很简单.
            //如果看起来有不明白,不防copy到vs.net上然后看指令描述信息:)
            dm = new DynamicMethod("Test2", null,
               new Type[] { typeof(string) }, typeof(string).Module);
            il = dm.GetILGenerator();
            il.DeclareLocal(typeof(string));
            il.Emit(OpCodes.Ldstr, "你好 ");
            il.Emit(OpCodes.Ldarg_0);
            call = typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });
            il.Emit(OpCodes.Call, call);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            call = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
            il.Emit(OpCodes.Call, call);
            il.Emit(OpCodes.Ret);
            test = (Action<string>)dm.CreateDelegate(typeof(Action<string>));
            test("henry");
            Console.Read();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
