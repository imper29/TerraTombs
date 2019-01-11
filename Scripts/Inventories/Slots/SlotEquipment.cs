using Characters;
using Inventories.Items;

namespace Inventories.Slots
{
    /// <summary>
    /// A slot that can contain equipment items.
    /// When items are inserted or extracted from this slot, the item's OnEquipped/OnUnequipped methods are called.
    /// </summary>
    /// <typeparam name="T">The type of equipment item this slot can contain.</typeparam>
    public class SlotEquipment<T> : Slot<T> where T : ItemEquipment
    {
        /// <summary>
        /// The character that owns this equipment slot.
        /// </summary>
        private readonly ICharacter2D character;


        /// <summary>
        /// Creates a new equipment slot.
        /// </summary>
        /// <param name="character">The character that owns this equipment slot.</param>
        public SlotEquipment(ICharacter2D character)
        {
            this.character = character;
        }


        /// <summary>
        /// Inserts items into this slot.
        /// </summary>
        /// <param name="insertItem">The item to insert.</param>
        /// <param name="insertAmount">The number of items to insert. (Must be greater than zero)</param>
        /// <returns>How many items were actually inserted into this slot.</returns>
        public override int InsertItems(Item insertItem, int insertAmount)
        {
            insertAmount = base.InsertItems(insertItem, insertAmount);
            if (insertAmount > 0)
                this.item.OnEquipped(stackSize, insertAmount, character);
            return insertAmount;
        }
        /// <summary>
        /// Extracts items from this slot.
        /// </summary>
        /// <param name="extractItem">The item to extract.</param>
        /// <param name="extractAmount">How many items to extract from this slot. (Must be greater than zero)</param>
        /// <returns>How many items were actually extracted from this slot.</returns>
        public override int ExtractItems(Item extractItem, int extractAmount)
        {
            extractAmount = base.ExtractItems(extractItem, extractAmount);
            if (extractAmount > 0)
                this.item.OnUnequipped(stackSize, extractAmount, character);
            return extractAmount;
        }
    }
}
