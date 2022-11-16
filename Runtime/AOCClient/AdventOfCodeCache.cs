using System;
using UnityEngine;

namespace JTuresson.AdventOfCode.AOCClient
{
    [Serializable]
    public class AdventOfCodeCacheModel
    {
        public string input;
        public string description;
    }

    public class AdventOfCodeCache : ScriptableObject, IAdventOfCodeCache
    {
        [SerializeField]
        private SerializableDictionary<int, SerializableDictionary<int, AdventOfCodeCacheModel>> cache =
            new();

        public void AddInput(int year, int day, string input)
        {
            if (!cache.ContainsKey(year))
                cache.Add(year, new SerializableDictionary<int, AdventOfCodeCacheModel>());
            if (cache[year].ContainsKey(day))
            {
                cache[year][day].input = input;
            }
            else
            {
                cache[year].Add(day, new AdventOfCodeCacheModel() {input = input});
            }
        }

        public bool HasInput(int year, int day)
        {
            var yearCache = cache.ContainsKey(year) ? cache[year] : null;
            return yearCache != null && yearCache.ContainsKey(day) &&
                   !yearCache[day].input.Equals(string.Empty);
        }

        public void DeleteDay(int year, int day)
        {
            if (cache.ContainsKey(year) && cache[year].ContainsKey(day))
                cache[year].Remove(day);
        }

        public void DeleteYear(int year)
        {
            if (cache.ContainsKey(year))
            {
                cache.Remove(year);
            }
        }

        public void DeleteAll()
        {
            cache.Clear();
        }

        public void AddDescription(int year, int day, string description)
        {
            if (!cache.ContainsKey(year))
                cache.Add(year, new SerializableDictionary<int, AdventOfCodeCacheModel>());
            if (cache[year].ContainsKey(day))
            {
                cache[year][day].description = description;
            }
            else
            {
                cache[year].Add(day, new AdventOfCodeCacheModel() {description = description});
            }
        }

        public bool HasDescription(int year, int day)
        {
            var yearCache = cache.ContainsKey(year) ? cache[year] : null;
            return yearCache != null && yearCache.ContainsKey(day) &&
                   !yearCache[day].description.Equals(string.Empty);
        }

        public string GetDescription(int year, int day)
        {
            var yearCache = cache.ContainsKey(year) ? cache[year] : null;
            if (yearCache == null)
                throw new ArgumentException($"No cached inputs for year {year} found.");
            if (!yearCache.ContainsKey(day))
                throw new ArgumentException($"Input for {year} day {day} not found in cache");
            return yearCache[day].description;
        }

        private bool InputExistsInCache(int year, int day)
        {
            var yearCache = cache.ContainsKey(year) ? cache[year] : null;
            return yearCache != null && yearCache.ContainsKey(day);
        }

        public string GetInput(int year, int day)
        {
            var yearCache = cache.ContainsKey(year) ? cache[year] : null;
            if (yearCache == null)
                throw new ArgumentException($"No cached inputs for year {year} found.");
            if (!yearCache.ContainsKey(day))
                throw new ArgumentException($"Input for {year} day {day} not found in cache");
            return yearCache[day].input;
        }
    }
}