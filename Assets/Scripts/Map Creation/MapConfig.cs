using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapConfig : MonoBehaviour
{
    int mapWidth = 30;
    int mapHeight = 30;
    
    void Start()
    {
        List<MapGenerationStage> stages = new List<MapGenerationStage>();
        stages.Add(new GenerateSingleContinent(mapWidth, mapHeight, 2f, 2, 0.05f, 0.2f, new float[1] {0.1f}));
        stages.Add(new GenerateMountains(mapWidth, mapHeight, 2, 0.2f, 0.3f, new float[2] { 0.75f, 0.55f }));
        stages.Add(new GenerateRivers(mapWidth, mapHeight));
        stages.Add(new GenerateForests(mapWidth, mapHeight, 2, 0.1f, 0.3f, new float[2] { 0.7f, 0.5f }));
        stages.Add(new GenerateRegions(mapWidth, mapHeight));

        FindObjectOfType<MapCreator>().SetMapGenerationStages(stages);
        FindObjectOfType<TurnManager>().EndTurn();
    }

}
