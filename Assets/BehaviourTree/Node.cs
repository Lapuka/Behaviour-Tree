using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject
{
   
    public string nodeName;
    [HideInInspector] public State state = State.Running;
    [HideInInspector] public bool isStarted = false;

    [HideInInspector] public List<Node> children = new List<Node>();
    [HideInInspector] public int index;

    [HideInInspector] public Vector2 position;    
    [HideInInspector] public string GUID;
    [HideInInspector] public Blackboard blackboard;

    public enum State
    {
        Running,
        Success,
        Failure
    }
    
    public State Tick()
    {
        
        if (!isStarted)
        {
            OnStart();
            isStarted = true;
        }

        state = OnTick();

        if (state != State.Running)
        {
            OnStop();
            isStarted = false;
        }

        return state;
    }

    public abstract void OnStart();
    public abstract void OnStop();
    public abstract State OnTick();

    public virtual Node Clone()
    {
        Node clone = Instantiate(this);        
        clone.children = clone.children.ConvertAll(x => x.Clone());
        return clone;
    }

}
