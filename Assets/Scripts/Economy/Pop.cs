using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pop
{
    Settlement home;
    IEconomicBuilding job;

    public void AssignToHome(Settlement home)
    {
        this.home = home;
    }
    
    public IEconomicBuilding GetJob()
    {
        return job;
    }
    
    public void AssignToJob(IEconomicBuilding job)
    {
        this.job = job;
    }
}
