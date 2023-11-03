using PipManager.Models.Package;
using Wpf.Ui.Controls;

namespace PipManager.Models.Pages;

public class LibraryListItem
{
    public LibraryListItem(SymbolIcon icon, string packageName, string packageVersion, PackageVersion packageDetailedVersion, string packageSummary, bool isSelected)
    {
        PackageIcon = icon;
        PackageName = packageName;
        PackageVersion = packageVersion;
        PackageSummary = packageSummary;
        IsSelected = isSelected;
        PackageDetailedVersion = packageDetailedVersion;
    }

    public SymbolIcon PackageIcon { get; set; }
    public string PackageName { get; set; }
    public string PackageVersion { get; set; }
    public PackageVersion PackageDetailedVersion { get; set; }
    public string PackageSummary { get; set; }
    public bool IsSelected { get; set; }
}