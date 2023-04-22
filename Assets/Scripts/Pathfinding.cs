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
        if(grid == null)
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
                node.pathFindingData.gCost = int.MaxValue;
                node.pathFindingData.CalculateFCost();
                node.pathFindingData.cameFromNode = null;
            }
        }

        startNode.pathFindingData.gCost = CalculateTerrainModifier(startNode);
        startNode.pathFindingData.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.pathFindingData.CalculateFCost();

        while(openList.Count > 0)
        {
            TileNode currentNode = GetLowestFCostNode(openList);
            if(currentNode == endNode)
            {
                return CalculatePath(endNode);     
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(TileNode adjacentNode in currentNode.GetNeighbors())
            {
                if (closedList.Contains(adjacentNode)) continue;
                if (adjacentNode.IsNodeOcean())
                {
                    closedList.Add(adjacentNode);
                    continue;
                }
                adjacentNode.pathFindingData.gCost += CalculateTerrainModifier(adjacentNode);
                int tentativeGCost = currentNode.pathFindingData.gCost + CalculateDistanceCost(currentNode, adjacentNode);
                if(tentativeGCost < adjacentNode.pathFindingData.gCost)
                {
                    adjacentNode.pathFindingData.cameFromNode = currentNode;
                    adjacentNode.pathFindingData.gCost = tentativeGCost;
                    adjacentNode.pathFindingData.hCost = CalculateDistanceCost(adjacentNode, endNode);
                    adjacentNode.pathFindingData.CalculateFCost();

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
        switch (node.GetTerrainType())
        {
            case MapCreator.TerrainType.plains:
                terrainModifer += TERRAIN_COST_PLAINS;
                break;
            case MapCreator.TerrainType.hills:
                terrainModifer += TERRAIN_COST_HILLS;
                break;
            case MapCreator.TerrainType.mountain:
                terrainModifer += TERRAIN_COST_MOUNTAIN;
                break;
        }
        terrainModifer += TERRAIN_COST_FOREST * node.GetForestLevel();
        return terrainModifer;
    }

    private TileNode GetLowestFCostNode(List<TileNode> nodeList)
    {
        TileNode lowestCostNode = nodeList[0];
        for(int i = 1; i < nodeList.Count; i++)
        {
            if(nodeList[i].pathFindingData.fCost < lowestCostNode.pathFindingData.fCost)
            {
                lowestCostNode = nodeList[i];
            }
        }
        return lowestCostNode;
    }

    private List<TileNode> CalculatePath(TileNode endNode)
    {
        List<TileNode> path = new List<TileNode>();
        path.Add(endNode);
        TileNode currentNode = endNode;
        while (currentNode.pathFindingData.cameFromNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.pathFindingData.cameFromNode;
        }
        path.Reverse();
        return path;
    }
}
