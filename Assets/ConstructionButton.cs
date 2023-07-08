using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI buildingName;
    [SerializeField] TextMeshProUGUI buildingConstructionTime;
    [SerializeField] Image buildingIcon;
    [SerializeField] ConstructionButtonCostField[] costFields;
    [SerializeField] GameObject cover;

    InputHandler inputHandler;

    IBuilding building;

    bool canAffordCost;

    public void SetUpButton(IBuilding building)
    {
        if (inputHandler == null)
        {
            FindObjectOfType<InputHandler>();
        }
        

        this.building = building;
        buildingName.text = building.GetBuildingName();
        buildingIcon.sprite = building.GetBuildingSprite();
        buildingConstructionTime.text = building.GetConstructionTime() + " Turns";
        ValidateCost();
        cover.SetActive(!canAffordCost);
    }

    private void ValidateCost()
    {
        if(inputHandler == null)
        {
            inputHandler = FindObjectOfType<InputHandler>();
        }
        
        canAffordCost = true;
        Treasury treasury = inputHandler.GetActivePlayer().GetCapitol().GetTreasury();
        List<Resource> constructionCost = building.GetConstructionCost();
        if (constructionCost != null && constructionCost.Count > 0)
        {
            for (int i = 0; i < constructionCost.Count; i++)
            {
                costFields[i].gameObject.SetActive(true);
                costFields[i].SetUpCostField(constructionCost[i]);
                if (!treasury.CheckCost(new List<Resource> { constructionCost[i] }))
                {
                    canAffordCost = false;
                    costFields[i].CanAffordCost(false);
                    cover.SetActive(true);
                }
                else
                {
                    costFields[i].CanAffordCost(true);
                }
            }
        }
    }

    public void SendBuildOrder()
    {
        Debug.Log("Building: " + building.GetBuildingName());
        if (canAffordCost)
        {
            inputHandler.SetToBuildBuilding(building);
        }
    }

    private void OnDisable()
    {
        foreach(ConstructionButtonCostField costField in costFields)
        {
            costField.gameObject.SetActive(false);
        }
        cover.SetActive(false);
    }
}
