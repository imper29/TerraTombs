using Maps;
using UnityEngine;

namespace Characters
{
    public struct CharacterPosition2D
    {
        /// <summary>
        /// The x and y values of this position.
        /// </summary>
        public float x, z;


        /// <summary>
        /// Creates a new character position.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="z">The z position.</param>
        public CharacterPosition2D(float x, float z)
        {
            this.x = x;
            this.z = z;
        }


        /// <summary>
        /// Gets the tile position this character position most aptely represents.
        /// </summary>
        /// <returns>The tile position this character position most aptely represents.</returns>
        public TilePosition2D GetTilePosition()
        {
            return new TilePosition2D(Mathf.FloorToInt(x), Mathf.FloorToInt(z));
        }


        /// <summary>
        /// Gets the distance between two character positions.
        /// </summary>
        /// <param name="a">The first character position.</param>
        /// <param name="b">The second character position.</param>
        /// <returns>The distance between two character positions.</returns>
        public static float GetDistance(CharacterPosition2D a, CharacterPosition2D b)
        {
            float x = a.x - b.x;
            float z = a.z - b.z;

            return Mathf.Sqrt(x * x + z * z);
        }
        /// <summary>
        /// Gets the squarded distance between two character positions.
        /// Use this method if you are simply comparing the distances between positions but do not care about the actual distance.
        /// </summary>
        /// <param name="a">The first character position.</param>
        /// <param name="b">The second character position.</param>
        /// <returns>The squarded distance between two character positions.</returns>
        public static float GetDistanceSqr(CharacterPosition2D a, CharacterPosition2D b)
        {
            float x = a.x - b.x;
            float z = a.z - b.z;

            return x * x + z * z;
        }
    }
}
