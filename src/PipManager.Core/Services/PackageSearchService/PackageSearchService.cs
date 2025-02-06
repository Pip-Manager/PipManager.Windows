using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using PipManager.Core.Wrappers.PackageSearchQueryWrapper;

namespace PipManager.Core.Services.PackageSearchService;

public partial class PackageSearchService(HttpClient httpClient) : IPackageSearchService
{
    private Dictionary<(string, int), QueryWrapper> QueryCaches { get; } = [];

    public async ValueTask<QueryWrapper> Query(string name, int page = 1)
    {
        name = Uri.EscapeDataString(name);
        if (QueryCaches.ContainsKey((name, page)))
        {
            return QueryCaches[(name, page)];
        }
        string htmlContent;
        try
        {
            var searchUrl = $"https://pypi.org/search/?q={name}&page={page}";
            htmlContent = await HandleAntiBotChallenge(searchUrl);
        }
        catch (Exception exception) when (exception is TaskCanceledException or HttpRequestException)
        {
            return new QueryWrapper
            {
                Status = QueryStatus.Timeout
            };
        }
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlContent);

        var queryWrapper = new QueryWrapper
        {
            ResultCount = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='left-layout__main']//strong").InnerText
        };
        queryWrapper.Status = queryWrapper.ResultCount != "0" ? QueryStatus.Success : QueryStatus.NoResults;
        if (queryWrapper.Status == QueryStatus.NoResults)
        {
            return queryWrapper;
        }
        var pageNode = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'button-group')]");
        queryWrapper.MaxPageNumber = pageNode == null ? 1 : int.Parse(pageNode.ChildNodes[^4].InnerText);

        try
        {
            var resultList = htmlDocument.DocumentNode.SelectSingleNode("//ul[@aria-label='Search results']")
                .ChildNodes
                .Where(result => result.InnerLength != 15)
                .Select(result => result.ChildNodes[1]);
            queryWrapper.Results = [];
            foreach (var resultItem in resultList)
            {
                queryWrapper.Results.Add(new QueryListItemModel
                {
                    Name = resultItem.ChildNodes[1].ChildNodes[1].InnerText,
                    Description = resultItem.ChildNodes[3].InnerText,
                    Url = $"https://pypi.org{resultItem.Attributes["href"].Value}",
                    // ReSharper disable once StringLiteralTypo
                    UpdateTime = DateTime.ParseExact(resultItem.ChildNodes[1].ChildNodes[3].FirstChild.Attributes["datetime"].Value, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.RoundtripKind)
                });
            }
        }
        catch (ArgumentOutOfRangeException)
        {
            QueryCaches.Add((name, page), queryWrapper);
            return queryWrapper;
        }
        QueryCaches.Add((name, page), queryWrapper);
        return queryWrapper;
    }

    // PyPi Search Anti-Bot Challenge
    // Thanks to AneryCoft for the solution (Issue #22)
    private async Task<string> HandleAntiBotChallenge(string searchUrl)
    {
        var initialResponse = await httpClient.GetAsync(searchUrl);
        var initialHtml = await initialResponse.Content.ReadAsStringAsync();

        var scriptUrlMatch = ScriptUrlRegex().Match(initialHtml);
        if (!scriptUrlMatch.Success) return initialHtml;

        var scriptUrl = scriptUrlMatch.Groups[1].Value;
        var scriptResponse = await httpClient.GetAsync($"https://pypi.org{scriptUrl}");
        var scriptContent = await scriptResponse.Content.ReadAsStringAsync();

        var paramMatch = ScriptContentRegex().Match(scriptContent);
        if (!paramMatch.Success) throw new HttpRequestException($"[{nameof(PackageSearchService)}] Failed to extract challenge parameters");

        var (baseStr, hash, hmac, expires, token, postUrlPart) = 
            (paramMatch.Groups[1].Value, paramMatch.Groups[2].Value, paramMatch.Groups[3].Value,
                paramMatch.Groups[4].Value, paramMatch.Groups[5].Value, paramMatch.Groups[6].Value);

        var answer = FindChallengeAnswer(baseStr, hash);
        if (string.IsNullOrEmpty(answer)) throw new HttpRequestException($"[{nameof(PackageSearchService)}] Failed to solve challenge");
        
        // Send payload
        var postUrl = $"https://pypi.org{postUrlPart}/fst-post-back";
        var payload = new
        {
            token,
            data = new
            {
                ty = "pow",
                @base = baseStr,
                answer,
                hmac,
                expires
            }
        };

        var json = JsonSerializer.Serialize(payload);
        await httpClient.PostAsync(postUrl, new StringContent(json, Encoding.UTF8, "application/json"));

        return await httpClient.GetStringAsync(searchUrl);
    }
    
    private static string? FindChallengeAnswer(string baseStr, string targetHash)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        
        foreach (var i in chars)
        foreach (var j in chars)
        {
            var candidate = baseStr + i + j;
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(candidate));
            var hexHash = Convert.ToHexStringLower(hashBytes);

            if (hexHash == targetHash)
                return $"{i}{j}";
        }

        return null;
    }

    [GeneratedRegex("script.src = '(.+?)'")]
    private static partial Regex ScriptUrlRegex();
    [GeneratedRegex(@"init\(\[.*?""base"":""(.+?)"",""hash"":""(.+?)"",""hmac"":""(.+?)"",""expires"":""(.+?)"".*?\],\s*""([^""]+)""\s*,\s*""([^""]+)""")]
    private static partial Regex ScriptContentRegex();
}   