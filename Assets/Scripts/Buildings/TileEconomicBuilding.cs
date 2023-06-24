using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class TileEconomicBuilding : TileBuilding, IEconomicBuilding
{
    Settlement attachedSettlement;
    protected List<Pop> workers = new List<Pop>();
    [SerializeField] protected int maxWorkers;
    [SerializeField] TextMeshPro workerCounter;
    [SerializeField] GameObject workerManagement;

    protected Treasury treasury;

    public void AddWorker()
    {
        if(workers.Count < maxWorkers)
        {
            Pop worker = attachedSettlement.GetIdleWorker();
            if(worker != null)
            {
                workers.Add(worker);
                worker.AssignToJob(this);
                UpdateWorkerCount();
            }
        }
    }

    public void RemoveWorker()
    {
        if(workers.Count > 0)
        {
            workers[0].AssignToJob(null);
            workers.RemoveAt(0);
            UpdateWorkerCount();
        }
    }

    public void SetToMaximumWorkers()
    {
        if(workers.Count < maxWorkers)
        {
            Pop worker = attachedSettlement.GetIdleWorker();
            if(worker != null)
            {
                workers.Add(worker);
                worker.AssignToJob(this);
                UpdateWorkerCount();
                SetToMaximumWorkers();
            }
        }
    }

    private void UpdateWorkerCount()
    {
        workerCounter.text = workers.Count + "/" + maxWorkers;
        attachedSettlement.UpdateSettlementManagerUI();
    }

    public void AssignToSettlement(Settlement settlement)
    {
        settlement.AddBuilding(this);
        attachedSettlement = settlement;
        treasury = attachedSettlement.GetTreasury();
    }

    public override void ActivateBuilding()
    {
        base.ActivateBuilding();
        AssignToSettlement(GetHomeRegion().GetSettlement());
    }

    public void HandleWorkerManagement(bool active)
    {
        workerManagement.SetActive(active);
        if (active)
        {
            UpdateWorkerCount();
        }
    }


    public abstract int GetPriority();

    public abstract void TakeAction();

    public abstract List<Resource> CalculateUpkeep();
    public abstract Resource CalculateResourseToAdd();
}
