using System.Collections;
using System.Collections.Generic;
using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        public IEnumerator<SemaphoreUnit> GetEnumerator()
        {
            AssertNotDisposed();
            return m_units.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            AssertNotDisposed();
            return GetEnumerator();
        }
    }
}