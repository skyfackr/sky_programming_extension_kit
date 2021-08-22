using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEkit.WinFormVarData
{
    public sealed partial class DataBinder<T>
    {
        public override string ToString()
        {
            return IsDisposed ? "This object is disposed." : $"DataBinder:{Data}, Type:{typeof(T)},{(IsDataFromINotifyPropertyChangedSource?$" from:{DataSource}":" No source.") }";
        }

        public bool Equals(DataBinder<T> other)
        {
            
            if (other is null) return false;
            if (IsDisposed || other.IsDisposed) return false;
            return ReferenceEquals(this, other);

        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is DataBinder<T> other && Equals(other);
        }

        

        public static bool operator ==(DataBinder<T> left, DataBinder<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DataBinder<T> left, DataBinder<T> right)
        {
            return !Equals(left, right);
        }
    }
}
