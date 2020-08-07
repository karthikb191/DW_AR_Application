using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPanel : UserPanel
{
    public SimplePanel floor1;
    public SimplePanel floor2;
    public LocationPoint[] locationPoints;
    LocationPoint previousActiveLocationPoint;
    LocationPoint activeLocationPoint;

    protected override void Start()
    {
        base.Start();
    }
    public void ActivateLocation(int index)
    {
        if (previousActiveLocationPoint)
            previousActiveLocationPoint.Deactivate();

        if(index < locationPoints.Length)
            locationPoints[index].Activate();
    }

}
