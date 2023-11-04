using PipManager.Controls;
using PipManager.Languages;
using PipManager.Models.AppConfigModels;
using PipManager.Models.Pages;
using PipManager.Services.Action;
using PipManager.Services.Configuration;
using PipManager.Services.Environment;
using PipManager.Services.Mask;
using PipManager.ViewModels.Windows;
using PipManager.Views.Pages.Action;
using PipManager.Views.Pages.Environment;
using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PipManager.Models.Action;
using PipManager.Services.Toast;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Environment;

public partial class EnvironmentViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;
    private readonly INavigationService _navigationService;
    private readonly IConfigurationService _configurationService;
    private readonly IEnvironmentService _environmentService;
    private readonly IActionService _actionService;
    private readonly IMaskService _maskService;
    private readonly IContentDialogService _contentDialogService;
    private readonly IToastService _toastService;

    public EnvironmentViewModel(INavigationService navigationService, IConfigurationService configurationService, IEnvironmentService environmentService, IActionService actionService, IMaskService maskService, IContentDialogService contentDialogService, IToastService toastService)
    {
        _navigationService = navigationService;
        _configurationService = configurationService;
        _environmentService = environmentService;
        _actionService = actionService;
        _maskService = maskService;
        _contentDialogService = contentDialogService;
        _toastService = toastService;
    }

    public async void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        await _configurationService.RefreshAllEnvironmentVersions();
        EnvironmentItems = new ObservableCollection<EnvironmentItem>(_configurationService.AppConfig.EnvironmentItems);
        var currentEnvironment = _configurationService.AppConfig.CurrentEnvironment;
        foreach (var environmentItem in EnvironmentItems)
        {
            if (currentEnvironment is not null && environmentItem.PythonPath == currentEnvironment.PythonPath)
            {
                CurrentEnvironment = environmentItem;
                var mainWindowViewModel = App.GetService<MainWindowViewModel>();
                mainWindowViewModel.ApplicationTitle = $"Pip Manager | {CurrentEnvironment.PipVersion} for {CurrentEnvironment.PythonVersion}";
                Log.Information($"[Environment] Current Environment changed: {CurrentEnvironment.PythonPath}");
            }
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
    private async Task DeleteEnvironmentAsync()
    {
        var result = await _contentDialogService.ShowSimpleDialogAsync(
            ContentDialogCreateOptions.Warning(Lang.ContentDialog_Message_EnvironmentDeletion,
                Lang.ContentDialog_PrimaryButton_Action));
        if (result != ContentDialogResult.Primary) return;
        Log.Information($"[Environment] Environment has been removed from list ({CurrentEnvironment!.PipVersion} for {CurrentEnvironment.PythonVersion})");
        EnvironmentItems.Remove(CurrentEnvironment!);
        CurrentEnvironment = null;
        _configurationService.AppConfig.CurrentEnvironment = null;
        _configurationService.AppConfig.EnvironmentItems = new List<EnvironmentItem>(EnvironmentItems);
        _configurationService.Save();
        var mainWindowViewModel = App.GetService<MainWindowViewModel>();
        mainWindowViewModel.ApplicationTitle = $"Pip Manager";
        EnvironmentSelected = false;
    }

    [RelayCommand]
    private async Task CheckEnvironment()
    {
        var environmentAvailable = _environmentService.CheckEnvironmentAvailable(CurrentEnvironment!);
        if (environmentAvailable.Item1)
        {
            Log.Information($"[Environment] Environment is available ({CurrentEnvironment!.PipVersion} for {CurrentEnvironment.PythonVersion})");
            _toastService.Info(Lang.ContentDialog_Message_EnvironmentCheckPassed);
        }
        else
        {
            Log.Error($"[Environment] Environment not available ({CurrentEnvironment!.PipVersion} for {CurrentEnvironment.PythonVersion})");
            var result = await _contentDialogService.ShowSimpleDialogAsync(
                ContentDialogCreateOptions.Error(Lang.ContentDialog_Message_EnvironmentCheckFailed,
                    Lang.ContentDialog_PrimaryButton_EnvironmentDeletion));
            if (result == ContentDialogResult.Primary)
            {
                await DeleteEnvironmentAsync();
            }
        }
    }

    [RelayCommand]
    private async Task CheckEnvironmentUpdate()
    {
        _maskService.Show(Lang.Environment_Operation_CheckEnvironmentUpdate);
        var latest = "";
        await Task.Run(async () =>
        {
            var versions = await _environmentService.GetVersions("pip");
            if (versions is not null)
            {
                latest = versions.Last();
            }
        });
        Task.WaitAll();
        _maskService.Hide();
        var current = _configurationService.AppConfig.CurrentEnvironment!.PipVersion!.Trim();
        if (latest != current && latest != string.Empty)
        {
            Log.Information($"[Environment] Environment update available ({current} => {latest})");
            var message = $"{Lang.ContentDialog_Message_FindUpdate}\n\n{current} => {latest}";
            var result = await _contentDialogService.ShowSimpleDialogAsync(ContentDialogCreateOptions.Notice(message));
            if (result == ContentDialogResult.Primary)
            {
                _actionService.AddOperation(new ActionListItem
                (
                    ActionType.Update,
                    "pip",
                    progressIntermediate: false,
                    totalSubTaskNumber: 1
                ));
                _navigationService.Navigate(typeof(ActionPage));
                await _configurationService.RefreshAllEnvironmentVersions();
            }
        }
        else if (latest == string.Empty)
        {
            _toastService.Error(Lang.ContentDialog_Message_NetworkError);
        }
        else
        {
            _toastService.Info(Lang.ContentDialog_Message_EnvironmentIsLatest);
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(CurrentEnvironment) && CurrentEnvironment is not null)
        {
            var mainWindowViewModel = App.GetService<MainWindowViewModel>();
            mainWindowViewModel.ApplicationTitle = $"Pip Manager | {CurrentEnvironment.PipVersion} for {CurrentEnvironment.PythonVersion}";
            _configurationService.AppConfig.CurrentEnvironment = CurrentEnvironment;
            _configurationService.Save();
            EnvironmentSelected = true;
            Log.Information($"[Environment] Environment changed ({CurrentEnvironment.PipVersion} for {CurrentEnvironment.PythonVersion})");
        }
    }

    #region Add Environment

    [ObservableProperty]
    private ObservableCollection<EnvironmentItem> _environmentItems = new();

    [RelayCommand]
    private void AddEnvironment()
    {
        _navigationService.Navigate(typeof(EnvironmentPage));
        _navigationService.NavigateWithHierarchy(typeof(AddEnvironmentPage));
    }

    #endregion Add Environment
}