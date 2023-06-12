using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : Settlement
{
    [SerializeField] SpriteRenderer cityFlag;
    Faction owner;
    InputHandler inputHandler;

    protected override void Start()
    {
        base.Start();
        if(owner is PlayerFaction)
        {
            inputHandler = gameObject.AddComponent<InputHandler>();
        }
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
}
