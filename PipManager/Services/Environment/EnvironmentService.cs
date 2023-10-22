using Newtonsoft.Json;
using PipManager.Models.AppConfigModels;
using PipManager.Models.PipInspection;
using PipManager.Services.Configuration;
using System.Diagnostics;
using System.IO;

namespace PipManager.Services.Environment;

public class EnvironmentService : IEnvironmentService
{
    private readonly IConfigurationService _configurationService;

    public EnvironmentService(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public bool CheckEnvironmentExists(EnvironmentItem environmentItem)
    {
        var environmentItems = _configurationService.AppConfig.EnvironmentItems;
        return environmentItems.Any(item => item.PythonPath == environmentItem.PythonPath);
    }

    

    public (bool, string) CheckEnvironmentAvailable(EnvironmentItem environmentItem)
    {
        var verify = _configurationService.GetEnvironmentItemFromCommand(environmentItem.PythonPath!, "-m pip -V");
        return verify != null && environmentItem.PythonPath != string.Empty ? (true, "") : (false, "Broken Environment");
    }

    public List<PipMetadata>? GetLibraries()
    {
        if (_configurationService.AppConfig.CurrentEnvironment is null)
        {
            return null;
        }
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _configurationService.AppConfig.CurrentEnvironment.PythonPath,
                Arguments = "-m pip inspect",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        var inspection = "";
        process.StartInfo.EnvironmentVariables.Add("PYTHONIOENCODING", "utf-8");
        process.OutputDataReceived += delegate (object _, DataReceivedEventArgs e)
        {
            inspection += e.Data;
        };
        process.Start();
        process.BeginOutputReadLine();
        process.WaitForExit();
        process.Close();
        return JsonConvert.DeserializeObject<PipInspection>(inspection)?.Installed;
    }

    public string[] GetVersions(string packageName)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _configurationService.AppConfig.CurrentEnvironment.PythonPath,
                Arguments = $"-m pip install \"{packageName}\"==random",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        var output = process.StandardError.ReadToEnd();
        process.WaitForExit();
        process.Close();
        return output.Split("ERROR: No m")[0].Split("from versions: ")[1].Replace(")", "").Split(", ");
    }

    public (bool, string) Update(string packageName)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _configurationService.AppConfig.CurrentEnvironment.PythonPath,
                Arguments = $"-m pip install --upgrade \"{packageName}\"",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        var output = process.StandardError.ReadToEnd();
        process.WaitForExit();
        process.Close();
        return string.IsNullOrEmpty(output) ? (true, "") : (false, output);
    }

    public (bool, string) Uninstall(string packageName)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _configurationService.AppConfig.CurrentEnvironment.PythonPath,
                Arguments = $"-m pip uninstall -y \"{packageName}\"",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        var output = process.StandardError.ReadToEnd();
        process.WaitForExit();
        process.Close();
        return string.IsNullOrEmpty(output) ? (true, "") : (false, output);
    }
}