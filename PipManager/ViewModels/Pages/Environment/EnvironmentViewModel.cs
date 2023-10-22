using PipManager.Controls;
using PipManager.Languages;
using PipManager.Models.AppConfigModels;
using PipManager.Services.Configuration;
using PipManager.Services.Environment;
using PipManager.ViewModels.Windows;
using PipManager.Views.Pages.Environment;
using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PipManager.Services.Action;
using Wpf.Ui.Controls;
using PipManager.Models.Pages;
using PipManager.Views.Pages.Action;

namespace PipManager.ViewModels.Pages.Environment;

public partial class EnvironmentViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;
    private readonly INavigationService _navigationService;
    private readonly IConfigurationService _configurationService;
    private readonly IEnvironmentService _environmentService;
    private readonly IActionService _actionService;

    public EnvironmentViewModel(INavigationService navigationService, IConfigurationService configurationService, IEnvironmentService environmentService, IActionService actionService)
    {
        _navigationService = navigationService;
        _configurationService = configurationService;
        _environmentService = environmentService;
        _actionService = actionService;
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
    private void DeleteEnvironment()
    {
        if (MsgBox.Warning(Lang.MsgBox_Message_EnvironmentDeletion, Lang.MsgBox_PrimaryButton_Action).Result !=
            MessageBoxResult.Primary) return;
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
        var result = _environmentService.CheckEnvironmentAvailable(CurrentEnvironment!);
        if (result.Item1)
        {
            Log.Information($"[Environment] Environment available ({CurrentEnvironment!.PipVersion} for {CurrentEnvironment.PythonVersion})");
            await MsgBox.Success(Lang.MsgBox_Message_EnvironmentCheckPassed);
        }
        else
        {
            Log.Error($"[Environment] Environment not available ({CurrentEnvironment!.PipVersion} for {CurrentEnvironment.PythonVersion})");

            if (MsgBox.Error(Lang.MsgBox_Message_EnvironmentCheckFailed, Lang.MsgBox_PrimaryButton_EnvironmentDeletion).Result == MessageBoxResult.Primary)
            {
                DeleteEnvironment();
            }
        }
    }
    [RelayCommand]
    private async Task CheckEnvironmentUpdate()
    {
        var latest = _environmentService.GetVersions("pip").Last().Trim();
        var current = _configurationService.AppConfig.CurrentEnvironment.PipVersion.Trim();
        if (latest != current)
        {
            Log.Information($"[Environment] Environment update available ({current} => {latest})");
            var message = $"{Lang.MsgBox_Message_FindUpdate}\n\n{current} => {latest}";
            if (MsgBox.Warning(message, Lang.MsgBox_PrimaryButton_Action, Lang.MsgBox_Title_Notice).Result == MessageBoxResult.Primary)
            {
                _actionService.ActionList.Add(new ActionListItem
                (
                    ActionType.Update,
                    Lang.Action_Operation_Update,
                    "pip",
                    progressIntermediate: false,
                    totalSubTaskNumber: 1
                ));
                _navigationService.Navigate(typeof(ActionPage));
                await _configurationService.RefreshAllEnvironmentVersions();
            }
        }
        else
        {
            await MsgBox.Success(Lang.MsgBox_Message_EnvironmentIsLatest);

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
        _navigationService.Navigate(typeof(AddEnvironmentPage));
    }

    #endregion Add Environment
}