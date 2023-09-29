using Newtonsoft.Json;
using PipManager.Models.AppConfigModels;

namespace PipManager.Models;

public class AppConfig
{
    [JsonProperty("personalization")] public Personalization Personalization { get; set; } = new();
}