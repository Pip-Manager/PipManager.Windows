using PipManager.Models.AppConfigModels;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Primitives;
using PipManager.Models.Pages;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace PipManager.ViewModels.Pages.Environment;

public partial class AddEnvironmentViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;
    private readonly INavigationService _navigationService;

    public AddEnvironmentViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        _navigationService.GetNavigationControl().BreadcrumbBar!.Visibility = Visibility.Collapsed;
        RefreshPipList();
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

    [ObservableProperty] private List<AddEnvironmentByWaysListItem> _byWaysListItems = new()
    {
        new AddEnvironmentByWaysListItem(new SymbolIcon(SymbolRegular.Box24, filled: true), "Environment Variables"),
        new AddEnvironmentByWaysListItem(new SymbolIcon(SymbolRegular.WindowApps24, filled:true), "Pip Command"),
        new AddEnvironmentByWaysListItem(new SymbolIcon(SymbolRegular.WindowText20, filled: true), "Python Command")
    };
    [ObservableProperty] private int _byWaysListSelectedIndex;
    [ObservableProperty] private bool _byEnvironmentVariablesGridVisibility = true;
    [ObservableProperty] private bool _byPipCommandGridVisibility;
    [ObservableProperty] private bool _byPythonCommandGridVisibility;

    [RelayCommand]
    private void ChangeWay()
    {
        ByEnvironmentVariablesGridVisibility = ByWaysListSelectedIndex == 0;
        ByPipCommandGridVisibility = ByWaysListSelectedIndex == 1;
        ByPythonCommandGridVisibility = ByWaysListSelectedIndex == 2;
    }

    #endregion

    #region By Environment Variables

    [ObservableProperty]
    private List<EnvironmentItem> _environmentItems = new();

    [ObservableProperty]
    private bool _loading = true;

    [RelayCommand]
    private async Task RefreshPipList()
    {
        await Task.Run(() =>
        {
            Loading = true;
            EnvironmentItems = new List<EnvironmentItem>();
            var value = System.Environment.GetEnvironmentVariable("Path")!.Split(';');
            foreach (var item in value)
            {
                if (!item.Contains("Python") || item.Contains("Scripts") ||
                    !File.Exists(Path.Combine(item, "python.exe"))) continue;
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = $"{Path.Combine(item, "python.exe")}",
                        Arguments = "-m pip -V",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                while (!proc.StandardOutput.EndOfStream)
                {
                    var output = proc.StandardOutput.ReadLine();
                    if (string.IsNullOrWhiteSpace(output)) continue;
                    var sections = output.Split(' ');
                    var pipDirStart = false;
                    var pipVersion = "";
                    var pythonVersion = "";
                    var pipDir = "";
                    for (var i = 0; i < sections.Length; i++)
                    {
                        if (sections[i] == "from")
                        {
                            pipVersion = sections[i - 1];
                            pipDirStart = true;
                        }
                        else if (sections[i] == "(python")
                        {
                            pythonVersion = sections[i + 1].Replace(")", "");
                            break;
                        }
                        else if (pipDirStart)
                        {
                            pipDir += sections[i] + ' ';
                        }
                    }

                    EnvironmentItems.Add(new EnvironmentItem(pipVersion.Trim(), pipDir.Trim(), pythonVersion.Trim()));
                }
            }
        }).ContinueWith(task => { Loading = false; });


    }

    #endregion By Environment Variables

    [ObservableProperty] private string _addEnvironmentByWay = string.Empty;

    [RelayCommand]
    private void OnChangeAddEnvironmentWay(string parameter)
    {
        AddEnvironmentByWay = parameter;
    }
}