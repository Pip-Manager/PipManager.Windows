using PipManager.Models.Package;
using System.Text.RegularExpressions;

namespace PipManager.Helpers;

public static partial class PackageValidator
{
    public static PackageVersion CheckVersion(string version)
    {
        var match = VersionRegex().Match(version);
        if (match.Success)
        {
            return new PackageVersion
            {
                Epoch = match.Groups["epoch"].Value,
                Release = match.Groups["release"].Value,
                PreL = match.Groups["pre_l"].Value,
                PreN = match.Groups["pre_n"].Value,
                PostN1 = match.Groups["post_n1"].Value,
                PostL = match.Groups["post_l"].Value,
                PostN2 = match.Groups["post_n2"].Value,
                DevL = match.Groups["dev_l"].Value,
                DevN = match.Groups["dev_n"].Value,
                Local = match.Groups["local"].Value
            };
        }

        return new PackageVersion();
    }

    [GeneratedRegex(
        "^\\s*\r\n            v?\r\n            (?:\r\n                (?:(?<epoch>[0-9]+)!)?\r\n                (?<release>[0-9]+(?:\\.[0-9]+)*)\r\n                (?<pre>\r\n                    [-_\\.]?\r\n                    (?<pre_l>(a|b|c|rc|alpha|beta|pre|preview))\r\n                    [-_\\.]?\r\n                    (?<pre_n>[0-9]+)?\r\n                )?\r\n                (?<post>\r\n                    (?:-(?<post_n1>[0-9]+))\r\n                    |\r\n                    (?:\r\n                        [-_\\.]?\r\n                        (?<post_l>post|rev|r)\r\n                        [-_\\.]?\r\n                        (?<post_n2>[0-9]+)?\r\n                    )\r\n                )?\r\n                (?<dev>\r\n                    [-_\\.]?\r\n                    (?<dev_l>dev)\r\n                    [-_\\.]?\r\n                    (?<dev_n>[0-9]+)?\r\n                )?\r\n            )\r\n            (?:\\+(?<local>[a-z0-9]+(?:[-_\\.][a-z0-9]+)*))?\\s*$",
        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)]
    private static partial Regex VersionRegex();
}