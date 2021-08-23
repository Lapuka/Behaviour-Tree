using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeInspector : VisualElement
{
    public new class UxmlFactory : UxmlFactory<NodeInspector, VisualElement.UxmlTraits> { }

    Editor editor;   
    internal void UpdateSelection(NodeView nodeView)
    {        
        Clear();
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(nodeView.node);

        IMGUIContainer container = new IMGUIContainer(() => {
            if (editor.target)
            {
                editor.OnInspectorGUI();
            }
        });
        Add(container);
    }
}
