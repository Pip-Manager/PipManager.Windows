using PipManager.Models.Pages;

namespace PipManager.Models.Package;

public class PackageItem
{
    public string Name { get; set; }
    public string Version { get; set; }
    public PackageVersion DetailedVersion { get; set; }
    public string Path { get; set; }
    public string DistInfoPath { get; set; }
    public string Summary { get; set; }
    public List<string> Author { get; set; }
    public string AuthorEmail { get; set; }
    public List<LibraryDetailProjectUrlModel> ProjectUrl { get; set; }
    public Dictionary<string, List<string>> Classifier { get; set; }
    public Dictionary<string, List<string>> Metadata { get; set; }
    public List<string> Record { get; set; }
}