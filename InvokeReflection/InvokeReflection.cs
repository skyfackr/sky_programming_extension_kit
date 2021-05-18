using System;
using System.Linq;
using System.Reflection;

namespace SPEkit.InvokeReflection
{
    /// <summary>
    ///     便于通过反射进行动态调用函数的包装
    /// </summary>
    public static partial class InvokeReflection
    {
        /// <summary>
        ///     反射调用无参数函数
        /// </summary>
        /// <param name="callObject">包含函数的实例对象</param>
        /// <param name="methodName">方法名</param>
        /// <returns>此函数返回的结果</returns>
        /// <exception cref="AmbiguousFuncError"></exception>
        /// <exception cref="FuncNotExistsError"></exception>
        public static object Invoke(object callObject, string methodName)
        {
            var t = callObject.GetType();
            MethodInfo method;
            try
            {
                method = t.GetMethod(methodName,Type.EmptyTypes);
            }
            catch (AmbiguousMatchException e)
            {
                //Console.WriteLine(e);
                throw new AmbiguousFuncError("found too many func", e);
            }

            if (method == null) throw new FuncNotExistsError();
            try
            {
                return method.Invoke(callObject, null);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException ?? e;
            }
        }

        /// <summary>
        ///     反射调用带参数函数
        /// </summary>
        /// <param name="callObject">包含函数的实例对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">参数</param>
        /// <returns>函数返回的结果</returns>
        /// <exception cref="AmbiguousFuncError"></exception>
        /// <exception cref="FuncNotExistsError"></exception>
        public static object Invoke(object callObject, string methodName, params object[] args)
        {
            var t = callObject.GetType();
            MethodInfo method;
            var typeClass = (from nowtype in args
                select nowtype.GetType()).ToArray();
            try
            {
                method = t.GetMethod(methodName, typeClass);
            }
            catch (AmbiguousMatchException e)
            {
                //Console.WriteLine(e);
                throw new AmbiguousFuncError("found too many func", e);
            }

            if (method == null) throw new FuncNotExistsError();
            try
            {
                return method.Invoke(callObject, args);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException ?? e;
            }
        }
    }
}