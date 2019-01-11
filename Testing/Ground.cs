using Maps;
using Maps.Rendering;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu]
public class Ground : TileGround2D
{
    public Unity.Rendering.MeshInstanceRenderer renderer, rendererGhost;
    public int weight;

    public override int GetNavigationWeight(Region2D region, TilePosition2D globalTilePosition)
    {
        return weight;
    }

    public override Material GetUiMaterial()
    {
        return renderer.material;
    }

    public override Mesh GetUiMesh()
    {
        return renderer.mesh;
    }

    public override string GetUnlocalizedName()
    {
        return GetRegistryName();
    }

    public override void OnPlaced(Region2D region, TilePosition2D globalTilePosition)
    {

    }
    public override void OnRemoved(Region2D region, TilePosition2D globalTilePosition)
    {

    }
    public override object OnRendered(Region2D region, TilePosition2D globalTilePosition)
    {
        Entity e = MapRenderingHandler2D.ENTITY_MANAGER.CreateEntity(typeof(Unity.Transforms.Position), typeof(Unity.Rendering.MeshInstanceRenderer));
        MapRenderingHandler2D.ENTITY_MANAGER.SetComponentData(e, new Unity.Transforms.Position()
        {
            Value = new Unity.Mathematics.float3(globalTilePosition.x + 0.5f, -0.5f, globalTilePosition.z + 0.5f)
        });
        MapRenderingHandler2D.ENTITY_MANAGER.SetSharedComponentData(e, renderer);
        return e;
    }
    public override void OnUnrendered(Region2D region, TilePosition2D globalTilePosition, object renderObject)
    {
        MapRenderingHandler2D.ENTITY_MANAGER.DestroyEntity((Entity)renderObject);
    }


    public override object OnGhostRendered(Region2D region, TilePosition2D globalTilePosition)
    {
        Entity e = MapRenderingHandler2D.ENTITY_MANAGER.CreateEntity(typeof(Unity.Transforms.Position), typeof(Unity.Rendering.MeshInstanceRenderer));
        MapRenderingHandler2D.ENTITY_MANAGER.SetComponentData(e, new Unity.Transforms.Position()
        {
            Value = new Unity.Mathematics.float3(globalTilePosition.x + 0.5f, -0.5f, globalTilePosition.z + 0.5f)
        });
        MapRenderingHandler2D.ENTITY_MANAGER.SetSharedComponentData(e, rendererGhost);
        return e;
    }
    public override void OnGhostUnrendered(Region2D region, TilePosition2D globalTilePosition, object renderObject)
    {
        MapRenderingHandler2D.ENTITY_MANAGER.DestroyEntity((Entity)renderObject);
    }
}
