using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class TreeGraphView : GraphView
{
    BehaviourTree tree;
    public Action<NodeView> OnNodeSelected;
    public new class UxmlFactory : UxmlFactory<TreeGraphView, GraphView.UxmlTraits> { }

    public TreeGraphView()
    {
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        Insert(0, new GridBackground());
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTree/Editor/BehaviourTreeVisualEditor.uss");
        styleSheets.Add(styleSheet);
        
    }  
    public void ClearView()
    {
        // remove listener so it doesn't trigger deletion of actual scriptable objects
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements.ToList());
        graphViewChanged += OnGraphViewChanged;

    }
    public void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;
        ClearView();

        //nodes in the graph
        tree.nodes.ForEach(n => CreateNodeView(n));

        //edges in the graph
        tree.nodes.ForEach(n => {
            var children = BehaviourTree.GetChildren(n);
            children.ForEach(c =>
            {
                NodeView parentView = FindNodeView(n);
                NodeView childView = FindNodeView(c);

                Edge edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);

            });
        });

       
    }

    NodeView FindNodeView(Node node)
    {
        return GetNodeByGuid(node.GUID) as NodeView;
    }
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem => {

                NodeView nodeView = elem as NodeView;
                if (nodeView != null)
                {
                    tree.DeleteNode(nodeView.node);
                }

                Edge edge = elem as Edge;
                if (edge != null)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    tree.RemoveChild(parentView.node, childView.node);
                }
            });
        }

        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                tree.AddChild(parentView.node, childView.node);

            });
        }

        //sort by position left to right
        nodes.ForEach((n) => {
            NodeView view = n as NodeView;
            view.SortChildren();
        });

        return graphViewChange;
    }

   
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);
        if (tree.nodes.Count == 0)
        {
            evt.menu.AppendAction($"Root Node", (a) => CreateRootNode());
        }
        else
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"Action Nodes/{type.Name}", (a) => CreateNode(type));
            }
            types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"Composite Nodes/{type.Name}", (a) => CreateNode(type));
            }
            types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"Decorator Nodes/{type.Name}", (a) => CreateNode(type));
            }
            evt.menu.AppendAction($"Refresh", (a) => PopulateView(tree));
        }
    }

    private void CreateRootNode()
    {       
        Node r = tree.CreateNode(typeof(RootNode));        
        tree.root = r;        
        CreateNodeView(r);
    }
    private void CreateNode(System.Type type)
    {
        Node node = tree.CreateNode(type);       
        CreateNodeView(node);
    }
    void CreateNodeView(Node node)
    {
        NodeView nodeView = new NodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }

    public void UpdateNodeState()
    {
        nodes.ForEach(node => {
            NodeView view = node as NodeView;
            view.UpdateState();
        });
    }
}
