using PipManager.Controls.Library;
using PipManager.Models.Action;
using PipManager.Models.Pages;
using PipManager.Services.Action;
using PipManager.Services.Environment;
using PipManager.Services.Mask;
using PipManager.Services.Toast;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Library;

public partial class LibraryInstallViewModel(IActionService actionService, IMaskService maskService, IContentDialogService contentDialogService, IEnvironmentService environmentService, IToastService toastService) : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [ObservableProperty] private ObservableCollection<LibraryInstallPackageItem> _preInstallPackages = [];

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

    [RelayCommand]
    private async Task AddTask()
    {
        var custom = new InstallAddContentDialog(contentDialogService.GetContentPresenter());
        var packageName = await custom.ShowAsync();
        if (PreInstallPackages.Any(package => package.PackageName == packageName))
        {
            toastService.Error("Package already in list");
            return;
        }
        maskService.Show("Verifying");
        var packageVersions = await environmentService.GetVersions(packageName);
        maskService.Hide();
        if (packageVersions is null)
        {
            toastService.Error("Package not found");
            return;
        }
        PreInstallPackages.Add(new LibraryInstallPackageItem
        {
            PackageName = packageName,
            AvailableVersions = new List<string>(packageVersions.Reverse())
        });
    }

    [RelayCommand]
    private void AddToAction()
    {
        List<string> operationCommand = [];
        operationCommand.AddRange(PreInstallPackages.Select(preInstallPackage => preInstallPackage.VersionSpecified
            ? $"{preInstallPackage.PackageName}=={preInstallPackage.TargetVersion}"
            : $"{preInstallPackage.PackageName}"));
        actionService.AddOperation(new ActionListItem
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