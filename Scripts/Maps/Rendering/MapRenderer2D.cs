using System.Collections.Generic;
using Utils.Threading;

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
        /// A queue of all the rendering operations that need to be done.
        /// </summary>
        private readonly RequestQueue<IRequest<MapRenderer2D>, MapRenderer2D> renderRequests;
        /// <summary>
        /// The render data for the regions currently rendered.
        /// </summary>
        private readonly Dictionary<RegionPosition2D, RegionRenderer2D> regionRenderers;
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
            renderRequests = new RequestQueue<IRequest<MapRenderer2D>, MapRenderer2D>();
            regionRenderers = new Dictionary<RegionPosition2D, RegionRenderer2D>();
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
                map.OnRegionCreated += CreateRegionRenderer;
                map.OnRegionDestroyed += DestroyRegionRenderer;
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
                MapRenderingHandler2D.OnRendererDisabled(this);

                //Stop listening to map events.
                map.OnRegionCreated -= CreateRegionRenderer;
                map.OnRegionDestroyed -= DestroyRegionRenderer;
                map.OnGroundPlaced -= Map_OnGroundPlaced;
                map.OnGroundRemoved -= Map_OnGroundRemoved;
                map.OnInteractablePlaced -= Map_OnInteractablePlaced;
                map.OnInteractableRemoved -= Map_OnInteractableRemoved;

                //Clear render operations.
                renderRequests.Clear();

                //Unrender all the regions.
                RegionPosition2D[] regionPositions = new RegionPosition2D[regionRenderers.Count];
                regionRenderers.Keys.CopyTo(regionPositions, 0);
                for (int i = 0; i < regionPositions.Length; i++)
                    UnrenderRegion(regionPositions[i]);
            }
        }



        /// <summary>
        /// Called when a ground tile is placed onto the map.
        /// </summary>
        /// <param name="region">The region the tile was placed into.</param>
        /// <param name="globalTilePosition">The position the tile was placed.</param>
        /// <param name="tile">The tile that was placed.</param>
        private void Map_OnGroundPlaced(Region2D region, TilePosition2D globalTilePosition, TileGround2D tile)
        {
            renderRequests.Enqueue(new _RenderGround(globalTilePosition, tile));
        }
        /// <summary>
        /// Called when a ground tile is removed from the map.
        /// </summary>
        /// <param name="region">The region the tile was removed from.</param>
        /// <param name="globalTilePosition">The position the tile was removed.</param>
        /// <param name="tile">The tile that was removed.</param>
        private void Map_OnGroundRemoved(Region2D region, TilePosition2D globalTilePosition, TileGround2D tile)
        {
            renderRequests.Enqueue(new _UnrenderGround(globalTilePosition, tile));
        }
        /// <summary>
        /// Called when an interactable tile is placed onto the map.
        /// </summary>
        /// <param name="region">The region the tile was placed into.</param>
        /// <param name="globalTilePosition">The position the tile was placed.</param>
        /// <param name="tile">The tile that was placed.</param>
        private void Map_OnInteractablePlaced(Region2D region, TilePosition2D globalTilePosition, TileInteractable2D tile)
        {
            renderRequests.Enqueue(new _RenderInteractable(globalTilePosition, tile));
        }
        /// <summary>
        /// Called when an interactable tile is removed from the map.
        /// </summary>
        /// <param name="region">The region the tile was removed from.</param>
        /// <param name="globalTilePosition">The position the tile was removed.</param>
        /// <param name="tile">The tile that was removed.</param>
        private void Map_OnInteractableRemoved(Region2D region, TilePosition2D globalTilePosition, TileInteractable2D tile)
        {
            renderRequests.Enqueue(new _UnrenderInteractable(globalTilePosition, tile));
        }



        /// <summary>
        /// Adds a request to render an entire region to the render queue.
        /// </summary>
        /// <param name="regionPosition2D">The position of the region to render.</param>
        public void RequestRenderRegion(RegionPosition2D regionPosition2D)
        {
            renderRequests.Enqueue(new _RenderRegion(regionPosition2D));
        }
        /// <summary>
        /// Adds a request to unrender an entire region to the render queue.
        /// </summary>
        /// <param name="regionPosition2D">The position of the region to unrender.</param>
        public void RequestUnrenderRegion(RegionPosition2D regionPosition2D)
        {
            renderRequests.Enqueue(new _UnrenderRegion(regionPosition2D));
        }

        /// <summary>
        /// Adds a request to render a tile.
        /// </summary>
        /// <param name="globalTilePosition">The position to render the tile.</param>
        /// <param name="tile">The tile to render.</param>
        public void RequestRenderInteractable(TilePosition2D globalTilePosition, TileInteractable2D tile)
        {
            if (tile != null)
                renderRequests.Enqueue(new _RenderInteractable(globalTilePosition, tile));
        }
        /// <summary>
        /// Adds a request to unrender a tile.
        /// </summary>
        /// <param name="globalTilePosition">The position to unrender the tile from.</param>
        /// <param name="tile">The tile to unrender.</param>
        public void RequestUnrenderInteractable(TilePosition2D globalTilePosition, TileInteractable2D tile)
        {
            if (tile != null)
                renderRequests.Enqueue(new _UnrenderInteractable(globalTilePosition, tile));
        }
        /// <summary>
        /// Adds a request to render a tile.
        /// </summary>
        /// <param name="globalTilePosition">The position to render the tile.</param>
        /// <param name="tile">The tile to render.</param>
        public void RequestRenderGround(TilePosition2D globalTilePosition, TileGround2D tile)
        {
            if (tile != null)
                renderRequests.Enqueue(new _RenderGround(globalTilePosition, tile));
        }
        /// <summary>
        /// Adds a request to unrender a tile.
        /// </summary>
        /// <param name="globalTilePosition">The position to unrender the tile from.</param>
        /// <param name="tile">The tile to unrender.</param>
        public void RequestUnrenderGround(TilePosition2D globalTilePosition, TileGround2D tile)
        {
            if (tile != null)
                renderRequests.Enqueue(new _UnrenderGround(globalTilePosition, tile));
        }



        /// <summary>
        /// Creates a region renderer.
        /// </summary>
        /// <param name="region">The region to create a region renderer for.</param>
        private void CreateRegionRenderer(Region2D region)
        {
            RegionRenderer2D regionRenderer = new RegionRenderer2D(region);
            regionRenderers.Add(region.GetPosition(), regionRenderer);
        }
        /// <summary>
        /// Unrenders and destroys a region renderer.
        /// </summary>
        /// <param name="region">The region to unrender.</param>
        private void DestroyRegionRenderer(Region2D region)
        {
            renderRequests.Enqueue(new _DestroyRegionRenderer(region.GetPosition()));
        }
        /// <summary>
        /// Renders a region.
        /// </summary>
        /// <param name="regionPosition">The position of the region to render.</param>
        private void RenderRegion(RegionPosition2D regionPosition)
        {
            if (regionRenderers.TryGetValue(regionPosition, out RegionRenderer2D regionRenderer))
            {
                Region2D region = regionRenderer.GetRegion();
                for (int x = 0; x < RegionPosition2D.REGION_SIZE; x++)
                {
                    for (int z = 0; z < RegionPosition2D.REGION_SIZE; z++)
                    {
                        TilePosition2D localTilePosition = new TilePosition2D(x, z);
                        TilePosition2D globalTilePosition = regionPosition.GetGlobalTilePosition(localTilePosition);

                        TileStack stack = region.GetTileStack(localTilePosition);
                        if (stack.ground != null)
                            regionRenderer.RenderGround(globalTilePosition, stack.ground);
                        if (stack.interactable != null)
                            regionRenderer.RenderInteractable(globalTilePosition, stack.interactable);
                    }
                }
            }
        }
        /// <summary>
        /// Unrenders a region.
        /// </summary>
        /// <param name="regionPosition">The position of the region to unrender.</param>
        private void UnrenderRegion(RegionPosition2D regionPosition)
        {
            if (regionRenderers.TryGetValue(regionPosition, out RegionRenderer2D regionRenderer))
            {
                Region2D region = regionRenderer.GetRegion();
                for (int x = 0; x < RegionPosition2D.REGION_SIZE; x++)
                {
                    for (int z = 0; z < RegionPosition2D.REGION_SIZE; z++)
                    {
                        TilePosition2D localTilePosition = new TilePosition2D(x, z);
                        TilePosition2D globalTilePosition = regionPosition.GetGlobalTilePosition(localTilePosition);

                        TileStack stack = region.GetTileStack(localTilePosition);
                        if (stack.ground != null)
                            regionRenderer.UnrenderGround(globalTilePosition, stack.ground);
                        if (stack.interactable != null)
                            regionRenderer.UnrenderInteractable(globalTilePosition, stack.interactable);
                    }
                }
            }
        }
        
        /// <summary>
        /// Renders a tile.
        /// </summary>
        /// <param name="globalTilePosition">The position of the tile to render.</param>
        /// <param name="tile">The tile to render.</param>
        private void RenderInteractable(TilePosition2D globalTilePosition, TileInteractable2D tile)
        {
            RegionPosition2D regionPosition = globalTilePosition.GetParentRegionPosition();
            if (regionRenderers.TryGetValue(regionPosition, out RegionRenderer2D regionRender))
                regionRender.RenderInteractable(globalTilePosition, tile);
        }
        /// <summary>
        /// Unrenders a tile.
        /// </summary>
        /// <param name="globalTilePosition">The position of the tile to unrender.</param>
        /// <param name="tile">The tile to unrender.</param>
        private void UnrenderInteractable(TilePosition2D globalTilePosition, TileInteractable2D tile)
        {
            RegionPosition2D regionPosition = globalTilePosition.GetParentRegionPosition();
            if (regionRenderers.TryGetValue(regionPosition, out RegionRenderer2D regionRender))
                regionRender.UnrenderInteractable(globalTilePosition, tile);
        }
        /// <summary>
        /// Renders a tile.
        /// </summary>
        /// <param name="globalTilePosition">The position of the tile to render.</param>
        /// <param name="tile">The tile to render.</param>
        private void RenderGround(TilePosition2D globalTilePosition, TileGround2D tile)
        {
            RegionPosition2D regionPosition = globalTilePosition.GetParentRegionPosition();
            if (regionRenderers.TryGetValue(regionPosition, out RegionRenderer2D regionRender))
                regionRender.RenderGround(globalTilePosition, tile);
        }
        /// <summary>
        /// Unrenders a tile.
        /// </summary>
        /// <param name="globalTilePosition">The position of the tile to unrender.</param>
        /// <param name="tile">The tile to unrender.</param>
        private void UnrenderGround(TilePosition2D globalTilePosition, TileGround2D tile)
        {
            RegionPosition2D regionPosition = globalTilePosition.GetParentRegionPosition();
            if (regionRenderers.TryGetValue(regionPosition, out RegionRenderer2D regionRender))
                regionRender.UnrenderGround(globalTilePosition, tile);
        }



        /// <summary>
        /// Processes the currently enqueued render requests.
        /// </summary>
        public void ProcessRenderRequests()
        {
            renderRequests.RunRequests(this, 1500);
        }



        /// <summary>
        /// A request to destroy a region renderer.
        /// </summary>
        private struct _DestroyRegionRenderer : IRequest<MapRenderer2D>
        {
            private readonly RegionPosition2D regionPosition;

            public _DestroyRegionRenderer(RegionPosition2D regionPosition)
            {
                this.regionPosition = regionPosition;
            }

            public void Process(MapRenderer2D val)
            {
                val.UnrenderRegion(regionPosition);
                val.regionRenderers.Remove(regionPosition);
            }
        }
        /// <summary>
        /// A request to render a region.
        /// </summary>
        private struct _RenderRegion : IRequest<MapRenderer2D>
        {
            private readonly RegionPosition2D regionPosition;

            public _RenderRegion(RegionPosition2D regionPosition)
            {
                this.regionPosition = regionPosition;
            }

            public void Process(MapRenderer2D val)
            {
                val.RenderRegion(regionPosition);
            }
        }
        /// <summary>
        /// A request to unrender a region.
        /// </summary>
        private struct _UnrenderRegion : IRequest<MapRenderer2D>
        {
            private readonly RegionPosition2D regionPosition;

            public _UnrenderRegion(RegionPosition2D regionPosition)
            {
                this.regionPosition = regionPosition;
            }

            public void Process(MapRenderer2D val)
            {
                val.UnrenderRegion(regionPosition);
            }
        }
        /// <summary>
        /// A request to render a ground tile.
        /// </summary>
        private struct _RenderGround : IRequest<MapRenderer2D>
        {
            private readonly TilePosition2D globalTilePosition;
            private readonly TileGround2D tile;

            public _RenderGround(TilePosition2D globalTilePosition, TileGround2D tile)
            {
                this.globalTilePosition = globalTilePosition;
                this.tile = tile;
            }

            public void Process(MapRenderer2D val)
            {
                val.RenderGround(globalTilePosition, tile);
            }
        }
        /// <summary>
        /// A request to unrender a ground tile.
        /// </summary>
        private struct _UnrenderGround : IRequest<MapRenderer2D>
        {
            private readonly TilePosition2D globalTilePosition;
            private readonly TileGround2D tile;

            public _UnrenderGround(TilePosition2D globalTilePosition, TileGround2D tile)
            {
                this.globalTilePosition = globalTilePosition;
                this.tile = tile;
            }

            public void Process(MapRenderer2D val)
            {
                val.UnrenderGround(globalTilePosition, tile);
            }
        }
        /// <summary>
        /// A request to render an interactable tile.
        /// </summary>
        private struct _RenderInteractable : IRequest<MapRenderer2D>
        {
            private readonly TilePosition2D globalTilePosition;
            private readonly TileInteractable2D tile;

            public _RenderInteractable(TilePosition2D globalTilePosition, TileInteractable2D tile)
            {
                this.globalTilePosition = globalTilePosition;
                this.tile = tile;
            }

            public void Process(MapRenderer2D val)
            {
                val.RenderInteractable(globalTilePosition, tile);
            }
        }
        /// <summary>
        /// A request to unrender an interactable tile.
        /// </summary>
        private struct _UnrenderInteractable : IRequest<MapRenderer2D>
        {
            private readonly TilePosition2D globalTilePosition;
            private readonly TileInteractable2D tile;

            public _UnrenderInteractable(TilePosition2D globalTilePosition, TileInteractable2D tile)
            {
                this.globalTilePosition = globalTilePosition;
                this.tile = tile;
            }

            public void Process(MapRenderer2D val)
            {
                val.UnrenderInteractable(globalTilePosition, tile);
            }
        }
    }
}
