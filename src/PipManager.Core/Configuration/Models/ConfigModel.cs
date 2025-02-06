using System.Text.Json.Serialization;

namespace PipManager.Core.Configuration.Models;

[JsonSerializable(typeof(ConfigModel))]
public partial class CliConfigModelContext : JsonSerializerContext;

public class ConfigModel
{
    [JsonPropertyName("selectedEnvironment")]
    public EnvironmentModel? SelectedEnvironment { get; set; }
    [JsonPropertyName("environments")]
    public List<EnvironmentModel> Environments { get; set; } = [];
    [JsonPropertyName("packageSource")]
    public PackageSourceModel PackageSource { get; set; } = new();
    [JsonPropertyName("personalization")]  // UI-based App Configuration
    public PersonalizationModel Personalization { get; set; } = new();
}