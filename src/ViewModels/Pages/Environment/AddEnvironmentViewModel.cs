using Microsoft.Win32;
using Serilog;
using System.IO;
using PipManager.Core.Configuration;
using PipManager.Core.Configuration.Models;
using PipManager.Core.PyEnvironment;
using PipManager.Core.PyEnvironment.Helpers;
using PipManager.Windows.Languages;
using PipManager.Windows.Services.Environment;
using PipManager.Windows.Services.Toast;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;

namespace PipManager.Windows.ViewModels.Pages.Environment;

public partial class AddEnvironmentViewModel(INavigationService navigationService, IEnvironmentService environmentService, IToastService toastService) : ObservableObject, INavigationAware
{
    private bool _isInitialized;

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
        Log.Information($"[AddEnvironment] Addition way changed to index: {ByWay}");
    }

    #endregion ByWaysList

    #region By Environment Variables

    [ObservableProperty]
    private List<EnvironmentModel> _environmentItems = [];

    [ObservableProperty]
    private EnvironmentModel? _environmentItemInList;

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
                if (!File.Exists(Path.Combine(item, "python.exe")))
                    continue;
                var environmentModel =
                    Detector.ByPythonPath(Path.Combine(item, "python.exe"));
                if (environmentModel == null) continue;
                EnvironmentItems.Add(environmentModel);
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
                toastService.Error(Lang.ContentDialog_Message_EnvironmentNoSelection);
                break;

            case 0:
                {
                    var result = environmentService.CheckEnvironmentAvailable(EnvironmentItemInList);
                    var alreadyExists = environmentService.CheckEnvironmentExists(EnvironmentItemInList);
                    if (result.Success)
                    {
                        if (alreadyExists)
                        {
                            toastService.Error(Lang.ContentDialog_Message_EnvironmentAlreadyExists);
                        }
                        else
                        {
                            Configuration.AppConfig!.SelectedEnvironment = EnvironmentItemInList;
                            Configuration.AppConfig.Environments.Add(EnvironmentItemInList);
                            Configuration.Save();
                            Log.Information($"[AddEnvironment] Environment added ({EnvironmentItemInList.PipVersion} for {EnvironmentItemInList.PythonVersion})");
                            navigationService.GoBack();
                        }
                    }
                    else
                    {
                        toastService.Error(result.Message);
                    }

                    break;
                }
            case 1:
                {
                    var result = WindowsSpecified.GetEnvironmentByCommand(PipCommand, "-V");
                    if (result != null)
                    {
                        var alreadyExists = environmentService.CheckEnvironmentExists(result);
                        if (alreadyExists)
                        {
                            toastService.Error(Lang.ContentDialog_Message_EnvironmentAlreadyExists);
                        }
                        else
                        {
                            Configuration.AppConfig!.SelectedEnvironment = result;
                            Configuration.AppConfig.Environments.Add(result);
                            Log.Information($"[AddEnvironment] Environment added ({result.PipVersion} for {result.PythonVersion})");
                            Configuration.Save();
                            navigationService.GoBack();
                        }
                    }
                    else
                    {
                        toastService.Error(Lang.ContentDialog_Message_EnvironmentInvaild);
                    }

                    break;
                }
            case 2:
                {
                    var result = Detector.ByPythonPath(PythonPath);
                    if (result != null)
                    {
                        var alreadyExists = environmentService.CheckEnvironmentExists(result);
                        if (alreadyExists)
                        {
                            toastService.Error(Lang.ContentDialog_Message_EnvironmentAlreadyExists);
                        }
                        else
                        {
                            Configuration.AppConfig!.SelectedEnvironment = result;
                            Configuration.AppConfig.Environments.Add(result);
                            Log.Information($"[AddEnvironment] Environment added ({result.PipVersion} for {result.PythonVersion})");
                            Configuration.Save();
                            navigationService.GoBack();
                        }
                    }
                    else
                    {
                        toastService.Error(Lang.ContentDialog_Message_EnvironmentInvaild);
                    }

                    break;
                }
        }
    }

    #endregion Add

    public Task OnNavigatedToAsync()
    {
        if (!_isInitialized)
            InitializeViewModel();
        ByWay = 0;
        _ = RefreshPipList();
        return Task.CompletedTask;
    }

    public Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }
}