using Unity.Entities;
using UnityEngine;

public class Testing : MonoBehaviour
{
    Maps.Map2D map;
    Maps.Editing.MapEditor2D mapEditor;
    Maps.Navigation.MapNavigator2D mapNavigator;
    Maps.Rendering.MapRenderer2D mapRenderer;
    Entity e;
    //EntityManager em;
    const int COUNT = 1000000;
    EntityManager em;

    private void Start()
    {
        //ModLoading.ModLoader.LoadMods();
        //ModLoading.ModLoader.OpenModsFolder();

        map = new Maps.Map2D();
        mapRenderer = new Maps.Rendering.MapRenderer2D(map);
        mapRenderer.Enable();
        mapEditor = new Maps.Editing.MapEditor2D(map);
        mapEditor.Enable();
        mapNavigator = new Maps.Navigation.MapNavigator2D(map);
        mapNavigator.Enable();

        map.OnRegionCreated += Map_OnRegionCreated;
    }

    private void Map_OnRegionCreated(Maps.Region2D region)
    {
        mapRenderer.RequestRenderRegion(region.GetPosition());
    }
}
