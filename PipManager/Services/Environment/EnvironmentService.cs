using PipManager.Models;
using PipManager.Models.AppConfigModels;
using System.Diagnostics;
using System.IO;

namespace PipManager.Services.Environment;

public class EnvironmentService : IEnvironmentService
{
    public AppConfig AppConfig { get; set; }

    public void Initialize(AppConfig appConfig)
    {
        AppConfig = appConfig;
    }

    public bool CheckEnvironmentExists(EnvironmentItem environmentItem)
    {
        var environmentItems = AppConfig.EnvironmentItems;
        return environmentItems.Any(item => item.PipDir == environmentItem.PipDir);
    }

    public EnvironmentItem? GetEnvironmentItemFromCommand(string command, string arguments)
    {
        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            }
        };
        try
        {
            proc.Start();
        }
        catch
        {
            return null;
        }

        var pipVersion = "";
        var pythonVersion = "";
        var pipDir = "";
        while (!proc.StandardOutput.EndOfStream)
        {
            var output = proc.StandardOutput.ReadLine();
            if (string.IsNullOrWhiteSpace(output)) continue;
            var sections = output.Split(' ');
            var pipDirStart = false;
            for (var i = 0; i < sections.Length; i++)
            {
                if (sections[i] == "from")
                {
                    pipVersion = sections[i - 1];
                    pipDirStart = true;
                }
                else if (sections[i] == "(python")
                {
                    pythonVersion = sections[i + 1].Replace(")", "");
                    break;
                }
                else if (pipDirStart)
                {
                    pipDir += sections[i] + ' ';
                }
            }
        }
        pipVersion = pipVersion.Trim();
        pipDir = pipDir.Trim();
        pythonVersion = pythonVersion.Trim();
        proc.Close();
        return pipDir.Length > 0 ? new EnvironmentItem(pipVersion, pipDir, pythonVersion) : null;
    }

    public (bool, string) CheckEnvironmentAvailable(EnvironmentItem environmentItem)
    {
        // Need more information
        var pipExePath = Path.Combine(new DirectoryInfo(environmentItem.PipDir).Parent.Parent.Parent.FullName,
            "python.exe");
        var pipExePathAttempt1 = Path.Combine(new DirectoryInfo(environmentItem.PipDir).Parent.Parent.FullName,
            "python.exe");
        if (!File.Exists(pipExePath))
        {
            pipExePath = pipExePathAttempt1;
        }
        var verify = GetEnvironmentItemFromCommand(pipExePath, "-m pip -V");
        return verify != null && verify.PipDir != string.Empty ? (true, "") : (false, "Broken Environment");
    }
}