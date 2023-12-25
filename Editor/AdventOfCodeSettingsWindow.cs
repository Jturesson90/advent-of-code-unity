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
        private const string SelectedYearEditorPrefsKey = "selected-year-editor-prefs-key";
        private const string SelectedDayEditorPrefsKey = "selected-day-editor-prefs-key";
        [SerializeField] private VisualTreeAsset visualTreeAsset;

        private AdventOfCodeClient _adventOfCodeClient;

        private static int SelectedYear
        {
            get => EditorPrefs.GetInt(SelectedYearEditorPrefsKey, -1);
            set => EditorPrefs.SetInt(SelectedYearEditorPrefsKey, value);
        }

        private static int SelectedDay
        {
            get => EditorPrefs.GetInt(SelectedDayEditorPrefsKey, -1);
            set => EditorPrefs.SetInt(SelectedDayEditorPrefsKey, value);
        }

        private void OnEnable()
        {
            _adventOfCodeClient = new AdventOfCodeClient();
        }

        public void CreateGUI()
        {
            bool isLoggedIn = EditorPrefs.GetString(SessionEditorPrefsKey, string.Empty) != string.Empty;
            rootVisualElement.Clear();
            visualTreeAsset.CloneTree(rootVisualElement);
            var setupContainer = rootVisualElement.Q<VisualElement>("setup-container");

            var yearDropdown = rootVisualElement.Q<DropdownField>("year-dropdown");
            var years = GetYears();
            int indexOfYear = years.IndexOf(SelectedYear);
            yearDropdown.choices = years.Select(a => a.ToString()).ToList();
            yearDropdown.index = indexOfYear;
            yearDropdown.RegisterCallback<ChangeEvent<string>>(evt => { SelectedYear = int.Parse(evt.newValue); });

            var days = GetDays();
            var dayDropdown = rootVisualElement.Q<DropdownField>("day-dropdown");
            int indexOfDay = days.IndexOf(SelectedDay);
            dayDropdown.choices = days.Select(a => a.ToString()).ToList();
            dayDropdown.index = indexOfDay;
            dayDropdown.RegisterCallback<ChangeEvent<string>>(evt => { SelectedDay = int.Parse(evt.newValue); });

            var button = rootVisualElement.Q<Button>("setup-button");
            button.clickable.clicked += () =>
            {
                SetupDay(EditorPrefs.GetString(SessionEditorPrefsKey, string.Empty), SelectedYear, SelectedDay,
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
                    errorLabel.text = "";
                    isLoggedIn = false;
                    EditorPrefs.DeleteKey(SessionEditorPrefsKey);
                    setupContainer.SetEnabled(isLoggedIn);
                }
                else
                {
                    errorLabel.text = "Loading";
                    string textfieldValue = sessionTextField.value;
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
            const string assetsFolder = "Assets";
            const string resourceFolder = "Resources";
            const string adventOfCodeFolder = "AdventOfCode";
            var yearFolder = $"{year}";
            var dayFileName = $"{day.ToString().PadLeft(2, '0')}.txt";
            if (!AssetDatabase.IsValidFolder(Path.Combine(assetsFolder, resourceFolder)))
                AssetDatabase.CreateFolder("Assets", resourceFolder);

            if (!AssetDatabase.IsValidFolder(Path.Combine(assetsFolder, resourceFolder, adventOfCodeFolder)))
                AssetDatabase.CreateFolder(Path.Combine(assetsFolder, resourceFolder), adventOfCodeFolder);
            string input = await _adventOfCodeClient.LoadDayInput(session, year, day);
            if (string.IsNullOrEmpty(input))
            {
                action(false);
                return;
            }

            if (!AssetDatabase.IsValidFolder(Path.Combine(assetsFolder, resourceFolder, adventOfCodeFolder,
                    yearFolder)))
                AssetDatabase.CreateFolder(Path.Combine(assetsFolder, resourceFolder, adventOfCodeFolder), yearFolder);

            string fullPath = Path.Combine(assetsFolder, resourceFolder, adventOfCodeFolder, yearFolder, dayFileName);
            await File.WriteAllTextAsync(fullPath, input.Trim());
            AssetDatabase.Refresh();
            CreateYear(year, day);
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
            int currentYear = DateTime.Now.Year;
            var years = new List<int>();
            for (int year = currentYear; year >= 2015; year--) years.Add(year);

            return years;
        }

        private async void SessionOk(string session, Action<bool> callback)
        {
            bool b = await _adventOfCodeClient.SessionIsValid(session);
            callback(b);
        }

        [MenuItem("Window/Advent of Code/Settings")]
        public static void ShowExample()
        {
            var wnd = GetWindow<AdventOfCodeSettingsWindow>();
            wnd.titleContent = new GUIContent("Advent of Code Settings");
        }

        public static void CreateYear(int yearDropdownValue2, int day2)
        {
            var yearString = yearDropdownValue2.ToString();
            string dayString = day2.ToString().PadLeft(2, '0');
            string assetsFolder = Path.Combine("Assets");
            string aocFolder = Path.Combine(assetsFolder, "AdventOfCode");
            string yearFolder = Path.Combine(aocFolder, yearString);
            string codeFolder = Path.Combine(yearFolder, "Code");
            string testsFolder = Path.Combine(yearFolder, "Tests");
            string dayFile = Path.Combine(codeFolder, $"Day{dayString}.cs");
            string dayTestFile = Path.Combine(testsFolder, $"Day{dayString}Tests.cs");

            var a = AssetDatabase.LoadAssetAtPath<TextAsset>(dayFile);
            if (a != null)
            {
                Debug.Log($"{dayFile} already exists. No need of creating new one");
                return;
            }

            if (!AssetDatabase.IsValidFolder(aocFolder))
            {
                Debug.Log("Creating " + aocFolder);
                AssetDatabase.CreateFolder(assetsFolder, "AdventOfCode");
            }

            if (!AssetDatabase.IsValidFolder(yearFolder))
            {
                Debug.Log("Creating " + yearFolder);
                AssetDatabase.CreateFolder(aocFolder, yearString);
            }

            if (!AssetDatabase.IsValidFolder(codeFolder))
            {
                Debug.Log("Create " + codeFolder);
                // Create asmdef
                AssetDatabase.CreateFolder(yearFolder, "Code");
                string asmdef = Path.Combine(codeFolder, $"AdventOfCode.{yearString}.asmdef");
                using var streamWriter = new StreamWriter(asmdef);
                streamWriter.WriteLine("{");
                streamWriter.WriteLine($"\t\"name\": \"AdventOfCode.{yearString}\",");
                streamWriter.WriteLine("\t\"references\": [");
                streamWriter.WriteLine("\t\t\"JTuresson.AdventOfCode\"");
                streamWriter.WriteLine("\t]");

                streamWriter.WriteLine("}");
                streamWriter.Close();
                AssetDatabase.Refresh();
            }

            if (!AssetDatabase.IsValidFolder(testsFolder))
            {
                Debug.Log("Create " + testsFolder);
                // Create asmdef
                AssetDatabase.CreateFolder(yearFolder, "Tests");
                string asmdef = Path.Combine(testsFolder, $"AdventOfCode.{yearString}.Tests.asmdef");
                using var streamWriter = new StreamWriter(asmdef);
                streamWriter.WriteLine("{");
                streamWriter.WriteLine($"\t\"name\": \"AdventOfCode.{yearString}.Tests\",");

                streamWriter.WriteLine("\t\"references\": [");
                streamWriter.WriteLine($"\t\t\"AdventOfCode.{yearString}\"");
                streamWriter.WriteLine("\t],");

                streamWriter.WriteLine("\t\"optionalUnityReferences\": [");
                streamWriter.WriteLine("\t\t\"TestAssemblies\"");
                streamWriter.WriteLine("\t],");
                streamWriter.WriteLine("\t\"includePlatforms\": [");
                streamWriter.WriteLine("\t\t\"Editor\"");
                streamWriter.WriteLine("\t]");
                streamWriter.WriteLine("}");
                AssetDatabase.Refresh();
            }

            if (AssetDatabase.LoadAssetAtPath<TextAsset>(dayFile) == null)
            {
                using var streamWriter = new StreamWriter(dayFile);
                streamWriter.WriteLine("using JTuresson.AdventOfCode;");
                streamWriter.WriteLine();
                streamWriter.WriteLine($"namespace AdventOfCode_{yearString}");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine($"\tpublic static class Day{dayString}");
                streamWriter.WriteLine("\t{");
                streamWriter.WriteLine("\t\tpublic static string PuzzleA(string input)");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine("\t\t\t//Solve puzzle A!");
                streamWriter.WriteLine("\t\t\treturn input;");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine("\t\tpublic static string PuzzleB(string input)");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine("\t\t\t//Solve puzzle B!");
                streamWriter.WriteLine("\t\t\treturn input;");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine("\t}");
                streamWriter.WriteLine("}");
                AssetDatabase.Refresh();
            }

            if (AssetDatabase.LoadAssetAtPath<TextAsset>(dayTestFile) == null)
            {
                using var streamWriter = new StreamWriter(dayTestFile);
                streamWriter.WriteLine("using System;");
                streamWriter.WriteLine("using System.Collections;");
                streamWriter.WriteLine("using System.Collections.Generic;");
                streamWriter.WriteLine("using NUnit.Framework;");
                streamWriter.WriteLine("using UnityEngine;");
                streamWriter.WriteLine("using UnityEngine.TestTools;");
                streamWriter.WriteLine($"using AdventOfCode_{yearString};");
                streamWriter.WriteLine("");
                streamWriter.WriteLine($"namespace AdventOfCode_{yearString}Tests");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("\t[TestFixture]");
                streamWriter.WriteLine($"\tpublic class Day{dayString}Tests");
                streamWriter.WriteLine("\t{");
                streamWriter.WriteLine("\t\tprivate string _input;");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("\t\t[SetUp]");
                streamWriter.WriteLine("\t\tpublic void Setup()");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine(
                    $"\t\t\t_input = Resources.Load<TextAsset>(\"AdventOfCode/{yearString}/{dayString}\").text;");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("\t\t[Test]");
                streamWriter.WriteLine("\t\tpublic void PuzzleATests()");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\tconst string expectedResult = \"\";");
                streamWriter.WriteLine($"\t\t\tstring result = Day{dayString}.PuzzleA(_input);");
                streamWriter.WriteLine("\t\t\tAssert.AreEqual(expectedResult, result);");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("\t\t[TestCase(\"input\",\"expectedResult\")]");
                streamWriter.WriteLine("\t\tpublic void PuzzleATests(string input, string expectedResult)");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\tstring result = Day{dayString}.PuzzleA(input);");
                streamWriter.WriteLine("\t\t\tAssert.AreEqual(expectedResult, result);");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("\t\t[Test]");
                streamWriter.WriteLine("\t\tpublic void PuzzleBTests()");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\tconst string expectedResult = \"\";");
                streamWriter.WriteLine($"\t\t\tstring result = Day{dayString}.PuzzleB(_input);");
                streamWriter.WriteLine("\t\t\tAssert.AreEqual(expectedResult, result);");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("\t\t[TestCase(\"input\",\"expectedResult\")]");
                streamWriter.WriteLine("\t\tpublic void PuzzleBTests(string input, string expectedResult)");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\tstring result = Day{dayString}.PuzzleB(input);");
                streamWriter.WriteLine("\t\t\tAssert.AreEqual(expectedResult, result);");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine("\t}");
                streamWriter.WriteLine("}");
                AssetDatabase.Refresh();
            }
        }
    }
}