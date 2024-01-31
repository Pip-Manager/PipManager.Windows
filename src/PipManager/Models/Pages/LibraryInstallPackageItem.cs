namespace PipManager.Models.Pages;

public class LibraryInstallPackageItem
{
    public required string PackageName { get; set; }
    public bool VersionSpecified { get; set; } = false;
    public string VersionSpecifiedType { get; set; } = "~=";
    public string TargetVersion { get; set; } = string.Empty;
    public required List<string> AvailableVersions { get; set; }
}