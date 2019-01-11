using UnityEngine;
using Utils.Collections;

namespace Inventories.Items
{
    [CreateAssetMenu]
    public class Item : ScriptableObject, IRegistryEntry<string>
    {
        [SerializeField]
        private string registryName;
        [SerializeField]
        private int maxStackSize;

        public string GetRegistryName()
        {
            return registryName;
        }
        public int GetMaxStackSize()
        {
            return maxStackSize;
        }
    }
}
