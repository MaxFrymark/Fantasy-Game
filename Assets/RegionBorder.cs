using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RegionBorder : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] borderSprites;
    
    void Start()
    {
        TileNode homeNode = NodeManager.Instance.GetTileNode(transform.position);
        TileNode[] neighbors = homeNode.GetNeighbors();
        for(int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] == null)
            {
                continue;
            }

            if (neighbors[i].GetTerrainType() == MapCreator.TerrainType.ocean)
            {
                continue;
            }

            if(homeNode.GetRegion() != neighbors[i].GetRegion())
            {
                borderSprites[i].gameObject.SetActive(true);
            }
        }
    }

}
