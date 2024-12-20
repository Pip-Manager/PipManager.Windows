﻿using CommunityToolkit.Mvvm.Messaging;
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
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace PipManager.Windows.ViewModels.Pages.Library;

public partial class LibraryViewModel : ObservableObject, INavigationAware
{
    private List<PackageDetailItem>? _library = [];
    private bool _isInitialized;
    private readonly INavigationService _navigationService;
    private readonly IEnvironmentService _environmentService;
    private readonly IActionService _actionService;
    private readonly IMaskService _maskService;
    private readonly IToastService _toastService;
    private readonly IContentDialogService _contentDialogService;
    private readonly IOverlayService _overlayService;

    public LibraryViewModel(INavigationService navigationService, IEnvironmentService environmentService, IOverlayService overlayService,
        IActionService actionService, IThemeService themeService, IMaskService maskService, IToastService toastService, IContentDialogService contentDialogService)
    {
        _navigationService = navigationService;
        _environmentService = environmentService;
        _actionService = actionService;
        _maskService = maskService;
        _toastService = toastService;
        _contentDialogService = contentDialogService;
        _overlayService = overlayService;

        themeService.SetTheme(Configuration.AppConfig!.Personalization.Theme switch
        {
            "light" => ApplicationTheme.Light,
            "dark" => ApplicationTheme.Dark,
            _ => ApplicationTheme.Dark
        });
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
        Log.Information("[Library] Initialized");
    }

    #region Install Package

    [RelayCommand]
    private void InstallPackage()
    {
        _navigationService.NavigateWithHierarchy(typeof(LibraryInstallPage));
        WeakReferenceMessenger.Default.Send(new LibraryInstallViewModel.InstalledPackagesMessage([.. LibraryList]));
    }

    #endregion Install Package

    #region Delete Package

    [RelayCommand]
    private async Task DeletePackageAsync()
    {
        var selected = LibraryList.Where(libraryListItem => libraryListItem.IsSelected).ToList();
        var custom = new DeletionWarningContentDialog(_contentDialogService.GetDialogHost(), selected);
        var result = await custom.ShowAsync();
        var command = selected.Aggregate("", (current, item) => current + (item.PackageName + ' '));
        if (result != ContentDialogResult.Primary) return;
        _actionService.AddOperation(new ActionListItem
        (
            ActionType.Uninstall,
            command.Trim().Split(' '),
            progressIntermediate: false
        ));
        _navigationService.Navigate(typeof(ActionPage));
    }

    #endregion Delete Package

    #region Check Update

    [RelayCommand]
    private async Task CheckUpdate()
    {
        _maskService.Show(Lang.Library_Operation_CheckUpdate);
        var msgList = new List<PackageUpdateItem>();
        var operationList = "";
        var ioTaskList = new List<Task>();
        var msgListLock = new object();
        var detectNonRelease = Configuration.AppConfig!.PackageSource.AllowNonRelease;
        await Task.Run(() =>
        {
            var selected = LibraryList.Where(libraryListItem => libraryListItem.IsSelected).ToList();
            ioTaskList.AddRange(selected.Select(item => Task.Run(async () =>
            {
                var latest = await _environmentService.GetVersions(item.PackageName.ToLower().Replace('_', '-'), new CancellationToken(), detectNonRelease);
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
        _maskService.Hide();
        if (msgList.Count == 0)
        {
            _toastService.Info(Lang.ContentDialog_Message_PackageIsLatest);
        }
        else
        {
            _overlayService.ShowPackageUpdateOverlay(msgList, () =>
            {
                _actionService.AddOperation(new ActionListItem
                (
                    ActionType.Update,
                    operationList.Trim().Split(' '),
                    progressIntermediate: false
                ));
                _navigationService.Navigate(typeof(ActionPage));
            });
        }
    }

    #endregion Check Update

    #region Details

    [RelayCommand]
    private void ToDetailPage(object parameter)
    {
        if (_library is null) return;
        _navigationService.NavigateWithHierarchy(typeof(LibraryDetailPage));
        var current = _library.Where(libraryListItem => libraryListItem.Name == parameter as string).ToList()[0];
        WeakReferenceMessenger.Default.Send(new LibraryDetailViewModel.LibraryDetailMessage(current, _library));
        Log.Information($"[Library] Turn to detail page: {current.Name}");
    }

    #endregion Details

    [ObservableProperty] private int _libraryListLength;
    [ObservableProperty] private ObservableCollection<PackageListItem> _libraryList = [];

    [ObservableProperty] private bool _environmentFoundVisible;
    
    [RelayCommand]
    private void NavigateToSelectEnvironment()
    {
        _navigationService.Navigate(typeof(EnvironmentPage));
    }

    [RelayCommand]
    private void NavigateToAddEnvironment()
    {
        _navigationService.Navigate(typeof(EnvironmentPage));
        _navigationService.Navigate(typeof(AddEnvironmentPage));
    }

    [ObservableProperty] private double _refreshTimeUsage;
    
    private readonly Stopwatch _refreshStopwatch = new();

    [RelayCommand]
    private async Task RefreshLibrary()
    {
        _refreshStopwatch.Reset();
        _refreshStopwatch.Start();
        LibraryList = [];
        EnvironmentFoundVisible = true;
        _maskService.Show(Lang.MainWindow_NavigationContent_Library);
        _library = [];
        if (Configuration.AppConfig!.SelectedEnvironment == null)
        {
            _maskService.Hide();
            EnvironmentFoundVisible = false;
            return;
        }
        await Task.Run(async () =>
        {
            _library = await _environmentService.GetLibraries();
        }).ContinueWith(_ =>
        {
            _maskService.Hide();
        });
        if (_library != null)
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
            LibraryListLength = _library.Count;
            _refreshStopwatch.Stop();
            RefreshTimeUsage = Math.Round(_refreshStopwatch.Elapsed.TotalMilliseconds / 1000.0, 3);
            Log.Information("[Library] Package list refreshed successfully");
            return;
        }
        RefreshTimeUsage = 0;
    }

    public Task OnNavigatedToAsync()
    {
        if (!_isInitialized)
            InitializeViewModel();
        Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            await RefreshLibrary();
        });
        return Task.CompletedTask;
    }

    public Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }
}