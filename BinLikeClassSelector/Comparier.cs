using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEkit.BinLikeClassSelectors
{
    public partial class BinLikeClassSelectorUnit : IEquatable<BinLikeClassSelectorUnit>
    {
        /// <summary>
        ///     转为<see cref="string" />
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetBin().ToString();
        }

        ///// <inheritdoc cref="Object.Equals(object?)" />
        //public override bool Equals(object? obj)
        //{
        //    if (obj == null) return false;
        //    if (obj is BinLikeClassSelectorUnit unit) return GetBin().Equals(unit.GetBin());
        //    if (obj is long unitLong) return GetBin().Equals(unitLong);
        //    return false;
        //}

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode")]
        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), _binObject);
        }

        /// <inheritdoc />
        public bool Equals(BinLikeClassSelectorUnit other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return _binObject == other._binObject;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is long unitLong) return _binObject == unitLong;
            return obj.GetType() == this.GetType() && Equals((BinLikeClassSelectorUnit) obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(BinLikeClassSelectorUnit left, BinLikeClassSelectorUnit right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(BinLikeClassSelectorUnit left, BinLikeClassSelectorUnit right)
        {
            return !Equals(left, right);
        }
    }
}
