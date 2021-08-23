using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNode : Node
{
    public bool newBlackboardEveryTime = true;
    public override void OnStart()
    {
        
    }

    public override void OnStop()
    {
        if (newBlackboardEveryTime)
        {
            blackboard.Reset();
        }
    }

    public override State OnTick()
    {
        if (children.Count == 0) return State.Failure; 
        return children[0].Tick();
    }
}
