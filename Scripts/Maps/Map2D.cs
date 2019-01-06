using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utils.Collections;

namespace Maps
{
    /// <summary>
    /// A map made up of regions.
    /// </summary>
    public class Map2D
    {
        /// <summary>
        /// A delegate used for various directory related events.
        /// </summary>
        /// <param name="directory">A directory.</param>
        public delegate void OnDirectoryDelegate(DirectoryInfo directory);
        /// <summary>
        /// Called when the map is imported from a directory.
        /// </summary>
        public event OnDirectoryDelegate OnMapImported;
        /// <summary>
        /// Called when the map is exported to a directory.
        /// </summary>
        public event OnDirectoryDelegate OnMapExported;

        /// <summary>
        /// A delegate used for various region related events.
        /// </summary>
        /// <param name="region">A region.</param>
        public delegate void OnRegionDelegate(Region2D region);
        /// <summary>
        /// Called when a region is created.
        /// </summary>
        public event OnRegionDelegate OnRegionCreated;
        /// <summary>
        /// Called when a region is destroyed.
        /// </summary>
        public event OnRegionDelegate OnRegionDestroyed;

        /// <summary>
        /// A delegate used for various tile related events.
        /// </summary>
        /// <typeparam name="T">The tile type.</typeparam>
        /// <param name="region">The region that contains the tile.</param>
        /// <param name="globalTilePosition">The position of the tile.</param>
        /// <param name="tile">The tile.</param>
        public delegate void OnTileDelegate<T>(Region2D region, TilePosition2D globalTilePosition, T tile) where T : Tile2D;
        /// <summary>
        /// Called when a ground tile is placed in a region that this map contains.
        /// Technically an event. It cannot actually be an event because it is called from Region2D.
        /// </summary>
        public OnTileDelegate<TileGround2D> OnGroundPlaced;
        /// <summary>
        /// Called when a ground tile is removed from a region that this map contains.
        /// Technically an event. It cannot actually be an event because it is called from Region2D.
        /// </summary>
        public OnTileDelegate<TileGround2D> OnGroundRemoved;
        /// <summary>
        /// Called when an interactable tile is removed from a region that this map contains.
        /// Technically an event. It cannot actually be an event because it is called from Region2D.
        /// </summary>
        public OnTileDelegate<TileInteractable2D> OnInteractablePlaced;
        /// <summary>
        /// Called when an interactable is removed from a region that this map contains.
        /// Technically an event. It cannot actually be an event because it is called from Region2D.
        /// </summary>
        public OnTileDelegate<TileInteractable2D> OnInteractableRemoved;


        /// <summary>
        /// A list containing all this map's regions.
        /// </summary>
        private readonly List<Region2D> regionsList;
        /// <summary>
        /// A dictionary containing all this map's regions.
        /// </summary>
        private readonly Dictionary<RegionPosition2D, Region2D> regionsDictionary;


        /// <summary>
        /// Creates a new map.
        /// </summary>
        public Map2D()
        {
            regionsList = new List<Region2D>();
            regionsDictionary = new Dictionary<RegionPosition2D, Region2D>();
        }


