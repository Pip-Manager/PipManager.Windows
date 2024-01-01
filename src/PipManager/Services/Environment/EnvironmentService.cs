using Newtonsoft.Json;
using PipManager.Helpers;
using PipManager.Models.AppConfigModels;
using PipManager.Models.Package;
using PipManager.Models.Pages;
using PipManager.Models.Pypi;
using PipManager.Services.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Wpf.Ui.Controls;
using Path = System.IO.Path;

namespace PipManager.Services.Environment;

public partial class EnvironmentService(IConfigurationService configurationService) : IEnvironmentService
{
    private readonly HttpClient _httpClient = App.GetService<HttpClient>();

    public bool CheckEnvironmentExists(EnvironmentItem environmentItem)
    {
        var environmentItems = configurationService.AppConfig.EnvironmentItems;
        return environmentItems.Any(item => item.PythonPath == environmentItem.PythonPath);
    }

    public (bool, string) CheckEnvironmentAvailable(EnvironmentItem environmentItem)
    {
        var verify = configurationService.GetEnvironmentItemFromCommand(environmentItem.PythonPath!, "-m pip -V");
        return verify != null && environmentItem.PythonPath != string.Empty
            ? (true, "")
            : (false, "Broken Environment");
    }

    public List<PackageItem>? GetLibraries()
    {
        if (configurationService.AppConfig.CurrentEnvironment is null)
        {
            return null;
        }

        var packageLock = new object();
        var packageDirInfo = new DirectoryInfo(Path.Combine(
            Path.GetDirectoryName(configurationService.AppConfig.CurrentEnvironment!.PythonPath)!,
            @"Lib\site-packages"));
        var packages = new List<PackageItem>();
        var ioTaskList = new List<Task>();
        foreach (var distInfoDirectory in packageDirInfo.GetDirectories()
                     .Where(path => path.Name.EndsWith(".dist-info")).ToList())
        {
            var task = Task.Run(() =>
            {
                var distInfoDirectoryFullName = distInfoDirectory.FullName;
                var distInfoDirectoryName = distInfoDirectory.Name;

                // Basic
                var packageBasicInfo = distInfoDirectoryName.Replace(".dist-info", "").Split('-');
                var packageName = packageBasicInfo[0];
                var packageVersion = packageBasicInfo[1];

                if (packageName == "pip") return;
                // Metadata
                var metadataDict = new Dictionary<string, List<string>>();
                var lastValidKey = "";
                var lastValidPos = 0;
                var classifiers = new Dictionary<string, List<string>>();
                foreach (var line in File.ReadLines(Path.Combine(distInfoDirectoryFullName, "METADATA")))
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
                            metadataDict.Add(key, new List<string>());
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
                        metadataDict[lastValidKey][lastValidPos] += '\n' + value;
                        ;
                    }
                }

                foreach (var item in metadataDict.GetValueOrDefault("classifier", new List<string>()))
                {
                    var key = item.Split(" :: ")[0];
                    var value = string.Join(" :: ", item.Split(" :: ")[1..]);
                    if (!classifiers.ContainsKey(key))
                    {
                        classifiers.Add(key, new List<string>());
                    }

                    classifiers[key].Add(value);
                }

                // Record
                var record = new List<string>();
                var actualPath = "";
                foreach (var line in File.ReadLines(Path.Combine(distInfoDirectoryFullName, "RECORD")))
                {
                    var dirIdentifier = line.Split('/')[0];
                    if (dirIdentifier == distInfoDirectoryName || dirIdentifier[0] == '.') continue;
                    record.Add(line);
                    if (actualPath == "")
                    {
                        actualPath = Path.Combine(packageDirInfo.FullName, dirIdentifier);
                    }
                }

                // Extra
                var projectUrl = metadataDict.GetValueOrDefault("project-url", new List<string>());
                var projectUrlDictionary = new List<LibraryDetailProjectUrlModel>();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (projectUrl.Count != 0)
                    {
                        projectUrlDictionary.AddRange(projectUrl.Select(url => new LibraryDetailProjectUrlModel
                        {
                            Icon = url.Split(", ")[0].ToLower() switch
                            {
                                "homepage" or "home" => new SymbolIcon(SymbolRegular.Home24),
                                "download" => new SymbolIcon(SymbolRegular.ArrowDownload24),
                                "changelog" or "changes" or "release notes" => new SymbolIcon(SymbolRegular
                                    .ClipboardTextEdit24),
                                "bug tracker" or "issue tracker" or "bug reports" or "issues" or "tracker" =>
                                    new SymbolIcon(SymbolRegular.Bug24),
                                "source code" or "source" or "repository" or "code" => new SymbolIcon(SymbolRegular
                                    .Code24),
                                "funding" or "donate" or "donations" => new SymbolIcon(SymbolRegular.Money24),
                                "documentation" => new SymbolIcon(SymbolRegular.Document24),
                                "commercial" => new SymbolIcon(SymbolRegular.PeopleMoney24),
                                "support" => new SymbolIcon(SymbolRegular.PersonSupport24),
                                "chat" or "q & a" => new SymbolIcon(SymbolRegular.ChatHelp24),
                                _ => new SymbolIcon(SymbolRegular.Link24)
                            },
                            UrlType = url.Split(", ")[0],
                            Url = url.Split(", ")[1]
                        }));
                    }
                    else
                    {
                        projectUrlDictionary.Add(new LibraryDetailProjectUrlModel
                        {
                            Icon = new SymbolIcon(SymbolRegular.Question24),
                            UrlType = "Unknown",
                            Url = ""
                        });
                    }
                });

