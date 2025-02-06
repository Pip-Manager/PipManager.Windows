using PipManager.Core.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PipManager.Cli.Commands.Environment;

public class EnvSettings : CommandSettings;

public class EnvironmentCommand : Command<EnvSettings>
{
    public override int Execute(CommandContext context, EnvSettings settings)
    {
        var selectedEnvironment = Configuration.AppConfig.SelectedEnvironment;
        var environments = Configuration.AppConfig.Environments;
        switch (environments.Count)
        {
            case 0:
                AnsiConsole.MarkupLine("Python environment has not been added yet, add it with the 'env add' command.");
                break;
            default:
                switch (selectedEnvironment)
                {
                    case null:
                        AnsiConsole.MarkupLine("No environment selected, select it with the 'env select' command.");
                        break;
                    default:
                        AnsiConsole.MarkupLine($"Selected environment: Pip {selectedEnvironment.PipVersion} (Python {selectedEnvironment.PythonVersion})");
                        AnsiConsole.MarkupLine($"Located at: {selectedEnvironment.PythonPath}");
                        break;
                }
                break;
        }

        return default;
    }
}