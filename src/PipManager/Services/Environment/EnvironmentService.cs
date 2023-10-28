using Newtonsoft.Json;
using PipManager.Models;
using PipManager.Models.AppConfigModels;
using PipManager.Services.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

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

    public List<PackageItem>? GetLibraries()
    {
        if (_configurationService.AppConfig.CurrentEnvironment is null)
        {
            return null;
        }
        var packageLock = new object();
        var packageDirInfo = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(_configurationService.AppConfig.CurrentEnvironment!.PythonPath)!, @"Lib\site-packages"));
        var packages = new List<PackageItem>();
        var ioTaskList = new List<Task>();
        foreach (var dir in packageDirInfo.GetDirectories().Where(path => path.Name.EndsWith(".dist-info")).ToList())
        {
            var task = Task.Run(() =>
            {
                var dirInfo = dir.Name.Replace(".dist-info", "").Split('-');
                var dirPath = dir.FullName;
                var packageName = dirInfo[0];
                var packageVersion = dirInfo[1];
                var metadataDict = new Dictionary<string, List<string>>();
                var lastValidKey = "";
                var lastValidPos = 0;
                foreach (var line in File.ReadLines(Path.Combine(dirPath, "METADATA")))
                {
                    if (line == "")
                    {
                        break;
                    }
                    var key = line.Split(": ")[0];
                    var value = line.Replace(key + ": ", "");
                    if (!string.IsNullOrWhiteSpace(key) && !key.StartsWith(' '))
                    {
                        key = key.ToLower();
                        if (!metadataDict.ContainsKey(key))
                        {
                            metadataDict.Add(key, new());
                            lastValidKey = key;
                            lastValidPos = 0;
                        }
                        else
                        {
                            lastValidPos++;
                        }
                        metadataDict[key].Add(value);
                    }
                    else
                    {
                        metadataDict[lastValidKey][lastValidPos] += '\n' + value; ;
                    }
                }
                
                lock (packageLock)
                {
                    packages.Add(new PackageItem
                    {
                        Name = packageName,
                        Version = packageVersion,
                        Path = dirPath,
                        Summary = metadataDict.GetValueOrDefault("summary", new List<string>{""})[0],
                        Classifier = metadataDict.GetValueOrDefault("classifier", new List<string> ()),
                        Metadata = metadataDict
                    });
                }
            });
            ioTaskList.Add(task);
        }
        Task.WaitAll(ioTaskList.ToArray());
        return packages.OrderBy(x => x.Name).ToList();
    }

    public string[] GetVersions(string packageName)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _configurationService.AppConfig!.CurrentEnvironment!.PythonPath,
                Arguments = $"-m pip install \"{packageName}\"==random -i {_configurationService.GetUrlFromPackageSourceType()}",
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
                FileName = _configurationService.AppConfig!.CurrentEnvironment!.PythonPath,
                Arguments = $"-m pip install --upgrade \"{packageName}\" -i {_configurationService.GetUrlFromPackageSourceType()}",
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
                FileName = _configurationService.AppConfig!.CurrentEnvironment!.PythonPath,
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