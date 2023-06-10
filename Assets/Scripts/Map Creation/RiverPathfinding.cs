using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverPathfinding : Pathfinding
{
    private new const int TERRAIN_COST_HILLS = 50;
    private const int RANDOM_FACTOR = 30;

    protected override List<TileNode> PathfindingLoop(TileNode endNode)
    {
        //Debug.Log("Start River Pathfinding");
        while (openList.Count > 0)
        {
            TileNode currentNode = GetLowestFCostNode(openList);
            if (FindIfNodeBordersOcean(currentNode))
            {
                endNode = currentNode;
            }
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            FindPathfindingCosts(endNode, currentNode);
        }
        return null;
    }

    protected override int CalculateTerrainModifier(TileNode node)
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
        terrainModifer += Random.Range(0, RANDOM_FACTOR);
        return terrainModifer;
    }

    private bool FindIfNodeBordersOcean(TileNode node)
    {
        foreach(TileNode neighbor in node.GetNodeNeighborData().GetNeighbors())
        {
            if (neighbor.GetNodeTerrainData().IsNodeOcean())
            {
                
                return true;
            }
        }
        return false;
    }
}
