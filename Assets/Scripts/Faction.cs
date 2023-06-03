using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Faction
{
    protected TurnManager turnManager;
    City capitolCity;
    List<Region> territory;
    string factionName;
    Color factionColor;

    public Faction(City capitolCity, Color factionColor)
    {
        turnManager = Object.FindObjectOfType<TurnManager>();
        turnManager.AddFaction(this);
        this.capitolCity = capitolCity;
        factionName = capitolCity.GetSettlementName();
        this.factionColor = factionColor;
        territory = new List<Region>() { capitolCity.GetHomeRegion() };
    }

    public Color GetFactionColor()
    {
        return factionColor;
    }

    public string GetFactionName()
    {
        return factionName;
    }

    public City GetCapitol()
    {
        return capitolCity;
    }

    public abstract void EndFactionTurn();
    public abstract void ResetTurn();

    protected void SendCommand(Command command)
    {
        turnManager.AddCommandToQueue(command);
    }
}

public class PlayerFaction : Faction
{
    public PlayerFaction(City capitolCity, Color factionColor) : base(capitolCity, factionColor)    {    }

    private bool hasEndedTurn = false;

    bool activeFaction;

    public bool GetHasEndedTurn()
    {
        return hasEndedTurn;
    }

    public void ReceiveCommandFromInput(Command command)
    {
        SendCommand(command);
    }

    public override void EndFactionTurn()
    {
        hasEndedTurn = true;
        turnManager.EndTurn();
    }

    public override void ResetTurn()
    {
        hasEndedTurn = false;
    }

    public void SetActiveFaction(bool active)
    {
        activeFaction = active;
    }

    public bool GetActiveFaction()
    {
        return activeFaction;
    }
}

public class AIFaction : Faction
{
    private AI ai;
    
    public AIFaction(City capitolCity, Color factionColor) : base(capitolCity, factionColor) 
    {
        ai = new AI(this);
    }

    public void ReceiveCommandFromAI(Command command)
    {
        SendCommand(command);
    }

    public override void EndFactionTurn()
    {
        throw new System.NotImplementedException();
    }

    public override void ResetTurn()
    {
        //ai.GenerateCommandsForFaction();
    }
}


