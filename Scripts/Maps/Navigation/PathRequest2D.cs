namespace Maps.Navigation
{
    public struct PathRequest2D
    {
        /// <summary>
        /// A delegate used for when a path is completely calculated.
        /// </summary>
        /// <param name="path">The path that was calculated.</param>
        public delegate void OnPathCompleteDelegate(Path2D path);

        public PathRequest2D(TilePosition2D originPos, TilePosition2D destinationPos, NavigationMode navigationMode, OnPathCompleteDelegate onPathComplete)
        {
            this.originPos = originPos;
            this.destinationPos = destinationPos;
            this.navigationMode = navigationMode;
            this.onPathComplete = onPathComplete;
        }

        public readonly TilePosition2D originPos;
        public readonly TilePosition2D destinationPos;
        public readonly NavigationMode navigationMode;
        public readonly OnPathCompleteDelegate onPathComplete;
    }
}