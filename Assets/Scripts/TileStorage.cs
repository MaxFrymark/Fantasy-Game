using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileStorage : MonoBehaviour
{
    [SerializeField] Tile[] oceanTiles;
    [SerializeField] Tile[] plainsTiles;
    [SerializeField] Tile[] mountainTiles;
    [SerializeField] Tile[] hillsTiles;

    public Tile[] GetOceanTiles()
    {
        return oceanTiles;
    }

    public Tile[] GetPlainsTiles()
    {
        return plainsTiles;
    }

    public Tile[] GetMountainTiles()
    {
        return mountainTiles;
    }

    public Tile[] GetHillsTiles()
    {
        return hillsTiles;
    }
}
