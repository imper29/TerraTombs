using UnityEngine;
using Utils.Collections;

namespace Maps
{
    /// <summary>
    /// Creates the interactable and ground tile registries.
    /// </summary>
    public class TileRegistry2D : MonoBehaviour
    {
        /// <summary>
        /// The registry containing all the ground tiles in the game.
        /// </summary>
        public static readonly Registry<TileGround2D, string> REGISTRY_GROUND = new Registry<TileGround2D, string>();
        /// <summary>
        /// The registry containing all the interactable tiles in the game.
        /// </summary>
        public static readonly Registry<TileInteractable2D, string> REGISTRY_INTERACTABLE = new Registry<TileInteractable2D, string>();

        /// <summary>
        /// Populates the registries.
        /// </summary>
        private void Awake()
        {
            REGISTRY_GROUND.Clear();
            TileGround2D[] tileGrounds = Resources.LoadAll<TileGround2D>("Tiles/Ground");
            for (int i = 0; i < tileGrounds.Length; i++)
                REGISTRY_GROUND.Add(tileGrounds[i]);

            REGISTRY_INTERACTABLE.Clear();
            TileInteractable2D[] tileInteractables = Resources.LoadAll<TileInteractable2D>("Tiles/Interactable");
            for (int i = 0; i < tileInteractables.Length; i++)
                REGISTRY_INTERACTABLE.Add(tileInteractables[i]);
        }
    }
}
