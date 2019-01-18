namespace ModLoading
{
    public abstract class ModBase
    {
        /// <summary>
        /// This method should be responsible for registering things like items, effects, etc to the registries.
        /// If this mod overrides any base-game functionality, those overrides should be done before this method ends.
        /// </summary>
        public abstract void PreInit();
        /// <summary>
        /// This method should be responsible for cross-mod interactions such as overriding / modifying functionality or registry entries from other mods.
        /// </summary>
        public abstract void Init();
        /// <summary>
        /// This method should be responsible for gathering references to items, effects, etc from the registries.
        /// By the time this method is called, the game's registries should not change.
        /// </summary>
        public abstract void PostInit();
    }
}
