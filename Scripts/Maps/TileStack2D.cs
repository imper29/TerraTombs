namespace Maps
{
    /// <summary>
    /// A struct containing all the tiles and their render objects in a specific tile position.
    /// </summary>
    public struct TileStack
    {
        public TileGround2D ground;
        public object groundRenderObject;
        public TileInteractable2D interactable;
        public object interactableRenderObject;
    }
}
