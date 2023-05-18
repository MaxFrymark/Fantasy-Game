using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapConfig : MonoBehaviour
{
    GenerateLandmass generateLandmass;
    GenerateMountains generateMountains;
    GenerateRivers generateRivers;
    GenerateForests generateForests;
    GenerateRegions generateRegions;

    int mapWidth = 30;
    int mapHeight = 30;
    
    void Start()
    {
        generateLandmass = new GenerateSingleContinent(mapWidth, mapHeight, 2f, 2, 0.01f, 0.02f, new float[1] {0.1f});
        generateMountains = new GenerateMountains(mapWidth, mapHeight, 2, 0.2f, 0.3f, new float[2] { 0.75f, 0.55f });
        generateRivers = new GenerateRivers(mapWidth, mapHeight);
        generateForests = new GenerateForests(mapWidth, mapHeight, 2, 0.1f, 0.2f, new float[2] { 0.65f, 0.45f });
        generateRegions = new GenerateRegions(mapWidth, mapHeight);

        FindObjectOfType<MapCreator>().SetMapGenerationStages(generateLandmass, generateMountains, generateRivers, generateForests, generateRegions);
    }

}
