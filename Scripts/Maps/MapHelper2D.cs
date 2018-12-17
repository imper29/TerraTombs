using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maps
{
    /// <summary>
    /// Provides some helper functions for dealing with maps.
    /// </summary>
    public static class MapHelper2D
    {
        /// <summary>
        /// A plane used for quickly finding approximate raycasting collisions.
        /// </summary>
        private static readonly Plane MAP_COLLISION_PLANE = new Plane(Vector3.down, Vector3.zero);

        /// <summary>
        /// Gets the tile position that intersects a ray created from the viewport and the screenPosition.
        /// If you passed in the main camera and the mouse position, you would get the tile position that the mouse is hovered over.
        /// </summary>
        /// <param name="viewport">The viewport to create a ray from.</param>
        /// <param name="screenPosition">The position to create a ray from.</param>
        /// <returns>The tile position that intersects a ray created from the viewport and the screenPosition.</returns>
        public static TilePosition2D GetTilePosition(Camera viewport, Vector3 screenPosition)
        {
            //Create a ray from the viewport.
            Ray ray = viewport.ScreenPointToRay(screenPosition);

            //Get the intersection of the ray and the map collision plane.
            float distance;
            MAP_COLLISION_PLANE.Raycast(ray, out distance);
            Vector3 pos = ray.GetPoint(distance);

            //Round it to the nearest tile position.
            return new TilePosition2D(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z));
        }
    }
}
