using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeManager : MonoBehaviour
{
    public static NodeManager Instance;
    
    [SerializeField] Tilemap tileMap;
    [SerializeField] TileStorage tileStorage;

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

    public void PlaceNode(Vector3Int coordinates, MapCreator.TerrainType terrainType)
    {


        TileNode tileNode = GetTileNode(coordinates);

        if (tileNode == null)
        {
            tileNode = new TileNode(this, coordinates, terrainType);
            tileNodes.Add(tileNode);
        }

        else if (tileNode.GetTerrainType() != terrainType)
        {
            tileNode.SetTerrainType(terrainType);
        }
        tileMap.SetTile(coordinates, tiles[(int)terrainType][tileNode.GetForestLevel()]);

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
}

public class TileNode
{
    public NodePathFindingData pathFindingData;
    NodeManager nodeManager;
    Vector3Int coordinates;
    MapCreator.TerrainType terrainType;
    TileNode[] neighbors;
    int forestLevel = 0;
    Region region;

    

    public TileNode(NodeManager nodeManager, Vector3Int coordinates, MapCreator.TerrainType terrainType)
    {
        pathFindingData = new NodePathFindingData();
        this.nodeManager = nodeManager;
        this.coordinates = coordinates;
        this.terrainType = terrainType;
        neighbors = new TileNode[6];
        FindAllNeighbors();
    }

    

    public Vector3Int GetCoordinates()
    {
        return coordinates;
    }

    public MapCreator.TerrainType GetTerrainType()
    {
        return terrainType;
    }

    public void SetTerrainType(MapCreator.TerrainType terrainType)
    {
        this.terrainType = terrainType;
    }

    public TileNode[] GetNeighbors()
    {
        return neighbors;
    }

    public int GetForestLevel()
    {
        return forestLevel;
    }

    public void ChangeForestLevel(int forrestAdjustment)
    {
        forestLevel += forrestAdjustment;
    }

    private void FindAllNeighbors()
    {
        //Debug.Log("Anchor Node: " + coordinates);
        for (int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if((x == 0 && y == 0))
                {
                    continue;
                }

                if(coordinates.y % 2 == 0)
                {
                    if(x == 1 && y != 0)
                    {
                        continue;
                    }
                }
                else
                {
                    if(x == -1 && y != 0)
                    {
                        continue;
                    }
                }

                //Debug.Log(x + ", " + y);


                TileNode node = nodeManager.GetTileNode(coordinates + new Vector3Int(x, y, 0));
                if(node != null)
                {
                    AssignNeighbor(node);
                    node.AssignNeighbor(this);
                }
            }
        }

        //Debug.Log("----------------");
    }

    public void AssignNeighbor(TileNode neighboringNode)
    {

        int index;
        if (coordinates.y % 2 == 0)
        {
            index = FindNeighborIndexEven(neighboringNode);
        }
        else
        {
            index = FindNeighborIndexOdd(neighboringNode);
        }


        if (index < 6)
        {
            //Debug.Log(coordinates + " connects with " + neighboringNode.coordinates);

            neighbors[index] = neighboringNode;
        }
        else
        {
            Debug.LogError("Attempted to assign invalid node as a neighbor. Node Coordinates: " + coordinates + "Neighbor Coordinates: " + neighboringNode.coordinates);
        }
    }

    private int FindNeighborIndexEven(TileNode neighborNode)
    {
        //Debug.Log("even");
        Vector2Int neighborDirection = GetNeighborDirection(neighborNode);
        //Debug.Log(neighborDirection);
        int neighborIndex;
        if(neighborDirection == Vector2Int.up)
        {
            neighborIndex = 0;
        }
        else if(neighborDirection == Vector2Int.right)
        {
            neighborIndex = 1;
        }

        else if(neighborDirection == Vector2Int.down)
        {
            neighborIndex = 2;
        }

        else if(neighborDirection == new Vector2Int(-1, -1))
        {
            neighborIndex = 3;
        }

        else if(neighborDirection == Vector2Int.left)
        {
            neighborIndex = 4;
        }

        else if(neighborDirection == new Vector2Int(-1, 1))
        {
            neighborIndex = 5;
        }

        else
        {
            //Debug.Log(neighborDirection);
            neighborIndex = 6;
        }


        return neighborIndex;
    }

    private int FindNeighborIndexOdd(TileNode neighborNode)
    {
        //Debug.Log("odd");
        Vector2Int neighborDirection = GetNeighborDirection(neighborNode);
        //Debug.Log(neighborDirection);
        int neighborIndex;
        if (neighborDirection == new Vector2Int(1, 1))
        {
            neighborIndex = 0;
        }
        else if (neighborDirection == Vector2Int.right)
        {
            neighborIndex = 1;
        }

        else if (neighborDirection == new Vector2Int(1, -1))
        {
            neighborIndex = 2;
        }

        else if (neighborDirection == Vector2Int.down)
        {
            neighborIndex = 3;
        }

        else if (neighborDirection == Vector2Int.left)
        {
            neighborIndex = 4;
        }

        else if (neighborDirection == Vector2Int.up)
        {
            neighborIndex = 5;
        }

        else
        {
            //Debug.Log(neighborDirection);
            neighborIndex = 6;
        }


        return neighborIndex;
    }

    private Vector2Int GetNeighborDirection(TileNode neighborNode)
    {
        return (Vector2Int)(neighborNode.GetCoordinates() - coordinates);
    }

    public void AssignRegion(Region region)
    {
        this.region = region;
    }

    public Region GetRegion()
    {
        return region;
    }

    public bool IsBorderTile()
    {
        foreach(TileNode node in neighbors)
        {
            if(!node.IsNodeOcean() && node.region != region)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsNodeOcean()
    {
        return terrainType == MapCreator.TerrainType.ocean;
    }
}

public class NodePathFindingData
{
    public TileNode cameFromNode;
    public int gCost;
    public int hCost;
    public int fCost;

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
}
