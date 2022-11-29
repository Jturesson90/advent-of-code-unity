using System;
using Editor;
using JTuresson.AdventOfCode.AOCClient;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;


[CustomEditor(typeof(AdventOfCodePagination))]
public class AdventOfCodePaginationEditor : UnityEditor.Editor
{
    [SerializeField] private VisualTreeAsset m_UXML = default;

    private AdventOfCodePagination _pagination;

    private void OnEnable()
    {
        _pagination = AdventOfCodePagination.instance;
    }

    private void UpdateText(string text, VisualElement parent)
    {
        if (text == null) return;
        int threshold = 500;
        parent.Clear();
        if (!text.Contains("\n"))
        {
            return;
        }

        var a = text.Split("\n");
        foreach (var n in a)
        {
            var t = new TextElement
            {
                text = n,
                enableRichText = false,
                delegatesFocus = false
            };
            parent.Add(t);
        }
    }

    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        m_UXML.CloneTree(root);

        var descriptionButton = root.Q<Button>("DescriptionButton");
        var inputButtonButton = root.Q<Button>("InputButton");
        var textContainer = root.Q<VisualElement>("TextContainer");
        var loadDayInput = root.Q<IntegerField>("LoadDayInput");
        var copyNotice = root.Q<Label>("CopyNotice");
        loadDayInput.RegisterCallback<ChangeEvent<int>>(
            x => _pagination.day = x.newValue
        );
        loadDayInput.value = _pagination.day;
        descriptionButton.RegisterCallback<MouseUpEvent>(async (evt) =>
            {
                descriptionButton.SetEnabled(false);
                inputButtonButton.SetEnabled(false);
                copyNotice.visible = false;
                _pagination.paginationState = AdventOfCodePagination.PaginationStateEnum.ShowDescription;
                var client = new AdventOfCodeClient(
                    AdventOfCodeSettings.Instance, AdventOfCodeSettings.Instance.GetCache());
                _pagination.description = await client.LoadDescription(_pagination.day);
                if (_pagination.input != null)
                {
                    UpdateText(_pagination.description, textContainer);
                }

                descriptionButton.SetEnabled(true);
                inputButtonButton.SetEnabled(true);
            }
        );
        inputButtonButton.RegisterCallback<MouseUpEvent>(async (evt) =>
            {
                descriptionButton.SetEnabled(false);
                inputButtonButton.SetEnabled(false);
                copyNotice.visible = false;
                _pagination.paginationState = AdventOfCodePagination.PaginationStateEnum.ShowInput;
                var client = new AdventOfCodeClient(
                    AdventOfCodeSettings.Instance, AdventOfCodeSettings.Instance.GetCache());
                _pagination.input = await client.LoadDayInput(_pagination.day);

                if (_pagination.input != null)
                {
                    copyNotice.visible = true;
                    Debug.Log(_pagination.input);
                    GUIUtility.systemCopyBuffer = _pagination.input;
                    UpdateText(_pagination.input, textContainer);
                }

                descriptionButton.SetEnabled(true);
                inputButtonButton.SetEnabled(true);
            }
        );
        switch (_pagination.paginationState)
        {
            case AdventOfCodePagination.PaginationStateEnum.ShowDescription:
                UpdateText(_pagination.description, textContainer);
                break;
            case AdventOfCodePagination.PaginationStateEnum.ShowInput:
                UpdateText(_pagination.input, textContainer);
                break;
            case AdventOfCodePagination.PaginationStateEnum.Non:
                UpdateText(string.Empty, textContainer);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return root;
    }
}