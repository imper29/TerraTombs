using System.Collections.Generic;
using System.IO;
using Utils.Collections;

namespace Maps.Editing
{
    public class MapBlueprint2D
    {
        /// <summary>
        /// How big the blueprint is in tiles.
        /// </summary>
        private TilePosition2D size;
        /// <summary>
        /// The tiles in this blueprint.
        /// </summary>
        private TileStack[,] tiles;
        /// <summary>
        /// The tile entities in this blueprint.
        /// </summary>
        private Dictionary<TilePosition2D, ITileEntity2D> tileEntities;


        /// <summary>
        /// Creates an empty blueprint.
        /// </summary>
        public MapBlueprint2D()
        {
            size = new TilePosition2D(0, 0);
            tiles = new TileStack[0, 0];
            tileEntities = new Dictionary<TilePosition2D, ITileEntity2D>();
        }
        /// <summary>
        /// Creates a blueprint of an area of a map.
        /// </summary>
        /// <param name="map">The map to blueprint.</param>
        /// <param name="globalTilePosition">The position to blueprint.</param>
        /// <param name="size">The size of the area to blueprint.</param>
        public MapBlueprint2D(Map2D map, TilePosition2D globalTilePosition, TilePosition2D size)
        {
            this.size = size;
            tiles = new TileStack[size.x, size.z];
            tileEntities = new Dictionary<TilePosition2D, ITileEntity2D>();


            //Copy all the tiles and tile entities from the map into this blueprint.
            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++)
                {
                    TilePosition2D tilePosition = new TilePosition2D(x + globalTilePosition.x, z + globalTilePosition.z);
                    //Get the tile stack.
                    Region2D region;
                    if (map.TryGetTileStack(tilePosition, out region, out tiles[x, z]) && tiles[x, z].interactable != null)
                    {
                        TilePosition2D localTilePosition = region.GetPosition().GetLocalTilePosition(tilePosition);
                        //If the tile stack exists and an interactable tile exists, check for a tile entity.
                        ITileEntity2D e;
                        if (region.TryGetTileEntity(localTilePosition, out e))
                        {
                            //The tile entity exists so copy it.
                            tileEntities.Add(tilePosition, e.Duplicate(region, tilePosition));
                        }
                    }
                }
        }


        /// <summary>
        /// Gets all the tiles in this blueprint.
        /// </summary>
        /// <returns>The tiles in this blueprint.</returns>
        public TileStack[,] GetTiles()
        {
            return tiles;
        }

