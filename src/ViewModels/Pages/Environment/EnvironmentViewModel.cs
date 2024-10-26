using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PipManager.Core.Configuration;
using PipManager.Core.Configuration.Models;
using PipManager.Windows.Controls;
using PipManager.Windows.Languages;
using PipManager.Windows.Models.Action;
using PipManager.Windows.Services.Action;
using PipManager.Windows.Services.Environment;
using PipManager.Windows.Services.Mask;
using PipManager.Windows.Services.Toast;
using PipManager.Windows.ViewModels.Windows;
using PipManager.Windows.Views.Pages.Action;
using PipManager.Windows.Views.Pages.Environment;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace PipManager.Windows.ViewModels.Pages.Environment;

public partial class EnvironmentViewModel(INavigationService navigationService,
        IEnvironmentService environmentService,
        IActionService actionService, IMaskService maskService, IContentDialogService contentDialogService,
        IToastService toastService)
    : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    private void InitializeViewModel()
    {
        _isInitialized = true;
        Log.Information("[Environment] Initialized");
    }

    [ObservableProperty]
    private EnvironmentModel? _currentEnvironment;

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
        Configuration.AppConfig!.SelectedEnvironment = null;
        Configuration.AppConfig.Environments = [..EnvironmentItems];
        Configuration.Save();
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
            var versions = await environmentService.GetVersions("pip", new CancellationToken(), Configuration.AppConfig!.PackageSource.AllowNonRelease);
            if (versions.Status == 0)
            {
                latest = versions.Versions!.Last();
            }
        });
        Task.WaitAll();
        maskService.Hide();
        var current = Configuration.AppConfig!.SelectedEnvironment!.PipVersion.Trim();
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
                    progressIntermediate: false
                ));
                navigationService.Navigate(typeof(ActionPage));
                Configuration.RefreshAllEnvironments();
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
            Configuration.AppConfig!.SelectedEnvironment = CurrentEnvironment;
            Configuration.Save();
            Log.Information($"[Environment] Environment changed ({CurrentEnvironment.PipVersion} for {CurrentEnvironment.PythonVersion})");
        }
    }

    #region Add Environment

    [ObservableProperty]
    private ObservableCollection<EnvironmentModel> _environmentItems = [];

    [RelayCommand]
    private void AddEnvironment()
    {
        navigationService.Navigate(typeof(EnvironmentPage));
        navigationService.NavigateWithHierarchy(typeof(AddEnvironmentPage));
    }

    #endregion Add Environment

    public Task OnNavigatedToAsync()
    {
        if (!_isInitialized)
            InitializeViewModel();
        
        Configuration.RefreshAllEnvironments();
        EnvironmentItems =
            new ObservableCollection<EnvironmentModel>(Configuration.AppConfig!.Environments);
        var currentEnvironment = Configuration.AppConfig.SelectedEnvironment;
        foreach (var environmentItem in EnvironmentItems)
        {
            if (currentEnvironment is null || environmentItem.PythonPath != currentEnvironment.PythonPath)
            {
                continue;
            }

            CurrentEnvironment = environmentItem;
        
            var mainWindowViewModel = App.GetService<MainWindowViewModel>();
            mainWindowViewModel.ApplicationTitle =
                $"Pip Manager | {CurrentEnvironment.PipVersion} for {CurrentEnvironment.PythonVersion}";
            Log.Information($"[Environment] Current Environment changed: {CurrentEnvironment.PythonPath}");
            break;
        }
        return Task.CompletedTask;
    }

    public Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }
}