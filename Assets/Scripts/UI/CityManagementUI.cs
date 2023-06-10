using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CityManagementUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI idleWorkerCounter;
    City city;

    public void OpenCityMangement(City city)
    {
        this.city = city;
        UpdateIdleWorkerCount();
    }

    public void UpdateIdleWorkerCount()
    {
        int idleWorkers = 0;
        foreach(Pop pop in city.GetWorkers())
        {
            if(pop.GetJob() == null)
            {
                idleWorkers++;
            }
        }

        idleWorkerCounter.text = "Idle Workers: " + idleWorkers;
    }

    public void CloseCityManagement()
    {
        city.CloseMenu();
        gameObject.SetActive(false);
    }
}
