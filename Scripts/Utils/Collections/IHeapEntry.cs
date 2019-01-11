using System;

namespace Utils.Collections
{
    /// <summary>
    /// An item that can be atted to a heap.
    /// </summary>
    /// <typeparam name="T">The type of heap it can be added into.</typeparam>
    public interface IHeapEntry<T> : IComparable<T>
    {
        int HeapIndex
        {
            get;
            set;
        }
    }
}