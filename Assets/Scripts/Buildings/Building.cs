using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    Region home;

    public void AssignHomeRegion(Region home)
    {
        this.home = home;
    }
}
