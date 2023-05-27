using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    PlayerFaction activePlayerFaction;
    NodeManager nodeManager;

    bool readingMouse = false;

    private delegate void MouseAction();
    private MouseAction mouseAction;

    [SerializeField] GameObject farmPrefab;
    [SerializeField] BuildingUnderConstruction farmUnderConstructionPrefab;
    [SerializeField] BuildingPlacer farmPlacerPrefab;
    BuildingPlacer buildingPlacer;

    private void Start()
    {
        nodeManager = FindObjectOfType<NodeManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            activePlayerFaction.ReceiveCommandFromInput(new TestCommandWithTimer(activePlayerFaction));
        }
        if (readingMouse)
        {
            mouseAction();
        }
    }

    public void SetActivePlayerFaction(PlayerFaction playerFaction)
    {
        activePlayerFaction = playerFaction;
        Debug.Log("Player " + playerFaction.GetFactionName() + " is active.");
    }

    public void PlayerEndedTurn()
    {
        activePlayerFaction.EndFactionTurn();
    }

    private TileNode FindMousePosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return nodeManager.FindClosestNodeToWorldPostition(mousePosition);
    }

    public void SetToBuildBuilding()
    {
        mouseAction = BuildBuilding;
        readingMouse = true;
    }

    private void BuildBuilding()
    {
        if(buildingPlacer == null)
        {
            buildingPlacer = Instantiate(farmPlacerPrefab);
        }

        bool validPlacement = buildingPlacer.FindValidBuildingPlacement(FindMousePosition());
        
        if(Input.GetMouseButtonDown(0))
        {
            if (validPlacement)
            {
                Instantiate(farmUnderConstructionPrefab, buildingPlacer.transform.position, Quaternion.identity);
                Destroy(buildingPlacer.gameObject);
                readingMouse = false;
                return;
            }
        }
    }
}
