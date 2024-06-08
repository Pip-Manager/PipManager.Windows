using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using PipManager.Windows.Models;
using PipManager.Windows.Models.AppConfigModels;
using PipManager.Windows.Models.Package;

namespace PipManager.Windows.Services.Configuration;

public partial class ConfigurationService : IConfigurationService
{
    public AppConfig AppConfig { get; set; } = LoadConfiguration();
    public bool ExperimentMode { get; set; }
    public bool DebugMode { get; set; }

    public static AppConfig LoadConfiguration()
    {
        if (!File.Exists(AppInfo.ConfigPath))
        {
            File.WriteAllText(AppInfo.ConfigPath, JsonConvert.SerializeObject(new AppConfig(), Formatting.Indented));
        }
        return JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(AppInfo.ConfigPath))!;
    }

    public void Save()
    {
        File.WriteAllText(AppInfo.ConfigPath, JsonConvert.SerializeObject(AppConfig, Formatting.Indented));
    }

    #region Environments

    public string FindPythonPathByPipDir(string pipDir)
    {
        // Need more information
        var pipExePath = Path.Combine(new DirectoryInfo(pipDir).Parent!.Parent!.Parent!.FullName,
            "python.exe");
        var pipExePathAttempt1 = Path.Combine(new DirectoryInfo(pipDir).Parent!.Parent!.FullName,
            "python.exe");
        if (!File.Exists(pipExePath))
        {
            pipExePath = pipExePathAttempt1;
        }

        return pipExePath;
    }

    [GeneratedRegex("__version__ = \"(.*?)\"", RegexOptions.IgnoreCase, "zh-CN")]
    private static partial Regex GetPipVersionInInitFile();

    /// <summary>
    /// Get the pip directory
    /// </summary>
    /// <param name="pythonDirectory">Python path</param>
    /// <returns>Pip directory if exists, otherwise null</returns>
    private string? GetPipDirectories(string pythonDirectory)
    {
        var sitePackageDirectory = Path.Combine(pythonDirectory, @"Lib\site-packages");
        if (!Directory.Exists(sitePackageDirectory))
        {
            return null;
        }

        var pipDirectory = Path.Combine(sitePackageDirectory, "pip");
        if (!Directory.Exists(pipDirectory))
        {
            return null;
        }

        return pipDirectory;
    }

    public EnvironmentItem? GetEnvironmentItem(string pythonPath)
    {
        var pythonVersion = FileVersionInfo.GetVersionInfo(pythonPath).FileVersion!;
        var pythonDirectory = Directory.GetParent(pythonPath)!;
        var pipDirectory = GetPipDirectories(pythonDirectory.FullName);
        var pythonDllName = $"{pythonDirectory.Name.ToLower().Replace(".", "")}.dll";
        var pythonDllPath = pythonDirectory.GetFiles(pythonDllName, SearchOption.AllDirectories).FirstOrDefault();
        if (pythonDllPath == null || pipDirectory == null)
        {
            return null;
        }

        var pipVersion = GetPipVersionInInitFile().Match(File.ReadAllText(Path.Combine(pipDirectory, @"__init__.py"))).Groups[1].Value;
        return new EnvironmentItem(pipVersion, pythonPath, pythonVersion, pythonDllPath.FullName);
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
        var pythonPath = FindPythonPathByPipDir(pipDir.Trim());
        var pythonDirectory = Directory.GetParent(pythonPath)!;
        var pythonDllName = $"{pythonDirectory.Name.ToLower().Replace(".", "")}.dll";
        var pythonDllPath = pythonDirectory.GetFiles(pythonDllName, SearchOption.AllDirectories).FirstOrDefault();
        pythonVersion = pythonVersion.Trim();
        proc.Close();
        return pipDir.Length > 0 && pythonDllPath != null ? new EnvironmentItem(pipVersion, pythonPath, pythonVersion, pythonDllPath.FullName) : null;
    }

    public void RefreshAllEnvironmentVersions()
    {
        foreach (var item in AppConfig.EnvironmentItems)
        {
            var environmentItem = GetEnvironmentItem(item.PythonPath!);
            if (environmentItem != null)
            {
                item.PipVersion = environmentItem.PipVersion;
            }
        }

        Save();
    }

    #endregion Environments

    #region Settings - Package Source

    public string GetUrlFromPackageSourceType(string index = "simple")
    {
        return AppConfig.PackageSource.PackageSourceType switch
        {
            PackageSourceType.Official => $"https://pypi.org/{index}/",
            PackageSourceType.Tsinghua => $"https://pypi.tuna.tsinghua.edu.cn/{index}/",
            PackageSourceType.Aliyun => $"https://mirrors.aliyun.com/pypi/{index switch
            {
                "pypi" => "web/pypi/",
                _ => index
            }}/",
            PackageSourceType.Douban => $"https://pypi.doubanio.com/{index}/",
            _ => throw new ArgumentOutOfRangeException(nameof(AppConfig.PackageSource.PackageSourceType), AppConfig.PackageSource.PackageSourceType, null)
        };
    }

    public string GetTestingUrlFromPackageSourceType(PackageSourceType packageSourceType)
    {
        return packageSourceType switch
        {
            PackageSourceType.Official => "https://files.pythonhosted.org/packages/62/35/0230421b8c4efad6624518028163329ad0c2df9e58e6b3bee013427bf8f6/requests-0.10.0.tar.gz",
            PackageSourceType.Tsinghua => "https://pypi.tuna.tsinghua.edu.cn/packages/62/35/0230421b8c4efad6624518028163329ad0c2df9e58e6b3bee013427bf8f6/requests-0.10.0.tar.gz",
            PackageSourceType.Aliyun => "https://mirrors.aliyun.com/pypi/packages/62/35/0230421b8c4efad6624518028163329ad0c2df9e58e6b3bee013427bf8f6/requests-0.10.0.tar.gz",
            PackageSourceType.Douban => "https://mirrors.cloud.tencent.com/pypi/packages/62/35/0230421b8c4efad6624518028163329ad0c2df9e58e6b3bee013427bf8f6/requests-0.10.0.tar.gz",
            _ => throw new ArgumentOutOfRangeException(nameof(packageSourceType), packageSourceType, null)
        };
    }

    #endregion Settings - Package Source
}