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
    [SerializeField] TextMeshProUGUI foodProjection;
    [SerializeField] TextMeshProUGUI woodCounter;
    [SerializeField] TextMeshProUGUI woodProjection;
    [SerializeField] TextMeshProUGUI metalCounter;
    [SerializeField] TextMeshProUGUI metalProjection;
    [SerializeField] TextMeshProUGUI armsCounter;
    [SerializeField] TextMeshProUGUI armsProjection;

    public void UpdateTurnCounter()
    {
        Calendar calendar = Calendar.Singleton;
        yearCounter.text = "Year: " + calendar.GetYear().ToString();
        monthCounter.text = "Month: " + calendar.GetMonth().ToString();
    }

    public void UpdateActivePlayer(PlayerFaction faction)
    {
        
        playerFlag.color = faction.GetFactionColor();
        playerName.text = faction.GetFactionName();
        UpdateResourceDisplay(faction.GetCapitol().GetTreasury());
        UpdateTurnCounter();
    }

    private void UpdateResourceDisplay(Treasury treasury)
    {
        foreach(Resource resource in treasury.GetResources())
        {
            switch(resource.GetResourceType)
            {
                case Resource.ResourceType.Food:
                    UpdateDisplay(foodCounter, resource);
                    break;
                case Resource.ResourceType.Wood:
                    UpdateDisplay(woodCounter, resource);
                    break;
                case Resource.ResourceType.Metal:
                    UpdateDisplay(metalCounter, resource);
                    break;
                case Resource.ResourceType.HeavyArms:
                    UpdateDisplay(armsCounter, resource);
                    break;
            }
        }
        CalculateProjectedIncome(treasury.GetEconomicObjects());
    }

    private void UpdateDisplay(TextMeshProUGUI display, Resource resource)
    {
        display.text = resource.ToString();
    }

    private void CalculateProjectedIncome(List<IEconomicObject> economicObjects)
    {
        List<Resource> incomes = new List<Resource>();
        foreach(IEconomicObject economicObject in economicObjects)
        {
            if(economicObject.CalculateUpkeep() != null)
            {
                foreach(Resource resource in economicObject.CalculateUpkeep())
                {
                    AddToIncomesList(incomes, resource);
                }
            }

            if(economicObject is IIncomeSource)
            {
                IIncomeSource incomeSource = economicObject as IIncomeSource;
                AddToIncomesList(incomes, incomeSource.CalculateResourseToAdd());
            }
        }

        UpdateProjectionDisplay(incomes);
    }

    private void AddToIncomesList(List<Resource> incomes, Resource resource)
    {
        if(incomes.Count == 0)
        {
            incomes.Add(resource);
        }

        else
        {
            bool addToExistingResource = false;
            for(int i = 0; i  < incomes.Count; i++)
            {
                if(incomes[i].GetResourceType == resource.GetResourceType)
                {
                    incomes[i] += resource;
                    addToExistingResource = true;
                }
            }
            if (!addToExistingResource)
            {
                incomes.Add(resource);
            }
        }
    }

    private void UpdateProjectionDisplay(List<Resource> incomes)
    {
        foreach(Resource resource in incomes)
        {
            switch(resource.GetResourceType)
            {
                case Resource.ResourceType.Food:
                    UpdateProjection(foodProjection, resource); 
                    break;
                case Resource.ResourceType.Wood:
                    UpdateProjection(woodProjection, resource);
                    break;
                case Resource.ResourceType.Metal:
                    UpdateProjection(metalProjection, resource);
                    break;
                case Resource.ResourceType.HeavyArms:
                    UpdateProjection(armsProjection, resource);
                    break;
            }
        }
    }

    private void UpdateProjection(TextMeshProUGUI display, Resource resource)
    {
        if(resource.Quantity >= 0)
        {
            display.color = Color.green;
            display.text = "+" + resource.Quantity;
        }

        else
        {
            display.color = Color.red;
            display.text = "-" + resource.Quantity;
        }
    }
}
