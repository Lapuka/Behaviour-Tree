using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogMessage : ActionNode
{
    public string message;
    public override void OnStart()
    {
        
    }

    public override void OnStop()
    {
        
    }

    public override State OnTick()
    {
        
       
        Debug.Log(message);
        return State.Success;
    }
}
