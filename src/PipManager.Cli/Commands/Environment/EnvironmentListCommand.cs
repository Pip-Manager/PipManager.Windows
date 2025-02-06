using PipManager.Core.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PipManager.Cli.Commands.Environment;

public class EnvironmentListCommand : Command<EnvSettings>
{
    public override ValidationResult Validate(CommandContext context, EnvSettings settings)
    {
        if(Configuration.AppConfig.Environments.Count == 0)
        {
            return ValidationResult.Error("Python environment has not been added yet, add it with the 'env add' command.");
        }

        return base.Validate(context, settings);
    }
    
    public override int Execute(CommandContext context, EnvSettings settings)
    {
        var table = new Table
        {
            Border = TableBorder.Rounded
        };

        table.AddColumns("Identifier", "Pip Version", "Python Version", "Python Path");
        foreach (var environment in Configuration.AppConfig.Environments)
        {
            table.AddRow(environment.Identifier, environment.PipVersion, environment.PythonVersion, environment.PythonPath);
        }

        AnsiConsole.Write(table);
        return default;
    }
}