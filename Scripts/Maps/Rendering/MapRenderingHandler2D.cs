using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Maps.Rendering
{
    /// <summary>
    /// Handles all the map renderers.
    /// </summary>
    public class MapRenderingHandler2D : MonoBehaviour
    {
        public static EntityManager ENTITY_MANAGER { get; private set; }
        private static readonly List<MapRenderer2D> mapRenderers = new List<MapRenderer2D>();

        private void Awake()
        {
            ENTITY_MANAGER = World.Active.GetOrCreateManager<EntityManager>();
            DontDestroyOnLoad(gameObject);
        }
        private void Update()
        {
            MapRenderer2D[] rends;
            lock (mapRenderers)
                rends = mapRenderers.ToArray();
            for (int i = 0; i < rends.Length; i++)
                rends[i].ProcessRenderRequests();
        }

        /// <summary>
        /// Called when a map renderer is enabled.
        /// Adds a map renderer for the navigation handler.
        /// </summary>
        /// <param name="mapRenderer">The map renderer to add.</param>
        public static void OnRendererEnabled(MapRenderer2D mapRenderer)
        {
            lock (mapRenderers)
                mapRenderers.Add(mapRenderer);
        }
        /// <summary>
        /// Called when a map renderer is disabled.
        /// Removes a map renderer from the navigation handler.
        /// </summary>
        /// <param name="mapRenderer">The map renderer to remove.</param>
        public static void OnRendererDisabled(MapRenderer2D mapRenderer)
        {
            lock (mapRenderers)
                mapRenderers.Remove(mapRenderer);
        }
    }
}
