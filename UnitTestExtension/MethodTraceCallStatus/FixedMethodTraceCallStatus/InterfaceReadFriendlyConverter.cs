namespace SPEkit.UnitTestExtension
{
    /// <summary>
    ///     实现了可以将<see cref="FixedMethodTraceCallStatus" />转换为自定义字符串的方法的接口
    /// </summary>
    public interface IReadFriendlyConverter
    {
        /// <summary>
        ///     此方法可以实现将<see cref="FixedMethodTraceCallStatus" />转为字符串
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public string Convert(FixedMethodTraceCallStatus me);
    }
}