using Maps.Navigation;

namespace Maps
{
    public abstract class TileInteractable2D : Tile2D
    {
        /// <summary>
        /// Determines which forms of navigation are supported by this interactable tile.
        /// </summary>
        /// <param name="region">The region this tile is inside.</param>
        /// <param name="globalTilePosition">The position of this tile.</param>
        /// <returns>Which forms of navigation are supported by this interactable tile.</returns>
        public virtual NavigationMode GetNavigationSupportedModes(Region2D region, TilePosition2D globalTilePosition)
        {
            return NavigationMode.All;
        }
        /// <summary>
        /// Creates the tile entity for this interactable tile. If the interactable tile doesn't need a tile entity, this method should return null.
        /// </summary>
        /// <param name="region">The region this tile is inside.</param>
        /// <param name="globalTilePosition">The position of this tile.</param>
        /// <returns>A new tile entity. If the interactable tile doesn't need to create a tile entity, this method should return null.</returns>
        public virtual ITileEntity2D CreateTileEntity(Region2D region, TilePosition2D globalTilePosition)
        {
            return null;
        }
    }
}
