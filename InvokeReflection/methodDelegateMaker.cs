using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Reflection;

namespace SPEkit.InvokeReflection
{
    public static partial class InvokeMethodAsString
    {
        public static T makeDelegate<T>(object caller, string methodName) where T:System.Delegate
        {
            if (!isExists(caller, methodName))
            {
                throw new FuncNotExistsError();
            }
            Type t = caller.GetType();
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

            //if (method == null) throw new FuncNotExistsError();
            return (T)method.CreateDelegate(typeof(T));
        }

        public static T makeDelegate<T>(object caller, string methodName, params Type[] typeClass)
            where T : System.Delegate
        {
            if (!isExists(caller, methodName,typeClass))
            {
                throw new FuncNotExistsError();
            }
            Type t = caller.GetType();
            MethodInfo method;
            try
            {
                method = t.GetMethod(methodName,typeClass);
            }
            catch (AmbiguousMatchException e)
            {
                //Console.WriteLine(e);
                throw new AmbiguityFuncError("found too many func", e);
            }

            //if (method == null) throw new FuncNotExistsError();
            return (T)method.CreateDelegate(typeof(T));
        }

        

    }
}
