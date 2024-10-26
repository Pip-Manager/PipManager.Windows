using Serilog;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using PipManager.Windows.Languages;
using PipManager.Windows.Models.Action;
using PipManager.Windows.Services.Action;
using PipManager.Windows.Services.Toast;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace PipManager.Windows.ViewModels.Pages.Action;

public partial class ActionExceptionViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [ObservableProperty]
    private ObservableCollection<ActionListItem> _exceptions;

    private readonly IActionService _actionService;
    private readonly IToastService _toastService;

    public ActionExceptionViewModel(IActionService actionService, IToastService toastService)
    {
        _actionService = actionService;
        _toastService = toastService;
        Exceptions = new ObservableCollection<ActionListItem>(_actionService.ExceptionList);
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
        Log.Information("[Action][Exceptions] Initialized");
    }

    private void UpdateActionExceptionList()
    {
        Exceptions = new ObservableCollection<ActionListItem>(_actionService.ExceptionList);
        Log.Information("[Action][Exceptions] Exception List updated ({Count} items)", Exceptions.Count);
    }

    private static string ExceptionFilter(string parameter)
    {
        var exceptionBuilder = new StringBuilder();
        using (StringReader reader = new (parameter))
        {
            while (reader.ReadLine() is { } line)
            {
                line = line.Trim();
                if (line.StartsWith("ERROR"))
                {
                    exceptionBuilder.Append(line).Append(' ');
                }
            }
        }

        return exceptionBuilder.ToString();
    }

    [RelayCommand]
    private static void ExceptionBingSearch(string? parameter)
    {
        if (parameter != null)
        {
            Process.Start(new ProcessStartInfo($"https://bing.com/search?q={HttpUtility.UrlEncode(ExceptionFilter(parameter))}") { UseShellExecute = true });
        }
    }

    [RelayCommand]
    private static void ExceptionGoogleSearch(string? parameter)
    {
        if (parameter != null)
        {
            Process.Start(new ProcessStartInfo($"https://www.google.com/search?q={HttpUtility.UrlEncode(ExceptionFilter(parameter))}") { UseShellExecute = true });
        }
    }

    [RelayCommand]
    private void ExceptionCopyToClipboard(string? parameter)
    {
        if (parameter == null)
        {
            return;
        }

        Clipboard.SetDataObject(ExceptionFilter(parameter));
        _toastService.Success(Lang.ActionException_CopyToClipboardNotice);
        Log.Information("[Action][Exceptions] Copied exception to clipboard");
    }

    public Task OnNavigatedToAsync()
    {
        if (!_isInitialized)
            InitializeViewModel();
        UpdateActionExceptionList();
        return Task.CompletedTask;
    }

    public Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }
}