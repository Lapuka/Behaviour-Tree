using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;
    public Node node;
    public Port input;
    public Port output;
    public NodeView(Node node) : base("Assets/BehaviourTree/Editor/NodeView.uxml")
    {
        this.node = node;
        if (node.nodeName == "" || node.nodeName == null)
        {
            this.title = node.name;            
        }
        else
        {
            this.title = node.nodeName;
           if(!Application.isPlaying) RenameNodeFile(node);

        }
        this.viewDataKey = node.GUID;

        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetType();
    }

    private void RenameNodeFile(Node node)
    {
        string assetPath = AssetDatabase.GetAssetPath(node.GetInstanceID());
        UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

        foreach (var item in assets)
        {
            if (item.GetInstanceID() == node.GetInstanceID())
            {
                if (item.name == node.nodeName) return;
                item.name = node.nodeName;
                EditorUtility.SetDirty(item);
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(assetPath);
    }
    //set classes so it can be accesses by UI Editor
    private void SetType()
    {
        if (node is RootNode)
        {
            AddToClassList("root");
        }
        else
        {
            if (node is ActionNode) AddToClassList("action");
            if (node is CompositeNode) AddToClassList("composite");
            if (node is DecoratorNode) AddToClassList("decorator");
        }
    }

    private void CreateInputPorts()
    {
        if (node is RootNode)
        {
            return;
        }

        if (node is Node)
        {
            input = new NodePort(Direction.Input, Port.Capacity.Single);
        }
            if (input != null)
        {
            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            input.portColor = new Color(0.25f, 0.61f, 0.62f);
            inputContainer.Add(input);
        }
    }
    private void CreateOutputPorts()
    {
        if (node is RootNode)
        {
            output = new NodePort(Direction.Output, Port.Capacity.Single);
        }

        if (node is CompositeNode)
            {
                output = new NodePort(Direction.Output, Port.Capacity.Multi);
        }

            if (node is DecoratorNode)
            {
                output = new NodePort(Direction.Output, Port.Capacity.Single);
        }

        

        if (output != null)
        {
            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            output.portColor = new Color(0.25f, 0.61f, 0.62f);
            outputContainer.Add(output);
        }

    }
    public override void OnSelected()
    {
        base.OnSelected();
        if (OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);       
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
        EditorUtility.SetDirty(node);
    }

    public void SortChildren()
    {
        node.children.Sort(SortByHorizontalPosition);
    }

    private int SortByHorizontalPosition(Node x, Node y)
    {
        return x.position.x < y.position.x ? -1 : 1;
    }
    //update classes so color can be changed in runtime depending on node state
    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");

        if (Application.isPlaying)
        {
            switch (node.state)
            {
                case Node.State.Running:
                    if (node.isStarted)
                    {
                        AddToClassList("running");
                    }
                    break;
                case Node.State.Failure:
                    AddToClassList("failure");
                    break;
                case Node.State.Success:
                    AddToClassList("success");
                    break;
            }
        }
    }
}
