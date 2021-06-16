using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPEkit.UnitTestExtension
{
    public sealed partial class FixedMethodTraceCallStatus
    {
        /// <summary>
        ///     当前<see cref="ToReadFriendly(int?)" />使用的<see cref="IReadFriendlyConverter" />实现
        /// </summary>
        /// <remarks>默认为<see cref="DefaultReadFriendlyConverter" /></remarks>
        public static IReadFriendlyConverter Converter { get; private set; } = new DefaultReadFriendlyConverter();


        public string ToReadFriendly(int? maxExceptionIndex = null)
        {
            return Converter.Convert(this, maxExceptionIndex);
        }

        public async Task<string> ToReadFriendlyAsync(int? maxExceptionIndex = null)
        {
            return await Converter.ConvertAsync(this, maxExceptionIndex);
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
            Converter = _createConverter(converterType);
        }

        public string ToReadFriendly<TConverter>(int? maxExceptionIndex = null)
            where TConverter : IReadFriendlyConverter, new()
        {
            return new TConverter().Convert(this, maxExceptionIndex);
        }

        public async Task<string> ToReadFriendlyAsync<TConverter>(int? maxExceptionIndex = null)
            where TConverter : IReadFriendlyConverter, new()
        {
            return await new TConverter().ConvertAsync(this, maxExceptionIndex);
        }

        public string ToReadFriendly(IReadFriendlyConverter converter, int? maxExceptionIndex = null)
        {
            return converter.Convert(this, maxExceptionIndex);
        }

        public async Task<string> ToReadFriendlyAsync(IReadFriendlyConverter converter, int? maxExceptionIndex = null)
        {
            return await converter.ConvertAsync(this, maxExceptionIndex);
        }

        public string ToReadFriendly(Type converterType, int? maxExceptionIndex = null)
        {
            var converter = _createConverter(converterType);
            return converter.Convert(this, maxExceptionIndex);
        }

        public async Task<string> ToReadFriendlyAsync(Type converterType, int? maxExceptionIndex = null)
        {
            return await _createConverter(converterType).ConvertAsync(this, maxExceptionIndex);
        }

        private IReadFriendlyConverter _createConverter(Type converterType)
        {
            if (!converterType.IsAssignableTo(typeof(IReadFriendlyConverter)))
                throw new ArgumentException(
                    $"{nameof(converterType)} should be derived from ${nameof(IReadFriendlyConverter)} but got type {converterType}");
            if (Activator.CreateInstance(converterType, false) is not IReadFriendlyConverter converter)
                throw new ArgumentException(
                    $"Cannot create {converterType} instance as {nameof(IReadFriendlyConverter)}");
            return converter;
        }
    }

    /// <summary>
    ///     默认的<see cref="IReadFriendlyConverter" />实现
    /// </summary>
    public sealed class DefaultReadFriendlyConverter : IReadFriendlyConverter
    {
        private static int _maxExceptionWarpIndex = 3;

        /// <inheritdoc />
        public int MAX_EXCEPTION_WARP_INDEX
        {
            get => _maxExceptionWarpIndex;
            set
            {
                if (value < 1) throw new ArgumentException($"Must >=1 but got {value}");
                _maxExceptionWarpIndex = value;
            }
        }

        /// <inheritdoc />
        public string Convert(FixedMethodTraceCallStatus me, int? maxExceptionIndex = null)
        {
            //写单元测试
            var ans = new StringBuilder();
            ans.AppendLine("***************TRACE-START***************");
            var sfTask = Task.Run(() => SessionsFormat(me, maxExceptionIndex));
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

        /// <inheritdoc />
        public async Task<string> ConvertAsync(FixedMethodTraceCallStatus me, int? maxExceptionIndex = null,
            CancellationToken? token = null)
        {
            return await Task.Run(() => Convert(me, maxExceptionIndex), token ?? CancellationToken.None);
        }

        private static string SessionsFormat(FixedMethodTraceCallStatus me, int? maxExceptionIndex)
        {
            if (!me.Sessions.Any()) return "There are 0 sessions.";
            var ans = new StringBuilder();
            ans.AppendLine($"There are {me.Sessions.Count} sessions.");

            var sessions = (from session in me.Sessions orderby session.Value.StartTime select session).ToArray();
            //var tasks = new Task<string>[sessions.Length];
            //for (var i = 1; i <= sessions.Length; i++)
            //{
            //    var i1 = i;
            //    tasks[i - 1] = Task.Run(() => OneSessionFormat(sessions[i1 - 1], i1, maxExceptionIndex));
            //}


            //foreach (var s in await Task.WhenAll(tasks)) ans.AppendLine(s);
            var sessionsString = new string[sessions.Length];
            Parallel.For(0, sessions.Length - 1,
                index => { sessionsString[index] = OneSessionFormat(sessions[index], index + 1, maxExceptionIndex); });
            foreach (var s in sessionsString) ans.AppendLine(s);


            return ans.ToString();
        }

        private static string OneSessionFormat(KeyValuePair<object, MethodTraceCallStatusAttribute.CallSession> one,
            int index, int? maxExceptionIndex)
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
                foreach (var s in StartExceptionFormatter(data.exce, maxExceptionIndex)
                    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
                {
                    ans.Append('|');
                    ans.AppendLine(s);
                }

            var stackMaker = from frame in data.Stack.GetFrames().AsParallel().AsOrdered()
                select
                    $"|+at {frame.GetMethod()?.Name ?? "<UNKNOWN FUNC>"} in {frame.GetFileName() ?? "<UNKNOWN FILE>"} ({frame.GetFileLineNumber()},{frame.GetFileColumnNumber()})";
            ans.AppendLine("|+Stack:");
            foreach (var frame in stackMaker) ans.AppendLine(frame);
            ans.AppendLine($"┕Session #{index} END");
            return ans.ToString();
        }

        private static string ExceptionFormatter(Exception exc, int index, int maxExceptionIndex)
        {
            var ans = new StringBuilder();
            ans.AppendLine($"*Exception:{exc}");
            ans.AppendLine($"*Message: {exc.Message}");
            ans.AppendLine($"*Source: {exc.Source ?? "null"}");
            var trace = exc.StackTrace?.Trim().Split(Environment.NewLine,
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (trace == null || !trace.Any())
            {
                ans.AppendLine("*-No stack trace data.");
            }
            else
            {
                ans.AppendLine("*-=Trace start");
                foreach (var s in trace) ans.AppendLine($"*-{s}");
                ans.AppendLine("*-=Trace end");
            }

            if (exc.InnerException == null)
            {
                ans.AppendLine("*No inner exception");
            }
            else
            {
                if (index >= maxExceptionIndex)
                    ans.AppendLine($"*Inner exception recursion limit reached:{maxExceptionIndex}");
                else
                    foreach (var inner in ExceptionFormatter(exc, index + 1, maxExceptionIndex)
                        .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
                    {
                        ans.Append('*');
                        ans.AppendLine(inner);
                    }
            }

            return ans.ToString();
        }

        private static string StartExceptionFormatter(Exception exc, int? maxExceptionIndex)
        {
            return ExceptionFormatter(exc, 0, maxExceptionIndex ?? _maxExceptionWarpIndex);
        }
    }
}