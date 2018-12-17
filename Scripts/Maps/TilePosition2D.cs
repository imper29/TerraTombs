using UnityEngine;

namespace Maps
{
    /// <summary>
    /// The position of a tile.
    /// </summary>
    public struct TilePosition2D
    {
        /// <summary>
        /// An offset to make local tile positions reference the correct tile in regions that have negative x or y positions.
        /// </summary>
        private const int REGION_NEGATIVE_LOCAL_OFFSET = RegionPosition2D.REGION_SIZE - 1;
        /// <summary>
        /// The x and y values of this position.
        /// </summary>
        public int x, z;


        /// <summary>
        /// Creates a new tile position.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="z">The z position.</param>
        public TilePosition2D(int x, int z)
        {
            this.x = x;
            this.z = z;
        }


        /// <summary>
        /// Gets the position of a region that would contain this tile position.
        /// </summary>
        /// <returns></returns>
        public RegionPosition2D GetParentRegionPosition()
        {
            //Create a copy of this tile position.
            TilePosition2D copy = this;

            //If the tile position is negative, move it so the rounding gives the correct region position.
            if (x < 0)
                copy.x -= REGION_NEGATIVE_LOCAL_OFFSET;
            if (z < 0)
                copy.z -= REGION_NEGATIVE_LOCAL_OFFSET;

            return new RegionPosition2D(Mathf.FloorToInt(copy.x / RegionPosition2D.REGION_SIZE), Mathf.FloorToInt(copy.z / RegionPosition2D.REGION_SIZE));
        }
    }
}
