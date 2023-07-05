using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpriteHolder : MonoBehaviour
{
    [SerializeField] Sprite foodSprite;
    [SerializeField] Sprite metalSprite;
    [SerializeField] Sprite woodSprite;
    [SerializeField] Sprite heavyArmsSprite;

    public Sprite GetResourceSprite(Resource resource)
    {
        switch (resource.GetResourceType)
        {
            case Resource.ResourceType.Food:
                return foodSprite;
            case Resource.ResourceType.Metal:
                return metalSprite;
            case Resource.ResourceType.Wood:
                return woodSprite;
            case Resource.ResourceType.HeavyArms:
                return heavyArmsSprite;
        }

        return null;
    }
}
