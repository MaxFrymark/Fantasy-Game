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

    [SerializeField] GameObject toolTip;
    bool toolTipCoroutinerunning = false;
    private Coroutine toolTipCoroutine;

    BuildingPlacer buildingPlacer;
    TileBuilding tempBuilding;

    Vector3 currentMousePosition;

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

        else
        {
            /*if (currentMousePosition == Camera.main.ScreenToWorldPoint(Input.mousePosition))
            {
                if (!toolTipCoroutinerunning && !toolTip.activeInHierarchy)
                {
                    toolTipCoroutine = StartCoroutine(EnableToolTip());
                }
            }

            else
            {
                if (toolTip.activeInHierarchy)
                {
                    toolTip.SetActive(false);
                }

                if (toolTipCoroutinerunning)
                {
                    StopCoroutine(toolTipCoroutine);
                    toolTipCoroutine = null;
                    toolTipCoroutinerunning = false;
                }

                currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }*/
        }
    }

    public void SetActivePlayerFaction(PlayerFaction playerFaction)
    {
        if(activePlayerFaction != null)
        {
            activePlayerFaction.SetActiveFaction(false);
        }
        activePlayerFaction = playerFaction;
        activePlayerFaction.SetActiveFaction(true);
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

    private IEnumerator EnableToolTip()
    {
        toolTipCoroutinerunning = true;
        yield return new WaitForSeconds(2f);
        toolTip.SetActive(true);
        toolTipCoroutinerunning = false;
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
                activePlayerFaction.ReceiveCommandFromInput(new BuildBuildingCommand(buildingUnderConstruction, tempBuilding, tempBuilding.GetConstructionTime()));
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
