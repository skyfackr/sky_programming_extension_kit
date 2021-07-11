using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        /// <inheritdoc />
        public void Add(SemaphoreUnit item)
        {
            AssertNotDisposed();
            m_units.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            AssertNotDisposed();
            m_units.Clear();
        }

        /// <inheritdoc />
        public bool Contains(SemaphoreUnit item)
        {
            AssertNotDisposed();
            return m_units.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(SemaphoreUnit[] array, int arrayIndex)
        {
            AssertNotDisposed();
            m_units.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(SemaphoreUnit item)
        {
            AssertNotDisposed();
            return m_units.Remove(item);
        }

        /// <inheritdoc />
        public int Count => m_units.Count;

        /// <inheritdoc />
        public bool IsReadOnly => m_units.IsReadOnly;

        /// <inheritdoc />
        public int IndexOf(SemaphoreUnit item)
        {
            AssertNotDisposed();
            return m_units.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, SemaphoreUnit item)
        {
            AssertNotDisposed();
            m_units.Insert(index, item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            AssertNotDisposed();
            m_units.RemoveAt(index);
        }

        /// <inheritdoc />
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