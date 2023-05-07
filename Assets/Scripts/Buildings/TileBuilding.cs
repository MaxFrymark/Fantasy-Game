using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileBuilding : Building
{
    TileNode homeTile;

    public void AssigneHomeTile(TileNode homeTile)
    {
        this.homeTile = homeTile;
    }
}
