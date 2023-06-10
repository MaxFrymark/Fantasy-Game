using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public struct Resource
{
    public enum ResourceType { None, Food, Metal, Wood }

    private ResourceType resourceType;
    public ResourceType GetResourceType { get { return resourceType; } }
    private int quantity;
    public int Quantity { get { return quantity; } }


    public static Resource operator + (Resource left, Resource right)
    {
        left.quantity += right.quantity;
        return left;
    }

    public static Resource operator - (Resource left, Resource right)
    {
        left.quantity -= right.quantity;
        return left;
    }

    public Resource(ResourceType resourceType, int quantity)
    {
        this.resourceType = resourceType;
        this.quantity = quantity;
    }
}
