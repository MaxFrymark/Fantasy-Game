using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TileNode;

public class NodeManager : MonoBehaviour
{
    public static NodeManager Instance;
    
    [SerializeField] Tilemap tileMap;
    [SerializeField] TileStorage tileStorage;
    [SerializeField] RiverNode riverNode;

    private List<TileNode> tileNodes = new List<TileNode> ();
    private List<List<Tile>> tiles;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        else
        {
            Debug.LogError("Duplicate NodeManager Created");
            Destroy(gameObject);
        }

        tileNodes = new List<TileNode>();
        tiles = new List<List<Tile>>();
        PopulateTileList();
    }

    private void PopulateTileList()
    {
        List<Tile> oceanTiles = new List<Tile>();
        foreach(Tile tile in tileStorage.GetOceanTiles())
        {
            oceanTiles.Add(tile);
        }
        tiles.Add(oceanTiles);

        List<Tile> plainsTiles = new List<Tile>();
        foreach (Tile tile in tileStorage.GetPlainsTiles())
        {
            plainsTiles.Add(tile);
        }
        tiles.Add(plainsTiles);

        List<Tile> mountainTiles = new List<Tile>();
        foreach (Tile tile in tileStorage.GetMountainTiles())
        {
            mountainTiles.Add(tile);
        }
        tiles.Add(mountainTiles);

        List<Tile> hillTiles = new List<Tile>();
        foreach (Tile tile in tileStorage.GetHillsTiles())
        {
            hillTiles.Add(tile);
        }
        tiles.Add(hillTiles);
    }

    public void PlaceNode(Vector3Int coordinates, TileNode.TerrainType terrainType)
    {
        TileNode tileNode = GetTileNode(coordinates);

        if (tileNode == null)
        {
            tileNode = new TileNode(this, coordinates, terrainType);
            tileNodes.Add(tileNode);
        }

        else if (tileNode.GetNodeTerrainData().GetTerrainType() != terrainType)
        {
            tileNode.GetNodeTerrainData().SetTerrainType(terrainType);
        }
        
        UpdadateNodeOnTileMap(tileNode);
    }

    public void UpdadateNodeOnTileMap(TileNode node)
    {
        NodeTerrainData terrainData = node.GetNodeTerrainData();
        tileMap.SetTile(node.GetCoordinates(), tiles[(int)terrainData.GetTerrainType()][terrainData.GetForestLevel()]);
    }

    public RiverNode PlaceRiverNode(TileNode tileNode, Transform parent)
    {
        GameObject riverTile = Instantiate(riverNode.gameObject, GetWorldPostitionFromTileNode(tileNode), Quaternion.identity, parent);
        return riverTile.GetComponent<RiverNode>();
    }

    public TileNode GetTileNode(Vector3Int coordinates)
    {
        TileNode tileNode = null;
        foreach(TileNode node in tileNodes)
        {
            if(coordinates == node.GetCoordinates())
            {
                tileNode = node;
                break;
            }
        }
        return tileNode;
    }

    public TileNode GetTileNode(Vector3 worldPosition)
    {
        Vector3Int coordinates = tileMap.LocalToCell(worldPosition);
        return GetTileNode(coordinates);
    }

    public TileNode GetTileNode(int x, int y)
    {
        return GetTileNode(new Vector3Int(x, y, 0));
    }

    public Tilemap GetTilemap()
    {
        return tileMap;
    }

    public Vector3 GetWorldPostitionFromTileNode(TileNode node)
    {
        return tileMap.GetCellCenterWorld(node.GetCoordinates());
    }

    public TileNode FindClosestNodeToWorldPostition(Vector3 worldPosition)
    {
        float shortestDistance = 10000;
        TileNode closestNode = null;
        foreach(TileNode node in tileNodes)
        {
            float distance = Vector2.Distance(worldPosition, tileMap.GetCellCenterWorld(node.GetCoordinates()));
            if(distance < shortestDistance)
            {
                closestNode = node;
                shortestDistance = distance;
            }
        }
        return closestNode;
    }
}


