using System.ComponentModel;
using PipManager.Windows.Languages;
using PipManager.Windows.Services.Action;
using PipManager.Windows.Services.Mask;
using PipManager.Windows.ViewModels.Windows;
using PipManager.Windows.Views.Pages.Library;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;

namespace PipManager.Windows.Views.Windows;

public partial class MainWindow
{
    public MainWindowViewModel ViewModel { get; }
    private readonly INavigationService _navigationService;

    public MainWindow(
        MainWindowViewModel viewModel,
        INavigationService navigationService,
        INavigationViewPageProvider pageService,
        IServiceProvider serviceProvider,
        IContentDialogService contentDialogService,
        IMaskService maskPresenter,
        IActionService actionService
    )
    {
        ViewModel = viewModel;
        _navigationService = navigationService;
        DataContext = this;

        SystemThemeWatcher.Watch(this);

        InitializeComponent();

        NavigationView.SetServiceProvider(serviceProvider);
        NavigationView.SetPageProviderService(pageService);
        navigationService.SetNavigationControl(NavigationView);
        maskPresenter.SetMaskPresenter(MaskPresenter);
        contentDialogService.SetDialogHost(RootContentDialog);
        var runnerThread = new Thread(actionService.Runner)
        {
            IsBackground = true,
            Priority = ThreadPriority.BelowNormal
        };
        runnerThread.Start();
    }

    private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        var actionList = App.GetService<IActionService>().ActionList;
        if (actionList.Count > 0)
        {
            var uiMessageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = Lang.ContentDialog_Title_Warning,
                Content = Lang.ContentDialog_Message_ActionStillRunning,
                PrimaryButtonText = Lang.ContentDialog_PrimaryButton_Action,
                CloseButtonText = Lang.ContentDialog_CloseButton_Cancel
            };

            var result = uiMessageBox.ShowDialogAsync();
            e.Cancel = result.Result != Wpf.Ui.Controls.MessageBoxResult.Primary;
        }
        else
        {
            e.Cancel = false;
        }
    }

    private void InstallMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        _navigationService.Navigate(typeof(LibraryPage));
        _navigationService.NavigateWithHierarchy(typeof(LibraryInstallPage));
    }
}