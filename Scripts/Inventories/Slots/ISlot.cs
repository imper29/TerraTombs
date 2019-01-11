using Inventories.Items;

namespace Inventories.Slots
{
    /// <summary>
    /// A slot that can contain a stack of items.
    /// </summary>
    public interface ISlot
    {
        /// <summary>
        /// Gets the item in this slot.
        /// </summary>
        /// <returns>The item in this slot.</returns>
        Item GetItem();
        /// <summary>
        /// Gets the number of items in this slot.
        /// </summary>
        /// <returns>The number of items in this slot.</returns>
        int GetStackSize();

        /// <summary>
        /// Inserts items into this slot.
        /// </summary>
        /// <param name="insertItem">The item to insert.</param>
        /// <param name="insertAmount">The number of items to insert. (Must be greater than zero)</param>
        /// <returns>How many items were actually inserted into this slot.</returns>
        int InsertItems(Item insertItem, int insertAmount);
        /// <summary>
        /// Extracts items from this slot.
        /// </summary>
        /// <param name="extractItem">The item to extract.</param>
        /// <param name="extractAmount">How many items to extract from this slot. (Must be greater than zero)</param>
        /// <returns>How many items were actually extracted from this slot.</returns>
        int ExtractItems(Item extractItem, int extractAmount);
    }
}
