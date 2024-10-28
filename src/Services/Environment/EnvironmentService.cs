using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using PipManager.Core.Configuration;
using PipManager.Core.Configuration.Models;
using PipManager.Core.Extensions;
using PipManager.Core.PyEnvironment.Helpers;
using PipManager.Core.PyPackage.Helpers;
using PipManager.Core.PyPackage.Models;
using PipManager.Windows.Models;
using PipManager.Windows.Services.Environment.Response;
using Serilog;
using Path = System.IO.Path;

namespace PipManager.Windows.Services.Environment;

public partial class EnvironmentService(HttpClient httpClient) : IEnvironmentService
{
    public bool CheckEnvironmentExists(EnvironmentModel environmentModel)
    {
        var environmentItems = Configuration.AppConfig!.Environments;
        return environmentItems.Any(item => item.PythonPath == environmentModel.PythonPath);
    }

    public ActionResponse CheckEnvironmentAvailable(EnvironmentModel environmentModel)
    {
        var verify = WindowsSpecified.GetEnvironmentByCommand(environmentModel.PythonPath, "-m pip -V");
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
                FileName = Configuration.AppConfig!.SelectedEnvironment!.PythonPath,
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

    public async Task<List<PackageDetailItem>?> GetLibraries()
    {
        if (Configuration.AppConfig?.SelectedEnvironment == null)
        {
            return null;
        }

        var packageDirInfo = new DirectoryInfo(Path.Combine(
            Path.GetDirectoryName(Configuration.AppConfig.SelectedEnvironment!.PythonPath)!,
            @"Lib\site-packages"));
        
        var packages = new ConcurrentQueue<PackageDetailItem>();
        var distInfoDirectories = packageDirInfo.GetDirectories()
                     .Where(path => path.Name.EndsWith(".dist-info"))
                     .ToList();

        var semaphore = new SemaphoreSlim(20);
        var ioTaskList = new List<Task>();

        foreach (var distInfoDirectory in distInfoDirectories)
        {
            await semaphore.WaitAsync();
            var task = Task.Run(async () =>
            {
                try
                {
                    await ProcessDistInfoDirectory(distInfoDirectory, packages);
                }
                finally
                {
                    semaphore.Release();
                }
            });
            ioTaskList.Add(task);
        }
        await Task.WhenAll(ioTaskList);
        Log.Information($"[EnvironmentService] Found {packages.Count} packages");
        
        return packages.OrderBy(x => x.Name).ToList();
    }

    private static async Task ProcessDistInfoDirectory(DirectoryInfo distInfoDirectory, ConcurrentQueue<PackageDetailItem> packages)
    {
        var distInfoDirectoryFullName = distInfoDirectory.FullName;
        var distInfoDirectoryName = distInfoDirectory.Name;

        // Basic
        var packageBasicInfo = distInfoDirectoryName[..^10].Split('-');
        var packageName = packageBasicInfo[0];
        var packageVersion = packageBasicInfo[1];

        if (packageName is "pip" or "~ip") return;

        // Metadata
        var metadataDict = new Dictionary<string, List<string>>();
        var classifiers = new Dictionary<string, List<string>>();
        
        var metadataFilePath = Path.Combine(distInfoDirectoryFullName, "METADATA");
        if (File.Exists(metadataFilePath))
        {
            string? currentKey = null;
            await foreach (var line in File.ReadLinesAsync(metadataFilePath))
            {
                if (string.IsNullOrWhiteSpace(line)) break;

                var parts = line.Split(": ", 2);
                if (parts.Length == 2)
                {
                    currentKey = parts[0].ToLower();
                    if (!metadataDict.TryGetValue(currentKey, out List<string>? value))
                    {
                        value = [];
                        metadataDict[currentKey] = value;
                    }

                    value.Add(parts[1]);
                }
                else if (currentKey != null)
                {
                    metadataDict[currentKey][^1] += "\n" + line;
                }
            }

            foreach (var item in metadataDict.GetValueOrDefault("classifier", []))
            {
                var keyValues = item.Split(" :: ");
                if (keyValues.Length < 2) continue;

                var key = keyValues[0];
                var value = string.Join(" :: ", keyValues[1..]);

                if (!classifiers.ContainsKey(key))
                {
                    classifiers[key] = [];
                }
                classifiers[key].Add(value);
            }
        }

        // Record
        var record = new List<string>();
        var recordFilePath = Path.Combine(distInfoDirectoryFullName, "RECORD");
        if (File.Exists(recordFilePath))
        {
            using var reader = new StreamReader(recordFilePath);
            while (await reader.ReadLineAsync() is { } line)
            {
                ReadOnlySpan<char> lineSpan = line.AsSpan();
                int slashIndex = lineSpan.IndexOf('/');
                ReadOnlySpan<char> dirIdentifierSpan = (slashIndex == -1) ? lineSpan : lineSpan[..slashIndex];

                if (dirIdentifierSpan.SequenceEqual(distInfoDirectoryName.AsSpan()) || dirIdentifierSpan[0] == '.')
                    continue;

                record.Add(line);
            }
        }
        
        // Extra
        var projectUrls = metadataDict.GetValueOrDefault("project-url", []);
        var projectUrlDictionary = projectUrls.Count != 0
            ? projectUrls.Select(url => new PackageDetailUrlModel
            {
                UrlIconType = url.Split(", ")[0].ToLower(),
                UrlType = url.Split(", ")[0],
                Url = url.Split(", ")[1]
            }).ToList()
            : [new PackageDetailUrlModel { UrlIconType = "", UrlType = "Unknown", Url = "" }];
        
        packages.Enqueue(new PackageDetailItem
        {
            Name = packageName,
            Version = packageVersion,
            DetailedVersion = PackageValidator.CheckVersion(packageVersion),
            Path = distInfoDirectoryFullName,
            DistInfoPath = distInfoDirectoryFullName,
            Summary = metadataDict.GetValueOrDefault("summary", [""]).First(),
            Author = metadataDict.GetValueOrDefault("author", []),
            AuthorEmail = metadataDict.GetValueOrDefault("author-email", [""]).First(),
            ProjectUrl = projectUrlDictionary,
            Classifier = classifiers,
            Metadata = metadataDict,
            Record = record
        });
    }

    public async Task<GetVersionsResponse> GetVersions(string packageName, CancellationToken cancellationToken, bool detectNonRelease = true)
    {
        try
        {
            var emptyArray = Array.Empty<string>();
            
            packageName = PackageNameFilterRegex().Replace(packageName, "");
            packageName = PackageNameNormalizerRegex().Replace(packageName, "-").ToLower();
            
            if (!PackageNameVerificationRegex().IsMatch(packageName))
                return new GetVersionsResponse { Status = 2, Versions = emptyArray };
            var responseUrl =
                $"{Configuration.AppConfig!.PackageSource.Source.GetPackageSourceUrl("pypi")}{packageName}/json";
            var responseMessage = await httpClient.GetAsync(responseUrl
                , cancellationToken);
            if (!responseMessage.IsSuccessStatusCode)
            {
                Log.Warning($"[EnvironmentService] Failed to fetch package {packageName}, StatusCode: {responseMessage.StatusCode}");
                return new GetVersionsResponse { Status = 1, Versions = emptyArray };
            }
            
            var response = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
            var pypiPackageInfo = JsonSerializer.Deserialize<PackageInfo>(response)
                ?.Releases?
                .Where(item => item.Value.Count != 0)
                .OrderBy(e => e.Value[0].UploadTime)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            if (!detectNonRelease && pypiPackageInfo != null)
            {
                pypiPackageInfo = pypiPackageInfo
                    .Where(item => PackageValidator.IsReleaseVersion(item.Key))
                    .ToDictionary();
            }
            
            if (pypiPackageInfo == null || pypiPackageInfo.Count == 0)
            {
                Log.Warning($"[EnvironmentService] {packageName} package not found");
                return new GetVersionsResponse { Status = 1, Versions = emptyArray };
            }

            Log.Information($"[EnvironmentService] Found {packageName}");
            return new GetVersionsResponse { Status = 0, Versions = pypiPackageInfo.Keys.ToArray() };
        }
        catch (TaskCanceledException)
        {
            return new GetVersionsResponse { Status = 1, Versions = Array.Empty<string>() };
        }
        catch (HttpRequestException e)
        {
            Log.Warning($"[EnvironmentService] Network error while fetching versions for {packageName}: {e.Message}");
            return new GetVersionsResponse { Status = 1, Versions = Array.Empty<string>() };
        }
        catch (JsonException e)
        {
            Log.Warning($"[EnvironmentService] JSON deserialization error for {packageName}: {e.Message}");
            return new GetVersionsResponse { Status = 1, Versions = Array.Empty<string>() };
        }
        catch (Exception e)
        {
            Log.Warning($"[EnvironmentService] Unexpected error while fetching versions for {packageName}: {e.Message}");
            return new GetVersionsResponse { Status = 1, Versions = Array.Empty<string>() };
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
                FileName = Configuration.AppConfig!.SelectedEnvironment!.PythonPath,
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
            $"-m pip install \"{packageName}\" -i {Configuration.AppConfig!.PackageSource.Source.GetPackageSourceUrl()} --retries 1 --timeout 6",
            consoleOutputCallback, extraParameters);
    
    public ActionResponse InstallByRequirements(string requirementsFilePath,
        DataReceivedEventHandler consoleOutputCallback)
        => RaiseProcess(
            $"-m pip install -r \"{requirementsFilePath}\" -i {Configuration.AppConfig!.PackageSource.Source.GetPackageSourceUrl()} --retries 1 --timeout 6",
            consoleOutputCallback);

    public ActionResponse Download(string packageName, string downloadPath, DataReceivedEventHandler consoleOutputCallback, string[]? extraParameters = null)
        => RaiseProcess(
            $"-m pip download -d \"{downloadPath}\" \"{packageName}\" -i {Configuration.AppConfig!.PackageSource.Source.GetPackageSourceUrl()} --retries 1 --timeout 6",
            consoleOutputCallback, extraParameters);

    public ActionResponse Update(string packageName, DataReceivedEventHandler consoleOutputCallback)
        => RaiseProcess(
            $"-m pip install --upgrade \"{packageName}\" -i {Configuration.AppConfig!.PackageSource.Source.GetPackageSourceUrl()} --retries 1 --timeout 6",
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