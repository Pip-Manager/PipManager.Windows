namespace PipManager.Core.Extensions;

public static class PackageSourceExtension
{
    public static string GetPackageSourceTestingUrl(this string packageSourceType)
        => packageSourceType switch
        {
            "official" => "https://files.pythonhosted.org/packages/62/35/0230421b8c4efad6624518028163329ad0c2df9e58e6b3bee013427bf8f6/requests-0.10.0.tar.gz",
            "tsinghua" => "https://pypi.tuna.tsinghua.edu.cn/packages/62/35/0230421b8c4efad6624518028163329ad0c2df9e58e6b3bee013427bf8f6/requests-0.10.0.tar.gz",
            "aliyun" => "https://mirrors.aliyun.com/pypi/packages/62/35/0230421b8c4efad6624518028163329ad0c2df9e58e6b3bee013427bf8f6/requests-0.10.0.tar.gz",
            "douban" => "https://mirrors.cloud.tencent.com/pypi/packages/62/35/0230421b8c4efad6624518028163329ad0c2df9e58e6b3bee013427bf8f6/requests-0.10.0.tar.gz",
            _ => throw new ArgumentOutOfRangeException(nameof(packageSourceType), packageSourceType, null)
        };
    
    public static string GetPackageSourceUrl(this string packageSourceType, string index = "simple")
        => packageSourceType switch
        {
            "official" => $"https://pypi.org/{index}/",
            "tsinghua" => $"https://pypi.tuna.tsinghua.edu.cn/{index}/",
            "aliyun" => $"https://mirrors.aliyun.com/pypi/{index switch
            {
                "pypi" => "web/pypi/",
                _ => index
            }}/",
            "douban" => $"https://pypi.doubanio.com/{index}/",
            _ => throw new ArgumentOutOfRangeException(nameof(packageSourceType), packageSourceType, null)
        };
}