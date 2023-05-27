using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;


    public bool FindValidBuildingPlacement(TileNode tileNode)
    {
        transform.position = tileNode.GetWorldPosition();
        TileNode.TerrainType terrain = tileNode.GetNodeTerrainData().GetTerrainType();
        if(terrain == TileNode.TerrainType.ocean || terrain == TileNode.TerrainType.mountain || tileNode.GetNodeTerrainData().GetForestLevel() > 0)
        {
            spriteRenderer.color = Color.red;
            return false;
        }
        else
        {
            spriteRenderer.color = Color.green;
            return true;
        }
    } 
}
