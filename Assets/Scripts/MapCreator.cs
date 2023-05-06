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

    [SerializeField] RiverNode riverNode;

    [SerializeField] Transform labelParent;
    bool labelsPlaced = false;


    int mapWidth = 30;
    int mapHeight = 30;

    private delegate void OperateOnTile(int x, int y, float[,] noiseMap, float[] terrainThreshholds);
    

    public enum TerrainType { ocean, plains, mountain, hills }

    void Start()
    {
        Debug.Log("Drawing Continent");
        DrawContinent();
        EliminateIslands();
        Debug.Log("Drawing Mountains");
        DrawMountains();
        Debug.Log("Drawing Rivers");
        DrawRivers();
        Debug.Log("Drawing Forests");
        DrawForest();
        Debug.Log("Creating Regions");
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
                if (!labelsPlaced)
                {
                    PlaceTileCoordinateLabel(new Vector3Int(x, y, 0));
                }
            }
        }
        labelsPlaced = true;
    }

    private List<NoiseWave> GenerateNoiseWaves(int noiseLayers, float frequencyMin, float frequencyMax)
    {
        List<NoiseWave> noiseWaves = new List<NoiseWave>();
        for (int i = 0; i < noiseLayers; i++)
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
        for (int x = 0; x <= mapWidth * 2; x++)
        {
            for (int y = 0; y <= mapHeight * 2; y++)
            {
                float normalization = 0f;
                foreach (NoiseWave wave in waves)
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
        if (Mathf.Abs(tilePos.x) > Mathf.Abs(tilePos.y))
        {
            return Mathf.Pow(tilePos.x, 2) / (float)Mathf.Pow(mapWidth, 2);
        }
        else
        {
            return Mathf.Pow(tilePos.y, 2) / (float)Mathf.Pow(mapHeight, 2);
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

    private List<TileNode> BuildGroupOfMatchingTiles(List<TileNode> checkedTiles, TileNode startingNode, bool debugging)
    {
        

        List<TileNode> tileGroup = new List<TileNode>();

        checkedTiles.Add(startingNode);
        tileGroup.Add(startingNode);

        foreach(TileNode node in startingNode.GetNeighbors())
        {
            if(node == null)
            {
                continue;
            }
            
            if (checkedTiles.Contains(node))
            {
                continue;
            }

            if (node.GetTerrainType() != startingNode.GetTerrainType())
            {
                continue;
            }

            foreach(TileNode tileNode in BuildGroupOfMatchingTiles(checkedTiles, node, false))
            {

                
                tileGroup.Add(tileNode);
            }
        }
        


        return tileGroup;
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

    private void DrawRivers()
    {
        List<TileNode> ocean = FindOcean();
        List<List<TileNode>> mountainRanges = FindMountainRanges();
        foreach(List<TileNode> mountainRange in mountainRanges)
        {

            List<TileNode> river = DrawRiverFromMountain(mountainRange, ocean);

            
            if (river != null && river.Count > 2)
            {
                Debug.Log("Start River");
                GameObject riverParent = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
                riverParent.name = "River";
                List<RiverNode> riverNodes = PlaceRiverTiles(river, riverParent.transform);
                foreach(RiverNode node in riverNodes)
                {
                    node.SelectVisibleRiverSegments(riverNodes);
                }
                Debug.Log("End River");
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
                TileNode node = nodeManager.GetTileNode(new Vector3Int(x, y));

                if (checkedTiles.Contains(node))
                {
                    continue;
                }


                if (node.IsNodeOcean())
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
        foreach(List<TileNode> sea in seas)
        {
            if(sea.Count > largestSea.Count)
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
                TileNode node = nodeManager.GetTileNode(new Vector3Int(x, y));

                if (checkedTiles.Contains(node))
                {
                    continue;
                }


                if (node.GetTerrainType() == TerrainType.mountain)
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
        foreach(TileNode neighbor in closestOceanTile.GetNeighbors())
        {
            if (!neighbor.IsNodeOcean())
            {
                destinationTile = neighbor;
                break;
            }
        }

        if(destinationTile == null)
        {
            Debug.LogError("DrawRiverFromMountain Returned an ocean tile that doesn't border land");
        }

        

        TileNode startingTile = FindClosestTile(mountainRange, destinationTile.GetCoordinates());

        if(startingTile == destinationTile)
        {
            return null;
        }
        
        return Pathfinding.Singleton.FindPath(startingTile.GetCoordinates(), destinationTile.GetCoordinates());
    }

    private Vector3 FindCenterOfMountainRange(List<TileNode> mountainRange)
    {
        Vector3 center = Vector3.zero;
        foreach(TileNode node in mountainRange)
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
        for(int i = 0; i < river.Count; i++)
        {
            GameObject riverTile = Instantiate(riverNode.gameObject, nodeManager.GetWorldPostitionFromTileNode(river[i]), Quaternion.identity, parent);
            RiverNode node = riverTile.GetComponent<RiverNode>();
            riverNodes.Add(node);
        }
        return riverNodes;
    }

    private void PlaceTileCoordinateLabel(Vector3Int coordinate)
    {
        TextMeshPro label = Instantiate(tileCoordinateLabel, nodeManager.GetTilemap().GetCellCenterWorld(coordinate), Quaternion.identity, labelParent);
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
