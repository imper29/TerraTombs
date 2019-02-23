namespace Maps.Rendering
{
    /// <summary>
    /// An object used to render a region.
    /// </summary>
    public class RegionRenderer2D
    {
        private readonly Region2D region;
        private readonly RegionPosition2D position;
        private readonly TileStackRenderData2D[,] tiles;

        /// <summary>
        /// Creates region rendering data for a specific region.
        /// </summary>
        /// <param name="region">The region this render data will render.</param>
        public RegionRenderer2D(Region2D region)
        {
            this.region = region;
            position = region.GetPosition();
            tiles = new TileStackRenderData2D[RegionPosition2D.REGION_SIZE, RegionPosition2D.REGION_SIZE];
        }

        /// <summary>
        /// Gets the region this region renderer is rendering.
        /// </summary>
        /// <returns>This region renderer's region.</returns>
        public Region2D GetRegion()
        {
            return region;
        }

        /// <summary>
        /// THIS METHOD SHOULD ONLY BE CALLED FROM MapRenderer2D.
        /// Renders a tile at globalTilePosition.
        /// </summary>
        /// <param name="globalTilePosition"></param>
        /// <param name="tile">The tile to render.</param>
        public void RenderGround(TilePosition2D globalTilePosition, TileGround2D tile)
        {
            TilePosition2D localTilePosition = position.GetLocalTilePosition(globalTilePosition);
            if (tiles[localTilePosition.x, localTilePosition.z].groundRenderObject == null)
                tiles[localTilePosition.x, localTilePosition.z].groundRenderObject = tile.OnRendered(region, globalTilePosition);
#if DEBUGGING
            else
                Debug.LogWarning("Attempted to render a ground tile at a tile position that is already rendered!");
#endif
        }
        /// <summary>
        /// THIS METHOD SHOULD ONLY BE CALLED FROM MapRenderer2D.
        /// Unrenders a tile at globalTilePosition.
        /// </summary>
        /// <param name="globalTilePosition"></param>
        /// <param name="tile">The tile to unrender.</param>
        public void UnrenderGround(TilePosition2D globalTilePosition, TileGround2D tile)
        {
            TilePosition2D localTilePosition = position.GetLocalTilePosition(globalTilePosition);
            if (tiles[localTilePosition.x, localTilePosition.z].groundRenderObject != null)
            {
                tile.OnUnrendered(region, globalTilePosition, tiles[localTilePosition.x, localTilePosition.z].groundRenderObject);
                tiles[localTilePosition.x, localTilePosition.z].groundRenderObject = null;
            }
#if DEBUGGING
            else
                Debug.LogWarning("Attempted to unrender a ground tile at a tile position that isn't already rendered!");
#endif
        }

        /// <summary>
        /// THIS METHOD SHOULD ONLY BE CALLED FROM MapRenderer2D.
        /// Renders a tile at globalTilePosition.
        /// </summary>
        /// <param name="globalTilePosition"></param>
        /// <param name="tile">The tile to render.</param>
        public void RenderInteractable(TilePosition2D globalTilePosition, TileInteractable2D tile)
        {
            TilePosition2D localTilePosition = position.GetLocalTilePosition(globalTilePosition);
            if (tiles[localTilePosition.x, localTilePosition.z].interactableRenderObject == null)
                tiles[localTilePosition.x, localTilePosition.z].interactableRenderObject = tile.OnRendered(region, globalTilePosition);
#if DEBUGGING
            else
                Debug.LogWarning("Attempted to render an interactable tile at a tile position that is already rendered!");
#endif
        }
        /// <summary>
        /// THIS METHOD SHOULD ONLY BE CALLED FROM MapRenderer2D.
        /// Unrenders a tile at globalTilePosition.
        /// </summary>
        /// <param name="globalTilePosition"></param>
        /// <param name="tile">The tile to unrender.</param>
        public void UnrenderInteractable(TilePosition2D globalTilePosition, TileInteractable2D tile)
        {
            TilePosition2D localTilePosition = position.GetLocalTilePosition(globalTilePosition);
            if (tiles[localTilePosition.x, localTilePosition.z].interactableRenderObject != null)
            {
                tile.OnUnrendered(region, globalTilePosition, tiles[localTilePosition.x, localTilePosition.z].interactableRenderObject);
                tiles[globalTilePosition.x, globalTilePosition.z].interactableRenderObject = null;
            }
#if DEBUGGING
            else
                Debug.LogWarning("Attempted to unrender an interactable tile at a tile position that isn't already rendered!");
#endif
        }
    }
}
