using PipManager.Controls;
using PipManager.Languages;
using PipManager.Models.AppConfigModels;
using PipManager.Models.Pages;
using PipManager.Services.Configuration;
using System.IO;
using Microsoft.Win32;
using PipManager.Services.Environment;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace PipManager.ViewModels.Pages.Environment;

public partial class AddEnvironmentViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;
    private readonly INavigationService _navigationService;
    private readonly IConfigurationService _configurationService;
    private readonly IEnvironmentService _environmentService;

    public AddEnvironmentViewModel(INavigationService navigationService, IConfigurationService configurationService, IEnvironmentService environmentService)
    {
        _navigationService = navigationService;
        _configurationService = configurationService;
        _environmentService = environmentService;
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        _navigationService.GetNavigationControl().BreadcrumbBar!.Visibility = Visibility.Collapsed;
        _ = RefreshPipList();
    }

    public void OnNavigatedFrom()
    {
        _navigationService.GetNavigationControl().BreadcrumbBar!.Visibility = Visibility.Visible;
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
    }

    #region ByWaysList

    [ObservableProperty]
    private List<AddEnvironmentByWaysListItem> _byWaysListItems = new()
    {
        new AddEnvironmentByWaysListItem(new SymbolIcon(SymbolRegular.Box24, filled: true), Lang.EnvironmentAdd_EnvironmentVariable_Title),
        new AddEnvironmentByWaysListItem(new SymbolIcon(SymbolRegular.WindowApps24, filled:true), Lang.EnvironmentAdd_PipCommand_Title),
        new AddEnvironmentByWaysListItem(new SymbolIcon(SymbolRegular.WindowText20, filled: true), Lang.EnvironmentAdd_PythonPath_Title)
    };

    [ObservableProperty] private int _byWaysListSelectedIndex;
    [ObservableProperty] private bool _byEnvironmentVariablesGridVisibility = true;
    [ObservableProperty] private bool _byPipCommandGridVisibility;
    [ObservableProperty] private bool _byPythonPathGridVisibility;

    [RelayCommand]
    private void ChangeWay()
    {
        ByEnvironmentVariablesGridVisibility = ByWaysListSelectedIndex == 0;
        ByPipCommandGridVisibility = ByWaysListSelectedIndex == 1;
        ByPythonPathGridVisibility = ByWaysListSelectedIndex == 2;
    }

    #endregion ByWaysList

    #region By Environment Variables

    [ObservableProperty]
    private List<EnvironmentItem> _environmentItems = new();

    [ObservableProperty]
    private EnvironmentItem? _environmentItemInList;

    [ObservableProperty]
    private bool _loading = true;

    [ObservableProperty]
    private bool _found = true;

    [RelayCommand]
    private async Task RefreshPipList()
    {
        await Task.Run(() =>
        {
            Loading = true;
            Found = false;
            EnvironmentItems = new List<EnvironmentItem>();
            var value = System.Environment.GetEnvironmentVariable("Path")!.Split(';');
            foreach (var item in value)
            {
                if (!item.Contains("Python") || item.Contains("Scripts") ||
                    !File.Exists(Path.Combine(item, "python.exe"))) continue;
                var environmentItem =
                    _environmentService.GetEnvironmentItemFromCommand(Path.Combine(item, "python.exe"), "-m pip -V");
                if (environmentItem == null) continue;
                EnvironmentItems.Add(environmentItem);
            }
        }).ContinueWith(_ => { Loading = false; Found = EnvironmentItems.Count == 0; });
    }

    #endregion By Environment Variables

    #region By Pip Command

    [ObservableProperty] private string _pipCommand = string.Empty;

    #endregion By Pip Command

    #region By Python Path

    [ObservableProperty] private string _pythonPath = string.Empty;

    [RelayCommand]
    private void BrowsePythonPath()
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "Python",
            FileName = "python.exe",
            DefaultExt = ".exe",
            Filter = "python|python.exe"
        };
        var result = openFileDialog.ShowDialog();
        if (result == true)
        {
            PythonPath = openFileDialog.FileName;
        }
    }
    #endregion By Python Path

    #region Add

    [RelayCommand]
    private async Task AddEnvironment(string parameter)
    {
        if (ByEnvironmentVariablesGridVisibility)
        {
            if (EnvironmentItemInList == null)
            {
                await MsgBox.Error(Lang.MsgBox_Message_EnvironmentNoSelection);
            }
            else
            {
                var result = _environmentService.CheckEnvironmentAvailable(EnvironmentItemInList);
                var alreadyExists = _environmentService.CheckEnvironmentExists(EnvironmentItemInList);
                if (result.Item1)
                {
                    if (alreadyExists)
                    {
                        await MsgBox.Error(Lang.MsgBox_Message_EnvironmentAlreadyExists);
                    }
                    else
                    {
                        _configurationService.AppConfig.EnvironmentItems.Add(EnvironmentItemInList);
                        _configurationService.Save();
                        _navigationService.GoBack();
                    }
                }
                else
                {
                    await MsgBox.Error(result.Item2);
                }
            }
        }
        else if (ByPipCommandGridVisibility)
        {
            var result = _environmentService.GetEnvironmentItemFromCommand(PipCommand, "-V");
            if (result != null)
            {
                var alreadyExists = _environmentService.CheckEnvironmentExists(result);
                if (alreadyExists)
                {
                    await MsgBox.Error(Lang.MsgBox_Message_EnvironmentAlreadyExists);
                }
                else
                {
                    _configurationService.AppConfig.EnvironmentItems.Add(result);
                    _configurationService.Save();
                    _navigationService.GoBack();
                }
            }
            else
            {
                await MsgBox.Error(Lang.MsgBox_Message_EnvironmentInvaild);
            }
        }
        else if (ByPythonPathGridVisibility)
        {
            var result = _environmentService.GetEnvironmentItemFromCommand(PythonPath, "-m pip -V");
            if (result != null)
            {
                var alreadyExists = _environmentService.CheckEnvironmentExists(result);
                if (alreadyExists)
                {
                    await MsgBox.Error(Lang.MsgBox_Message_EnvironmentAlreadyExists);
                }
                else
                {
                    _configurationService.AppConfig.EnvironmentItems.Add(result);
                    _configurationService.Save();
                    _navigationService.GoBack();
                }
            }
            else
            {
                await MsgBox.Error(Lang.MsgBox_Message_EnvironmentInvaild);
            }
        }
    }

    #endregion Add
}