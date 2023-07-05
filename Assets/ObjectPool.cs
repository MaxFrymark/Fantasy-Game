using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] TileBuilding[] tileBuildingPrefabs;
    
    List<TileBuilding> tileBuildingPool = new List<TileBuilding>();

    private void InitialPoolSetUp()
    {
        foreach(TileBuilding tileBuilding in tileBuildingPrefabs)
        {
            InstantiateTileBuildingPrefab(tileBuilding);
        }
    }

    public TileBuilding GetTileBuildingFromPool(string buildingTag)
    {
        if(tileBuildingPool.Count == 0)
        {
            InitialPoolSetUp();
        }
        
        TileBuilding tileBuilding = null;
        foreach(TileBuilding building in tileBuildingPool)
        {
            if(building.GetObjectTag() == buildingTag && !building.gameObject.activeInHierarchy) 
            {
                tileBuilding = building;
            }
        }

        if(tileBuilding == null)
        {
            tileBuilding = AddTileBuildingToPool(buildingTag);
        }

        return tileBuilding;
    }

    private TileBuilding AddTileBuildingToPool(string buildingTag)
    {
        TileBuilding newTilebuilding = null;
        foreach(TileBuilding building in tileBuildingPrefabs)
        {
            if(buildingTag == building.GetObjectTag())
            {
                newTilebuilding = InstantiateTileBuildingPrefab(building);
                break;
            }
        }
        return newTilebuilding;
    }

    private TileBuilding InstantiateTileBuildingPrefab(TileBuilding tileBuilding)
    {
        TileBuilding newTileBuilding = Instantiate(tileBuilding);
        newTileBuilding.transform.parent = transform;
        newTileBuilding.gameObject.SetActive(false);
        tileBuildingPool.Add(newTileBuilding);
        return newTileBuilding;
    }
}
