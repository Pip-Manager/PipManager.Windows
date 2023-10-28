using PipManager.Services.Action;
using PipManager.Services.OverlayLoad;
using PipManager.ViewModels.Windows;
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
            IsBackground = true
        };
        runnerThread.Start();

        NavigationView.SetServiceProvider(serviceProvider);
    }
}