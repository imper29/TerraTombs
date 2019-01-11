using System.Collections.Generic;
using UnityEngine;

namespace Maps.Rendering
{
    /// <summary>
    /// Does the rendering for a map.
    /// </summary>
    public class MapRenderer2D
    {
        /// <summary>
        /// The map to render.
        /// </summary>
        private readonly Map2D map;
        /// <summary>
        /// The regions currently rendered.
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
        private readonly List<IRenderOperation> renderOperations;
        /// <summary>
        /// Is this renderer enabled?
        /// </summary>
        private bool enabled;


        /// <summary>
        /// Creates a disabled map renderer.
        /// </summary>
        /// <param name="map">The map this map renderer will render.</param>
        public MapRenderer2D(Map2D map)
        {
            this.map = map;
            renderedRegionsList = new List<Region2D>();
            renderedRegionsDictionary = new Dictionary<RegionPosition2D, Region2D>();
            renderOperations = new List<IRenderOperation>();
        }


        /// <summary>
        /// Enables this renderer.
        /// </summary>
        public void Enable()
        {
            if (!enabled)
            {
                enabled = true;
                MapRenderingHandler2D.OnRendererEnabled(this);

                //Start listening to map events.
                map.OnRegionDestroyed += Map_OnRegionDestroyed;
                map.OnGroundPlaced += Map_OnGroundPlaced;
                map.OnGroundRemoved += Map_OnGroundRemoved;
                map.OnInteractablePlaced += Map_OnInteractablePlaced;
                map.OnInteractableRemoved += Map_OnInteractableRemoved;
            }
        }
        /// <summary>
        /// Disables this renderer.
        /// </summary>
        public void Disable()
        {
            if (enabled)
            {
                enabled = false;
                
                //Stop listening to map events.
                map.OnRegionDestroyed -= Map_OnRegionDestroyed;
                map.OnGroundPlaced -= Map_OnGroundPlaced;
                map.OnGroundRemoved -= Map_OnGroundRemoved;
                map.OnInteractablePlaced -= Map_OnInteractablePlaced;
                map.OnInteractableRemoved -= Map_OnInteractableRemoved;

                //Clear render operations.
                renderOperations.Clear();

                //Unrender all the regions.
                Region2D[] regions = renderedRegionsList.ToArray();
                for (int i = 0; i < regions.Length; i++)
                    UnrenderRegion(regions[i].GetPosition());

                //Make the map renderer be removed from the list of active map renderers after the regions have all been unrendered.
                renderOperations.Add(new OnMapRendererDisabled(this));
            }
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
        private void Map_OnGroundPlaced(Region2D region, TilePosition2D globalTilePosition, TileGround2D tile)
        {
            //If the region is currently rendered, render the tile.
            if (renderedRegionsList.Contains(region))
                lock (renderOperations)
                    renderOperations.Add(new RenderGround(tile, globalTilePosition, region));
        }
        /// <summary>
        /// Called when a ground tile is removed.
        /// </summary>
        /// <param name="region">The region that contains the tile.</param>
        /// <param name="globalTilePosition">The position the tile was removed from.</param>
        /// <param name="tile">The tile that was removed.</param>
        private void Map_OnGroundRemoved(Region2D region, TilePosition2D globalTilePosition, TileGround2D tile)
        {
            //If the region is currently rendered, unrender the tile.
            if (renderedRegionsList.Contains(region))
                lock (renderOperations)
                    renderOperations.Add(new UnrenderGround(tile, globalTilePosition, region));
        }
        /// <summary>
        /// Called when an interactable tile is placed.
        /// </summary>
        /// <param name="region">The region that contains the tile.</param>
        /// <param name="globalTilePosition">The position the tile was removed from.</param>
        /// <param name="tile">The tile that was removed.</param>
        private void Map_OnInteractablePlaced(Region2D region, TilePosition2D globalTilePosition, TileInteractable2D tile)
        {
            //If the region is currently rendered, render the tile.
            if (renderedRegionsList.Contains(region))
                lock (renderOperations)
                    renderOperations.Add(new RenderInteractable(tile, globalTilePosition, region));
        }
        /// <summary>
        /// Called when an interactable tile is removed.
        /// </summary>
        /// <param name="region">The region that contains the tile.</param>
        /// <param name="globalTilePosition">The position the tile was removed from.</param>
        /// <param name="tile">The tile that was removed.</param>
        private void Map_OnInteractableRemoved(Region2D region, TilePosition2D globalTilePosition, TileInteractable2D tile)
        {
            //If the region is currently rendered, unrender the tile.
            if (renderedRegionsList.Contains(region))
                lock (renderOperations)
                    renderOperations.Add(new UnrenderInteractable(tile, globalTilePosition, region));
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
                    TileStack[,] tiles = r.GetTileStacks();
                    lock (renderOperations)
                    {
                        for (int x = 0; x < RegionPosition2D.REGION_SIZE; x++)
                            for (int z = 0; z < RegionPosition2D.REGION_SIZE; z++)
                            {
                                TilePosition2D tp = regionPosition.GetGlobalTilePosition(x, z);
                                //Render the tiles.
                                if (tiles[x, z].ground != null)
                                    renderOperations.Add(new RenderGround(tiles[x, z].ground, tp, r));
                                if (tiles[x, z].interactable != null)
                                    renderOperations.Add(new RenderInteractable(tiles[x, z].interactable, tp, r));
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
                TileStack[,] tiles = r.GetTileStacks();
                lock (renderOperations)
                {
                    for (int x = 0; x < RegionPosition2D.REGION_SIZE; x++)
                        for (int z = 0; z < RegionPosition2D.REGION_SIZE; z++)
                        {
                            TilePosition2D tp = regionPosition.GetGlobalTilePosition(x, z);
                            //Unrender the tiles.
                            if (tiles[x, z].ground != null)
                                renderOperations.Add(new UnrenderGround(tiles[x, z].ground, tp, r));
                            if (tiles[x, z].interactable != null)
                                renderOperations.Add(new UnrenderInteractable(tiles[x, z].interactable, tp, r));
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
            IRenderOperation[] operations;
            lock (renderOperations)
            {
                operations = renderOperations.ToArray();
                renderOperations.Clear();
            }
            for (int i = 0; i < operations.Length; i++)
                operations[i].Process();
        }
        /// <summary>
        /// Adds a render operation to the list of render operations to do when ProcessRenderOperations is called.
        /// Note that it will not add the operation if the region is not rendered.
        /// </summary>
        /// <param name="operation">What type of render operation to do.</param>
        /// <param name="globalTilePosition">The position of tile to operate on.</param>
        /// <param name="tile">The tile to operate on.</param>
        public void RequestRenderOperation(IRenderOperation operation)
        {
            if (enabled)
            {
                RegionPosition2D regionPosition = operation.GetGlobalTilePosition().GetParentRegionPosition();
                Region2D region;
                if (renderedRegionsDictionary.TryGetValue(regionPosition, out region))
                    renderOperations.Add(operation);
            }
        }

        /// <summary>
        /// A struct that is added to the render operations to disable the map renderer.
        /// </summary>
        private struct OnMapRendererDisabled : IRenderOperation
        {
            private readonly MapRenderer2D mapRenderer;

            public OnMapRendererDisabled(MapRenderer2D mapRenderer)
            {
                this.mapRenderer = mapRenderer;
            }

            public TilePosition2D GetGlobalTilePosition()
            {
                return new TilePosition2D();
            }
            public void Process()
            {
                MapRenderingHandler2D.OnRendererDisabled(mapRenderer);
            }
        }
    }

    public interface IRenderOperation
    {
        TilePosition2D GetGlobalTilePosition();
        void Process();
    }
    /// <summary>
    /// A render operation to render interactable tiles.
    /// </summary>
    public struct RenderInteractable : IRenderOperation
    {
        private readonly TileInteractable2D tile;
        private readonly TilePosition2D globalTilePosition;
        private readonly Region2D region;

        public RenderInteractable(TileInteractable2D tile, TilePosition2D globalTilePosition, Region2D region)
        {
            this.tile = tile;
            this.globalTilePosition = globalTilePosition;
            this.region = region;
        }

        public void Process()
        {
            region.RenderInteractable(globalTilePosition, tile);
        }
        public TilePosition2D GetGlobalTilePosition()
        {
            return globalTilePosition;
        }
    }
    /// <summary>
    /// A render operation to unrender interactable tiles.
    /// </summary>
    public struct UnrenderInteractable : IRenderOperation
    {
        private readonly TileInteractable2D tile;
        private readonly TilePosition2D globalTilePosition;
        private readonly Region2D region;

        public UnrenderInteractable(TileInteractable2D tile, TilePosition2D globalTilePosition, Region2D region)
        {
            this.tile = tile;
            this.globalTilePosition = globalTilePosition;
            this.region = region;
        }

        public void Process()
        {
            region.UnrenderInteractable(globalTilePosition, tile);
        }
        public TilePosition2D GetGlobalTilePosition()
        {
            return globalTilePosition;
        }
    }
    /// <summary>
    /// A render operation to render ground tiles.
    /// </summary>
    public struct RenderGround : IRenderOperation
    {
        private readonly TileGround2D tile;
        private readonly TilePosition2D globalTilePosition;
        private readonly Region2D region;

        public RenderGround(TileGround2D tile, TilePosition2D globalTilePosition, Region2D region)
        {
            this.tile = tile;
            this.globalTilePosition = globalTilePosition;
            this.region = region;
        }

        public void Process()
        {
            region.RenderGround(globalTilePosition, tile);
        }
        public TilePosition2D GetGlobalTilePosition()
        {
            return globalTilePosition;
        }
    }
    /// <summary>
    /// A render operation to unrender ground tiles.
    /// </summary>
    public struct UnrenderGround : IRenderOperation
    {
        private readonly TileGround2D tile;
        private readonly TilePosition2D globalTilePosition;
        private readonly Region2D region;

        public UnrenderGround(TileGround2D tile, TilePosition2D globalTilePosition, Region2D region)
        {
            this.tile = tile;
            this.globalTilePosition = globalTilePosition;
            this.region = region;
        }

        public void Process()
        {
            region.UnrenderGround(globalTilePosition, tile);
        }
        public TilePosition2D GetGlobalTilePosition()
        {
            return globalTilePosition;
        }
    }
}
