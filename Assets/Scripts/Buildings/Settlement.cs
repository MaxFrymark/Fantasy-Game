using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Settlement : TileBuilding, IEconomicObject
{
    [SerializeField] TextMeshPro settlementNameField;
    List<Pop> settlementPopulation = new List<Pop>();
    [SerializeField] int startingPopulation;
    string settlementName;
    Treasury treasury;

    int growthTarget;
    int currentGrowth = 0;
    int growthPerTurn = 20;

    protected List<IBuilding> buildings = new List<IBuilding>();

    bool settlementStarving = false;

    [SerializeField] Farm farm;

    public override void ActivateBuilding()
    {
        growthTarget = CalculateGrowthTarget();
        treasury = new Treasury();
        treasury.AddEconomicObject(this);
        for (int i = 0; i < startingPopulation; i++)
        {
            AddPop();
        }

        BuildStartingBuildings();
    }

    protected virtual void BuildStartingBuildings()
    {
        ObjectPool objectPool = FindObjectOfType<ObjectPool>();

        Farm newFarm = objectPool.GetTileBuildingFromPool("Farm") as Farm;
        newFarm.gameObject.SetActive(true);
        TileNode farmNode = newFarm.GetValidLocationForBuilding(GetHomeRegion());
        if (farmNode != null)
        {
            newFarm.transform.position = farmNode.GetWorldPosition();
            newFarm.transform.parent = transform;
            newFarm.ActivateBuilding();
            farmNode.GetNodeTerrainData().RemoveForest();
            newFarm.SetToMaximumWorkers();
        }
        else
        {
            newFarm.gameObject.SetActive(false);
            Debug.Log("Region Lacks Valid Space for Farm");
        }
    }

    public void SetName(string settlementName)
    {
        this.settlementName = settlementName;
        settlementNameField.text = settlementName;
    }

    public string GetSettlementName()
    {
        return settlementName;
    }

    public void AddBuilding(IBuilding building)
    {
        buildings.Add(building);
        if(building is IEconomicObject)
        {
            IEconomicObject economicBuilding = (IEconomicObject)building;
            treasury.AddEconomicObject(economicBuilding);
        }
    }

    public void HandlePopulationGrowth()
    {
        if (settlementStarving)
        {
            currentGrowth -= 10;
            if (currentGrowth < 0)
            {
                settlementPopulation[0].KillPop();
                settlementPopulation.RemoveAt(0);
                growthTarget = CalculateGrowthTarget();
                currentGrowth = growthTarget / 2;
            }
        }
        else
        {
            currentGrowth += growthPerTurn;
            if (currentGrowth >= growthTarget)
            {
                GrowPopulation();
            }
        }
    }

    private void GrowPopulation()
    {
        AddPop();
        currentGrowth -= growthTarget;
        growthTarget = CalculateGrowthTarget();
    }

    private void AddPop()
    {
        Pop pop = new Pop();
        pop.AssignToHome(this);
        settlementPopulation.Add(pop);
    }

    public Pop GetIdleWorker()
    {
        foreach(Pop pop in settlementPopulation)
        {
            if(pop.GetJob() == null)
            {
                return pop;
            }
        }

        return null;
    }

    public Treasury GetTreasury() 
    { 
        if(treasury == null)
        {
            treasury = new Treasury();
        }
        return treasury; 
    }

    public List<Pop> GetWorkers()
    {
        return settlementPopulation;
    }

    public List<IBuilding> GetBuildingList()
    {
        return buildings;
    }

    public virtual void UpdateSettlementManagerUI()
    {

    }

    public void TakeAction()
    {
        settlementStarving = !treasury.PayUpkeep(CalculateUpkeep());
        HandlePopulationGrowth();
        
    }

    public int GetPriority()
    {
        return 1;
    }

    public List<Resource> CalculateUpkeep()
    {
        List<Resource> upkeep = new List<Resource> { new Resource(Resource.ResourceType.Food, -settlementPopulation.Count) };
        return upkeep;
    }

    private int CalculateGrowthTarget()
    {
        return 100;
    }

    public int GetGrowthPerTurn()
    {
        if (settlementStarving)
        {
            return -10;
        }

        return growthPerTurn;
    }

    public int GetCurrentGrowth()
    {
        return currentGrowth;
    }

    public int GetGrowthTarget()
    {
        return growthTarget;
    }

    public override bool IsBuildingAvailable()
    {
        return false;
    }
}
