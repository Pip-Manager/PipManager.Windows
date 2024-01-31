namespace PipManager.Models.Pages;

public class AboutNugetLibraryListItem(string libraryName, string libraryVersion, string libraryLicenseType, string libraryCopyright, string libraryUrl)
{
    public string LibraryName { get; set; } = libraryName;
    public string LibraryVersion { get; set; } = libraryVersion;
    public string LibraryLicenseType { get; set; } = libraryLicenseType;
    public string LibraryCopyright { get; set; } = libraryCopyright;
    public string LibraryUrl { get; set; } = libraryUrl;
}