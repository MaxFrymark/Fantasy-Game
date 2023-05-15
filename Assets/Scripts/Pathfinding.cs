using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding 
{
    private static Pathfinding instance;
    
    private const int BASE_MOVE_COST = 10;

    private const int TERRAIN_COST_PLAINS = 5;
    private const int TERRAIN_COST_HILLS = 10;
    private const int TERRAIN_COST_FOREST = 5;
    private const int TERRAIN_COST_MOUNTAIN = 5000;

    private NodeManager grid;
    private List<TileNode> openList;
    private List<TileNode> closedList;

    public static Pathfinding Singleton
    {
        get
        {
            if(instance == null)
            {
                instance = new Pathfinding();
            }
            return instance;
        }
    }

    public List<TileNode> FindPath(Vector3Int origin, Vector3Int destination)
    {
        
        if (grid == null)
        {
            grid = NodeManager.Instance;
        }
        
        TileNode startNode = grid.GetTileNode(origin);
        TileNode endNode = grid.GetTileNode(destination);
        openList = new List<TileNode> { startNode };
        closedList = new List<TileNode>();
        

        for(int x = -30; x <= 30; x++)
        {
            for(int y = -30; y <= 30; y++)
            {
                TileNode node = grid.GetTileNode(x, y);
                node.GetNodePathFindingData().gCost = int.MaxValue;
                node.GetNodePathFindingData().CalculateFCost();
                node.GetNodePathFindingData().cameFromNode = null;
            }
        }

        startNode.GetNodePathFindingData().gCost = CalculateTerrainModifier(startNode);
        startNode.GetNodePathFindingData().hCost = CalculateDistanceCost(startNode, endNode);
        startNode.GetNodePathFindingData().CalculateFCost();

        while(openList.Count > 0)
        {
            TileNode currentNode = GetLowestFCostNode(openList);
            if(currentNode == endNode)
            {
                return CalculatePath(endNode);     
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(TileNode adjacentNode in currentNode.GetNodeNeighborData().GetNeighbors())
            {
                if (closedList.Contains(adjacentNode)) continue;
                if (adjacentNode.GetNodeTerrainData().IsNodeOcean())
                {
                    closedList.Add(adjacentNode);
                    continue;
                }
                int tentativeGCost = currentNode.GetNodePathFindingData().gCost + CalculateDistanceCost(currentNode, adjacentNode);
                if(tentativeGCost < adjacentNode.GetNodePathFindingData().gCost)
                {
                    adjacentNode.GetNodePathFindingData().cameFromNode = currentNode;
                    adjacentNode.GetNodePathFindingData().gCost = tentativeGCost + CalculateTerrainModifier(adjacentNode);
                    adjacentNode.GetNodePathFindingData().hCost = CalculateDistanceCost(adjacentNode, endNode);
                    adjacentNode.GetNodePathFindingData().CalculateFCost();

                    if (!openList.Contains(adjacentNode))
                    {
                        openList.Add(adjacentNode);
                    }
                }
            }
        }
        return null;
    }

    private int CalculateDistanceCost(TileNode a, TileNode b)
    {
        int xDistance = Mathf.Abs(a.GetCoordinates().x - b.GetCoordinates().x);
        int yDistance = Mathf.Abs(a.GetCoordinates().y - b.GetCoordinates().y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        
        return (BASE_MOVE_COST * remaining);
    }

    private int CalculateTerrainModifier(TileNode node)
    {
        int terrainModifer = 0;
        switch (node.GetNodeTerrainData().GetTerrainType())
        {
            case TileNode.TerrainType.plains:
                terrainModifer += TERRAIN_COST_PLAINS;
                break;
            case TileNode.TerrainType.hills:
                terrainModifer += TERRAIN_COST_HILLS;
                break;
            case TileNode.TerrainType.mountain:
                terrainModifer += TERRAIN_COST_MOUNTAIN;
                break;
        }
        terrainModifer += TERRAIN_COST_FOREST * node.GetNodeTerrainData().GetForestLevel();
        return terrainModifer;
    }

    private TileNode GetLowestFCostNode(List<TileNode> nodeList)
    {
        TileNode lowestCostNode = nodeList[0];
        for(int i = 1; i < nodeList.Count; i++)
        {
            if(nodeList[i].GetNodePathFindingData().fCost < lowestCostNode.GetNodePathFindingData().fCost)
            {
                lowestCostNode = nodeList[i];
            }
        }
        return lowestCostNode;
    }

    private List<TileNode> CalculatePath(TileNode endNode)
    {
        List<TileNode> path = new List<TileNode>();
        //path.Add(endNode);
        TileNode currentNode = endNode;
        while (currentNode.GetNodePathFindingData().cameFromNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.GetNodePathFindingData().cameFromNode;
        }
        path.Reverse();
        return path;
    }
}
