using PipManager.Core.Configuration;
using PipManager.Core.PyEnvironment;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PipManager.Cli.Commands.Environment;


public class EnvRemoveSettings : EnvSettings
{
    [CommandOption("-p|--python-path <path>")]

    public string? PythonPath { get; init; }
    [CommandOption("-i|--identifier <identifier>")]

    public string? Identifier { get; init; }
}

public class EnvironmentRemoveCommand : Command<EnvRemoveSettings>
{
    public override ValidationResult Validate(CommandContext context, EnvRemoveSettings settings)
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
    public override int Execute(CommandContext context, EnvRemoveSettings settings)
    {
        if (!string.IsNullOrWhiteSpace(settings.PythonPath))
        {
            var response = Search.FindEnvironmentByPythonPath(settings.PythonPath);
            if (response is not null)
            {
                Configuration.AppConfig.Environments.Remove(response);
                if(Configuration.AppConfig.SelectedEnvironment == response)
                {
                    Configuration.AppConfig.SelectedEnvironment = null;
                }
                Configuration.Save();
                AnsiConsole.MarkupLine("[green]Environment removed successfully[/]");
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
                Configuration.AppConfig.Environments.Remove(response);
                if(Configuration.AppConfig.SelectedEnvironment == response)
                {
                    Configuration.AppConfig.SelectedEnvironment = null;
                }
                Configuration.Save();
                AnsiConsole.MarkupLine("[green]Environment removed successfully[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Environment not found[/]");
            }
        }

        return default;
    }
}