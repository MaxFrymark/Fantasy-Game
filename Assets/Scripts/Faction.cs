using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Faction
{
    TurnManager turnManager;
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

    public abstract void EndFactionTurn();
    public abstract void ResetTurn();
}

public class PlayerFaction : Faction
{
    public PlayerFaction(City capitolCity, Color factionColor) : base(capitolCity, factionColor)    {    }

    private bool hasEndedTurn = false;

    public bool GetHasEndedTurn()
    {
        return hasEndedTurn;
    }

    public override void EndFactionTurn()
    {
        hasEndedTurn = true;
    }

    public override void ResetTurn()
    {
        hasEndedTurn = true;
    }
}

public class AIFaction : Faction
{
    public AIFaction(City capitolCity, Color factionColor) : base(capitolCity, factionColor)    {    }

    public override void EndFactionTurn()
    {
        throw new System.NotImplementedException();
    }

    public override void ResetTurn()
    {
        throw new System.NotImplementedException();
    }
}


