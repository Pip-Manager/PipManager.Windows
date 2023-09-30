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
}