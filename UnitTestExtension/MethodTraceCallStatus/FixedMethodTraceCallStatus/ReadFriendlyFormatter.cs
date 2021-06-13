using System;

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
            var converter = Activator.CreateInstance(converterType, false) as IReadFriendlyConverter;
            if (converter == null)
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
        /// <inheritdoc />
        public string Convert(FixedMethodTraceCallStatus me)
        {
            throw new NotImplementedException();
        }
    }
}