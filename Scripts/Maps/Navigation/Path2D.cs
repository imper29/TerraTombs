using System.Collections.Generic;
using System.Linq;

namespace Maps.Navigation
{
    public class Path2D
    {
        /// <summary>
        /// The map this path is navigating.
        /// </summary>
        private readonly Map2D map;
        /// <summary>
        /// The positions in this path.
        /// </summary>
        private readonly Queue<TilePosition2D> positions;
        /// <summary>
        /// Is this path dirty?
        /// </summary>
        private bool isDirty;


        /// <summary>
        /// Creates an empty dirty path.
        /// </summary>
        public Path2D(Map2D map)
        {
            this.map = map;
            positions = new Queue<TilePosition2D>();
            isDirty = true;
        }
        /// <summary>
        /// Creates a new path.
        /// </summary>
        /// <param name="positions">The positions in the path.</param>
        public Path2D(Map2D map, TilePosition2D[] positions)
        {
            this.map = map;
            this.positions = new Queue<TilePosition2D>(positions.Reverse());

            map.OnRegionDestroyed += Map_OnRegionDestroyed;

            map.OnGroundPlaced += Map_OnGroundChanged;
            map.OnGroundRemoved += Map_OnGroundChanged;

            map.OnInteractablePlaced += Map_OnInteractableChanged;
            map.OnInteractableRemoved += Map_OnInteractableChanged;
        }


        /// <summary>
        /// Called when a region in the map is destroyed.
        /// </summary>
        /// <param name="region">The region that was destroyed.</param>
        private void Map_OnRegionDestroyed(Region2D region)
        {
            foreach (TilePosition2D p in positions)
            {
                if (p.GetParentRegionPosition().Equals(region.GetPosition()))
                {
                    MarkDirty();
                    return;
                }
            }
        }
        /// <summary>
        /// Called when a ground tile is placed or removed.
        /// </summary>
        /// <param name="region">The region that contains the tile.</param>
        /// <param name="globalTilePosition">The position the tile that was changed.</param>
        /// <param name="tile">The tile that was removed.</param>
        private void Map_OnGroundChanged(Region2D region, TilePosition2D globalTilePosition, TileGround2D tile)
        {
            if (positions.Contains(globalTilePosition))
                MarkDirty();
        }
        /// <summary>
        /// Called when an interactable tile is placed or removed.
        /// </summary>
        /// <param name="region">The region that contains the tile.</param>
        /// <param name="globalTilePosition">The position the tile that was changed.</param>
        /// <param name="tile">The tile that was removed.</param>
        private void Map_OnInteractableChanged(Region2D region, TilePosition2D globalTilePosition, TileInteractable2D tile)
        {
            if (positions.Contains(globalTilePosition))
                MarkDirty();
        }


        /// <summary>
        /// Makes the current position the next position in the positions queue.
        /// </summary>
        /// <returns>The new current position.</returns>
        public TilePosition2D GetNextPosition()
        {
            TilePosition2D p = positions.Dequeue();
            if (positions.Count == 0)
                Destroy();
            return p;
        }

        /// <summary>
        /// Marks this path dirty.
        /// </summary>
        public void MarkDirty()
        {
            isDirty = true;
        }
        /// <summary>
        /// Determines if the current position is the last position.
        /// </summary>
        /// <returns>True if the current position is the last node.</returns>
        public bool IsAtEnd()
        {
            return positions.Count == 0;
        }
        /// <summary>
        /// Determines if the path is dirty.
        /// </summary>
        /// <returns>If the path is dirty.</returns>
        public bool IsDirty()
        {
            return isDirty;
        }
        /// <summary>
        /// Destroys this path.
        /// </summary>
        public void Destroy()
        {
            map.OnRegionDestroyed -= Map_OnRegionDestroyed;
            map.OnGroundPlaced -= Map_OnGroundChanged;
            map.OnGroundRemoved -= Map_OnGroundChanged;
            map.OnInteractablePlaced -= Map_OnInteractableChanged;
            map.OnInteractableRemoved -= Map_OnInteractableChanged;
        }
    }
}
