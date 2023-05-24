using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] InputHandler inputHandler;
    int currentTurnNumber = 1;
    List<Command> commandQueue = new List<Command>();
    List<Faction> factions = new List<Faction>();

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
                    inputHandler.SetActionPlayerFaction(playerFaction);
                    return true;
                }
            }
        }
        return false;
    }

    private void ExecuteAllCommands()
    {
        foreach(Command command in commandQueue)
        {
            command.ExecuteCommand();
        }
    }
    private void ResetTurn()
    {
        foreach(Faction faction in factions)
        {
            faction.ResetTurn();
        }
        currentTurnNumber++;
    }
}
