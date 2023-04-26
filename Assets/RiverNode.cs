using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverNode : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] riverSprites;

    public void SelectVisibleRiverSegments(List<RiverNode> river)
    {
        TileNode thisNode = NodeManager.Instance.GetTileNode(transform.position);
        TileNode previousNode;
        TileNode nextNode;
        int index = 0;
        for(int i = 0; i < river.Count; i++)
        {
            if(this == river[i])
            {
                index = i;
                break;
            }
        }

        if(index == 0)
        {
            foreach(TileNode node in thisNode.GetNeighbors())
            {
                if(node.GetTerrainType() == MapCreator.TerrainType.mountain)
                {
                    previousNode = node;
                    break;
                }
            }
        }

        else
        {
            previousNode = NodeManager.Instance.GetTileNode(river[index - 1].transform.position);
        }

        if (index == river.Count - 1)
        {
            foreach (TileNode node in thisNode.GetNeighbors())
            {
                if (node.GetTerrainType() == MapCreator.TerrainType.ocean)
                {
                    nextNode = node;
                    break;
                }
            }
        }

        else
        {
            nextNode = NodeManager.Instance.GetTileNode(river[index + 1].transform.position);
        }
    }

    private TileNode FindPreviousNode(int index)
    {

    }
}
