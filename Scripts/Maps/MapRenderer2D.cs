using System.Collections.Generic;
using UnityEngine;

namespace Maps
{
    /// <summary>
    /// Does the rendering of a map.
    /// </summary>
    public class MapRenderer2D
    {
        /// <summary>
        /// The map to render.
        /// </summary>
        private readonly Map2D map;
        /// <summary>
        /// The positions of currently rendered regions.
        /// </summary>
        private readonly List<Region2D> renderedRegionsList;
        /// <summary>
        /// The regions currently rendered.
        /// </summary>
        private readonly Dictionary<RegionPosition2D, Region2D> renderedRegionsDictionary;

        /// <summary>
        /// Used to ensure that interacting with maps is thread-safe.
        /// The operations ProcessRenderOperations will process.
        /// Every time a tile needs to be rendered, an operation is added to this list.
        /// When the operations are processed, the list is cleared.
        /// </summary>
        private readonly List<RenderOperation> renderOperations;


        /// <summary>
        /// Creates a new map renderer.
        /// </summary>
        /// <param name="map">The map this map renderer will render.</param>
        public MapRenderer2D(Map2D map)
        {
            this.map = map;
            renderedRegionsList = new List<Region2D>();
            renderedRegionsDictionary = new Dictionary<RegionPosition2D, Region2D>();

            renderOperations = new List<RenderOperation>();

            //Subscribe to a bunch of events.
            map.OnRegionDestroyed += Map_OnRegionDestroyed;
            map.OnGroundPlaced += Map_OnGroundPlaced;
            map.OnGroundRemoved += Map_OnGroundRemoved;
            map.OnInteractablePlaced += Map_OnInteractablePlaced;
            map.OnInteractableRemoved += Map_OnInteractableRemoved;
        }


        /// <summary>
        /// Called when a region in the map is destroyed.
        /// </summary>
        /// <param name="region">The region that was destroyed.</param>
        private void Map_OnRegionDestroyed(Region2D region)
        {
            //If the region that was destroyed was also rendered, unrender it.
            if (renderedRegionsDictionary.ContainsValue(region))
                UnrenderRegion(region.GetPosition());
        }
        /// <summary>
        /// Called when a ground tile is placed.
        /// </summary>
        /// <param name="region">The region that contains the tile.</param>
        /// <param name="globalTilePosition">The position the tile was removed from.</param>
        /// <param name="tile">The tile that was removed.</param>
        private void Map_OnGroundPlaced(Region2D region, TilePosition2D globalTilePosition, ITileGround2D tile)
        {
            //If the region is currently rendered, render the tile.
            if (renderedRegionsList.Contains(region))
                lock (renderOperations)
                    renderOperations.Add(new RenderOperation(RenderOperationType.Render, globalTilePosition, region, tile));
        }
        /// <summary>
        /// Called when a ground tile is removed.
        /// </summary>
        /// <param name="region">The region that contains the tile.</param>
        /// <param name="globalTilePosition">The position the tile was removed from.</param>
        /// <param name="tile">The tile that was removed.</param>
        private void Map_OnGroundRemoved(Region2D region, TilePosition2D globalTilePosition, ITileGround2D tile)
        {
            //If the region is currently rendered, unrender the tile.
            if (renderedRegionsList.Contains(region))
                lock (renderOperations)
                    renderOperations.Add(new RenderOperation(RenderOperationType.Unrender, globalTilePosition, region, tile));
        }
        /// <summary>
        /// Called when an interactable tile is placed.
        /// </summary>
        /// <param name="region">The region that contains the tile.</param>
        /// <param name="globalTilePosition">The position the tile was removed from.</param>
        /// <param name="tile">The tile that was removed.</param>
        private void Map_OnInteractablePlaced(Region2D region, TilePosition2D globalTilePosition, ITileInteractable2D tile)
        {
            //If the region is currently rendered, render the tile.
            if (renderedRegionsList.Contains(region))
                lock (renderOperations)
                    renderOperations.Add(new RenderOperation(RenderOperationType.Render, globalTilePosition, region, tile));
        }
        /// <summary>
        /// Called when an interactable tile is removed.
        /// </summary>
        /// <param name="region">The region that contains the tile.</param>
        /// <param name="globalTilePosition">The position the tile was removed from.</param>
        /// <param name="tile">The tile that was removed.</param>
        private void Map_OnInteractableRemoved(Region2D region, TilePosition2D globalTilePosition, ITileInteractable2D tile)
        {
            //If the region is currently rendered, unrender the tile.
            if (renderedRegionsList.Contains(region))
                lock (renderOperations)
                    renderOperations.Add(new RenderOperation(RenderOperationType.Unrender, globalTilePosition, region, tile));
        }


