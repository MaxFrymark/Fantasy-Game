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
    
}
