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
    }

    public override void TakeAction()
    {
        Resource resource = CalculateResourseToAdd();
        if(resource.Quantity > 0)
        {
            treasury.AdjustResources(resource);
        }
    }

    public override Resource CalculateResourseToAdd()
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
            Resource harvest = Harvest();
            maxWorkers = 0;
            ReduceToMaximumWorkforce();
            return harvest;
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
}
