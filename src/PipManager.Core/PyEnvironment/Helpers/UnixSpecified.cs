using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text;
using PipManager.Core.Configuration.Models;

namespace PipManager.Core.PyEnvironment.Helpers;


[SupportedOSPlatform("Linux")]
[SupportedOSPlatform("OSX")]
public class UnixSpecified
{
    public static EnvironmentModel GetEnvironment(string pythonPath)
    {
        var pythonVersion = FileVersionInfo.GetVersionInfo(pythonPath).FileVersion!;
        var pipDirectory = GetPackageDirectory(pythonPath);

        if (pipDirectory == null)
        {
            throw new DirectoryNotFoundException($"Pip directory not found in {pythonPath}");
        }
        pipDirectory = Path.Combine(pipDirectory, "pip");
        var pipVersion = Common.GetPipVersionInInitFile().Match(File.ReadAllText(Path.Combine(pipDirectory, "__init__.py"))).Groups[1].Value;
        return new EnvironmentModel { Identifier = "", PipVersion = pipVersion, PythonPath = pythonPath, PythonVersion = pythonVersion};
    }
    
    private static string GetPackageDirectory(string pythonPath)
    {
        var process = new Process();
        process.StartInfo.FileName = pythonPath;
        process.StartInfo.Arguments = "-m site";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();

        var output = new StringBuilder();
        using (var reader = process.StandardOutput)
        {
            while (reader.ReadLine() is { } line)
            {
                output.AppendLine(line);
                if (line.Contains(']'))
                {
                    break;
                }
            }
        }

        process.WaitForExit();

        return output.ToString().Split('\n')[1..^2].Select(item => item.Trim()[1..^2]).Reverse()
            .FirstOrDefault(path => path.EndsWith("site-packages") || path.EndsWith("dist-packages")) ?? throw new DirectoryNotFoundException($"Package directory not found in {pythonPath}");
    }
}