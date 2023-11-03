using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PipManager.Services;
using PipManager.Services.Action;
using PipManager.Services.Configuration;
using PipManager.Services.Environment;
using PipManager.Services.Mask;
using PipManager.ViewModels.Pages.Environment;
using PipManager.ViewModels.Pages.Library;
using PipManager.ViewModels.Pages.Tools;
using PipManager.ViewModels.Windows;
using PipManager.Views.Pages.About;
using PipManager.Views.Pages.Action;
using PipManager.Views.Pages.Environment;
using PipManager.Views.Pages.Library;
using PipManager.Views.Pages.Search;
using PipManager.Views.Pages.Settings;
using PipManager.Views.Pages.Tools;
using PipManager.Views.Windows;
using Serilog;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Wpf.Ui;
using AboutViewModel = PipManager.ViewModels.Pages.About.AboutViewModel;
using ActionViewModel = PipManager.ViewModels.Pages.Action.ActionViewModel;
using LibraryViewModel = PipManager.ViewModels.Pages.Library.LibraryViewModel;
using SearchViewModel = PipManager.ViewModels.Pages.Search.SearchViewModel;
using SettingsViewModel = PipManager.ViewModels.Pages.Settings.SettingsViewModel;

namespace PipManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private static readonly IHost Host = Microsoft.Extensions.Hosting.Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(c => { c.SetBasePath(AppContext.BaseDirectory); })
        .ConfigureServices((_, services) =>
        {
            services.AddHostedService<ApplicationHostService>();

            services.AddTransient(_ =>
            {
                var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All }) { DefaultRequestVersion = HttpVersion.Version20 };
                client.DefaultRequestHeaders.Add("User-Agent", $"PipManager/{AppInfo.AppVersion}");
                client.Timeout = TimeSpan.FromSeconds(6);
                return client;
            });

            // Window
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            // Services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<IMaskService, MaskService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddSingleton<IEnvironmentService, EnvironmentService>();
            services.AddSingleton<IActionService, ActionService>();
            services.AddSingleton<IThemeService, ThemeService>();

            // Pages
            services.AddSingleton<LibraryPage>();
            services.AddSingleton<LibraryViewModel>();
            services.AddSingleton<LibraryDetailPage>();
            services.AddSingleton<LibraryDetailViewModel>();

            services.AddSingleton<ActionPage>();
            services.AddSingleton<ActionViewModel>();
            services.AddSingleton<SearchPage>();
            services.AddSingleton<SearchViewModel>();
            services.AddSingleton<ToolsPage>();
            services.AddSingleton<ToolsViewModel>();

            services.AddSingleton<EnvironmentPage>();
            services.AddSingleton<EnvironmentViewModel>();
            services.AddSingleton<AddEnvironmentPage>();
            services.AddSingleton<AddEnvironmentViewModel>();

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

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool FreeConsole();

    private bool _showConsoleWindow;

    /// <summary>
    /// Occurs when the application is loading.
    /// </summary>
    private void OnStartup(object sender, StartupEventArgs e)
    {
        for (var i = 0; i != e.Args.Length; ++i)
        {
            if (e.Args[i] == "/console")
            {
                _showConsoleWindow = true;
            }
        }
        var appStarting = new AppStarting
        {
            ShowConsoleWindow = _showConsoleWindow
        };
        appStarting.StartLogging();
        appStarting.LoadLanguage();
        appStarting.LogDeletion();
        appStarting.CrushesDeletion();
        Host.Start();
    }

    /// <summary>
    /// Occurs when the application is closing.
    /// </summary>
    private async void OnExit(object sender, ExitEventArgs e)
    {
        if (_showConsoleWindow)
        {
            FreeConsole();
        }
        await Host.StopAsync();
        Host.Dispose();
        Log.Information("Logging ended");
        await Log.CloseAndFlushAsync();
    }

    /// <summary>
    /// Occurs when an exception is thrown by an application but not handled.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Error($"Exception: {e.Exception}");
        Directory.CreateDirectory(AppInfo.CrushesDir);
        var file = Path.Combine(AppInfo.CrushesDir, $"crash_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
        File.WriteAllText(file, e.Exception.ToString());
        var exceptionWindow = new ExceptionWindow();
        exceptionWindow.Initialize(e.Exception);
        exceptionWindow.ShowDialog();
    }
}