using UnityEngine;
using Utils.Collections;

namespace Maps
{
    /// <summary>
    /// A base class for ground and interactable tiles.
    /// </summary>
    public abstract class Tile2D : ScriptableObject, IRegistryEntry<string>
    {
        /// <summary>
        /// The name of this tile.
        /// </summary>
        [SerializeField]
        private string registryName;

        /// <summary>
        /// Gets this tile's registry name.
        /// </summary>
        /// <returns>This tile's registry name.</returns>
        public string GetRegistryName()
        {
            return registryName;
        }

        /// <summary>
        /// Called when the tils is placed onto a map for any reason.
        /// </summary>
        /// <param name="region">The region this tile is inside.</param>
        /// <param name="globalTilePosition">The position of this tile.</param>
        public abstract void OnPlaced(Region2D region, TilePosition2D globalTilePosition);
        /// <summary>
        /// Called when the tile is removed from a map for any reason.
        /// </summary>
        /// <param name="region">The region this tile is inside.</param>
        /// <param name="globalTilePosition">The position of this tile.</param>
        public abstract void OnRemoved(Region2D region, TilePosition2D globalTilePosition);

        /// <summary>
        /// Called when the tile needs to be rendered.
        /// </summary>
        /// <param name="region">The region this tile is inside.</param>
        /// <param name="globalTilePosition">The position of this tile.</param>
        /// <returns>An object to store a reference to the rendering data. This object is passed into OnUnrendered when the tile is unrendered.</returns>
        public abstract object OnRendered(Region2D region, TilePosition2D globalTilePosition);
        /// <summary>
        /// Called when the tile needs to be unrendered.
        /// </summary>
        /// <param name="region">The region this tile is inside.</param>
        /// <param name="globalTilePosition">The position of this tile.</param>
        /// <param name="renderObject">The object that holds the rendering data. The object was created from OnRendered.</param>
        public abstract void OnUnrendered(Region2D region, TilePosition2D globalTilePosition, object renderObject);

        /// <summary>
        /// Called when the tile needs to be rendered as a ghost.
        /// </summary>
        /// <param name="region">The region this tile is inside.</param>
        /// <param name="globalTilePosition">The position of this tile.</param>
        /// <returns>An object to store a reference to the rendering data. This object is passed into OnUnrendered when the tile is unrendered.</returns>
        public abstract object OnGhostRendered(Region2D region, TilePosition2D globalTilePosition);
        /// <summary>
        /// Called when the tile needs to be unrendered from a ghost.
        /// </summary>
        /// <param name="region">The region this tile is inside.</param>
        /// <param name="globalTilePosition">The position of this tile.</param>
        /// <param name="renderObject">The object that holds the rendering data. The object was created from OnGhostRendered.</param>
        public abstract void OnGhostUnrendered(Region2D region, TilePosition2D globalTilePosition, object renderObject);

        /// <summary>
        /// Gets the unlocalized name of this tile.
        /// </summary>
        /// <returns>The unlocalized name of this tile.</returns>
        public abstract string GetUnlocalizedName();

        /// <summary>
        /// Gets the mesh used to render this tile in the UI.
        /// </summary>
        /// <returns>The mesh used to render this tile in the UI.</returns>
        public abstract Mesh GetUiMesh();
        /// <summary>
        /// Gets the material used to render this tile in the UI.
        /// </summary>
        /// <returns>The material used to render this tile in the UI.</returns>
        public abstract Material GetUiMaterial();
    }
}
