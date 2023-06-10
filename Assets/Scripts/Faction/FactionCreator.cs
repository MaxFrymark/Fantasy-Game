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

    public PlayerFaction CreatePlayerFaction(City city)
    {
        PlayerFaction faction = new PlayerFaction(city, factionColors[0]);
        factionColors.RemoveAt(0);
        return faction;
    }

    public AIFaction CreateAIFaction(City city)
    {
        AIFaction faction = new AIFaction(city, factionColors[0]);
        factionColors.RemoveAt(0);
        return faction;
    }
}
