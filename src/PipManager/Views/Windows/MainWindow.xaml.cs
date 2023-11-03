using System.ComponentModel;
using Microsoft.Extensions.Hosting;
using PipManager.Controls;
using PipManager.Languages;
using PipManager.Services.Action;
using PipManager.Services.OverlayLoad;
using PipManager.ViewModels.Windows;
using Serilog;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace PipManager.Views.Windows;

public partial class MainWindow
{
    public MainWindowViewModel ViewModel { get; }

    public MainWindow(
        MainWindowViewModel viewModel,
        INavigationService navigationService,
        IServiceProvider serviceProvider,
        ISnackbarService snackbarService,
        IContentDialogService contentDialogService,
        IOverlayLoadService overlayLoadPresenter,
        IActionService actionService
    )
    {
        ViewModel = viewModel;
        DataContext = this;

        SystemThemeWatcher.Watch(this);

        InitializeComponent();
        navigationService.SetNavigationControl(NavigationView);
        snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        overlayLoadPresenter.SetOverlayLoadPresenter(OverlayLoadPresenter);
        contentDialogService.SetContentPresenter(RootContentDialog);
        var runnerThread = new Thread(actionService.Runner)
        {
            IsBackground = true, Priority = ThreadPriority.AboveNormal
        };
        runnerThread.Start();

        NavigationView.SetServiceProvider(serviceProvider);
    }

    private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        var actionList = App.GetService<IActionService>().ActionList;
        if (actionList.Count > 0)
        {
            e.Cancel =
               MsgBox.Warning(Lang.MsgBox_Message_ActionStillRunning, Lang.MsgBox_PrimaryButton_Action).Result != MessageBoxResult.Primary;
        }
        else
        {
            e.Cancel = false;
        }
    }
}