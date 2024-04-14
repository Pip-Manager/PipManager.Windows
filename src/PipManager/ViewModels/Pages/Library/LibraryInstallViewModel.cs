using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using PipManager.Resources.Library;
using PipManager.Languages;
using PipManager.Models.Action;
using PipManager.Models.Pages;
using PipManager.Services.Action;
using PipManager.Services.Environment;
using PipManager.Services.Mask;
using PipManager.Services.Toast;
using PipManager.Views.Pages.Action;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Library;

public partial class LibraryInstallViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    public record InstalledPackagesMessage(List<LibraryListItem> InstalledPackages);

    private readonly IActionService _actionService;
    private readonly IMaskService _maskService;
    private readonly IContentDialogService _contentDialogService;
    private readonly IEnvironmentService _environmentService;
    private readonly IToastService _toastService;
    private readonly INavigationService _navigationService;

    public LibraryInstallViewModel(IActionService actionService, IMaskService maskService, IContentDialogService contentDialogService, IEnvironmentService environmentService, IToastService toastService, INavigationService navigationService)
    {
        _actionService = actionService;
        _maskService = maskService;
        _contentDialogService = contentDialogService;
        _environmentService = environmentService;
        _toastService = toastService;
        _installWheelDependencies = false;
        _navigationService = navigationService;
        WeakReferenceMessenger.Default.Register<InstalledPackagesMessage>(this, Receive);
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        InstallWheelDependencies = true;
    }

    public void OnNavigatedFrom()
    {
        PreInstallPackages.Clear();
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
    }

    private void Receive(object recipient, InstalledPackagesMessage message)
    {
        _installedPackages = message.InstalledPackages;
    }

    #region Install

    [ObservableProperty] private ObservableCollection<LibraryInstallPackageItem> _preInstallPackages = [];
    private List<LibraryListItem> _installedPackages = [];

    [RelayCommand]
    private async Task AddDefaultTask()
    {
        var custom = new InstallAddContentDialog(_contentDialogService.GetDialogHost());
        var packageName = await custom.ShowAsync();
        if (packageName == "")
        {
            return;
        }
        if (PreInstallPackages.Any(package => package.PackageName == packageName))
        {
            _toastService.Error(Lang.LibraryInstall_Add_AlreadyInList);
            return;
        }
        if (_installedPackages.Any(item => item.PackageName == packageName))
        {
            _toastService.Error(Lang.LibraryInstall_Add_AlreadyInstalled);
            return;
        }
        _maskService.Show(Lang.LibraryInstall_Add_Verifying);
        var packageVersions = await _environmentService.GetVersions(packageName);
        _maskService.Hide();
        switch (packageVersions.Status)
        {
            case 1:
                _toastService.Error(Lang.LibraryInstall_Add_PackageNotFound);
                return;

            case 2:
                _toastService.Error(Lang.LibraryInstall_Add_InvalidPackageName);
                return;

            default:
                PreInstallPackages.Add(new LibraryInstallPackageItem
                {
                    PackageName = packageName,
                    AvailableVersions = new List<string>(packageVersions.Versions!.Reverse())
                });
                break;
        }
    }

    [RelayCommand]
    private void AddDefaultToAction()
    {
        List<string> operationCommand = [];
        operationCommand.AddRange(PreInstallPackages.Select(preInstallPackage => preInstallPackage.VersionSpecified
            ? $"{preInstallPackage.PackageName}=={preInstallPackage.TargetVersion}"
            : $"{preInstallPackage.PackageName}"));
        _actionService.AddOperation(new ActionListItem
        (
            ActionType.Install,
            operationCommand.ToArray()
        ));
        PreInstallPackages.Clear();
    }

    [RelayCommand]
    private void DeleteDefaultTask(object? parameter)
    {
        var target = -1;
        for (int index = 0; index < PreInstallPackages.Count; index++)
        {
            if (ReferenceEquals(PreInstallPackages[index].PackageName, parameter))
            {
                target = index;
            }
        }

        if (target != -1)
        {
            PreInstallPackages.RemoveAt(target);
        }
    }

    #endregion Install

    #region Requirements Import

    [ObservableProperty] private string _requirements = "";

    [RelayCommand]
    private void AddRequirementsTask()
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "requirements.txt",
            FileName = "requirements.txt",
            DefaultExt = ".txt",
            Filter = "requirements|*.txt",
            RestoreDirectory = true
        };
        var result = openFileDialog.ShowDialog();
        if (result == true)
        {
            Requirements = File.ReadAllText(openFileDialog.FileName);
        }
    }

    [RelayCommand]
    private void AddRequirementsToAction()
    {
        _actionService.AddOperation(new ActionListItem
        (
            ActionType.InstallByRequirements,
            [Requirements],
            displayCommand: "requirements.txt"
        ));
        Requirements = "";
        _navigationService.Navigate(typeof(ActionPage));
    }

    #endregion Requirements Import

    #region Download Wheel File

    [ObservableProperty] private ObservableCollection<LibraryInstallPackageItem> _preDownloadPackages = [];
    [ObservableProperty] private string _downloadDistributionsFolderPath = "";
    [ObservableProperty] private bool _downloadDistributionsEnabled;
    [ObservableProperty] private bool _downloadWheelDependencies;

    [RelayCommand]
    private async Task DownloadDistributionsTask()
    {
        var custom = new InstallAddContentDialog(_contentDialogService.GetDialogHost());
        var packageName = await custom.ShowAsync();
        if (packageName == "")
        {
            return;
        }
        if (PreDownloadPackages.Any(package => package.PackageName == packageName))
        {
            _toastService.Error(Lang.LibraryInstall_Add_AlreadyInList);
            return;
        }
        _maskService.Show(Lang.LibraryInstall_Add_Verifying);
        var packageVersions = await _environmentService.GetVersions(packageName);
        _maskService.Hide();
        switch (packageVersions.Status)
        {
            case 1:
                _toastService.Error(Lang.LibraryInstall_Add_PackageNotFound);
                return;

            case 2:
                _toastService.Error(Lang.LibraryInstall_Add_InvalidPackageName);
                return;

            default:
                PreDownloadPackages.Add(new LibraryInstallPackageItem
                {
                    PackageName = packageName,
                    AvailableVersions = new List<string>(packageVersions.Versions!.Reverse())
                });
                DownloadDistributionsEnabled = DownloadDistributionsFolderPath.Length > 0;
                break;
        }
    }

    [RelayCommand]
    private void BrowseDownloadDistributionsFolderTask()
    {
        var openFolderDialog = new OpenFolderDialog
        {
            Title = "Download Folder (for wheel files)"
        };
        var result = openFolderDialog.ShowDialog();
        if (result == true)
        {
            DownloadDistributionsFolderPath = openFolderDialog.FolderName;
            DownloadDistributionsEnabled = PreDownloadPackages.Count > 0;
        }
    }

    [RelayCommand]
    private void DownloadDistributionsToAction()
    {
        List<string> operationCommand = [];
        operationCommand.AddRange(PreDownloadPackages.Select(preDownloadPackage => preDownloadPackage.VersionSpecified
            ? $"{preDownloadPackage.PackageName}=={preDownloadPackage.TargetVersion}"
            : $"{preDownloadPackage.PackageName}"));
        _actionService.AddOperation(new ActionListItem
        (
            ActionType.Download,
            operationCommand.ToArray(),
            path: DownloadDistributionsFolderPath,
            extraParameters: DownloadWheelDependencies ? null : ["--no-deps"]
        ));
        PreDownloadPackages.Clear();
        _navigationService.Navigate(typeof(ActionPage));
    }

    [RelayCommand]
    private void DeleteDownloadDistributionsTask(object? parameter)
    {
        var target = -1;
        for (int index = 0; index < PreDownloadPackages.Count; index++)
        {
            if (ReferenceEquals(PreDownloadPackages[index].PackageName, parameter))
            {
                target = index;
            }
        }

        if (target != -1)
        {
            PreDownloadPackages.RemoveAt(target);
        }
        DownloadDistributionsEnabled = PreDownloadPackages.Count > 0;
    }

    #endregion Download Wheel File

    #region Install via distributions
    [ObservableProperty] private ObservableCollection<LibraryInstallPackageItem> _preInstallDistributions = [];
    [ObservableProperty] private bool _installWheelDependencies;

    [RelayCommand]
    private async Task SelectDistributions()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Wheel Files|*.whl;*.tar.gz",
            Multiselect = true
        };
        if (openFileDialog.ShowDialog() != true)
        {
            return;
        }

        foreach (var fileName in openFileDialog.FileNames)
        {
            var packageName = "";
            var packageVersion = "";
            try
            {
                if (fileName.EndsWith(".whl"))
                {
                    await using var wheelFileStream = new FileStream(fileName, FileMode.Open);
                    using var wheelFileArchive = new ZipArchive(wheelFileStream, ZipArchiveMode.Read);
                    foreach (ZipArchiveEntry entry in wheelFileArchive.Entries)
                    {
                        if (!entry.FullName.Contains(".dist-info/METADATA") && !entry.FullName.Contains("PKG-INFO"))
                        {
                            continue;
                        }

                        using var streamReader = new StreamReader(entry.Open());
                        while (await streamReader.ReadLineAsync() is { } line)
                        {
                            if (line.StartsWith("Name: "))
                            {
                                packageName = line[6..];
                            }
                            else if (line.StartsWith("Version: "))
                            {
                                packageVersion = line[9..];
                                break;
                            }
                        }


                    }
                }
                else if (fileName.EndsWith(".tar.gz"))
                {
                    var inStream = File.OpenRead(fileName);
                    var gzipStream = new GZipInputStream(inStream);
                    var tarArchive = TarArchive.CreateInputTarArchive(gzipStream, Encoding.UTF8);
                    var randomizedDirectory = Path.Combine(AppInfo.CachesDir, $"tempTarGz-{Guid.NewGuid():N}");
                    tarArchive.ExtractContents(randomizedDirectory);
                    tarArchive.Close();
                    gzipStream.Close();
                    inStream.Close();
                    string targetDirectory = Directory.GetDirectories(randomizedDirectory)[0];

                    if (File.Exists(Path.Combine(targetDirectory, "PKG-INFO")))
                    {
                        using var streamReader = File.OpenText(Path.Combine(targetDirectory, "PKG-INFO"));
                        while (await streamReader.ReadLineAsync() is { } line)
                        {
                            if (line.StartsWith("Name: "))
                            {
                                packageName = line[6..];
                            }
                            else if (line.StartsWith("Version: "))
                            {
                                packageVersion = line[9..];
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e) when (e is IOException or InvalidDataException)
            {
                _toastService.Error(Lang.LibraryInstall_InstallDistributions_IOError);
                return;
            }

            if (packageName == "" || packageVersion == "")
            {
                _toastService.Error(Lang.LibraryInstall_InstallDistributions_InvalidFile);
                return;
            }

            if (PreInstallDistributions.Any(item => item.PackageName == packageName))
            {
                _toastService.Error(Lang.LibraryInstall_InstallDistributions_AlreadyExists);
                return;
            }
            
            PreInstallDistributions.Add(new LibraryInstallPackageItem
            {
                PackageName = packageName,
                TargetVersion = packageVersion,
                DistributionFilePath = fileName
            });
        }
    }
    
    [RelayCommand]
    private void DeleteInstallDistributions(object? parameter)
    {
        var target = -1;
        for (int index = 0; index < PreInstallDistributions.Count; index++)
        {
            if (ReferenceEquals(PreInstallDistributions[index].PackageName, parameter))
            {
                target = index;
            }
        }

        if (target != -1)
        {
            PreInstallDistributions.RemoveAt(target);
        }
    }

    [RelayCommand]
    private void InstallDistributionsToAction()
    {
        List<string> operationCommand = [];
        operationCommand.AddRange(PreInstallDistributions.Select(preInstallPackage => preInstallPackage.DistributionFilePath)!);
        _actionService.AddOperation(new ActionListItem
        (
            ActionType.Install,
            operationCommand.ToArray(),
            extraParameters: DownloadWheelDependencies ? null : ["--no-deps"]
        ));
        PreInstallDistributions.Clear();
    }

    #endregion
}