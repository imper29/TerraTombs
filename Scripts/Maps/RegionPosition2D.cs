namespace Maps
{
    /// <summary>
    /// The position of a region.
    /// </summary>
    public struct RegionPosition2D
    {
        /// <summary>
        /// The size of a region in tiles.
        /// </summary>
        public const int REGION_SIZE = 50;
        /// <summary>
        /// The x and y values of this position.
        /// </summary>
        public int x, z;


        /// <summary>
        /// Creates a new region position.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="z">The z position.</param>
        public RegionPosition2D(int x, int z)
        {
            this.x = x;
            this.z = z;
        }


        /// <summary>
        /// Gets a tile position that is relative to this region position.
        /// </summary>
        /// <param name="globalTilePosition">The tile position relative to the origin.</param>
        /// <returns></returns>
        public TilePosition2D GetLocalTilePosition(TilePosition2D globalTilePosition)
        {
            //Create the local position.
            TilePosition2D returnPos = new TilePosition2D(globalTilePosition.x - x * REGION_SIZE, globalTilePosition.z - z * REGION_SIZE);

            //Return the new local position.
            return returnPos;
        }
        /// <summary>
        /// Gets a tile position that is relative to the origin.
        /// </summary>
        /// <param name="localTilePosition">The tile position relative to this region.</param>
        /// <returns></returns>
        public TilePosition2D GetGlobalTilePosition(TilePosition2D localTilePosition)
        {
            //Create the global position.
            TilePosition2D returnPos = new TilePosition2D(localTilePosition.x + x * RegionPosition2D.REGION_SIZE, localTilePosition.z + z * RegionPosition2D.REGION_SIZE);

            //Return the new global position.
            return returnPos;
        }
        /// <summary>
        /// Gets a tile position that is relative to the origin.
        /// </summary>
        /// <param name="localTilePositionX">The tile x position relative to this region.</param>
        /// <param name="localTilePositionZ">The tile z position relative to this region.</param>
        /// <returns></returns>
        public TilePosition2D GetGlobalTilePosition(int localTilePositionX, int localTilePositionZ)
        {
            //Create the global position.
            TilePosition2D returnPos = new TilePosition2D(localTilePositionX + x * RegionPosition2D.REGION_SIZE, localTilePositionZ + z * RegionPosition2D.REGION_SIZE);

            //Return the new global position.
            return returnPos;
        }
    }
}