        /// <summary>
        /// Gets an array of all the regions in the map.
        /// </summary>
        /// <returns>An array of all the regions in the map.</returns>
        public Region2D[] GetRegions()
        {
            return regionsList.ToArray();
        }
        /// <summary>
        /// Gets the region at regionPosition.
        /// </summary>
        /// <param name="regionPosition">The position of the region to get.</param>
        /// <returns>The region at regionPosition.</returns>
        public Region2D GetRegion(RegionPosition2D regionPosition)
        {
            Region2D r;
            regionsDictionary.TryGetValue(regionPosition, out r);
            return r;
        }
        /// <summary>
        /// Gets or creates a region at regionPosition.
        /// </summary>
        /// <param name="regionPosition">The position of the region to get.</param>
        /// <returns>The region at regionPosition.</returns>
        public Region2D GetOrCreateRegion(RegionPosition2D regionPosition)
        {
            Region2D r;
            if (regionsDictionary.TryGetValue(regionPosition, out r))
                return r;
            else
                return CreateRegion(regionPosition);
        }
        /// <summary>
        /// Creates a region at regionPosition.
        /// </summary>
        /// <param name="regionPosition">The position of the region to create.</param>
        /// <returns>A new region positioned at regionPosition.</returns>
        private Region2D CreateRegion(RegionPosition2D regionPosition)
        {
            //Create the region.
            Region2D r = new Region2D(this, regionPosition);
            //Add the region to the regions list/dictionary.
            regionsDictionary[regionPosition] = r;
            regionsList.Add(r);
            //Call the OnRegionCreated event.
            OnRegionCreated?.Invoke(r);

            return r;
        }
        /// <summary>
        /// Destroys the region at regionPosition.
        /// </summary>
        /// <param name="regionPosition">The position of the region to destroy.</param>
        public void DestroyRegion(RegionPosition2D regionPosition)
        {
            Region2D r;
            if (regionsDictionary.TryGetValue(regionPosition, out r))
                DestroyRegion(r);
            else
                Debug.LogWarning("Tried to destroy a region that does not exist!");
        }
        /// <summary>
        /// Destroys a region.
        /// </summary>
        /// <param name="region">The region to destroy.</param>
        public void DestroyRegion(Region2D region)
        {
            //If the region could be removed from the list...
            if (regionsList.Remove(region))
            {
                //Remove the region from the dictionary.
                regionsDictionary.Remove(region.GetPosition());
                //Call the OnRegionDestroyed event.
                OnRegionDestroyed?.Invoke(region);
            }
            else
                Debug.LogWarning("Tried to destroy a region that is not part of this map!");
        }
        /// <summary>
        /// Determines if a region exists.
        /// </summary>
        /// <param name="regionPosition">The position of the region to check.</param>
        /// <returns>True if the region exists. False otherwise.</returns>
        public bool HasRegion(RegionPosition2D regionPosition)
        {
            return regionsDictionary.ContainsKey(regionPosition);
        }
        /// <summary>
        /// Determines how many regions exist.
        /// </summary>
        /// <returns>The number of regions in this map.</returns>
        public int GetRegionCount()
        {
            return regionsList.Count;
        }


        /// <summary>
        /// Finds the tile stack.
        /// </summary>
        /// <param name="globalTilePosition">The position of the tile stack.</param>
        /// <param name="tileStack">The tile stack that was found.</param>
        /// <returns>True if the tile stack was found.</returns>
        public bool TryGetTileStack(TilePosition2D globalTilePosition, out TileStack tileStack)
        {
            RegionPosition2D regionPosition = globalTilePosition.GetParentRegionPosition();
            Region2D region;
            if (regionsDictionary.TryGetValue(regionPosition, out region))
            {
                TilePosition2D localTilePosition = regionPosition.GetLocalTilePosition(globalTilePosition);
                tileStack = region.GetTileStack(localTilePosition);
                return true;
            }
            tileStack = new TileStack();
            return false;
        }
        /// <summary>
        /// Finds the tile stack.
        /// </summary>
        /// <param name="globalTilePosition">The position of the tile stack.</param>
        /// <param name="region">The region the tile was found inside.</param>
        /// <param name="tileStack">The tile stack that was found.</param>
        /// <returns>True if the tile stack was found.</returns>
        public bool TryGetTileStack(TilePosition2D globalTilePosition, out Region2D region, out TileStack tileStack)
        {
            RegionPosition2D regionPosition = globalTilePosition.GetParentRegionPosition();
            if (regionsDictionary.TryGetValue(regionPosition, out region))
            {
                TilePosition2D localTilePosition = regionPosition.GetLocalTilePosition(globalTilePosition);
                tileStack = region.GetTileStack(localTilePosition);
                return true;
            }
            tileStack = new TileStack();
            return false;
        }


