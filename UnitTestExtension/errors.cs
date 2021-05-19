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
}
