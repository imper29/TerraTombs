//Uncomment the #define DEBUGGING to enable debug logs.
//#define DEBUGGING

using System.Collections.Generic;
using System.IO;
using Utils.Collections;
#if DEBUGGING
using UnityEngine;
#endif

namespace Maps
{
    /// <summary>
    /// A square area that contains a bunch of tiles.
    /// Maps are collections of regions.
    /// </summary>
    public class Region2D
    {
        /// <summary>
        /// The map that contains this region.
        /// </summary>
        private readonly Map2D map;
        /// <summary>
        /// The position of this region.
        /// </summary>
        private readonly RegionPosition2D position;
        /// <summary>
        /// The tiles in this region.
        /// </summary>
        private readonly TileStack[,] tiles;
        /// <summary>
        /// The tile entities in this region.
        /// </summary>
        private readonly Dictionary<TilePosition2D, ITileEntity2D> tileEntities;


        /// <summary>
        /// Creates a new region at a specific position.
        /// </summary>
        /// <param name="position">The position of the new region.</param>
        public Region2D(Map2D map, RegionPosition2D position)
        {
            this.map = map;
            this.position = position;
            tiles = new TileStack[RegionPosition2D.REGION_SIZE, RegionPosition2D.REGION_SIZE];
            tileEntities = new Dictionary<TilePosition2D, ITileEntity2D>();
        }


        /// <summary>
        /// Gets the map that contains this region.
        /// </summary>
        /// <returns>The map that contains this region.</returns>
        public Map2D GetMap()
        {
            return map;
        }
        /// <summary>
        /// Gets the position of this region.
        /// </summary>
        /// <returns>The position of this region.</returns>
        public RegionPosition2D GetPosition()
        {
            return position;
        }
        /// <summary>
        /// Gets the 2D array containing all the tiles.
        /// </summary>
        /// <returns>The 2D array containing all the tiles.</returns>
        public TileStack[,] GetTileStacks()
        {
            return tiles;
        }
        /// <summary>
        /// Gets the entire tilestack at localTilePosition.
        /// </summary>
        /// <param name="localTilePosition">The position of the tilestack to get.</param>
        /// <returns>The entire tilestack at localTilePosition.</returns>
        public TileStack GetTileStack(TilePosition2D localTilePosition)
        {
            return tiles[localTilePosition.x, localTilePosition.z];
        }


        /// <summary>
        /// Places a ground tile at localTilePosition.
        /// If a ground tile already exists in that position, the ground tile is first removed.
        /// </summary>
        /// <param name="localTilePosition">Where to place the ground tile.</param>
        /// <param name="tile">The ground tile to place.</param>
        public void PlaceGround(TilePosition2D localTilePosition, TileGround2D tile)
        {
            //Remove the existing tile.
            if (tiles[localTilePosition.x, localTilePosition.z].ground != null)
                RemoveGround(localTilePosition);

            //Place the new tile.
            tiles[localTilePosition.x, localTilePosition.z].ground = tile;
            tile.OnPlaced(this, localTilePosition);
            //Call the OnTilePlaced map event.
            map?.OnGroundPlaced(this, position.GetGlobalTilePosition(localTilePosition), tile);
        }
        /// <summary>
        /// Gets the ground tile at localTilePosition.
        /// If no ground tile exists at the position, null is returned.
        /// </summary>
        /// <param name="localTilePosition">The position of the ground tile to find.</param>
        /// <returns>The ground tile at localTilePosition.</returns>
        public TileGround2D GetGround(TilePosition2D localTilePosition)
        {
            return tiles[localTilePosition.x, localTilePosition.z].ground;
        }
        /// <summary>
        /// Removes the ground tile at localTilePosition.
        /// </summary>
        /// <param name="localTilePosition">The position of the ground tile to remove.</param>
        public void RemoveGround(TilePosition2D localTilePosition)
        {
            //Get the tile.
            TileGround2D tile = tiles[localTilePosition.x, localTilePosition.z].ground;

            //Remove the tile.
            if (tile != null)
            {
                tile.OnRemoved(this, localTilePosition);
                tiles[localTilePosition.x, localTilePosition.z].ground = null;
                //Call the OnTileRemoved map event.
                map?.OnGroundRemoved(this, position.GetGlobalTilePosition(localTilePosition), tile);
            }
        }


