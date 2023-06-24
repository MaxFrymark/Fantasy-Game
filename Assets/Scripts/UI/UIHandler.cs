using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using static UnityEditor.SceneView;

public class UIHandler : MonoBehaviour
{
    [SerializeField] CameraMover cameraMover;
    
    [SerializeField] TopBar topBar;
    
    [SerializeField] GameObject endTurnButton;
    [SerializeField] GameObject buildMenuButton;
    [SerializeField] GameObject buildMenu;
    [SerializeField] CityManagementUI cityManager;

    public void OpenBuildMenu()
    {
        buildMenu.SetActive(!buildMenu.activeInHierarchy);
    }

    public void OpenCityManager(Faction faction)
    {
        endTurnButton.SetActive(false);
        buildMenuButton.SetActive(false);
        buildMenu.SetActive(false);
        
        cityManager.gameObject.SetActive(true);
        cityManager.OpenCityMangement(faction.GetCapitol());
        cameraMover.SetCameraTarget(faction.GetCapitol().transform.position);
        cameraMover.SetCameraLock(true);
        foreach (IBuilding building in faction.GetCapitol().GetBuildingList())
        {
            if (building is TileEconomicBuilding)
            {
                TileEconomicBuilding economicBuilding = building as TileEconomicBuilding;
                economicBuilding.HandleWorkerManagement(true);
            }
        }
    }

    public void UpdateSettlementManager(PlayerFaction faction)
    {
        if (cityManager.gameObject.activeSelf)
        {
            cityManager.UpdateIdleWorkerCount();
            topBar.UpdateActivePlayer(faction);
        }
    }

    public void CloseCityManager(PlayerFaction faction)
    {
        endTurnButton.SetActive(true);
        buildMenuButton.SetActive(true);

        cameraMover.SetCameraLock(false);
        faction.GetCapitol().CloseMenu();
        cityManager.gameObject.SetActive(false);
    }

    public void UpdateTopBar(PlayerFaction faction)
    {
        topBar.UpdateActivePlayer(faction);
    }
}
