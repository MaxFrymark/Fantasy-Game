using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forge : CityBuilding
{
    
    public Forge(Faction owner, Region homeRegion) : base(owner, homeRegion)
    {
        buildingName = "Forge";
    }

    

    public override void SetUpConstructionCost(List<Resource> constructionCost)
    {
        constructionCost.Add(new Resource(Resource.ResourceType.Wood, 25));
    }

    public override List<Resource> CalculateUpkeep()
    {
        List<Resource> upkeep = new List<Resource>() { new Resource(Resource.ResourceType.Metal, -workers.Count) };
        return upkeep;
    }

    public override Resource CalculateResourseToAdd()
    {
        if(treasury.GetResource(Resource.ResourceType.Metal).Quantity >= Mathf.Abs(CalculateUpkeep()[0].Quantity)) 
        {
            return new Resource(Resource.ResourceType.HeavyArms, workers.Count);
        }

        else
        {
            return Resource.none;
        }
    }

    public override void TakeAction()
    {
        treasury.AdjustResources(CalculateResourseToAdd());
        treasury.AdjustResources(CalculateUpkeep()[0]);
    }

    public override bool IsBuildingAvailable(Faction faction)
    {
        return true;
    }

}
