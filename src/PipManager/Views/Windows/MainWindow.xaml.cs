using PipManager.Languages;
using PipManager.Services.Action;
using PipManager.Services.Mask;
using PipManager.ViewModels.Windows;
using PipManager.Views.Pages.Library;
using System.ComponentModel;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace PipManager.Views.Windows;

public partial class MainWindow
{
    public MainWindowViewModel ViewModel { get; }
    private readonly INavigationService _navigationService;

    public MainWindow(
        MainWindowViewModel viewModel,
        INavigationService navigationService,
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
        navigationService.SetNavigationControl(NavigationView);
        maskPresenter.SetMaskPresenter(MaskPresenter);
        contentDialogService.SetDialogHost(RootContentDialog);
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