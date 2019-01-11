using System.Collections.Generic;
using UnityEngine;

namespace Utils.Collections
{
    /// <summary>
    /// Like a dictionary except values determine their keys instead of the key being an arbitrary value.
    /// </summary>
    /// <typeparam name="VALUE">The value type.</typeparam>
    /// <typeparam name="KEY">The key type.</typeparam>
    public class Registry<VALUE, KEY> where VALUE : IRegistryEntry<KEY>
    {
        /// <summary>
        /// All the values in the registry.
        /// </summary>
        private readonly Dictionary<KEY, VALUE> valuesDict;
        /// <summary>
        /// All the values in the registry.
        /// </summary>
        private readonly List<VALUE> values;


        /// <summary>
        /// Creates an empty registry.
        /// </summary>
        public Registry()
        {
            valuesDict = new Dictionary<KEY, VALUE>();
            values = new List<VALUE>();
        }


        /// <summary>
        /// Adds a value to the registry.
        /// </summary>
        /// <param name="value">The value to add to the registry.</param>
        public void Add(VALUE value)
        {
            KEY key = value.GetRegistryName();
            if (valuesDict.ContainsKey(key))
                Debug.LogWarning("Tried to add duplicate key:" + key.ToString());
            else
            {
                valuesDict.Add(key, value);
                values.Add(value);
            }
        }
        /// <summary>
        /// Removes a value from the registry.
        /// </summary>
        /// <param name="key">The key that points to the value to remove.</param>
        public void Remove(KEY key)
        {
            VALUE value;
            if (valuesDict.TryGetValue(key, out value))
            {
                valuesDict.Remove(key);
                values.Remove(value);
            }
        }
        /// <summary>
        /// Removes a value from the registry.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        public void Remove(VALUE value)
        {
            if (valuesDict.Remove(value.GetRegistryName()))
                values.Remove(value);
        }

        /// <summary>
        /// Removes all the values from the registry.
        /// </summary>
        public void Clear()
        {
            values.Clear();
            valuesDict.Clear();
        }

        /// <summary>
        /// Gets a value from the registry.
        /// </summary>
        /// <param name="key">The key that points to the desired value.</param>
        /// <returns>The value that the key points to.</returns>
        public VALUE GetValue(KEY key)
        {
            VALUE e;
            valuesDict.TryGetValue(key, out e);
            return e;
        }
        /// <summary>
        /// Gets a value from the registry.
        /// </summary>
        /// <param name="key">The key that points to the desired value.</param>
        /// <param name="value">The value that the key points to.</param>
        /// <returns>True if the value was found.</returns>
        public bool TryGetValue(KEY key, out VALUE value)
        {
            return valuesDict.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets all the values in the registry.
        /// </summary>
        /// <returns>All the values in the registry.</returns>
        public VALUE[] GetValues()
        {
            return values.ToArray();
        }
    }
}
