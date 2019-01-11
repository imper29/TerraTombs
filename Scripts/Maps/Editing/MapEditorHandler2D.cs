using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Maps.Editing
{
    /// <summary>
    /// Does all the gui related stuff for the currently active map editor.
    /// </summary>
    public class MapEditorHandler2D : MonoBehaviour
    {
        private static MapEditorHandler2D Instance;
        /// <summary>
        /// The active map editor.
        /// </summary>
        private static MapEditor2D activeMapEditor;

        [SerializeField]
        private RectTransform interactableTilesPanel, groundTilesPanel;
        [SerializeField]
        private RectTransform prefab;

        private Button[] groundTileButtons;
        private Button[] interactableTileButtons;

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            //Loop through all the tiles in the ground tile registry and create buttons for each of them.
            TileGround2D[] groundTiles = TileRegistry2D.REGISTRY_GROUND.GetValues();
            groundTileButtons = new Button[groundTiles.Length];
            for (int i = 0; i < groundTiles.Length; i++)
            {
                int index = i;
                TileGround2D tile = groundTiles[i];

                GameObject obj = Instantiate(prefab, groundTilesPanel).gameObject;

                obj.GetComponent<MeshUI>().SetDisplay(groundTiles[i].GetUiMesh(), groundTiles[i].GetUiMaterial(), 90f);
                obj.GetComponentInChildren<Text>().text = LocalizationHandler.GetLocalizedText(groundTiles[i].GetUnlocalizedName());
                obj.SetActive(true);

                groundTileButtons[i] = obj.GetComponent<Button>();
                groundTileButtons[i].onClick.AddListener(() => { GetActiveMapEditor()?.SetTile(tile); SetGroundSelection(index); });
            }

            //Loop through all the tiles in the interactable tile registry and create buttons for each of them.
            TileInteractable2D[] interactableTiles = TileRegistry2D.REGISTRY_INTERACTABLE.GetValues();
            interactableTileButtons = new Button[interactableTiles.Length];
            for (int i = 0; i < interactableTiles.Length; i++)
            {
                int index = i;
                TileInteractable2D tile = interactableTiles[i];

                GameObject obj = Instantiate(prefab, interactableTilesPanel).gameObject;

                obj.GetComponent<MeshUI>().SetDisplay(interactableTiles[i].GetUiMesh(), interactableTiles[i].GetUiMaterial(), 90f);
                obj.GetComponentInChildren<Text>().text = LocalizationHandler.GetLocalizedText(interactableTiles[i].GetUnlocalizedName());
                obj.SetActive(true);

                interactableTileButtons[i] = obj.GetComponent<Button>();
                interactableTileButtons[i].onClick.AddListener(() => { GetActiveMapEditor()?.SetTile(tile); SetInteractableSelection(index); });
            }
        }

        public void SetEditModePaint()
        {
            activeMapEditor?.SetEditingMode(MapEditingMode.Paint);
        }
        public void SetEditModeFill()
        {
            activeMapEditor?.SetEditingMode(MapEditingMode.Fill);
        }
        public void SetEditModeCopyPaste()
        {
            activeMapEditor?.SetEditingMode(MapEditingMode.CopyPaste);
        }

        /// <summary>
        /// Called when a map editor is enabled.
        /// Sets which map editor is active.
        /// </summary>
        /// <param name="_mapEditor">The map editor that was enabled.</param>
        public static void OnMapEditorEnabled(MapEditor2D _mapEditor)
        {
            if (activeMapEditor != null)
                activeMapEditor.Disable();

            activeMapEditor = _mapEditor;
        }
        /// <summary>
        /// Called when a map editor is disabled.
        /// If the active map editor was disabled, the active map editor is set to null.
        /// </summary>
        /// <param name="_mapEditor">The map editor that was disabled.</param>
        public static void OnMapEditorDisbled(MapEditor2D _mapEditor)
        {
            if (_mapEditor == activeMapEditor)
                activeMapEditor = null;
        }
        /// <summary>
        /// Gets the active map editor.
        /// </summary>
        /// <returns>The map editor that is active.</returns>
        public static MapEditor2D GetActiveMapEditor()
        {
            return activeMapEditor;
        }
        
        private void SetGroundSelection(int index)
        {
            for (int i = 0; i < groundTileButtons.Length; i++)
                groundTileButtons[i].interactable = true;
            groundTileButtons[index].interactable = false;
        }
        private void SetInteractableSelection(int index)
        {
            for (int i = 0; i < interactableTileButtons.Length; i++)
                interactableTileButtons[i].interactable = true;
            interactableTileButtons[index].interactable = false;
        }
    }
}
