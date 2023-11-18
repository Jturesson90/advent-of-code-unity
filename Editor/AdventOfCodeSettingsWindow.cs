using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JTuresson.AdventOfCode.AOCClient;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace JTuresson.AdventOfCode.Editor
{
    public class AdventOfCodeSettingsWindow : EditorWindow
    {
        private const string SessionEditorPrefsKey = "session-editor-prefs-key";
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private int selectedYear = -1;
        [SerializeField] private int selectedDay = -1;
        private AdventOfCodeClient _adventOfCodeClient;

        private void OnEnable()
        {
            _adventOfCodeClient = new AdventOfCodeClient();
        }

        public void CreateGUI()
        {
            var isLoggedIn = EditorPrefs.GetString(SessionEditorPrefsKey, string.Empty) != string.Empty;
            rootVisualElement.Clear();
            visualTreeAsset.CloneTree(rootVisualElement);

            var setupContainer = rootVisualElement.Q<VisualElement>("setup-container");

            var yearDropdown = rootVisualElement.Q<DropdownField>("year-dropdown");
            var years = GetYears();
            var indexOfYear = years.IndexOf(selectedYear);
            yearDropdown.choices = years.Select(a => a.ToString()).ToList();
            yearDropdown.index = indexOfYear;
            yearDropdown.RegisterCallback<ChangeEvent<string>>(evt => { selectedYear = int.Parse(evt.newValue); });

            var days = GetDays();
            var dayDropdown = rootVisualElement.Q<DropdownField>("day-dropdown");
            var indexOfDay = days.IndexOf(selectedDay);
            dayDropdown.choices = days.Select(a => a.ToString()).ToList();
            dayDropdown.index = indexOfDay;
            dayDropdown.RegisterCallback<ChangeEvent<string>>(evt => { selectedDay = int.Parse(evt.newValue); });

            var button = rootVisualElement.Q<Button>("setup-button");
            button.clickable.clicked += () =>
            {
                SetupDay(EditorPrefs.GetString(SessionEditorPrefsKey, string.Empty), selectedYear, selectedDay,
                    b => { });
            };

            setupContainer.SetEnabled(isLoggedIn);
            var errorLabel = rootVisualElement.Q<Label>("error-label");
            errorLabel.text = "";
            var sessionTextField = rootVisualElement.Q<TextField>("session-textfield");
            sessionTextField.value = EditorPrefs.GetString(SessionEditorPrefsKey, string.Empty);
            var loginButton = rootVisualElement.Q<Button>("login-button");
            loginButton.text = isLoggedIn ? "Log out" : "Log in";
            loginButton.clickable.clicked += () =>
            {
                if (errorLabel.text == "Loading...") return;
                if (isLoggedIn)
                {
                    loginButton.text = "Log in";
                    isLoggedIn = false;
                    EditorPrefs.DeleteKey(SessionEditorPrefsKey);
                    setupContainer.SetEnabled(isLoggedIn);
                }
                else
                {
                    errorLabel.text = "Loading";
                    var textfieldValue = sessionTextField.value;
                    SessionOk(sessionTextField.value, b =>
                    {
                        if (b)
                        {
                            errorLabel.text = "Success!";
                            loginButton.text = "Log out";
                            isLoggedIn = true;
                            EditorPrefs.SetString(SessionEditorPrefsKey, textfieldValue);
                            setupContainer.SetEnabled(isLoggedIn);
                        }
                        else
                        {
                            errorLabel.text = "Could not login, try again";
                        }
                    });
                }
            };
        }

        private async void SetupDay(string session, int year, int day, Action<bool> action)
        {
            var t = Resources.Load<TextAsset>($"AdventOfCode/{year}/{day}.asset");
            const string assetsFolder = "Assets";
            const string resourceFolder = "Resources";
            const string adventOfCodeFolder = "AdventOfCode";
            var yearFolder = $"{year}";
            var dayFileName = $"{day}.txt";
            if (!AssetDatabase.IsValidFolder(Path.Combine(assetsFolder, resourceFolder)))
                AssetDatabase.CreateFolder("Assets", resourceFolder);

            if (!AssetDatabase.IsValidFolder(Path.Combine(assetsFolder, resourceFolder, adventOfCodeFolder)))
                AssetDatabase.CreateFolder(Path.Combine(assetsFolder, resourceFolder), adventOfCodeFolder);
            var input = await _adventOfCodeClient.LoadDayInput(session, year, day);
            if (string.IsNullOrEmpty(input))
            {
                action(false);
                return;
            }

            if (!AssetDatabase.IsValidFolder(Path.Combine(assetsFolder, resourceFolder, adventOfCodeFolder,
                    yearFolder)))
                AssetDatabase.CreateFolder(Path.Combine(assetsFolder, resourceFolder, adventOfCodeFolder), yearFolder);

            Debug.Log(input);
            var fullPath = Path.Combine(assetsFolder, resourceFolder, adventOfCodeFolder, yearFolder, dayFileName);
            await File.WriteAllTextAsync(fullPath, input.Trim());
            AssetDatabase.Refresh();
            action(true);
        }

        private static List<int> GetDays()
        {
            var days = new List<int>();
            for (var day = 25; day >= 1; day--) days.Add(day);

            return days;
        }

        private static List<int> GetYears()
        {
            var currentYear = DateTime.Now.Year;
            var years = new List<int>();
            for (var year = currentYear; year >= 2015; year--) years.Add(year);

            return years;
        }

        private async void SessionOk(string session, Action<bool> callback)
        {
            var b = await _adventOfCodeClient.SessionIsValid(session);
            callback(b);
        }

        [MenuItem("Window/Advent of Code/Settings")]
        public static void ShowExample()
        {
            var wnd = GetWindow<AdventOfCodeSettingsWindow>();
            wnd.titleContent = new GUIContent("Advent of Code Settings Window");
        }

        public static void CreateYear(string yearDropdownValue)
        {
            if (!AssetDatabase.IsValidFolder("Assets" + "/" + yearDropdownValue))
            {
                AssetDatabase.CreateFolder("Assets/AdventOfCode", yearDropdownValue);
                var yearPath = "Assets/AdventOfCode/" + yearDropdownValue;
                AssetDatabase.CreateFolder(yearPath, "Code");
                AssetDatabase.CreateFolder(yearPath, "Inputs");
                AssetDatabase.CreateFolder(yearPath, "Tests");
                var codeFolderPath = "Assets/AdventOfCode/" + yearDropdownValue + "/Code";
                for (var i = 1; i <= 25; i++)
                {
                    var name = $"Day{i}";
                    var fileName = $"{name}.cs";
                    var outfile = codeFolderPath + "/" + fileName;
                    using var sw = new StreamWriter(outfile, false);
                    var s =
                        "namespace AdventOfCode" + yearDropdownValue +
                        "\n{" +
                        "\n\tpublic class " + name +
                        "\n\t{" +
                        "\n\t\tpublic string PuzzleA(string input)" +
                        "\n\t\t{" +
                        "\n\t\t\t return input;" +
                        "\n\t\t}" +
                        "\n\t\tpublic string PuzzleB(string input)" +
                        "\n\t\t{" +
                        "\n\t\t\t return input;" +
                        "\n\t\t}" +
                        "\n\t}" +
                        "\n}";

                    sw.Write(s);
                }

                AssetDatabase.Refresh();
            }
        }
    }
}