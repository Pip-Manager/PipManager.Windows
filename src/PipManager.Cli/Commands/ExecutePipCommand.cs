using System.Diagnostics;
using PipManager.Core.Configuration;
using Spectre.Console;

namespace PipManager.Cli.Commands;


public class ExecutePipCommand
{
    private static string[] ReplacePackageSource(string[] args)
    {
        var shortSourceIndexArgPosition = Array.IndexOf(args, "-i");
        var longSourceIndexArgPosition = Array.IndexOf(args, "--index-url");
        var sourceIndexArgPosition = shortSourceIndexArgPosition != -1 ? shortSourceIndexArgPosition : longSourceIndexArgPosition;
        if (sourceIndexArgPosition != -1)
        {
            AnsiConsole.MarkupLine("[orange1]The source index has been specified, the source index in the configuration file will be ignored.[/]");
            return args;
        }
        var packageSource = Configuration.AppConfig.PackageSource.Source;
        if (packageSource == "official")
        {
            return args;
        }
        args = Configuration.PackageSources.TryGetValue(packageSource, out var source) 
            ? args.Append("-i").Append(source).ToArray() 
            : args.Append("-i").Append(packageSource).ToArray();
        AnsiConsole.MarkupLine($"[aqua]The mirror index has been applied ({packageSource}).[/]");
        return args;
    }
    
    public static void Start(string[] args)
    {
        if(Configuration.AppConfig.Environments.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Python environment has not been added yet, add it with the 'env add' command.[/]");
            return;
        }
        if(Configuration.AppConfig.SelectedEnvironment == null)
        {
            AnsiConsole.MarkupLine("[red]No environment selected, select it with the 'env select' command.[/]");
            return;
        }

        Configuration.UpdateSelectedEnvironment();
        args = ReplacePackageSource(args);

        AnsiConsole.MarkupLine($"[green]Running under Pip {Configuration.AppConfig.SelectedEnvironment!.PipVersion} (Python {Configuration.AppConfig.SelectedEnvironment.PythonVersion})[/]");

        var process = new Process();
        process.StartInfo.FileName = Configuration.AppConfig.SelectedEnvironment.PythonPath;
        process.StartInfo.Arguments = string.Join(" ", ["-m", "pip", ..args]);
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
            {
                AnsiConsole.WriteLine(e.Data);
            }
        };
        
        process.ErrorDataReceived += (_, e) => 
        {
            if (e.Data != null)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]{e.Data}[/]");
            }
        };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Exception: {ex.Message}");
        }
    }
}