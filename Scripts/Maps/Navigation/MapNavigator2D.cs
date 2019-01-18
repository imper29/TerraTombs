using System.Collections.Generic;
using UnityEngine;
using Utils.Collections;

namespace Maps.Navigation
{
    /// <summary>
    /// Provides navigation and pathfinding capabilities to a map.
    /// </summary>
    public class MapNavigator2D
    {
        /// <summary>
        /// The default navigation weight for a node.
        /// </summary>
        public const int DEFAULT_NAVIGATIONAL_WEIGHT = 10;

        /// <summary>
        /// The map that holds the tiles.
        /// </summary>
        private readonly Map2D map;
        /// <summary>
        /// All the requests for paths.
        /// </summary>
        private readonly Queue<PathRequest2D> pathRequests;
        /// <summary>
        /// Is this renderer enabled?
        /// </summary>
        private bool enabled;


        /// <summary>
        /// Creates a disabled map navigator.
        /// </summary>
        /// <param name="map">The map that this navigator will navigation for.</param>
        public MapNavigator2D(Map2D map)
        {
            this.map = map;
            pathRequests = new Queue<PathRequest2D>();
        }

        /// <summary>
        /// Enables this renderer.
        /// </summary>
        public void Enable()
        {
            if (!enabled)
            {
                enabled = true;
                MapNavigatorHandler2D.OnNavigatorEnabled(this);
            }
        }
        /// <summary>
        /// Disables this renderer.
        /// </summary>
        public void Disable()
        {
            if (enabled)
            {
                enabled = false;
                MapNavigatorHandler2D.OnNavigatorDisabled(this);
            }
        }


