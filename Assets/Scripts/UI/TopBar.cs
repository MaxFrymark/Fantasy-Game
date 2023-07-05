using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopBar : MonoBehaviour
{
    [SerializeField] ResourceSpriteHolder spriteHolder;
    
    [SerializeField] TextMeshProUGUI yearCounter;
    [SerializeField] TextMeshProUGUI monthCounter;

    [SerializeField] Image playerFlag;
    [SerializeField] TextMeshProUGUI playerName;

    Calendar calendar;

    [SerializeField] Transform[] resourceCounterPoints;
    int curentCounterPoint = 0;

    [SerializeField] ResourceCounter ressourceCounterPrefab;
    List<ResourceCounter> resourceCounterList = new List<ResourceCounter>();

    public void UpdateTurnCounter()
    {
        if(calendar == null)
        {
            calendar = Calendar.Singleton;
        }
        
        yearCounter.text = "Year: " + calendar.GetYear().ToString();
        monthCounter.text = "Month: " + calendar.GetMonth().ToString();
    }

    public void UpdateActivePlayer(PlayerFaction faction)
    {
        playerFlag.color = faction.GetFactionColor();
        playerName.text = faction.GetFactionName();
        UpdateResourceDisplay(faction.GetCapitol().GetTreasury());
        CalculateProjectedIncome(faction.GetCapitol().GetTreasury().GetEconomicObjects());
        UpdateTurnCounter();
    }

    private void UpdateResourceDisplay(Treasury treasury)
    {
        foreach(Resource resource in treasury.GetResources())
        {
            GetMatchingResourceCounter(resource).UpdateResourceCount(resource);
        }
    }

    private ResourceCounter GetMatchingResourceCounter(Resource resource)
    {
        foreach(ResourceCounter resourceCounter in resourceCounterList)
        {
            if(resourceCounter.GetCounterTag() == resource.GetResourceType)
            {
                return resourceCounter;
            }
        }

        ResourceCounter newResourceCounter = Instantiate(ressourceCounterPrefab, resourceCounterPoints[curentCounterPoint].position, Quaternion.identity, transform);
        newResourceCounter.SetUpCounter(spriteHolder.GetResourceSprite(resource), resource);
        resourceCounterList.Add(newResourceCounter);
        curentCounterPoint++;
        return newResourceCounter;
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

        foreach(Resource income in incomes)
        {
            GetMatchingResourceCounter(income).UpdateResourceIncome(income);
        }
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
