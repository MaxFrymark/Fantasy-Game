using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuilding
{

    void AssignHomeRegion(Region home);
    Region GetHomeRegion();

    int GetConstructionTime();

    void ActivateBuilding();

}
