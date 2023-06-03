using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddWorker : MonoBehaviour
{
    IEconomicBuilding economicBuilding;

    private void Start()
    {
        economicBuilding = transform.parent.GetComponentInParent<IEconomicBuilding>();
    }

    private void OnMouseDown()
    {
        economicBuilding.AddWorker();
    }
}
