using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionButtonCostField : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI cost;
    [SerializeField] Image icon;

    public void SetUpCostField(Resource resource)
    {
        cost.text = resource.ToString();
    }

    public void CanAffordCost(bool canAffordCost)
    {
        if (!canAffordCost)
        {
            cost.color = Color.red;
        }

        else
        {
            cost.color = Color.white;
        }
    }
}