        /// <summary>
        /// Places a ground tile at globalTilePosition.
        /// If a ground tile already exists in that position, the ground tile is first removed.
        /// </summary>
        /// <param name="globalTilePosition">Where to place the ground tile.</param>
        /// <param name="tile">The ground tile to place.</param>
        public void PlaceGround(TilePosition2D globalTilePosition, TileGround2D tile)
        {
            RegionPosition2D regionPosition = globalTilePosition.GetParentRegionPosition();
            Region2D region = GetOrCreateRegion(regionPosition);
            region.PlaceGround(regionPosition.GetLocalTilePosition(globalTilePosition), tile);
        }
        /// <summary>
        /// Finds the ground tile at globalTilePosition.
        /// </summary>
        /// <param name="globalTilePosition">The position of the ground tile to fine.</param>
        /// <returns>The ground tile at globalTilePosition.</returns>
        public TileGround2D GetGround(TilePosition2D globalTilePosition)
        {
            TileStack tileStack;
            if (TryGetTileStack(globalTilePosition, out tileStack))
                return tileStack.ground;
            return null;
        }
        /// <summary>
        /// Removes the ground tile at globalTilePosition.
        /// </summary>
        /// <param name="globalTilePosition">The position of the ground tile to remove.</param>
        public void RemoveGround(TilePosition2D globalTilePosition)
        {
            RegionPosition2D regionPosition = globalTilePosition.GetParentRegionPosition();
            Region2D region;
            if (regionsDictionary.TryGetValue(regionPosition, out region))
                region.RemoveGround(regionPosition.GetLocalTilePosition(globalTilePosition));
        }


        /// <summary>
        /// Places an interactable tile at globalTilePosition.
        /// If an interactable tile already exists in that position, the interactable tile is first removed.
        /// </summary>
        /// <param name="globalTilePosition">Where to place the interactable tile.</param>
        /// <param name="tile">The interactable tile to place.</param>
        public void PlaceInteractable(TilePosition2D globalTilePosition, TileInteractable2D tile)
        {
            RegionPosition2D regionPosition = globalTilePosition.GetParentRegionPosition();
            Region2D region = GetOrCreateRegion(regionPosition);
            region.PlaceInteractable(regionPosition.GetLocalTilePosition(globalTilePosition), tile);
        }
        /// <summary>
        /// Finds the interactable tile at globalTilePosition.
        /// </summary>
        /// <param name="globalTilePosition">The position of the interactable tile to fine.</param>
        /// <returns>The interactable tile at globalTilePosition.</returns>
        public TileInteractable2D GetInteractable(TilePosition2D globalTilePosition)
        {
            TileStack tileStack;
            if (TryGetTileStack(globalTilePosition, out tileStack))
                return tileStack.interactable;
            return null;
        }
        /// <summary>
        /// Removes the interactable tile at globalTilePosition.
        /// </summary>
        /// <param name="globalTilePosition">The position of the interactable tile to remove.</param>
        public void RemoveInteractable(TilePosition2D globalTilePosition)
        {
            RegionPosition2D regionPosition = globalTilePosition.GetParentRegionPosition();
            Region2D region;
            if (regionsDictionary.TryGetValue(regionPosition, out region))
                region.RemoveInteractable(regionPosition.GetLocalTilePosition(globalTilePosition));
        }


        /// <summary>
        /// Gets the tile entity at globalTilePosition.
        /// If no tile entity exists at the position, null is returned.
        /// </summary>
        /// <param name="globalTilePosition">The position of the tile entity to find.</param>
        /// <returns>The tile entity at globalTilePosition</returns>
        public ITileEntity2D GetTileEntity(TilePosition2D globalTilePosition)
        {
            RegionPosition2D regionPosition = globalTilePosition.GetParentRegionPosition();
            Region2D region;
            if (regionsDictionary.TryGetValue(regionPosition, out region))
                return region.GetTileEntity(regionPosition.GetLocalTilePosition(globalTilePosition));
            return null;
        }
        /// <summary>
        /// Gets the tile entity at globalTilePosition.
        /// </summary>
        /// <param name="globalTilePosition">The position of the tile entity to find.</param>
        /// <param name="tileEntity">The tile entity at globalTilePosition.</param>
        /// <returns>True if a tile entity was found.</returns>
        public bool TryGetTileEntity(TilePosition2D globalTilePosition, out ITileEntity2D tileEntity)
        {
            RegionPosition2D regionPosition = globalTilePosition.GetParentRegionPosition();
            Region2D region;
            if (regionsDictionary.TryGetValue(regionPosition, out region))
                return region.TryGetTileEntity(regionPosition.GetLocalTilePosition(globalTilePosition), out tileEntity);

            tileEntity = default(ITileEntity2D);
            return false;
        }


