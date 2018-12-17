namespace Maps
{
    /// <summary>
    /// A base interface for ground and interactable tiles.
    /// </summary>
    public interface ITile2D
    {
        /// <summary>
        /// Called when the tils is placed onto a map for any reason.
        /// </summary>
        /// <param name="region">The region this tile is inside.</param>
        /// <param name="globalTilePosition">The position of this tile.</param>
        void OnPlaced(Region2D region, TilePosition2D globalTilePosition);
        /// <summary>
        /// Called when the tile is removed from a map for any reason.
        /// </summary>
        /// <param name="region">The region this tile is inside.</param>
        /// <param name="globalTilePosition">The position of this tile.</param>
        void OnRemoved(Region2D region, TilePosition2D globalTilePosition);

        /// <summary>
        /// Called when the tile needs to be rendered.
        /// </summary>
        /// <param name="region">The region this tile is inside.</param>
        /// <param name="globalTilePosition">The position of this tile.</param>
        void OnRendered(Region2D region, TilePosition2D globalTilePosition);
        /// <summary>
        /// Called when the tile needs to be unrendered.
        /// </summary>
        /// <param name="region">The region this tile is inside.</param>
        /// <param name="globalTilePosition">The position of this tile.</param>
        void OnUnrendered(Region2D region, TilePosition2D globalTilePosition);
    }
}
