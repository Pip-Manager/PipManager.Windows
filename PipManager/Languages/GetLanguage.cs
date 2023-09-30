using System.IO;
using Newtonsoft.Json;
using PipManager.Models;

namespace PipManager.Languages;

public static class GetLanguage
{
    public static readonly Dictionary<string, string> LanguageList = new()
    {
        {"English", "en-US"},
        {"简体中文", "zh-CN"}
    };

    public static string? FromFile(string path)
    {
        var language = "Auto";
        if (File.Exists(path))
        {
            language = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(path)).Personalization.Language;
        }
        return language != "Auto" ? language : null;
    }
    
}