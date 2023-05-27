using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingUnderConstruction : MonoBehaviour
{
    [SerializeField] TextMeshPro buildingCountDown;
    [SerializeField] SpriteRenderer spriteRenderer;

    public void UpdateBuildingCountDown(int remainingTurns)
    {
        buildingCountDown.text = remainingTurns.ToString();
    }
}
