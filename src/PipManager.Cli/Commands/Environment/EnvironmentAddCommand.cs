using PipManager.Core.Configuration;
using PipManager.Core.Configuration.Models;
using PipManager.Core.PyEnvironment;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PipManager.Cli.Commands.Environment;

public class EnvAddSettings : EnvSettings
{
    [CommandOption("-a|--auto")]
    public bool AutomaticallyDetect { get; init; }
    [CommandOption("-p|--python-path <path>")]

    public string? PythonPath { get; init; }
    [CommandOption("-i|--identifier <identifier>")]

    public string? Identifier { get; init; }
}

public class EnvironmentAddCommand : Command<EnvAddSettings>
{
    public override ValidationResult Validate(CommandContext context, EnvAddSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.PythonPath) && !settings.AutomaticallyDetect)
        {
            return ValidationResult.Error("Specify a Python path or use the -a/--auto flag");
        }
        if (!string.IsNullOrWhiteSpace(settings.PythonPath) && settings.AutomaticallyDetect)
        {
            return ValidationResult.Error("Specify a Python path or use the -a/--auto flag, not both");
        }

        return base.Validate(context, settings);
    }

    public override int Execute(CommandContext context, EnvAddSettings settings)
    {
        EnvironmentModel? environment;
        if (settings.AutomaticallyDetect)
        {
            var environments = Detector.ByEnvironmentVariable();
            var formattedEnvironments = environments.Select(env => $"Pip {env.PipVersion} (Python {env.PythonVersion}) located at {env.PythonPath}");
                
            var targetEnvironment = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Select environment (from environment variables):[/]")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more environments)[/]")
                    .AddChoices(formattedEnvironments));
                
            environment = environments.First(env => targetEnvironment.Contains(env.PythonPath));
        }
        else
        {
            environment = Detector.ByPythonPath(settings.PythonPath!);
            if(environment == null)
            {
                AnsiConsole.MarkupLine("[red]Python path is not found[/]");
                return default;
            }
        }
        
        if (Search.FindEnvironmentByPythonPath(environment.PythonPath) is not null)
        {
            AnsiConsole.MarkupLine("[red]Environment already exists[/]");
            return default;
        }

        var identifier = settings.Identifier ?? AnsiConsole.Ask<string>("Set an [bold]identifier[/] for this environment: ");

        if(string.IsNullOrWhiteSpace(identifier))
        {
            AnsiConsole.MarkupLine("[red]Identifier cannot be empty[/]");
            return default;
        }
        if (Search.FindEnvironmentByIdentifier(identifier) is not null)
        {
            AnsiConsole.MarkupLine("[red]Identifier already exists[/]");
            return default;
        }
        environment.Identifier = identifier;
                
        if (AnsiConsole.Confirm("Switch to this environment?"))
        {
            Configuration.AppConfig.SelectedEnvironment = environment;
        }
                
        Configuration.AppConfig.Environments.Add(environment);
        Configuration.Save();
                    
        AnsiConsole.MarkupLine("[bold green]Environment added successfully[/]");

        return default;
    }
}