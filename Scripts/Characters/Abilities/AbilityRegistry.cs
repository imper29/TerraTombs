using UnityEngine;
using Utils.Collections;

namespace Characters
{
    /// <summary>
    /// Creates the ability registry.
    /// </summary>
    public class AbilityRegistry : MonoBehaviour
    {
        /// <summary>
        /// The registry containing all the abilities in the game.
        /// </summary>
        public static readonly Registry<Ability, string> REGISTRY_ABILITY = new Registry<Ability, string>();

        /// <summary>
        /// Populates the registries.
        /// </summary>
        private void Awake()
        {
            REGISTRY_ABILITY.Clear();
            Ability[] abilities = Resources.LoadAll<Ability>("Abilities");
            for (int i = 0; i < abilities.Length; i++)
                REGISTRY_ABILITY.Add(abilities[i]);
        }
    }
}
