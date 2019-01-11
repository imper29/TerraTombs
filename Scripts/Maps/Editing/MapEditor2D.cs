using Maps.Rendering;
using UnityEngine;

namespace Maps.Editing
{
    /// <summary>
    /// Allows the player to add and remove tiles from a map.
    /// </summary>
    public class MapEditor2D
    {
        /// <summary>
        /// The map to edit.
        /// </summary>
        private readonly Map2D map;

        /// <summary>
        /// How the map should be modified.
        /// </summary>
        private MapEditingMode editingMode;

        /// <summary>
        /// The ground tile to place.
        /// </summary>
        private TileGround2D ground;
        /// <summary>
        /// The interactable tile to place.
        /// </summary>
        private TileInteractable2D interactable;
        /// <summary>
        /// The blueprint for what is currently in the map editor's clipboard.
        /// </summary>
        private MapBlueprint2D clipboard;
        /// <summary>
        /// The ghost renderer to render tiles being placed.
        /// </summary>
        private AreaGhostRenderer2D ghostRenderer;


        /// <summary>
        /// Creates a disabled map editor.
        /// </summary>
        /// <param name="map">The map to edit.</param>
        public MapEditor2D(Map2D map)
        {
            this.map = map;
        }


        /// <summary>
        /// Enables this editor.
        /// </summary>
        public void Enable()
        {
            if (MapEditorHandler2D.GetActiveMapEditor() != this)
            {
                MouseHandler.OnButtonClicked += MouseHandler_OnButtonClicked;
                MouseHandler.OnButtonDragged += MouseHandler_OnButtonDragged;
                MouseHandler.OnButtonReleased += MouseHandler_OnButtonReleased;
                MouseHandler.OnMouseMoved += MouseHandler_OnMouseMoved;

                MapEditorHandler2D.OnMapEditorEnabled(this);
            }
        }
        /// <summary>
        /// Disables this editor.
        /// </summary>
        public void Disable()
        {
            if (MapEditorHandler2D.GetActiveMapEditor() == this)
            {
                MouseHandler.OnButtonClicked -= MouseHandler_OnButtonClicked;
                MouseHandler.OnButtonDragged -= MouseHandler_OnButtonDragged;
                MouseHandler.OnButtonReleased -= MouseHandler_OnButtonReleased;
                MouseHandler.OnMouseMoved -= MouseHandler_OnMouseMoved;

                MapEditorHandler2D.OnMapEditorDisbled(this);
            }
        }

        /// <summary>
        /// Sets the tile this editor will place.
        /// </summary>
        /// <param name="tile">The tile this editor will place.</param>
        public void SetTile(TileGround2D tile)
        {
            ground = tile;
            interactable = null;
        }
        /// <summary>
        /// Sets the tile this editor will place.
        /// </summary>
        /// <param name="tile">The tile this editor will place.</param>
        public void SetTile(TileInteractable2D tile)
        {
            interactable = tile;
            ground = null;
        }
        /// <summary>
        /// Sets this map editor's editing mode.
        /// </summary>
        /// <param name="editingMode">The new editing mode.</param>
        public void SetEditingMode(MapEditingMode editingMode)
        {
            this.editingMode = editingMode;
        }



