﻿using PipManager.Windows.Models.Action;
using PipManager.Windows.Services.Action;
using Serilog;
using Wpf.Ui.Controls;

namespace PipManager.Windows.ViewModels.Pages.Lab;

public partial class LabViewModel(IActionService actionService)
    : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [RelayCommand]
    private void ActionTest()
    {
        actionService.AddOperation(new ActionListItem
        (
            ActionType.Install,
            ["pytorch"],
            progressIntermediate: false
        ));
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