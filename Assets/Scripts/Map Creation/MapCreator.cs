using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCreator : MonoBehaviour
{

    [SerializeField] NodeManager nodeManager;
    [SerializeField] RegionManager regionManager;

    [SerializeField] TextMeshPro tileCoordinateLabel;

    [SerializeField] RiverNode riverNode;

    [SerializeField] Transform labelParent;
    bool labelsPlaced = false;

    public void SetMapGenerationStages(List<MapGenerationStage> stages)
    {
        foreach(MapGenerationStage stage in stages)
        {
            Debug.Log(stage.GetDescription());
            stage.GenerateMap();
        }

        PlaceSettlements();
    }

    private void PlaceTileCoordinateLabel(Vector3Int coordinate)
    {
        TextMeshPro label = Instantiate(tileCoordinateLabel, nodeManager.GetTilemap().GetCellCenterWorld(coordinate), Quaternion.identity, labelParent);
        label.text = ((Vector2Int)coordinate).ToString();
    }

    

    private void PlaceSettlements()
    {
        regionManager.PlaceSettlements();
    }
}


