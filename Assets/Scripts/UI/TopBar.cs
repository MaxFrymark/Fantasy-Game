using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI yearCounter;
    [SerializeField] TextMeshProUGUI monthCounter;

    [SerializeField] Image playerFlag;
    [SerializeField] TextMeshProUGUI playerName;

    [SerializeField] TextMeshProUGUI foodCounter;
    [SerializeField] TextMeshProUGUI woodCounter;
    [SerializeField] TextMeshProUGUI metalCounter;

    public void UpdateTurnCounter(int year, int month)
    {
        yearCounter.text = "Year: " + year.ToString();
        monthCounter.text = "Month: " + month.ToString();
    }

    public void UpdateActivePlayer(PlayerFaction faction)
    {
        playerFlag.color = faction.GetFactionColor();
        playerName.text = faction.GetFactionName();
        UpdateResourceDisplay(faction.GetCapitol().GetTreasury());
    }

    private void UpdateResourceDisplay(Treasury treasury)
    {
        foodCounter.text = treasury.GetResource(Resource.ResourceType.Food).Quantity.ToString();
        woodCounter.text = treasury.GetResource(Resource.ResourceType.Wood).Quantity.ToString();
        metalCounter.text = treasury.GetResource(Resource.ResourceType.Metal).Quantity.ToString();
    }
}
