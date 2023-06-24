using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCounter : MonoBehaviour
{
    [SerializeField] Image counterSprite;
    [SerializeField] TextMeshProUGUI resourceCount;
    [SerializeField] TextMeshProUGUI resourceIncome;
    
    Resource.ResourceType counterTag;

    public Resource.ResourceType GetCounterTag()
    {
        return counterTag;
    }

    public void SetUpCounter(Sprite sprite, Resource resource)
    {
        counterSprite.sprite = sprite;
        counterTag = resource.GetResourceType;
        UpdateResourceCount(resource);
    }

    public void UpdateResourceCount(Resource resource)
    {
        resourceCount.text = resource.ToString();
    }

    public void UpdateResourceIncome(Resource resource)
    {
        if(resource.Quantity >= 0)
        {
            resourceIncome.color = Color.green;
            resourceIncome.text = "+" + resource;
        }

        else
        {
            resourceIncome.color = Color.red;
            resourceIncome.text = "-" + resource;
        }
    }
}
