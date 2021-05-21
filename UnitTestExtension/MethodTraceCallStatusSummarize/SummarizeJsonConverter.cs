using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SPEkit.UnitTestExtension
{
    public sealed partial class MethodTraceCallStatusSummarizeAttribute
    {
        public static Dictionary<MethodInfo, MethodTraceCallStatusAttribute> DeserializeFromJson()
    }
}
