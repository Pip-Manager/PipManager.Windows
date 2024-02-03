using Microsoft.Win32;
using PipManager.Languages;
using PipManager.Models.AppConfigModels;
using PipManager.Services.Configuration;
using PipManager.Services.Environment;
using PipManager.Services.Toast;
using Serilog;
using System.IO;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Environment;

public partial class AddEnvironmentViewModel(INavigationService navigationService, IConfigurationService configurationService, IEnvironmentService environmentService, IToastService toastService) : ObservableObject, INavigationAware
{
    private bool _isInitialized;
    private readonly INavigationService _navigationService = navigationService;
    private readonly IConfigurationService _configurationService = configurationService;
    private readonly IEnvironmentService _environmentService = environmentService;
    private readonly IToastService _toastService = toastService;

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        ByWay = 0;
        _ = RefreshPipList();
    }

    public void OnNavigatedFrom()
    {
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
        Log.Information("[AddEnvironment] Initialized");
    }

    #region ByWaysList

    [ObservableProperty] private int _byWay;

    [RelayCommand]
    private void ChangeWay()
    {
        Log.Information($"[AddEnvironment] Addition way changed to index:{ByWay}");
    }

    #endregion ByWaysList

    #region By Environment Variables

    [ObservableProperty]
    private List<EnvironmentItem> _environmentItems = [];

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
            EnvironmentItems = [];
            var value = System.Environment.GetEnvironmentVariable("Path")!.Split(';');
            foreach (var item in value)
            {
                if (!item.Contains("Python") || item.Contains("Scripts") ||
                    !File.Exists(Path.Combine(item, "python.exe"))) continue;
                var environmentItem =
                    _configurationService.GetEnvironmentItemFromCommand(Path.Combine(item, "python.exe"), "-m pip -V");
                if (environmentItem == null) continue;
                EnvironmentItems.Add(environmentItem);
            }
        }).ContinueWith(_ => { Loading = false; Found = EnvironmentItems.Count == 0; Log.Information($"[AddEnvironment] Pip list in environment variable refreshed"); });
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
        var initialDirectory = @"C:\";
        if (Directory.Exists(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), @"AppData\Local\Programs\Python")))
        {
            initialDirectory =
                Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile),
                    @"AppData\Local\Programs\Python");
        }
        var openFileDialog = new OpenFileDialog
        {
            Title = "Python",
            FileName = "python.exe",
            DefaultExt = ".exe",
            Filter = "python|python.exe",
            InitialDirectory = initialDirectory,
            RestoreDirectory = true
        };
        var result = openFileDialog.ShowDialog();
        if (result == true)
        {
            PythonPath = openFileDialog.FileName;
        }
        Log.Information($"[AddEnvironment] python.exe found in {PythonPath}");
    }

    #endregion By Python Path

    #region Add

    [RelayCommand]
    private void AddEnvironment(string parameter)
    {
        switch (ByWay)
        {
            case 0 when EnvironmentItemInList == null:
                _toastService.Error(Lang.ContentDialog_Message_EnvironmentNoSelection);
                break;

            case 0:
                {
                    var result = _environmentService.CheckEnvironmentAvailable(EnvironmentItemInList);
                    var alreadyExists = _environmentService.CheckEnvironmentExists(EnvironmentItemInList);
                    if (result.Success)
                    {
                        if (alreadyExists)
                        {
                            _toastService.Error(Lang.ContentDialog_Message_EnvironmentAlreadyExists);
                        }
                        else
                        {
                            _configurationService.AppConfig.CurrentEnvironment = EnvironmentItemInList;
                            _configurationService.AppConfig.EnvironmentItems.Add(EnvironmentItemInList);
                            _configurationService.Save();
                            Log.Information($"[AddEnvironment] Environment added ({EnvironmentItemInList.PipVersion} for {EnvironmentItemInList.PythonVersion})");
                            _navigationService.GoBack();
                        }
                    }
                    else
                    {
                        _toastService.Error(result.Message);
                    }

                    break;
                }
            case 1:
                {
                    var result = _configurationService.GetEnvironmentItemFromCommand(PipCommand, "-V");
                    if (result != null)
                    {
                        var alreadyExists = _environmentService.CheckEnvironmentExists(result);
                        if (alreadyExists)
                        {
                            _toastService.Error(Lang.ContentDialog_Message_EnvironmentAlreadyExists);
                        }
                        else
                        {
                            _configurationService.AppConfig.CurrentEnvironment = result;
                            _configurationService.AppConfig.EnvironmentItems.Add(result);
                            Log.Information($"[AddEnvironment] Environment added ({result.PipVersion} for {result.PythonVersion})");
                            _configurationService.Save();
                            _navigationService.GoBack();
                        }
                    }
                    else
                    {
                        _toastService.Error(Lang.ContentDialog_Message_EnvironmentInvaild);
                    }

                    break;
                }
            case 2:
                {
                    var result = _configurationService.GetEnvironmentItemFromCommand(PythonPath, "-m pip -V");
                    if (result != null)
                    {
                        var alreadyExists = _environmentService.CheckEnvironmentExists(result);
                        if (alreadyExists)
                        {
                            _toastService.Error(Lang.ContentDialog_Message_EnvironmentAlreadyExists);
                        }
                        else
                        {
                            _configurationService.AppConfig.CurrentEnvironment = result;
                            _configurationService.AppConfig.EnvironmentItems.Add(result);
                            Log.Information($"[AddEnvironment] Environment added ({result.PipVersion} for {result.PythonVersion})");
                            _configurationService.Save();
                            _navigationService.GoBack();
                        }
                    }
                    else
                    {
                        _toastService.Error(Lang.ContentDialog_Message_EnvironmentInvaild);
                    }

                    break;
                }
        }
    }

    #endregion Add
}