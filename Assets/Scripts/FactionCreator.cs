using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionCreator
{
    List<Color> factionColors;
    
    public FactionCreator()
    {
        factionColors = new List<Color>() { Color.red, Color.blue, Color.green, Color.yellow, Color.cyan, Color.magenta };
    }

    public Faction CreateFaction(City city)
    {
        Faction faction = new Faction(city, factionColors[0]);
        factionColors.RemoveAt(0);
        return faction;
    }
}
