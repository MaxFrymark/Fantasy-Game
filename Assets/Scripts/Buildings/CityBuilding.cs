using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CityBuilding : IEconomicBuilding
{
    Region home;
    Faction owner;
    City attachedCity;
    protected int constructionTime;
    protected int priority;
    List<Pop> workers;
    protected int maxWorkers;


    public CityBuilding(Faction owner, Region homeRegion)
    {
        this.owner = owner;
        AssignHomeRegion(homeRegion);
        AssignToSettlement(homeRegion.GetSettlement());
    }

    private void AssignHomeRegion(Region home)
    {
        this.home = home;
    }
    public Region GetHomeRegion()
    {
        return home;
    }

    public int GetConstructionTime()
    {
        return constructionTime;
    }

    public abstract void ActivateBuilding();

    List<Resource> GetConstructionCost()
    {
        List<Resource> cost = new List<Resource>();
        SetUpConstructionCost(cost);
        return cost;
    }

    protected abstract void SetUpConstructionCost(List<Resource> constructionCost);

    public abstract void TakeAction();
    public int GetPriority()
    {
        return priority;
    }

    public abstract List<Resource> CalculateUpkeep();

    public abstract Resource CalculateResourseToAdd();

    public void AddWorker()
    {
        if (workers.Count < maxWorkers)
        {
            Pop worker = attachedCity.GetIdleWorker();
            if (worker != null)
            {
                workers.Add(worker);
                worker.AssignToJob(this);
                //UpdateWorkerCount();
            }
        }
    }
    public void RemoveWorker()
    {
        if (workers.Count > 0)
        {
            workers[0].AssignToJob(null);
            workers.RemoveAt(0);
            //UpdateWorkerCount();
        }
    }

    public void SetToMaximumWorkers()
    {
        if (workers.Count < maxWorkers)
        {
            Pop worker = attachedCity.GetIdleWorker();
            if (worker != null)
            {
                workers.Add(worker);
                worker.AssignToJob(this);
                //UpdateWorkerCount();
                SetToMaximumWorkers();
            }
        }
    }
    public void AssignToSettlement(Settlement settlement)
    {
        attachedCity = settlement as City;
    } 
}
