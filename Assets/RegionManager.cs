using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RegionManager : MonoBehaviour
{
    [SerializeField] Tilemap regionBorders;
    [SerializeField] RuleTile borderTile;

    [SerializeField] City city;
    [SerializeField] Village village;

    private List<Region> regions = new List<Region>();

    int minimumRegionSize = 7;
    int maximumRegionSize = 12;

    int numberOfCities = 6;

    public void CreateRegion(TileNode startingNode)
    {
        Region region = new Region();
        int regionSize = Random.Range(minimumRegionSize, maximumRegionSize);

        region.AddTileNode(startingNode);
        BuildRegion(regionSize, region);

        regions.Add(region);
    }

    private void BuildRegion(int regionSize, Region region)
    {
        regionSize -= AddNodesToRegion(regionSize, region.GetTilesInRegion()[Random.Range(0, region.GetTilesInRegion().Count)]);

        if (regionSize > 0)
        {
            if (AreAnyMoreSpacesOpen(region))
            {
                BuildRegion(regionSize, region);
            }
        }
    }

    private int AddNodesToRegion(int regionSize, TileNode node)
    {
        int tilesAdded = 0;
        foreach (TileNode neighboringNode in node.GetNodeNeighborData().GetNeighbors())
        {
            if (CanNodeBeAddedToRegion(regionSize, neighboringNode))
            {
                node.GetRegion().AddTileNode(neighboringNode);
                regionSize--;
                tilesAdded++;
            }
        }
        return tilesAdded;
    }

    private bool CanNodeBeAddedToRegion(int regionSize, TileNode neighboringNode)
    {
        if (regionSize == 0)
        {
            return false;
        }

        if (neighboringNode.GetNodeTerrainData().GetTerrainType() == TileNode.TerrainType.ocean)
        {
            return false;
        }

        if (neighboringNode.GetRegion() != null)
        {
            return false;
        }

        return true;
    }

    private bool AreAnyMoreSpacesOpen(Region region)
    {
        foreach (TileNode node in region.GetTilesInRegion())
        {
            foreach (TileNode neighboringNode in node.GetNodeNeighborData().GetNeighbors())
            {
                if (neighboringNode.GetNodeTerrainData().IsNodeOcean())
                {
                    continue;
                }

                if (neighboringNode.GetRegion() == null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public List<Region> GetRegions()
    {
        return regions;
    }

    public void DisolveExtraRegions()
    {
        DisolveTooSmallRegions();

        for(int i = regions.Count - 1; i >= 0; i--)
        {
            if(regions[i].GetTilesInRegion().Count == 0)
            {
                regions.RemoveAt(i);
            }
        }
    }

    private void DisolveTooSmallRegions()
    {
        foreach(Region region in regions)
        {
            if(region.GetTilesInRegion().Count < minimumRegionSize)
            {
                DisolveRegion(region);
            }
        }
    }

    private void DisolveRegion(Region region)
    {
        for(int i = region.GetTilesInRegion().Count - 1; i >= 0; i--)
        {
            TileNode tileNode = region.GetTilesInRegion()[i];
            if(tileNode.IsBorderTile())
            {
                List<Region> neighboringRegions = new List<Region>();
                foreach(TileNode neighboringNode in tileNode.GetNodeNeighborData().GetNeighbors())
                {
                    if (neighboringNode.GetNodeTerrainData().IsNodeOcean())
                    {
                        continue;
                    }
                    
                    Region nodeRegion = neighboringNode.GetRegion();
                    if(nodeRegion == region)
                    {
                        continue;
                    }

                    if (neighboringRegions.Contains(nodeRegion))
                    {
                        continue;
                    }

                    neighboringRegions.Add(nodeRegion);
                }

                if (neighboringRegions.Count == 1)
                {
                    tileNode.AssignRegion(neighboringRegions[0]);
                }

                else if(neighboringRegions.Count > 1)
                {
                    Region smallestNeighboringRegion = neighboringRegions[0];
                    foreach(Region neighboringRegion in neighboringRegions)
                    {
                        if (neighboringRegion.GetTilesInRegion().Count < smallestNeighboringRegion.GetTilesInRegion().Count)
                        {

                            smallestNeighboringRegion = neighboringRegion;
                        }
                    }

                    tileNode.AssignRegion(smallestNeighboringRegion);
                }

                else
                {
                    Debug.LogError("ERROR Disolving Region");
                }

                region.GetTilesInRegion().Remove(tileNode);
            }
        }

        if(region.GetTilesInRegion().Count > 0)
        {
            DisolveRegion(region);
        }
    }

    public void DrawBorders(Vector3Int coordinates)
    {
        regionBorders.SetTile(coordinates, borderTile);
    }

    public void ShrinkTooLargeRgeions()
    {
        List<Region> newRegions = new List<Region>();
        foreach(Region region in regions)
        {
            if(region.GetTilesInRegion().Count > maximumRegionSize)
            {
                if(region.GetTilesInRegion().Count >= 2 * minimumRegionSize)
                {
                    newRegions.Add(SplitRegion(region));
                }
            }
        }
        foreach(Region region in newRegions)
        {
            regions.Add(region);
        }
    }

    private Region SplitRegion(Region regionToSplit)
    {
        Region newRegion = new Region();
        TileNode startingNode = null;
        foreach(TileNode tileNode in regionToSplit.GetTilesInRegion())
        {
            if (tileNode.IsBorderTile())
            {
                startingNode = tileNode;
                newRegion.AddTileNode(startingNode);
                break;
            }
        }

        BuildRegionFromLargerRegion(minimumRegionSize - 1, newRegion, regionToSplit);

        return newRegion;
    }

    private void BuildRegionFromLargerRegion(int regionSize, Region region, Region startingRegion)
    {
        regionSize -= AddNodesToRegion(regionSize, region.GetTilesInRegion()[Random.Range(0, region.GetTilesInRegion().Count)], startingRegion);

        if (regionSize > 0)
        {
            if (AreAnyMoreSpacesOpen(region))
            {
                BuildRegionFromLargerRegion(regionSize, region, startingRegion);
            }
        }
    }

    private int AddNodesToRegion(int regionSize, TileNode node, Region startingRegion)
    {
        int tilesAdded = 0;
        foreach (TileNode neighboringNode in node.GetNodeNeighborData().GetNeighbors())
        {
            if (CanNodeBeAddedToRegion(regionSize, neighboringNode, startingRegion))
            {
                node.GetRegion().AddTileNode(neighboringNode);
                regionSize--;
                tilesAdded++;
            }
        }
        return tilesAdded;
    }

    private bool CanNodeBeAddedToRegion(int regionSize, TileNode neighboringNode, Region startingRegion)
    {
        if (regionSize == 0)
        {
            return false;
        }

        if (neighboringNode.GetNodeTerrainData().GetTerrainType() == TileNode.TerrainType.ocean)
        {
            return false;
        }

        if (neighboringNode.GetRegion() != startingRegion)
        {
            return false;
        }

        return true;
    }

    public void PlaceSettlements()
    {
        List<Region> occupiedRegions = new List<Region>();
        FactionCreator factionCreator = new FactionCreator();
        for(int i = 0; i < numberOfCities; i++)
        {
            occupiedRegions.Add(PlaceCity(occupiedRegions, factionCreator));
        }

        foreach(Region region in regions)
        {
            if (!occupiedRegions.Contains(region))
            {
                Village newVillage = Instantiate(village);
                newVillage.transform.position = NodeManager.Instance.GetWorldPostitionFromTileNode(region.SetSettlement(newVillage));
                newVillage.SetName(CityNameGenerator.Instance.GetRandomCityName());
                occupiedRegions.Add(region);
            }
        }
    }

    public Region PlaceCity(List<Region> occupiedRegions, FactionCreator factionCreator)
    {
        Region region = regions[Random.Range(0, regions.Count)];
        if (occupiedRegions.Contains(region))
        {
            return PlaceCity(occupiedRegions, factionCreator);
        }
        else
        {
            City newCity = Instantiate(city);
            newCity.transform.position = NodeManager.Instance.GetWorldPostitionFromTileNode(region.SetSettlement(newCity));
            newCity.SetName(CityNameGenerator.Instance.GetRandomCityName());
            Faction faction = factionCreator.CreateFaction(newCity);
            newCity.SetCityFlagColor(faction.GetFactionColor());
            region.SetOwner(faction);
            return region;
        }
    }
}

public class Region
{
    Faction owner;
    
    List<TileNode> tilesInRegion;
    List<RegionBorder> borders;
    public List<Region> neighbors;

    Settlement settlement;

    public Region()
    {
        tilesInRegion = new List<TileNode>();
        borders = new List<RegionBorder>();
    }

    public List<TileNode> GetTilesInRegion()
    {
        return tilesInRegion;
    }

    public void AddTileNode(TileNode newNode)
    {
        newNode.AssignRegion(this);
        tilesInRegion.Add(newNode);
    }

    public TileNode SetSettlement(Settlement settlement)
    {
        this.settlement = settlement;
        TileNode node = tilesInRegion[Random.Range(0, tilesInRegion.Count)];
        node.SetBuilding(settlement);
        return node;
    }

    public Settlement GetSettlement()
    {
        return settlement;
    }

    public void FindNeighboringRegions()
    {
        foreach(TileNode tileNode in tilesInRegion)
        {
            if (tileNode.IsBorderTile())
            {
                foreach(TileNode neighbor in tileNode.GetNodeNeighborData().GetNeighbors())
                {
                    Region neighboringRegion = neighbor.GetRegion();
                    if(neighboringRegion != null && neighboringRegion != this && !neighbors.Contains(neighboringRegion))
                    {
                        neighbors.Add(neighboringRegion);
                    }
                }
            }
        }
    }

    public void SetOwner(Faction owner)
    {
        this.owner = owner;
        foreach(RegionBorder border in borders)
        {
            border.ChangeBoderColor(owner.GetFactionColor());
        }
    }

    public void AddBorderToBorderList(RegionBorder regionBorder)
    {
        borders.Add(regionBorder);
        if (owner != null)
        {
            regionBorder.ChangeBoderColor(owner.GetFactionColor());
        }
    }
}
