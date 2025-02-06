using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using PipManager.Core.Extensions;
using PipManager.Core.PyPackage.Models;
using PipManager.Core.Wrappers.PackageSearchIndexWrapper;

namespace PipManager.Core.Services.PackageSearchService;

public partial class PackageSearchService(HttpClient httpClient) : IPackageSearchService
{
    public async Task<List<IndexItemModel>?> GetIndexAsync(PackageSourceType packageSourceType)
    {
        var packageSourceIndexUrl = packageSourceType.GetPackageSourceUrl();
        try
        {
            var indexContent = await httpClient.GetStringAsync(packageSourceIndexUrl);
            var matches = IndexContentRegex().Matches(indexContent);
            var indexItemModels = new List<IndexItemModel>();

            foreach (Match match in matches)
            {
                indexItemModels.Add(new IndexItemModel
                {
                    Url = match.Groups[1].Value,
                    Name = match.Groups[2].Value
                });
            }

            return indexItemModels;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    [GeneratedRegexAttribute(@"<a\s+.*?href=""([^""]+)""[^>]*>\s*([^<]+?)\s*</a>", RegexOptions.IgnoreCase, "zh-CN")]
    private static partial Regex IndexContentRegex();
}   