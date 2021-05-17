using System;
using System.Collections.Generic;
using System.Text;

namespace SPEkit.InvokeReflection
{
    public static partial class InvokeMethodAsString
    {
        public class FuncNotExistsError : Exception
        {
            public FuncNotExistsError() : base() { }
            public FuncNotExistsError(string s) : base(s) { }
            public FuncNotExistsError(string s,Exception e) : base(s, e) { }
        };

        public class AmbiguityFuncError : Exception
        {
            public AmbiguityFuncError() : base() { }
            public AmbiguityFuncError(string s) : base() { }
            public AmbiguityFuncError(string s,Exception e) : base(s, e) { }
        };


    }
}