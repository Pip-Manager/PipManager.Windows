using Newtonsoft.Json;
using PipManager.Helpers;
using PipManager.Models;
using PipManager.Models.AppConfigModels;
using PipManager.Models.Package;
using PipManager.Models.Pages;
using PipManager.Models.Pypi;
using PipManager.Services.Configuration;
using PipManager.Services.Environment.Response;
using Serilog;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using Python.Runtime;
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
            : new ActionResponse { Success = false, Exception = ExceptionType.EnvironmentBroken };
    }

    public ActionResponse PurgeEnvironmentCache(EnvironmentItem environmentItem)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = configurationService.AppConfig.CurrentEnvironment!.PythonPath,
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
        if (configurationService.AppConfig.CurrentEnvironment is null)
        {
            return null;
        }

        var packageDirInfo = new DirectoryInfo(Path.Combine(
            Path.GetDirectoryName(configurationService.AppConfig.CurrentEnvironment!.PythonPath)!,
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
                var metadataDict = new Dictionary<string, List<string>>();
                var lastValidKey = "";
                var lastValidPos = 0;
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
                        metadataDict[lastValidKey][lastValidPos] = string.Join(metadataDict[lastValidKey][lastValidPos], "\n", value);
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
                var projectUrl = metadataDict.GetValueOrDefault("project-url", []);
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
                    Summary = metadataDict.GetValueOrDefault("summary", [""])[0],
                    Author = metadataDict.GetValueOrDefault("author", []),
                    AuthorEmail = metadataDict.GetValueOrDefault("author-email", [""])[0],
                    ProjectUrl = projectUrlDictionary,
                    Classifier = classifiers,
                    Metadata = metadataDict,
                    Record = record
                });
            });
            ioTaskList.Add(task);
        }
        await Task.WhenAll([.. ioTaskList]);
        Log.Information($"[EnvironmentService] Found {packages.Count} packages");
        return [.. packages.OrderBy(x => x.Name)];
    }

    public ParseRequirementsResponse ParseRequirements(IEnumerable<string> requirements)
    {
        var parsedRequirements = new ParseRequirementsResponse { Success = true, Requirements = [] };
        Runtime.PythonDLL = configurationService.AppConfig.CurrentEnvironment!.PythonDllPath!;
        PythonEngine.Initialize();
        using (Py.GIL())
        {
            try
            {
                dynamic packagingModule = Py.Import("packaging.requirements");
                dynamic requirementClass = packagingModule.Requirement;
                foreach (var requirement in requirements)
                {
                    dynamic requirementParser = requirementClass(requirement);
                    parsedRequirements.Requirements.Add(new ParsedRequirement
                    {
                        Name = requirementParser.name.ToString(),
                        Specifier = requirementParser.specifier.ToString()
                    });
                }
            }
            catch (PythonException ex)
            {
                Log.Error(ex.Message);
            }
            
        }

        return parsedRequirements;
    }

    public async Task<GetVersionsResponse> GetVersions(string packageName)
    {
        try
        {
            packageName = PackageNameFilterRegex().Replace(packageName, "");
            packageName = PackageNameNormalizerRegex().Replace(packageName, "-").ToLower();
            if (!PackageNameVerificationRegex().IsMatch(packageName))
                return new GetVersionsResponse { Status = 2, Versions = [] };
            var responseMessage =
                await _httpClient.GetAsync($"{configurationService.GetUrlFromPackageSourceType("pypi")}{packageName}/json");
            var response = await responseMessage.Content.ReadAsStringAsync();

            var pypiPackageInfo = JsonConvert.DeserializeObject<PypiPackageInfo>(response)
                ?.Releases?
                .Where(item => item.Value.Count != 0).OrderBy(e => e.Value[0].UploadTime)
                .ThenBy(e => e.Value[0].UploadTime).ToDictionary(pair => pair.Key, pair => pair.Value);
            if (pypiPackageInfo == null || pypiPackageInfo.Count == 0)
            {
                Log.Warning($"[EnvironmentService] {packageName} package not found");
                return new GetVersionsResponse { Status = 1, Versions = [] };
            }
            Log.Information($"[EnvironmentService] Found {packageName}");
            return new GetVersionsResponse { Status = 0, Versions = pypiPackageInfo.Keys.ToArray() };
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
    
    private ActionResponse RaiseProcess(string arguments, DataReceivedEventHandler consoleOutputCallback,
        string[]? extraParameters = null)
    {
        string? extra = extraParameters != null ? string.Join(" ", extraParameters) : null;
        BasicCommandProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = configurationService.AppConfig.CurrentEnvironment!.PythonPath,
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
        var error = BasicCommandProcess.StandardError.ReadToEnd();
        BasicCommandProcess.WaitForExit();
        BasicCommandProcess.Close();
        BasicCommandProcess.Dispose();
        return new ActionResponse { Success = string.IsNullOrEmpty(error), Exception = ExceptionType.ProcessError, Message = error };
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