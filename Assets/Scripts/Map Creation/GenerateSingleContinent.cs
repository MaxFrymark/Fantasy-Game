using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSingleContinent : GenerateLandmass
{
    public GenerateSingleContinent(int mapWidth, int mapHeight, float positionModifierExponent, int numberOfNoiseLayers, float frequencyMin, float frequencyMax, float[] terrainThreshholds)
        : base(mapWidth, mapHeight, positionModifierExponent, numberOfNoiseLayers, frequencyMin, frequencyMax, terrainThreshholds)
    {

    }

    public override void GenerateMap()
    {
        DrawLand();
        EliminateIslands();
    }

    private void EliminateIslands()
    {
        List<TileNode> checkedTiles = new List<TileNode>();
        List<List<TileNode>> islands = new List<List<TileNode>>();

        for (int x = -mapWidth; x <= mapWidth; x++)
        {
            for (int y = -mapHeight; y <= mapHeight; y++)
            {
                TileNode node = NodeManager.Instance.GetTileNode(new Vector3Int(x, y));

                if (checkedTiles.Contains(node))
                {
                    continue;
                }


                if (node.GetNodeTerrainData().GetTerrainType() == TileNode.TerrainType.plains)
                {
                    islands.Add(BuildGroupOfMatchingTiles(checkedTiles, node, false));
                }

                else
                {
                    checkedTiles.Add(node);
                }
            }
        }

        ReplaceIslandsWithOcean(islands);
    }

    private void ReplaceIslandsWithOcean(List<List<TileNode>> islands)
    {
        List<TileNode> largestIsland = null;
        foreach (List<TileNode> island in islands)
        {
            if (largestIsland == null)
            {
                largestIsland = island;
            }

            else if (island.Count > largestIsland.Count)
            {
                largestIsland = island;
            }
        }

        foreach (List<TileNode> island in islands)
        {
            if (island != largestIsland)
            {
                foreach (TileNode tileNode in island)
                {
                    NodeManager.Instance.PlaceNode(tileNode.GetCoordinates(), TileNode.TerrainType.ocean);
                }
            }
        }
    }
}
