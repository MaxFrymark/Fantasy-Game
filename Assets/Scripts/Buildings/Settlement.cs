using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Settlement : TileBuilding
{
    [SerializeField] TextMeshPro settlementNameField;
    List<Pop> settlementPopulation = new List<Pop>();
    [SerializeField] int startingPopulation;
    string settlementName;
    Treasury treasury;

    protected List<IBuilding> buildings = new List<IBuilding>();

    [SerializeField] Farm farm;

    protected virtual void Start()
    {
        treasury = new Treasury();
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
        newFarm.transform.position = farmNode.GetWorldPosition();
        newFarm.ActivateBuilding();
        farmNode.GetNodeTerrainData().RemoveForest();
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

    public override void ActivateBuilding()
    {
        
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
}
