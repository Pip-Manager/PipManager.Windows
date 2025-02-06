using PipManager.Cli.Extensions;
using PipManager.Core.Configuration;
using PipManager.Core.PyEnvironment;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PipManager.Cli.Commands.Environment;

public class EnvSwitchSettings : EnvSettings
{
    [CommandOption("-p|--python-path <path>")]

    public string? PythonPath { get; init; }
    [CommandOption("-i|--identifier <identifier>")]

    public string? Identifier { get; init; }
}

public class EnvironmentSwitchCommand : Command<EnvSwitchSettings>
{
    public override ValidationResult Validate(CommandContext context, EnvSwitchSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.PythonPath) && string.IsNullOrWhiteSpace(settings.Identifier))
        {
            return ValidationResult.Error("Specify a Python path or an identifier");
        }

        if (!string.IsNullOrWhiteSpace(settings.PythonPath) && !string.IsNullOrWhiteSpace(settings.Identifier))
        {
            return ValidationResult.Error("Specify a Python path or an identifier, not both");
        }

        return base.Validate(context, settings);
    }

    public override int Execute(CommandContext context, EnvSwitchSettings settings)
    {
        AnsiConsole.MarkupLine($"Current Environment: {Configuration.AppConfig.SelectedEnvironment!.Formatted()}");
        if (!string.IsNullOrWhiteSpace(settings.PythonPath))
        {
            var response = Search.FindEnvironmentByPythonPath(settings.PythonPath);
            if (response is not null)
            {
                if(response.PythonPath == Configuration.AppConfig.SelectedEnvironment?.PythonPath)
                {
                    AnsiConsole.MarkupLine("[orange1]This environment is already selected[/]");
                    return default;
                }
                Configuration.AppConfig.SelectedEnvironment = response;
                Configuration.Save();
                AnsiConsole.MarkupLine($"[green]Environment switched: {response.Identifier} (Python {response.PythonVersion} | Pip {response.PipVersion})[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Environment not found[/]");
            }
        }
        else
        {
            var response = Search.FindEnvironmentByIdentifier(settings.Identifier!);
            if (response is not null)
            {
                if(response.Identifier == Configuration.AppConfig.SelectedEnvironment?.Identifier)
                {
                    AnsiConsole.MarkupLine("[orange1]This environment is already selected[/]");
                    return default;
                }
                Configuration.AppConfig.SelectedEnvironment = response;
                Configuration.Save();
                AnsiConsole.MarkupLine($"[green]Environment switched: {response.Identifier} (Python {response.PythonVersion} | Pip {response.PipVersion})[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Environment not found[/]");
            }
        }

        return default;
    }
}