        /// <summary>
        /// Pastes thie blueprint onto a map.
        /// Removes all the tiles from the map then places the blueprint.
        /// </summary>
        /// <param name="map">The map the paste the blueprint onto.</param>
        /// <param name="pastePosition">The position to paste the blueprint.</param>
        public void PasteReplaceAll(Map2D map, TilePosition2D pastePosition)
        {
            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++)
                {
                    TilePosition2D globalTilePosition = new TilePosition2D(x + pastePosition.x, z + pastePosition.z);
                    TileStack stack = tiles[x, z];

                    //Remove the tiles at the current position from the map.
                    map.RemoveGround(globalTilePosition);
                    map.RemoveInteractable(globalTilePosition);

                    //Place the ground tile onto the map.
                    if (stack.ground != null)
                        map.PlaceGround(globalTilePosition, stack.ground);
                    //Place the interactable tile onto the map.
                    if (stack.interactable != null)
                    {
                        map.PlaceInteractable(globalTilePosition, stack.interactable);

                        //Export the tile entity values from the blueprint onto the map.
                        ITileEntity2D blueE, mapE;
                        if (tileEntities.TryGetValue(globalTilePosition, out blueE) && map.TryGetTileEntity(globalTilePosition, out mapE))
                            mapE.ImportValues(blueE);
                    }
                }
        }
        /// <summary>
        /// Pastes thie blueprint onto a map.
        /// Only places the blueprint tiles where there is air on the map.
        /// </summary>
        /// <param name="map">The map the paste the blueprint onto.</param>
        /// <param name="pastePosition">The position to paste the blueprint.</param>
        public void PasteReplaceAir(Map2D map, TilePosition2D pastePosition)
        {
            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++)
                {
                    TilePosition2D globalTilePosition = new TilePosition2D(x + pastePosition.x, z + pastePosition.z);
                    TileStack stack = tiles[x, z];

                    TileStack ts;
                    //Remove the tiles at the current position from the map.
                    if (map.TryGetTileStack(globalTilePosition, out ts))
                    {
                        //Place the ground tile onto the map.
                        if (ts.ground == null)
                            if (stack.ground != null)
                                map.PlaceGround(globalTilePosition, stack.ground);

                        //Place the interactable tile onto the map.
                        if (ts.interactable == null)
                            if (stack.interactable != null)
                            {
                                map.PlaceInteractable(globalTilePosition, stack.interactable);

                                //Export the tile entity values from the blueprint onto the map.
                                ITileEntity2D blueE, mapE;
                                if (tileEntities.TryGetValue(globalTilePosition, out blueE) && map.TryGetTileEntity(globalTilePosition, out mapE))
                                    mapE.ImportValues(blueE);
                            }
                    }
                    else
                    {
                        //Place the ground tile onto the map.
                        if (stack.ground != null)
                            map.PlaceGround(globalTilePosition, stack.ground);
                        //Place the interactable tile onto the map.
                        if (stack.interactable != null)
                        {
                            map.PlaceInteractable(globalTilePosition, stack.interactable);

                            //Export the tile entity values from the blueprint onto the map.
                            ITileEntity2D blueE, mapE;
                            if (tileEntities.TryGetValue(globalTilePosition, out blueE) && map.TryGetTileEntity(globalTilePosition, out mapE))
                                mapE.ImportValues(blueE);
                        }
                    }
                }
        }
        /// <summary>
        /// Pastes thie blueprint onto a map.
        /// Replaces any tiles that are in the way of the blueprint's tiles with the blueprint's tiles.
        /// </summary>
        /// <param name="map">The map the paste the blueprint onto.</param>
        /// <param name="pastePosition">The position to paste the blueprint.</param>
        public void PasteReplaceOverlap(Map2D map, TilePosition2D pastePosition)
        {
            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++)
                {
                    TilePosition2D globalTilePosition = new TilePosition2D(x + pastePosition.x, z + pastePosition.z);
                    TileStack stack = tiles[x, z];
                    
                    //Place the ground tile onto the map.
                    if (stack.ground != null)
                        map.PlaceGround(globalTilePosition, stack.ground);
                    //Place the interactable tile onto the map.
                    if (stack.interactable != null)
                    {
                        map.PlaceInteractable(globalTilePosition, stack.interactable);

                        //Export the tile entity values from the blueprint onto the map.
                        ITileEntity2D blueE, mapE;
                        if (tileEntities.TryGetValue(globalTilePosition, out blueE) && map.TryGetTileEntity(globalTilePosition, out mapE))
                            mapE.ImportValues(blueE);
                    }
                }
        }



        /// <summary>
        /// Exports a blueprint.
        /// </summary>
        /// <param name="writer">The stream to export into.</param>
        /// <param name="groundRegistry">The dynamic registry that holds all the ground tiles.</param>
        /// <param name="interactableRegistry">The dynamic registry that holds all the intertactable tiles.</param>
        public void Export(BinaryWriter writer)
        {
            DynamicRegistry<TileGround2D, string> groundRegistry = new DynamicRegistry<TileGround2D, string>(TileRegistry2D.REGISTRY_GROUND);
            groundRegistry.Export(writer, (BinaryWriter w, string str) => w.Write(str));
            DynamicRegistry<TileInteractable2D, string> interactableRegistry = new DynamicRegistry<TileInteractable2D, string>(TileRegistry2D.REGISTRY_INTERACTABLE);
            interactableRegistry.Export(writer, (BinaryWriter w, string str) => w.Write(str));

            writer.Write(size.x);
            writer.Write(size.z);

            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++)
                {
                    TilePosition2D tp = new TilePosition2D(x, z);

                    //Export ground.
                    TileGround2D ground = tiles[x, z].ground;
                    writer.Write(groundRegistry.GetId(ground));

                    //Export interactable.
                    TileInteractable2D interactable = tiles[x, z].interactable;
                    writer.Write(interactableRegistry.GetId(interactable));
                    if (interactable != null)
                    {
                        //Export tile entity.
                        ITileEntity2D tileEntity;
                        if (tileEntities.TryGetValue(tp, out tileEntity))
                            tileEntity.Export(writer);
                    }
                }
        }
        /// <summary>
        /// Imports a blueprint.
        /// </summary>
        /// <param name="reader">The stream to import from.</param>
        /// <param name="groundRegistry">The dynamic registry that holds all the ground tiles.</param>
        /// <param name="interactableRegistry">The dynamic registry that holds all the intertactable tiles.</param>
        public void Import(BinaryReader reader)
        {
            DynamicRegistry<TileGround2D, string> groundRegistry = new DynamicRegistry<TileGround2D, string>();
            groundRegistry.Import(reader, (BinaryReader r) => { return r.ReadString(); }, TileRegistry2D.REGISTRY_GROUND);
            DynamicRegistry<TileInteractable2D, string> interactableRegistry = new DynamicRegistry<TileInteractable2D, string>();
            interactableRegistry.Import(reader, (BinaryReader r) => { return r.ReadString(); }, TileRegistry2D.REGISTRY_INTERACTABLE);

            size = new TilePosition2D(reader.ReadInt32(), reader.ReadInt32());
            tiles = new TileStack[size.x, size.z];
            tileEntities.Clear();

            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++)
                {
                    //Import ground.
                    TileGround2D ground = groundRegistry.GetValue(reader.ReadUInt16());
                    if (ground != null)
                        tiles[x, z].ground = ground;

                    //Import interactable.
                    TileInteractable2D interactable = interactableRegistry.GetValue(reader.ReadUInt16());
                    if (interactable != null)
                    {
                        tiles[x, z].interactable = interactable;

                        TilePosition2D tp = new TilePosition2D(x, z);
                        ITileEntity2D e = interactable.CreateTileEntity(null, tp);
                        //Import tile entity.
                        if (e != null)
                        {
                            tileEntities.Add(tp, e);
                            e.Import(reader);
                        }
                    }
                }
        }
    }
}
