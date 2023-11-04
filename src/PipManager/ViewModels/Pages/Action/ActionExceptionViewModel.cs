using PipManager.Models.Action;
using PipManager.Services.Action;
using PipManager.Views.Windows.Action;
using Serilog;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Action;

public partial class ActionExceptionViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [ObservableProperty]
    private ObservableCollection<ActionListItem> _exceptions;

    private readonly IActionService _actionService;

    public ActionExceptionViewModel(IActionService actionService)
    {
        _actionService = actionService;
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
    private void ShowException(object parameter)
    {
        var actionExceptionWindow = App.GetService<ActionExceptionWindow>();
        actionExceptionWindow.Initialize(parameter as ActionListItem);
        actionExceptionWindow.Show();
    }
}