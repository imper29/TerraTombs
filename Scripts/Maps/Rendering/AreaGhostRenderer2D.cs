using UnityEngine;

namespace Maps.Rendering
{
    /// <summary>
    /// Renders an area of tiles as ghost tiles.
    /// </summary>
    public class AreaGhostRenderer2D
    {
        /// <summary>
        /// The tile stacks this area renderer will render.
        /// </summary>
        private readonly TileStack[,] tiles;
        private readonly TileStackRenderData2D[,] tileRenderers;
        /// <summary>
        /// Determines if the ghost renderer is currently rendered.
        /// </summary>
        private bool rendered;

        
        /// <summary>
        /// Creates a new area ghost renderer.
        /// </summary>
        /// <param name="tiles">The tiles this renderer will render / unrender.</param>
        public AreaGhostRenderer2D(TileStack[,] tiles)
        {
            this.tiles = tiles;
            tileRenderers = new TileStackRenderData2D[tiles.GetLength(0), tiles.GetLength(1)];
            rendered = false;
        }


        /// <summary>
        /// Renders the tiles in this area ghost renderer as ghosts.
        /// </summary>
        /// <param name="tilePosition">The position to render this ghost renderer.</param>
        public void Render(TilePosition2D tilePosition)
        {
            if (rendered)
            {
                Debug.LogWarning("Attempted to render a ghost area that is already rendered!");
            }
            else
            {
                rendered = true;
                int width = tiles.GetLength(0);
                int height = tiles.GetLength(1);

                for (int x = 0; x < width; x++)
                    for (int z = 0; z < height; z++)
                    {
                        TilePosition2D globalTilePosition = new TilePosition2D(x + tilePosition.x, z + tilePosition.z);

                        //render the ghost ground tile.
                        if (tiles[x, z].ground != null)
                            tileRenderers[x, z].groundRenderObject = tiles[x, z].ground.OnGhostRendered(null, globalTilePosition);
                        //Render the ghost interactable tile.
                        if (tiles[x, z].interactable != null)
                            tileRenderers[x, z].interactableRenderObject = tiles[x, z].interactable.OnGhostRendered(null, globalTilePosition);
                    }
            }
        }
        /// <summary>
        /// Unrenders the tiles in this area ghost renderer as ghosts.
        /// </summary>
        /// <param name="tilePosition">The position to unrender this ghost renderer.</param>
        public void Unrender(TilePosition2D tilePosition)
        {
            if (rendered)
            {
                rendered = false;
                int width = tiles.GetLength(0);
                int height = tiles.GetLength(1);

                for (int x = 0; x < width; x++)
                    for (int z = 0; z < height; z++)
                    {
                        TilePosition2D globalTilePositon = new TilePosition2D(x + tilePosition.x, z + tilePosition.z);
                        //Unrender the ground tile.
                        if (tiles[x, z].ground != null)
                        {
                            tiles[x, z].ground.OnGhostUnrendered(null, globalTilePositon, tileRenderers[x, z].groundRenderObject);
                            tileRenderers[x, z].groundRenderObject = null;
                        }
                        //Unrender the interactable tile.
                        if (tiles[x, z].interactable != null)
                        {
                            tiles[x, z].interactable.OnGhostUnrendered(null, globalTilePositon, tileRenderers[x, z].interactableRenderObject);
                            tileRenderers[x, z].interactableRenderObject = null;
                        }
                    }
            }
            else
            {
                Debug.LogWarning("Attempted to unrender a ghost area that is not rendered!");
            }
        }
    }
}
