using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Blackboard 
{
    public string message;

    public void Reset()
    {
        message = "";
    }
}
