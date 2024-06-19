using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CollisionInfoExtractor : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase tile;
    private List<Vector3Int> _tilePositions = new();

    private void Awake()
    {
        foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.GetTile(position: position))
            {
                _tilePositions.Add(position);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Vector3Int position in _tilePositions)
        {
            Debug.Log(position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
