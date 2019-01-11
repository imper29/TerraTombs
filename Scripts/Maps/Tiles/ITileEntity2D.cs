using System.IO;

namespace Maps
{
    public interface ITileEntity2D
    {
        /// <summary>
        /// Copies the values from another tile entity.
        /// </summary>
        /// <param name="source">The tile entity to copy values from.</param>
        void ImportValues(ITileEntity2D source);
        /// <summary>
        /// Creates a duplicate of this tile entity.
        /// </summary>
        /// <param name="region">The region this tile is inside.</param>
        /// <param name="globalTilePosition">The position of this tile.</param>
        ITileEntity2D Duplicate(Region2D region, TilePosition2D globalTilePosition);

        /// <summary>
        /// Called when the interactable tile that placed this tile entity is placed onto a map for any reason.
        /// </summary>
        /// <param name="region">The region this tile is inside.</param>
        /// <param name="globalTilePosition">The position of this tile.</param>
        void OnPlaced(Region2D region, TilePosition2D globalTilePosition);
        /// <summary>
        /// Called when the interactable tile that placed this tile entity is removed from a map for any reason.
        /// </summary>
        /// <param name="region">The region this tile is inside.</param>
        /// <param name="globalTilePosition">The position of this tile.</param>
        void OnRemoved(Region2D region, TilePosition2D globalTilePosition);


        /// <summary>
        /// Exports this region.
        /// </summary>
        /// <param name="writer">The stream to export into.</param>
        void Export(BinaryWriter writer);
        /// <summary>
        /// Imports this region.
        /// </summary>
        /// <param name="reader">The stream to import from.</param>
        void Import(BinaryReader reader);
    }
}
