using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Settlement : TileBuilding
{
    [SerializeField] TextMeshPro settlementNameField;
    string settlementName;

    public void SetName(string settlementName)
    {
        this.settlementName = settlementName;
        settlementNameField.text = settlementName;
    }
}
