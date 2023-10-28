using PipManager.Models.Pages;

namespace PipManager.Models;

public class PackageItem
{
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required string Path { get; set; }
    public required string DistInfoPath { get; set; }
    public required string Summary { get; set; }
    public required List<string> Author { get; set; }
    public required string AuthorEmail { get; set; }
    public required List<LibraryDetailProjectUrlModel> ProjectUrl { get; set; }
    public required Dictionary<string, List<string>> Classifier { get; set; }
    public required Dictionary<string, List<string>> Metadata { get; set; }
    public required List<string> Record { get; set; }
}