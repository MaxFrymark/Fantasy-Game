using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRivers : MapGenerationStage
{
    public GenerateRivers(int mapWidth, int mapHeight) : base(mapWidth, mapWidth)
    {
        description = "Generating Rivers.";
    }

    public override void GenerateMap()
    {
        DrawRivers();
    }

    private void DrawRivers()
    {
        List<TileNode> ocean = FindOcean();
        List<List<TileNode>> mountainRanges = FindMountainRanges();
        foreach (List<TileNode> mountainRange in mountainRanges)
        {

            List<TileNode> river = DrawRiverFromMountain(mountainRange, ocean);


            if (river != null && river.Count > 2)
            {
                //Debug.Log("Start River");
                GameObject riverParent = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
                riverParent.name = "River";
                List<RiverNode> riverNodes = PlaceRiverTiles(river, riverParent.transform);
                foreach (RiverNode node in riverNodes)
                {
                    node.SelectVisibleRiverSegments(riverNodes);
                }
                //Debug.Log("End River");
            }
        }
    }

    private List<TileNode> FindOcean()
    {
        List<TileNode> checkedTiles = new List<TileNode>();
        List<List<TileNode>> seas = new List<List<TileNode>>();

        for (int x = -mapWidth; x <= mapWidth; x++)
        {
            for (int y = -mapHeight; y <= mapHeight; y++)
            {
                TileNode node = NodeManager.Instance.GetTileNode(new Vector3Int(x, y));

                if (checkedTiles.Contains(node))
                {
                    continue;
                }


                if (node.GetNodeTerrainData().IsNodeOcean())
                {
                    seas.Add(BuildGroupOfMatchingTiles(checkedTiles, node, true));
                }

                else
                {
                    checkedTiles.Add(node);
                }
            }
        }

        List<TileNode> largestSea = seas[0];
        foreach (List<TileNode> sea in seas)
        {
            if (sea.Count > largestSea.Count)
            {
                largestSea = sea;
            }
        }
        return largestSea;
    }

    private List<List<TileNode>> FindMountainRanges()
    {
        List<TileNode> checkedTiles = new List<TileNode>();
        List<List<TileNode>> mountainRanges = new List<List<TileNode>>();

        for (int x = -mapWidth; x <= mapWidth; x++)
        {
            for (int y = -mapHeight; y <= mapHeight; y++)
            {
                TileNode node = NodeManager.Instance.GetTileNode(new Vector3Int(x, y));

                if (checkedTiles.Contains(node))
                {
                    continue;
                }


                if (node.GetNodeTerrainData().GetTerrainType() == TileNode.TerrainType.mountain)
                {
                    mountainRanges.Add(BuildGroupOfMatchingTiles(checkedTiles, node, false));
                }

                else
                {
                    checkedTiles.Add(node);
                }
            }
        }

        return mountainRanges;
    }

    private List<TileNode> DrawRiverFromMountain(List<TileNode> mountainRange, List<TileNode> ocean)
    {
        Vector3 center = FindCenterOfMountainRange(mountainRange);
        TileNode closestOceanTile = FindClosestTile(ocean, center);

        TileNode destinationTile = null;
        foreach (TileNode neighbor in closestOceanTile.GetNodeNeighborData().GetNeighbors())
        {
            if (!neighbor.GetNodeTerrainData().IsNodeOcean())
            {
                destinationTile = neighbor;
                break;
            }
        }

        if (destinationTile == null)
        {
            Debug.LogError("DrawRiverFromMountain Returned an ocean tile that doesn't border land");
        }



        TileNode startingTile = FindClosestTile(mountainRange, destinationTile.GetCoordinates());

        if (startingTile == destinationTile)
        {
            return null;
        }

        return Pathfinding.Singleton.FindPath(startingTile.GetCoordinates(), destinationTile.GetCoordinates());
    }

    private Vector3 FindCenterOfMountainRange(List<TileNode> mountainRange)
    {
        Vector3 center = Vector3.zero;
        foreach (TileNode node in mountainRange)
        {
            center += node.GetCoordinates();
        }
        center = new Vector3(center.x / mountainRange.Count, center.y / mountainRange.Count, 0);
        return center;
    }

    private TileNode FindClosestTile(List<TileNode> tileNodes, Vector3 origin)
    {
        float distanceToTile = 10000;
        TileNode closestTile = null;
        foreach (TileNode node in tileNodes)
        {
            float distance = Vector3.Distance(origin, node.GetCoordinates());
            if (distance < distanceToTile)
            {
                closestTile = node;
                distanceToTile = distance;
            }
        }

        return closestTile;
    }

    private List<RiverNode> PlaceRiverTiles(List<TileNode> river, Transform parent)
    {
        List<RiverNode> riverNodes = new List<RiverNode>();
        for (int i = 0; i < river.Count; i++)
        {

            RiverNode node = NodeManager.Instance.PlaceRiverNode(river[i], parent);
            riverNodes.Add(node);
        }
        return riverNodes;
    }
}
