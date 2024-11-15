using Microsoft.Extensions.DependencyInjection;
using PipManager.Core.Services.PackageSearchService;
using PipManager.Windows.Services.Action;
using PipManager.Windows.Services.Environment;
using PipManager.Windows.Services.Mask;
using PipManager.Windows.Services.Overlay;
using PipManager.Windows.Services.Page;
using PipManager.Windows.Services.Toast;
using PipManager.Windows.ViewModels.Pages.About;
using PipManager.Windows.ViewModels.Pages.Action;
using PipManager.Windows.ViewModels.Pages.Environment;
using PipManager.Windows.ViewModels.Pages.Library;
using PipManager.Windows.ViewModels.Pages.Overlay;
using PipManager.Windows.ViewModels.Pages.Search;
using PipManager.Windows.ViewModels.Pages.Settings;
using PipManager.Windows.Views.Pages.About;
using PipManager.Windows.Views.Pages.Action;
using PipManager.Windows.Views.Pages.Environment;
using PipManager.Windows.Views.Pages.Library;
using PipManager.Windows.Views.Pages.Overlay;
using PipManager.Windows.Views.Pages.Search;
using PipManager.Windows.Views.Pages.Settings;
using Wpf.Ui;
using Wpf.Ui.Abstractions;

namespace PipManager.Windows.Extensions;

public static class ServiceRegisterExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<INavigationViewPageProvider, PageService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ISnackbarService, SnackbarService>();
        services.AddSingleton<IMaskService, MaskService>();
        services.AddSingleton<IToastService, ToastService>();
        services.AddSingleton<IEnvironmentService, EnvironmentService>();
        services.AddSingleton<IActionService, ActionService>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<IContentDialogService, ContentDialogService>();
        services.AddSingleton<ITaskBarService, TaskBarService>();
        services.AddSingleton<IPackageSearchService, PackageSearchService>();
        services.AddSingleton<IOverlayService, OverlayService>();
    }

    public static void RegisterViews(this IServiceCollection services)
    {
        services.AddSingleton<LibraryPage>();
        services.AddSingleton<LibraryDetailPage>();
        services.AddSingleton<LibraryInstallPage>();
        services.AddSingleton<OverlayPage>();
        services.AddSingleton<ActionPage>();
        services.AddSingleton<ActionExceptionPage>();
        services.AddSingleton<SearchPage>();
        services.AddSingleton<SearchDetailPage>();
        services.AddSingleton<EnvironmentPage>();
        services.AddSingleton<AddEnvironmentPage>();
        services.AddSingleton<SettingsPage>();
        services.AddSingleton<AboutPage>();
    }

    public static void RegisterViewModels(this IServiceCollection services)
    {
        services.AddSingleton<LibraryViewModel>();
        services.AddSingleton<LibraryDetailViewModel>();
        services.AddSingleton<LibraryInstallViewModel>();
        services.AddSingleton<OverlayViewModel>();
        services.AddSingleton<ActionViewModel>();
        services.AddSingleton<ActionExceptionViewModel>();
        services.AddSingleton<SearchViewModel>();
        services.AddSingleton<SearchDetailViewModel>();
        services.AddSingleton<EnvironmentViewModel>();
        services.AddSingleton<AddEnvironmentViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<AboutViewModel>();
    }
}