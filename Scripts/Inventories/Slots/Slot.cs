using Inventories.Items;

namespace Inventories.Slots
{
    /// <summary>
    /// A generic slot that can contain a stack of items.
    /// </summary>
    /// <typeparam name="T">The type of item this slot can contain.</typeparam>
    public class Slot<T> : ISlot where T : Item
    {
        /// <summary>
        /// The item in this slot.
        /// </summary>
        protected T item;
        /// <summary>
        /// The number of items in this slot.
        /// </summary>
        protected int stackSize;

        /// <summary>
        /// Gets the item in this slot.
        /// </summary>
        /// <returns>The item in this slot.</returns>
        public T GetItemExact()
        {
            return item;
        }
        /// <summary>
        /// Gets the item in this slot.
        /// </summary>
        /// <returns>The item in this slot.</returns>
        public Item GetItem()
        {
            return item;
        }
        /// <summary>
        /// Gets the number of items in this slot.
        /// </summary>
        /// <returns>The number of items in this slot.</returns>
        public int GetStackSize()
        {
            return stackSize;
        }

        /// <summary>
        /// Inserts items into this slot.
        /// </summary>
        /// <param name="insertItem">The item to insert.</param>
        /// <param name="insertAmount">The number of items to insert. (Must be greater than zero)</param>
        /// <returns>How many items were actually inserted into this slot.</returns>
        public virtual int InsertItems(Item insertItem, int insertAmount)
        {
            //The slot doesn't contain an item.
            if (item == null)
            {
                //The new item can be put into this slot.
                if (insertItem is T)
                {
                    //Set the item in this slot to be the inserted item.
                    item = (T)insertItem;
                    //The maximum stack size of the slot.
                    int maxStackSize = item.GetMaxStackSize();
                    //Tried to insert more than the maxStackSize so insert _stackSize - maxStackSize items.
                    if (insertAmount > maxStackSize)
                    {
                        stackSize = maxStackSize;
                        return maxStackSize;
                    }
                    //Tried to insert less than the maxStackSize so insert all the items.
                    else
                    {
                        stackSize = insertAmount;
                        return insertAmount;
                    }
                }
                //The slot cannot contain the new item.
                return 0;
            }
            //The slot contains the same item so.
            else if (item == insertItem)
            {
                //The maximum stack size of the slot.
                int maxStackSize = item.GetMaxStackSize();
                //How many items can still fit in the slot.
                int availableSpace = maxStackSize - stackSize;
                //Tried to insert more items than could fit so insert all the items that can be inserted.
                if (insertAmount > availableSpace)
                {
                    stackSize = maxStackSize;
                    return availableSpace;
                }
                //Tried to insert less items than could fit so insert all the items.
                else
                {
                    stackSize += insertAmount;
                    return insertAmount;
                }
            }
            //The slot contains a different item so no items were inserted.
            return 0;
        }
        /// <summary>
        /// Extracts items from this slot.
        /// </summary>
        /// <param name="extractItem">The item to extract.</param>
        /// <param name="extractAmount">How many items to extract from this slot. (Must be greater than zero)</param>
        /// <returns>How many items were actually extracted from this slot.</returns>
        public virtual int ExtractItems(Item extractItem, int extractAmount)
        {
            //Tried to extract an item that this slot contains.
            if (item == extractItem)
            {
                //Tried to extract more items than what is in this slot, so extract all the items from this slot.
                if (extractAmount > stackSize)
                {
                    //Preserve the number of items that were in this slot.
                    int temp = stackSize;
                    //Remove the item from the slot.
                    stackSize = 0;
                    item = null;
                    //Return the number of items that were in this slot.
                    return temp;
                }
                //Tried to extract less items than what is in this slot, so extract all the requested items.
                else
                {
                    stackSize -= extractAmount;
                    return extractAmount;
                }
            }
            //Tried to extract an item that this slot doesn't contain so don't extract any.
            return 0;
        }
    }
}
