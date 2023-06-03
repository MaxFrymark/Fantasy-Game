using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    protected bool commandComplete = false;

    public abstract void ExecuteCommand();

    public bool GetCommandComplete()
    {
        return commandComplete;
    }

    protected void FinishCommand()
    {
        commandComplete = true;
    }
}

public class TestCommand : Command
{
    private Faction sendingFaction;
    
    public TestCommand(Faction faction)
    {
        sendingFaction = faction;
    }

    public override void ExecuteCommand()
    {
        Debug.Log(sendingFaction.GetFactionName() + " sent command.");
        FinishCommand();
    }
}

public class TestCommandWithTimer : Command
{
    private Faction sendingFaction;
    int duration = 2;

    public TestCommandWithTimer(Faction faction)
    {
        sendingFaction = faction;
    }

    public override void ExecuteCommand()
    {
        if(duration > 0)
        {
            Debug.Log(sendingFaction.GetFactionName() + " timed command. Remaining duration: " + duration);
            duration--;
        }
        else
        {
            Debug.Log(sendingFaction.GetFactionName() + " timed command complete.");
            FinishCommand();
        }
    }
}

public class BuildBuildingCommand : Command
{
    BuildingUnderConstruction underConstruction;
    TileBuilding building;
    int remainingTurns;

    public BuildBuildingCommand(BuildingUnderConstruction underConstruction, TileBuilding building, int remainingTurns)
    {
        this.underConstruction = underConstruction;
        this.building = building;
        this.remainingTurns = remainingTurns;
    }

    public override void ExecuteCommand()
    {
        if(remainingTurns > 1)
        {
            remainingTurns--;
            underConstruction.UpdateBuildingCountDown(remainingTurns);
        }

        else
        {
            building.gameObject.SetActive(true);
            building.transform.position = underConstruction.transform.position;
            building.ActivateBuilding();
            Object.Destroy(underConstruction.gameObject);
            FinishCommand();
        }
    }
}
