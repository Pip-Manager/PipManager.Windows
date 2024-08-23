namespace PipManager.Windows.Models.Pages;

public class LibraryInstallPackageItem
{
    public string? PackageName { get; set; }
    public bool VersionSpecified { get; set; }
    public string? DistributionFilePath { get; set; } // Distribution Install Only
    public string TargetVersion { get; set; } = string.Empty;
    public List<string>? AvailableVersions { get; set; }
}