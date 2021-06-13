using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SPEkit.UnitTestExtension
{
    public sealed partial class FixedMethodTraceCallStatus
    {
        /// <summary>
        ///     当前<see cref="ToReadFriendly()" />使用的<see cref="IReadFriendlyConverter" />实现
        /// </summary>
        /// <remarks>默认为<see cref="DefaultReadFriendlyConverter" /></remarks>
        public static IReadFriendlyConverter Converter { get; private set; } = new DefaultReadFriendlyConverter();

        /// <summary>
        ///     通过<see cref="Converter" />进行字符串转换
        /// </summary>
        /// <returns></returns>
        public string ToReadFriendly()
        {
            return Converter.Convert(this);
        }

        public void SetConverter(IReadFriendlyConverter converter)
        {
            Converter = converter;
        }

        public void SetConverter<TConverter>() where TConverter : IReadFriendlyConverter, new()
        {
            Converter = new TConverter();
        }

        public void SetConverter(Type converterType)
        {
            if (!converterType.IsAssignableTo(typeof(IReadFriendlyConverter)))
                throw new ArgumentException(
                    $"{nameof(converterType)} should be derived from ${nameof(IReadFriendlyConverter)} but got type {converterType}");

            Converter = Activator.CreateInstance(converterType, false) as IReadFriendlyConverter;
        }

        public string ToReadFriendly<TConverter>() where TConverter : IReadFriendlyConverter, new()
        {
            return new TConverter().Convert(this);
        }

        public string ToReadFriendly(IReadFriendlyConverter converter)
        {
            return converter.Convert(this);
        }

        public string ToReadFriendly(Type converterType)
        {
            if (!converterType.IsAssignableTo(typeof(IReadFriendlyConverter)))
                throw new ArgumentException(
                    $"{nameof(converterType)} should be derived from ${nameof(IReadFriendlyConverter)} but got type {converterType}");
            if (Activator.CreateInstance(converterType, false) is not IReadFriendlyConverter converter)
                throw new ArgumentException(
                    $"Cannot create {converterType} instance as {nameof(IReadFriendlyConverter)}");
            return converter.Convert(this);
        }
    }

    /// <summary>
    ///     默认的<see cref="IReadFriendlyConverter" />实现
    /// </summary>
    public sealed class DefaultReadFriendlyConverter : IReadFriendlyConverter
    {
        private static int _MAX_EXCEPTION_WARP_INDEX = 3;

        public static int MAX_EXCEPTION_WARP_INDEX
        {
            get => _MAX_EXCEPTION_WARP_INDEX;
            set
            {
                if (value < 1) throw new ArgumentException($"Must >=1 but got {value}");
                _MAX_EXCEPTION_WARP_INDEX = value;
            }
        }

        /// <inheritdoc />
        public string Convert(FixedMethodTraceCallStatus me)
        {
            //throw new NotImplementedException();
            //写默认实现
            //写单元测试
            var ans = new StringBuilder();
            ans.AppendLine("***************TRACE-START***************");
            var sfTask = Task.Run(() => SessionsFormat(me));
            ans.Append($"Method:{me.Name}(");
            if (me.ParametersTypeName.Any()) ans.AppendJoin(",", me.ParametersTypeName);

            ans.AppendLine($") Token:{me.MetaDataToken}");
            if (me.Method is not MethodInfo meInfo || meInfo.ReturnType == typeof(void))
                ans.AppendLine("This function will not return anything. (Or cannot identify return type.)");
            else
                ans.AppendLine($"Function returns {me.ReturnTypeName}");

            if (me.ParametersTypeName.Any()) ans.AppendLine($"Declared by {me.ParentTypeName}");
            else
                ans.AppendLine("This function have no parent. (Or cannot identify type.)");

            if (me.CustomAttributes.Any())
                ans.AppendLine(
                    $"Custom Attributes:[{string.Join(",", from i in me.CustomAttributes select i.ToString())}]");
            else
                ans.AppendLine(
                    "This function have no custom attribute. (Or cannot identify type such as PostSharp Attribute.)");
            ans.AppendLine(sfTask.GetAwaiter().GetResult());
            ans.AppendLine("****************TRACE-END****************");
            return ans.ToString();
        }

        private static async Task<string> SessionsFormat(FixedMethodTraceCallStatus me)
        {
            if (!me.Sessions.Any()) return "There are 0 sessions.";
            var ans = new StringBuilder();
            ans.AppendLine($"There are {me.Sessions.Count} sessions.");
            throw new NotImplementedException();
        }

        private static string OneSessionFormat(KeyValuePair<object, MethodTraceCallStatusAttribute.CallSession> one,
            int index)
        {
            var ans = new StringBuilder();
            ans.AppendLine($"┍Session #{index} KeyToken:{one.Key.GetHashCode()}");
            var data = one.Value;
            if (data.Arguments.Any()) ans.AppendLine($"|Arguments:[{string.Join(",", data.Arguments)}]");
            else
                ans.AppendLine("|No arguments.");

            if (data.ReturnValue != null) ans.AppendLine($"|Return: {data.ReturnValue}");
            else
                ans.AppendLine("|No returns.");

            ans.AppendLine($"|Session status:{Enum.GetName(data.Status)}");
            ans.Append("|┮");
            if (data.StartTime == null) ans.AppendLine("Not start.");
            else
                ans.AppendLine($"Start at {data.StartTime.Value.ToLocalTime()}");

            ans.AppendLine($"||Current execution time: {data.ExcuteTime}");
            ans.Append("|┴");
            if (data.EndTime == null)
                ans.AppendLine("Not end.");
            else
                ans.AppendLine($"End at {data.EndTime.Value.ToLocalTime()}");

            if (data.exce == null) ans.AppendLine("|No Exception.");
            else
                foreach (var s in ExceptionFormatter(data.exce, 0)
                    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
                {
                    ans.Append('|');
                    ans.AppendLine(s);
                }

            throw new NotImplementedException();
        }

        private static string ExceptionFormatter(Exception exc, int index)
        {
            throw new NotImplementedException();
        }
    }
}