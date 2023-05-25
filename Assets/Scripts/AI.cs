using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI
{
    private AIFaction faction;

    public AI(AIFaction faction)
    {
        this.faction = faction;
    }

    public void GenerateCommandsForFaction()
    {
        faction.ReceiveCommandFromAI(new TestCommand(faction));
    }
}
