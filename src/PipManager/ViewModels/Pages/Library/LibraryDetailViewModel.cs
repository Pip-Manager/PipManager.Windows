using PipManager.Languages;
using PipManager.Models.Pages;
using PipManager.Services.Action;
using PipManager.Services.Configuration;
using PipManager.Services.Environment;
using PipManager.Views.Pages.Action;
using PipManager.Views.Pages.Environment;
using Serilog;
using System.Collections.ObjectModel;
using PipManager.Controls.Library;
using PipManager.Models;
using PipManager.Services.OverlayLoad;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;

namespace PipManager.ViewModels.Pages.Library;

public partial class LibraryDetailViewModel : ObservableObject, INavigationAware
{
    public record LibraryDetailMessage(PackageItem Package);
    private bool _isInitialized;
    private readonly INavigationService _navigationService;
    private readonly IEnvironmentService _environmentService;
    private readonly IOverlayLoadService _overlayLoadService;

    [ObservableProperty]
    private PackageItem? _package;

    public LibraryDetailViewModel(INavigationService navigationService, IEnvironmentService environmentService, IOverlayLoadService overlayLoadService)
    {
        _navigationService = navigationService;
        _environmentService = environmentService;
        _overlayLoadService = overlayLoadService;
        WeakReferenceMessenger.Default.Register<LibraryDetailMessage>(this, Receive);
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        _navigationService.GetNavigationControl().BreadcrumbBar!.Visibility = Visibility.Collapsed;
    }

    public void OnNavigatedFrom()
    {
        _navigationService.GetNavigationControl().BreadcrumbBar!.Visibility = Visibility.Visible;
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
    }

    public void Receive(object recipient, LibraryDetailMessage message)
    {
        Package = message.Package;
    }
}