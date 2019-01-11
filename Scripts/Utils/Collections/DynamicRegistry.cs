using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utils.Collections
{
    public class DynamicRegistry<VALUE, KEY> where VALUE : IRegistryEntry<KEY>
    {
        /// <summary>
        /// All the values in the registry.
        /// </summary>
        private readonly Dictionary<ushort, VALUE> dictionary;
        /// <summary>
        /// All the values in the registry.
        /// </summary>
        private readonly List<VALUE> values = new List<VALUE>();

        /// <summary>
        /// Creates a new empty dynamic registry.
        /// </summary>
        public DynamicRegistry()
        {
            dictionary = new Dictionary<ushort, VALUE>();
        }
        /// <summary>
        /// Creates a new dynamic registry and populates the new registry with an existing static registry.
        /// </summary>
        /// <param name="staticRegistry">The static registry to add to this dynamic registry.</param>
        public DynamicRegistry(Registry<VALUE, KEY> staticRegistry)
        {
            dictionary = new Dictionary<ushort, VALUE>();
            VALUE[] values = staticRegistry.GetValues();
            for (int i = 0; i < values.Length; i++)
                Add(values[i]);
        }

        /// <summary>
        /// Clears all the values from this dynamic registry.
        /// </summary>
        public void Clear()
        {
            dictionary.Clear();
            values.Clear();
        }

        /// <summary>
        /// Adds a value to the registry.
        /// </summary>
        /// <param name="value">The value to add.</param>
        private void Add(VALUE value)
        {
            if (values.Contains(value))
            {
                Debug.LogWarning("Tried to add a value that already exists in this dynamic registry!");
            }
            else
            {
                dictionary.Add((ushort)values.Count, value);
                values.Add(value);
            }
        }
        /// <summary>
        /// Gets the ID of a value.
        /// </summary>
        /// <param name="value">The value to get an ID for.</param>
        /// <returns>The ID of a value. The registry adds the value if it doesn't already contain the value.</returns>
        public ushort GetId(VALUE value)
        {
            int id = values.IndexOf(value);
            if (id == -1)
            {
                values.Add(value);
                return (ushort)(values.Count - 1);
            }
            return (ushort)id;
        }
        /// <summary>
        /// Gets a value from an ID.
        /// </summary>
        /// <param name="id">The ID of the value to get.</param>
        /// <returns>A value with a specific ID.</returns>
        public VALUE GetValue(ushort id)
        {
            if (id >= values.Count || id < 0)
                return default(VALUE);
            return values[id];
        }


        /// <summary>
        /// Clears the registry then imports registry key value pairs from an external file.
        /// </summary>
        /// <param name="file">The file to import from.</param>
        /// <param name="keyImporter">A method to convert the file stream into a key.</param>
        /// <param name="staticRegistry">The static registry that is used to convert keys from the file into values.</param>
        public void Import(FileInfo file, Func<BinaryReader, KEY> keyImporter, Registry<VALUE, KEY> staticRegistry)
        {
            if (file.Exists)
            {
                BinaryReader reader = new BinaryReader(file.OpenRead());
                Import(reader, keyImporter, staticRegistry);
                reader.Close();
            }
            else
                Debug.LogWarning("Failed to import dynamic registry because the source file doesn't exist!");
        }
        /// <summary>
        /// Clears the registry then imports registry key value pairs from an external file.
        /// </summary>
        /// <param name="reader">The stream to import from.</param>
        /// <param name="keyImporter">A method to convert the file stream into a key.</param>
        /// <param name="staticRegistry">The static registry that is used to convert keys from the file into values.</param>
        public void Import(BinaryReader reader, Func<BinaryReader, KEY> keyImporter, Registry<VALUE, KEY> staticRegistry)
        {
            //Clear the registry.
            dictionary.Clear();
            values.Clear();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                VALUE value;
                if (staticRegistry.TryGetValue(keyImporter(reader), out value))
                    Add(value);
            }
        }
        /// <summary>
        /// Exports all the keys in this registry.
        /// </summary>
        /// <param name="file">The file to export to.</param>
        /// <param name="keyExporter">A method to export keys into the file stream.</param>
        public void Export(FileInfo file, Action<BinaryWriter, KEY> keyExporter)
        {
            if (file.Directory.Exists)
            {
                BinaryWriter writer = new BinaryWriter(file.Create());
                Export(writer, keyExporter);
                writer.Close();
            }
            else
                Debug.LogWarning("Failed to export dynamic registry because the destination file directory doesn't exist!");
        }
        /// <summary>
        /// Exports all the keys in this registry.
        /// </summary>
        /// <param name="writer">The stream to export to.</param>
        /// <param name="keyExporter">A method to export keys into the file stream.</param>
        public void Export(BinaryWriter writer, Action<BinaryWriter, KEY> keyExporter)
        {
            writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
                keyExporter(writer, values[i].GetRegistryName());
        }
    }
}
