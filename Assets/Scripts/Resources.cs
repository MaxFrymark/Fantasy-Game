using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Playables;

public struct Resource
{
    public enum ResourceType { None, Food, Metal, Wood, HeavyArms }

    private ResourceType resourceType;
    public ResourceType GetResourceType { get { return resourceType; } }
    private int quantity;
    public int Quantity { get { return quantity; } }

    public static readonly Resource none = new Resource(ResourceType.None, 0);

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

    public static Resource operator - (Resource right)
    {
        right.quantity = -right.quantity;
        return right;
    }

    public override string ToString()
    {
        return quantity.ToString();
    }

    public Resource(ResourceType resourceType, int quantity)
    {
        this.resourceType = resourceType;
        this.quantity = quantity;
    }
}
