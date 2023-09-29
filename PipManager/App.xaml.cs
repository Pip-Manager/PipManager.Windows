using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PipManager.Services;
using PipManager.ViewModels.Pages;
using PipManager.ViewModels.Windows;
using PipManager.Views.Pages;
using PipManager.Views.Windows;
using System.IO;
using System.Reflection;
using System.Windows.Threading;
using PipManager.Services.Configuration;
using PipManager.Views.Pages.About;
using PipManager.Views.Pages.Library;
using PipManager.Views.Pages.Search;
using PipManager.Views.Pages.Settings;
using AboutViewModel = PipManager.ViewModels.Pages.About.AboutViewModel;
using LibraryViewModel = PipManager.ViewModels.Pages.Library.LibraryViewModel;
using SearchViewModel = PipManager.ViewModels.Pages.Search.SearchViewModel;
using SettingsViewModel = PipManager.ViewModels.Pages.Settings.SettingsViewModel;

namespace PipManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    private static readonly IHost Host = Microsoft.Extensions.Hosting.Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory)!); })
        .ConfigureServices((_, services) =>
        {
            services.AddHostedService<ApplicationHostService>();

            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();
            services.AddSingleton<IConfigurationService, ConfigurationService>();

            services.AddSingleton<LibraryPage>();
            services.AddSingleton<LibraryViewModel>();
            services.AddSingleton<SearchPage>();
            services.AddSingleton<SearchViewModel>();
            services.AddSingleton<SettingsPage>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<AboutPage>();
            services.AddSingleton<AboutViewModel>();
        }).Build();

    /// <summary>
    /// Gets registered service.
    /// </summary>
    /// <typeparam name="T">Type of the service to get.</typeparam>
    /// <returns>Instance of the service or <see langword="null"/>.</returns>
    public static T GetService<T>()
        where T : class
    {
        return Host.Services.GetService(typeof(T)) as T ?? throw new InvalidOperationException("Service not found.");
    }

    /// <summary>
    /// Occurs when the application is loading.
    /// </summary>
    private void OnStartup(object sender, StartupEventArgs e)
    {
        Host.Start();
    }

    /// <summary>
    /// Occurs when the application is closing.
    /// </summary>
    private async void OnExit(object sender, ExitEventArgs e)
    {
        await Host.StopAsync();

        Host.Dispose();
    }

    /// <summary>
    /// Occurs when an exception is thrown by an application but not handled.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var folder = Path.Combine(Directory.GetCurrentDirectory(), "crashes");
        Directory.CreateDirectory(folder);
        var file = Path.Combine(folder, $"crash_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
        File.WriteAllText(file, e.Exception.ToString());
    }
}