using System.Text.Json.Serialization;

namespace PipManager.Core.Configuration.Models;

[JsonSerializable(typeof(PackageSourceModel))]
public partial class PackageSourceModelContext : JsonSerializerContext;

public class PackageSourceModel
{
    [JsonPropertyName("source")] public string Source { get; set; } = "official";

    [JsonPropertyName("allow_non_release")] public bool AllowNonRelease { get; set; }
}