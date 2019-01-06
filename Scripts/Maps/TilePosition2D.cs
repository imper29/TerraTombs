using UnityEngine;

namespace Maps
{
    /// <summary>
    /// The position of a tile.
    /// </summary>
    public struct TilePosition2D
    {
        /// <summary>
        /// The tile position directly to the right of this tile position.
        /// </summary>
        public TilePosition2D Right
        {
            get
            {
                return new TilePosition2D(x + 1, z);
            }
        }
        /// <summary>
        /// The tile position directly to the left of this tile position.
        /// </summary>
        public TilePosition2D Left
        {
            get
            {
                return new TilePosition2D(x - 1, z);
            }
        }
        /// <summary>
        /// The tile position directly above this tile position.
        /// </summary>
        public TilePosition2D Above
        {
            get
            {
                return new TilePosition2D(x, z + 1);
            }
        }
        /// <summary>
        /// The tile position directly below this tile position.
        /// </summary>
        public TilePosition2D Below
        {
            get
            {
                return new TilePosition2D(x, z - 1);
            }
        }

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
                copy.x -= RegionPosition2D.REGION_SIZE_MINUS_ONE;
            if (z < 0)
                copy.z -= RegionPosition2D.REGION_SIZE_MINUS_ONE;

            return new RegionPosition2D(Mathf.FloorToInt(copy.x / RegionPosition2D.REGION_SIZE), Mathf.FloorToInt(copy.z / RegionPosition2D.REGION_SIZE));
        }

        /// <summary>
        /// Gets a tile position with the minimum x and z positions from two tile positions.
        /// </summary>
        /// <param name="a">The first tile position.</param>
        /// <param name="b">The second tile position.</param>
        /// <returns>A tile position with the minimum x and z positions from two tile positions.</returns>
        public static TilePosition2D GetMinPosition(TilePosition2D a, TilePosition2D b)
        {
            return new TilePosition2D(Mathf.Min(a.x, b.x), Mathf.Min(a.z, b.z));
        }
        /// <summary>
        /// Gets a tile position with the maximum x and z positions from two tile positions.
        /// </summary>
        /// <param name="a">The first tile position.</param>
        /// <param name="b">The second tile position.</param>
        /// <returns>A tile position with the maximum x and z positions from two tile positions.</returns>
        public static TilePosition2D GetMaxPosition(TilePosition2D a, TilePosition2D b)
        {
            return new TilePosition2D(Mathf.Max(a.x, b.x), Mathf.Max(a.z, b.z));
        }

        /// <summary>
        /// Gets the distance between two tile positions.
        /// </summary>
        /// <param name="a">The first tile position.</param>
        /// <param name="b">The second tile position.</param>
        /// <returns>The distance between two tile positions.</returns>
        public static int GetDistance(TilePosition2D a, TilePosition2D b)
        {
            int x = Mathf.Abs(a.x - b.x);
            int z = Mathf.Abs(a.z - b.z);

            if (x > z)
                return (x - z) * 10 + z * 14;
            return (z - x) * 10 + x * 14;
        }


        /// <summary>
        /// Adds two tile positions together.
        /// </summary>
        /// <param name="a">The first tile position.</param>
        /// <param name="b">The second tile position.</param>
        /// <returns></returns>
        public static TilePosition2D operator +(TilePosition2D a, TilePosition2D b)
        {
            return new TilePosition2D(a.x + b.x, a.z + b.z);
        }
        /// <summary>
        /// Subtracts one tile position from another.
        /// </summary>
        /// <param name="a">The first tile position.</param>
        /// <param name="b">The tile position to subtract.</param>
        /// <returns></returns>
        public static TilePosition2D operator -(TilePosition2D a, TilePosition2D b)
        {
            return new TilePosition2D(a.x - b.x, a.z - b.z);
        }
    }
}
