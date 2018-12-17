using System.Collections.Generic;
using UnityEngine;

namespace Maps
{
    /// <summary>
    /// A map made up of regions.
    /// </summary>
    public class Map2D
    {
        /// <summary>
        /// A delegate used for various region related events.
        /// </summary>
        /// <param name="region"></param>
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
        public delegate void OnTileDelegate<T>(Region2D region, TilePosition2D globalTilePosition, T tile) where T : ITile2D;
        /// <summary>
        /// Called when a ground tile is placed in a region that this map contains.
        /// Technically an event. It cannot actually be an event because it is called from Region2D.
        /// </summary>
        public OnTileDelegate<ITileGround2D> OnGroundPlaced;
        /// <summary>
        /// Called when a ground tile is removed from a region that this map contains.
        /// Technically an event. It cannot actually be an event because it is called from Region2D.
        /// </summary>
        public OnTileDelegate<ITileGround2D> OnGroundRemoved;
        /// <summary>
        /// Called when an interactable tile is removed from a region that this map contains.
        /// Technically an event. It cannot actually be an event because it is called from Region2D.
        /// </summary>
        public OnTileDelegate<ITileInteractable2D> OnInteractablePlaced;
        /// <summary>
        /// Called when an interactable is removed from a region that this map contains.
        /// Technically an event. It cannot actually be an event because it is called from Region2D.
        /// </summary>
        public OnTileDelegate<ITileInteractable2D> OnInteractableRemoved;


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
    }
}
