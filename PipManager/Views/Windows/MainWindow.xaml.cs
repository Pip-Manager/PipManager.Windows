using PipManager.ViewModels.Windows;

namespace PipManager.Views.Windows;

public partial class MainWindow
{
    public MainWindowViewModel ViewModel { get; }

    public MainWindow(
        MainWindowViewModel viewModel,
        INavigationService navigationService,
        IServiceProvider serviceProvider,
        ISnackbarService snackbarService,
        IContentDialogService contentDialogService
    )
    {
        Wpf.Ui.Appearance.Watcher.Watch(this);

        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        navigationService.SetNavigationControl(NavigationView);
        snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        contentDialogService.SetContentPresenter(RootContentDialog);

        NavigationView.SetServiceProvider(serviceProvider);
    }
}