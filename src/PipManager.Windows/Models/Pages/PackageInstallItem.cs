namespace PipManager.Windows.Models.Pages;

public class PackageInstallItem
{
    public string? PackageName { get; init; }
    public bool VersionSpecified { get; set; }
    public string? DistributionFilePath { get; init; }  // Distribution Install Only
    public string TargetVersion { get; set; } = string.Empty;
    public List<string>? AvailableVersions { get; init; }
}