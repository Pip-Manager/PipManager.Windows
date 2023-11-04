using PipManager.Models;
using Serilog;
using System.Windows.Media;
using PipManager.Languages;
using PipManager.Models.Action;
using PipManager.Services.Action;
using PipManager.Views.Pages.Action;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Lab;

public partial class LabViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;
    private readonly IActionService _actionService;
    private readonly INavigationService _navigationService;

    public LabViewModel(INavigationService navigationService, IActionService actionService)
    {
        _navigationService = navigationService;
        _actionService = actionService;
    }

    [RelayCommand]
    private void ActionTest()
    {
        _actionService.ActionList.Add(new ActionListItem
        (
            ActionType.Update,
            Lang.Action_Operation_Update,
            "114510==114514 114511==114514",
            progressIntermediate: false,
            totalSubTaskNumber: 2
        ));
        ///_navigationService.Navigate(typeof(ActionPage));
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    public void OnNavigatedFrom()
    {
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
        Log.Information("[Lab] Initialized");
    }
}