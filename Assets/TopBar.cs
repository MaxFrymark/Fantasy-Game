using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI turnCounter;

    [SerializeField] Image playerFlag;
    [SerializeField] TextMeshProUGUI playerName;

    [SerializeField] TextMeshProUGUI foodCounter;
    [SerializeField] TextMeshProUGUI woodCounter;
    [SerializeField] TextMeshProUGUI metalCounter;

    public void UpdateTurnCounter(int turn)
    {
        turnCounter.text = "Turn: " + turn.ToString();
    }

    public void UpdateActivePlayer(PlayerFaction faction)
    {
        playerFlag.color = faction.GetFactionColor();
        playerName.text = faction.GetFactionName();
        UpdateResourceDisplay(faction.GetCapitol().GetTreasury());
    }

    private void UpdateResourceDisplay(Treasury treasury)
    {
        foodCounter.text = treasury.GetFoodQuantity().ToString();
        woodCounter.text = treasury.GetWoodQuantity().ToString();
        metalCounter.text = treasury.GetMetalQuantity().ToString();
    }
}
