using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEkit.UnitTestExtension
{
    public sealed class AttributeNotRegisterException : Exception
    {
        internal AttributeNotRegisterException(string msg):base($"Attribute {msg} excepted."){}

    }

    public sealed class AttributeNotFoundException : Exception
    {
        internal AttributeNotFoundException(string msg, Type type) : base(
            $"Cannot find attribute {msg} in {type.ToString()} class"){}
    }

    public sealed class AttributeConfigurationNullException : Exception
    {
        internal AttributeConfigurationNullException():base("Cannot use this function without config any monitored type"){}
    }
}
