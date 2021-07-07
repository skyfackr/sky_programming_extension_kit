using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        public void Add(SemaphoreUnit item)
        {
            AssertNotDisposed();
            m_units.Add(item);
        }

        public void Clear()
        {
            AssertNotDisposed();
            m_units.Clear();
        }

        public bool Contains(SemaphoreUnit item)
        {
            AssertNotDisposed();
            return m_units.Contains(item);
        }

        public void CopyTo(SemaphoreUnit[] array, int arrayIndex)
        {
            AssertNotDisposed();
            m_units.CopyTo(array, arrayIndex);
        }

        public bool Remove(SemaphoreUnit item)
        {
            AssertNotDisposed();
            return m_units.Remove(item);
        }

        public int Count => m_units.Count;

        public bool IsReadOnly => m_units.IsReadOnly;

        public int IndexOf(SemaphoreUnit item)
        {
            AssertNotDisposed();
            return m_units.IndexOf(item);
        }

        public void Insert(int index, SemaphoreUnit item)
        {
            AssertNotDisposed();
            m_units.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            AssertNotDisposed();
            m_units.RemoveAt(index);
        }

        public SemaphoreUnit this[int index]
        {
            get
            {
                AssertNotDisposed();
                return m_units[index];
            }
            set
            {
                AssertNotDisposed();
                m_units[index] = value;
            }
        }
    }
}