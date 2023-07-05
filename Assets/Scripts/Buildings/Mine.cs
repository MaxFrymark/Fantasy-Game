using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : TileEconomicBuilding
{
    public override string GetObjectTag()
    {
        return "Mine";
    }

    public override void TakeAction()
    {
        treasury.AdjustResources(CalculateResourseToAdd());
    }

    public override Resource CalculateResourseToAdd()
    {
        return new Resource(Resource.ResourceType.Metal, workers.Count * 2);
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
        cost.Add(new Resource(Resource.ResourceType.Wood, 50));
    }

    public override bool IsBuildingAvailable()
    {
        return true;
    }
}
