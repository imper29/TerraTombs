using UnityEngine;
using Utils.Collections;

namespace Characters
{
    public abstract class Ability : ScriptableObject, IRegistryEntry<string>
    {
        /// <summary>
        /// Gets the registry name for this ability.
        /// </summary>
        /// <returns>The registry name for this ability.</returns>
        public string GetRegistryName()
        {
            return name;
        }

        /// <summary>
        /// Uses this ability on a character.
        /// </summary>
        /// <param name="source">The character that used the ability.</param>
        /// <param name="target">The character the ability was used on.</param>
        public abstract void UseAbility(ICharacter2D source, ICharacter2D target);
    }
}
