using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileNode;

public class TileNode
{
    NodePathFindingData pathFindingData;
    NodeNeighborData neighborData;
    NodeTerrainData terrainData;

    NodeManager nodeManager;
    Vector3Int coordinates;
    
    Region region;
    TileBuilding building;

    public enum TerrainType { ocean, plains, mountain, hills }

    public TileNode(NodeManager nodeManager, Vector3Int coordinates, TerrainType terrainType)
    {
        this.nodeManager = nodeManager;
        this.coordinates = coordinates;

        pathFindingData = new NodePathFindingData();
        neighborData = new NodeNeighborData(this);
        terrainData = new NodeTerrainData(this);
        terrainData.SetTerrainType(terrainType);
    }

    public Vector3Int GetCoordinates()
    {
        return coordinates;
    }

    public Vector3 GetWorldPosition()
    {
        return nodeManager.GetWorldPostitionFromTileNode(this);
    }

    public NodePathFindingData GetNodePathFindingData()
    {
        return pathFindingData;
    }

    public NodeNeighborData GetNodeNeighborData()
    {
        return neighborData;
    }

    public NodeTerrainData GetNodeTerrainData()
    {
        return terrainData;
    }

    public NodeManager GetNodeManager()
    {
        return nodeManager;
    }

    public void SetBuilding(TileBuilding building)
    {
        this.building = building;
        if(building is Settlement)
        {
            terrainData.RemoveForest();
        }
    }

    public TileBuilding GetBuilding()
    {
        return building;
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
        foreach (TileNode node in neighborData.GetNeighbors())
        {
            if (!node.terrainData.IsNodeOcean() && node.region != region)
            {
                return true;
            }
        }

        return false;
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

public class NodeNeighborData
{
    TileNode[] neighbors;
    TileNode parentNode;

    public NodeNeighborData(TileNode parentNode)
    {
        neighbors = new TileNode[6];
        this.parentNode = parentNode;
        FindAllNeighbors();
    }

    public TileNode[] GetNeighbors()
    {
        return neighbors;
    }

    private void FindAllNeighbors()
    {
        //Debug.Log("Anchor Node: " + coordinates);
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if ((x == 0 && y == 0))
                {
                    continue;
                }

                if (parentNode.GetCoordinates().y % 2 == 0)
                {
                    if (x == 1 && y != 0)
                    {
                        continue;
                    }
                }
                else
                {
                    if (x == -1 && y != 0)
                    {
                        continue;
                    }
                }

                //Debug.Log(x + ", " + y);


                TileNode node = parentNode.GetNodeManager().GetTileNode(parentNode.GetCoordinates() + new Vector3Int(x, y, 0));
                if (node != null)
                {
                    AssignNeighbor(node);
                    node.GetNodeNeighborData().AssignNeighbor(parentNode);
                }
            }
        }

        //Debug.Log("----------------");
    }


    public void AssignNeighbor(TileNode neighboringNode)
    {

        int index;
        if (parentNode.GetCoordinates().y % 2 == 0)
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
            Debug.LogError("Attempted to assign invalid node as a neighbor. Node Coordinates: " + parentNode.GetCoordinates() + "Neighbor Coordinates: " + neighboringNode.GetCoordinates());
        }
    }

    private int FindNeighborIndexEven(TileNode neighborNode)
    {
        //Debug.Log("even");
        Vector2Int neighborDirection = GetNeighborDirection(neighborNode);
        //Debug.Log(neighborDirection);
        int neighborIndex;
        if (neighborDirection == Vector2Int.up)
        {
            neighborIndex = 0;
        }
        else if (neighborDirection == Vector2Int.right)
        {
            neighborIndex = 1;
        }

        else if (neighborDirection == Vector2Int.down)
        {
            neighborIndex = 2;
        }

        else if (neighborDirection == new Vector2Int(-1, -1))
        {
            neighborIndex = 3;
        }

        else if (neighborDirection == Vector2Int.left)
        {
            neighborIndex = 4;
        }

        else if (neighborDirection == new Vector2Int(-1, 1))
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
        return (Vector2Int)(neighborNode.GetCoordinates() - parentNode.GetCoordinates());
    }

    public int GetClockwiseNeighbor(int startPoint)
    {
        //Debug.Log("StartPoint: " + startPoint);
        if (startPoint == 5)
        {
            startPoint = 0;
        }
        else
        {
            startPoint++;
        }
        //Debug.Log("Clockwise StartPoint: " + startPoint);

        return startPoint;
    }

    public int GetCounterClockwiseNeighbor(int startPoint)
    {
        //Debug.Log("StartPoint: " + startPoint);

        if (startPoint == 0)
        {
            startPoint = 5;
        }
        else
        {
            startPoint--;
        }

        //Debug.Log("Counter Clockwise StartPoint: " + startPoint);

        return startPoint;
    }

    public int GetOppositeSide(int startPoint)
    {
        int oppositeSide = 0;
        switch (startPoint)
        {
            case 0:
                oppositeSide = 3;
                break;
            case 1:
                oppositeSide = 4;
                break;
            case 2:
                oppositeSide = 5;
                break;
            case 3:
                oppositeSide = 0;
                break;
            case 4:
                oppositeSide = 1;
                break;
            case 5:
                oppositeSide = 2;
                break;
            default:
                Debug.LogWarning("ERROR: TileNode.GetOppositeSide invalid startPoint " + startPoint);
                break;
        }

        return oppositeSide;
    }

}

public class NodeTerrainData
{
    TileNode parentNode;
    RiverNode[] riverBorders;
    int forestLevel = 0;

    TerrainType terrainType;

    int baseFertility;
    int currentFertility;

    public NodeTerrainData(TileNode parentNode)
    {
        riverBorders = new RiverNode[6];
        this.parentNode = parentNode;
    }


    public int GetForestLevel()
    {
        return forestLevel;
    }

    public RiverNode[] GetRiverBorders()
    {
        return riverBorders;
    }

    public TerrainType GetTerrainType()
    {
        return terrainType;
    }
    public void SetTerrainType(TerrainType terrainType)
    {
        this.terrainType = terrainType;
        CalculateFertility();
    }

    public void AssignRiverToRiverBorder(RiverNode node, int index)
    {
        riverBorders[index] = node;
        CalculateFertility();
    }

    public void ChangeForestLevel(int forrestAdjustment)
    {
        forestLevel += forrestAdjustment;
        CalculateFertility();
    }

    public void RemoveForest()
    {
        if(forestLevel > 0)
        {
            ChangeForestLevel(-1);
            RemoveForest();
        }
        parentNode.GetNodeManager().UpdadateNodeOnTileMap(parentNode);
    }

    private void CalculateFertility()
    {
        int fertility = 0;
        switch (terrainType)
        {
            case TerrainType.plains:
                fertility = 150;
                break;
            case TerrainType.hills:
                fertility = 75;
                break;
        }

        if(fertility > 0)
        {
            fertility -= forestLevel * 20;
            bool hasRiverBorder = false;
            foreach(RiverNode node in riverBorders)
            {
                if(node != null)
                {
                    hasRiverBorder = true;
                }
            }
            if (hasRiverBorder)
            {
                fertility += fertility / 2;
            }
        }

        baseFertility = fertility;
        currentFertility = baseFertility;
    }

    public int GetFertility()
    {
        return currentFertility;
    }

    public bool IsNodeOcean()
    {
        return terrainType == TerrainType.ocean;
    }
}
