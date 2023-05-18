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

    GenerateLandmass generateLandmass;
    GenerateMountains generateMountains;
    GenerateRivers generateRivers;
    GenerateForests generateForests;
    GenerateRegions generateRegions;


    public void SetMapGenerationStages(GenerateLandmass generateLandmass, GenerateMountains generateMountains, GenerateRivers generateRivers,
         GenerateForests generateForests, GenerateRegions generateRegions)
    {
        this.generateLandmass = generateLandmass;
        this.generateMountains = generateMountains;
        this.generateRivers = generateRivers;
        this.generateForests = generateForests;
        this.generateRegions = generateRegions;

        CreateMap();
    }

    void CreateMap()
    {
        Debug.Log("Drawing Continent");
        generateLandmass.GenerateMap();
        Debug.Log("Drawing Mountains");
        generateMountains.GenerateMap();
        Debug.Log("Drawing Rivers");
        generateRivers.GenerateMap();
        Debug.Log("Drawing Forests");
        generateForests.GenerateMap();
        Debug.Log("Creating Regions");
        generateRegions.GenerateMap();
        Debug.Log("Placeing Settlements");
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


