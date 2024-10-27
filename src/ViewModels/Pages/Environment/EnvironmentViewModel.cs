using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

    #region Environment Operation

    [RelayCommand]
    private static void RevealEnvironmentFolder(EnvironmentModel environment)
        => Process.Start("explorer.exe", Path.GetDirectoryName(environment.PythonPath)!);
    
    [RelayCommand]
    private static void RevealEnvironmentConsole(EnvironmentModel environment)
        => Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/K {environment.PythonPath}",
            UseShellExecute = true
        });
    
     [RelayCommand]
    private async Task CheckEnvironmentUpdate(EnvironmentModel environment)
    {
        maskService.Show(Lang.Environment_Operation_CheckEnvironmentUpdate);
        var latest = "";
        try
        {
            var versions = await Task.Run(() => environmentService.GetVersions("pip", new CancellationToken(), Configuration.AppConfig!.PackageSource.AllowNonRelease));
            if (versions.Status == 0)
            {
                latest = versions.Versions!.Last();
            }
        }
        finally
        {
            maskService.Hide();
        }
        
        var current = environment.PipVersion.Trim();
        if (!string.IsNullOrEmpty(latest) && latest != current)
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
                    path: environment.PythonPath,
                    progressIntermediate: false
                ));
                navigationService.Navigate(typeof(ActionPage));
                Configuration.RefreshAllEnvironments();
            }
        }
        else if (string.IsNullOrEmpty(latest))
        {
            toastService.Error(Lang.ContentDialog_Message_NetworkError);
            Log.Error("[Environment] Network error while checking for updates (environment: {environment})", environment.PipVersion);
        }
        else
        {
            toastService.Info(Lang.ContentDialog_Message_EnvironmentIsLatest);
            Log.Information("[Environment] Environment is already up to date (environment: {environment})", environment.PipVersion);
        }
    }

    [RelayCommand]
    private async Task CheckEnvironment(EnvironmentModel environment)
    {
        maskService.Show(Lang.Environment_Operation_VerifyEnvironment);
        var environmentAvailable = await Task.Run(() => environmentService.CheckEnvironmentAvailable(environment));
        Task.WaitAll();
        maskService.Hide();

        if (environmentAvailable.Success)
        {
            Log.Information($"[Environment] Environment is available ({environment.PipVersion} for {environment.PythonVersion})");
            toastService.Info(Lang.ContentDialog_Message_EnvironmentCheckPassed);
        }
        else
        {
            Log.Error($"[Environment] Environment not available ({environment.PipVersion} for {environment.PythonVersion})");
            var result = await contentDialogService.ShowSimpleDialogAsync(
                ContentDialogCreateOptions.Error(Lang.ContentDialog_Message_EnvironmentCheckFailed,
                    Lang.ContentDialog_PrimaryButton_EnvironmentDeletion));
            if (result == ContentDialogResult.Primary)
            {
                await RemoveEnvironment(environment);
            }
        }
    }

    [RelayCommand]
    private void ClearCache(EnvironmentModel environment)
    {
        var result = environmentService.PurgeEnvironmentCache(environment);
        if (result.Success)
        {
            Log.Information($"[Environment] Cache cleared ({environment.PipVersion} for {environment.PythonVersion})");
            toastService.Info(string.Format(Lang.ContentDialog_Message_CacheCleared, result.Message));
        }
        else
        {
            Log.Error($"[Environment] Cache clear failed ({environment.PipVersion} for {environment.PythonVersion})");
            toastService.Error(Lang.ContentDialog_Message_CacheClearFailed);
        }
    }
    
    [RelayCommand]
    private async Task RemoveEnvironment(EnvironmentModel environment)
    {
        var result = await contentDialogService.ShowSimpleDialogAsync(
            ContentDialogCreateOptions.Warning(Lang.ContentDialog_Message_EnvironmentDeletion,
                Lang.ContentDialog_PrimaryButton_Proceed));
        if (result != ContentDialogResult.Primary) return;
        
        Log.Information($"[Environment] Environment has been removed from list ({environment.PipVersion} for {environment.PythonVersion})");
        
        EnvironmentItems.Remove(environment);
        if (CurrentEnvironment == null)
        {
            Configuration.AppConfig!.SelectedEnvironment = null;
        }
        Configuration.AppConfig!.Environments = [..EnvironmentItems];
        Configuration.Save();
        
        var mainWindowViewModel = App.GetService<MainWindowViewModel>();
        mainWindowViewModel.ApplicationTitle = "Pip Manager";
    }
    
    #endregion
    
    public async Task OnNavigatedToAsync()
    {
        if (!_isInitialized)
            InitializeViewModel();
        
        Configuration.RefreshAllEnvironments();
        EnvironmentItems =
            new ObservableCollection<EnvironmentModel>(Configuration.AppConfig!.Environments);
        EnvironmentItems = new ObservableCollection<EnvironmentModel>(Configuration.AppConfig.Environments);
        await Task.Delay(50);
        CurrentEnvironment = EnvironmentItems.FirstOrDefault(item => item.PythonPath == Configuration.AppConfig.SelectedEnvironment?.PythonPath);

        if (CurrentEnvironment != null)
        {
            var mainWindowViewModel = App.GetService<MainWindowViewModel>();
            mainWindowViewModel.ApplicationTitle = $"Pip Manager | {CurrentEnvironment.PipVersion} for {CurrentEnvironment.PythonVersion}";
            Log.Information($"[Environment] Current Environment set: {CurrentEnvironment.PythonPath}");
        }
    }

    public Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }
}