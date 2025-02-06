using System.Text.RegularExpressions;

namespace PipManager.Core.PyEnvironment.Helpers;

public static partial class Common
{
    [GeneratedRegex("__version__ = \"(.*?)\"", RegexOptions.IgnoreCase)]
    public static partial Regex GetPipVersionInInitFile();
}