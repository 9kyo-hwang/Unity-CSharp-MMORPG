using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine.Tilemaps;
#endif

public class MapEditor
{
#if UNITY_EDITOR
    [MenuItem("Tools/GenerateMap %#g")]  // %: Ctrl, #: Shift, g: Keycode.G
    private static void GenerateCollisionMap()
    {
        foreach (GameObject map in Resources.LoadAll<GameObject>("Prefabs/Map"))
        {
            Tilemap baseMap = Util.FindChild<Tilemap>(map, "Tilemap_Base", true);
            Tilemap collisionMap = Util.FindChild<Tilemap>(map, "Tilemap_Collision", true);
            if (!baseMap || !collisionMap)
            {
                Debug.Log($"Cannot Find Tilemap in {map.gameObject.name}!");
                continue;
            }
        
            // File 생성.
            using var writer = File.CreateText($"Assets/Resources/Map/{map.gameObject.name}.txt");

            var xMax = baseMap.cellBounds.xMax;
            var xMin = baseMap.cellBounds.xMin;
            var yMax = baseMap.cellBounds.yMax;
            var yMin = baseMap.cellBounds.yMin;

            writer.Write($"{xMin}\n{xMax}\n{yMin}\n{yMax}\n");
            for (int y = yMax - 1; y >= yMin; --y)
            {
                for (int x = xMin; x < xMax; ++x)
                {
                    writer.Write(collisionMap.GetTile(new Vector3Int(x, y, 0)) ? "1" : "0");
                }
                writer.WriteLine();
            }
        }
    }
#endif
}
