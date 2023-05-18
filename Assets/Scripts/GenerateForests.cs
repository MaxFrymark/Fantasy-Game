using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateForests : GenerateTerrainWithNoiseMap
{
    public GenerateForests(int mapWidth, int mapHeight, int numberOfNoiseLayers, float frequencyMin, float frequencyMax, float[] terrainThreshholds)
        : base(mapWidth, mapHeight, numberOfNoiseLayers, frequencyMin, frequencyMax, terrainThreshholds)
    {

    }

    public override void GenerateMap()
    {
        DrawForest();
    }

    private void DrawForest()
    {
        List<NoiseWave> noiseWaves = GenerateNoiseWaves();
        float[,] noiseMap = GenerateNoiseMap(noiseWaves);
        IterateThroughAllTiles(MapOperation_DrawForest, noiseMap, terrainThreshholds);
    }

    private void MapOperation_DrawForest(int x, int y, float[,] noiseMap, float[] terrainThreshholds)
    {
        Vector3Int tilePos = new Vector3Int(x, y);
        TileNode tileNode = NodeManager.Instance.GetTileNode(tilePos);
        TileNode.TerrainType terrainType = tileNode.GetNodeTerrainData().GetTerrainType();
        if (terrainType == TileNode.TerrainType.ocean || terrainType == TileNode.TerrainType.mountain)
        {
            return;
        }

        float noisemapValue = noiseMap[x + mapWidth, y + mapHeight];
        int forrestAdjustment = 0;
        if (noisemapValue > terrainThreshholds[0])
        {
            forrestAdjustment = 2;
        }

        else if (noisemapValue > terrainThreshholds[1])
        {
            forrestAdjustment = 1;
        }

        tileNode.GetNodeTerrainData().ChangeForestLevel(forrestAdjustment);
        NodeManager.Instance.PlaceNode(tileNode.GetCoordinates(), terrainType);
    }
}
