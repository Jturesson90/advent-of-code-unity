using System;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace JTuresson.AdventOfCode.AOCClient
{
    public class AdventOfCodeClient
    {
        public async Task<bool> SessionIsValid(string session)
        {
            const string uri = "https://adventofcode.com/2021/day/10/input";
            using var www = UnityWebRequest.Get(uri);
            www.SetRequestHeader("Cookie", $"session={session}");
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

        public async Task<bool> CanGetDay(string session, int year, int day)
        {
            var uri = $"https://adventofcode.com/{year}/day/{day}/input";
            using var www = UnityWebRequest.Get(uri);
            www.SetRequestHeader("Cookie", $"session={session}");
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
/*
        public async Task<string> LoadDescription(int year, int day)
        {
            if (_cache.HasDescription(year, day))
            {
                Debug.Log("Got Desc from Cache for day " + day);
                return _cache.GetDescription(year, day);
            }

            var uri = $"https://adventofcode.com/{year}/day/{day}";
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
                    _cache.AddDescription(year, day, b.InnerText);
                    return await Task.FromResult(b.InnerText);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }*/

        public async Task<string> LoadDayInput(string session, int year, int day)
        {
            var uri = $"https://adventofcode.com/{year}/day/{day}/input";
            using var www = UnityWebRequest.Get(uri);
            www.SetRequestHeader("Cookie", $"session={session}");
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
                    return await Task.FromResult(text);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}