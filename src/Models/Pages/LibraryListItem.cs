using PipManager.Core.PyPackage.Models;

namespace PipManager.Windows.Models.Pages;

public class LibraryListItem(
    string packageName,
    string packageVersion,
    PackageVersion packageDetailedVersion,
    string packageSummary,
    bool isSelected)
{
    public string PackageName { get; set; } = packageName;
    public string PackageVersion { get; set; } = packageVersion;
    public PackageVersion PackageDetailedVersion { get; set; } = packageDetailedVersion;
    public string PackageSummary { get; set; } = packageSummary;
    public bool IsSelected { get; set; } = isSelected;
}