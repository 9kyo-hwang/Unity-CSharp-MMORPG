using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public struct Pos
{
    public int Y, X;

    public Pos(int y, int x)
    {
        Y = y;
        X = x;
    }
}

public struct Node : IComparable<Node>
{
    public int F, G, Y, X;

    public int CompareTo(Node other)
    {
        if (F == other.F)
        {
            return 0;
        }

        return F < other.F ? 1 : -1;
    }
}

public class MapManager
{
    // 현재 활성화 중인 맵 정보
    public Grid ActiveMap { get; private set; }
    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }
    public int CountX => MaxX - MinX;
    public int CountY => MaxY - MinY;

    private bool[,] _isCollider;

    public bool CanGo(Vector3Int cell)
    {
        if (cell.y <= MinY || cell.y > MaxY || cell.x < MinX || cell.x >= MaxX)
        {
            return false;
        }

        int x = cell.x - MinX;
        int y = MaxY - cell.y;
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
    
    #region A* Pathfinding

    private int[] _dy = { 1, -1, 0, 0 };
    private int[] _dx = { 0, 0, -1, 1 };
    private int[] _cost = { 10, 10, 10, 10 };

    // source: AI position, destination: player position
    // bIgnoreDestinationCollision: 목적지에는 플레이어가 있기 때문에 그 칸 한정으로 충돌을 무시해야 함
    public List<Vector3Int> FindPath(Vector3Int srcCell, Vector3Int dstCell, bool bIgnoreDstCollision = false)
    {
        List<Pos> path = new List<Pos>();
        bool[,] closed = new bool[CountY, CountX];
        int[,] open = new int[CountY, CountX];
        for (int y = 0; y < CountY; ++y)
        {
            for (int x = 0; x < CountX; ++x)
            {
                open[y,x] = Int32.MaxValue;
            }
        }

        Pos[,] parent = new Pos[CountY, CountX];
        PriorityQueue<Node> pq = new PriorityQueue<Node>();

        Pos src = Cell2Pos(srcCell);
        Pos dst = Cell2Pos(dstCell);

        open[src.Y, src.X] =
            10 * (Math.Abs(dst.Y - src.Y) + Math.Abs(dst.X - src.X));
        pq.Push(new Node
        {
            F = 10 * (Math.Abs(dst.Y - src.Y) + Math.Abs(dst.X - src.X)), G = 0,
            Y = src.Y, X = src.X
        });
        parent[src.Y, src.X] = new Pos(src.Y, src.X);

        while (pq.Count > 0)
        {
            Node node = pq.Pop();
            if (closed[node.Y, node.X])
            {
                continue;
            }

            closed[node.Y, node.X] = true;
            if (node.Y == dst.Y && node.X == dst.X)
            {
                break;
            }
            
            for (int i = 0; i < _dy.Length; ++i)
            {
                Pos next = new Pos(node.Y + _dy[i], node.X + _dx[i]);
                if (!bIgnoreDstCollision || next.Y != dst.Y || next.X != dst.X)  // check wall
                {
                    if (!CanGo(Pos2Cell(next)))
                    {
                        continue;
                    }
                }

                if (closed[next.Y, next.X])
                {
                    continue;
                }

                int g = 0;
                int h = 10 * ((dst.Y - next.Y) * (dst.Y - next.Y) + (dst.X - next.X) * (dst.X - next.X));  // manhattan distance
                if (open[next.Y, next.X] < g + h)
                {
                    continue;
                }

                open[next.Y, next.X] = g + h;
                pq.Push(new Node {F = g + h, G = g, Y = next.Y, X = next.X});
                parent[next.Y, next.X] = new Pos(node.Y, node.X);
            }
        }

        return CellPathFromParent(parent, dst);
    }

    List<Vector3Int> CellPathFromParent(Pos[,] parent, Pos dst)
    {
        List<Vector3Int> cells = new List<Vector3Int>();
        int y = dst.Y, x = dst.X;
        while (parent[y, x].Y != y || parent[y, x].X != x)
        {
            cells.Add(Pos2Cell(new Pos(y, x)));
            y = parent[y, x].Y;
            x = parent[y, x].X;
        }
        cells.Add(Pos2Cell(new Pos(y, x)));
        cells.Reverse();

        return cells;
    }

    private Pos Cell2Pos(Vector3Int cell)
    {
        return new Pos(MaxY - cell.y, cell.x - MinX);
    }

    private Vector3Int Pos2Cell(Pos pos)
    {
        return new Vector3Int(pos.X + MinX, MaxY - pos.Y, 0);
    }

    #endregion
}
