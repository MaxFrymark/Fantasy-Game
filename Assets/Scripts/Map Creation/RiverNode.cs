using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverNode : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] riverSprites;

    List<RiverNode> river;
    TileNode thisNode;
    TileNode previousNode;
    int previousNodeBorderIndex;
    TileNode nextNode;
    int nextNodeBorderIndex;

    private bool riverRunsClockwise;

    int riverStartIndex;
    int riverEndIndex;

    public delegate int IterateRiver(int index);

    public void SelectVisibleRiverSegments(List<RiverNode> river)
    {
        //Debug.Log("Start River Node");
        this.river = river;
        thisNode = NodeManager.Instance.GetTileNode(transform.position);
        //Debug.Log("Coordinates: " + thisNode.GetCoordinates());
        int index = FindThisNodesIndex();
        //Debug.Log("Index: " + index);
        //Debug.Log("This Node: " + thisNode.GetCoordinates());
        FindPreviousNode(index);
        //Debug.Log("Previous Node Border Index: " + previousNodeBorderIndex);
        FindNextNode(index);
        ChooseRiverDirection();

        FindRiverStartIndex();
        FindRiverEndIndex();

        IterateRiver iterateRiver;
        if (riverRunsClockwise)
        {
            iterateRiver = thisNode.GetNodeNeighborData().GetClockwiseNeighbor;
        }
        else
        {
            iterateRiver = thisNode.GetNodeNeighborData().GetCounterClockwiseNeighbor;
        }
        //Debug.Log("Ta-Da!");
        //Debug.Log("Start Point: " + riverStartIndex);
        //Debug.Log("End Point: " + riverEndIndex);
        //Debug.Log("IsClockwise: " + riverRunsClockwise);

        PlaceRiver(iterateRiver, riverStartIndex);
        //Debug.Log("End River Node");
    }

    private int FindThisNodesIndex()
    {
        int index = 0;
        for (int i = 0; i < river.Count; i++)
        {
            if (this == river[i])
            {
                index = i;
                break;
            }
        }

        return index;
    }

    private void FindPreviousNode(int index)
    {
        if (index == 0)
        {
            foreach (TileNode node in thisNode.GetNodeNeighborData().GetNeighbors())
            {
                if (node.GetNodeTerrainData().GetTerrainType() == TileNode.TerrainType.mountain)
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

        //Debug.Log("Previous Node: " + previousNode.GetCoordinates());

        previousNodeBorderIndex = FindBorderIndex(previousNode);
    }

    private void FindNextNode(int index)
    {
        if (index == river.Count - 1)
        {
            //Debug.Log("This Tile Is Last Tile");
            foreach (TileNode node in thisNode.GetNodeNeighborData().GetNeighbors())
            {
                if (node.GetNodeTerrainData().GetTerrainType() == TileNode.TerrainType.ocean)
                {
                    nextNode = node;
                    break;
                }
            }
        }

        else
        {
            //Debug.Log("meow");
            nextNode = NodeManager.Instance.GetTileNode(river[index + 1].transform.position);
        }
        //Debug.Log("Next Node: " + nextNode.GetCoordinates());


        nextNodeBorderIndex = FindBorderIndex(nextNode);
    }

    private int FindBorderIndex(TileNode node)
    {
        TileNode[] nodeNeighbors = node.GetNodeNeighborData().GetNeighbors();
        int borderIndex = 7;
        for(int i = 0; i < nodeNeighbors.Length; i++)
        {
            //Debug.Log("Neighbor Coords: " + nodeNeighbors[i].GetCoordinates());
            if (nodeNeighbors[i] == thisNode)
            {
                borderIndex = thisNode.GetNodeNeighborData().GetOppositeSide(i);
                //Debug.Log("Boder Index: " + borderIndex);
            }
        }

        if(borderIndex == 7)
        {
            Debug.LogError("ERROR: RiverNode.FindBorderIndex nodes don't boder each other");
        }

        return borderIndex;
    }

    private void ChooseRiverDirection()
    {
        if (thisNode.GetNodeNeighborData().GetClockwiseNeighbor(previousNodeBorderIndex) == nextNodeBorderIndex)
        {
            riverRunsClockwise = false;
        }

        else if (thisNode.GetNodeNeighborData().GetCounterClockwiseNeighbor(previousNodeBorderIndex) == nextNodeBorderIndex)
        {
            riverRunsClockwise = true;
        }

        else
        {
            int clockWiseDistance = 0;
            int counterClockWiseDistance = 0;

            clockWiseDistance = ShiftRiver(thisNode.GetNodeNeighborData().GetClockwiseNeighbor, previousNodeBorderIndex, nextNodeBorderIndex, 0);
            counterClockWiseDistance = ShiftRiver(thisNode.GetNodeNeighborData().GetCounterClockwiseNeighbor, previousNodeBorderIndex, nextNodeBorderIndex, 0);

            riverRunsClockwise = clockWiseDistance < counterClockWiseDistance;
        }
    }

    private int ShiftRiver(IterateRiver iterateRiver,int index, int target, int i)
    {
        //Debug.Log("Starting Index:" + index + " " + target);

        index = iterateRiver(index);
        //Debug.Log("New Index: " + index);
        i++;
        //Debug.Log("Number of Iterations: " + i);
        if(index == target)
        {
            //Debug.Log("Hi");
            return i;
        }
        else
        {
            return ShiftRiver(iterateRiver, index, target, i);
        }
    }

    private void FindRiverStartIndex()
    {
        bool validBorderRiverNode = false;
        RiverNode[] riverNodes = previousNode.GetNodeTerrainData().GetRiverBorders();
        int index = previousNode.GetNodeNeighborData().GetOppositeSide(previousNodeBorderIndex);
        if (river.Contains(riverNodes[index]))
        {
            Debug.Log("River borders connect");
        }
        else if (previousNode.GetNodeTerrainData().GetTerrainType() == TileNode.TerrainType.mountain)
        {
            validBorderRiverNode = true;
            if (riverRunsClockwise)
            {
                riverStartIndex = thisNode.GetNodeNeighborData().GetClockwiseNeighbor(previousNodeBorderIndex);
            }
            else
            {
                riverStartIndex = thisNode.GetNodeNeighborData().GetCounterClockwiseNeighbor(previousNodeBorderIndex);
            }
        }
        else if (river.Contains(riverNodes[previousNode.GetNodeNeighborData().GetClockwiseNeighbor(index)]))
        {
            validBorderRiverNode = true;
            if (riverRunsClockwise)
            {
                riverStartIndex = previousNodeBorderIndex;
            }
            else
            {
                riverStartIndex = thisNode.GetNodeNeighborData().GetCounterClockwiseNeighbor(previousNodeBorderIndex);
            }
        }
        else if (river.Contains(riverNodes[previousNode.GetNodeNeighborData().GetCounterClockwiseNeighbor(index)]))
        {
            validBorderRiverNode = true;
            if (riverRunsClockwise)
            {
                riverStartIndex = thisNode.GetNodeNeighborData().GetClockwiseNeighbor(previousNodeBorderIndex);
            }
            else
            {
                riverStartIndex = previousNodeBorderIndex;
            }
        }

        if (!validBorderRiverNode)
        {
            Debug.LogError("No Valid River Border");
            /*if(Random.value > 0.5f)
            {
                riverStartIndex = thisNode.GetCounterClockwiseNeighbor(previousNodeBorderIndex);
            }
            else
            {
                riverStartIndex = thisNode.GetClockwiseNeighbor(previousNodeBorderIndex);
            }*/
        }

    }

    private void FindRiverEndIndex()
    {
        if (riverRunsClockwise)
        {
            riverEndIndex = thisNode.GetNodeNeighborData().GetCounterClockwiseNeighbor(nextNodeBorderIndex);
        }
        else
        {
            riverEndIndex = thisNode.GetNodeNeighborData().GetClockwiseNeighbor(nextNodeBorderIndex);
        }
    }

    private void PlaceRiver(IterateRiver iterateRiver, int currentIndex)
    {
        riverSprites[currentIndex].gameObject.SetActive(true);
        thisNode.GetNodeTerrainData().AssignRiverToRiverBorder(this, currentIndex);
        thisNode.GetNodeNeighborData().GetNeighbors()[currentIndex].GetNodeTerrainData().AssignRiverToRiverBorder(this, thisNode.GetNodeNeighborData().GetOppositeSide(currentIndex));
        if(currentIndex != riverEndIndex)
        {
            currentIndex = iterateRiver(currentIndex);
            PlaceRiver(iterateRiver, currentIndex);
        }
    }
}
