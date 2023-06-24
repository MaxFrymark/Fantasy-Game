using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileBuilding : MonoBehaviour, IBuilding
{
    [SerializeField] protected int constructionTime;

    [SerializeField] List<TileNode.TerrainType> validTerrain;
    [SerializeField] List<int> validForest;

    [SerializeField] Sprite blankSprite;

    TileNode homeTile;
    Region homeRegion;

    public int GetConstructionTime()
    {
        return constructionTime;
    }
    
    public void AssignHomeRegion(Region home)
    {
        homeRegion = home;
    }
    public Region GetHomeRegion()
    {
        return homeRegion;
    }

    public void AssignHomeTile(TileNode homeTile)
    {
        this.homeTile = homeTile;
    }

    public bool CheckIfTerrainValid(NodeTerrainData terrain)
    {
        return validTerrain.Contains(terrain.GetTerrainType()) && validForest.Contains(terrain.GetForestLevel());
    }

    public Sprite GetBlankSprite()
    {
        return blankSprite;
    }

    protected TileNode GetHomeTile()
    {
        return homeTile;
    }

    public virtual void ActivateBuilding()
    {
        AssignHomeTile(FindObjectOfType<NodeManager>().FindClosestNodeToWorldPostition(transform.position));
        AssignHomeRegion(homeTile.GetRegion());
    }
    
    public virtual TileNode GetValidLocationForBuilding(Region region)
    {
        TileNode nodeToPlace = null;
        List<TileNode> possibleLocations = new List<TileNode>();
        foreach(TileNode node in region.GetTilesInRegion())
        {
            if(node.GetBuilding() == null && CheckIfTerrainValid(node.GetNodeTerrainData()))
            {
                possibleLocations.Add(node);
            }
        }

        if(possibleLocations.Count > 0)
        {
            nodeToPlace = possibleLocations[Random.Range(0, possibleLocations.Count)];
        }

        return nodeToPlace;
    }

    public List<Resource> GetConstructionCost()
    {
        List<Resource> cost = new List<Resource>();
        SetUpConstructionCost(cost);
        return cost;
    }

    public abstract void SetUpConstructionCost(List<Resource> cost);
}
