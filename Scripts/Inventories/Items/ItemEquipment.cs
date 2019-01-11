using Characters;
using UnityEngine;

namespace Inventories.Items
{
    [CreateAssetMenu]
    public abstract class ItemEquipment : Item
    {
        /// <summary>
        /// Called when an item is inserted into an equipment slot.
        /// </summary>
        /// <param name="totalStackSize">The total number of items that are in the equipment slot.</param>
        /// <param name="deltaStackSize">The number of items that were equipped.</param>
        /// <param name="character">The character that equipped this item.</param>
        public abstract void OnEquipped(int totalStackSize, int deltaStackSize, ICharacter2D character);
        /// <summary>
        /// Called when the item is removed from an equipment slot.
        /// </summary>
        /// <param name="totalStackSize">The total number of items that are in the equipment slot.</param>
        /// <param name="deltaStackSize">The number of items that were unequipped.</param>
        /// <param name="character">The character that unequipped this item.</param>
        public abstract void OnUnequipped(int totalStackSize, int deltaStackSize, ICharacter2D character);
    }
}
