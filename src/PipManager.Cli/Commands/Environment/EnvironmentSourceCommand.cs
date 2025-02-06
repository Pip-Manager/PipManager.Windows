using PipManager.Core.Configuration;
using PipManager.Core.Extensions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PipManager.Cli.Commands.Environment;

public class EnvSourceSettings : EnvSettings
{
    [CommandOption("--default")]
    public bool UseDefault { get; init; }
    
    [CommandOption("--tsinghua")]
    public bool UseTsinghua { get; init; }
    
    [CommandOption("--custom <url>")]
    public string? CustomUrl { get; init; }
}

public class EnvironmentSourceCommand : Command<EnvSourceSettings>
{
    public override ValidationResult Validate(CommandContext context, EnvSourceSettings settings)
    {
        var check = (settings.UseDefault ? 1 : 0) + (settings.UseTsinghua ? 1 : 0) +
                    (string.IsNullOrWhiteSpace(settings.CustomUrl) ? 0 : 1);
        if (!settings.CustomUrl!.CheckUrlValid() && settings is { UseDefault: false, UseTsinghua: false })
        {
            return ValidationResult.Error("Invalid URL");
        }
        return check != 1 ? ValidationResult.Error("Only one of Default, Tsinghua and Custom can be specified, not all of them.") : base.Validate(context, settings);
    }
    
    public override int Execute(CommandContext context, EnvSourceSettings settings)
    {
        if (settings.UseDefault)
        {
            Configuration.AppConfig.PackageSource.Source = "official";
        }
        else if (settings.UseTsinghua)
        {
            Configuration.AppConfig.PackageSource.Source = "tsinghua";
        }
        else
        {
            Configuration.AppConfig.PackageSource.Source = settings.CustomUrl!;
        }
        Configuration.Save();
        return default;
    }
}