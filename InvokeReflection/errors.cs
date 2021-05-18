using System;

namespace SPEkit.InvokeReflection
{
    public static partial class InvokeReflection
    {
        /// <summary>
        ///     函数没找着异常
        /// </summary>
        public sealed class FuncNotExistsError : Exception
        {
            internal FuncNotExistsError()
            {
            }

            internal FuncNotExistsError(string s) : base(s)
            {
            }

            internal FuncNotExistsError(string s, Exception e) : base(s, e)
            {
            }
        }

        /// <summary>
        ///     函数找着太多个异常
        /// </summary>
        public sealed class AmbiguousFuncError : Exception
        {
            internal AmbiguousFuncError()
            {
            }

            internal AmbiguousFuncError(string s)
            {
            }

            internal AmbiguousFuncError(string s, Exception e) : base(s, e)
            {
            }
        }
    }
}