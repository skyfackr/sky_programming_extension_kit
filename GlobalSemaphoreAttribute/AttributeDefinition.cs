﻿using System;
using System.Data.SqlTypes;
using PostSharp.Aspects;

namespace SPEkit.SemaphoreSlimAttribute
{
    [AttributeUsage(AttributeTargets.Constructor|AttributeTargets.Delegate|AttributeTargets.Event|AttributeTargets.Field|AttributeTargets.Method|AttributeTargets.Module|AttributeTargets.Property,AllowMultiple = true,Inherited = false)]
    public sealed partial class SlotWaitAttribute : OnMethodBoundaryAspect
    {

    }
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Module | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed partial class SlotWaitAsyncAttribute : OnMethodBoundaryAspect
    {

    }
}
