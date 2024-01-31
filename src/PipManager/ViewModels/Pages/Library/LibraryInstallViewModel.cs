using PipManager.Controls.Library;
using PipManager.Models.Action;
using PipManager.Models.Pages;
using PipManager.Services.Action;
using PipManager.Services.Environment;
using PipManager.Services.Mask;
using PipManager.Services.Toast;
using System.Collections.ObjectModel;
using PipManager.Languages;
using PipManager.Models.Package;
using Wpf.Ui;
using Wpf.Ui.Controls;
using CommunityToolkit.Mvvm.Messaging;
using static PipManager.ViewModels.Pages.Library.LibraryDetailViewModel;

namespace PipManager.ViewModels.Pages.Library;

public partial class LibraryInstallViewModel: ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [ObservableProperty] private ObservableCollection<LibraryInstallPackageItem> _preInstallPackages = [];
    private List<LibraryListItem> _installedPackages = [];
    public record InstalledPackagesMessage(List<LibraryListItem> InstalledPackages);

    private readonly IActionService _actionService;
    private readonly IMaskService _maskService;
    private readonly IContentDialogService _contentDialogService;
    private readonly IEnvironmentService _environmentService;
    private readonly IToastService _toastService;

    public LibraryInstallViewModel(IActionService actionService, IMaskService maskService, IContentDialogService contentDialogService, IEnvironmentService environmentService, IToastService toastService)
    {
        _actionService = actionService;
        _maskService = maskService;
        _contentDialogService = contentDialogService;
        _environmentService = environmentService;
        _toastService = toastService;
        WeakReferenceMessenger.Default.Register<InstalledPackagesMessage>(this, Receive);
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    public void OnNavigatedFrom()
    {
        PreInstallPackages.Clear();
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
    }

    public void Receive(object recipient, InstalledPackagesMessage message)
    {
        _installedPackages = message.InstalledPackages;
    }

    [RelayCommand]
    private async Task AddTask()
    {
        var custom = new InstallAddContentDialog(_contentDialogService.GetContentPresenter());
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
    private void AddToAction()
    {
        List<string> operationCommand = [];
        operationCommand.AddRange(PreInstallPackages.Select(preInstallPackage => preInstallPackage.VersionSpecified
            ? $"{preInstallPackage.PackageName}=={preInstallPackage.TargetVersion}"
            : $"{preInstallPackage.PackageName}"));
        _actionService.AddOperation(new ActionListItem
        (
            ActionType.Install,
            string.Join(' ', operationCommand),
            progressIntermediate: false,
            totalSubTaskNumber: operationCommand.Count
        ));
        PreInstallPackages.Clear();
    }

    [RelayCommand]
    private void DeleteTask(object? parameter)
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
}