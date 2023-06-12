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

    int numberOfPlayerFactions = 1;
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

    public void BuildRegionNeighborLists()
    {
        foreach(Region region in regions)
        {
            region.FindNeighboringRegions();
        }
    }

    public void PlaceSettlements()
    {
        PlaceCities();

        foreach(Region region in regions)
        {
            if (region.GetSettlement() == null)
            {
                Village newVillage = Instantiate(village);
                TileNode node = region.SetSettlement(newVillage);
                if (node == null)
                {
                    Destroy(newVillage.gameObject);
                }

                else
                {
                    newVillage.transform.position = NodeManager.Instance.GetWorldPostitionFromTileNode(node);
                    newVillage.SetName(CityNameGenerator.Instance.GetRandomCityName());
                }
            }
        }
    }

    private void PlaceCities()
    {
        List<List<Region>> regionClusters = BuildRegionClusters();
        FactionCreator factionCreator = new FactionCreator();
        foreach(List<Region> regionCluster in regionClusters)
        {
            PlaceCity(FindRegionWithinCluster(regionCluster, regionClusters), factionCreator);
        }
    }

    private List<List<Region>> BuildRegionClusters()
    {
        int regionClusterSize = regions.Count / numberOfCities;
        List<List<Region>> regionClusters = new List<List<Region>>();
        for(int i = 0; i < numberOfCities; i++)
        {
            regionClusters.Add(new List<Region>());
        }

        for(int i = 0; i < numberOfCities; i++)
        {
            regionClusters[i].Add(GetNextRegionNotInCluster(regionClusters));
            BuildRegionCluster(regionClusters[i], regionClusterSize, regionClusters);
        }

        return regionClusters;
    }

    private Region GetNextRegionNotInCluster(List<List<Region>> regionClusters)
    {
        foreach(Region region in regions)
        {
            //Debug.Log("Sent From Get Next Region");
            if (!CheckIfRegionIsInCluster(region, regionClusters))
            {
                return region;
            }
        }

        Debug.LogError("Problem Getting Region Clusters");
        return null;
    }

    private bool CheckIfRegionIsInCluster(Region region, List<List<Region>> regionClusters)
    {
        //Debug.Log("hi");
        bool regionAlredyInCluster = false;
        for (int i = 0; i < regionClusters.Count; i++)
        {
            if (regionClusters[i].Contains(region))
            {
                regionAlredyInCluster = true;
            }
        }
        //Debug.Log("Region in Cluster: " + regionAlredyInCluster);
        return regionAlredyInCluster;
    }

    private void BuildRegionCluster(List<Region> regionCluster, int regionClusterSize, List<List<Region>> regionClusters)
    {
        int regionsAdded = 0;
        Region region = regionCluster[regionCluster.Count - 1];
        foreach(Region neighbor in region.neighbors)
        {
            //Debug.Log("Sent From Build Region");
            if(!CheckIfRegionIsInCluster(neighbor, regionClusters))
            {
                regionCluster.Add(neighbor);
                regionsAdded++;
                if (regionCluster.Count >= regionClusterSize)
                {
                    //Debug.Log("Cluster At full size");
                    return;
                }
            }
        }
        if(regionsAdded == 0)
        {
            //Debug.Log("No Regions Added");
            return;
        }
        //Debug.Log("Function recurs");
        BuildRegionCluster(regionCluster, regionClusterSize, regionClusters);
    }

    private Region FindRegionWithinCluster(List<Region> regionCluster, List<List<Region>> regionClusters)
    {
        List<Region> regionsThatDoNotBorderOtherClusters = new List<Region>();

        foreach(Region region in regionCluster)
        {
            bool regionBordersAnotherCluster = false;

            foreach(Region neighbor in region.neighbors)
            {
                if (regionBordersAnotherCluster)
                {
                    continue;
                }

                foreach(List<Region> neighboringCluster in regionClusters)
                {
                    if(neighboringCluster == regionCluster)
                    {
                        continue;
                    }

                    if (neighboringCluster.Contains(neighbor))
                    {
                        regionBordersAnotherCluster = true;
                        continue;
                    }
                }
            }

            if (!regionBordersAnotherCluster)
            {
                regionsThatDoNotBorderOtherClusters.Add(region);
            }
        }

        if(regionsThatDoNotBorderOtherClusters.Count > 0)
        {
            return regionsThatDoNotBorderOtherClusters[Random.Range(0, regionsThatDoNotBorderOtherClusters.Count)];
        }
        else
        {
            return regionCluster[Random.Range(0, regionCluster.Count)];
        }
    }

    private void PlaceCity(Region region, FactionCreator factionCreator)
    {
        City newCity = Instantiate(city);
        newCity.transform.position = NodeManager.Instance.GetWorldPostitionFromTileNode(region.SetSettlement(newCity));
        newCity.SetName(CityNameGenerator.Instance.GetRandomCityName());
        Faction faction;
        if(numberOfPlayerFactions > 0)
        {
            faction = factionCreator.CreatePlayerFaction(newCity);
            numberOfPlayerFactions--;
        }
        else
        {
            faction = factionCreator.CreateAIFaction(newCity);
        }
        newCity.SetCityOwner(faction);
        region.SetOwner(faction);
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
        settlement.AssignHomeRegion(this);
        List<TileNode> nodes = new List<TileNode>();
        foreach (TileNode n in tilesInRegion)
        {
            NodeTerrainData terrainData = n.GetNodeTerrainData();
            if (terrainData.GetTerrainType() != TileNode.TerrainType.mountain)
            {
                nodes.Add(n);
            }
        }

        if (nodes.Count > 0)
        {
            TileNode node = nodes[Random.Range(0, nodes.Count)];
            node.SetBuilding(settlement);
            return node;
        }

        else
        {
            Debug.LogError("Region entirely mountains.");
            return null;
        }
    }

    public Settlement GetSettlement()
    {
        return settlement;
    }

    public void FindNeighboringRegions()
    {
        neighbors = new List<Region>();
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

    public Faction GetOwner()
    {
        return owner;
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
