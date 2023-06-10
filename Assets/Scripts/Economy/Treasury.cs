using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasury
{
    List<Resource> resources = new List<Resource>();

    private List<IEconomicObject> economicObjects = new List<IEconomicObject>();

    public Treasury() 
    {
        UnityEngine.Object.FindObjectOfType<TurnManager>().OnUpdateEconomy += EconomicActivityForTurn;

        resources.Add(new Resource(Resource.ResourceType.Food, 0));
        resources.Add(new Resource(Resource.ResourceType.Metal, 0));
        resources.Add(new Resource (Resource.ResourceType.Wood, 0));
    }

    public void AdjustResources(Resource resource)
    {
        bool resourceAdded = false;
        for(int i = 0; i < resources.Count; i++)
        {
            if (resources[i].GetResourceType == resource.GetResourceType)
            {
                resourceAdded = true;
                resources[i] += resource;
                break;
            }
        }
        if(!resourceAdded)
        {
            AddResource(resource);
        }
    }

    public Resource GetResource(Resource.ResourceType resourceType)
    {
        Resource resourseToReturn = new Resource(Resource.ResourceType.None, 0);
        foreach (Resource r in resources)
        {
            if(r.GetResourceType == resourceType)
            {
                resourseToReturn = r;
            }
        }

        return resourseToReturn;
    }

    private void AddResource(Resource resource)
    {
        resources.Add(resource);
    }

    public void EconomicActivityForTurn(object sender, EventArgs e)
    {
        foreach (IEconomicObject economicObject in economicObjects)
        {
            economicObject.TakeAction();
        }
    }

    public void AddEconomicObject(IEconomicObject economicObject)
    {
        economicObjects.Add(economicObject);
        economicObjects.Sort();
    }
}
