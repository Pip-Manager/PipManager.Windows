namespace PipManager.Core.Extensions;

public static class PathExtension
{
    public static bool PathEquals(this string path1, string path2)
    {
        return string.Equals(Path.GetFullPath(path1), Path.GetFullPath(path2), StringComparison.OrdinalIgnoreCase);
    }
}