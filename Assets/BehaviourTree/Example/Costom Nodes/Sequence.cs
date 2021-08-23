using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : CompositeNode
{
    public SequenceType sequenceType = SequenceType.AllSuccess;
    public enum SequenceType
    {
        AllSuccess,
        AllFailure,
        FirstSuccess,
        FirstFailure,
        AllRegardless
    }
    public override void OnStart()
    {       
        index = 0;
    }

    public override void OnStop()
    {
        
    }

    public override State OnTick()
    {
        switch (sequenceType)
        {
            case SequenceType.AllSuccess:
                return AllSuccess();
            case SequenceType.AllFailure:
                return AllFailure();
            case SequenceType.FirstSuccess:
                return FirstSuccess();
            case SequenceType.FirstFailure:
                return FirstFailure();
            case SequenceType.AllRegardless:
                return AllRegardless();
        }
        return State.Failure;
    }

    private State AllSuccess()
    {
        switch (children[index].Tick())
        {
            case State.Running:
                return State.Running;
            case State.Success:
                index++;
                break;
            case State.Failure:
                return State.Failure;
        }

        return index < children.Count ? State.Running : State.Success;
    }

    private State AllFailure()
    {
        switch (children[index].Tick())
        {
            case State.Running:
                return State.Running;
            case State.Success:
                return State.Failure;
            case State.Failure:
                index++;
                break;
        }

        return index < children.Count ? State.Running : State.Success;
    }

    private State FirstFailure()
    {
        switch (children[index].Tick())
        {
            case State.Running:
                return State.Running;
            case State.Success:
                index++;
                break;
            case State.Failure:
                return State.Success;
        }

        return index < children.Count ? State.Running : State.Failure;
    }

    private State FirstSuccess()
    {
        switch (children[index].Tick())
        {
            case State.Running:
                return State.Running;
            case State.Success:
                return State.Success;
            case State.Failure:
                index++;
                break;
        }

        return index < children.Count ? State.Running : State.Failure;
    }

    private State AllRegardless()
    {
        children[index].Tick();
        index++;

        return index < children.Count ? State.Running : State.Success;
    }
}
