using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRegions : MapGenerationStage
{
    RegionManager regionManager;
    
    public GenerateRegions(int mapWidth, int mapHeight) : base(mapWidth, mapHeight)
    {
        regionManager = GameObject.FindObjectOfType<RegionManager>();
        description = "Generating Regions.";
    }

    public override void GenerateMap()
    {
        CreateRegions();
        DrawBorders();
    }

    private void CreateRegions()
    {
        IterateThroughAllTiles(MapOperation_CreateRegions, null, null);
        regionManager.DisolveExtraRegions();
        regionManager.ShrinkTooLargeRgeions();
    }

    private void MapOperation_CreateRegions(int x, int y, float[,] noiseMap, float[] terrainThreshholds)
    {
        TileNode node = NodeManager.Instance.GetTileNode(new Vector3Int(x, y, 0));
        if (node.GetNodeTerrainData().IsNodeOcean())
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
        TileNode node = NodeManager.Instance.GetTileNode(coordinates);

        if (node.GetNodeTerrainData().GetTerrainType() == TileNode.TerrainType.ocean)
        {
            return;
        }

        if (node.IsBorderTile())
        {
            regionManager.DrawBorders(coordinates);
        }
    }
}
