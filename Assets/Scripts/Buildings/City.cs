using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : Settlement
{
    [SerializeField] SpriteRenderer cityFlag;

    public void SetCityFlagColor(Color color)
    {
        cityFlag.color = color;
    }
}
