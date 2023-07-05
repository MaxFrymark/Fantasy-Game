using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] InputHandler inputHandler;

    int currentTurnNumber = 1;
    List<Command> commandQueue = new List<Command>();
    List<Faction> factions = new List<Faction>();

    public event EventHandler OnUpdateEconomy;

    public void AddFaction(Faction faction)
    {
        factions.Add(faction);
    }
    
    public void EndTurn()
    {
        if (CheckIfAnyPlayersStillActive())
        {
            return;
        }
        ExecuteAllCommands();
        CleanUpCommandQueue();
        UpdateCaldendar();
        UpdateEconomy();
        ResetTurn();
    }

    private bool CheckIfAnyPlayersStillActive()
    {
        foreach(Faction faction in factions)
        {
            if(faction is PlayerFaction)
            {
                PlayerFaction playerFaction = (PlayerFaction)faction;
                if (!playerFaction.GetHasEndedTurn())
                {
                    UpdateActivePlayerFaction(playerFaction);
                    return true;
                }
            }
        }
        return false;
    }

    public void AddCommandToQueue(Command command)
    {
        commandQueue.Add(command);
    }

    private void ExecuteAllCommands()
    {
        foreach(Command command in commandQueue)
        {
            command.ExecuteCommand();
        }
    }

    private void CleanUpCommandQueue()
    {
        for(int i = commandQueue.Count - 1; i >= 0; i--)
        {
            if (commandQueue[i].GetCommandComplete())
            {
                commandQueue.RemoveAt(i);
            }
        }
    }

    private void UpdateEconomy()
    {
        OnUpdateEconomy.Invoke(this, EventArgs.Empty);
    }

    private void ResetTurn()
    {
        PlayerFaction firstPlayer = null;
        foreach(Faction faction in factions)
        {
            faction.ResetTurn();
            if(faction is PlayerFaction && firstPlayer == null)
            {
                firstPlayer = faction as PlayerFaction;
                UpdateActivePlayerFaction(firstPlayer);
            }
        }
    }

    private void UpdateActivePlayerFaction(PlayerFaction faction)
    {
        inputHandler.SetActivePlayerFaction(faction);
    }

    private void UpdateCaldendar()
    {
        currentTurnNumber++;
        Calendar.Singleton.Increment();
    }
}

public class Calendar
{
    public static Calendar singleton;
    
    public Calendar()
    {
        
    }

    public static Calendar Singleton
    {
        get
        {
            if(singleton == null)
            {
                singleton = new Calendar();
            }
            return singleton;
        }
    }

    int year = 1;
    int month = 1;

    public void Increment()
    {
        if(month == 12)
        {
            month = 1;
            year++;
        }

        else
        {
            month++;
        }
    }

    public int GetYear()
    {
        return year;
    }

    public int GetMonth()
    {
        return month;
    }
}
