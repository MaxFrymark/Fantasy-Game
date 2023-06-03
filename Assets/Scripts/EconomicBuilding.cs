using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEconomicBuilding
{
    public void AddWorker();
    public void RemoveWorker();
    public void AssignToSettlement(Settlement settlement);
    public void TakeAction();
}
