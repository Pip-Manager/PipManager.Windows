namespace PipManager.Models;

public class PackageItem
{
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required string Path { get; set; }
    public required string Summary { get; set; }
    public required Dictionary<string, List<string>> Metadata { get; set; }
}