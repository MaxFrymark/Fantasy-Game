using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    PlayerFaction activePlayerFaction;
    NodeManager nodeManager;

    bool readingMouse = false;

    private delegate void MouseAction();
    private MouseAction mouseAction;

    [SerializeField] List<TileBuilding> buildingPrefabs;
    TileBuilding activeBuildingPrefab;

    [SerializeField] BuildingUnderConstruction buildingUnderConstructionPrefab;
    [SerializeField] BuildingPlacer buildingPlacerPrefab;

    BuildingPlacer buildingPlacer;
    TileBuilding tempBuilding;

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
        /*if(Input.GetMouseButton(0))
        {
            TileNode node = FindMousePosition();
            Debug.Log(node.GetNodeTerrainData().GetTerrainType());
        }*/

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

    public void SetToBuildBuilding(int index)
    {
        activeBuildingPrefab = buildingPrefabs[index];

        mouseAction = BuildBuilding;
        readingMouse = true;
    }

    private void BuildBuilding()
    {
        if (tempBuilding == null)
        {
            tempBuilding = Instantiate(activeBuildingPrefab, new Vector3(1000, 0, 0), Quaternion.identity);
        }

        if (buildingPlacer == null)
        {
            buildingPlacer = Instantiate(buildingPlacerPrefab);
            buildingPlacer.SetSprite(tempBuilding.GetBlankSprite());
        }

        
        TileNode currentNode = FindMousePosition();
        bool validPlacement = ValidatePlacement(currentNode);
        buildingPlacer.FindValidBuildingPlacement(currentNode, validPlacement);

        if (Input.GetMouseButtonDown(0))
        {
            if (validPlacement)
            {
                BuildingUnderConstruction buildingUnderConstruction = Instantiate(buildingUnderConstructionPrefab, buildingPlacer.transform.position, Quaternion.identity);
                buildingUnderConstruction.SetSprite(tempBuilding.GetBlankSprite());
                buildingUnderConstruction.UpdateBuildingCountDown(tempBuilding.GetConstructionTime());
                Destroy(buildingPlacer.gameObject);
                activePlayerFaction.ReceiveCommandFromInput(new BuildBuildingCommand(buildingUnderConstruction, tempBuilding.gameObject, tempBuilding.GetConstructionTime()));
                tempBuilding.gameObject.SetActive(false);
                readingMouse = false;
                tempBuilding = null;
                return;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Destroy(tempBuilding.gameObject);
            Destroy(buildingPlacer.gameObject);
            readingMouse = false;
        }
    }

    private bool ValidatePlacement(TileNode currentNode)
    {
        return tempBuilding.CheckIfTerrainValid(currentNode.GetNodeTerrainData()) && currentNode.GetRegion().GetOwner() == activePlayerFaction && currentNode.GetBuilding() == null;
    }
}