        /// <summary>
        /// Renders a region.
        /// </summary>
        /// <param name="regionPosition">The position of the region to render.</param>
        public void RenderRegion(RegionPosition2D regionPosition)
        {
            if (renderedRegionsDictionary.ContainsKey(regionPosition))
                Debug.LogWarning("Tried to render a region that is already rendered!");
            else
            {
                Region2D r = map.GetRegion(regionPosition);
                if (r != null)
                {
                    //Add the region to the rendered regions dictionary / list.
                    renderedRegionsDictionary.Add(regionPosition, r);
                    renderedRegionsList.Add(r);

                    //Loop through all the tiles in the region.
                    Region2D.TileStack[,] tiles = r.GetTileStacks();
                    lock (renderOperations)
                    {
                        for (int x = 0; x < RegionPosition2D.REGION_SIZE; x++)
                            for (int z = 0; z < RegionPosition2D.REGION_SIZE; z++)
                            {
                                TilePosition2D tp = regionPosition.GetGlobalTilePosition(x, z);
                                //Render the tiles.
                                if (tiles[x, z].ground != null)
                                    renderOperations.Add(new RenderOperation(RenderOperationType.Render, tp, r, tiles[x, z].ground));
                                if (tiles[x, z].interactable != null)
                                    renderOperations.Add(new RenderOperation(RenderOperationType.Render, tp, r, tiles[x, z].interactable));
                            }
                    }
                }
                else
                    Debug.LogWarning("Tried to render a region that doesn't exist!");
            }
        }
        /// <summary>
        /// Unrenders a region.
        /// </summary>
        /// <param name="regionPosition">The position of the region to unrender.</param>
        public void UnrenderRegion(RegionPosition2D regionPosition)
        {
            //If the region is currently rendered by this renderer...
            Region2D r;
            if (renderedRegionsDictionary.TryGetValue(regionPosition, out r))
            {
                //Get the region and remove it from the rendered regions dictionary / list.
                renderedRegionsDictionary.Remove(regionPosition);
                renderedRegionsList.Remove(r);

                //Loop through all the tiles in the region.
                Region2D.TileStack[,] tiles = r.GetTileStacks();
                lock (renderOperations)
                {
                    for (int x = 0; x < RegionPosition2D.REGION_SIZE; x++)
                        for (int z = 0; z < RegionPosition2D.REGION_SIZE; z++)
                        {
                            TilePosition2D tp = regionPosition.GetGlobalTilePosition(x, z);
                            //Unrender the tiles.
                            if (tiles[x, z].ground != null)
                                renderOperations.Add(new RenderOperation(RenderOperationType.Unrender, tp, r, tiles[x, z].ground));
                            if (tiles[x, z].interactable != null)
                                renderOperations.Add(new RenderOperation(RenderOperationType.Unrender, tp, r, tiles[x, z].interactable));
                        }
                }
            }
            else
                Debug.LogWarning("Tried to unrender a region that isn't currently rendered!");
        }

        /// <summary>
        /// Processes all the requested render operations. This method should be called from the main rendering thread.
        /// </summary>
        public void ProcessRenderOperations()
        {
            RenderOperation[] operations;
            lock (renderOperations)
            {
                operations = renderOperations.ToArray();
                renderOperations.Clear();
            }
            for (int i = 0; i < operations.Length; i++)
                operations[i].Process();
        }

        /// <summary>
        /// A struct for holding information about which tiles need to be rendered/unrendered.
        /// The information needs to be temporarily saved for running in a seperate thread.
        /// </summary>
        private struct RenderOperation
        {
            private readonly RenderOperationType type;
            private readonly TilePosition2D globalTilePosition;
            private readonly Region2D region;
            private readonly ITile2D tile;


            public RenderOperation(RenderOperationType type, TilePosition2D globalTilePosition, Region2D region, ITile2D tile)
            {
                this.type = type;
                this.globalTilePosition = globalTilePosition;
                this.region = region;
                this.tile = tile;
            }


            public void Process()
            {
                if (type == RenderOperationType.Render)
                    tile.OnRendered(region, globalTilePosition);
                else
                    tile.OnUnrendered(region, globalTilePosition);
            }
        }
        /// <summary>
        /// Used to determine if a render operation should render or unrender a tile.
        /// </summary>
        private enum RenderOperationType : byte
        {
            Render,
            Unrender
        }
    }
}
