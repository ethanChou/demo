﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;

namespace ActiveXHost
{
    /// <summary>
    /// Generates delegates according to the specified signature on runtime
    /// </summary>
 [ComVisible(false)]
    public static class EventHandlerFacotroy
    {
        /// <summary>
        /// Generates a delegate with a matching signature of the supplied eventHandlerType
        /// This method only supports Events that have a delegate of type void
        /// </summary>
        /// <param name="eventInfo">The delegate type to wrap. Note that this must always be a void delegate</param>
        /// <param name="methodToInvoke">The method to invoke</param>
        /// <param name="methodInvoker">The object where the method resides</param>
        /// <returns>Returns a delegate with the same signature as eventHandlerType that calls the methodToInvoke inside</returns>
        public static Delegate CreateDelegate(Type eventHandlerType, MethodInfo methodToInvoke, object methodInvoker)
        {
            //Get the eventHandlerType signature
            var eventHandlerInfo = eventHandlerType.GetMethod("Invoke");
            Type returnType = eventHandlerInfo.ReturnParameter.ParameterType;
            if (returnType != typeof(void))
                throw new ApplicationException("Delegate has a return type. This only supprts event handlers that are void");

            ParameterInfo[] delegateParameters = eventHandlerInfo.GetParameters();
            //Get the list of type of parameters. Please note that we do + 1 because we have to push the object where the method resides i.e methodInvoker parameter
            Type[] hookupParameters = new Type[delegateParameters.Length + 1];
            hookupParameters[0] = methodInvoker.GetType();
            for (int i = 0; i < delegateParameters.Length; i++)
                hookupParameters[i + 1] = delegateParameters[i].ParameterType;

            DynamicMethod handler = new DynamicMethod("", null,
                hookupParameters, typeof(EventHandlerFacotroy));

            ILGenerator eventIL = handler.GetILGenerator();

            //load the parameters or everything will just BAM :)
            LocalBuilder local = eventIL.DeclareLocal(typeof(object[]));
            eventIL.Emit(OpCodes.Ldc_I4, delegateParameters.Length + 1);
            eventIL.Emit(OpCodes.Newarr, typeof(object));
            eventIL.Emit(OpCodes.Stloc, local);

            //start from 1 because the first item is the instance. Load up all the arguments
            for (int i = 1; i < delegateParameters.Length + 1; i++)
            {
                eventIL.Emit(OpCodes.Ldloc, local);
                eventIL.Emit(OpCodes.Ldc_I4, i);
                eventIL.Emit(OpCodes.Ldarg, i);
                eventIL.Emit(OpCodes.Stelem_Ref);
            }

            eventIL.Emit(OpCodes.Ldloc, local);

            //Load as first argument the instance of the object for the methodToInvoke i.e methodInvoker
            eventIL.Emit(OpCodes.Ldarg_0);
            eventIL.Emit(OpCodes.Ldarg_1);
            eventIL.Emit(OpCodes.Ldarg_2);
            eventIL.Emit(OpCodes.Ldarg_3);
            //Now that we have it all set up call the actual method that we want to call for the binding
            eventIL.EmitCall(OpCodes.Call, methodToInvoke,null);
            
            eventIL.Emit(OpCodes.Pop);
            eventIL.Emit(OpCodes.Ret);

            //create a delegate from the dynamic method
            return handler.CreateDelegate(eventHandlerType, methodInvoker);
        }

        /// <summary>
        /// 生成 Microsoft 中间语言 (MSIL) 指令。不需要C#编译器了
        /// test demo
        /// </summary>
        private static void CreateAssembly()
        {
            //动态创建程序集
            AssemblyName DemoName = new AssemblyName("DynamicAssembly");
            AssemblyBuilder dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(DemoName, AssemblyBuilderAccess.RunAndSave);
            //动态创建模块
            ModuleBuilder mb = dynamicAssembly.DefineDynamicModule(DemoName.Name, DemoName.Name + ".dll");
            //动态创建类MyClass
            TypeBuilder tb = mb.DefineType("MyClass", TypeAttributes.Public);
            //动态创建字段
            FieldBuilder fb = tb.DefineField("myField", typeof(System.String), FieldAttributes.Private);
            //动态创建构造函数
            Type[] clorType = new Type[] { typeof(System.String) };
            ConstructorBuilder cb1 = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, clorType);
            //生成指令
            ILGenerator ilg = cb1.GetILGenerator();//生成 Microsoft 中间语言 (MSIL) 指令
            ilg.Emit(OpCodes.Ldarg_0);
            ilg.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            ilg.Emit(OpCodes.Ldarg_0);
            ilg.Emit(OpCodes.Ldarg_1);
            ilg.Emit(OpCodes.Stfld, fb);
            ilg.Emit(OpCodes.Ret);
            //动态创建属性
            tb.DefineProperty("MyProperty", PropertyAttributes.HasDefault, typeof(string), null);
            //动态创建方法
            MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName;
            MethodBuilder myMethod = tb.DefineMethod("get_Field", getSetAttr, typeof(string), Type.EmptyTypes);
            //生成指令
            ILGenerator numberGetIl = myMethod.GetILGenerator();
            numberGetIl.Emit(OpCodes.Ldarg_0);
            numberGetIl.Emit(OpCodes.Ldfld, fb);
            numberGetIl.Emit(OpCodes.Ret);
            //使用动态类创建类型
            tb.CreateType();
            //保存动态创建的程序集 (程序集将保存在程序目录下调试时就在Debug下)
            dynamicAssembly.Save(DemoName.Name + ".dll");
        }

    }
}
