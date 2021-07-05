using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SPEkit.BinLikeClassSelectors
{
    /// <summary>
    /// 此模块可以用于借助二进制形式数值<see cref="long"/>以及位运算达到极快速判断输入选项是否被选中（仅限或运算）
    /// <para>具体功能详见<seealso cref="BinLikeClassSelectorUnit"/><para>此类不得直接创建或者无参构造，可使用<see cref="CreateBinLikeClassSelectorUnit"/>创建或对其派生新类</para></para>
    /// </summary>
    public static partial class BinLikeClassSelector
    {
        /// <summary>
        /// 创建一个<see cref="BinLikeClassSelectorUnit"></see>，同样的输入将导致输出同一个实例引用
        /// </summary>
        /// <param name="binObject">以二进制表明被选中位的数字，可以以十进制输入</param>
        /// <returns>一个实例化的<see cref="BinLikeClassSelectorUnit"/>，同样的输入将导致输出同一个实例引用</returns>
        public static BinLikeClassSelectorUnit CreateBinLikeClassSelectorUnit(long binObject)
        {
            BinLikeClassSelectorUnit ans;
            if (_cache.ContainsKey(binObject))
            {
                ans = _cache[binObject];
            }
            else
            {
                ans = new BinLikeClassSelectorUnit(binObject);
                _cache.Add(binObject,ans);
            }

            return ans;
        }

        private static readonly Dictionary<long, BinLikeClassSelectorUnit> _cache =
            new Dictionary<long, BinLikeClassSelectorUnit>();
    }
}
