using System.Collections;
using System.Collections.Generic;
using PostSharp.Patterns.Threading;
using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.Utils
{
    [ReaderWriterSynchronized]
    internal class SemaphoreUnitList : IList<SemaphoreUnit>
    {
        private readonly IList<SemaphoreUnit> m_list;


        public SemaphoreUnitList(IList<SemaphoreUnit> list)
        {
            m_list = list;
        }

        [Reader]
        public IEnumerator<SemaphoreUnit> GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        [Writer]
        public void Add(SemaphoreUnit item)
        {
            m_list.Add(item);
        }

        [Writer]
        public void Clear()
        {
            m_list.Clear();
        }

        [Reader]
        public bool Contains(SemaphoreUnit item)
        {
            return m_list.Contains(item);
        }

        [Reader]
        public void CopyTo(SemaphoreUnit[] array, int arrayIndex)
        {
            m_list.CopyTo(array, arrayIndex);
        }

        [Writer]
        public bool Remove(SemaphoreUnit item)
        {
            return m_list.Remove(item);
        }


        public int Count => GetCount();

        public bool IsReadOnly => GetReadOnly();

        [Reader]
        public int IndexOf(SemaphoreUnit item)
        {
            return m_list.IndexOf(item);
        }

        [Writer]
        public void Insert(int index, SemaphoreUnit item)
        {
            m_list.Insert(index, item);
        }

        [Writer]
        public void RemoveAt(int index)
        {
            m_list.RemoveAt(index);
        }

        public SemaphoreUnit this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Reader]
        private int GetCount()
        {
            return m_list.Count;
        }

        [Reader]
        private bool GetReadOnly()
        {
            return m_list.IsReadOnly;
        }

        [Reader]
        private SemaphoreUnit Get(int index)
        {
            return m_list[index];
        }

        [Writer]
        private void Set(int index, SemaphoreUnit v)
        {
            m_list[index] = v;
        }
    }
}