        /// <summary>
        /// Places an interactable tile at localTilePosition.
        /// If an interactable tile already exists in that position, the interactable tile is first removed.
        /// </summary>
        /// <param name="localTilePosition">Where to place the interactable tile.</param>
        /// <param name="tile">The interactable tile to place.</param>
        public ITileEntity2D PlaceInteractable(TilePosition2D localTilePosition, TileInteractable2D tile)
        {
            //Remove the existing tile.
            if (tiles[localTilePosition.x, localTilePosition.z].ground != null)
                RemoveInteractable(localTilePosition);

            //Place the new tile.
            tiles[localTilePosition.x, localTilePosition.z].interactable = tile;
            tile.OnPlaced(this, localTilePosition);

            //Create the interactable tile's tile entity.
            ITileEntity2D tileEntity = tile.CreateTileEntity(this, localTilePosition);
            if (tileEntity != null)
            {
                tileEntities.Add(localTilePosition, tileEntity);
                tileEntity.OnPlaced(this, localTilePosition);
            }

            //Call the OnTilePlaced map event.
            map?.OnInteractablePlaced(this, position.GetGlobalTilePosition(localTilePosition), tile);

            return tileEntity;
        }
        /// <summary>
        /// Gets the interactable tile at localTilePosition.
        /// If no interactable tile exists at the position, null is returned.
        /// </summary>
        /// <param name="localTilePosition">The position of the interactable tile to find.</param>
        /// <returns>The interactable tile at localTilePosition.</returns>
        public TileInteractable2D GetInteractable(TilePosition2D localTilePosition)
        {
            return tiles[localTilePosition.x, localTilePosition.z].interactable;
        }
        /// <summary>
        /// Removes the interactable tile at localTilePosition.
        /// </summary>
        /// <param name="localTilePosition">The position of the interactable tile to remove.</param>
        public void RemoveInteractable(TilePosition2D localTilePosition)
        {
            //Get the tile.
            TileInteractable2D tile = tiles[localTilePosition.x, localTilePosition.z].interactable;

            //Remove the tile.
            if (tile != null)
            {
                TilePosition2D globalTilePosition = position.GetGlobalTilePosition(localTilePosition);

                //Remove the tile entity.
                ITileEntity2D tileEntity;
                if (tileEntities.TryGetValue(localTilePosition, out tileEntity))
                {
                    tileEntity.OnRemoved(this, globalTilePosition);
                    tileEntities.Remove(localTilePosition);
                }

                tile.OnRemoved(this, localTilePosition);
                tiles[localTilePosition.x, localTilePosition.z].ground = null;
                //Call the OnTileRemoved map event.
                map?.OnInteractableRemoved(this, globalTilePosition, tile);
            }
        }
        

        /// <summary>
        /// Gets the tile entity at localTilePosition.
        /// If no tile entity exists at the position, null is returned.
        /// </summary>
        /// <param name="localTilePosition">The position of the tile entity to find.</param>
        /// <returns>The tile entity at localTilePosition</returns>
        public ITileEntity2D GetTileEntity(TilePosition2D localTilePosition)
        {
            return tileEntities.ContainsKey(localTilePosition) ? tileEntities[localTilePosition] : null;
        }
        /// <summary>
        /// Gets the tile entity at localTilePosition.
        /// </summary>
        /// <param name="localTilePosition">The position of the tile entity to find.</param>
        /// <param name="tileEntity">The tile entity to find.</param>
        /// <returns>True if a tile entity was found. False otherwise.</returns>
        public bool TryGetTileEntity(TilePosition2D localTilePosition, out ITileEntity2D tileEntity)
        {
            return tileEntities.TryGetValue(localTilePosition, out tileEntity);
        }


        /// <summary>
        /// Exports this region.
        /// </summary>
        /// <param name="writer">The stream to export into.</param>
        /// <param name="groundRegistry">The dynamic registry that holds all the ground tiles.</param>
        /// <param name="interactableRegistry">The dynamic registry that holds all the intertactable tiles.</param>
        public void Export(BinaryWriter writer, DynamicRegistry<TileGround2D, string> groundRegistry, DynamicRegistry<TileInteractable2D, string> interactableRegistry)
        {
            for (int x = 0; x < RegionPosition2D.REGION_SIZE; x++)
                for (int z = 0; z < RegionPosition2D.REGION_SIZE; z++)
                {
                    TilePosition2D tp = new TilePosition2D(x, z);

                    //Export ground.
                    TileGround2D ground = GetGround(tp);
                    writer.Write(groundRegistry.GetId(ground));

                    //Export interactable.
                    TileInteractable2D interactable = GetInteractable(tp);
                    writer.Write(interactableRegistry.GetId(interactable));
                    if (interactable != null)
                    {
                        //Export tile entity.
                        ITileEntity2D tileEntity = GetTileEntity(tp);
                        if (tileEntity != null)
                            tileEntity.Export(writer);
                    }
                }
        }
        /// <summary>
        /// Imports this region.
        /// </summary>
        /// <param name="reader">The stream to import from.</param>
        /// <param name="groundRegistry">The dynamic registry that holds all the ground tiles.</param>
        /// <param name="interactableRegistry">The dynamic registry that holds all the intertactable tiles.</param>
        public void Import(BinaryReader reader, DynamicRegistry<TileGround2D, string> groundRegistry, DynamicRegistry<TileInteractable2D, string> interactableRegistry)
        {
            for (int x = 0; x < RegionPosition2D.REGION_SIZE; x++)
                for (int z = 0; z < RegionPosition2D.REGION_SIZE; z++)
                {
                    TilePosition2D tp = new TilePosition2D(x, z);

                    //Import ground.
                    TileGround2D ground = groundRegistry.GetValue(reader.ReadUInt16());
                    if (ground != null)
                        PlaceGround(tp, ground);

                    //Import interactable.
                    TileInteractable2D interactable = interactableRegistry.GetValue(reader.ReadUInt16());
                    if (interactable != null)
                    {
                        ITileEntity2D e = PlaceInteractable(tp, interactable);
                        //Import tile entity.
                        if (e != null)
                            e.Import(reader);
                    }
                }
        }
    }
}
