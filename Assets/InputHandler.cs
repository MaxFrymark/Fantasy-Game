using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    PlayerFaction activePlayerFaction;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            activePlayerFaction.ReceiveCommandFromInput(new TestCommand(activePlayerFaction));
        }
    }

    public void SetActivePlayerFaction(PlayerFaction playerFaction)
    {
        activePlayerFaction = playerFaction;
        Debug.Log("Player " + playerFaction.GetFactionName() + " is active.");
    }

    public void PlayerEndedTurn()
    {
        activePlayerFaction.EndFactionTurn();
    }
}
