namespace Maps
{
    /// <summary>
    /// A square area that contains a bunch of tiles.
    /// Maps are collections of regions.
    /// </summary>
    public class Region2D
    {
        /// <summary>
        /// The map that contains this region.
        /// </summary>
        private readonly Map2D map;
        /// <summary>
        /// The position of this region.
        /// </summary>
        private readonly RegionPosition2D position;
        /// <summary>
        /// The tiles in this region.
        /// </summary>
        private readonly TileStack[,] tiles;


        /// <summary>
        /// Creates a new region at a specific position.
        /// </summary>
        /// <param name="position">The position of the new region.</param>
        public Region2D(Map2D map, RegionPosition2D position)
        {
            this.map = map;
            this.position = position;
            tiles = new TileStack[RegionPosition2D.REGION_SIZE, RegionPosition2D.REGION_SIZE];
        }


        /// <summary>
        /// Gets the map that contains this region.
        /// </summary>
        /// <returns>The map that contains this region.</returns>
        public Map2D GetMap()
        {
            return map;
        }
        /// <summary>
        /// Gets the 2D array containing all the tiles.
        /// </summary>
        /// <returns>The 2D array containing all the tiles.</returns>
        public TileStack[,] GetTileStacks()
        {
            return tiles;
        }
        /// <summary>
        /// Gets the position of this region.
        /// </summary>
        /// <returns>The position of this region.</returns>
        public RegionPosition2D GetPosition()
        {
            return position;
        }

        /// <summary>
        /// Places a ground tile at globalTilePosition.
        /// If a ground tile already exists in that position, the ground tile is first removed.
        /// </summary>
        /// <param name="localTilePosition">Where to place the ground tile.</param>
        /// <param name="tile">The ground tile to place.</param>
        public void PlaceGround(TilePosition2D localTilePosition, ITileGround2D tile)
        {
            //Remove the existing tile.
            if (tiles[localTilePosition.x, localTilePosition.z].ground != null)
                RemoveGround(localTilePosition);

            //Place the new tile.
            tiles[localTilePosition.x, localTilePosition.z].ground = tile;
            tile.OnPlaced(this, localTilePosition);
            //Call the OnTilePlaced map event.
            map?.OnGroundPlaced(this, position.GetGlobalTilePosition(localTilePosition), tile);
        }
        /// <summary>
        /// Gets the ground tile at localTilePosition.
        /// If no ground tile exists at the position, null is returned.
        /// </summary>
        /// <param name="localTilePosition">The position of the ground tile to find.</param>
        /// <returns>The ground tile at localTilePosition.</returns>
        public ITileGround2D GetGround(TilePosition2D localTilePosition)
        {
            return tiles[localTilePosition.x, localTilePosition.z].ground;
        }
        /// <summary>
        /// Removes the ground tile at localTilePosition.
        /// </summary>
        /// <param name="localTilePosition">The position of the ground tile to remove.</param>
        public void RemoveGround(TilePosition2D localTilePosition)
        {
            //Get the tile.
            ITileGround2D tile = tiles[localTilePosition.x, localTilePosition.z].ground;

            //Remove the tile.
            if (tile != null)
            {
                tile.OnRemoved(this, localTilePosition);
                tiles[localTilePosition.x, localTilePosition.z].ground = null;
                //Call the OnTileRemoved map event.
                map?.OnGroundRemoved(this, position.GetGlobalTilePosition(localTilePosition), tile);
            }
        }

        /// <summary>
        /// Places an interactable tile at globalTilePosition.
        /// If an interactable tile already exists in that position, the interactable tile is first removed.
        /// </summary>
        /// <param name="localTilePosition">Where to place the interactable tile.</param>
        /// <param name="tile">The interactable tile to place.</param>
        public void PlaceInteractable(TilePosition2D localTilePosition, ITileInteractable2D tile)
        {
            //Remove the existing tile.
            if (tiles[localTilePosition.x, localTilePosition.z].ground != null)
                RemoveInteractable(localTilePosition);

            //Place the new tile.
            tiles[localTilePosition.x, localTilePosition.z].interactable = tile;
            tile.OnPlaced(this, localTilePosition);
            //Call the OnTilePlaced map event.
            map?.OnInteractablePlaced(this, position.GetGlobalTilePosition(localTilePosition), tile);
        }
        /// <summary>
        /// Gets the interactable tile at localTilePosition.
        /// If no interactable tile exists at the position, null is returned.
        /// </summary>
        /// <param name="localTilePosition">The position of the interactable tile to find.</param>
        /// <returns>The interactable tile at localTilePosition.</returns>
        public ITileInteractable2D GetInteractable(TilePosition2D localTilePosition)
        {
            return tiles[localTilePosition.x, localTilePosition.z].interactable;
        }
        /// <summary>
        /// Removes the interactable tile at localTilePosition.
        /// </summary>
        /// <param name="localTilePosition">The position of the interactable tile to remove.</param>
        public void RemoveInteractable(TilePosition2D localTilePosition)
        {
            //Get the tile.
            ITileInteractable2D tile = tiles[localTilePosition.x, localTilePosition.z].interactable;

            //Remove the tile.
            if (tile != null)
            {
                tile.OnRemoved(this, localTilePosition);
                tiles[localTilePosition.x, localTilePosition.z].ground = null;
                //Call the OnTileRemoved map event.
                map?.OnInteractableRemoved(this, position.GetGlobalTilePosition(localTilePosition), tile);
            }
        }


        /// <summary>
        /// A struct containing all the tiles in a specific tile position.
        /// </summary>
        public struct TileStack
        {
            public ITileGround2D ground;
            public ITileInteractable2D interactable;
        }
    }
}
