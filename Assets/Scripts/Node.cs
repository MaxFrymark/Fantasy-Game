using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node : MonoBehaviour
{
    [SerializeField] Tile[] tiles;
    [SerializeField] TMPro.TextMeshPro m_TextMeshPro;
    Tile activeTile;
    Vector3Int coordinates;
    
    private MapCreator.TerrainType nodeTerrainType;

    public void PlaceNode(int x, int y, Tilemap tilemap, MapCreator.TerrainType nodeTerrainType)
    {
        transform.parent = tilemap.transform;
        this.nodeTerrainType = nodeTerrainType;
        activeTile = tiles[(int)nodeTerrainType];
        coordinates = new Vector3Int(x, y);
        tilemap.SetTile(coordinates, activeTile);
        transform.position = tilemap.GetCellCenterWorld(coordinates);
    }
}
