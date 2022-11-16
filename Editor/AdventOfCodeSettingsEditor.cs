using JTuresson.AdventOfCode.AOCClient;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(AdventOfCodeSettings))]
    public class AdventOfCodeSettingsEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset m_UXML;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            m_UXML.CloneTree(root);
            var uxmlButton = root.Q<Button>("AdventClearCache");
            uxmlButton.RegisterCallback<MouseUpEvent>((evt) =>
                ((AdventOfCodeSettings) target).ClearCache());
            return root;
        }
    }
}