        /// <summary>
        /// Exports this map.
        /// </summary>
        /// <param name="directory">The directory to export into.</param>
        /// <param name="groundRegistry">The dynamic registry that holds all the ground tiles.</param>
        /// <param name="interactableRegistry">The dynamic registry that holds all the intertactable tiles.</param>
        public void Export(DirectoryInfo directory, DynamicRegistry<TileGround2D, string> groundRegistry, DynamicRegistry<TileInteractable2D, string> interactableRegistry)
        {
            if (directory.Exists)
            {
                //Call the on exported event.
                OnMapExported?.Invoke(directory);

                //Create the regions sub directory.
                DirectoryInfo regionsFolder = directory.CreateSubdirectory("regions");

                //Export all the regions.
                Region2D[] regionsArray = regionsList.ToArray();
                for (int i = 0; i < regionsArray.Length; i++)
                {
                    Region2D region = regionsArray[i];
                    RegionPosition2D regionPosition = region.GetPosition();

                    FileInfo fileInfo = new FileInfo(regionsFolder.FullName + "/" + regionPosition.x + "," + regionPosition.z + ".region");
                    BinaryWriter writer = new BinaryWriter(fileInfo.Create());
                    region.Export(writer, groundRegistry, interactableRegistry);
                    writer.Close();
                }
            }
            else
                Debug.LogWarning("Tried to export a map into a non-existent directory!");
        }
        /// <summary>
        /// Imports this map.
        /// </summary>
        /// <param name="directory">The directory to import from.</param>
        /// <param name="groundRegistry">The dynamic registry that holds all the ground tiles.</param>
        /// <param name="interactableRegistry">The dynamic registry that holds all the intertactable tiles.</param>
        public void Import(DirectoryInfo directory, DynamicRegistry<TileGround2D, string> groundRegistry, DynamicRegistry<TileInteractable2D, string> interactableRegistry)
        {
            if (directory.Exists)
            {
                //Call the on imported event.
                OnMapImported?.Invoke(directory);

                //Destroy all the old regions.
                Region2D[] regionsArray = regionsList.ToArray();
                for (int i = 0; i < regionsArray.Length; i++)
                    DestroyRegion(regionsArray[i]);

                //Get the regions sub directory.
                DirectoryInfo regionsFolder = new DirectoryInfo(directory.FullName + "/regions");

                //Load all the new regions if the regions folder exists..
                if (regionsFolder.Exists)
                {
                    FileInfo[] files = regionsFolder.GetFiles("*.region", SearchOption.TopDirectoryOnly);
                    for (int i = 0; i < files.Length; i++)
                    {
                        string[] fileName = Path.GetFileNameWithoutExtension(files[i].Name).Split(',');
                        if (fileName.Length == 2)
                        {
                            int x;
                            if (int.TryParse(fileName[0], out x))
                            {
                                int z;
                                if (int.TryParse(fileName[1], out z))
                                {
                                    Region2D region = CreateRegion(new RegionPosition2D(x, z));
                                    BinaryReader reader = new BinaryReader(files[i].OpenRead());
                                    region.Import(reader, groundRegistry, interactableRegistry);
                                    reader.Close();
                                }
                            }
                        }
                    }
                }
            }
            else
                Debug.LogWarning("Tried to import a map from a non-existent directory!");
        }
    }
}
