using PipManager.Languages;
using PipManager.Models.Action;

namespace PipManager.Helpers;

public static class ActionExceptionAnalyzer
{
    public static string Analyze(this ActionListItem action)
    {
        if (!action.DetectIssue) return "";
        var speculations = "";
        foreach (var line in action.ConsoleError.Split('\n'))
        {
            var message = line.Trim();
            if (message.Contains("ERROR: No matching distribution found for "))
            {
                message = message.Replace("ERROR: No matching distribution found for ", "");
                if (message.Contains("=="))
                {
                    speculations += string.Format(Lang.ActionExceptionAnalyze_NoMatchingDistFound_VersionMatching, message.Split("==")[0], message.Split("==")[1]) + '\n';
                }
                else if (message.Contains("~="))
                {
                    speculations += string.Format(Lang.ActionExceptionAnalyze_NoMatchingDistFound_Compatible, message.Split("~=")[0], message.Split("~=")[1]) + '\n';
                }
                else if (message.Contains("<="))
                {
                    speculations += string.Format(Lang.ActionExceptionAnalyze_NoMatchingDistFound_InclusiveOrderedLess, message.Split("<=")[0], message.Split("<=")[1]) + '\n';
                }
                else if (message.Contains(">="))
                {
                    speculations += string.Format(Lang.ActionExceptionAnalyze_NoMatchingDistFound_InclusiveOrderedMore, message.Split(">=")[0], message.Split(">=")[1]) + '\n';
                }
                else if (message.Contains('<'))
                {
                    speculations += string.Format(Lang.ActionExceptionAnalyze_NoMatchingDistFound_ExclusiveOrderedLess, message.Split("<")[0], message.Split("<")[1]) + '\n';
                }
                else if (message.Contains('>'))
                {
                    speculations += string.Format(Lang.ActionExceptionAnalyze_NoMatchingDistFound_InclusiveOrderedMore, message.Split(">")[0], message.Split(">")[1]) + '\n';
                }
                else if (message.Contains("==="))
                {
                    speculations += string.Format(Lang.ActionExceptionAnalyze_NoMatchingDistFound_ArbitraryEquality, message.Split("===")[0], message.Split("===")[1]) + '\n';
                }
            }
        }

        return speculations;
    }
}