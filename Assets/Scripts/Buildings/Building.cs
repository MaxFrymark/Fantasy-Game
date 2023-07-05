using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuilding
{
    string GetObjectTag();
    
    void AssignHomeRegion(Region home);
    Region GetHomeRegion();

    int GetConstructionTime();

    void ActivateBuilding();

    List<Resource> GetConstructionCost();

    void SetUpConstructionCost(List<Resource> constructionCost);

    bool IsBuildingAvailable();
}
