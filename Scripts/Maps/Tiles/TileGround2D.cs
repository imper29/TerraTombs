namespace Maps
{
    public abstract class TileGround2D : Tile2D
    {
        /// <summary>
        /// Determines the cost of traveling over this tile.
        /// </summary>
        /// <param name="region">The region this tile is inside.</param>
        /// <param name="globalTilePosition">The position of this tile.</param>
        /// <returns>The cost of traveling over this tile.</returns>
        public virtual int GetNavigationWeight(Region2D region, TilePosition2D globalTilePosition)
        {
            return Navigation.MapNavigator2D.DEFAULT_NAVIGATIONAL_WEIGHT;
        }
    }
}
