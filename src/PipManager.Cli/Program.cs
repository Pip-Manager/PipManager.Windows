using PipManager.Cli.Commands;
using PipManager.Cli.Commands.Environment;
using PipManager.Core.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PipManager.Cli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (!File.Exists(Configuration.ConfigPath))
        {
            AnsiConsole.MarkupLine($"[Orange1]It seems to be using PipManager CLI for the first time, the settings file has been created ({Configuration.ConfigPath})[/]");
        }

        if (!Configuration.Initialize())
        {
            Console.Write("config.json is broken, reset? [y/n]: ");
            var result = Console.ReadLine()?.ToLower();
            if (result == "y")
            {
                Configuration.Reset();
            }
            else
            {
                return 0;
            }
        }
        
        if(args.Length > 0 && args[0] != "env")
        {
            ExecutePipCommand.Start(args);
            return 0;
        }
        
        var app = new CommandApp();
        app.Configure(config =>
        {
            config.SetApplicationName("pipm");
            
            config.AddBranch<EnvSettings>("env", env =>
            {
                env.AddCommand<EnvironmentAddCommand>("add");
                env.AddCommand<EnvironmentListCommand>("list");
                env.AddCommand<EnvironmentRemoveCommand>("remove");
                env.AddCommand<EnvironmentInfoCommand>("info");
                env.AddCommand<EnvironmentSwitchCommand>("switch");
                env.AddCommand<EnvironmentSourceCommand>("source");
            });

            config.SetExceptionHandler((ex, _) =>
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.NoStackTrace | ExceptionFormats.ShortenTypes);
                return 1;
            });
        });
        return await app.RunAsync(args);
    }
}