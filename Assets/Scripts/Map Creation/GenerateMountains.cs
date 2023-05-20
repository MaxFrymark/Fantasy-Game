using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMountains : GenerateTerrainWithNoiseMap
{
    public GenerateMountains(int mapWidth, int mapHeight, int numberOfNoiseLayers, float frequencyMin, float frequencyMax, float[] terrainThreshholds)
        : base(mapWidth, mapHeight, numberOfNoiseLayers, frequencyMin, frequencyMax, terrainThreshholds)
    {
        description = "Generating Mountains";
    }

    public override void GenerateMap()
    {
        DrawMountains();
    }

    private void DrawMountains()
    {
        List<NoiseWave> noiseWaves = GenerateNoiseWaves();
        float[,] noiseMap = GenerateNoiseMap(noiseWaves);
        IterateThroughAllTiles(MapOperation_DrawMountains, noiseMap, terrainThreshholds);
    }

    private void MapOperation_DrawMountains(int x, int y, float[,] noiseMap, float[] terrainThreshholds)
    {
        Vector3Int tilePos = new Vector3Int(x, y);
        NodeManager nodeManager = NodeManager.Instance;
        if (nodeManager.GetTileNode(tilePos).GetNodeTerrainData().IsNodeOcean())
        {
            return;
        }

        float noisemapValue = noiseMap[x + mapWidth, y + mapHeight];

        if (noisemapValue > terrainThreshholds[0])
        {
            nodeManager.PlaceNode(tilePos, TileNode.TerrainType.mountain);
        }

        else if (noisemapValue > terrainThreshholds[1])
        {
            nodeManager.PlaceNode(tilePos, TileNode.TerrainType.hills);
        }
    }
}
