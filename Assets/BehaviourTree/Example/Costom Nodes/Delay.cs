using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delay : DecoratorNode
{
    public float delay;
    private float startTime;
    public override void OnStart()
    {
        startTime = Time.time;
    }

    public override void OnStop()
    {
       
    }

    public override State OnTick()
    {        
        if(Time.time - startTime < delay)
        {
            return State.Running;
        }

        
        return children[0].Tick();
    }
}
