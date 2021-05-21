using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Aspects;

namespace SPEkit.UnitTestExtension
{
    public sealed partial class MethodTraceCallStatusSummarizeAttribute
    {
        public MethodTraceCallStatusSummarizeAttribute(Type[] includeExtraTypes = null, bool ignoreThisType = false) :
            base()
        {
            this._includeExtraTypes = includeExtraTypes;
            this._ignoreThisType = ignoreThisType;
        }

        private readonly Type[] _includeExtraTypes;
        private readonly bool _ignoreThisType;

        public static Dictionary<MethodInfo, MethodTraceCallStatusAttribute> GetTraceMsgInSpecifiedClass(Type type)
        {

            var methods = _getRegisteredMethods(type);
            if (!methods.Any())
                throw new AttributeNotFoundException(typeof(MethodTraceCallStatusAttribute).ToString(), type);
            var ans = methods.ToDictionary(method => method, method => method.GetCustomAttributes<MethodTraceCallStatusAttribute>().First());
            return ans;
        }

        public static bool IsRegistered(MethodBase method)
        {
            return method.IsDefined(typeof(MethodTraceCallStatusSummarizeAttribute));
        }

        public static Dictionary<MethodInfo, MethodTraceCallStatusAttribute> GetTraceMsg()
        {
            var callMethod = (new StackTrace()).GetFrame(1).GetMethod();
            if (callMethod == null || IsRegistered(callMethod))
            {
                throw new AttributeNotRegisterException(typeof(MethodTraceCallStatusSummarizeAttribute).ToString());
            }

            var attr = callMethod.GetCustomAttributes<MethodTraceCallStatusSummarizeAttribute>().First();
            var callClass = callMethod.DeclaringType;
            if (callClass == null )
                throw new AttributeNotFoundException(typeof(MethodTraceCallStatusAttribute).ToString(), typeof(object));
            var methods = _getRegisteredMethods(callClass);
            if (!methods.Any())
                throw new AttributeNotFoundException(typeof(MethodTraceCallStatusAttribute).ToString(), callClass);
            return methods.ToDictionary(method => method, method => method.GetCustomAttributes<MethodTraceCallStatusAttribute>().First());
        }

        private static IOrderedEnumerable<MethodInfo> _getRegisteredMethods(Type type)
        {
            return from method in type.GetMethods()
                where Attribute.IsDefined(method, typeof(MethodTraceCallStatusAttribute))
                orderby method.Name
                select method;
        }

        public static Dictionary<Type, Dictionary<MethodInfo, MethodTraceCallStatusAttribute>> GetTraceMsgs()
        {
            var callMethod = (new StackTrace()).GetFrame(1).GetMethod();
            if (callMethod == null || IsRegistered(callMethod))
            {
                throw new AttributeNotRegisterException(typeof(MethodTraceCallStatusSummarizeAttribute).ToString());
            }

            var attr = callMethod.GetCustomAttributes<MethodTraceCallStatusSummarizeAttribute>().First();
            var callClass = callMethod.DeclaringType;
            if (callClass == null)
                throw new AttributeNotFoundException(typeof(MethodTraceCallStatusAttribute).ToString(), typeof(object));
            var types = attr._includeExtraTypes.AsEnumerable();
            if (!attr._ignoreThisType)
                types=types.Append(callClass);
            if (!types.Any()) throw new AttributeConfigurationNullException();
            return types.ToDictionary(
                type => type, GetTraceMsgInSpecifiedClass);
        }
    }
}
