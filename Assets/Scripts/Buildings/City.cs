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
                foreach (IBuilding building in buildings)
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
        foreach (IBuilding building in buildings)
        {
            if (building is TileEconomicBuilding)
            {
                TileEconomicBuilding economicBuilding = building as TileEconomicBuilding;
                economicBuilding.HandleWorkerManagement(false);
            }
        }
    }

    public override void UpdateSettlementManagerUI()
    {
        if (cityManagementUI.gameObject.activeSelf)
        {
            cityManagementUI.UpdateIdleWorkerCount();
        }
    }
}
