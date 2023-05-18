using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapGenerationStage
{
    protected int mapWidth;
    protected int mapHeight;
    protected float positionModiferExponent;

    protected delegate void OperateOnTile(int x, int y, float[,] noiseMap, float[] terrainThreshholds);

    public MapGenerationStage(int mapWidth, int mapHeight)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
    }

    public abstract void GenerateMap();
    
    protected void IterateThroughAllTiles(OperateOnTile operateOnTile, float[,] noiseMap, float[] terrainThreshholds)
    {
        for (int x = -mapWidth; x <= mapWidth; x++)
        {
            for (int y = -mapHeight; y <= mapHeight; y++)
            {
                operateOnTile(x, y, noiseMap, terrainThreshholds);
            }
        }
    }

    protected float GetPositionModifier(Vector3Int tilePos)
    {
        if (Mathf.Abs(tilePos.x) > Mathf.Abs(tilePos.y))
        {
            return Mathf.Pow(tilePos.x, 2) / (float)Mathf.Pow(mapWidth, positionModiferExponent);
        }
        else
        {
            return Mathf.Pow(tilePos.y, 2) / (float)Mathf.Pow(mapHeight, positionModiferExponent);
        }
    }

    protected List<TileNode> BuildGroupOfMatchingTiles(List<TileNode> checkedTiles, TileNode startingNode, bool debugging)
    {


        List<TileNode> tileGroup = new List<TileNode>();

        checkedTiles.Add(startingNode);
        tileGroup.Add(startingNode);

        foreach (TileNode node in startingNode.GetNodeNeighborData().GetNeighbors())
        {
            if (node == null)
            {
                continue;
            }

            if (checkedTiles.Contains(node))
            {
                continue;
            }

            if (node.GetNodeTerrainData().GetTerrainType() != startingNode.GetNodeTerrainData().GetTerrainType())
            {
                continue;
            }

            foreach (TileNode tileNode in BuildGroupOfMatchingTiles(checkedTiles, node, false))
            {


                tileGroup.Add(tileNode);
            }
        }



        return tileGroup;
    }
}
