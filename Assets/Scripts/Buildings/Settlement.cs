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

    protected List<IBuilding> buildings = new List<IBuilding>();

    [SerializeField] Farm farm;

    public override void ActivateBuilding()
    {
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
        Farm newFarm = Instantiate(farm);
        newFarm.gameObject.SetActive(true);
        TileNode farmNode = newFarm.GetValidLocationForBuilding(GetHomeRegion());
        if (farmNode != null)
        {
            newFarm.transform.position = farmNode.GetWorldPosition();
            newFarm.ActivateBuilding();
            farmNode.GetNodeTerrainData().RemoveForest();
            newFarm.SetToMaximumWorkers();
        }
        else
        {
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
        List<Resource> upkeep = CalculateUpkeep();
        foreach(Resource resource in upkeep)
        {
            treasury.AdjustResources(resource);
        }
    }

    public int GetPriority()
    {
        return 1;
    }

    public List<Resource> CalculateUpkeep()
    {
        List<Resource> upkeep = new List<Resource> { new Resource(Resource.ResourceType.Food, -startingPopulation) };
        return upkeep;
    }
}
