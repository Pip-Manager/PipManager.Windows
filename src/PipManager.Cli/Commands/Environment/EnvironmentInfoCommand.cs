using System.Runtime.InteropServices;
using PipManager.Core.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PipManager.Cli.Commands.Environment;

public class EnvironmentInfoCommand : Command<EnvSettings>
{
    public override int Execute(CommandContext context, EnvSettings settings)
    {
        if(Configuration.AppConfig.Environments.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Python environment has not been added yet, add it with the 'env add' command.[/]");
            return default;
        }
        if(Configuration.AppConfig.SelectedEnvironment == null)
        {
            AnsiConsole.MarkupLine("[red]No environment selected, select it with the 'env select' command.[/]");
            return default;
        }
        
        Configuration.UpdateSelectedEnvironment();
        var environment = Configuration.AppConfig.SelectedEnvironment;
        
        AnsiConsole.MarkupLine("[Orange1]System Information[/]");
        AnsiConsole.MarkupLine($"[green]Platform:[/] {RuntimeInformation.OSDescription}");
        AnsiConsole.MarkupLine($"[blue]Architecture:[/] {RuntimeInformation.OSArchitecture}");
        
        AnsiConsole.MarkupLine("[Orange1]Current Environment Information[/]");
        AnsiConsole.MarkupLine($"[green]Identifier:[/] {environment.Identifier}");
        AnsiConsole.MarkupLine($"[blue]Pip Version:[/] {environment.PipVersion}");
        AnsiConsole.MarkupLine($"[blue]Python Version:[/] {environment.PythonVersion}");
        AnsiConsole.MarkupLine($"[blue]Python Path:[/] {environment.PythonPath}");

        return default;
    }
}