using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;

namespace ModLoading
{
    public class ModLoader : MonoBehaviour
    {
        /// <summary>
        /// A singleton to the modloader.
        /// </summary>
        private ModLoader INSTANCE;
        /// <summary>
        /// The version of the game.
        /// </summary>
        public static readonly ModVersion GAME_VERSION = new ModVersion(2019, 1, 15);

        /// <summary>
        /// All the currently loaded mods.
        /// </summary>
        private static readonly List<ModData> LOADED_MODS = new List<ModData>();
        
        private void Awake()
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Gets a mod by its ID.
        /// </summary>
        /// <param name="modID">The id of the mod to get.</param>
        /// <returns>The mod with a specific ID. If no mod with that id exists, null is returned.</returns>
        public static ModBase GetMod(string modID)
        {
            for (int i = 0; i < LOADED_MODS.Count; i++)
                if (LOADED_MODS[i].info.id == modID)
                    return LOADED_MODS[i].mod;
            return null;
        }
        /// <summary>
        /// Gets a mod's info by its ID.
        /// </summary>
        /// <param name="modID">The id of the mod to get.</param>
        /// <returns>The mod's info with a specific ID. If no mod with that id exists, default(ModInfo) is returned.</returns>
        public static ModInfo GetModInfo(string modID)
        {
            for (int i = 0; i < LOADED_MODS.Count; i++)
                if (LOADED_MODS[i].info.id == modID)
                    return LOADED_MODS[i].info;
            return default(ModInfo);
        }
        /// <summary>
        /// Finds all the mods in a folder.
        /// </summary>
        /// <param name="modInfoList">All the mods in a folder.</param>
        /// <param name="directory">The directory to search.</param>
        public static void FindAllMods(List<ModInfo> modInfoList, DirectoryInfo directory)
        {
            //Find all the mod dll files.
            FileInfo[] enabledFiles = directory.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
            FileInfo[] disabledFiles = directory.GetFiles("*.dll.disabled", SearchOption.TopDirectoryOnly);
            FileInfo[] modFiles = new FileInfo[enabledFiles.Length + disabledFiles.Length];
            enabledFiles.CopyTo(modFiles, 0);
            disabledFiles.CopyTo(modFiles, enabledFiles.Length);

            //Find all the modInfo.xml resources from the dll files.
            XmlSerializer modInfoSerializer = new XmlSerializer(typeof(ModInfo));
            for (int fileIndex = 0; fileIndex < modFiles.Length; fileIndex++)
            {
                Assembly assembly = Assembly.ReflectionOnlyLoadFrom(modFiles[fileIndex].FullName);
                Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".modInfo.xml");

                //Load the modInfo.
                if (stream != null)
                {
                    object info = modInfoSerializer.Deserialize(stream);
                    if (info != null)
                    {
                        //The mod info was loaded properly so add it to the mods list.
                        modInfoList.Add((ModInfo)info);
                    }
                    else
                    {
                        Debug.Log("Corrupt modInfo.xml resource!");
                    }
                    stream.Close();
                }
                else
                {
                    Debug.Log("No modInfo.xml resource found!");
                }
            }
        }
        /// <summary>
        /// Sorts a collection of mods based on the mod load order.
        /// </summary>
        /// <param name="modInfoList">The mods to sort.</param>
        /// <param name="modLoadOrder">The mod loading order.</param>
        public static void SortMods(List<ModInfo> modInfoList, params string[] modLoadOrder)
        {
            //Sort the mods.
            for (int o = 0; o < modLoadOrder.Length; o++)
                for (int m = o; m < modInfoList.Count; m++)
                {
                    ModInfo md = modInfoList[m];

                    if (md.id == modLoadOrder[o])
                    {
                        modInfoList.Remove(md);
                        modInfoList.Insert(0, md);
                        break;
                    }
                }
            modInfoList.Reverse();
        }
        /// <summary>
        /// Tries to load all the enabled mods in the mods folder based on a mod load order.
        /// </summary>
        internal static void LoadMods(params string[] modLoadOrder)
        {
            DirectoryInfo modsDirectory = new DirectoryInfo(@"C:\Users\marte\Desktop\Unity\TerraTombs\Mod");
            List<ModData> modDataList = new List<ModData>();
            FindMods(modDataList, modsDirectory);
            SortMods(modDataList, modLoadOrder);
            LoadMods(modDataList);
        }


