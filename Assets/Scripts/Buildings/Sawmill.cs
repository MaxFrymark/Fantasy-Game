using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Sawmill : TileEconomicBuilding
{
    
    public override void TakeAction()
    {
        treasury.AdjustResources(CalculateResourseToAdd());
    }

    public override Resource CalculateResourseToAdd()
    {
        return new Resource(Resource.ResourceType.Wood, workers.Count * 3);
    }

    public override int GetPriority()
    {
        return 1;
    }

    public override List<Resource> CalculateUpkeep()
    {
        return null;
    }

    public override void SetUpConstructionCost(List<Resource> cost)
    {
        cost.Add(new Resource(Resource.ResourceType.Metal, 50));
    }
}
