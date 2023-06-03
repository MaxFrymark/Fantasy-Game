using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : TileEconomicBuilding
{
    public override void TakeAction()
    {
        treasury.AdjustResources(Treasury.ResourceType.Metal, workers.Count * 2);
    }
}