                lock (packageLock)
                {
                    packages.Add(new PackageItem
                    {
                        Name = packageName,
                        Version = packageVersion,
                        DetailedVersion = PackageValidator.CheckVersion(packageVersion),
                        Path = actualPath,
                        DistInfoPath = distInfoDirectoryFullName,
                        Summary = metadataDict.GetValueOrDefault("summary", new List<string> { "" })[0],
                        Author = metadataDict.GetValueOrDefault("author", new List<string>()),
                        AuthorEmail = metadataDict.GetValueOrDefault("author-email", new List<string> { "" })[0],
                        ProjectUrl = projectUrlDictionary,
                        Classifier = classifiers,
                        Metadata = metadataDict,
                        Record = record
                    });
                }
            });
            ioTaskList.Add(task);
        }

        Task.WaitAll(ioTaskList.ToArray());
        return packages.OrderBy(x => x.Name).ToList();
    }

    public async Task<string[]?> GetVersions(string packageName)
    {
        try
        {
            var responseMessage =
                await _httpClient.GetAsync($"{configurationService.GetUrlFromPackageSourceType("pypi")}{packageName}/json");
            var response = await responseMessage.Content.ReadAsStringAsync();

            var pypiPackageInfo = JsonConvert.DeserializeObject<PypiPackageInfo>(response)
                ?.Releases
                .Where(item => item.Value.Count != 0).OrderBy(e => e.Value[0].UploadTime)
                .ThenBy(e => e.Value[0].UploadTime).ToDictionary(pair => pair.Key, pair => pair.Value);
            return pypiPackageInfo?.Keys.ToArray();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public (bool, string) Install(string packageName)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = configurationService.AppConfig!.CurrentEnvironment!.PythonPath,
                Arguments =
                    $"-m pip install \"{packageName}\" -i {configurationService.GetUrlFromPackageSourceType()} --retries 1 --timeout 6",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        process.Close();
        process.Dispose();
        return (string.IsNullOrEmpty(error), error);
    }

    public (bool, string) Update(string packageName)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = configurationService.AppConfig!.CurrentEnvironment!.PythonPath,
                Arguments =
                    $"-m pip install --upgrade \"{packageName}\" -i {configurationService.GetUrlFromPackageSourceType()} --retries 1 --timeout 6",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        process.Close();
        process.Dispose();
        return (string.IsNullOrEmpty(error), error);
    }

    public (bool, string) Uninstall(string packageName)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = configurationService.AppConfig!.CurrentEnvironment!.PythonPath,
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