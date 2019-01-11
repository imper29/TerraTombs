using System;
using System.Reflection;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private void Start()
    {
        string path = "C:\\Users\\marte\\Desktop\\Unity\\TerraTombs\\Mod";
        Assembly a = Assembly.LoadFrom(path + "\\TestMod.dll");

        Type[] types = a.GetTypes();
        Debug.Log(types.Length);
        for (int i = 0; i < types.Length; i++)
        {
            Debug.Log(types[i].BaseType);
            if (types[i].BaseType.Name == typeof(ModBase).Name)
            {
                ModBase mod = (ModBase)Activator.CreateInstance(types[i]);
                mod.Init();
                break;
            }
        }
    }
}