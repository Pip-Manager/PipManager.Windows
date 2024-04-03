using PipManager.Controls;
using PipManager.Languages;
using PipManager.Models.Action;
using PipManager.Models.AppConfigModels;
using PipManager.Services.Action;
using PipManager.Services.Configuration;
using PipManager.Services.Environment;
using PipManager.Services.Mask;
using PipManager.Services.Toast;
using PipManager.ViewModels.Windows;
using PipManager.Views.Pages.Action;
using PipManager.Views.Pages.Environment;
using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace PipManager.ViewModels.Pages.Environment;

public partial class EnvironmentViewModel(INavigationService navigationService,
        IConfigurationService configurationService, IEnvironmentService environmentService,
        IActionService actionService, IMaskService maskService, IContentDialogService contentDialogService,
        IToastService toastService)
    : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        configurationService.RefreshAllEnvironmentVersions();
        EnvironmentItems = new ObservableCollection<EnvironmentItem>(configurationService.AppConfig.EnvironmentItems);
        var currentEnvironment = configurationService.AppConfig.CurrentEnvironment;
        foreach (var environmentItem in EnvironmentItems)
        {
            if (currentEnvironment is null || environmentItem.PythonPath != currentEnvironment.PythonPath)
            {
                continue;
            }

            CurrentEnvironment = environmentItem;
            var mainWindowViewModel = App.GetService<MainWindowViewModel>();
            mainWindowViewModel.ApplicationTitle = $"Pip Manager | {CurrentEnvironment.PipVersion} for {CurrentEnvironment.PythonVersion}";
            Log.Information($"[Environment] Current Environment changed: {CurrentEnvironment.PythonPath}");
        }
    }

    public void OnNavigatedFrom()
    {
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
        Log.Information("[Environment] Initialized");
    }

    [ObservableProperty]
    private EnvironmentItem? _currentEnvironment;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteEnvironmentCommand), nameof(CheckEnvironmentCommand))]
    private bool _environmentSelected;

    [RelayCommand]
    private async Task DeleteEnvironment()
    {
        var result = await contentDialogService.ShowSimpleDialogAsync(
            ContentDialogCreateOptions.Warning(Lang.ContentDialog_Message_EnvironmentDeletion,
                Lang.ContentDialog_PrimaryButton_Action));
        if (result != ContentDialogResult.Primary) return;
        Log.Information($"[Environment] Environment has been removed from list ({CurrentEnvironment!.PipVersion} for {CurrentEnvironment.PythonVersion})");
        EnvironmentItems.Remove(CurrentEnvironment!);
        CurrentEnvironment = null;
        configurationService.AppConfig.CurrentEnvironment = null;
        configurationService.AppConfig.EnvironmentItems = [..EnvironmentItems];
        configurationService.Save();
        var mainWindowViewModel = App.GetService<MainWindowViewModel>();
        mainWindowViewModel.ApplicationTitle = "Pip Manager";
        EnvironmentSelected = false;
    }

    [RelayCommand]
    private async Task CheckEnvironment()
    {
        var environmentAvailable = environmentService.CheckEnvironmentAvailable(CurrentEnvironment!);
        if (environmentAvailable.Success)
        {
            Log.Information($"[Environment] Environment is available ({CurrentEnvironment!.PipVersion} for {CurrentEnvironment.PythonVersion})");
            toastService.Info(Lang.ContentDialog_Message_EnvironmentCheckPassed);
        }
        else
        {
            Log.Error($"[Environment] Environment not available ({CurrentEnvironment!.PipVersion} for {CurrentEnvironment.PythonVersion})");
            var result = await contentDialogService.ShowSimpleDialogAsync(
                ContentDialogCreateOptions.Error(Lang.ContentDialog_Message_EnvironmentCheckFailed,
                    Lang.ContentDialog_PrimaryButton_EnvironmentDeletion));
            if (result == ContentDialogResult.Primary)
            {
                await DeleteEnvironment();
            }
        }
    }

    [RelayCommand]
    private async Task CheckEnvironmentUpdate()
    {
        maskService.Show(Lang.Environment_Operation_CheckEnvironmentUpdate);
        var latest = "";
        await Task.Run(async () =>
        {
            var versions = await environmentService.GetVersions("pip");
            if (versions.Status == 0)
            {
                latest = versions.Versions!.Last();
            }
        });
        Task.WaitAll();
        maskService.Hide();
        var current = configurationService.AppConfig.CurrentEnvironment!.PipVersion!.Trim();
        if (latest != current && latest != string.Empty)
        {
            Log.Information($"[Environment] Environment update available ({current} => {latest})");
            var message = $"{Lang.ContentDialog_Message_FindUpdate}\n\n{Lang.EnvironmentCheckEnvironmentUpdate_CurrentVersion}{current}\n{Lang.EnvironmentCheckEnvironmentUpdate_LatestVersion}{latest}";
            var result = await contentDialogService.ShowSimpleDialogAsync(ContentDialogCreateOptions.Notice(message));
            if (result == ContentDialogResult.Primary)
            {
                actionService.AddOperation(new ActionListItem
                (
                    ActionType.Update,
                    ["pip"],
                    progressIntermediate: false,
                    totalSubTaskNumber: 1
                ));
                navigationService.Navigate(typeof(ActionPage));
                configurationService.RefreshAllEnvironmentVersions();
            }
        }
        else if (latest == string.Empty)
        {
            toastService.Error(Lang.ContentDialog_Message_NetworkError);
            Log.Error("[Environment] Network error while checking for updates (environment: {environment})", CurrentEnvironment!.PipVersion);
        }
        else
        {
            toastService.Info(Lang.ContentDialog_Message_EnvironmentIsLatest);
            Log.Information("[Environment] Environment is already up to date (environment: {environment})", CurrentEnvironment!.PipVersion);
        }
    }

    [RelayCommand]
    private void ClearCache()
    {
        var result = environmentService.PurgeEnvironmentCache(CurrentEnvironment!);
        if (result.Success)
        {
            Log.Information($"[Environment] Cache cleared ({CurrentEnvironment!.PipVersion} for {CurrentEnvironment.PythonVersion})");
            toastService.Info(string.Format(Lang.ContentDialog_Message_CacheCleared, result.Message));
        }
        else
        {
            Log.Error($"[Environment] Cache clear failed ({CurrentEnvironment!.PipVersion} for {CurrentEnvironment.PythonVersion})");
            toastService.Error(Lang.ContentDialog_Message_CacheClearFailed);
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(CurrentEnvironment) && CurrentEnvironment is not null)
        {
            var mainWindowViewModel = App.GetService<MainWindowViewModel>();
            mainWindowViewModel.ApplicationTitle = $"Pip Manager | {CurrentEnvironment.PipVersion} for {CurrentEnvironment.PythonVersion}";
            configurationService.AppConfig.CurrentEnvironment = CurrentEnvironment;
            configurationService.Save();
            EnvironmentSelected = true;
            Log.Information($"[Environment] Environment changed ({CurrentEnvironment.PipVersion} for {CurrentEnvironment.PythonVersion})");
        }
    }

    #region Add Environment

    [ObservableProperty]
    private ObservableCollection<EnvironmentItem> _environmentItems = [];

    [RelayCommand]
    private void AddEnvironment()
    {
        navigationService.Navigate(typeof(EnvironmentPage));
        navigationService.NavigateWithHierarchy(typeof(AddEnvironmentPage));
    }

    #endregion Add Environment
}