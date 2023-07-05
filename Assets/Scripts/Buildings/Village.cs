using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : Settlement
{
    public override string GetObjectTag()
    {
        return "Village";
    }

    public override void ActivateBuilding()
    {
        base.ActivateBuilding();
        GetTreasury().AdjustResources(new Resource(Resource.ResourceType.Food, 100));
    }

    public override void SetUpConstructionCost(List<Resource> cost)
    {
        return;
    }


}
