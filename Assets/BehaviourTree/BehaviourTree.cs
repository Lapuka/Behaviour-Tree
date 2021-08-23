using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public Node root;
    public Node.State treeState = Node.State.Running;
    public Blackboard blackboard = new Blackboard();
    public List<Node> nodes = new List<Node>();
    public Node.State Tick()
    {

        if (treeState == Node.State.Running)
        {
            treeState = root.Tick();
        }

        return treeState;
    }

    public static void Traverse(Node node, System.Action<Node> visiter)
    {
        if (node)
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visiter)); 
        }
    }
    public BehaviourTree Clone()
    {
        BehaviourTree clone = Instantiate(this);
        clone.root = clone.root.Clone();
        clone.nodes = new List<Node>();

        Traverse(clone.root, (n) => clone.nodes.Add(n));
        AddBlackboard(clone);
        return clone;
    }
    public Node CreateNode(System.Type type)
    {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.GUID = GUID.Generate().ToString();      

       
        nodes.Add(node);

       if(!Application.isPlaying) AssetDatabase.AddObjectToAsset(node, this);      

        AssetDatabase.SaveAssets(); 
        return node;
    }

    public static List<Node> GetChildren(Node parent)
    {
        return parent.children;
    }

    public void DeleteNode(Node node)
    {       
        nodes.Remove(node); 
        AssetDatabase.RemoveObjectFromAsset(node);
   
        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {       
          
            parent.children.Add(child);
            EditorUtility.SetDirty(parent);       

    }

    public void RemoveChild(Node parent, Node child)
    {       
          
            parent.children.Remove(child);
            EditorUtility.SetDirty(parent);
       
    }

    public void AddBlackboard(BehaviourTree tree)
    {
        Traverse(tree.root, (n) => n.blackboard = blackboard);
    }
}
