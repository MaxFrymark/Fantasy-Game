using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : Settlement
{
    [SerializeField] SpriteRenderer cityFlag;
    Faction owner;
    InputHandler inputHandler;

    public override string GetObjectTag()
    {
        return "City";
    }

    public override void ActivateBuilding()
    {
        if (owner is PlayerFaction)
        {
            inputHandler = FindObjectOfType<InputHandler>();
        }
        base.ActivateBuilding();
        GetTreasury().AdjustResources(new Resource(Resource.ResourceType.Food, 200));
        GetTreasury().AdjustResources(new Resource(Resource.ResourceType.Wood, 50));
        GetTreasury().AdjustResources(new Resource(Resource.ResourceType.Metal, 50));
    }

    public void SetCityOwner(Faction faction)
    {
        owner = faction;
        cityFlag.color = faction.GetFactionColor();
    }

    private void OnMouseDown()
    {
        if(inputHandler != null)
        {
            inputHandler.OpenCityManager((PlayerFaction)owner);
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
        if(owner is PlayerFaction)
        {
            inputHandler.UpdateSettlementManagerUI();
        }
    }

    public override void SetUpConstructionCost(List<Resource> cost)
    {
        return;
    }
}
