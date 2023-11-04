using PipManager.Controls;
using PipManager.Languages;
using PipManager.Services.Action;
using PipManager.Services.Mask;
using PipManager.ViewModels.Windows;
using System.ComponentModel;
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
        IMaskService maskPresenter,
        IActionService actionService
    )
    {
        ViewModel = viewModel;
        DataContext = this;

        SystemThemeWatcher.Watch(this);

        InitializeComponent();
        navigationService.SetNavigationControl(NavigationView);
        snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        maskPresenter.SetMaskPresenter(MaskPresenter);
        contentDialogService.SetContentPresenter(RootContentDialog);
        var runnerThread = new Thread(actionService.Runner)
        {
            IsBackground = true,
            Priority = ThreadPriority.AboveNormal
        };
        runnerThread.Start();

        NavigationView.SetServiceProvider(serviceProvider);
    }

    private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        var actionList = App.GetService<IActionService>().ActionList;
        if (actionList.Count > 0)
        {
            var uiMessageBox = new MessageBox
            {
                Title = Lang.ContentDialog_Title_Warning,
                Content = Lang.ContentDialog_Message_ActionStillRunning,
                PrimaryButtonText = Lang.ContentDialog_PrimaryButton_Action,
                CloseButtonText = Lang.ContentDialog_CloseButton_Cancel
            };

            var result = uiMessageBox.ShowDialogAsync();
            e.Cancel = result.Result != MessageBoxResult.Primary;
        }
        else
        {
            e.Cancel = false;
        }
    }
}