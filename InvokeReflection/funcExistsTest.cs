using System;
using System.Reflection;

// ReSharper disable MemberCanBePrivate.Global

namespace SPEkit.InvokeReflection
{
    public static partial class InvokeReflection
    {
        /// <summary>
        ///     检查是否存在此无参函数
        /// </summary>
        /// <param name="callerObject">检查的对象实例</param>
        /// <param name="methodName">方法名</param>
        /// <returns></returns>
        public static bool IsExists(object callerObject, string methodName)
        {
            try
            {
                return callerObject.GetType().GetMethod(methodName) != null;
            }
            catch (AmbiguousMatchException)
            {
                return true;
            }
        }

        /// <summary>
        ///     检查是否存在此有参函数
        /// </summary>
        /// <param name="callerObject">检查的对象实例</param>
        /// <param name="methodName">方法名</param>
        /// <param name="typeClass">方法参数类表</param>
        /// <returns></returns>
        public static bool IsExists(object callerObject, string methodName, params Type[] typeClass)
        {
            try
            {
                return callerObject.GetType().GetMethod(methodName, typeClass) != null;
            }
            catch (AmbiguousMatchException)
            {
                return true;
            }
        }

        /// <summary>
        ///     检查无参函数是否是独有的
        /// </summary>
        /// <param name="callerObject">检查的对象实例</param>
        /// <param name="methodName">方法名</param>
        /// <returns></returns>
        [Obsolete("历史遗留函数，现在没啥蛋用")]
        public static bool IsUnique(object callerObject, string methodName)
        {
            if (!IsExists(callerObject, methodName)) return false;
            try
            {
                callerObject.GetType().GetMethod(methodName,Type.EmptyTypes);
            }
            catch (AmbiguousMatchException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     检查有参函数是否是独有的
        /// </summary>
        /// <param name="callerObject">检查的对象实例</param>
        /// <param name="methodName">方法名</param>
        /// <param name="typeClass">方法参数类表</param>
        /// <returns></returns>
        [Obsolete("历史遗留函数，现在没啥蛋用")]
        public static bool IsUnique(object callerObject, string methodName, params Type[] typeClass)
        {
            if (!IsExists(callerObject, methodName, typeClass)) return false;
            try
            {
                callerObject.GetType().GetMethod(methodName, typeClass);
            }
            catch (AmbiguousMatchException)
            {
                return false;
            }

            return true;
        }
    }
}