using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faction
{
    City capitolCity;
    List<Region> territory;
    string factionName;
    Color factionColor;

    public Faction(City capitolCity, Color factionColor)
    {
        this.capitolCity = capitolCity;
        factionName = capitolCity.GetSettlementName();
        this.factionColor = factionColor;
        territory = new List<Region>() { capitolCity.GetHomeRegion() };
    }

    public Color GetFactionColor()
    {
        return factionColor;
    }
}