        /// <summary>
        /// Calculates a path from an origin to a destination.
        /// Implements the A* algorithm.
        /// </summary>
        /// <param name="originPos">The path's start position.</param>
        /// <param name="destinationPos">The path's destination position.</param>
        /// <param name="navigationMode">The type of navigation to use when path finding.</param>
        /// <returns>A path that goes from originPos to destinationPos.</returns>
        private Path2D GetPath(TilePosition2D originPos, TilePosition2D destinationPos, NavigationMode navigationMode)
        {
            int maxNodeCount = map.GetRegionCount() * RegionPosition2D.REGION_SIZE * RegionPosition2D.REGION_SIZE;
            if (maxNodeCount == 0)
            {
                Debug.LogWarning("Failed to find a path from " + originPos.x + "," + originPos.z + " to " + destinationPos.x + "," + destinationPos.z);
                return new Path2D(map);
            }

            //A collection all the existing nodes.
            Dictionary<TilePosition2D, Node2D> allNodes = new Dictionary<TilePosition2D, Node2D>();
            //All the nodes that have already been checked.
            HashSet<TilePosition2D> closedSet = new HashSet<TilePosition2D>();
            //All the nodes to check.
            Heap<Node2D> openSet = new Heap<Node2D>(maxNodeCount);
            //Add the first node to the collection of nodes to check.
            openSet.Add(new Node2D(originPos, destinationPos, 0));

            while (openSet.Count > 0)
            {
                //Get the next node to check. The one with the lowest total distance.
                Node2D current = openSet.RemoveFirst();
                //Add the current node to the closed list.
                closedSet.Add(current.position);

                //If the current node is the destination, return the path.
                if (current.position.Equals(destinationPos))
                {
                    List<TilePosition2D> positions = new List<TilePosition2D>();
                    while (current != null)
                    {
                        positions.Add(current.position);
                        current = current.parent;
                    }
                    return new Path2D(map, positions.ToArray());
                }

                //Get the node's neighbour positions.
                int neighbourCount;
                Neighbour[] neighbours = GetNeighbours(current.position, navigationMode, out neighbourCount);
                //Loop through the node's neighbours.
                for (int i = 0; i < neighbourCount; i++)
                {
                    //Skip the neighbour if it is in the closed set.
                    Neighbour neighbour = neighbours[i];
                    if (closedSet.Contains(neighbour.position))
                        continue;

                    //How far is the node from the start?
                    int costFromStart = current.costFromStart + TilePosition2D.GetDistance(neighbour.position, current.position) * neighbour.weight;
                    //If the node doesn't exist, create it.
                    Node2D node;
                    if (!allNodes.TryGetValue(neighbour.position, out node))
                    {
                        node = new Node2D(neighbour.position, destinationPos, costFromStart);
                        allNodes[neighbour.position] = node;
                    }
                    //If the new path to the neighbour is shorter than existing one or if no path exists, make a new path from the current node to the neighbour.
                    bool isInOpen = openSet.Contains(node);
                    if (costFromStart < node.costFromStart || !isInOpen)
                    {
                        node.costFromStart = costFromStart;
                        node.parent = current;

                        //Add the neighbour to the open list if it isn't already.
                        if (!isInOpen)
                            openSet.Add(node);
                        //If it is, update its position because the priority of the node was changed.
                        else
                            openSet.UpdateItem(node);
                    }
                }
            }

            Debug.LogWarning("Failed to find a path from " + originPos.x + "," + originPos.z + " to " + destinationPos.x + "," + destinationPos.z);
            return new Path2D(map);
        }
        /// <summary>
        /// Gets all the neighbouring node positions and weights.
        /// </summary>
        /// <param name="position">The position of the node to find neighbours for.</param>
        /// <param name="mode">What type of navigation is supported by the path finding?</param>
        /// <param name="count">How many neighbours were found.</param>
        /// <returns>All the neighbouring node positions and weights.</returns>
        private Neighbour[] GetNeighbours(TilePosition2D position, NavigationMode mode, out int count)
        {
            Neighbour[] neighbours = new Neighbour[8];
            count = 0;
            for (int x = position.x - 1; x <= position.x + 1; x++)
                for (int z = position.z - 1; z <= position.z + 1; z++)
                {
                    if (x == position.x && z == position.z)
                        continue;

                    Region2D region;
                    TileStack tileStack;
                    if (map.TryGetTileStack(position, out region, out tileStack))
                    {
                        if ((GetSupportedNavigationModes(ref tileStack, region, ref position) & mode) != NavigationMode.None)
                        {
                            int weight = tileStack.ground.GetNavigationWeight(region, position);

                            neighbours[count] = new Neighbour(x, z, weight);
                            count++;
                        }
                    }
                }
            return neighbours;
        }
        /// <summary>
        /// Determines what forms of navigation a tile stack can support.
        /// </summary>
        /// <param name="tileStack">The tile stack to determine navigational support.</param>
        /// <param name="region">The region the tile stack is inside.</param>
        /// <param name="globalTilePosition">The position to determine navigational support.</param>
        /// <returns>What forms of navigation the tile stack can support.</returns>
        private NavigationMode GetSupportedNavigationModes(ref TileStack tileStack, Region2D region, ref TilePosition2D globalTilePosition)
        {
            if (tileStack.interactable != null)
                return tileStack.interactable.GetNavigationSupportedModes(region, globalTilePosition);
            if (tileStack.ground != null)
                return NavigationMode.All;
            return NavigationMode.None;
        }


        /// <summary>
        /// Called by the map navigator handler.
        /// Calculates all the currently requested paths.
        /// </summary>
        internal void ProcessPathRequests()
        {
            while (pathRequests.Count > 0)
            {
                PathRequest2D request;
                lock (pathRequests)
                    request = pathRequests.Dequeue();

                Path2D path = GetPath(request.originPos, request.destinationPos, request.navigationMode);
                request.onPathComplete(path);
            }
        }
        /// <summary>
        /// Adds a request for a path to be created.
        /// </summary>
        /// <param name="originPos">The path's start position.</param>
        /// <param name="destinationPos">The path's destination position.</param>
        /// <param name="navigationMode">The type of navigation to use when path finding.</param>
        /// <param name="onPathComplete">The method to call when the path is created.</param>
        public void RequestPath(TilePosition2D originPos, TilePosition2D destinationPos, NavigationMode navigationMode, PathRequest2D.OnPathCompleteDelegate onPathComplete)
        {
            lock (pathRequests)
                pathRequests.Enqueue(new PathRequest2D(originPos, destinationPos, navigationMode, onPathComplete));
        }

        /// <summary>
        /// A structure to hold a tile position and the navigation weight of the tile at that position.
        /// </summary>
        private struct Neighbour
        {
            public TilePosition2D position;
            public int weight;

            public Neighbour(int x, int z, int weight)
            {
                this.position = new TilePosition2D(x, z);
                this.weight = weight;
            }
        }
    }
}
