using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using JTuresson.AdventOfCode.AOCClient;

namespace Editor
{
    public class AdventOfCodeSettingsWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private AdventOfCodeSettings settings;
        [SerializeField] private AdventOfCodePagination pagination;

        [MenuItem("Window/Advent of Code/Settings")]
        public static void ShowExample()
        {
            var wnd = GetWindow<AdventOfCodeSettingsWindow>();
            wnd.titleContent = new GUIContent("Advent of Code Settings Window");
        }

        private void OnProjectChange()
        {
            settings = AdventOfCodeSettings.Instance;
        }

        private void OnEnable()
        {
            settings = AdventOfCodeSettings.Instance;
            pagination = AdventOfCodePagination.instance;
            AdventOfCodeSettings.ExistChanged += OnAdventOfCodeSettingsExistChanged;
        }

        private void OnFocus()
        {
            settings = AdventOfCodeSettings.Instance;
            pagination = AdventOfCodePagination.instance;
            CreateGUI();
        }


        private void OnDisable()
        {
            AdventOfCodeSettings.ExistChanged -= OnAdventOfCodeSettingsExistChanged;
        }

        private void OnAdventOfCodeSettingsExistChanged()
        {
            settings = AdventOfCodeSettings.Instance;
            pagination = AdventOfCodePagination.instance;
            CreateGUI();
        }

        public void CreateGUI()
        {
            rootVisualElement.Clear();
            visualTreeAsset.CloneTree(rootVisualElement);
            var body = rootVisualElement.Q("body");
            var scrollView = new ScrollView() {viewDataKey = "AdventOfCodeSettingsScrollView"};
            if (settings == null)
            {
                VisualElement button = new Button(AdventOfCodeSettings.CreateAsset)
                {
                    text = "Create settings"
                };

                scrollView.Add(button);
            }
            else
            {
                scrollView.Add(new InspectorElement(settings));
                if (pagination)
                    scrollView.Add(new InspectorElement(pagination));
            }

            body.Add(scrollView);
        }
    }
}