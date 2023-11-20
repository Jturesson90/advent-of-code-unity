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
        //    yearDropdown.label = "Year";
            var years = GetYears();
            var indexOfYear = years.IndexOf(selectedYear);
            yearDropdown.choices = years.Select(a => a.ToString()).ToList();
            yearDropdown.index = indexOfYear;
            yearDropdown.RegisterCallback<ChangeEvent<string>>(evt => { selectedYear = int.Parse(evt.newValue); });

            var days = GetDays();
            var dayDropdown = rootVisualElement.Q<DropdownField>("day-dropdown");
//            yearDropdown.label = "Day";
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

            var fullPath = Path.Combine(assetsFolder, resourceFolder, adventOfCodeFolder, yearFolder, dayFileName);
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
            wnd.titleContent = new GUIContent("Advent of Code Settings");
        }

        public static void CreateYear(int yearDropdownValue, int day)
        {
            var assetsFolder = Path.Combine("Assets");
            var aocFolder = Path.Combine(assetsFolder, "AdventOfCode");
            var yearFolder = Path.Combine(aocFolder, yearDropdownValue.ToString());
            var codeFolder = Path.Combine(yearFolder, "Code");
            var testsFolder = Path.Combine(yearFolder, "Tests");
            var dayFile = Path.Combine(codeFolder, $"Day{day}.cs");
            var dayTestFile = Path.Combine(testsFolder, $"Day{day}Tests.cs");

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
                AssetDatabase.CreateFolder(aocFolder, yearDropdownValue.ToString());
            }

            if (!AssetDatabase.IsValidFolder(codeFolder))
            {
                Debug.Log("Create " + codeFolder);
                // Create asmdef
                AssetDatabase.CreateFolder(yearFolder, "Code");
                var asmdef = Path.Combine(codeFolder, $"AdventOfCode.{yearDropdownValue}.asmdef");
                using var streamWriter = new StreamWriter(asmdef);
                streamWriter.WriteLine("{");
                streamWriter.WriteLine($"\t\"name\": \"AdventOfCode.{yearDropdownValue}\",");
                streamWriter.WriteLine("\t\"references\": [");
                streamWriter.WriteLine("\t\t\"JTuresson.AdventOfCode\"");
                streamWriter.WriteLine("\t]");

                streamWriter.WriteLine("}");
                streamWriter.Close();
                AssetDatabase.Refresh();
/*
 * {
        "name": "NewAssembly"
    }

 */
            }

            if (!AssetDatabase.IsValidFolder(testsFolder))
            {
                Debug.Log("Create " + testsFolder);
                // Create asmdef
                AssetDatabase.CreateFolder(yearFolder, "Tests");
                var asmdef = Path.Combine(testsFolder, $"AdventOfCode.{yearDropdownValue}.Tests.asmdef");
                using var streamWriter = new StreamWriter(asmdef);
                streamWriter.WriteLine("{");
                streamWriter.WriteLine($"\t\"name\": \"AdventOfCode.{yearDropdownValue}.Tests\",");

                streamWriter.WriteLine("\t\"references\": [");
                streamWriter.WriteLine($"\t\t\"AdventOfCode.{yearDropdownValue}\"");
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
                streamWriter.WriteLine($"namespace AdventOfCode_{yearDropdownValue}");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine($"\tpublic static class Day{day}");
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
                streamWriter.WriteLine($"using AdventOfCode_{yearDropdownValue};");
                streamWriter.WriteLine("");
                streamWriter.WriteLine($"namespace AdventOfCode_{yearDropdownValue}Tests");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("\t[TestFixture]");
                streamWriter.WriteLine($"\tpublic class Day{day}Tests");
                streamWriter.WriteLine("\t{");
                streamWriter.WriteLine("\t\tprivate string _input;");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("\t\t[SetUp]");
                streamWriter.WriteLine("\t\tpublic void Setup()");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine(
                    $"\t\t\t_input = Resources.Load<TextAsset>(\"AdventOfCode/{yearDropdownValue}/{day}\").text;");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("\t\t[Test]");
                streamWriter.WriteLine("\t\tpublic void PuzzleATests()");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\tvar result = Day{day}.PuzzleA(_input);");
                streamWriter.WriteLine("\t\t\tAssert.AreEqual(\"expected result\", result);");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("\t\t[Test]");
                streamWriter.WriteLine("\t\tpublic void PuzzleBTests()");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine($"\t\t\tvar result = Day{day}.PuzzleB(_input);");
                streamWriter.WriteLine("\t\t\tAssert.AreEqual(\"expected result\", result);");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine("\t}");
                streamWriter.WriteLine("}");
                AssetDatabase.Refresh();
            }
/*
 * using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
 */
        }
    }
}