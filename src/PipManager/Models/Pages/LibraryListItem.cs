using PipManager.Models.Package;
using Wpf.Ui.Controls;

namespace PipManager.Models.Pages;

public class LibraryListItem(
    SymbolIcon icon,
    string packageName,
    string packageVersion,
    PackageVersion packageDetailedVersion,
    string packageSummary,
    bool isSelected)
{
    public SymbolIcon PackageIcon { get; set; } = icon;
    public string PackageName { get; set; } = packageName;
    public string PackageVersion { get; set; } = packageVersion;
    public PackageVersion PackageDetailedVersion { get; set; } = packageDetailedVersion;
    public string PackageSummary { get; set; } = packageSummary;
    public bool IsSelected { get; set; } = isSelected;
}