        private void MouseHandler_OnButtonClicked(ref MouseState mouseState, MouseButton buttons)
        {
            //Mouse is over a gui element so ignore the mouse event.
            if (mouseState.IsOverGUI())
                return;

            //Only the left button is pressed!
            if (mouseState.GetHeldButtons() == MouseButton.Left)
            {
                //Painting so just place a tile.
                if (editingMode == MapEditingMode.Paint)
                    PlaceTile(MapHelper2D.GetTilePosition(Camera.main, mouseState.GetCurrentMousePosition()));
                //Copy pasting so just paste the clipboard.
                else if (editingMode == MapEditingMode.CopyPaste && clipboard != null)
                    clipboard.PasteReplaceAll(map, MapHelper2D.GetTilePosition(Camera.main, mouseState.GetCurrentMousePosition()));
            }
            //Only the right button is pressed!
            if (mouseState.GetHeldButtons() == MouseButton.Right)
            {
                //Copying a region so don't render the clipboard.
                if (editingMode == MapEditingMode.CopyPaste)
                {
                    if (ghostRenderer != null)
                    {
                        ghostRenderer.Unrender(MapHelper2D.GetTilePosition(Camera.main, mouseState.GetCurrentMousePosition()));
                        ghostRenderer = null;
                    }
                }
                //Erasing a tile so don't render the editor's selected tile.
                else if (editingMode == MapEditingMode.Paint)
                    RemoveTile(MapHelper2D.GetTilePosition(Camera.main, mouseState.GetCurrentMousePosition()));
            }
        }
        private void MouseHandler_OnButtonDragged(ref MouseState mouseState, MouseButton buttons)
        {
            //Mouse is over a gui element so ignore the mouse event.
            if (mouseState.IsOverGUI())
                return;

            //Only the left button is pressed!
            if (mouseState.GetHeldButtons() == MouseButton.Left)
            {
                //Painting so just place a tile.
                if (editingMode == MapEditingMode.Paint)
                    PlaceTile(MapHelper2D.GetTilePosition(Camera.main, mouseState.GetCurrentMousePosition()));
            }
            //Only the right button is pressed!
            else if (editingMode == MapEditingMode.Paint && mouseState.GetHeldButtons() == MouseButton.Right)
                //Painting so just remove a tile.
                RemoveTile(MapHelper2D.GetTilePosition(Camera.main, mouseState.GetCurrentMousePosition()));
        }
        private void MouseHandler_OnButtonReleased(ref MouseState mouseState, MouseButton buttons)
        {
            //Mouse is over a gui element so ignore the mouse event.
            if (mouseState.IsOverGUI())
                return;

            if (mouseState.GetHeldButtons() == MouseButton.None)
            {
                //Only the left button was pressed!
                if (buttons == MouseButton.Left)
                {
                    //Filling so just place tiles from the click position to the release position.
                    if (editingMode == MapEditingMode.Fill)
                    {
                        TilePosition2D clickPos = MapHelper2D.GetTilePosition(Camera.main, mouseState.GetClickedPosition(MouseButton.Left));
                        TilePosition2D curPos = MapHelper2D.GetTilePosition(Camera.main, mouseState.GetCurrentMousePosition());
                        TilePosition2D minPos = TilePosition2D.GetMinPosition(clickPos, curPos);
                        TilePosition2D maxPos = TilePosition2D.GetMaxPosition(clickPos, curPos);

                        for (int x = minPos.x; x <= maxPos.x; x++)
                            for (int z = minPos.z; z <= maxPos.z; z++)
                                PlaceTile(new TilePosition2D(x, z));
                    }
                }
                //Only the right button was pressed!
                else if (buttons == MouseButton.Right)
                {
                    //Filling so just remove tiles from the click position to the release position.
                    if (editingMode == MapEditingMode.Fill)
                    {
                        TilePosition2D clickPos = MapHelper2D.GetTilePosition(Camera.main, mouseState.GetClickedPosition(MouseButton.Right));
                        TilePosition2D curPos = MapHelper2D.GetTilePosition(Camera.main, mouseState.GetCurrentMousePosition());
                        TilePosition2D minPos = TilePosition2D.GetMinPosition(clickPos, curPos);
                        TilePosition2D maxPos = TilePosition2D.GetMaxPosition(clickPos, curPos);

                        for (int x = minPos.x; x <= maxPos.x; x++)
                            for (int z = minPos.z; z <= maxPos.z; z++)
                                RemoveTile(new TilePosition2D(x, z));
                    }
                    //Copy pasting so just create a blueprint from the click position to the release position.
                    else if (editingMode == MapEditingMode.CopyPaste)
                    {
                        TilePosition2D clickPos = MapHelper2D.GetTilePosition(Camera.main, mouseState.GetClickedPosition(MouseButton.Right));
                        TilePosition2D curPos = MapHelper2D.GetTilePosition(Camera.main, mouseState.GetCurrentMousePosition());

                        TilePosition2D minPos = TilePosition2D.GetMinPosition(clickPos, curPos);
                        TilePosition2D size = TilePosition2D.GetMaxPosition(clickPos, curPos) - minPos + new TilePosition2D(1, 1);
                        clipboard = new MapBlueprint2D(map, minPos, size);

                        //Render the new clipboard as a ghost.
                        TilePosition2D currentFramePos = MapHelper2D.GetTilePosition(Camera.main, mouseState.GetCurrentMousePosition());
                        TileStack[,] tiles = clipboard.GetTiles();
                        ghostRenderer = new AreaGhostRenderer2D(tiles);
                        ghostRenderer.Render(currentFramePos);
                    }
                }
            }
        }
        private void MouseHandler_OnMouseMoved(ref MouseState mouseState)
        {
            //Mouse is over a gui element so ignore the mouse event.
            if (mouseState.IsOverGUI())
            {
                if (ghostRenderer != null)
                {
                    ghostRenderer.Unrender(MapHelper2D.GetTilePosition(Camera.main, mouseState.GetCurrentMousePosition()));
                    ghostRenderer = null;
                }
                return;
            }

            TilePosition2D lastFramePos = MapHelper2D.GetTilePosition(Camera.main, mouseState.GetLastMousePosition());
            TilePosition2D currentFramePos = MapHelper2D.GetTilePosition(Camera.main, mouseState.GetCurrentMousePosition());

            //The mouse tile position has moved since the last frame.
            if (!lastFramePos.Equals(currentFramePos))
            {
                //Unrender the old ghost renderer.
                if (ghostRenderer != null)
                    ghostRenderer.Unrender(lastFramePos);

                //Create and render a new ghost renderer.
                if (editingMode == MapEditingMode.CopyPaste)
                {
                    if (clipboard != null && mouseState.GetHeldButtons() != MouseButton.Right)
                    {
                        TileStack[,] tiles = clipboard.GetTiles();
                        ghostRenderer = new AreaGhostRenderer2D(tiles);
                        ghostRenderer.Render(currentFramePos);
                    }
                }
                else if (editingMode == MapEditingMode.Fill)
                {
                    if (mouseState.GetHeldButtons() == MouseButton.Left)
                    {
                        TilePosition2D clickPos = MapHelper2D.GetTilePosition(Camera.main, mouseState.GetClickedPosition(MouseButton.Left));
                        TilePosition2D min = TilePosition2D.GetMinPosition(clickPos, currentFramePos);
                        TilePosition2D max = TilePosition2D.GetMaxPosition(clickPos, currentFramePos);
                        TilePosition2D size = max - min;

                        TileStack[,] tiles = new TileStack[size.x + 1, size.z + 1];
                        for (int x = 0; x <= size.x; x++)
                            for (int z = 0; z <= size.z; z++)
                            {
                                tiles[x, z].ground = ground;
                                tiles[x, z].interactable = interactable;
                            }
                        ghostRenderer = new AreaGhostRenderer2D(tiles);
                        ghostRenderer.Render(min);
                    }
                    else if(mouseState.GetHeldButtons() == MouseButton.None)
                    {
                        TilePosition2D mousePos = MapHelper2D.GetTilePosition(Camera.main, mouseState.GetCurrentMousePosition());
                        TileStack[,] tiles = new TileStack[1, 1];
                        tiles[0, 0].ground = ground;
                        tiles[0, 0].interactable = interactable;
                        ghostRenderer = new AreaGhostRenderer2D(tiles);
                        ghostRenderer.Render(mousePos);
                    }
                }
                else if (editingMode == MapEditingMode.Paint)
                {
                    TileStack[,] tiles = new TileStack[1, 1];
                    tiles[0, 0].ground = ground;
                    tiles[0, 0].interactable = interactable;
                    ghostRenderer = new AreaGhostRenderer2D(tiles);
                    ghostRenderer.Render(currentFramePos);
                }
            }
        }

        private void PlaceTile(TilePosition2D globalTilePosition)
        {
            if (ground != null)
            {
                if (map.GetGround(globalTilePosition) != ground)
                    map.PlaceGround(globalTilePosition, ground);
            }
            else if (interactable != null && map.GetInteractable(globalTilePosition) != interactable)
                map.PlaceInteractable(globalTilePosition, interactable);
        }
        private void RemoveTile(TilePosition2D globalTilePosition)
        {
            if (ground != null)
                map.RemoveGround(globalTilePosition);
            else if (interactable != null)
                map.RemoveInteractable(globalTilePosition);
        }
    }
}
