using Utils.Collections;

namespace Characters
{
    public class CharacterPrefab2D : IRegistryEntry<string>
    {
        /// <summary>
        /// The registry name for this character prefab.
        /// </summary>
        private readonly string registryName;
        /// <summary>
        /// The set of abilities for this character prefab.
        /// </summary>
        private readonly AbilitySet abilitySet;

        /// <summary>
        /// Gets the registry name for this character prefab.
        /// </summary>
        /// <returns>The registry name for this character prefab.</returns>
        public string GetRegistryName()
        {
            return registryName;
        }
        /// <summary>
        /// Gets the ability set for this character prefab.
        /// </summary>
        /// <returns>The ability set for this character prefab.</returns>
        public AbilitySet GetAbilitySet()
        {
            return abilitySet;
        }
    }
}
