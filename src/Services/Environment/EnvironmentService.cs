using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using PipManager.Core.Configuration.Models;
using PipManager.Core.PyPackage.Helpers;
using PipManager.Windows.Models;
using PipManager.Windows.Models.Package;
using PipManager.Windows.Models.Pages;
using PipManager.Windows.Models.Pypi;
using PipManager.Windows.Services.Configuration;
using PipManager.Windows.Services.Environment.Response;
using Serilog;
using Wpf.Ui.Controls;
using Path = System.IO.Path;

namespace PipManager.Windows.Services.Environment;

public partial class EnvironmentService(IConfigurationService configurationService) : IEnvironmentService
{
    private readonly HttpClient _httpClient = App.GetService<HttpClient>();

    public bool CheckEnvironmentExists(EnvironmentModel environmentModel)
    {
        var environmentItems = configurationService.AppConfig.Environments;
        return environmentItems.Any(item => item.PythonPath == environmentModel.PythonPath);
    }

    public ActionResponse CheckEnvironmentAvailable(EnvironmentModel environmentModel)
    {
        var verify = configurationService.GetEnvironmentItemFromCommand(environmentModel.PythonPath, "-m pip -V");
        return verify != null && environmentModel.PythonPath != string.Empty
            ? new ActionResponse { Success = true }
            : new ActionResponse { Success = false, Exception = ExceptionType.EnvironmentBroken };
    }

