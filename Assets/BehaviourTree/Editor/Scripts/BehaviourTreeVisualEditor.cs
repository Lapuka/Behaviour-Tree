using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

public class BehaviourTreeVisualEditor : EditorWindow
{
    public NodeInspector inspector;
    public TreeGraphView graphView;
    public IMGUIContainer blackboardView;

    SerializedObject treeObject;
    SerializedProperty blackboard;

    [MenuItem("Behaviour Tree/Tree Editor")]
    public static void OpenWindow()
    {
        BehaviourTreeVisualEditor wnd = GetWindow<BehaviourTreeVisualEditor>();
        wnd.titleContent = new GUIContent("BehaviourTree Visual Editor");
    }

    public void CreateGUI()
    {

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;       

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BehaviourTree/Editor/BehaviourTreeVisualEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTree/Editor/BehaviourTreeVisualEditor.uss");      
        root.styleSheets.Add(styleSheet);

        inspector = root.Q<NodeInspector>();
        graphView = root.Q<TreeGraphView>();
        blackboardView = root.Q<IMGUIContainer>();

        blackboardView.onGUIHandler = () =>
        {
            if (treeObject != null && treeObject.targetObject != null)
            {
                treeObject.Update();
                EditorGUILayout.PropertyField(blackboard);
                treeObject.ApplyModifiedProperties();
            }
        };

        graphView.OnNodeSelected = OnNodeSelectionChanged;
        OnSelectionChange();
        
       
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
        }
    }
    private void OnSelectionChange()
    {
        //delay call so editor window gets updated before we call it
        EditorApplication.delayCall += () => {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;
            if (!tree)
            {
                if (Selection.activeGameObject && Application.isPlaying)
                {
                    BehaviourTreeManager runner = Selection.activeGameObject.GetComponent<BehaviourTreeManager>();
                    if (runner)
                    {
                        tree = runner.tree;                        
                    }
                }
            }

            if (tree != null)
            {
                graphView?.PopulateView(tree);
                RefreshBlackboardView(tree);
            }
            else
            {
                graphView?.ClearView();
                inspector?.Clear();
            }
           
        };
    }

    private void RefreshBlackboardView(BehaviourTree tree)
    {
        treeObject = new SerializedObject(tree);
        blackboard = treeObject.FindProperty("blackboard");
    }

    void OnNodeSelectionChanged(NodeView nodeView)
    {
        inspector.UpdateSelection(nodeView);
    }

    private void OnInspectorUpdate()
    {
       if(graphView != null) graphView.UpdateNodeState();       
    }
}