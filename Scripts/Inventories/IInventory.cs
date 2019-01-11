using Inventories.Items;
using Inventories.Slots;

namespace Inventories
{
    public interface IInventory
    {
        /// <summary>
        /// Finds the maximum number of slots the inventory can contain.
        /// </summary>
        /// <returns>The maximum number of slots the inventory can contain.</returns>
        int GetMaxSize();
        /// <summary>
        /// Finds the number of slots this inventory contains.
        /// </summary>
        /// <returns>The number of slots this inventory contains.</returns>
        int GetSize();
        /// <summary>
        /// Finds how many of an item is in this inventory.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>How many of an item is in this inventory.</returns>
        int GetItemCount(Item item);
        /// <summary>
        /// Finds a slot based on the slot's index.
        /// </summary>
        /// <param name="slotIndex">The index of the slot to find.</param>
        /// <returns>The slot with an index of slotIndex.</returns>
        ISlot GetSlot(int slotIndex);

        /// <summary>
        /// Inserts items into this inventory.
        /// </summary>
        /// <param name="insertItem">The item to insert.</param>
        /// <param name="insertAmount">The number of items to insert. (Must be greater than zero)</param>
        /// <returns>How many items were actually inserted into this inventory.</returns>
        int InsertItems(Item insertItem, int insertAmount);
        /// <summary>
        /// Extracts items from this inventory.
        /// </summary>
        /// <param name="extractItem">The item to extract.</param>
        /// <param name="extractAmount">How many items to extract from this inventory. (Must be greater than zero)</param>
        /// <returns>How many items were actually extracted from this inventory.</returns>
        int ExtractItems(Item extractItem, int extractAmount);
    }
}
