using CommunityToolkit.Mvvm.Messaging;
using Serilog;
using System.Collections.ObjectModel;
using System.Diagnostics;
using PipManager.Core.Configuration;
using PipManager.Core.PyPackage.Models;
using PipManager.Windows.Languages;
using PipManager.Windows.Models.Action;
using PipManager.Windows.Models.Pages;
using PipManager.Windows.Resources.Library;
using PipManager.Windows.Services.Action;
using PipManager.Windows.Services.Environment;
using PipManager.Windows.Services.Mask;
using PipManager.Windows.Services.Overlay;
using PipManager.Windows.Services.Toast;
using PipManager.Windows.Views.Pages.Action;
using PipManager.Windows.Views.Pages.Environment;
using PipManager.Windows.Views.Pages.Library;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace PipManager.Windows.ViewModels.Pages.Library;

public partial class LibraryViewModel(
    INavigationService navigationService, IEnvironmentService environmentService,
    IOverlayService overlayService, IActionService actionService,
    IMaskService maskService, IToastService toastService,
    IContentDialogService contentDialogService)
    : ObservableObject, INavigationAware
{
    private List<PackageDetailItem>? _library = [];
    private bool _isInitialized;

    private void InitializeViewModel()
    {
        _isInitialized = true;
        Log.Information("[Library] Initialized");
    }

    #region Install Package

    [RelayCommand]
    private void InstallPackage()
    {
        navigationService.NavigateWithHierarchy(typeof(LibraryInstallPage));
        WeakReferenceMessenger.Default.Send(new LibraryInstallViewModel.InstalledPackagesMessage([.. LibraryList]));
    }

    #endregion Install Package

    #region Delete Package

    [RelayCommand]
    private async Task DeletePackageAsync()
    {
        var selected = LibraryList.Where(libraryListItem => libraryListItem.IsSelected).ToList();
        var custom = new DeletionWarningContentDialog(contentDialogService.GetDialogHost(), selected);
        var result = await custom.ShowAsync();
        var command = selected.Aggregate("", (current, item) => current + (item.PackageName + ' '));
        if (result != ContentDialogResult.Primary) return;
        actionService.AddOperation(new ActionListItem
        (
            ActionType.Uninstall,
            command.Trim().Split(' '),
            progressIntermediate: false
        ));
        navigationService.Navigate(typeof(ActionPage));
    }

    #endregion Delete Package

    #region Check Update

    [RelayCommand]
    private async Task CheckUpdate()
    {
        maskService.Show(Lang.Library_Operation_CheckUpdate);
        var msgList = new List<PackageUpdateItem>();
        var operationList = "";
        var ioTaskList = new List<Task>();
        var msgListLock = new object();
        var detectNonRelease = Configuration.AppConfig.PackageSource.AllowNonRelease;
        await Task.Run(() =>
        {
            var selected = LibraryList.Where(libraryListItem => libraryListItem.IsSelected).ToList();
            ioTaskList.AddRange(selected.Select(item => Task.Run(async () =>
            {
                var latest = await environmentService.GetVersions(item.PackageName.ToLower().Replace('_', '-'), new CancellationToken(), detectNonRelease);
                if (latest.Status != 0 || item.PackageVersion == latest.Versions!.Last()) return;
                lock (msgListLock)
                {
                    operationList += $"{item.PackageName}=={latest.Versions!.Last()} ";
                    msgList.Add(new PackageUpdateItem
                    {
                        PackageName = item.PackageName,
                        PackageVersion = item.PackageVersion,
                        NewVersion = latest.Versions!.Last()
                    });
                }
            })));
            Task.WaitAll([.. ioTaskList]);
        });
        maskService.Hide();
        if (msgList.Count == 0)
        {
            toastService.Info(Lang.ContentDialog_Message_PackageIsLatest);
        }
        else
        {
            overlayService.ShowPackageUpdateOverlay(msgList, () =>
            {
                actionService.AddOperation(new ActionListItem
                (
                    ActionType.Update,
                    operationList.Trim().Split(' '),
                    progressIntermediate: false
                ));
                navigationService.Navigate(typeof(ActionPage));
            });
        }
    }

    #endregion Check Update

    #region Details

    [RelayCommand]
    private void ToDetailPage(object parameter)
    {
        if (_library is null) return;
        navigationService.NavigateWithHierarchy(typeof(LibraryDetailPage));
        var current = _library.Where(libraryListItem => libraryListItem.Name == parameter as string).ToList()[0];
        WeakReferenceMessenger.Default.Send(new LibraryDetailViewModel.LibraryDetailMessage(current, _library));
        Log.Information($"[Library] Turn to detail page: {current.Name}");
    }

    #endregion Details
    
    [RelayCommand]
    private void NavigateToSelectEnvironment()
        => navigationService.Navigate(typeof(EnvironmentPage));

    [RelayCommand]
    private void NavigateToAddEnvironment()
        => navigationService.Navigate(typeof(AddEnvironmentPage));

    [ObservableProperty] private int _libraryListLength;
    [ObservableProperty] private ObservableCollection<PackageListItem> _libraryList = [];
    [ObservableProperty] private bool _environmentFoundVisible;
    [ObservableProperty] private double _refreshTimeUsage;
    
    private readonly Stopwatch _refreshStopwatch = new();

    [RelayCommand]
    private async Task RefreshLibrary()
    {
        _refreshStopwatch.Reset();
        _refreshStopwatch.Start();
        
        LibraryList = [];
        _library = [];
        LibraryListLength = 0;
        EnvironmentFoundVisible = true;
        if (Configuration.AppConfig.SelectedEnvironment == null)
        {
            EnvironmentFoundVisible = false;
            return;
        }

        try
        {
            _ = maskService.ShowAsync(Lang.MainWindow_NavigationContent_Library, 300);
            var fetchTask = environmentService.GetLibraries();
            _library = await fetchTask;
            maskService.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[Library] Error fetching libraries");
            maskService.Hide();
            RefreshTimeUsage = 0;
            return;
        }
        finally
        {
            _refreshStopwatch.Stop();
            RefreshTimeUsage = Math.Round(_refreshStopwatch.Elapsed.TotalMilliseconds / 1000.0, 3);
        }

        if (_library != null)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (var package in _library)
                {
                    LibraryList.Add(new PackageListItem
                    {
                        PackageName = package.Name!,
                        PackageVersion = package.Version!,
                        PackageDetailedVersion = package.DetailedVersion!,
                        PackageSummary = package.Summary!,
                        IsSelected = false
                    });
                }
            });
            
            LibraryListLength = _library.Count;
            Log.Information("[Library] Package list refreshed successfully");
            return;
        }
        RefreshTimeUsage = 0;
    }

    public async Task OnNavigatedToAsync()
    {
        if (!_isInitialized)
            InitializeViewModel();
        await RefreshLibrary();
    }

    public Task OnNavigatedFromAsync()
        => Task.CompletedTask;
}