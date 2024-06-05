using PipManager.Models.Pages;

namespace PipManager.Models.Package;

public class PackageItem
{
    public string? Name { get; init; }
    public string? Version { get; init; }
    public PackageVersion? DetailedVersion { get; init; }
    public string? Path { get; init; }
    public string? DistInfoPath { get; init; }
    public string? Summary { get; init; }
    public List<string>? Author { get; init; }
    public string? AuthorEmail { get; init; }
    public List<LibraryDetailProjectUrlModel>? ProjectUrl { get; init; }
    public Dictionary<string, List<string>>? Classifier { get; init; }
    public Dictionary<string, List<string>>? Metadata { get; init; }
    public List<string>? Record { get; init; }
}