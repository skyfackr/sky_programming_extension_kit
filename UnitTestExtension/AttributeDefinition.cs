using System;
using PostSharp.Aspects;

namespace SPEkit.UnitTestExtension
{
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Property,AllowMultiple = true,Inherited = false)]
    public sealed partial class MethodTraceCallStatusAttribute:OnMethodBoundaryAspect
    {
        
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed partial class MethodTraceCallStatusSummarizeAttribute : OnMethodBoundaryAspect
    {

    }

}
