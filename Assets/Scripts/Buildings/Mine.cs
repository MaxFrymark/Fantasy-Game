using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : TileEconomicBuilding
{
    
    
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
}