    public ActionResponse PurgeEnvironmentCache(EnvironmentModel environmentModel)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = configurationService.AppConfig.SelectedEnvironment!.PythonPath,
                Arguments = "-m pip cache purge",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        process.Close();
        process.Dispose();
        error = error.Replace("WARNING: No matching packages", "").Trim();
        return !string.IsNullOrEmpty(error) ? new ActionResponse { Success = false, Exception = ExceptionType.ProcessError, Message = error } : new ActionResponse { Success = true, Message = output[15..].TrimEnd()};
    }

    public async Task<List<PackageItem>?> GetLibraries()
    {
        if (configurationService.AppConfig.SelectedEnvironment is null)
        {
            return null;
        }

        var packageDirInfo = new DirectoryInfo(Path.Combine(
            Path.GetDirectoryName(configurationService.AppConfig.SelectedEnvironment!.PythonPath)!,
            @"Lib\site-packages"));
        
        var packages = new ConcurrentBag<PackageItem>();
        var ioTaskList = new List<Task>();
        var distInfoDirectories = packageDirInfo.GetDirectories()
                     .Where(path => path.Name.EndsWith(".dist-info"))
                     .AsParallel()
                     .ToList();

        foreach (var distInfoDirectory in distInfoDirectories)
        {
            var task = Task.Run(async () =>
            {
                var distInfoDirectoryFullName = distInfoDirectory.FullName;
                var distInfoDirectoryName = distInfoDirectory.Name;

                // Basic
                var packageBasicInfo = distInfoDirectoryName[..^10].Split('-');
                var packageName = packageBasicInfo[0];
                var packageVersion = packageBasicInfo[1];

                if (packageName == "pip") return;

                // Metadata
                var metadataDict = new Dictionary<string, List<StringBuilder>>();
                var lastValidKey = "";
                var lastValidIndex = 0;
                var classifiers = new Dictionary<string, List<string>>();
                await foreach (var line in File.ReadLinesAsync(Path.Combine(distInfoDirectoryFullName, "METADATA")))
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
                            lastValidIndex = 0;
                        }
                        else
                        {
                            lastValidIndex++;
                        }

                        metadataDict[key].Add(new StringBuilder(value));
                    }
                    else
                    {
                        metadataDict[lastValidKey][lastValidIndex].Append('\n').Append(value);
                    }
                }
                var metadata = metadataDict.ToDictionary(
                    pair => pair.Key, 
                    pair => pair.Value.Select(sb => sb.ToString()).ToList()
                );
                foreach (var item in metadata.GetValueOrDefault("classifier", []))
                {
                    var keyValues = item.Split(" :: ");

                    if (keyValues.Length < 2)
                    {
                        continue;
                    }

                    var key = keyValues[0];
                    var value = string.Join(" :: ", keyValues[1..]);

                    if (!classifiers.TryGetValue(key, out var existingList))
                    {
                        existingList = [];
                        classifiers.Add(key, existingList);
                    }

                    existingList.Add(value);
                }

                // Record
                var record = new List<string>();
                var actualPath = "";
                await foreach (var line in File.ReadLinesAsync(Path.Combine(distInfoDirectoryFullName, "RECORD")))
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
                var projectUrl = metadata.GetValueOrDefault("project-url", []);
                var projectUrlDictionary = new List<LibraryDetailProjectUrlModel>();

                if (projectUrl.Count != 0)
                {
                    projectUrlDictionary.AddRange(projectUrl.Select(url => new LibraryDetailProjectUrlModel
                    {
                        Icon = url.Split(", ")[0].ToLower() switch
                        {
                            "homepage" or "home" => SymbolRegular.Home24,
                            "download" => SymbolRegular.ArrowDownload24,
                            "changelog" or "changes" or "release notes" => SymbolRegular
                                .ClipboardTextEdit24,
                            "bug tracker" or "issue tracker" or "bug reports" or "issues" or "tracker" =>
                                SymbolRegular.Bug24,
                            "source code" or "source" or "repository" or "code" => SymbolRegular
                                .Code24,
                            "funding" or "donate" or "donations" => SymbolRegular.Money24,
                            "documentation" => SymbolRegular.Document24,
                            "commercial" => SymbolRegular.PeopleMoney24,
                            "support" => SymbolRegular.PersonSupport24,
                            "chat" or "q & a" => SymbolRegular.ChatHelp24,
                            _ => SymbolRegular.Link24
                        },
                        UrlType = url.Split(", ")[0],
                        Url = url.Split(", ")[1]
                    }));
                }
                else
                {
                    projectUrlDictionary.Add(new LibraryDetailProjectUrlModel
                    {
                        Icon = SymbolRegular.Question24,
                        UrlType = "Unknown",
                        Url = ""
                    });
                }

                
                packages.Add(new PackageItem
                {
                    Name = packageName,
                    Version = packageVersion,
                    DetailedVersion = PackageValidator.CheckVersion(packageVersion),
                    Path = actualPath,
                    DistInfoPath = distInfoDirectoryFullName,
                    Summary = metadata.GetValueOrDefault("summary", [""])[0],
                    Author = metadata.GetValueOrDefault("author", []),
                    AuthorEmail = metadata.GetValueOrDefault("author-email", [""])[0],
                    ProjectUrl = projectUrlDictionary,
                    Classifier = classifiers,
                    Metadata = metadata,
                    Record = record
                });
            });
            ioTaskList.Add(task);
        }
        await Task.WhenAll([.. ioTaskList]);
        Log.Information($"[EnvironmentService] Found {packages.Count} packages");
        return [.. packages.OrderBy(x => x.Name)];
    }

    public async Task<GetVersionsResponse> GetVersions(string packageName, CancellationToken cancellationToken, bool detectNonRelease = true)
    {
        try
        {
            packageName = PackageNameFilterRegex().Replace(packageName, "");
            packageName = PackageNameNormalizerRegex().Replace(packageName, "-").ToLower();
            if (!PackageNameVerificationRegex().IsMatch(packageName))
                return new GetVersionsResponse { Status = 2, Versions = [] };
            var responseMessage =
                await _httpClient.GetAsync(
                    $"{configurationService.GetUrlFromPackageSourceType("pypi")}{packageName}/json", cancellationToken);
            var response = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

            var pypiPackageInfo = JsonConvert.DeserializeObject<PypiPackageInfo>(response)
                ?.Releases?
                .Where(item => item.Value.Count != 0).OrderBy(e => e.Value[0].UploadTime)
                .ThenBy(e => e.Value[0].UploadTime).ToDictionary(pair => pair.Key, pair => pair.Value);

            if (!detectNonRelease && pypiPackageInfo != null)
            {
                pypiPackageInfo = pypiPackageInfo.Where(item => PackageValidator.IsReleaseVersion(item.Key)).ToDictionary();
            }
            
            if (pypiPackageInfo == null || pypiPackageInfo.Count == 0)
            {
                Log.Warning($"[EnvironmentService] {packageName} package not found");
                return new GetVersionsResponse { Status = 1, Versions = [] };
            }

            Log.Information($"[EnvironmentService] Found {packageName}");
            return new GetVersionsResponse { Status = 0, Versions = pypiPackageInfo.Keys.ToArray() };
        }
        catch (TaskCanceledException)
        {
            return new GetVersionsResponse { Status = 1, Versions = [] };
        }
        catch (Exception)
        {
            Log.Warning($"[EnvironmentService] Unexpected error when get versions of {packageName} package");
            return new GetVersionsResponse { Status = 1, Versions = [] };
        }
    }
    
    private Process? BasicCommandProcess { get; set; }
    
    public bool TryKillProcess()
    {
        if (BasicCommandProcess is null) return false;
        try
        {
            BasicCommandProcess.Kill();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static string ProcessErrorFilter(string error)
    {
        var errorBuilder = new StringBuilder();
        foreach (var errorLine in error.Split('\n'))
        {
            if(!string.IsNullOrWhiteSpace(error) && !errorLine.Contains("[notice]"))
                errorBuilder.Append(errorLine).Append('\n');
        }
        return errorBuilder.ToString().Trim();
    }
    
    private ActionResponse RaiseProcess(string arguments, DataReceivedEventHandler consoleOutputCallback,
        string[]? extraParameters = null)
    {
        string? extra = extraParameters != null ? string.Join(" ", extraParameters) : null;
        BasicCommandProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = configurationService.AppConfig.SelectedEnvironment!.PythonPath,
                Arguments = $"{arguments} {extra}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        BasicCommandProcess.OutputDataReceived += consoleOutputCallback;
        BasicCommandProcess.Start();
        BasicCommandProcess.BeginOutputReadLine();
        var error = ProcessErrorFilter(BasicCommandProcess.StandardError.ReadToEnd());
        BasicCommandProcess.WaitForExit();
        BasicCommandProcess.Close();
        BasicCommandProcess.Dispose();
        return new ActionResponse { Success = string.IsNullOrEmpty(error), Exception = ExceptionType.ProcessError, Message =
           error };
    }

    #region Basic Command

    public ActionResponse Install(string packageName, DataReceivedEventHandler consoleOutputCallback,
        string[]? extraParameters = null)
        => RaiseProcess(
            $"-m pip install \"{packageName}\" -i {configurationService.GetUrlFromPackageSourceType()} --retries 1 --timeout 6",
            consoleOutputCallback, extraParameters);
    
    public ActionResponse InstallByRequirements(string requirementsFilePath,
        DataReceivedEventHandler consoleOutputCallback)
        => RaiseProcess(
            $"-m pip install -r \"{requirementsFilePath}\" -i {configurationService.GetUrlFromPackageSourceType()} --retries 1 --timeout 6",
            consoleOutputCallback);

    public ActionResponse Download(string packageName, string downloadPath, DataReceivedEventHandler consoleOutputCallback, string[]? extraParameters = null)
        => RaiseProcess(
            $"-m pip download -d \"{downloadPath}\" \"{packageName}\" -i {configurationService.GetUrlFromPackageSourceType()} --retries 1 --timeout 6",
            consoleOutputCallback, extraParameters);

    public ActionResponse Update(string packageName, DataReceivedEventHandler consoleOutputCallback)
        => RaiseProcess(
            $"-m pip install --upgrade \"{packageName}\" -i {configurationService.GetUrlFromPackageSourceType()} --retries 1 --timeout 6",
            consoleOutputCallback);

    public ActionResponse Uninstall(string packageName, DataReceivedEventHandler consoleOutputCallback)
        => RaiseProcess(
            $"-m pip uninstall -y \"{packageName}\"", consoleOutputCallback);

    #endregion

    #region Package Version Validation

    [GeneratedRegex("[-_.]+", RegexOptions.IgnoreCase)]
    private static partial Regex PackageNameNormalizerRegex();

    [GeneratedRegex("\\[[^\\]]*\\]", RegexOptions.IgnoreCase)]
    private static partial Regex PackageNameFilterRegex();

    [GeneratedRegex("^([A-Z0-9]|[A-Z0-9][A-Z0-9._-]*[A-Z0-9])$", RegexOptions.IgnoreCase)]
    private static partial Regex PackageNameVerificationRegex();

    #endregion 
}