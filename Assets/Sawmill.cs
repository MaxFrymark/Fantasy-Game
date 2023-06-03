using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sawmill : TileEconomicBuilding
{
    public override void TakeAction()
    {
        treasury.AdjustResources(Treasury.ResourceType.Wood, workers.Count * 3);
    }
}
