namespace PipManager.Core.PyPackage.Models;

public class PackageUpdateItem
{
    public required string PackageName { get; init; }
    public required string PackageVersion { get; init; }
    public required string NewVersion { get; init; }
}
