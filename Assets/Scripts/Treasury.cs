using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasury
{
    public enum ResourceType { Food, Metal, Wood }

    int food = 0;
    int metal = 0;
    int wood = 0;

    public void AdjustResources(ResourceType resource, int quantity)
    {
        switch (resource)
        {
            case ResourceType.Food:
                food += quantity;
                break;
            case ResourceType.Metal:
                metal += quantity; 
                break;
            case ResourceType.Wood: 
                wood += quantity; 
                break;
        }
    }

    public int GetFoodQuantity()
    {
        return food;
    }

    public int GetMetalQuantity()
    {
        return metal;
    }

    public int GetWoodQuantity()
    {
        return wood;
    }
}
