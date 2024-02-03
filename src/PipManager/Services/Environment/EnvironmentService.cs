using Newtonsoft.Json;
using PipManager.Helpers;
using PipManager.Models;
using PipManager.Models.AppConfigModels;
using PipManager.Models.Package;
using PipManager.Models.Pages;
using PipManager.Models.Pypi;
using PipManager.Services.Configuration;
using PipManager.Services.Environment.Response;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
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

    public ActionResponse CheckEnvironmentAvailable(EnvironmentItem environmentItem)
    {
        var verify = configurationService.GetEnvironmentItemFromCommand(environmentItem.PythonPath!, "-m pip -V");
        return verify != null && environmentItem.PythonPath != string.Empty
            ? new ActionResponse { Success = true }
            : new ActionResponse { Success = false, Exception = ExceptionType.Environment_Broken };
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
                            metadataDict.Add(key, []);
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

                foreach (var item in metadataDict.GetValueOrDefault("classifier", []))
                {
                    var keyValues = item.Split(" :: ");

                    if (keyValues.Length >= 2)
                    {
                        var key = keyValues[0];
                        var value = string.Join(" :: ", keyValues[1..]);

                        if (!classifiers.TryGetValue(key, out var existingList))
                        {
                            existingList = [];
                            classifiers.Add(key, existingList);
                        }

                        existingList.Add(value);
                    }
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
                var projectUrl = metadataDict.GetValueOrDefault("project-url", []);
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
                        Summary = metadataDict.GetValueOrDefault("summary", [""])[0],
                        Author = metadataDict.GetValueOrDefault("author", []),
                        AuthorEmail = metadataDict.GetValueOrDefault("author-email", [""])[0],
                        ProjectUrl = projectUrlDictionary,
                        Classifier = classifiers,
                        Metadata = metadataDict,
                        Record = record
                    });
                }
            });
            ioTaskList.Add(task);
        }

        Task.WaitAll([.. ioTaskList]);
        return [.. packages.OrderBy(x => x.Name)];
    }

    public async Task<GetVersionsResponse> GetVersions(string packageName)
    {
        try
        {
            packageName = PackageNameFilterRegex().Replace(packageName, "");
            packageName = PackageNameNormalizerRegex().Replace(packageName, "-").ToLower();
            if (!PackageNameVerificationRegex().IsMatch(packageName))
                return new GetVersionsResponse { Status = 2, Versions = [] };
            var sth = $"{configurationService.GetUrlFromPackageSourceType("pypi")}{packageName}/json";
            var responseMessage =
                await _httpClient.GetAsync($"{configurationService.GetUrlFromPackageSourceType("pypi")}{packageName}/json");
            var response = await responseMessage.Content.ReadAsStringAsync();

            var pypiPackageInfo = JsonConvert.DeserializeObject<PypiPackageInfo>(response)
                ?.Releases?
                .Where(item => item.Value.Count != 0).OrderBy(e => e.Value[0].UploadTime)
                .ThenBy(e => e.Value[0].UploadTime).ToDictionary(pair => pair.Key, pair => pair.Value);
            return new GetVersionsResponse { Status = 0, Versions = pypiPackageInfo?.Keys.ToArray() };
        }
        catch (Exception)
        {
            return new GetVersionsResponse { Status = 1, Versions = [] };
        }
    }

    public ActionResponse Install(string packageName, DataReceivedEventHandler consoleOutputCallback)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = configurationService.AppConfig!.CurrentEnvironment!.PythonPath,
                Arguments =
                    $"-m pip install \"{packageName}\" -i {configurationService.GetUrlFromPackageSourceType()} --retries 1 --timeout 6",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        process.OutputDataReceived += consoleOutputCallback;
        process.Start();
        process.BeginOutputReadLine();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        process.Close();
        process.Dispose();
        return new ActionResponse { Success = string.IsNullOrEmpty(error), Exception = ExceptionType.Process_Error, Message = error };
    }

    public ActionResponse InstallByRequirements(string requirementsFilePath, DataReceivedEventHandler consoleOutputCallback)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = configurationService.AppConfig!.CurrentEnvironment!.PythonPath,
                Arguments =
                    $"-m pip install -r \"{requirementsFilePath}\" -i {configurationService.GetUrlFromPackageSourceType()} --retries 1 --timeout 6",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        process.OutputDataReceived += consoleOutputCallback;
        process.Start();
        process.BeginOutputReadLine();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        process.Close();
        process.Dispose();
        return new ActionResponse { Success = string.IsNullOrEmpty(error), Exception = ExceptionType.Process_Error, Message = error };
    }

    public ActionResponse Update(string packageName, DataReceivedEventHandler consoleOutputCallback)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = configurationService.AppConfig!.CurrentEnvironment!.PythonPath,
                Arguments =
                    $"-m pip install --upgrade \"{packageName}\" -i {configurationService.GetUrlFromPackageSourceType()} --retries 1 --timeout 6",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        process.OutputDataReceived += consoleOutputCallback;
        process.Start();
        process.BeginOutputReadLine();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        process.Close();
        process.Dispose();
        return new ActionResponse { Success = string.IsNullOrEmpty(error), Exception = ExceptionType.Process_Error, Message = error };
    }

    public ActionResponse Uninstall(string packageName, DataReceivedEventHandler consoleOutputCallback)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = configurationService.AppConfig!.CurrentEnvironment!.PythonPath,
                Arguments = $"-m pip uninstall -y \"{packageName}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        process.OutputDataReceived += consoleOutputCallback;
        process.Start();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        process.Close();
        return new ActionResponse { Success = string.IsNullOrEmpty(error), Exception = ExceptionType.Process_Error, Message = error };
    }

    // Package Version Validation

    [GeneratedRegex("[-_.]+", RegexOptions.IgnoreCase)]
    private static partial Regex PackageNameNormalizerRegex();

    [GeneratedRegex("\\[[^\\]]*\\]", RegexOptions.IgnoreCase)]
    private static partial Regex PackageNameFilterRegex();

    [GeneratedRegex("^([A-Z0-9]|[A-Z0-9][A-Z0-9._-]*[A-Z0-9])$", RegexOptions.IgnoreCase)]
    private static partial Regex PackageNameVerificationRegex();
}