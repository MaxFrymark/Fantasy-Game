using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IEconomicObject
{
    public void TakeAction();
    public int GetPriority();

    public List<Resource> CalculateUpkeep();

}

public interface IIncomeSource : IEconomicObject
{
    public Resource CalculateResourseToAdd();
}

public interface IEconomicBuilding : IIncomeSource
{
    public void AddWorker();
    public void RemoveWorker();

    public void SetToMaximumWorkers();
    public void AssignToSettlement(Settlement settlement);
}
