using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : TileEconomicBuilding
{
    Calendar calendar;
    int possibleHarvest = 0;
    int maxHarvestWorkers;

    public override void ActivateBuilding()
    {
        base.ActivateBuilding();
        calendar = FindObjectOfType<TurnManager>().GetCalendar();
        TakeAction();
    }

    public override void TakeAction()
    {
        Resource resource = CalculateResourseToAdd();
        if(resource.Quantity > 0)
        {
            treasury.AdjustResources(resource);
        }
        CalculateMaximumWorkers();
    }

    private void CalculateMaximumWorkers()
    {
        int month = calendar.GetMonth();
        if (month == 1)
        {
            maxWorkers = 5;
        }

        if (month == 2)
        {
            Plant();
            maxWorkers = 1;
            ReduceToMaximumWorkforce();

        }

        else if (month == 9)
        {
            MaintainHarvest();
            maxWorkers = maxHarvestWorkers;
        }

        else if (month == 10)
        {
            maxWorkers = 0;
            ReduceToMaximumWorkforce();
        }

        else if (month > 2 && month < 9)
        {
            if (possibleHarvest > 0)
            {
                maxWorkers = 1;
                MaintainHarvest();
            }
            else
            {
                maxWorkers = 0;
            }
        }
    }

    public override Resource CalculateResourseToAdd()
    {
        int month = calendar.GetMonth();

        if (month == 10)
        {
            Resource harvest = Harvest();
            
            return harvest;
        }

        

        return new Resource(Resource.ResourceType.Food, 0);
    }

    private void Plant()
    {
        maxHarvestWorkers = workers.Count;
        float workerRatio = (float)workers.Count / (float)maxWorkers;
        float harvest = GetHomeTile().GetNodeTerrainData().GetFertility() * workerRatio;
        possibleHarvest = (int)harvest;
    }

    private Resource Harvest()
    {
        if (maxWorkers > 0)
        {
            float workerRatio = (float)workers.Count / (float)maxWorkers;

            float harvest = possibleHarvest * workerRatio;
            int totalHarvest = (int)harvest;

            return new Resource(Resource.ResourceType.Food, totalHarvest);
        }

        else
        {
            return new Resource(Resource.ResourceType.Food, 0);
        }
    }

    private void ReduceToMaximumWorkforce()
    {
        RemoveWorker();
        if(workers.Count > maxWorkers)
        {
            ReduceToMaximumWorkforce();
        }
    }

    private void MaintainHarvest()
    {
        if(workers.Count < maxWorkers)
        {
            float harvest = (float)possibleHarvest * 0.2f;
            possibleHarvest -= (int)harvest;
        }
    }

    public override int GetPriority()
    {
        return 1;
    }

    public override TileNode GetValidLocationForBuilding(Region region)
    {
        TileNode nodeToPlace = null;
        List<TileNode> nodesWithHighestFertility = new List<TileNode>();
        foreach(TileNode node in region.GetTilesInRegion())
        {
            NodeTerrainData terrainData = node.GetNodeTerrainData();
            if(node.GetBuilding() == null && terrainData.GetTerrainType() != TileNode.TerrainType.mountain)
            {
                if(nodesWithHighestFertility.Count == 0)
                {
                    nodesWithHighestFertility.Add(node);
                }
                else
                {
                    if (nodesWithHighestFertility[0].GetNodeTerrainData().GetFertility() < terrainData.GetFertility())
                    {
                        nodesWithHighestFertility.Clear();
                        nodesWithHighestFertility.Add(node);
                    }
                    else if(nodesWithHighestFertility[0].GetNodeTerrainData().GetFertility() == terrainData.GetFertility())
                    {
                        nodesWithHighestFertility.Add(node);
                    }
                }
            }
        }

        if(nodesWithHighestFertility.Count > 0)
        {
            nodeToPlace = nodesWithHighestFertility[Random.Range(0, nodesWithHighestFertility.Count)];
        }

        return nodeToPlace;
    }

    public override List<Resource> CalculateUpkeep()
    {
        return null;
    }

    public override void SetUpConstructionCost(List<Resource> cost)
    {
        cost.Add(new Resource(Resource.ResourceType.Wood, 25));
        cost.Add(new Resource(Resource.ResourceType.Metal, 25));
    }
}
