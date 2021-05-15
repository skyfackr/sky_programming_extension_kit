using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
#nullable enable

namespace SPEkit.BinLikeClassSelector
{
    /// <summary>
    /// 此模块可以用于借助二进制形式数值 <see cref="long"/> 以及位运算达到极快速判断输入选项是否被选中（仅限或运算）
    /// <para>该unit为最低单元，由 <see cref="BinLikeClassSelector.CreateBinLikeClassSelectorUnit"/> 创建，不得自行创建或者无参构造</para>
    /// </summary>
    public partial class BinLikeClassSelectorUnit
    {
        
        /// <summary>
        /// 这是用于存储创建对象时保存数据的只读成员
        /// <para>请注意，此对象仅允许创建时赋值，因此本类禁止无参构造</para>
        /// </summary>
        protected readonly long _binObject;

        private List<long>? _binListCache = null;

        /// <summary>
        /// 可直接获取输入的被选中位数据，以 <see cref="long"/> 数值导出
        /// </summary>
        /// <remarks>与强制转换为 <see cref="long"/> 效果等同</remarks>
        /// <returns>一个数值，以 <see cref="long"/> 导出</returns>
        public long GetBin()
        {
            return this._binObject;
        }

        /// <summary>
        /// 获取一个 <see cref="long"/> 组成的 <see cref="List{T}"/>，以被选中位数值组成
        /// </summary>
        /// <returns>
        /// 一个 <see cref="long"/> 组成的 <see cref="List{T}"/>
        /// <para>每个数值为在二进制上仅有一个位是1的 <see cref="long"/> 数值，表示被选中位</para>
        /// </returns>
        /// <example>
        /// 如果输入选中位数值的二进制为
        /// <code>1010111000</code>
        /// ,则可以得到如下 <see cref="long"/> 二进制数值得到的list
        /// <para>1000000000
        /// <para>10000000
        /// <para>100000
        /// <para>10000
        /// <para>1000</para>
        /// </para>
        /// </para>
        /// </para>
        /// </para>
        /// </example>
        public List<long> GetValidBinList()
        {
            if (this._binListCache != null)
            {
                return this._binListCache;
            }
            var lastBin = this._binObject;
            var binList = new List<long>();
            while ((lastBin|0)!=0)
            {
                var now = lastBin & (-lastBin);
                binList.Add(now);
                lastBin -= now;
            }

            this._binListCache = binList;
            return binList;
        }

        /// <summary>
        /// <see cref="GetValidBinList"/> 的转换为 <see cref="Array"/> 版本
        /// <para>具体用法请参阅 <seealso cref="GetValidBinList"/></para>
        /// </summary>
        /// <returns>
        /// 一个 <see cref="long"/> 组成的 <see cref="Array"/>
        /// <para>每个数值为在二进制上仅有一个位是1的 <see cref="long"/> 数值，表示被选中位</para>
        /// </returns>
        /// <example>
        /// 如果输入选中位数值的二进制为
        /// <code>1010111000</code>
        /// ,则可以得到如下 <see cref="long"/> 二进制数值得到的list
        /// <para>1000000000
        /// <para>10000000
        /// <para>100000
        /// <para>10000
        /// <para>1000</para>
        /// </para>
        /// </para>
        /// </para>
        /// </para>
        /// </example>
        public long[] GetValidBinArray()
        {
            return this.GetValidBinList().ToArray();
        }

        /// <summary>
        /// 标准构造函数，在外部请以 <see cref="BinLikeClassSelector.CreateBinLikeClassSelectorUnit"/> 调用
        /// </summary>
        /// <param name="binObject">以二进制表明被选中位的数字，可以以十进制输入</param>
        /// <remarks>如对该类派生以自定义构造方法，请注意对成员 <see cref="_binObject"/> 初始化</remarks>
        internal BinLikeClassSelectorUnit(long binObject)
        {
            this._binObject = binObject;
        }

        /// <inheritdoc cref="GetBin"/>
        public static implicit operator long(BinLikeClassSelectorUnit data)
        {
            return data.GetBin();
        }

        /// <inheritdoc cref="GetValidBinList"/>
        public static implicit operator List<long>(BinLikeClassSelectorUnit data)
        {
            return data.GetValidBinList();
        }

