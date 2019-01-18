using Utils.Collections;

namespace Maps.Navigation
{
    internal class Node2D : IHeapEntry<Node2D>
    {
        /// <summary>
        /// The position of this node.
        /// </summary>
        public readonly TilePosition2D position;
        /// <summary>
        /// Current minimum distance to start destination.
        /// </summary>
        public int costFromStart;
        /// <summary>
        /// Estimated distance to end destination.
        /// </summary>
        public int costToEnd;
        /// <summary>
        /// The node that leads to this node.
        /// </summary>
        public Node2D parent;


        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <param name="position">The position of this node.</param>
        /// <param name="destination">The position of the destination node.</param>
        /// <param name="costFromStart">The cost from the start.</param>
        public Node2D(TilePosition2D position, TilePosition2D destination, int costFromStart)
        {
            this.position = position;
            this.costFromStart = costFromStart;
            costToEnd = TilePosition2D.GetDistance(position, destination);
        }

        
        /// <summary>
        /// The total cost of the node. Used in pathfinding as a weight.
        /// </summary>
        public int CostTotal
        {
            get
            {
                return costToEnd + costFromStart;
            }
        }
        /// <summary>
        /// The index of this node in the heap.
        /// </summary>
        public int HeapIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Used for heap sorting.
        /// </summary>
        /// <param name="other">The other node.</param>
        /// <returns>1 if the other node is further from a solution than this node. 0 if they are equal. -1 if the other node is closer to a solution than this node.</returns>
        public int CompareTo(Node2D other)
        {
            int comp = CostTotal.CompareTo(other.CostTotal);
            if (comp == 0)
                comp = costToEnd.CompareTo(other.costToEnd);
            return -comp;
        }
    }
}
