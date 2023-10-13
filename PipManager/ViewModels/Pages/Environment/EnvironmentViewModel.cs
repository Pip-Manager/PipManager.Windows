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
using Wpf.Ui.Controls;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;

namespace PipManager.ViewModels.Pages.Environment;

public partial class EnvironmentViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;
    private readonly INavigationService _navigationService;
    private readonly IConfigurationService _configurationService;
    private readonly IEnvironmentService _environmentService;

    public EnvironmentViewModel(INavigationService navigationService, IConfigurationService configurationService, IEnvironmentService environmentService)
    {
        _navigationService = navigationService;
        _configurationService = configurationService;
        _environmentService = environmentService;
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        EnvironmentItems = new ObservableCollection<EnvironmentItem>(_configurationService.AppConfig.EnvironmentItems);
        var currentEnvironment = _configurationService.AppConfig.CurrentEnvironment;
        foreach (var environmentItem in EnvironmentItems)
        {
            if (currentEnvironment is not null && environmentItem.PythonPath == currentEnvironment.PythonPath)
            {
                CurrentEnvironment = environmentItem;
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