using CommunityToolkit.Mvvm.Messaging;
using PipManager.Controls.Library;
using PipManager.Languages;
using PipManager.Models;
using PipManager.Models.Pages;
using PipManager.Services.Action;
using PipManager.Services.Configuration;
using PipManager.Services.Environment;
using PipManager.Services.OverlayLoad;
using PipManager.Views.Pages.Action;
using PipManager.Views.Pages.Environment;
using PipManager.Views.Pages.Library;
using Serilog;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using static PipManager.ViewModels.Pages.Library.LibraryDetailViewModel;

namespace PipManager.ViewModels.Pages.Library;

public partial class LibraryViewModel : ObservableObject, INavigationAware
{
    private List<PackageItem>? _library = new();
    private bool _isInitialized;
    private readonly INavigationService _navigationService;
    private readonly IEnvironmentService _environmentService;
    private readonly IConfigurationService _configurationService;
    private readonly IActionService _actionService;
    private readonly IThemeService _themeService;
    private readonly IOverlayLoadService _overlayLoadService;

    public LibraryViewModel(INavigationService navigationService, IEnvironmentService environmentService,
        IConfigurationService configurationService, IActionService actionService, IThemeService themeService, IOverlayLoadService overlayLoadService)
    {
        _navigationService = navigationService;
        _environmentService = environmentService;
        _configurationService = configurationService;
        _actionService = actionService;
        _themeService = themeService;
        _overlayLoadService = overlayLoadService;

        _themeService.SetTheme(_configurationService.AppConfig.Personalization.Theme switch
        {
            "light" => ApplicationTheme.Light,
            "dark" => ApplicationTheme.Dark,
            _ => ApplicationTheme.Dark
        });
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        _ = RefreshLibrary();
    }

    public void OnNavigatedFrom()
    {
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
        Log.Information("[Library] Initialized");
    }

    #region Delete Package

    [RelayCommand]
    private Task DeletePackageAsync()
    {
        var selected = LibraryList.Where(libraryListItem => libraryListItem.IsSelected).ToList();
        var custom = new DeletionWarningMsgBox(selected);
        var command = selected.Aggregate("", (current, item) => current + (item.PackageName + ' '));
        if (custom.ShowAsync().Result != MessageBoxResult.Primary) return Task.CompletedTask;
        _actionService.ActionList.Add(new ActionListItem
        (
            ActionType.Uninstall,
            Lang.Action_Operation_Uninstall,
            command.Trim(),
            progressIntermediate: false,
            totalSubTaskNumber: selected.Count
        ));
        _navigationService.Navigate(typeof(ActionPage));

        return Task.CompletedTask;
    }

    #endregion Delete Package

    #region Check Update

    [RelayCommand]
    private async Task CheckUpdate()
    {
        _overlayLoadService.Show(Lang.Library_Operation_CheckUpdate);
        var msgList = new List<LibraryCheckUpdateMsgBoxContentListItem>();
        var ioTaskList = new List<Task>();
        var msgListLock = new object();
        await Task.Run(()=>
        {
            var selected = LibraryList.Where(libraryListItem => libraryListItem.IsSelected).ToList();
            foreach (var item in selected)
            {
                var task = Task.Run(() =>
                {
                    lock (msgListLock)
                    {
                        msgList.Add(new LibraryCheckUpdateMsgBoxContentListItem(item, _environmentService.GetVersions(item.PackageName).Last().Trim()));
                    }
                });
                ioTaskList.Add(task);
            }
            Task.WaitAll(ioTaskList.ToArray());
        });
        _overlayLoadService.Hide();
        var custom = new CheckUpdateMsgBox(msgList);
        if (custom.ShowAsync().Result != MessageBoxResult.Primary) return;
        return;
    }

    #endregion

    #region Details

    [RelayCommand]
    private void ToDetailPage(object parameter)
    {
        if (_library is null) return;
        _navigationService.Navigate(typeof(LibraryDetailPage));
        var current = _library.Where(libraryListItem => libraryListItem.Name == parameter as string).ToList()[0];
        WeakReferenceMessenger.Default.Send(new LibraryDetailMessage(current));
        Log.Information($"[Library] Turn to detail page: {current.Name}");
    }

    #endregion Details

    [ObservableProperty] private int _libraryListLength;
    [ObservableProperty] private ObservableCollection<LibraryListItem> _libraryList = new();

    [ObservableProperty] private bool _environmentFoundVisible;
    [ObservableProperty] private bool _listVisible;

    [RelayCommand]
    private void NavigateToAddEnvironment()
    {
        _navigationService.Navigate(typeof(EnvironmentPage));
        _navigationService.Navigate(typeof(AddEnvironmentPage));
    }

    [RelayCommand]
    private async Task RefreshLibrary()
    {
        LibraryList = new ObservableCollection<LibraryListItem>();
        EnvironmentFoundVisible = true;
        _overlayLoadService.Show(Lang.MainWindow_NavigationContent_Library);
        _library = new List<PackageItem>();
        if (_configurationService.AppConfig.CurrentEnvironment == null)
        {
            _overlayLoadService.Hide();
            EnvironmentFoundVisible = false;
            return;
        }
        await Task.Run(() =>
        {
            _library = _environmentService.GetLibraries();
        }).ContinueWith(_ =>
        {
            _overlayLoadService.Hide();
        });
        if (_library != null)
        {
            foreach (var package in _library)
            {
                LibraryList.Add(new LibraryListItem
                (
                    new SymbolIcon(SymbolRegular.Box24), package.Name, package.Version, package.Summary, false
                ));
            }
            LibraryListLength = _library.Count;
            Log.Information("[Library] Package list refreshed successfully");
        }
    }
}