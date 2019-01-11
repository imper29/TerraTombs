using System.Collections.Generic;
using System.IO;
using Utils.Collections;

namespace Characters
{
    public class AbilitySet : IRegistryEntry<string>
    {
        /// <summary>
        /// The registry name for this ability set.
        /// </summary>
        private string registryName;
        /// <summary>
        /// All the abilities this ability set provides.
        /// </summary>
        private HashSet<Ability> abilitiesHashSet;
        /// <summary>
        /// All the abilities this ability set provides.
        /// </summary>
        private Ability[] abilitiesArray;


        /// <summary>
        /// Creates a new ability set.
        /// </summary>
        /// <param name="registryName">The name of this ability set.</param>
        /// <param name="abilities">All the abilities this ability set will provide.</param>
        public AbilitySet(string registryName, params Ability[] abilities)
        {
            this.registryName = registryName;
            abilitiesHashSet = new HashSet<Ability>(abilities);
            abilitiesArray = abilities;
        }


        /// <summary>
        /// Gets the registry name for ability set.
        /// </summary>
        /// <returns>The registry name for this ability set.</returns>
        public string GetRegistryName()
        {
            return registryName;
        }

        /// <summary>
        /// Determines if this ability set contains a specific ability.
        /// </summary>
        /// <param name="ability">The ability to find.</param>
        /// <returns>True if the this ability set contains a specific ability.</returns>
        public bool HasAbility(Ability ability)
        {
            return abilitiesHashSet.Contains(ability);
        }
        /// <summary>
        /// Gets all the abilities in this ability set.
        /// </summary>
        /// <returns>All the abilities in this ability set.</returns>
        public Ability[] GetAbilities()
        {
            return abilitiesArray;
        }


        /// <summary>
        /// Exports this ability set.
        /// </summary>
        /// <param name="writer">The stream to export into.</param>
        public void Export(BinaryWriter writer)
        {
            //Export the number of abilities.
            writer.Write((uint)abilitiesArray.Length);
            //Export all the abilities.
            for (int i = 0; i < abilitiesArray.Length; i++)
                writer.Write(abilitiesArray[i].GetRegistryName());
        }
        /// <summary>
        /// Imports this ability set.
        /// </summary>
        /// <param name="reader">The stream to import from.</param>
        public void Import(BinaryReader reader)
        {
            //Import the number of abilities.
            uint count = reader.ReadUInt32();
            //Import all the abilities.
            List<Ability> abilities = new List<Ability>();
            for (uint i = 0; i < count; i++)
            {
                Ability a;
                if (AbilityRegistry.REGISTRY_ABILITY.TryGetValue(reader.ReadString(), out a))
                    abilities.Add(a);
            }

            //Set the abilities this set has to the imported abilities.
            abilitiesArray = abilities.ToArray();
            abilitiesHashSet = new HashSet<Ability>(abilities);
        }
    }
}
