using Microsoft.Extensions.DependencyInjection;
using PipManager.Desktop.ViewModels;
using PipManager.Desktop.ViewModels.Pages;
using PipManager.Desktop.Views;
using PipManager.Desktop.Views.Pages;

namespace PipManager.Desktop.Services;

internal static class ServiceExtensions
{
    internal static IServiceCollection AddViews(this IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        
        // Top Pages
        services.AddSingleton<PackagePage>();
        
        // Bottom Pages
        services.AddSingleton<SettingsPage>();
        
        return services;
    }
    
    internal static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddSingleton<MainWindowViewModel>();
        
        // Top Pages
        services.AddSingleton<PageViewModelBase, PackageViewModel>();
        
        // Bottom Pages
        services.AddSingleton<PageViewModelBase, SettingsViewModel>();
        
        return services;
    }
    
    internal static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services;
    }
}