using System.ComponentModel;
using Newtonsoft.Json;
using PipManager.Models;
using PipManager.Models.AppConfigModels;
using System.Diagnostics;
using System.IO;
using PipManager.Controls;
using PipManager.Languages;
using Serilog;

namespace PipManager.Services.Configuration;

public class ConfigurationService : IConfigurationService
{
    public AppConfig AppConfig { get; set; }

    public ConfigurationService()
    {
        AppConfig = LoadConfiguration();
    }

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