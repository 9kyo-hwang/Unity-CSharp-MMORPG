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
            Tilemap collisionMap = Util.FindChild<Tilemap>(map, "Tilemap_Collision", true);
            if (!collisionMap)
            {
                Debug.Log($"Cannot Find Tilemap_Collision in {map.gameObject.name}!");
                continue;
            }
        
            // File 생성.
            using var writer = File.CreateText($"Assets/Resources/Map/{map.gameObject.name}.txt");

            var xMax = collisionMap.cellBounds.xMax;
            var xMin = collisionMap.cellBounds.xMin;
            var yMax = collisionMap.cellBounds.yMax;
            var yMin = collisionMap.cellBounds.yMin;

            writer.WriteLine($"{xMin} {xMax} {yMin} {yMax}");
            for (int y = yMax; y >= yMin; --y)
            {
                for (int x = xMin; x <= xMax; ++x)
                {
                    writer.Write(collisionMap.GetTile(new Vector3Int(x, y, 0)) ? "1" : "0");
                }
                writer.WriteLine();
            }
        }
    }
#endif
}
