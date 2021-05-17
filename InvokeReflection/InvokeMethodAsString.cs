using System;
using System.Reflection;

namespace SPEkit.InvokeReflection
{
    public static partial class InvokeMethodAsString
    {
        public static object invoke(object callObject, string methodName)
        {
            Type t = callObject.GetType();
            MethodInfo method;
            try
            {
                method = t.GetMethod(methodName);
            }
            catch (AmbiguousMatchException e)
            {
                //Console.WriteLine(e);
                throw new AmbiguityFuncError("found too many func", e);
            }

            if (method == null) throw new FuncNotExistsError();
            return method.Invoke(callObject, null);
        }

        public static object invoke(object callObject, string methodName, Type[] typeClass, params object[] args)
        {
            Type t = callObject.GetType();
            MethodInfo method;
            try
            {
                method = t.GetMethod(methodName, typeClass);
            }
            catch (AmbiguousMatchException e)
            {
                //Console.WriteLine(e);
                throw new AmbiguityFuncError("found too many func", e);
            }

            if (method == null) throw new FuncNotExistsError();
            return method.Invoke(callObject, args);
        }
    }
}