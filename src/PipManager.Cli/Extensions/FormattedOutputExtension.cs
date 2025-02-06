using PipManager.Core.Configuration.Models;

namespace PipManager.Cli.Extensions;

public static class FormattedOutputExtension
{
    public static string Formatted(this EnvironmentModel environment, bool showPath = false)
    {
        return $"[bold]{environment.Identifier}[/] (Python {environment.PythonVersion} | Pip {environment.PipVersion})" + (showPath ? $" [grey]located at {environment.PythonPath})[/]" : "");
    }
}