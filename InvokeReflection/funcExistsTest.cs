using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SPEkit.InvokeReflection
{
    public static partial class InvokeMethodAsString
    {
        public static bool isExists(object callerObject, string methodName)
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

        public static bool isExists(object callerObject, string methodName, params Type[] typeClass)
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

        public static bool isUnique(object callerObject, string methodName)
        {
            if (!isExists(callerObject, methodName)) return false;
            try
            {
                callerObject.GetType().GetMethod(methodName);
            }
            catch (AmbiguousMatchException)
            {
                return false;
            }

            return true;
        }

        public static bool isUnique(object callerObject, string methodName, params Type[] typeClass)
        {
            if (!isExists(callerObject, methodName,typeClass)) return false;
            try
            {
                callerObject.GetType().GetMethod(methodName,typeClass);
            }
            catch (AmbiguousMatchException)
            {
                return false;
            }

            return true;
        }
    }
}
