using Inventories.Items;
using Inventories.Slots;
using System.Collections.Generic;

namespace Inventories
{
    /// <summary>
    /// A generic collection of items.
    /// </summary>
    /// <typeparam name="T">The type of item this inventory can contain.</typeparam>
    public class InventoryGeneric<T> : IInventory where T : Item
    {
        /// <summary>
        /// The maximum number of slots this inventory can contain.
        /// </summary>
        private readonly int maxSize;
        /// <summary>
        /// The slots in this inventory.
        /// </summary>
        private readonly List<Slot<T>> slots;


        /// <summary>
        /// Creates an empty inventory.
        /// </summary>
        /// <param name="maxSize">The maximum number of slots this inventory can contain.</param>
        public InventoryGeneric(int maxSize)
        {
            this.maxSize = maxSize;
            slots = new List<Slot<T>>(maxSize);
        }


        /// <summary>
        /// Finds the maximum number of slots the inventory can contain.
        /// </summary>
        /// <returns>The maximum number of slots the inventory can contain.</returns>
        public int GetMaxSize()
        {
            return maxSize;
        }
        /// <summary>
        /// Finds the number of slots this inventory contains.
        /// </summary>
        /// <returns>The number of slots this inventory contains.</returns>
        public int GetSize()
        {
            return slots.Count;
        }
        /// <summary>
        /// Finds how many of an item is in this inventory.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>How many of an item is in this inventory.</returns>
        public int GetItemCount(Item item)
        {
            int count = 0;
            for (int i = 0; i < slots.Count; i++)
                if (slots[i].GetItem() == item)
                    count += slots[i].GetStackSize();
            return count;
        }
        /// <summary>
        /// Finds a slot based on the slot's index.
        /// </summary>
        /// <param name="slotIndex">The index of the slot to find.</param>
        /// <returns>The slot with an index of slotIndex.</returns>
        public ISlot GetSlot(int slotIndex)
        {
            return slots[slotIndex];
        }

        /// <summary>
        /// Inserts items into this inventory.
        /// </summary>
        /// <param name="insertItem">The item to insert.</param>
        /// <param name="insertAmount">The number of items to insert. (Must be greater than zero)</param>
        /// <returns>How many items were actually inserted into this inventory.</returns>
        public int InsertItems(Item insertItem, int insertAmount)
        {
            int originalInsertAmount = insertAmount;
            //Loop through all the slots and try to insert the items.
            for (int i = 0; i < slots.Count && insertAmount > 0; i++)
                insertAmount -= slots[i].InsertItems(insertItem, insertAmount);
            //If not all the items fit inside the existing slots and there is room for a new slot, create a new slot.
            while (insertAmount > 0 && slots.Count < maxSize)
            {
                Slot<T> slot = new Slot<T>();
                insertAmount -= slot.InsertItems(insertItem, insertAmount);
                slots.Add(slot);
            }
            return originalInsertAmount - insertAmount;
        }
        /// <summary>
        /// Extracts items from this inventory.
        /// </summary>
        /// <param name="extractItem">The item to extract.</param>
        /// <param name="extractAmount">How many items to extract from this inventory. (Must be greater than zero)</param>
        /// <returns>How many items were actually extracted from this inventory.</returns>
        public int ExtractItems(Item extractItem, int extractAmount)
        {
            int originalExtractAmount = extractAmount;
            //Loop through all the slots and try to extract the items.
            for (int i = slots.Count - 1; i >= 0 && extractAmount > 0; i++)
            {
                int e = slots[i].ExtractItems(extractItem, extractAmount);
                //The slot had items removed.
                if (e > 0)
                {
                    //Remove the number of items that were extracted from the number of items to extract.
                    extractAmount -= e;
                    //If the slot is empty, destroy the slot.
                    if (slots[i].GetStackSize() == 0)
                    {
                        slots.RemoveAt(i);
                        i--;
                    }
                }
            }
            return originalExtractAmount - extractAmount;
        }
    }
}
