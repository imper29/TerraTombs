using System.Collections.Generic;
using System.Linq;

namespace Maps.Navigation
{
    public class Path2D
    {
        /// <summary>
        /// The positions in this path.
        /// </summary>
        private readonly Queue<TilePosition2D> positions;


        /// <summary>
        /// Creates an empty path.
        /// </summary>
        public Path2D()
        {
            positions = new Queue<TilePosition2D>();
        }
        /// <summary>
        /// Creates a new path.
        /// </summary>
        /// <param name="positions">The positions in the path.</param>
        public Path2D(TilePosition2D[] positions)
        {
            this.positions = new Queue<TilePosition2D>(positions);
            this.positions.Reverse();
        }


        /// <summary>
        /// Makes the current position the next position in the positions queue.
        /// </summary>
        /// <returns>The new current position.</returns>
        public TilePosition2D GetNextPosition()
        {
            return positions.Dequeue();
        }

        /// <summary>
        /// Determines if the current position is the last position.
        /// </summary>
        /// <returns>True if the current position is the last node.</returns>
        public bool IsAtEnd()
        {
            return positions.Count == 0;
        }
    }
}
