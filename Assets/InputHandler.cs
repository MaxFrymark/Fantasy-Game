using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    PlayerFaction activePlayerFaction;

    public void SetActionPlayerFaction(PlayerFaction playerFaction)
    {
        activePlayerFaction = playerFaction;
    }
}
