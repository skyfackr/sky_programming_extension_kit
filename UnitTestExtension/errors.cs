using System;

namespace SPEkit.UnitTestExtension
{
    /// <summary>
    ///     相关Attribute未标识
    /// </summary>
    public sealed class AttributeNotRegisterException : Exception
    {
        internal AttributeNotRegisterException(string msg) : base($"Attribute {msg} excepted.")
        {
        }
    }

    /// <summary>
    ///     无法找到有相关attribute的Method
    /// </summary>
    public sealed class AttributeNotFoundException : Exception
    {
        internal AttributeNotFoundException(string msg, Type type) : base(
            $"Cannot find attribute {msg} in {type} class")
        {
        }
    }

    /// <summary>
    ///     统计方法attribute注册时未注册任何跟踪类，并排除了自身类
    /// </summary>
    public sealed class AttributeConfigurationNullException : Exception
    {
        internal AttributeConfigurationNullException() : base(
            "Cannot use this function without config any monitored type")
        {
        }
    }
}