using PipManager.Services.Configuration;
using PipManager.Services.Environment;
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
        IContentDialogService contentDialogService,
        IConfigurationService configurationService,
        IEnvironmentService environmentService
    )
    {
        Wpf.Ui.Appearance.Watcher.Watch(this);

        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        navigationService.SetNavigationControl(NavigationView);
        snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        contentDialogService.SetContentPresenter(RootContentDialog);
        environmentService.Initialize(configurationService.AppConfig);

        NavigationView.SetServiceProvider(serviceProvider);
    }
}