        /// <summary>
        /// Finds all the enabled mods in a folder.
        /// </summary>
        /// <param name="modDataList">All the mods in a folder.</param>
        /// <param name="directory">The folder to search..</param>
        private static void FindMods(List<ModData> modDataList, DirectoryInfo directory)
        {
            //Find all the mod dll files.
            FileInfo[] modFiles = directory.GetFiles("*.dll", SearchOption.TopDirectoryOnly);

            //Find all the modInfo.xml resources from the dll files.
            XmlSerializer modInfoSerializer = new XmlSerializer(typeof(ModInfo));
            for (int fileIndex = 0; fileIndex < modFiles.Length; fileIndex++)
            {
                Assembly assembly = Assembly.ReflectionOnlyLoadFrom(modFiles[fileIndex].FullName);
                Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".modInfo.xml");

                //Load the modInfo.
                if (stream != null)
                {
                    object info = modInfoSerializer.Deserialize(stream);
                    if (info != null)
                    {
                        //The mod info was loaded properly so add it to the mods list.
                        modDataList.Add(new ModData(null, (ModInfo)info, modFiles[fileIndex]));
                    }
                    else
                    {
                        Debug.Log("Corrupt modInfo.xml resource!");
                    }
                    stream.Close();
                }
                else
                {
                    Debug.Log("No modInfo.xml resource found!");
                }
            }
        }
        /// <summary>
        /// Sorts a collection of mods based on the mod load order.
        /// </summary>
        /// <param name="modDataList">The mods to sort.</param>
        /// <param name="modLoadOrder">The mod loading order.</param>
        private static void SortMods(List<ModData> modDataList, params string[] modLoadOrder)
        {
            //Sort the mods.
            for (int o = 0; o < modLoadOrder.Length; o++)
                for (int m = o; m < modDataList.Count; m++)
                {
                    ModData md = modDataList[m];

                    if (md.info.id == modLoadOrder[o])
                    {
                        modDataList.Remove(md);
                        modDataList.Insert(0, md);
                        break;
                    }
                }
            modDataList.Reverse();
        }
        /// <summary>
        /// Loads a collection of mods.
        /// </summary>
        /// <param name="modDataList">The mods to load.</param>
        private static void LoadMods(List<ModData> modDataList)
        {
            //Load all the mods that can be loaded.
            for (int modDataIndex = 0; modDataIndex < modDataList.Count; modDataIndex++)
            {
                ModData modData = modDataList[modDataIndex];


                //Determine if the mod can be loaded.
                ModLoadError modLoadError = ModCanBeLoaded(modDataList[modDataIndex].info);

                //The mod could be loaded so load it.
                if (modLoadError == ModLoadError.None)
                {
                    Assembly assembly = Assembly.LoadFile(modDataList[modDataIndex].fileInfo.FullName);
                    foreach (Type t in assembly.GetTypes())
                        if (typeof(ModBase).IsAssignableFrom(t))
                        {
                            modData.mod = (ModBase)Activator.CreateInstance(t);
                            LOADED_MODS.Add(modData);
                            break;
                        }
                }
                //The mod couldn't be loaded so debug why it couldn't be loaded.
                else if (modLoadError == ModLoadError.DependencyNotLoaded)
                {
                    string dep = modData.info.dependencies[0].ToString();
                    for (int i = 1; i < modData.info.dependencies.Length; i++)
                        dep += ", " + modData.info.dependencies[i];
                    Debug.Log("[" + modData.info.id + "] Missing Mod Dependency - " + dep);
                }
                else if (modLoadError == ModLoadError.BadDependecyVersion)
                {
                    string dep = modData.info.dependencies[0].ToString();
                    for (int i = 1; i < modData.info.dependencies.Length; i++)
                        dep += modData.info.dependencies[i];
                    Debug.Log("[" + modData.info.id + "] Missing Mod Dependency - " + dep);
                }
                else if (modLoadError == ModLoadError.BadTargetVersion)
                {
                    Debug.Log("[" + modData.info.id + "] Designed for " + modData.info.minTargetVersion + "-" + modData.info.maxTargetVersion);
                }
            }

            //Pre init the mods.
            for (int i = 0; i < LOADED_MODS.Count; i++)
                LOADED_MODS[i].mod.PreInit();
            //Init the mods.
            for (int i = 0; i < LOADED_MODS.Count; i++)
                LOADED_MODS[i].mod.Init();
            //Post init hte mods.
            for (int i = 0; i < LOADED_MODS.Count; i++)
                LOADED_MODS[i].mod.PostInit();
        }

        /// <summary>
        /// Checks to see if a mod can be loaded.
        /// </summary>
        /// <param name="modInfo">The mod info for the mod to load.</param>
        /// <returns>Why the mod cannot be loaded.</returns>
        private static ModLoadError ModCanBeLoaded(ModInfo modInfo)
        {
            //Check to ensure the version is correct.
            if (modInfo.maxTargetVersion < GAME_VERSION || modInfo.minTargetVersion > GAME_VERSION)
                return ModLoadError.BadTargetVersion;

            //Check to ensure all dependencies are loaded and are acceptable versions.
            ModDependency[] dependencies = modInfo.dependencies;
            for (int i = 0; i < dependencies.Length; i++)
            {
                ModDependency dep = dependencies[i];
                ModInfo depInfo = GetModInfo(dep.dependencyId);

                //Dependency mod not loaded!
                if (depInfo.Equals(default(ModInfo)))
                    return ModLoadError.DependencyNotLoaded;
                //Bad dependency mod version!
                else if (dep.maxDependencyVersion < depInfo.modVersion || dep.minDependencyVersion > depInfo.modVersion)
                    return ModLoadError.BadDependecyVersion;
            }

            //The mod can be loaded!
            return ModLoadError.None;
        }
        /// <summary>
        /// An enum to represent why a mod couldn't be loaded.
        /// </summary>
        private enum ModLoadError
        {
            None,
            BadTargetVersion,
            DependencyNotLoaded,
            BadDependecyVersion,
        }

        /// <summary>
        /// A struct to hold a mod, a mod's info, and a mod's file info.
        /// </summary>
        private struct ModData
        {
            public ModBase mod;
            public ModInfo info;
            public FileInfo fileInfo;


            public ModData(ModBase mod, ModInfo info, FileInfo fileInfo)
            {
                this.mod = mod;
                this.info = info;
                this.fileInfo = fileInfo;
            }
        }
    }
}
