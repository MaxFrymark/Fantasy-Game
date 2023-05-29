using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;


    public void FindValidBuildingPlacement(TileNode tileNode, bool validPlacement)
    {
        transform.position = tileNode.GetWorldPosition();
        if(validPlacement)
        {
            spriteRenderer.color = Color.green;
        }
        else
        {
            spriteRenderer.color = Color.red;
        }
    } 
}