        /// <inheritdoc cref="GetValidBinArray"/>
        public static implicit operator long[](BinLikeClassSelectorUnit data)
        {
            return data.GetValidBinArray();
        }


#pragma warning disable CS0419 // cref 特性中有不明确的引用

#pragma warning disable CS1734 // XML 注释中有 paramref 标记，但是没有该名称的参数
        /// <summary>
        /// 用于方法 <see cref="BinLikeClassSelectorUnit.MatchDo"/> 中参数 <paramref name="executer"/> 的
        /// <see cref="Delegate"/> 约定
        /// <para>要求无输出，会提供该实例化对象本身作为参数 <paramref name="mine"/> 传递</para>
        /// </summary>
        /// <param name="mine">
        /// 被调用的 <see cref="BinLikeClassSelectorUnit.MatchDo"/> 所属的 <see
        /// cref="BinLikeClassSelectorUnit"/> 实例对象
        /// </param>
        [Obsolete("可使用action替代的情况下不建议使用此委托及其对应方法，已提供使用了action类型参数的重载")]
#pragma warning restore CS1734 // XML 注释中有 paramref 标记，但是没有该名称的参数
#pragma warning restore CS0419 // cref 特性中有不明确的引用
        public delegate void Executor(BinLikeClassSelectorUnit mine);

        /// <summary>
        /// 如果 <paramref name="matchRuleCode"/> 中的二进制选中位在被选中位中均有对应（or关系），则执行 <paramref
        /// name="executor"/> 委托
        /// <para>对应关系判断规则详见 <seealso cref="Match"/></para>
        /// </summary>
        /// <param name="matchRuleCode">以二进制数值表示期望的选中位</param>
        /// <param name="executor">详见 <seealso cref="Executor"/></param>
        /// <returns>被调用方法所属对象自身，用于linq编程风格</returns>
        /// <remarks>此函数支持linq编程风格，每次调用返回自身，可以继续调用</remarks>
        [Obsolete("可使用action替代的情况下不建议使用此委托及其对应方法，已提供使用了action类型参数的重载")]
        public virtual BinLikeClassSelectorUnit MatchDo(long matchRuleCode,Executor executor)
        {
            if (this.Match(matchRuleCode))
            {
                executor(this);
            }

            return this;
        }

        /// <summary>
        /// 如果 <paramref name="matchRuleCode"/> 中的二进制选中位在被选中位中均有对应（or关系），则执行 <paramref
        /// name="executor"/> 委托
        /// <para>对应关系判断规则详见 <seealso cref="Match"/></para>
        /// </summary>
        /// <param name="matchRuleCode">以二进制数值表示期望的选中位</param>
        /// <param name="executor">一个<see cref="Action{T}"/>对象，接收一个<see cref="BinLikeClassSelectorUnit"/>自身指针作为参数</param>
        /// <returns>被调用方法所属对象自身，用于linq编程风格</returns>
        /// <remarks>此函数支持linq编程风格，每次调用返回自身，可以继续调用</remarks>
        public virtual BinLikeClassSelectorUnit MatchDo(long matchRuleCode, Action<BinLikeClassSelectorUnit> executor)
        {
            if (this.Match(matchRuleCode))
            {
                executor(this);
            }

            return this;
        }
        /// <summary>
        /// 如果 <paramref name="matchRuleCode"/> 中的二进制选中位在被选中位中均有对应（or关系），则返回 <see cerf="true"/>
        /// <para>
        /// <list type="table">
        /// <listheader>
        /// <term>被选中位</term>
        /// <description>输入参数 <paramref name="matchRuleCode"/> 及输出</description>
        /// </listheader>
        /// <item>
        /// <term>1010</term>
        /// <description>1000 √</description>
        /// </item>
        /// <item>
        /// <term>110010</term>
        /// <description>10010 √</description>
        /// </item>
        /// <item>
        /// <term>100010</term>
        /// <description>100 ×</description>
        /// </item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="matchRuleCode"></param>
        /// <returns>判断结果</returns>
        public virtual bool Match(long matchRuleCode)
        {
            if ((matchRuleCode | this._binObject) != 0)
            {
                return true;
            }

            return false;
        }


    }
}
