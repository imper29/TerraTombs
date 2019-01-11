namespace Maps.Navigation
{
    /// <summary>
    /// Determines how a thing should navigate through the world.
    /// </summary>
    public enum NavigationMode
    {
        //The thing cannot do anything.
        None = 0,

        //The thing walk.
        CanWalk = 1 << 0,
        //The thing can fly.
        CanFly = 1 << 1,
        //The thing can pass through solid objects.
        CanPhase = 1 << 2,

        //The thing can do anything.
        All = CanWalk | CanFly | CanPhase
    }
}
