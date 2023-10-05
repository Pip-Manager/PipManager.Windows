using Wpf.Ui.Controls;

namespace PipManager.Models.Pages;

public class LibraryListItem
{
    public LibraryListItem(SymbolIcon icon, string packageName, string packageVersion, string packageSummary)
    {
        PackageIcon = icon;
        PackageName = packageName;
        PackageVersion = packageVersion;
        PackageSummary = packageSummary;
    }

    public SymbolIcon PackageIcon { get; set; }
    public string PackageName { get; set; }
    public string PackageVersion { get; set; }
    public string PackageSummary { get; set; }
}