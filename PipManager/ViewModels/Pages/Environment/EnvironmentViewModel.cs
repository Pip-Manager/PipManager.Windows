using PipManager.Models.AppConfigModels;
using PipManager.Services.Configuration;
using PipManager.Views.Pages.Environment;
using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PipManager.Controls;
using PipManager.Languages;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;

namespace PipManager.ViewModels.Pages.Environment;

public partial class EnvironmentViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;
    private readonly INavigationService _navigationService;
    private readonly IConfigurationService _configurationService;

    public EnvironmentViewModel(INavigationService navigationService, IConfigurationService configurationService)
    {
        _navigationService = navigationService;
        _configurationService = configurationService;
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        EnvironmentItems = new ObservableCollection<EnvironmentItem>(_configurationService.AppConfig.EnvironmentItems);
        var currentEnvironment = _configurationService.AppConfig.CurrentEnvironment;
        foreach (var environmentItem in EnvironmentItems)
        {
            if (environmentItem.PipDir == currentEnvironment)
            {
                CurrentEnvironment = environmentItem;
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

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(DeleteEnvironmentCommand), nameof(CheckEnvironmentCommand))]
    private bool _environmentSelected;

    [RelayCommand]
    private void DeleteEnvironment()
    {
        if (MsgBox.Warning(Lang.MsgBox_Message_EnvironmentDeletion, Lang.MsgBox_PrimaryButton_Action).Result == MessageBoxResult.Primary)
        {
            EnvironmentItems.Remove(CurrentEnvironment);
            CurrentEnvironment = null;
            _configurationService.AppConfig.CurrentEnvironment = string.Empty;
            _configurationService.AppConfig.EnvironmentItems = new List<EnvironmentItem>(EnvironmentItems);
            _configurationService.Save();
            EnvironmentSelected = false;
        }
    }

    [RelayCommand]
    private void CheckEnvironment()
    {
        MessageBox.Show("c");

    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(CurrentEnvironment) && CurrentEnvironment is not null)
        {
            _configurationService.AppConfig.CurrentEnvironment = CurrentEnvironment.PipDir;
            _configurationService.Save();
            EnvironmentSelected = true;
        }
    }

    #region Add Environment

    [ObservableProperty]
    private ObservableCollection<EnvironmentItem> _environmentItems = new();

    [RelayCommand]
    private void AddEnvironment()
    {
        _navigationService.Navigate(typeof(AddEnvironmentPage));
    }

    #endregion Add Environment
}