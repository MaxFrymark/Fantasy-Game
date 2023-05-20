using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenerateLandmass : GenerateTerrainWithNoiseMap
{
    
    
    public GenerateLandmass(int mapWidth, int mapHeight, float positionModifierExponent, int numberOfNoiseLayers, float frequencyMin, float frequencyMax, float[] terrainThreshholds) 
        : base(mapWidth, mapHeight, numberOfNoiseLayers, frequencyMin, frequencyMax, terrainThreshholds)
    {
        this.positionModiferExponent = positionModifierExponent;
        description = "Generating Land.";
    }

    

    protected void DrawLand()
    {
        List<NoiseWave> noiseWaves = GenerateNoiseWaves();
        float[,] noiseMap = GenerateNoiseMap(noiseWaves);
        IterateThroughAllTiles(MapOperation_DrawContinent, noiseMap, terrainThreshholds);
    }

    private void MapOperation_DrawContinent(int x, int y, float[,] noiseMap, float[] terrainThreshholds)
    {
        Vector3Int tilePos = new Vector3Int(x, y);
        float noisemapValue = noiseMap[x + mapWidth, y + mapHeight] - GetPositionModifier(tilePos);
        TileNode.TerrainType terrainToAssign = TileNode.TerrainType.ocean;
        if (noisemapValue > terrainThreshholds[0])
        {
            terrainToAssign = TileNode.TerrainType.plains;
        }
        NodeManager.Instance.PlaceNode(tilePos, terrainToAssign);
    }
}
