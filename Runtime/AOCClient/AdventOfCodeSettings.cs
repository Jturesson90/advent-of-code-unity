using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace JTuresson.AdventOfCode.AOCClient
{
    public class AdventOfCodeSettings : ScriptableObject, IAdventOfCodeSettings
    {
        [SerializeField] private string session;
        [SerializeField] private int year;
        [SerializeField] private AdventOfCodeCache adventOfCodeCache;
        public string Session => session;
        public int Year => year;
        public static AdventOfCodeSettings Instance { get; private set; }

        public static event Action ExistChanged;
        private void OnEnable()
        {
            Instance = this;
        }
#if UNITY_EDITOR
        public static void CreateAsset()
        {
            var path = EditorUtility.SaveFilePanelInProject("Advent of Code Settings",
                "AdventOfCodeSettings", "asset",
                "Please create the Settings for AdventOfCode");
            if (string.IsNullOrEmpty(path))
                return;

            var configObject = CreateInstance<AdventOfCodeSettings>();
            AssetDatabase.CreateAsset(configObject, path);

            // Add the config asset to the build
            var preloadedAssets = UnityEditor.PlayerSettings.GetPreloadedAssets().ToList();
            preloadedAssets.Add(configObject);
            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            Instance = configObject;
            var item = CreateInstance<AdventOfCodeCache>();
            item.name = ObjectNames.NicifyVariableName(nameof(AdventOfCodeCache));
            AssetDatabase.AddObjectToAsset(item, Instance);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(item));
            Instance.adventOfCodeCache = item;
            ExistChanged?.Invoke();
        }
#endif
        public void ClearCache()
        {
            adventOfCodeCache.DeleteAll();
        }

        public IAdventOfCodeCache GetCache()
        {
            if (adventOfCodeCache)
                return adventOfCodeCache;
            adventOfCodeCache =
                AssetDatabase.LoadAssetAtPath<AdventOfCodeCache>(AssetDatabase.GetAssetPath(this));
            if (adventOfCodeCache)
                return adventOfCodeCache;
            throw new Exception(
                "Cant find Cache, please remove this AdventOfCode Settings and create new");
        }

        private void OnDestroy()
        {
            Debug.Log("AdventOfCodeSettings OnDestroy");
            Instance = null;
            ExistChanged?.Invoke();
        }
    }
}