using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Maps.Navigation
{
    /// <summary>
    /// Handles all the map navigators and provides a seperate thread to do pathfinding.
    /// </summary>
    internal class MapNavigatorHandler2D : MonoBehaviour
    {
        private static readonly List<MapNavigator2D> mapNavigators = new List<MapNavigator2D>();
        private static readonly Thread thread = new Thread(ThreadFunc);
        private static bool threadIsRunning;

        private void Start()
        {
            threadIsRunning = true;
            thread.Start();
        }
        private void OnApplicationQuit()
        {
            threadIsRunning = false;
        }
        private void Update()
        {
            ThreadFunc();
        }

        /// <summary>
        /// The function that runs in a seperate thread.
        /// This function processes node refreshing and pathfinding requests.
        /// </summary>
        private static void ThreadFunc()
        {
            //while (threadIsRunning)
            {
                MapNavigator2D[] navs;
                lock (mapNavigators)
                    navs = mapNavigators.ToArray();
                for (int i = 0; i < navs.Length; i++)
                    navs[i].ProcessPathRequests();
            }
        }


        /// <summary>
        /// Called when a navigator is enabled.
        /// Adds a map navigator for the navigation handler.
        /// </summary>
        /// <param name="mapNavigator">The map navigator to add.</param>
        public static void OnNavigatorEnabled(MapNavigator2D mapNavigator)
        {
            lock (mapNavigators)
                if (!mapNavigators.Contains(mapNavigator))
                    mapNavigators.Add(mapNavigator);
        }
        /// <summary>
        /// Called when a navigator is disabled.
        /// Removes a map navigator from the navigation handler.
        /// </summary>
        /// <param name="mapNavigator">The map navigator to remove.</param>
        public static void OnNavigatorDisabled(MapNavigator2D mapNavigator)
        {
            lock (mapNavigators)
                mapNavigators.Remove(mapNavigator);
        }
    }
}
