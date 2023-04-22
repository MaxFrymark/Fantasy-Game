using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCreator : MonoBehaviour
{

    [SerializeField] NodeManager nodeManager;
    [SerializeField] RegionManager regionManager;

    [SerializeField] TextMeshPro tileCoordinateLabel;
    bool labelsPlaced = false;


    int mapWidth = 30;
    int mapHeight = 30;

    private delegate void OperateOnTile(int x, int y, float[,] noiseMap, float[] terrainThreshholds);
    

    public enum TerrainType { ocean, plains, mountain, hills }

    void Start()
    {
        DrawContinent();
        EliminateIslands();
        DrawMountains();
        DrawForest();
        CreateRegions();
        DrawBorders();
    }

    private void IterateThroughAllTiles(OperateOnTile operateOnTile, float[,] noiseMap, float[] terrainThreshholds)
    {
        for(int x = -mapWidth; x <= mapWidth; x++)
        {
            for(int y = - mapHeight; y <= mapHeight; y++)
            {
                operateOnTile(x, y, noiseMap, terrainThreshholds);
            }
        }
    }

    private void DrawContinent()
    {
        List<NoiseWave> noiseWaves = GenerateNoiseWaves(2, 0.1f, 0.2f);
        float[,] noiseMap = GenerateNoiseMap(noiseWaves);
        float[] terrainThreshholds = new float[1] { 0.1f };
        IterateThroughAllTiles(MapOperation_DrawContinent, noiseMap, terrainThreshholds);
    }

    private void MapOperation_DrawContinent(int x, int y, float[,] noiseMap, float[] terrainThreshholds)
    {
        Vector3Int tilePos = new Vector3Int(x, y);
        float noisemapValue = noiseMap[x + mapWidth, y + mapHeight] - GetPositionModifier(tilePos);
        TerrainType terrainToAssign = TerrainType.ocean;
        if (noisemapValue > terrainThreshholds[0])
        {
            terrainToAssign = TerrainType.plains;
        }
        nodeManager.PlaceNode(tilePos, terrainToAssign);
    }

    private void EliminateIslands()
    {
        List<TileNode> checkedTiles = new List<TileNode>();
        List<List<TileNode>> islands = new List<List<TileNode>>();

        for(int x = -mapWidth; x <= mapWidth; x++)
        {
            for(int y = -mapHeight; y <= mapHeight; y++)
            {
                TileNode node = nodeManager.GetTileNode(new Vector3Int(x, y));

                if (checkedTiles.Contains(node))
                {
                    continue;
                }
                
                
                if (node.GetTerrainType() == TerrainType.plains)
                {
                    islands.Add(BuildIsland(checkedTiles, node));
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
        foreach(List<TileNode> island in islands)
        {
            if(largestIsland == null)
            {
                largestIsland = island;
            }

            else if(island.Count > largestIsland.Count)
            {
                largestIsland = island;
            }
        }

        foreach(List<TileNode> island in islands)
        {
            if(island != largestIsland)
            {
                foreach(TileNode tileNode in island)
                {
                    nodeManager.PlaceNode(tileNode.GetCoordinates(), TerrainType.ocean);
                }
            }
        }
    }

    private List<TileNode> BuildIsland(List<TileNode> checkedTiles, TileNode startingNode)
    {
        List<TileNode> island = new List<TileNode>();

        checkedTiles.Add(startingNode);
        island.Add(startingNode);

        foreach(TileNode node in startingNode.GetNeighbors())
        {
            if (checkedTiles.Contains(node))
            {
                continue;
            }

            if (node.GetTerrainType() == TerrainType.ocean)
            {
                continue;
            }

            foreach(TileNode tileNode in BuildIsland(checkedTiles, node))
            {
                island.Add(tileNode);
            }
        }

        return island;
    }

    private void DrawMountains()
    {
        List<NoiseWave> noiseWaves = GenerateNoiseWaves(2, 0.2f, 0.3f);
        float[,] noiseMap = GenerateNoiseMap(noiseWaves);
        float[] terrainThreshholds = new float[2] { 0.75f, 0.55f };
        IterateThroughAllTiles(MapOperation_DrawMountains, noiseMap, terrainThreshholds);
    }

    private void MapOperation_DrawMountains(int x, int y, float[,] noiseMap, float[] terrainThreshholds)
    {
        Vector3Int tilePos = new Vector3Int(x, y);

        if (nodeManager.GetTileNode(tilePos).IsNodeOcean())
        {
            return;
        }

        float noisemapValue = noiseMap[x + mapWidth, y + mapHeight];

        if (noisemapValue > terrainThreshholds[0])
        {
            nodeManager.PlaceNode(tilePos, TerrainType.mountain);
        }

        else if (noisemapValue > terrainThreshholds[1])
        {
            nodeManager.PlaceNode(tilePos, TerrainType.hills);
        }
    }

    private void DrawForest()
    {
        List<NoiseWave> noiseWaves = GenerateNoiseWaves(2, .1f, .2f);
        float[,] noiseMap = GenerateNoiseMap(noiseWaves);
        float[] terrainThreshholds = new float[2] { 0.65f, 0.45f };
        IterateThroughAllTiles(MapOperation_DrawForest, noiseMap, terrainThreshholds);
    }

    private void MapOperation_DrawForest(int x, int y, float[,] noiseMap, float[] terrainThreshholds)
    {
        Vector3Int tilePos = new Vector3Int(x, y);
        TileNode tileNode = nodeManager.GetTileNode(tilePos);
        TerrainType terrainType = tileNode.GetTerrainType();
        if (terrainType == TerrainType.ocean || terrainType == TerrainType.mountain)
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

        tileNode.ChangeForestLevel(forrestAdjustment);
        nodeManager.PlaceNode(tileNode.GetCoordinates(), terrainType);
    }

    private List<NoiseWave> GenerateNoiseWaves(int noiseLayers, float frequencyMin, float frequencyMax)
    {
        List<NoiseWave> noiseWaves = new List<NoiseWave>();
        for(int i = 0; i < noiseLayers; i++)
        {
            float frequency = Random.Range(frequencyMin, frequencyMax);
            NoiseWave wave = new NoiseWave(Random.Range(0, 500), frequency, (float)i / noiseLayers);
            noiseWaves.Add(wave);
        }

        return noiseWaves;
    }

    private float[,] GenerateNoiseMap(List<NoiseWave> waves)
    {
        float[,] noiseMap = new float[mapWidth * 2 + 1, mapHeight * 2 + 1];
        for(int x = 0; x <= mapWidth * 2; x++)
        {
            for(int y = 0; y <= mapHeight * 2; y++)
            {
                float normalization = 0f;
                foreach(NoiseWave wave in waves)
                {
                    noiseMap[x, y] += wave.amplitude * Mathf.PerlinNoise(x * wave.frequency + wave.seed, y * wave.frequency + wave.seed);
                    normalization += wave.amplitude;
                }
                noiseMap[x, y] /= normalization;
            }
        }

        return noiseMap;
    }

    private float GetPositionModifier(Vector3Int tilePos)
    {
        if(Mathf.Abs(tilePos.x) > Mathf.Abs(tilePos.y))
        {
            return Mathf.Pow(tilePos.x, 2) / (float)Mathf.Pow(mapWidth, 2);
        }
        else
        {
            return Mathf.Pow(tilePos.y, 2) / (float)Mathf.Pow(mapHeight, 2);
        }
    }
    
    private void PlaceTileCoordinateLabel(Vector3Int coordinate)
    {
        TextMeshPro label = Instantiate(tileCoordinateLabel, nodeManager.GetTilemap().GetCellCenterWorld(coordinate), Quaternion.identity);
        label.text = ((Vector2Int)coordinate).ToString();

    }

    private void CreateRegions()
    {
        IterateThroughAllTiles(MapOperation_CreateRegions, null, null);
        regionManager.DisolveExtraRegions();
        regionManager.ShrinkTooLargeRgeions();
    }

    private void MapOperation_CreateRegions(int x, int y, float[,] noiseMap, float[] terrainThreshholds)
    {
        TileNode node = nodeManager.GetTileNode(new Vector3Int(x, y, 0));
        if (node.IsNodeOcean())
        {
            return;
        }
        if (node.GetRegion() != null)
        {
            return;
        }

        regionManager.CreateRegion(node);
    }

    private void DrawBorders()
    {
        IterateThroughAllTiles(MapOperation_DrawBorders, null, null);
    }

    private void MapOperation_DrawBorders(int x, int y, float[,] noiseMap, float[] terrainThreshholds)
    {
        Vector3Int coordinates = new Vector3Int(x, y, 0);
        TileNode node = nodeManager.GetTileNode(coordinates);

        if (node.GetTerrainType() == TerrainType.ocean)
        {
            return;
        }

        if (node.IsBorderTile())
        {
            regionManager.DrawBorders(coordinates);
        }
    }
}

public class NoiseWave
{
    public float seed;
    public float frequency;
    public float amplitude;

    public NoiseWave(float seed, float frequency, float amplitude)
    {
        this.seed = seed;
        this.frequency = frequency;
        this.amplitude = amplitude;
    }
}
