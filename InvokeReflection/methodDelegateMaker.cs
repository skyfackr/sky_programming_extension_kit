using System;
using System.Reflection;

namespace SPEkit.InvokeReflection
{
    public static partial class InvokeReflection
    {
        /// <summary>
        ///     通过反射查找一个无参函数并创建其委托
        /// </summary>
        /// <typeparam name="T">可用于该函数的委托类型</typeparam>
        /// <param name="caller">含有该函数的实例</param>
        /// <param name="methodName">函数名</param>
        /// <param name="nonPublic">是否是非公开函数</param>
        /// <returns>函数委托</returns>
        /// <exception cref="AmbiguousFuncError"></exception>
        /// <exception cref="FuncNotExistsError"></exception>
        public static T MakeDelegate<T>(object caller, string methodName, bool nonPublic = false) where T : Delegate
        {
            //if (!IsExists(caller, methodName))
            //{
            //    throw new FuncNotExistsError();
            //}
            var t = caller.GetType();
            MethodInfo method;
            try
            {
                method = t.GetMethod(methodName,
                    BindingFlags.Instance | (nonPublic ? BindingFlags.NonPublic : BindingFlags.Default), null,
                    Type.EmptyTypes, null) ?? t.GetMethod(methodName,
                    BindingFlags.Static | (nonPublic ? BindingFlags.NonPublic : BindingFlags.Default), null,
                    Type.EmptyTypes, null);
            }
            catch (AmbiguousMatchException e)
            {
                //Console.WriteLine(e);
                throw new AmbiguousFuncError("found too many func", e);
            }

            if (method == null) throw new FuncNotExistsError();
            //Trace.WriteLine(typeof(T).GetMethod("Invoke").ToString());
            return _createReturnDelegate<T>(caller, method);
        }

        /// <summary>
        ///     通过反射查找一个有参函数并创建其委托
        /// </summary>
        /// <typeparam name="T">可用于该函数的委托类型</typeparam>
        /// <param name="caller">含有该函数的实例</param>
        /// <param name="methodName">函数名</param>
        /// <param name="nonPublic">是否公开函数</param>
        /// <param name="typeClass">此函数参数各自类型</param>
        /// <returns>函数委托</returns>
        /// <exception cref="AmbiguousFuncError"></exception>
        /// <exception cref="FuncNotExistsError"></exception>
        public static T MakeDelegate<T>(object caller, string methodName, bool nonPublic, params Type[] typeClass)
            where T : Delegate
        {
            //if (!IsExists(caller, methodName,typeClass))
            //{
            //    throw new FuncNotExistsError();
            //}
            var t = caller.GetType();
            MethodInfo method;
            try
            {
                method =
                    t.GetMethod(methodName,
                        BindingFlags.Instance | (nonPublic ? BindingFlags.NonPublic : BindingFlags.Default), null,
                        typeClass, null) ?? t.GetMethod(methodName,
                        BindingFlags.Static | (nonPublic ? BindingFlags.NonPublic : BindingFlags.Default), null,
                        typeClass, null);
            }
            catch (AmbiguousMatchException e)
            {
                //Console.WriteLine(e);
                throw new AmbiguousFuncError("found too many func", e);
            }

            if (method == null) throw new FuncNotExistsError();
            return _createReturnDelegate<T>(caller, method);
        }

        /// <inheritdoc cref="MakeDelegate{T}(object,string,bool,Type[])" />
        public static T MakeDelegate<T>(object caller, string methodName, params Type[] typeClass)
            where T : Delegate
        {
            return MakeDelegate<T>(caller, methodName, false, typeClass);
        }

        private static T _createReturnDelegate<T>(object caller, MethodInfo method) where T : Delegate
        {
            if (method.IsStatic) return method.CreateDelegate<T>(null);
            return method.CreateDelegate<T>(caller);
        }
    }
}