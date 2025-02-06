using PipManager.Core.PyPackage.Models;

namespace PipManager.Windows.Models.Pages;

public class PackageListItem
{
    public required string PackageName { get; init; }
    public required string PackageVersion { get; init; }
    public required PackageVersion PackageDetailedVersion { get; init; }
    public required string PackageSummary { get; init; }
    public required bool IsSelected { get; set; }
}