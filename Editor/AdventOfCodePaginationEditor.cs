using System;
using Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


[CustomEditor(typeof(AdventOfCodePagination))]
public class AdventOfCodePaginationEditor : UnityEditor.Editor
{
    [SerializeField] private VisualTreeAsset m_UXML = default;

    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        m_UXML.CloneTree(root);

        return root;
    }
}