using System.ComponentModel;
using Newtonsoft.Json;
using PipManager.Models;
using PipManager.Models.AppConfigModels;
using System.Diagnostics;
using System.IO;
using PipManager.Controls;
using PipManager.Languages;

namespace PipManager.Services.Configuration;

public class ConfigurationService : IConfigurationService
{
    public AppConfig AppConfig { get; set; }

    public static AppConfig LoadConfiguration()
    {
        if (!File.Exists(AppInfo.ConfigPath))
        {
            File.WriteAllText(AppInfo.ConfigPath, JsonConvert.SerializeObject(new AppConfig(), Formatting.Indented));
        }
        return JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(AppInfo.ConfigPath))!;
    }

    public void Initialize()
    {
        AppConfig = LoadConfiguration();
    }

    public void Save()
    {
        File.WriteAllText(AppInfo.ConfigPath, JsonConvert.SerializeObject(AppConfig, Formatting.Indented));
    }

    #region Environment

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
        try
        {
            var pipExePathAttempt1 = Path.Combine(new DirectoryInfo(environmentItem.PipDir).Parent.Parent.Parent.FullName,
                "python.exe");
            var verify = GetEnvironmentItemFromCommand(pipExePathAttempt1, "-m pip -V");
            if (verify.PipDir != string.Empty)
            {
                return (true, "");
            }
        }
        catch
        {
            var pipExePathAttempt2 = Path.Combine(new DirectoryInfo(environmentItem.PipDir).Parent.Parent.FullName,
                "python.exe");
            var verify = GetEnvironmentItemFromCommand(pipExePathAttempt2, "-m pip -V");
            return verify.PipDir != string.Empty ? (true, "") : (false, "Broken Environment");
        }

        return (false, "Broken Environment");
    }

    #endregion Environment

    #region Settings - Package Source

    public string GetUrlFromPackageSourceType(PackageSourceType packageSourceType)
    {
        return packageSourceType switch
        {
            PackageSourceType.Official => "https://pypi.org/simple",
            PackageSourceType.Tsinghua => "https://pypi.tuna.tsinghua.edu.cn/simple",
            PackageSourceType.Aliyun => "https://mirrors.aliyun.com/pypi/simple/",
            PackageSourceType.Douban => "https://pypi.doubanio.com/simple/",
            _ => throw new ArgumentOutOfRangeException(nameof(packageSourceType), packageSourceType, null)
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