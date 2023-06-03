using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : Settlement
{
    [SerializeField] SpriteRenderer cityFlag;
    Faction owner;
    CityManagementUI cityManagementUI;

    protected override void Start()
    {
        base.Start();
        cityManagementUI = FindObjectOfType<CityManagementUI>(true);
    }

    public void SetCityOwner(Faction faction)
    {
        owner = faction;
        cityFlag.color = faction.GetFactionColor();
    }

    private void OnMouseDown()
    {
        if (owner is PlayerFaction)
        {
            PlayerFaction faction = (PlayerFaction)owner;
            if (faction.GetActiveFaction())
            {
                cityManagementUI.gameObject.SetActive(true);
                cityManagementUI.OpenCityMangement(this);
                foreach (IEconomicBuilding building in economicBuildings)
                {
                    if (building is TileEconomicBuilding)
                    {
                        TileEconomicBuilding economicBuilding = building as TileEconomicBuilding;
                        economicBuilding.HandleWorkerManagement(true);
                    }
                }
            }
        }
    }

    public void CloseMenu()
    {
        foreach (IEconomicBuilding building in economicBuildings)
        {
            if (building is TileEconomicBuilding)
            {
                TileEconomicBuilding economicBuilding = building as TileEconomicBuilding;
                economicBuilding.HandleWorkerManagement(false);
            }
        }
    }
}
