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

    

    protected List<IEconomicBuilding> economicBuildings = new List<IEconomicBuilding>();

    protected virtual void Start()
    {
        FindObjectOfType<TurnManager>().OnUpdateEconomy += EconomicActivityForTurn;
        if(treasury == null) 
        {
            treasury = new Treasury();
        }
        for (int i = 0; i < startingPopulation; i++)
        {
            AddPop();
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

    public void AddEconoomicBuilding(IEconomicBuilding economicBuilding)
    {
        economicBuildings.Add(economicBuilding);
    }

    public void HandlePopulationGrowth()
    {

    }

    public void EconomicActivityForTurn(object sender, EventArgs e)
    {
        foreach(IEconomicBuilding economicBuilding in economicBuildings)
        {
            economicBuilding.TakeAction();
        }
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
}
