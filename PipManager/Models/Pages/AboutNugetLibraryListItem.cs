namespace PipManager.Models.Pages;

public class AboutNugetLibraryListItem
{
    public AboutNugetLibraryListItem(string libraryName, string libraryVersion, string libraryLicenseType, string libraryCopyright, string libraryUrl)
    {
        LibraryName = libraryName;
        LibraryVersion = libraryVersion;
        LibraryLicenseType = libraryLicenseType;
        LibraryCopyright = libraryCopyright;
        LibraryUrl = libraryUrl;
    }

    public string LibraryName { get; set; }
    public string LibraryVersion { get; set; }
    public string LibraryLicenseType { get; set; }
    public string LibraryCopyright { get; set; }
    public string LibraryUrl { get; set; }
}