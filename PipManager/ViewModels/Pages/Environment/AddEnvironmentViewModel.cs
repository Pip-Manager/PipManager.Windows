using System.Diagnostics;
using System.IO;
using PipManager.Models.AppConfigModels;
using Serilog;
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
    }

    public void OnNavigatedFrom()
    {
        _navigationService.GetNavigationControl().BreadcrumbBar!.Visibility = Visibility.Visible;
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
    }

    [ObservableProperty] private string _addEnvironmentByWay;

    [RelayCommand]
    private void OnChangeAddEnvironmentWay(string parameter)
    {
        AddEnvironmentByWay = parameter;
    }

    #region By Environment Variables

    [ObservableProperty]
    private List<EnvironmentItem> _environmentItems;

    [RelayCommand]
    private void RefreshPipList()
    {
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
                        pipVersion = sections[i-1];
                        pipDirStart = true;
                    }
                    else if (sections[i] == "(python")
                    {
                        pythonVersion = sections[i+1].Replace(")", "");
                        break;
                    }
                    else if(pipDirStart)
                    {
                        pipDir += sections[i] + ' ';
                    }
                }
                pipVersion = pipVersion.Trim();
                pythonVersion = pythonVersion.Trim();
                pipDir = pipDir.Trim();
                EnvironmentItems.Add(new EnvironmentItem("", pipVersion, pipDir, pythonVersion));
            }
        }
    }

    #endregion
}