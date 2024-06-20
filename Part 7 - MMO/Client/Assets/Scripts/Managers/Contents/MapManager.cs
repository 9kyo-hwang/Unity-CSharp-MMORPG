using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapManager
{
    // 현재 활성화 중인 맵 정보
    public Grid ActiveMap { get; private set; }
    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }

    private bool[,] _isCollider;

    public bool CanGo(Vector3Int position)
    {
        if (position.y <= MinY || position.y > MaxY || position.x < MinX || position.x >= MaxX)
        {
            return false;
        }

        int x = position.x - MinX;
        int y = MaxY - position.y;
        return !_isCollider[y, x];
    }
    
    public void Load(int mapId)
    {
        Destroy();  // 혹시 모르니 지우고 시작
        
        // Map Prefab 로드
        string mapName = $"Map_{mapId:000}";  // 3자리 숫자로 설정.
        GameObject map = Managers.Resource.Instantiate($"Map/{mapName}");
        map.name = mapName;

        // Prefab 상에는 collision 타일맵이 Active 상태이므로 여기서 Off 시켜줘야 함
        GameObject collisionMap = Util.FindChild(map, "Tilemap_Collision", true);
        if (collisionMap)
        {
            collisionMap.SetActive(false);
        }

        ActiveMap = map.GetComponent<Grid>();
        
        // Collision 정보 Load
        TextAsset collisionData = Managers.Resource.Load<TextAsset>($"Map/{mapName}");
        if (!collisionData)
        {
            Debug.Log($"Cannot find {mapName}'s collision data file");
            return;
        }
        
        StringReader reader = new StringReader(collisionData.text);

        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());
        int yCount = MaxY - MinY;
        int xCount = MaxX - MinX;
        
        _isCollider = new bool[yCount, xCount];
        for (int y = 0; y < yCount; ++y)
        {
            string line = reader.ReadLine();
            for (int x = 0; x < xCount; ++x)
            {
                _isCollider[y, x] = line[x] == '1';
            }
        }
    }

    public void Destroy()
    {
        GameObject map = GameObject.Find("Map");
        if (map)
        {
            Object.Destroy(map);
            ActiveMap = null;
        }
    }
}
