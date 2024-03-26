using PipManager.Languages;
using PipManager.Models.Action;
using PipManager.Services.Action;
using PipManager.Services.Toast;
using Serilog;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Web;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Action;

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

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        UpdateActionExceptionList();
    }

    public void OnNavigatedFrom()
    {
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
        Log.Information("[Action][Exceptions] Initialized");
    }

    public void UpdateActionExceptionList()
    {
        Exceptions = new ObservableCollection<ActionListItem>(_actionService.ExceptionList);
    }

    [RelayCommand]
    private static void ExceptionBingSearch(string? parameter)
    {
        if (parameter != null)
        {
            Process.Start(new ProcessStartInfo($"https://bing.com/search?q={HttpUtility.UrlEncode(parameter)}") { UseShellExecute = true });
        }
    }

    [RelayCommand]
    private static void ExceptionGoogleSearch(string? parameter)
    {
        if (parameter != null)
        {
            Process.Start(new ProcessStartInfo($"https://www.google.com/search?q={HttpUtility.UrlEncode(parameter)}") { UseShellExecute = true });
        }
    }

    [RelayCommand]
    private void ExceptionCopyToClipboard(string? parameter)
    {
        if (parameter != null)
        {
            Clipboard.SetDataObject(parameter);
            _toastService.Success(Lang.ActionException_CopyToClipboardNotice);
        }
    }
}