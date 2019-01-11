namespace Utils.Collections
{
    /// <summary>
    /// Essentially an array that sorts items based on their priority.
    /// It is a very efficient way of sorting an array.
    /// </summary>
    /// <typeparam name="T">The type of item to sort.</typeparam>
    public class Heap<T> where T : IHeapEntry<T>
    {
        readonly T[] items;
        int currentItemCount;

        /// <summary>
        /// Determines how many items are in this heap.
        /// </summary>
        public int Count
        {
            get
            {
                return currentItemCount;
            }
        }


        /// <summary>
        /// Creates a new heap.
        /// </summary>
        /// <param name="maxHeapSize">The most number of items that can be in the heap.</param>
        public Heap(int maxHeapSize)
        {
            items = new T[maxHeapSize];
        }


        /// <summary>
        /// Adds an item to the heap.
        /// </summary>
        /// <param name="item">The item to add to the heap.</param>
        public void Add(T item)
        {
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            SortUp(item);
            currentItemCount++;
        }
        /// <summary>
        /// Removes the first item in the heap. The first item has the highest priority.
        /// </summary>
        /// <returns>The item with the highest priority in the heap.</returns>
        public T RemoveFirst()
        {
            T firstItem = items[0];
            currentItemCount--;

            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return firstItem;

        }
        /// <summary>
        /// Updates the position of the item.
        /// This method must be called if the item's priority changes.
        /// </summary>
        /// <param name="item">The item to update.</param>
        public void UpdateItem(T item)
        {
            SortUp(item);
            SortDown(item);
        }

        /// <summary>
        /// Determines if an item is in the heap.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>True if the heap contains the item. False otherwise.</returns>
        public bool Contains(T item)
        {
            return Equals(item, items[item.HeapIndex]);
        }

        /// <summary>
        /// Shifts an item up through the heap until it is in its proper position.
        /// </summary>
        /// <param name="item">The item to sort.</param>
        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;
            while (true)
            {
                T parentItem = items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                    Swap(item, parentItem);
                else
                    break;

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }
        /// <summary>
        /// Shifts an item down through the heap until it is in its proper position.
        /// </summary>
        /// <param name="item">The item to sort.</param>
        private void SortDown(T item)
        {
            while (true)
            {
                int childIndexLeft = item.HeapIndex * 2 + 1;
                int childIndexRight = item.HeapIndex * 2 + 2;
                int swapIndex = 0;

                if (childIndexLeft < currentItemCount)
                {
                    swapIndex = childIndexLeft;
                    if (childIndexRight < currentItemCount)
                    {
                        if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(items[swapIndex]) < 0)
                        Swap(item, items[swapIndex]);
                    else
                        return;
                }
                else
                    return;
            }
        }
        /// <summary>
        /// Swaps the position of two items.
        /// </summary>
        /// <param name="itemA">The first item to swap.</param>
        /// <param name="itemB">The second item to swap.</param>
        private void Swap(T itemA, T itemB)
        {
            items[itemA.HeapIndex] = itemB;
            items[itemB.HeapIndex] = itemA;

            int temp = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = temp;
        }
    }
}
