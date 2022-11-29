using System;
using System.Threading.Tasks;
using HtmlAgilityPack;
using UnityEngine;
using UnityEngine.Networking;

namespace JTuresson.AdventOfCode.AOCClient
{
    public class AdventOfCodeClient
    {
        private readonly IAdventOfCodeSettings _settings;
        private readonly IAdventOfCodeCache _cache;

        public AdventOfCodeClient(IAdventOfCodeSettings settings, IAdventOfCodeCache cache)
        {
            _settings = settings;
            _cache = cache;
        }

        public async Task<bool> SessionIsValid()
        {
            const string uri = "https://adventofcode.com/2021/day/10/input";
            using var www = UnityWebRequest.Get(uri);
            www.SetRequestHeader("Cookie", $"session={_settings.Session}");
            var operation = www.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();
            switch (www.result)
            {
                case UnityWebRequest.Result.InProgress:
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.DataProcessingError:
                    return await Task.FromResult(false);
                case UnityWebRequest.Result.Success:
                    return await Task.FromResult(true);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<bool> CanGetDay(int day)
        {
            var uri = $"https://adventofcode.com/{_settings.Year}/day/{day}/input";
            using var www = UnityWebRequest.Get(uri);
            www.SetRequestHeader("Cookie", $"session={_settings.Session}");
            var operation = www.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();
            var text = www.downloadHandler.text;
            switch (www.result)
            {
                case UnityWebRequest.Result.InProgress:
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.DataProcessingError:
                    return await Task.FromResult(false);
                case UnityWebRequest.Result.Success:
                    return await Task.FromResult(
                        !text.Equals(string.Empty) && !text.Contains(
                            "Please don't repeatedly request this endpoint before it unlocks!") &&
                        !text.Contains("404 Not Found"));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<string> LoadDescription(int day)
        {
            if (_cache.HasDescription(_settings.Year, day))
            {
                Debug.Log("Got Desc from Cache for day " + day);
                return _cache.GetDescription(_settings.Year, day);
            }

            var uri = $"https://adventofcode.com/{_settings.Year}/day/{day}";
            using var www = UnityWebRequest.Get(uri);
            www.SetRequestHeader("Cookie", $"session={_settings.Session}");
            var operation = www.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();
            var html = www.downloadHandler.text;
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var b = htmlDoc.DocumentNode.SelectSingleNode("//article");
            switch (www.result)
            {
                case UnityWebRequest.Result.InProgress:
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.DataProcessingError:
                    return await Task.FromResult(string.Empty);
                case UnityWebRequest.Result.Success:
                    _cache.AddDescription(_settings.Year, day, b.InnerText);
                    return await Task.FromResult(b.InnerText);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<string> LoadDayInput(int day)
        {
            if (_cache.HasInput(_settings.Year, day))
            {
                Debug.Log("Got Input from Cache for day " + day);
                return _cache.GetInput(_settings.Year, day);
            }

            var uri = $"https://adventofcode.com/{_settings.Year}/day/{day}/input";
            using var www = UnityWebRequest.Get(uri);
            www.SetRequestHeader("Cookie", $"session={_settings.Session}");
            var operation = www.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();
            var text = www.downloadHandler.text;
            switch (www.result)
            {
                case UnityWebRequest.Result.InProgress:
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.DataProcessingError:
                    return await Task.FromResult(string.Empty);
                case UnityWebRequest.Result.Success:
                    _cache.AddInput(_settings.Year, day, text);
                    return await Task.FromResult(text);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}