using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CityManagementUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI idleWorkerCounterTMP;
    [SerializeField] TextMeshProUGUI populationCounterTMP;
    [SerializeField] GameObject cityBuildingMenu;

    [SerializeField] TextMeshProUGUI growthPerTurnTMP;
    [SerializeField] TextMeshProUGUI currentGrowthTMP;
    [SerializeField] TextMeshProUGUI targetGrowthTMP;
    [SerializeField] TextMeshProUGUI turnsUntilPopulationGrowthTMP;

    City city;

    public void OpenCityMangement(City city)
    {
        this.city = city;
        UpdateIdleWorkerCount();
        UpdatePopulationGrowthUI();
    }

    public void UpdateIdleWorkerCount()
    {
        populationCounterTMP.text = "Population: " + city.GetWorkers().Count;
        int idleWorkers = 0;
        foreach(Pop pop in city.GetWorkers())
        {
            if(pop.GetJob() == null)
            {
                idleWorkers++;
            }
        }

        idleWorkerCounterTMP.text = "Idle Workers: " + idleWorkers;
    }

    public void OpenOrCloseCityBuildingMenu()
    {
        cityBuildingMenu.SetActive(!cityBuildingMenu.activeInHierarchy);
    }

    public void UpdatePopulationGrowthUI()
    {
        int growthPerTurn = city.GetGrowthPerTurn();
        int currentGrowth = city.GetCurrentGrowth();
        int targetGrowth = city.GetGrowthTarget();
        DisplayGrowthPerTurn(growthPerTurn);
        DisplayCurrenGrowth(currentGrowth);
        DisplayTargetGrowth(targetGrowth);
        DisplayTurnsUntilPopulationGrowth(growthPerTurn, currentGrowth, targetGrowth);
    }

    private void DisplayGrowthPerTurn(int growthPerTurn)
    {
        if(growthPerTurn < 0)
        {
            growthPerTurnTMP.text = "Growth Per Turn: " + growthPerTurn + " Starvation!";
        }

        else
        {
            growthPerTurnTMP.text = "Growth Per Turn: +" + growthPerTurn; 
        }
    }

    private void DisplayCurrenGrowth(int currentGrowth)
    {
        currentGrowthTMP.text = "Current Growth: " + currentGrowth;
    }

    private void DisplayTargetGrowth(int targetGrowth)
    {
        targetGrowthTMP.text = "Target Growth: " + targetGrowth;
    }

    private void DisplayTurnsUntilPopulationGrowth(int growthPerTurn,  int currentGrowth, int targetGrowth)
    {
        if (growthPerTurn <= 0)
        {
            turnsUntilPopulationGrowthTMP.text = "Turns Until Population Growth: --";
        }

        else 
        {
            int growthRemaining = targetGrowth - currentGrowth;
            int remainingTurns = growthRemaining / growthPerTurn;
            turnsUntilPopulationGrowthTMP.text = "Turns Until Population Growth: " + remainingTurns;
        }

    }

}
